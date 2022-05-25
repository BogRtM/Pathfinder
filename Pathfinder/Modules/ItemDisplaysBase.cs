﻿using RoR2;
using System.Collections.Generic;

namespace PathfinderMod.Modules.Characters {
    public abstract class ItemDisplaysBase {

        public void SetItemDisplays(ItemDisplayRuleSet itemDisplayRuleSet) {

            List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemDisplayRules = new List<ItemDisplayRuleSet.KeyAssetRuleGroup>();

            SetItemDisplayRules(itemDisplayRules);

            itemDisplayRuleSet.keyAssetRuleGroups = itemDisplayRules.ToArray();
        }

        protected abstract void SetItemDisplayRules(List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemDisplayRules);
    }
}