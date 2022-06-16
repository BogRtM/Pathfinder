using Pathfinder.SkillStates;
using Pathfinder.SkillStates.Empower;
using System.Collections.Generic;
using System;

namespace Pathfinder.Modules
{
    public static class States
    {
        internal static void RegisterStates()
        {
            Modules.Content.AddEntityState(typeof(Thrust));

            Modules.Content.AddEntityState(typeof(JavelinToss));
            Modules.Content.AddEntityState(typeof(Lunge));
            Modules.Content.AddEntityState(typeof(Sweep));

            Modules.Content.AddEntityState(typeof(Haste));

            Modules.Content.AddEntityState(typeof(AirFlip));
        }
    }
}