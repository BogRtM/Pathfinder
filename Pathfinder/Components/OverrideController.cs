using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using Pathfinder.Content;
using RoR2.UI;
using System.Linq;
using RoR2.Skills;
using System.Collections.Generic;
//using Pathfinder.Modules.Misc;

namespace Pathfinder.Components
{
    internal class OverrideController : MonoBehaviour //, IOnDamageDealtServerReceiver
    {

        private Animator modelAnimator;
        private SkillLocator skillLocator;

        private CharacterBody selfBody;

        internal static SkillDef javelinSkill;
        internal static SkillDef squallAttack;
        internal static SkillDef squallFollow;
        //internal static SkillDef squallUtility;
        internal GenericSkill squallSpecial;

        internal int squallSpecialMaxStock = 1;
        internal int squallSpecialCurrentStock;

        internal float squallSpecialRechargeInterval;

        internal bool javelinReady;

        private void Awake()
        {
            modelAnimator = base.GetComponentInChildren<Animator>();
            skillLocator = base.GetComponent<SkillLocator>();
        }

        private void Start()
        {
            selfBody = base.GetComponent<CharacterBody>();
            squallSpecial = skillLocator.FindSkill("SquallSpecial");
            squallSpecialMaxStock = squallSpecialMaxStock + selfBody.inventory.GetItemCount(DLC1Content.Items.EquipmentMagazineVoid);
            squallSpecialCurrentStock = 0;

        }

        private void OnDestroy()
        {
            squallSpecialMaxStock = 1;
        }

        public void ReadyJavelin()
        {
            javelinReady = true;
            skillLocator.primary.SetSkillOverride(base.gameObject, javelinSkill, GenericSkill.SkillOverridePriority.Contextual);
            if (modelAnimator)
            {
                modelAnimator.SetLayerWeight(modelAnimator.GetLayerIndex("JavelinReady"), 1f);
            }
        }

        public void UnreadyJavelin()
        {
            javelinReady = false;
            skillLocator.primary.UnsetSkillOverride(base.gameObject, javelinSkill, GenericSkill.SkillOverridePriority.Contextual);
            if (modelAnimator)
            {
                modelAnimator.SetLayerWeight(modelAnimator.GetLayerIndex("JavelinReady"), 0f);
            }
        }

        internal void SetCommandSkills()
        {
            skillLocator.primary.SetSkillOverride(base.gameObject, squallAttack, GenericSkill.SkillOverridePriority.Contextual);
            skillLocator.secondary.SetSkillOverride(base.gameObject, squallFollow, GenericSkill.SkillOverridePriority.Contextual);
            //base.skillLocator.primary.SetSkillOverride(base.gameObject, PathfinderController.squallAttack, GenericSkill.SkillOverridePriority.Contextual);
            skillLocator.special.SetSkillOverride(base.gameObject, squallSpecial.skillDef, GenericSkill.SkillOverridePriority.Contextual);
        }

        internal void UnsetCommandSkills()
        {
            skillLocator.primary.UnsetSkillOverride(base.gameObject, squallAttack, GenericSkill.SkillOverridePriority.Contextual);
            skillLocator.secondary.UnsetSkillOverride(base.gameObject, squallFollow, GenericSkill.SkillOverridePriority.Contextual);
            //base.skillLocator.primary.SetSkillOverride(base.gameObject, PathfinderController.squallAttack, GenericSkill.SkillOverridePriority.Contextual);
            skillLocator.special.UnsetSkillOverride(base.gameObject, squallSpecial.skillDef, GenericSkill.SkillOverridePriority.Contextual);
        }
    }
}
