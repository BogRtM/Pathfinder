using UnityEngine;
using RoR2;
using EntityStates;
using Pathfinder.Components;
using Pathfinder;

namespace Skillstates.Pathfinder.Command
{
    internal class BaseIssueCommand : BaseState
    {
        protected FalconerComponent falconerComponent;
        protected OverrideController overrideController;

        public static float baseDuration = 0.5f;
        protected float duration;
        protected CommandTracker commandTracker;
        public override void OnEnter()
        {
            base.OnEnter();
            falconerComponent = base.GetComponent<FalconerComponent>();
            overrideController = base.GetComponent<OverrideController>();
            commandTracker = base.GetComponent<CommandTracker>();
            duration = baseDuration / base.attackSpeedStat;
            base.StartAimMode(duration + 0.1f, false);
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
            return InterruptPriority.Any;
        }
    }
}
