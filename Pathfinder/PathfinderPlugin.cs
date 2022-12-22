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
using EntityStates.Merc;

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

        public const string MODVERSION = "0.5.0";

        // a prefix for name tokens to prevent conflicts- please capitalize all name tokens for convention
        public const string DEVELOPER_PREFIX = "BOG";

        public static PathfinderPlugin instance;

        public static GameObject pathfinderBodyPrefab;
        public static GameObject squallBodyPrefab;
        //public static GameObject squallMasterPrefab;
        public static GameObject commandCrosshair;

        public static BodyIndex squallBodyIndex;
        public static BodyIndex teslaTrooperBodyIndex;

        public static SkillDef javelinSkill;

        //internal static DamageAPI.ModdedDamageType goForThroat;
        internal static DamageAPI.ModdedDamageType piercing;

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

            piercing = DamageAPI.ReserveDamageType();

            //make bird
            new Modules.NPC.Squall().Initialize();

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
            On.RoR2.BodyCatalog.SetBodyPrefabs += BodyCatalog_SetBodyPrefabs;
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            On.RoR2.PrimarySkillShurikenBehavior.OnSkillActivated += PrimarySkillShurikenBehavior_OnSkillActivated;
            On.RoR2.SkillLocator.ApplyAmmoPack += SkillLocator_ApplyAmmoPack;
            On.RoR2.CharacterBody.AddBuff_BuffIndex += CharacterBody_AddBuff_BuffIndex;
        }

        private void BodyCatalog_SetBodyPrefabs(On.RoR2.BodyCatalog.orig_SetBodyPrefabs orig, GameObject[] newBodyPrefabs)
        {
            orig(newBodyPrefabs);

            squallBodyIndex = BodyCatalog.FindBodyIndex(squallBodyPrefab);
            teslaTrooperBodyIndex = BodyCatalog.FindBodyIndex("TeslaTrooperBody");
            Log.Warning("Squall's body index is: " + squallBodyIndex);
        }

        private void CharacterBody_AddBuff_BuffIndex(On.RoR2.CharacterBody.orig_AddBuff_BuffIndex orig, CharacterBody self, BuffIndex buffType)
        {
            orig(self, buffType);

            if(buffType == BuffCatalog.FindBuffIndex("Charged") && self.bodyIndex == squallBodyIndex)
            {
                BatteryComponent batteryComponent = self.GetComponent<BatteryComponent>();
                if(batteryComponent)
                {
                    batteryComponent.Recharge(3f, true);
                }
            }
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
            if (!msg.attacker) return;

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
            if (self.body.bodyIndex == squallBodyIndex)
            {
                if(damageInfo.attacker)
                {
                    CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                    if (attackerBody)
                    {
                        if (attackerBody.bodyIndex == teslaTrooperBodyIndex)
                        {
                            if (attackerBody.teamComponent.teamIndex == self.body.teamComponent.teamIndex)
                            {
                                if (NetworkServer.active)
                                    self.body.AddBuff(BuffCatalog.FindBuffIndex("Charged"));
                            }
                        }
                    }
                }
            }

            if(damageInfo.HasModdedDamageType(piercing) && !damageInfo.rejected && damageInfo.attacker)
            {
                CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                if (attackerBody)
                {
                    float distance = Vector3.Distance(attackerBody.corePosition, damageInfo.position);
                    if (distance >= 11f)
                    {
                        damageInfo.damage *= 1.3f;
                        damageInfo.damageColorIndex = DamageColorIndex.WeakPoint;
                        if (self.body.armor > 0f)
                            damageInfo.damageType = DamageType.BypassArmor;

                        EffectManager.SimpleImpactEffect(Modules.Assets.thrustTipImpact, damageInfo.position, Vector3.zero, true);
                    }
                    else
                    {
                        EffectManager.SimpleImpactEffect(GroundLight.comboHitEffectPrefab, damageInfo.position, Vector3.zero, true);
                    }
                }
            }

            orig(self, damageInfo);

            /*
            if(damageInfo.HasModdedDamageType(goForThroat) && !damageInfo.rejected)
            {
                if (NetworkServer.active) self.body.AddTimedBuff(Modules.Buffs.armorShred, 7f);
            }
            */
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

                if (self.HasBuff(Modules.Buffs.rendingTalonMS))
                {
                    self.moveSpeed *= 1.2f;
                }

                if(self.bodyIndex == squallBodyIndex)
                {
                    BatteryComponent batteryComponent = self.GetComponent<BatteryComponent>();

                    if (batteryComponent)
                        batteryComponent.rechargeRate = Modules.Config.batteryRechargeRate.Value * self.attackSpeed;
                }
            }
        }
    }
}