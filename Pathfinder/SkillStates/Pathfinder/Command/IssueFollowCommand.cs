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

            if(!pathfinderController.javelinReady)
                base.PlayCrossfade("Gesture, Override", "Wave", "Hand.playbackRate", duration, 0.1f);
            else
                base.PlayCrossfade("Gesture, Override", "JavWave", "Hand.playbackRate", duration, 0.1f);

            pathfinderController.SetToFollow();
        }
    }
}
