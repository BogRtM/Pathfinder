using BepInEx;
using Pathfinder.Modules.Survivors;
using R2API.Utils;
using R2API;
using RoR2;
using RoR2.Skills;
using RoR2.UI;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.Networking;
using Pathfinder.Components;
using UnityEngine.UI;
using System;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace Pathfinder
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]
    [R2APISubmoduleDependency(new string[]
    {
        "PrefabAPI",
        "LanguageAPI",
        "SoundAPI",
        "UnlockableAPI",
        "DamageAPI",
        "DotAPI"
    })]

    public class PathfinderPlugin : BaseUnityPlugin
    {
        // if you don't change these you're giving permission to deprecate the mod-
        //  please change the names to your own stuff, thanks
        //   this shouldn't even have to be said
        public const string MODUID = "com.Bog.Pathfinder";
        public const string MODNAME = "Pathfinder";
        public const string MODVERSION = "0.1.0";

        // a prefix for name tokens to prevent conflicts- please capitalize all name tokens for convention
        public const string DEVELOPER_PREFIX = "BOG";

        public static PathfinderPlugin instance;

        public static GameObject pathfinderBodyPrefab;
        public static GameObject squallBodyPrefab;
        //public static GameObject squallMasterPrefab;
        public static GameObject commandCrosshair;

        public static SkillDef javelinSkill;

        internal static DamageAPI.ModdedDamageType shredding;
        internal static DamageAPI.ModdedDamageType squallGun;
        internal static DamageAPI.ModdedDamageType squallMissile;

        private void Awake()
        {
            instance = this;

            Log.Init(Logger);
            Modules.Assets.Initialize(); // load assets and read config
            Modules.Config.ReadConfig(this);
            Modules.States.RegisterStates(); // register states for networking
            Modules.Buffs.RegisterBuffs(); // add and register custom buffs/debuffs
            Modules.Projectiles.RegisterProjectiles(); // add and register custom projectiles
            Modules.Tokens.AddTokens(); // register name tokens
            Modules.ItemDisplays.PopulateDisplays(); // collect item display prefabs for use in our display rules

            shredding = DamageAPI.ReserveDamageType();
            squallGun = DamageAPI.ReserveDamageType();
            squallMissile = DamageAPI.ReserveDamageType();

            //make bird
            new Content.NPC.Squall().Initialize();

            // survivor initialization
            new Modules.Survivors.Pathfinder().Initialize();

            // now make a content pack and add it- this part will change with the next update
            new Modules.ContentPacks().Initialize();

            Subscriptions();
            Hook();
        }

        private void Subscriptions()
        {
            GlobalEventManager.onClientDamageNotified += GlobalEventManager_onClientDamageNotified;
        }

        private void Hook()
        {
            // run hooks here, disabling one is as simple as commenting out the line
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            On.RoR2.PrimarySkillShurikenBehavior.OnSkillActivated += PrimarySkillShurikenBehavior_OnSkillActivated;
            On.RoR2.SkillLocator.ApplyAmmoPack += SkillLocator_ApplyAmmoPack;
        }

        private void SkillLocator_ApplyAmmoPack(On.RoR2.SkillLocator.orig_ApplyAmmoPack orig, SkillLocator self)
        {
            orig(self);

            FalconerComponent falconerComponent = self.GetComponent<FalconerComponent>();

            if (!falconerComponent) return;

            if (falconerComponent.squallController)
            {
                if (NetworkServer.active)
                {
                    foreach(var i in falconerComponent.squallController.skillLocator.allSkills)
                    {
                        if (i.CanApplyAmmoPack()) i.ApplyAmmoPack();
                    }
                }
            }
        }

        private void PrimarySkillShurikenBehavior_OnSkillActivated(On.RoR2.PrimarySkillShurikenBehavior.orig_OnSkillActivated orig, PrimarySkillShurikenBehavior self, GenericSkill skill)
        {
            OverrideController overrideController = self.GetComponent<OverrideController>();

            if (overrideController)
            {
                if (overrideController.javelinReady || overrideController.inCommandMode) return;
            }

            orig(self, skill);
        }

        private void GlobalEventManager_onClientDamageNotified(DamageDealtMessage msg)
        {
            if (BodyCatalog.FindBodyIndex(msg.attacker) != BodyCatalog.FindBodyIndex(squallBodyPrefab)) return;

            SquallController squallController = msg.attacker.GetComponent<SquallController>();
            if (!squallController || !squallController.owner) return;

            if (!msg.victim || msg.isSilent)
            {
                return;
            }
            HealthComponent component = msg.victim.GetComponent<HealthComponent>();
            if (!component || component.dontShowHealthbar)
            {
                return;
            }
            TeamIndex objectTeam = TeamComponent.GetObjectTeam(component.gameObject);
            foreach (CombatHealthBarViewer combatHealthBarViewer in CombatHealthBarViewer.instancesList)
            {
                if (squallController.owner == combatHealthBarViewer.viewerBodyObject && combatHealthBarViewer.viewerBodyObject)
                {
                    combatHealthBarViewer.HandleDamage(component, objectTeam);
                }
            }
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            orig(self, damageInfo);

            if(damageInfo.HasModdedDamageType(shredding) && !damageInfo.rejected)
            {
                if (NetworkServer.active) self.body.AddTimedBuff(Modules.Buffs.armorShred, 7f);
            }
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);

            // a simple stat hook, adds armor after stats are recalculated
            if (self)
            {
                if (self.HasBuff(Modules.Buffs.electrocute))
                {
                    self.moveSpeed *= Modules.Config.electrocuteSlowAmount.Value;
                }

                if(self.HasBuff(Modules.Buffs.armorShred))
                {
                    self.armor -= (Modules.Config.specialArmorShred.Value * self.GetBuffCount(Modules.Buffs.armorShred));
                }

                BatteryComponent batteryComponent = self.GetComponent<BatteryComponent>();
                if (!batteryComponent) return;
                batteryComponent.rechargeRate = Modules.Config.batteryRechargeRate.Value * self.attackSpeed;
            }
        }
    }
}