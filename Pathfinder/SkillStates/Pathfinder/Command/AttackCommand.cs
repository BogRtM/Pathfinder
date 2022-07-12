using UnityEngine;
using RoR2;
using EntityStates;
using Pathfinder.Components;
using Pathfinder;

namespace Skillstates.Pathfinder.Command
{
    internal class AttackCommand : BaseIssueCommand
    {
        public HurtBox target;

        private string javString = "";
        public override void OnEnter()
        {
            base.OnEnter();

            if (pathfinderController.javelinReady) javString = "Jav";
            
            base.PlayCrossfade("Gesture, Override", javString + "Point", "Hand.playbackRate", duration, 0.1f);

            pathfinderController.AttackOrder(target);
        }
    }
}
