using Pathfinder.SkillStates;
using Pathfinder.SkillStates.Empower;
using Pathfinder.SkillStates.BaseStates;
using System.Collections.Generic;
using System;

namespace Pathfinder.Modules
{
    public static class States
    {
        internal static void RegisterStates()
        {
            Modules.Content.AddEntityState(typeof(BaseMeleeAttack));
            Modules.Content.AddEntityState(typeof(Thrust));

            Modules.Content.AddEntityState(typeof(JavelinToss));
            Modules.Content.AddEntityState(typeof(TriCombo));
            Modules.Content.AddEntityState(typeof(Lunge));

            Modules.Content.AddEntityState(typeof(Haste));
        }
    }
}