using EntityStates;
using UnityEngine;
using Pathfinder.Modules;
using Pathfinder.Misc;

namespace Pathfinder.SkillStates
{
    internal class Haste : BaseState
    {
        public static float baseDuration = 0.3f;
        public static float speedCoefficient = 1.8f;

        private EmpowerComponent empowerComponent;
        private Vector3 dashVector;
        private Animator animator;

        public override void OnEnter()
        {
            base.OnEnter();
            empowerComponent = base.gameObject.GetComponent<EmpowerComponent>();
            animator = base.GetModelAnimator();
            dashVector = (base.inputBank.moveVector == Vector3.zero) ? base.characterDirection.forward : base.inputBank.moveVector;

            if(Vector3.Dot(dashVector, base.inputBank.moveVector.normalized) < 0)
            {
                PlayAnimation("FullBody, Override", "GroundDashB", "Dash.playbackRate", baseDuration);
            } else
            {
                PlayAnimation("FullBody, Override", "GroundDashF", "Dash.playbackRate", baseDuration);
            }

            empowerComponent.SetPrimary(base.skillLocator);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(base.characterMotor && base.isAuthority)
            {
                base.characterMotor.velocity.y = 0f;
                base.characterMotor.rootMotion += dashVector * (speedCoefficient * base.moveSpeedStat * Time.fixedDeltaTime);
            }

            if(base.fixedAge >= baseDuration)
            {
                base.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}