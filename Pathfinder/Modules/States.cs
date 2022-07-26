using Skillstates.Pathfinder;
using Skillstates.Pathfinder;
using Skillstates.Pathfinder.Command;
using Skillstates.Squall;
using System.Collections.Generic;
using System;

namespace Pathfinder.Modules
{
    public static class States
    {
        internal static void RegisterStates()
        {
            #region Pathfinder
            Modules.Content.AddEntityState(typeof(Thrust));

            Modules.Content.AddEntityState(typeof(Evade));
            Modules.Content.AddEntityState(typeof(JavelinToss));

            Modules.Content.AddEntityState(typeof(AirFlip));
            Modules.Content.AddEntityState(typeof(AimBolas));

            Modules.Content.AddEntityState(typeof(CommandMode));
            Modules.Content.AddEntityState(typeof(BaseIssueCommand));
            Modules.Content.AddEntityState(typeof(AttackCommand));
            Modules.Content.AddEntityState(typeof(FollowCommand));
            Modules.Content.AddEntityState(typeof(SpecialCommand));
            #endregion

            #region Squall
            Modules.Content.AddEntityState(typeof(SquallMainState));
            Modules.Content.AddEntityState(typeof(DiveToPoint));
            Modules.Content.AddEntityState(typeof(MountedGuns));
            Modules.Content.AddEntityState(typeof(MissileLauncher));
            Modules.Content.AddEntityState(typeof(SquallEvis));
            #endregion
        }
    }
}