using EntityStates;
using RoR2;
using Pathfinder.Components;
using UnityEngine;
using Pathfinder;

namespace Skillstates.Pathfinder.Command
{
    internal class CommandMode : BaseState
    {
        public static float minDuration = 0.1f;

        private CommandTracker tracker;
        private HurtBox target;
        public override void OnEnter()
        {
            base.OnEnter();
            tracker = base.GetComponent<CommandTracker>();
            tracker.ActivateIndicator();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            
        }

        public override void OnExit()
        {
            tracker.DeactivateIndicator();
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}