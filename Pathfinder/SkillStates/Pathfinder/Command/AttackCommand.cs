using UnityEngine;
using RoR2;
using EntityStates;
using Pathfinder.Components;
using Pathfinder;

namespace Skillstates.Pathfinder.Command
{
    internal class AttackCommand : BaseIssueCommand
    {
        private CommandTracker commandTracker;

        private HurtBox target;

        private string javString = "";
        public override void OnEnter()
        {
            base.OnEnter();

            commandTracker = base.GetComponent<CommandTracker>();

            target = commandTracker.GetTrackingTarget();

            if (pathfinderController.javelinReady) javString = "Jav";
            
            base.PlayCrossfade("Gesture, Override", javString + "Point", "Hand.playbackRate", duration, 0.1f);

            pathfinderController.AttackOrder(target);
        }
    }
}
