using EntityStates;
using EntityStates.Merc;
using EntityStates.Croco;
using UnityEngine;
using RoR2;
using System;

namespace Pathfinder.SkillStates
{
    internal class AirFlip : BaseState
    {
        private Animator animator;
        Vector3 flipVector;

        private OverlapAttack airSpinAttack;
        private OverlapAttack groundSpinAttack;

        private bool isCrit;
        private bool inAirSpin;
        private bool inGroundSpin;
        private bool flipFinished;
        private float spinStopwatch;
        private float spinDuration;
        private float spinFinishTime;
        private float previousAirControl;

        private bool isInHitPause;
        private BaseState.HitStopCachedState hitStopCachedState;
        private float hitPauseTimer;

        public static float flipBaseDuration = 0.2f;
        public static float spinBaseDuration = 1f;
        public static float forwardVelocity = 3f;
        public static float upwardVelocity = 20f;
        public override void OnEnter()
        {
            base.OnEnter();
            animator = base.GetModelAnimator();
            spinDuration = spinBaseDuration / base.attackSpeedStat;
            spinFinishTime = spinDuration * 0.325f;

            base.PlayCrossfade("FullBody, Override", "AirFlip2", "Flip.playbackRate", flipBaseDuration, 0.2f);

            base.characterBody.bodyFlags |= RoR2.CharacterBody.BodyFlags.IgnoreFallDamage;

            base.characterMotor.Motor.ForceUnground();
            previousAirControl = base.characterMotor.airControl;
            base.characterMotor.airControl = BaseLeap.airControl;
            base.characterBody.isSprinting = true;

            flipVector = base.GetAimRay().direction;

            base.StartAimMode(0.1f, true);
            base.characterMotor.velocity.y = 0f;
            base.characterMotor.velocity.y += upwardVelocity;
            base.characterMotor.velocity += flipVector * base.moveSpeedStat * forwardVelocity;

            Transform modelTransform = base.GetModelTransform();
            HitBoxGroup hitBoxGroup = null;

            if (modelTransform)
            {
                hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "Spear");
            }

            isCrit = base.RollCrit();

            this.airSpinAttack = new OverlapAttack();
            airSpinAttack.attacker = base.gameObject;
            airSpinAttack.inflictor = base.gameObject;
            airSpinAttack.damageType = DamageType.Generic;
            airSpinAttack.procCoefficient = 1f;
            airSpinAttack.teamIndex = base.GetTeam();
            airSpinAttack.isCrit = isCrit;
            airSpinAttack.forceVector = Vector3.zero;
            airSpinAttack.pushAwayForce = 1f;
            airSpinAttack.damage = 7f * base.damageStat;
            airSpinAttack.hitBoxGroup = hitBoxGroup;
            airSpinAttack.hitEffectPrefab = GroundLight.comboHitEffectPrefab;

            this.groundSpinAttack = new OverlapAttack();
            groundSpinAttack.attacker = base.gameObject;
            groundSpinAttack.inflictor = base.gameObject;
            groundSpinAttack.damageType = DamageType.Stun1s;
            groundSpinAttack.procCoefficient = 1f;
            groundSpinAttack.teamIndex = base.GetTeam();
            groundSpinAttack.isCrit = isCrit;
            groundSpinAttack.forceVector = -Vector3.up * 6000f;
            groundSpinAttack.pushAwayForce = 500f;
            groundSpinAttack.damage = 7f * base.damageStat;
            groundSpinAttack.hitBoxGroup = hitBoxGroup;
            groundSpinAttack.hitEffectPrefab = GroundLight.finisherHitEffectPrefab;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            base.characterDirection.forward = flipVector;

            if(inGroundSpin && flipFinished)
            {
                spinStopwatch += Time.fixedDeltaTime;
            }

            if(spinStopwatch >= this.spinDuration)
            {
                this.outer.SetNextStateToMain();
            }

            /*
            if(base.fixedAge >= flipBaseDuration && !characterMotor.isGrounded && !inAirSpin && !flipFinished)
            {
                inAirSpin = true;
            }
            */

            if (!base.characterMotor.isGrounded && !flipFinished)
            {
                base.characterBody.isSprinting = true;
                FireAttack(airSpinAttack);
            }

            if (base.characterMotor.isGrounded && !inGroundSpin)
            {
                StartGroundSpin();
            }

            if (inGroundSpin && flipFinished && (spinStopwatch <= spinFinishTime))
            {
                FireAttack(groundSpinAttack);
            }
        }

        public void StartDrop()
        {
            inAirSpin = true;
            base.PlayAnimation("FullBody, Override", "AirFlip2");
            base.characterMotor.velocity.y = 0;
            base.characterMotor.velocity.y -= 90f;
        }

        public void StartGroundSpin()
        {
            inAirSpin = false;
            flipFinished = true;
            base.characterMotor.velocity = Vector3.zero;
            base.PlayAnimation("FullBody, Override", "SpinSweep", "Flip.playbackRate", spinDuration);
            Util.PlayAttackSpeedSound(GroundLight.finisherAttackSoundString, base.gameObject, GroundLight.slashPitch);
            inGroundSpin = true;
        }

        public void FireAttack(OverlapAttack attack)
        {
            if (attack.Fire() && !isInHitPause)
            {
                if (!this.isInHitPause)
                {
                    this.hitStopCachedState = base.CreateHitStopCachedState(base.characterMotor, this.animator, "Flip.playbackRate");
                    this.hitPauseTimer = GroundLight.hitPauseDuration / base.attackSpeedStat;
                    this.isInHitPause = true;
                }
                if (isInHitPause && hitPauseTimer <= 0)
                {
                    base.ConsumeHitStopCachedState(this.hitStopCachedState, base.characterMotor, this.animator);
                    this.isInHitPause = false;
                }
            }
        }

        public override void OnExit()
        {
            base.characterBody.bodyFlags &= ~RoR2.CharacterBody.BodyFlags.IgnoreFallDamage;
            base.characterMotor.airControl = previousAirControl;
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }

        /*
        public void FireAirSpin()
        {
            if(airSpinAttack.Fire() && !isInHitPause)
            {
                if (!this.isInHitPause)
                {
                    this.hitStopCachedState = base.CreateHitStopCachedState(base.characterMotor, this.animator, "Flip.playbackRate");
                    this.hitPauseTimer = GroundLight.hitPauseDuration / base.attackSpeedStat;
                    this.isInHitPause = true;
                }
                if(isInHitPause && hitPauseTimer <= 0)
                {
                    base.ConsumeHitStopCachedState(this.hitStopCachedState, base.characterMotor, this.animator);
                    this.isInHitPause = false;
                }
            }
        }

        public void FireGroundSpin()
        {
            if (groundSpinAttack.Fire() && !isInHitPause)
            {
                if (!this.isInHitPause)
                {
                    this.hitStopCachedState = base.CreateHitStopCachedState(base.characterMotor, this.animator, "Thrust.playbackRate");
                    this.hitPauseTimer = GroundLight.hitPauseDuration / base.attackSpeedStat;
                    this.isInHitPause = true;
                }
                if (isInHitPause && hitPauseTimer <= 0)
                {
                    base.ConsumeHitStopCachedState(this.hitStopCachedState, base.characterMotor, this.animator);
                    this.isInHitPause = false;
                }
            }
        }
        */
        }
    }
