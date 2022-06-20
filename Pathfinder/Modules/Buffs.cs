using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinder.Modules
{
    public static class Buffs
    {
        // armor buff gained during roll
        internal static BuffDef armorBuff;
        internal static BuffDef empowerBuff;

        internal static void RegisterBuffs()
        {
            empowerBuff = AddNewBuff("EmpowerBuff", null, Color.cyan, false, false);
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