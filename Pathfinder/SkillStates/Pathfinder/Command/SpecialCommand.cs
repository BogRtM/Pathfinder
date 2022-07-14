using UnityEngine;
using RoR2;
using EntityStates;
using Pathfinder.Components;
using Pathfinder;

namespace Skillstates.Pathfinder.Command
{
    internal class SpecialCommand : BaseIssueCommand
    {
        public HurtBox target;
        public override void OnEnter()
        {
            base.OnEnter();
            target = commandTracker.GetTrackingTarget();
            Chat.AddMessage(target.healthComponent.gameObject.name);
        }
    }
}
