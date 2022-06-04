using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace Pathfinder.Modules
{
    public static class EmpowerHelpers
    {
        public static SkillDef GetEmpowerSkill(CharacterBody characterBody)
        {
            return characterBody.skillLocator.FindSkillByFamilyName((Skills.empowerFamily as ScriptableObject).name).skillDef;
        }

        public static void SetPrimary(CharacterBody characterBody)
        {
            SkillDef skillToSet = GetEmpowerSkill(characterBody);
            characterBody.skillLocator.primary.SetSkillOverride(characterBody.gameObject, skillToSet, GenericSkill.SkillOverridePriority.Contextual);
        }

        public static void ResetPrimary(CharacterBody characterBody)
        {
            SkillDef skillToSet = GetEmpowerSkill(characterBody);
            characterBody.skillLocator.primary.UnsetSkillOverride(characterBody.gameObject, skillToSet, GenericSkill.SkillOverridePriority.Contextual);
        }
    }
}
