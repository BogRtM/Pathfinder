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

            if (!pathfinderController.javelinReady)
                base.PlayCrossfade("Gesture, Override", "Point", "Hand.playbackRate", duration, 0.1f);
            else
                base.PlayCrossfade("Gesture, Override", "JavPoint", "Hand.playbackRate", duration, 0.1f);

            if (target)
            {
                pathfinderController.SetTarget(target);
            }
        }
    }
}
