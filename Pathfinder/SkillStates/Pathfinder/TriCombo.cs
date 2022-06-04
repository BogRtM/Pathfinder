using EntityStates;
using UnityEngine;

namespace Pathfinder.SkillStates.Empower
{
    internal class TriCombo : BaseState
    {
        private Animator animator;

        public static float baseDuration = 1f;
        public float duration;
        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / base.attackSpeedStat;
            base.StartAimMode(baseDuration + 0.1f, false);
            animator = GetComponent<Animator>();

            if (animator)
            {
                if (!animator.GetBool("isMoving") && animator.GetBool("isGrounded"))
                {
                    base.PlayAnimation("Gesture, Override", "Thrust");
                }
                else
                {
                    base.PlayAnimation("FullBody, Override", "Thrust");
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.duration)
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