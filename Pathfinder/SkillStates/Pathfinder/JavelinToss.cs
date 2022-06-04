using EntityStates;
using UnityEngine;
using Pathfinder.Misc;

namespace Pathfinder.SkillStates.Empower
{
    internal class JavelinToss : BaseState
    {
        private Animator animator;
        private ChildLocator childLocator;
        private GameObject javelin;
        private EmpowerComponent empowerComponent;

        public static float baseDuration = 0.5f;
        public float duration;
        public override void OnEnter()
        {
            Log.Warning("JavelinToss");
            base.OnEnter();
            duration = baseDuration / base.attackSpeedStat;
            base.StartAimMode(baseDuration + 0.1f, false);
            animator = base.GetModelAnimator();
            childLocator = base.GetModelChildLocator();
            empowerComponent = base.GetComponent<EmpowerComponent>();

            javelin = childLocator.FindChild("Spear").gameObject;
            javelin.SetActive(false);

            if (animator)
            {
                if (!animator.GetBool("isMoving") && animator.GetBool("isGrounded"))
                {
                    base.PlayAnimation("FullBody, Override", "JavelinToss", "Hand.playbackRate", duration);
                }
                else
                {
                    base.PlayAnimation("Gesture, Override", "JavelinToss", "Hand.playbackRate", duration);
                }
            }

            empowerComponent.ResetPrimary(base.skillLocator);
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