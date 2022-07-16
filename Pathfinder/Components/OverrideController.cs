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
        internal static SkillDef attackCommand;
        internal static SkillDef followCommand;
        internal static SkillDef cancelCommand;
        internal GenericSkill squallSpecial;

        internal int squallSpecialMaxStock;
        internal int squallSpecialCurrentStock;

        internal float specialStopwatch;
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

            squallSpecialRechargeInterval = squallSpecial.baseRechargeInterval;
            squallSpecialMaxStock = squallSpecial.maxStock;
            squallSpecialCurrentStock = squallSpecialMaxStock;

        }

        private void FixedUpdate()
        {
            if (specialStopwatch < squallSpecialRechargeInterval)
            {
                specialStopwatch += Time.fixedDeltaTime;
            }
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
            skillLocator.primary.SetSkillOverride(base.gameObject, attackCommand, GenericSkill.SkillOverridePriority.Contextual);
            skillLocator.secondary.SetSkillOverride(base.gameObject, followCommand, GenericSkill.SkillOverridePriority.Contextual);
            skillLocator.utility.SetSkillOverride(base.gameObject, cancelCommand, GenericSkill.SkillOverridePriority.Contextual);
            skillLocator.special.SetSkillOverride(base.gameObject, squallSpecial.skillDef, GenericSkill.SkillOverridePriority.Contextual);
            //skillLocator.special.set
        }

        internal void UnsetCommandSkills()
        {
            skillLocator.primary.UnsetSkillOverride(base.gameObject, attackCommand, GenericSkill.SkillOverridePriority.Contextual);
            skillLocator.secondary.UnsetSkillOverride(base.gameObject, followCommand, GenericSkill.SkillOverridePriority.Contextual);
            skillLocator.utility.UnsetSkillOverride(base.gameObject, cancelCommand, GenericSkill.SkillOverridePriority.Contextual);
            skillLocator.special.UnsetSkillOverride(base.gameObject, squallSpecial.skillDef, GenericSkill.SkillOverridePriority.Contextual);
        }
    }
}
