using RoR2;
using EntityStates;
using EntityStates.Merc;
using UnityEngine;
using System;

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
        private bool hasFired = false;
        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / base.attackSpeedStat;
            earlyExitTime = duration * 0.67f;
            fireTime = duration * 0.3f;
            base.StartAimMode(0.1f + duration, true);
            animator = base.GetModelAnimator();

            animator.SetLayerWeight(animator.GetLayerIndex("AimPitch"), 0f);

            if (!animator.GetBool("isMoving") && animator.GetBool("isGrounded"))
            {
                //Log.Warning("FullBody");
                base.PlayAnimation("FullBody, Override", "Thrust", "Thrust.playbackRate", duration);
            }
            else
            {
                //Log.Warning("Gesture");
                base.PlayAnimation("Gesture, Override", "Thrust", "Thrust.playbackRate", duration);
            }

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

            if(base.fixedAge >= fireTime && !hasFired)
            {
                this.attack.Fire();
                hasFired = true;
            }

            if(base.fixedAge >= earlyExitTime && base.inputBank.skill1.down)
            {
                base.characterBody.isSprinting = false;
                this.outer.SetNextState(new Thrust());
            }

            if(base.fixedAge >= this.duration)
            {
                base.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            //do thing here
            animator.SetLayerWeight(animator.GetLayerIndex("AimPitch"), 1f);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
