using UnityEngine;
using RoR2;
using EntityStates;
using Pathfinder.Components;
using Pathfinder;

namespace Skillstates.Pathfinder
{
    internal class IssueCommand : BaseState
    {
        private PathfinderController pathfinderController;
        public HurtBox target;
        public static float baseDuration = 0.5f;
        private float duration;
        public override void OnEnter()
        {
            base.OnEnter();
            pathfinderController = base.GetComponent<PathfinderController>();
            duration = baseDuration / base.attackSpeedStat;
            base.StartAimMode(0.2f, false);
            base.PlayCrossfade("Gesture, Override", "Point", "Hand.playbackRate", 0.5f, 0.1f);
            if(target)
            {
                pathfinderController.ChooseTarget(target);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(base.fixedAge >= duration)
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
            return InterruptPriority.PrioritySkill;
        }
    }
}
