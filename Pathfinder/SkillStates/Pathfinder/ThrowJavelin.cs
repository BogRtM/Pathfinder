using EntityStates;
using UnityEngine;

namespace Pathfinder.SkillStates
{
    internal class ThrowJavelin : BaseState
    {
        private Animator animator;
        private ChildLocator childLocator;
        private GameObject javelin;

        public static float baseDuration = 0.5f;
        public float duration;
        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / base.attackSpeedStat;
            base.StartAimMode(baseDuration + 0.1f, false);
            animator = GetComponent<Animator>();
            childLocator = base.GetModelChildLocator();

            javelin = childLocator.FindChild("Spear").gameObject;
            javelin.SetActive(false);

            if (animator)
            {
                if (!animator.GetBool("isMoving") && animator.GetBool("isGrounded"))
                {
                    base.PlayAnimation("Gesture, Override", "JavelinToss");
                }
                else
                {
                    base.PlayAnimation("FullBody, Override", "JavelinToss");
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
            javelin.SetActive(true);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}