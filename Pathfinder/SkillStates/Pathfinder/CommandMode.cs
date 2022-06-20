using EntityStates;
using RoR2;
using Pathfinder.Components;
using UnityEngine;
using Pathfinder;

namespace Skillstates.Pathfinder
{
    internal class CommandMode : BaseState
    {
        public static float minDuration = 0.1f;

        private CommandTracker tracker;
        private PathfinderController pathfinderController;
        private HurtBox target;
        public override void OnEnter()
        {
            base.OnEnter();
            tracker = base.GetComponent<CommandTracker>();
            //pathfinderController = base.GetComponent<PathfinderController>();
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
                        this.outer.SetNextState(new IssueCommand() { target = this.target });
                    }
                    else
                    {
                        Log.Warning("Command failed");
                        this.outer.SetNextStateToMain();
                    }
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
            return InterruptPriority.PrioritySkill;
        }
    }
}