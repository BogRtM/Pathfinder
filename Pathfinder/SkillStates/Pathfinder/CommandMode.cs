using EntityStates;
using RoR2;
using Pathfinder.Components;

namespace Skillstates.Pathfinder
{
    internal class CommandMode : BaseState
    {
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

            if (base.inputBank.skill1.down || base.inputBank.skill2.down || base.inputBank.skill3.down)
            {
                target = tracker.GetTrackingTarget();
                if(target && target.healthComponent) target.healthComponent.Suicide();
                this.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            tracker.DeactivateIndicator();
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}