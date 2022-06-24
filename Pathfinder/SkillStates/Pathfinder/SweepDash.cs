using EntityStates;
using UnityEngine;
using Pathfinder.Modules;
using Pathfinder.Components;
using RoR2;
using RoR2.Skills;
using System;
using EntityStates.Merc;

namespace Skillstates.Pathfinder
{
    internal class SweepDash : BaseState
    {
        public static float baseDuration = 0.2f;
        public static float speedCoefficient = 11f;

        private OverlapAttack attack;

        private Vector3 dashVector;
        private Animator animator;

        private string hitboxString;

        public override void OnEnter()
        {
            base.OnEnter();
            animator = base.GetModelAnimator();
            dashVector = ((base.inputBank.moveVector == Vector3.zero) ? base.characterDirection.forward : base.inputBank.moveVector).normalized;
            base.characterDirection.forward = dashVector;
            hitboxString = base.characterMotor.isGrounded ? "GroundSpin" : "AirSpin";

            PlayAnimation("FullBody, Override", "SpinSweep", "Flip.playbackRate", baseDuration);
            Util.PlayAttackSpeedSound(GroundLight.finisherAttackSoundString, base.gameObject, GroundLight.slashPitch);

            Transform modelTransform = base.GetModelTransform();
            HitBoxGroup hitBoxGroup = null;

            if (modelTransform)
            {
                hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == hitboxString);
            }

            this.attack = new OverlapAttack();
            attack.attacker = base.gameObject;
            attack.inflictor = base.gameObject;
            attack.damageType = DamageType.Generic;
            attack.procCoefficient = 1f;
            attack.teamIndex = base.GetTeam();
            attack.isCrit = base.RollCrit();
            attack.forceVector = Vector3.zero;
            attack.pushAwayForce = 1f;
            attack.damage = 6f * base.damageStat;
            attack.hitBoxGroup = hitBoxGroup;
            attack.hitEffectPrefab = GroundLight.finisherHitEffectPrefab;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority)
            {
                base.characterDirection.forward = dashVector;
                base.characterMotor.rootMotion += dashVector * (speedCoefficient * base.moveSpeedStat * Time.fixedDeltaTime);
                base.characterMotor.velocity.y = 0f;
                attack.Fire();
            }

            if (base.fixedAge >= baseDuration)
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