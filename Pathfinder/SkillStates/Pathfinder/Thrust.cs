using RoR2;
using EntityStates;
using EntityStates.Merc;
using UnityEngine;
using System;
using Pathfinder.Modules;

namespace Skillstates.Pathfinder
{
    internal class Thrust : BaseState
    {
        private Animator animator;
        private OverlapAttack attack;

        public static float baseDuration = 0.8f;
        public static float smallHopVelocity = 5.5f;

        private float duration;
        private float earlyExitTime;
        private float fireTime;

        private bool hasHopped;
        private bool hasFired;
        private bool isCrit;
        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / base.attackSpeedStat;
            earlyExitTime = duration * 0.67f;
            fireTime = duration * 0.3f;
            animator = base.GetModelAnimator();
            animator.SetLayerWeight(animator.GetLayerIndex("AimPitch"), 0f);

            base.PlayAnimation("Gesture, Override", "Thrust", "Thrust.playbackRate", duration);
            

            Transform modelTransform = base.GetModelTransform();
            HitBoxGroup hitBoxGroup = null;

            if (modelTransform)
            {
                hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "Spear");
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
            attack.damage = 2.8f * base.damageStat;
            attack.hitBoxGroup = hitBoxGroup;
            attack.hitEffectPrefab = GroundLight.comboHitEffectPrefab;
            
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            base.StartAimMode(0.1f, true);

            if (base.fixedAge >= fireTime && !hasFired && base.isAuthority)
            {
                EffectManager.SimpleMuzzleFlash(Assets.thrustEffect, base.gameObject, "SpearTip", true);

                Util.PlaySound("PF_Thrust", base.gameObject);

                if(this.attack.Fire() && !base.characterMotor.isGrounded && !hasHopped)
                {
                    base.SmallHop(base.characterMotor, smallHopVelocity);
                    hasHopped = true;
                }
                hasFired = true;
            }

            if(base.fixedAge >= earlyExitTime)
            {
                animator.SetLayerWeight(animator.GetLayerIndex("AimPitch"), 1f);
            }

            if(base.fixedAge >= earlyExitTime && base.inputBank.skill1.down && base.isAuthority)
            {
                base.characterBody.isSprinting = false;
                this.outer.SetNextState(new Thrust());
            } else if(base.fixedAge >= this.duration && base.isAuthority)
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
            return InterruptPriority.Skill;
        }
    }
}
