using EntityStates;
using UnityEngine;
using Pathfinder.Modules;
using Pathfinder.Components;
using RoR2;
using RoR2.Skills;

namespace Skillstates.Pathfinder
{
    internal class Pursuit : BaseState
    {
        public static float baseDuration = 0.2f;
        public static float speedCoefficient = 8f;

        public static SkillDef javelinSkill;

        private PathfinderController controller;

        private Vector3 dashVector;
        private Animator animator;

        public override void OnEnter()
        {
            base.OnEnter();
            animator = base.GetModelAnimator();
            controller = base.GetComponent<PathfinderController>();
            dashVector = ((base.inputBank.moveVector == Vector3.zero) ? base.characterDirection.forward : base.inputBank.moveVector).normalized;
            base.characterDirection.forward = dashVector;
            base.characterMotor.velocity *= 0.1f;

            if (!controller.javelinReady)
            {
                PlayAnimation("FullBody, Override", "GroundDashF", "Dash.playbackRate", baseDuration);
                controller.ReadyJavelin();
            } else
            {
                PlayAnimation("FullBody, Override", "JavGroundDash", "Dash.playbackRate", baseDuration);
            }

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