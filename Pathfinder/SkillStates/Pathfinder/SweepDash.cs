using EntityStates;
using UnityEngine;
using Pathfinder.Modules;
using Pathfinder.Components;
using RoR2;
using RoR2.Skills;
using RoR2.Projectile;

namespace Skillstates.Pathfinder
{
    internal class SweepDash : BaseState
    {
        public static float baseDuration = 0.2f;
        public static float speedCoefficient = 11f;
        public static float throwForce = 150f;

        public static SkillDef javelinSkill;

        private PathfinderController controller;

        private Vector3 dashVector;
        private Animator animator;
        private Ray aimRay;

        public override void OnEnter()
        {
            base.OnEnter();
            animator = base.GetModelAnimator();
            controller = base.GetComponent<PathfinderController>();
            dashVector = ((base.inputBank.moveVector == Vector3.zero) ? base.characterDirection.forward : base.inputBank.moveVector).normalized;
            base.characterDirection.forward = dashVector;

            if (!controller.javelinReady)
            {
                PlayAnimation("FullBody, Override", "GroundDashF", "Dash.playbackRate", baseDuration);
                //controller.ReadyJavelin();
            }
            else
            {
                PlayAnimation("FullBody, Override", "JavGroundDash", "Dash.playbackRate", baseDuration);
            }

            aimRay = base.GetAimRay();

            FireProjectileInfo fireProjectileInfo = new FireProjectileInfo();
            fireProjectileInfo.crit = base.RollCrit();
            fireProjectileInfo.damage = Config.JavelinDamage.Value * base.damageStat;
            fireProjectileInfo.force = throwForce;
            fireProjectileInfo.owner = base.gameObject;
            fireProjectileInfo.position = aimRay.origin; //leftHand.position;
            fireProjectileInfo.rotation = Util.QuaternionSafeLookRotation(aimRay.direction);
            fireProjectileInfo.projectilePrefab = Projectiles.javelinPrefab;
            ProjectileManager.instance.FireProjectile(fireProjectileInfo);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority)
            {
                base.characterDirection.forward = dashVector;
                base.characterMotor.rootMotion += dashVector * (speedCoefficient * base.moveSpeedStat * Time.fixedDeltaTime);
                base.characterMotor.velocity.y = 0f;
            }

            if (base.fixedAge >= baseDuration)
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