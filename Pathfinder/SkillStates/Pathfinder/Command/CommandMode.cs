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

            if(base.fixedAge >= minDuration)
            {
                if ((base.inputBank.skill1.down))
                {
                    target = tracker.GetTrackingTarget();
                    //base.skillLocator.special.AddOneStock();
                    this.outer.SetNextState(new AttackCommand() { target = this.target });
                }

                if ((base.inputBank.skill2.down))
                {
                    //base.skillLocator.special.AddOneStock();
                    this.outer.SetNextState(new FollowCommand());
                }
            }
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