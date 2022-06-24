using UnityEngine;
using RoR2;
using EntityStates;
using Pathfinder.Components;
using Pathfinder;

namespace Skillstates.Pathfinder.Command
{
    internal class BaseIssueCommand : BaseState
    {
        protected PathfinderController pathfinderController;

        public static float baseDuration = 1f;
        protected float duration;
        public override void OnEnter()
        {
            base.OnEnter();
            pathfinderController = base.GetComponent<PathfinderController>();
            duration = baseDuration / base.attackSpeedStat;
            base.StartAimMode(duration, false);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= duration)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}
