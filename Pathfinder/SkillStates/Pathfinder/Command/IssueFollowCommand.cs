using UnityEngine;
using RoR2;
using EntityStates;
using Pathfinder.Components;
using Pathfinder;

namespace Skillstates.Pathfinder.Command
{
    internal class IssueFollowCommand : BaseIssueCommand
    {
        public override void OnEnter()
        {
            base.OnEnter();

            pathfinderController.SetToFollow();
        }
    }
}
