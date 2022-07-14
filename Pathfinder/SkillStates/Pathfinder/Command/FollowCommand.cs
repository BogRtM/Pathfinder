using UnityEngine;
using RoR2;
using EntityStates;
using Pathfinder.Components;
using Pathfinder;

namespace Skillstates.Pathfinder.Command
{
    internal class FollowCommand : BaseIssueCommand
    {
        private string javString = "";
        public override void OnEnter()
        {
            base.OnEnter();

            if (overrideController.javelinReady) javString = "Jav";

            base.PlayCrossfade("Gesture, Override", javString + "Wave", "Hand.playbackRate", duration, 0.1f);

            falconerComponent.FollowOrder();
        }
    }
}
