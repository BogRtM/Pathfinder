using System;
using System.Collections.Generic;
using System.Text;
using EntityStates;
using UnityEngine;

namespace Pathfinder.SkillStates
{
    internal class Thrust : BaseState
    {
        private Animator animator;

        public static float baseDuration = 0.7f;
        public float duration;
        public override void OnEnter()
        {
            Log.Warning("Thrust");
            base.OnEnter();
            duration = baseDuration / base.attackSpeedStat;
            base.StartAimMode(baseDuration + 0.1f, false);
            animator = base.GetModelAnimator();

            if (!animator.GetBool("isMoving") && animator.GetBool("isGrounded"))
            {
                Log.Warning("FullBody");
                base.PlayAnimation("FullBody, Override", "Thrust", "Thrust.playbackRate", duration);
            }
            else
            {
                Log.Warning("Gesture");
                base.PlayAnimation("Gesture, Override", "Thrust", "Thrust.playbackRate", duration);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(base.fixedAge >= this.duration)
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
            return InterruptPriority.Skill;
        }
    }
}
