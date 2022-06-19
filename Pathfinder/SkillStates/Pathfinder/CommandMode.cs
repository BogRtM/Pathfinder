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
            pathfinderController = base.GetComponent<PathfinderController>();
            tracker.ActivateIndicator();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(base.fixedAge >= minDuration)
            {
                if ((base.inputBank.skill1.down))// || base.inputBank.skill2.down || base.inputBank.skill3.down || base.inputBank.skill4.down))
                {
                    base.StartAimMode(0.2f, false);
                    base.PlayCrossfade("Gesture, Override", "Point", "Hand.playbackRate", 0.5f, 0.1f);
                    target = tracker.GetTrackingTarget();
                    if(target)
                    {
                        Log.Warning("Attempting attack command");
                        pathfinderController.ChooseTarget(target);
                    }
                    this.outer.SetNextStateToMain();
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