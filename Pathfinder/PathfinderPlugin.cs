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

        public static GameObject squallBodyPrefab;
        public static GameObject squallMasterPrefab;

        public static SkillDef javelinSkill;

        internal static DamageAPI.ModdedDamageType marking;
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

            marking = DamageAPI.ReserveDamageType();
            squallGun = DamageAPI.ReserveDamageType();
            squallMissile = DamageAPI.ReserveDamageType();

            //make bird
            new Content.Squall().Initialize();

            // survivor initialization
            new Modules.Survivors.Pathfinder().Initialize();

            // now make a content pack and add it- this part will change with the next update
            new Modules.ContentPacks().Initialize();

            Hook();
        }

        private void Hook()
        {
            // run hooks here, disabling one is as simple as commenting out the line
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if(self.body.HasBuff(Modules.Buffs.raptorMark) && !damageInfo.rejected)
            {
                CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                if (attackerBody.bodyIndex == BodyCatalog.FindBodyIndex("PathfinderBody"))
                {
                    if(!damageInfo.crit && damageInfo.damage >= (attackerBody.damage * 5f))
                    {
                        damageInfo.crit = true;
                        self.body.RemoveBuff(Modules.Buffs.raptorMark);
                    } else if (damageInfo.crit)
                    {
                        damageInfo.damage *= 2f;
                        self.body.RemoveBuff(Modules.Buffs.raptorMark);
                    }
                }
            }

            orig(self, damageInfo);

            if(damageInfo.HasModdedDamageType(marking) && !damageInfo.rejected && !self.body.HasBuff(Modules.Buffs.raptorMark))
            {
                if(NetworkServer.active) self.body.AddBuff(Modules.Buffs.raptorMark);
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
                    self.armor -= 20f;
                    self.moveSpeed *= 0.5f;
                }
            }
        }
    }
}