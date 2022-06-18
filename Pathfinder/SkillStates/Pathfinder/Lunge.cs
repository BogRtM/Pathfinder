using EntityStates;
using EntityStates.Merc;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using Pathfinder.Components;
using System;
using UnityEngine.AddressableAssets;

namespace Skillstates.Pathfinder
{
    internal class Lunge : BaseState
    {
        private Animator animator;
        private OverlapAttack lungeAttack;

        public static float lungeDuration = 0.3f;
        public static float speedCoefficient = 10f;

        private bool doSweep;
        private float duration;
        private float sweepStopwatch;

        private Ray aimRay;
        private Vector3 dashVector;
        private EmpowerComponent empowerComponent;
        public override void OnEnter()
        {
            base.OnEnter();
            base.StartAimMode(duration + 0.1f, true);
            animator = base.GetModelAnimator();
            base.characterMotor.Motor.ForceUnground();
            aimRay = base.GetAimRay();
            empowerComponent = base.GetComponent<EmpowerComponent>();

            if (animator)
            {
                animator.SetBool("isThrusting", true);
                animator.SetBool("isGrounded", false);
                base.PlayAnimation("FullBody, Override", "Lunge");
            }

            if(NetworkServer.active)
            {
                base.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
            }

            dashVector = aimRay.direction;
            base.characterMotor.disableAirControlUntilCollision = true;
            base.characterMotor.velocity.y = 0f;

            Transform modelTransform = base.GetModelTransform();
            HitBoxGroup hitBoxGroup = null;

            if (modelTransform)
            {
                hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "Spear");
            }

            this.lungeAttack = new OverlapAttack();
            lungeAttack.attacker = base.gameObject;
            lungeAttack.inflictor = base.gameObject;
            lungeAttack.damageType = DamageType.Generic;
            lungeAttack.procCoefficient = 1f;
            lungeAttack.teamIndex = base.GetTeam();
            lungeAttack.isCrit = base.RollCrit();
            lungeAttack.forceVector = Vector3.zero;
            lungeAttack.pushAwayForce = 1f;
            lungeAttack.damage = 3f * base.damageStat;
            lungeAttack.hitBoxGroup = hitBoxGroup;
            lungeAttack.hitEffectPrefab = GroundLight.comboHitEffectPrefab;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.characterDirection.forward = dashVector;
            base.characterDirection.moveVector = dashVector;
            base.characterMotor.rootMotion += dashVector * base.moveSpeedStat * speedCoefficient * Time.fixedDeltaTime;

            if (lungeAttack.Fire())
            {
                doSweep = true;
                base.characterMotor.disableAirControlUntilCollision = false;
                base.characterMotor.velocity = Vector3.zero;
                base.outer.SetNextState(new Sweep());
            }

            if ((base.fixedAge >= lungeDuration && !doSweep))
            {
                base.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.PlayAnimation("FullBody, Override", "BufferEmpty");

            empowerComponent.ResetPrimary(base.skillLocator);

            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
                base.characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, 0.5f);
            }

            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}