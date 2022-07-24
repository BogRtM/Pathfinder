using RoR2;
using R2API;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System;

namespace Pathfinder.Modules
{
    public static class Buffs
    {
        internal static BuffDef electrocute;
        internal static BuffDef armorShred;

        internal static DotController.DotIndex electrocuteDoT;

        internal static void RegisterBuffs()
        {
            BuffDef teslaField = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/ShockNearby/bdTeslaField.asset").WaitForCompletion();
            BuffDef pulverized = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/ArmorReductionOnHit/bdPulverized.asset").WaitForCompletion();

            electrocute = AddNewBuff("Electrocuted", teslaField.iconSprite, Color.cyan, false, true);
            armorShred = AddNewBuff("ArmorShred", pulverized.iconSprite, Color.cyan, true, true);

            RegisterDoTs();
        }

        private static void RegisterDoTs()
        {
            electrocuteDoT = DotAPI.RegisterDotDef(0.2f, (Config.electrocuteDPS.Value * 0.2f), DamageColorIndex.Default, electrocute, null, null);
        }

        // simple helper method
        internal static BuffDef AddNewBuff(string buffName, Sprite buffIcon, Color buffColor, bool canStack, bool isDebuff)
        {
            BuffDef buffDef = ScriptableObject.CreateInstance<BuffDef>();
            buffDef.name = buffName;
            buffDef.buffColor = buffColor;
            buffDef.canStack = canStack;
            buffDef.isDebuff = isDebuff;
            buffDef.eliteDef = null;
            buffDef.iconSprite = buffIcon;

            Modules.Content.AddBuffDef(buffDef);

            return buffDef;
        }
    }
}