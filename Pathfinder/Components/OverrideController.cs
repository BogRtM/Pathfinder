using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using Pathfinder.Modules;
using Pathfinder.Modules;
using RoR2.UI;
using System.Linq;
using RoR2.Skills;
using System.Collections.Generic;
using RoR2.Projectile;
//using Pathfinder.Modules.Misc;

namespace Pathfinder.Components
{
    internal class OverrideController : MonoBehaviour //, IOnDamageDealtServerReceiver
    {

        private Animator modelAnimator;
        private SkillLocator skillLocator;

        private CharacterBody selfBody;
        //private CrosshairUtils.OverrideRequest overrideRequest;

        internal static SkillDef utilityCommandSkillDef;

        internal static SkillDef javelinSkill;
        internal static SkillDef strongThrustSkill;

        internal static SkillDef attackCommand;
        internal static SkillDef followCommand;
        internal static SkillDef cancelCommand;
        internal static SkillDef specialCommand;
        //internal GenericSkill squallSpecial;

        internal bool javelinReady;
        internal bool heartseekerReady;
        internal bool inCommandMode;

        private void Awake()
        {
            modelAnimator = base.GetComponentInChildren<Animator>();
            skillLocator = base.GetComponent<SkillLocator>();
        }

        private void Start()
        {
            selfBody = base.GetComponent<CharacterBody>();
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

        public void ReadyHeartseeker()
        {
            heartseekerReady = true;

            skillLocator.primary.SetSkillOverride(base.gameObject, strongThrustSkill, GenericSkill.SkillOverridePriority.Contextual);
        }

        public void UnreadyHeartseeker()
        {
            heartseekerReady = false;

            skillLocator.primary.UnsetSkillOverride(base.gameObject, strongThrustSkill, GenericSkill.SkillOverridePriority.Contextual);
        }

        internal void SetCommandSkills()
        {
            skillLocator.primary.SetSkillOverride(base.gameObject, attackCommand, GenericSkill.SkillOverridePriority.Contextual);
            skillLocator.secondary.SetSkillOverride(base.gameObject, followCommand, GenericSkill.SkillOverridePriority.Contextual);
            /*
            if(skillLocator.special.baseSkill == utilityCommandSkillDef)
            {
                skillLocator.special.SetSkillOverride(base.gameObject, cancelCommand, GenericSkill.SkillOverridePriority.Contextual);
                skillLocator.utility.SetSkillOverride(base.gameObject, specialCommand, GenericSkill.SkillOverridePriority.Contextual);
                return;
            }
            */

            skillLocator.utility.SetSkillOverride(base.gameObject, cancelCommand, GenericSkill.SkillOverridePriority.Contextual);
            skillLocator.special.SetSkillOverride(base.gameObject, specialCommand, GenericSkill.SkillOverridePriority.Contextual);
        }

        internal void UnsetCommandSkills()
        {
            
            skillLocator.primary.UnsetSkillOverride(base.gameObject, attackCommand, GenericSkill.SkillOverridePriority.Contextual);
            skillLocator.secondary.UnsetSkillOverride(base.gameObject, followCommand, GenericSkill.SkillOverridePriority.Contextual);
            
            /*
            if (skillLocator.special.baseSkill == utilityCommandSkillDef)
            {
                skillLocator.special.UnsetSkillOverride(base.gameObject, cancelCommand, GenericSkill.SkillOverridePriority.Contextual);
                skillLocator.utility.UnsetSkillOverride(base.gameObject, specialCommand, GenericSkill.SkillOverridePriority.Contextual);
                return;
            }
            */

            skillLocator.utility.UnsetSkillOverride(base.gameObject, cancelCommand, GenericSkill.SkillOverridePriority.Contextual);
            skillLocator.special.UnsetSkillOverride(base.gameObject, specialCommand, GenericSkill.SkillOverridePriority.Contextual);
        }
    }
}
