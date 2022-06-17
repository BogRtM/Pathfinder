using EntityStates;
using UnityEngine;
using Pathfinder.Modules;
using Pathfinder.Misc;

namespace Pathfinder.SkillStates
{
    internal class Haste : BaseState
    {
        public static float baseDuration = 0.2f;
        public static float speedCoefficient = 11f;

        private bool startedGrounded;

        private EmpowerComponent empowerComponent;
        private Vector3 dashVector;
        private Animator animator;

        public override void OnEnter()
        {
            base.OnEnter();
            empowerComponent = base.gameObject.GetComponent<EmpowerComponent>();
            animator = base.GetModelAnimator();
            dashVector = ((base.inputBank.moveVector == Vector3.zero) ? base.characterDirection.forward : base.inputBank.moveVector).normalized;
            base.characterDirection.forward = dashVector;

            PlayAnimation("FullBody, Override", "GroundDashF", "Dash.playbackRate", baseDuration);
            
            empowerComponent.SetPrimary(base.skillLocator);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(base.isAuthority)
            {
                base.characterDirection.forward = dashVector;
                base.characterMotor.rootMotion += dashVector * (speedCoefficient * base.moveSpeedStat * Time.fixedDeltaTime);
                base.characterMotor.velocity.y = 0f;
            }

            if(base.fixedAge >= baseDuration)
            {
                base.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            PlayCrossfade("FullBody, Override", "BufferEmpty", 0.2f);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}