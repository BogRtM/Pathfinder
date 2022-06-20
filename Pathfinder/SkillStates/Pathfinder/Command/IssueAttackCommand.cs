using UnityEngine;
using RoR2;
using EntityStates;
using Pathfinder.Components;
using Pathfinder;

namespace Skillstates.Pathfinder.Command
{
    internal class IssueAttackCommand : BaseIssueCommand
    {
        public HurtBox target;
        public override void OnEnter()
        {
            base.OnEnter();

            if(target)
            {
                pathfinderController.SetTarget(target);
            }
        }
    }
}
