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
                if ((base.inputBank.skill1.down))// || base.inputBank.skill2.down || base.inputBank.skill3.down || base.inputBank.skill4.down))
                {
                    target = tracker.GetTrackingTarget();
                    if (target)
                    {
                        this.outer.SetNextState(new AttackCommand() { target = this.target });
                    }
                    else
                    {
                        Log.Warning("Command failed");
                        base.skillLocator.special.AddOneStock();
                        this.outer.SetNextStateToMain();
                    }
                }

                if ((base.inputBank.skill2.down))// || base.inputBank.skill2.down || base.inputBank.skill3.down || base.inputBank.skill4.down))
                {
                    this.outer.SetNextState(new FollowCommand());
                    base.skillLocator.special.AddOneStock();
                }
                /*
                if ((base.inputBank.skill4.down))// || base.inputBank.skill2.down || base.inputBank.skill3.down || base.inputBank.skill4.down))
                {
                    target = tracker.GetTrackingTarget();
                    if (target)
                    {
                        this.outer.SetNextState(new SpecialCommand() { target = this.target });
                    }
                    else
                    {
                        this.outer.SetNextStateToMain();
                    }
                }
                */
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