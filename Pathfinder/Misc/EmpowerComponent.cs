using RoR2;
using RoR2.Skills;
using UnityEngine;
using Pathfinder.Modules;

namespace Pathfinder.Misc

{
    public class EmpowerComponent : MonoBehaviour
    {
        private SkillDef skillToSet;

        public void GetEmpowerSkill(SkillLocator skillLocator)
        {
            skillToSet = skillLocator.FindSkill("EmpowerSkill").skillDef;
        }

        public void SetPrimary(SkillLocator skillLocator)
        {
            GetEmpowerSkill(skillLocator);
            skillLocator.primary.SetSkillOverride(skillLocator.gameObject, skillToSet, GenericSkill.SkillOverridePriority.Contextual);
        }

        public void ResetPrimary(SkillLocator skillLocator)
        {
            GetEmpowerSkill(skillLocator);
            skillLocator.primary.UnsetSkillOverride(skillLocator.gameObject, skillToSet, GenericSkill.SkillOverridePriority.Contextual);
        }
    }
}
