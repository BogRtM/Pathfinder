using EntityStates;
using EntityStates.Merc;
using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using System.Text;
using Pathfinder.Components;
using Pathfinder;
using Pathfinder.Modules;

namespace Skillstates.Squall
{
    public class SquallEvis : BaseState
    {
        public static float diveDuration = 0.2f;
        public static float attackDuration = 1.3f;
        public static float baseAttackInterval = 0.129f;
        public static float damagePerHit = 0.7f;
        public static float chargePerHit = 1f;

        internal HurtBox target;

        private Transform modelTransform;
        private CharacterModel characterModel;
        private BatteryComponent batteryComponent;
        private SquallVFXComponent squallVFXComponent;

        private float attackInterval;
        private float stopwatch;

        private Vector3 startPosition;
        private Vector3 enemyPosition;

        private bool isCrit;
        private bool attackFinished;

        public override void OnEnter()
        {
            base.OnEnter();
            attackInterval = baseAttackInterval / base.attackSpeedStat;
            isCrit = base.RollCrit();
            modelTransform = base.GetModelTransform();
            characterModel = modelTransform.GetComponent<CharacterModel>();
            batteryComponent = base.GetComponent<BatteryComponent>();
            squallVFXComponent = base.GetComponent<SquallVFXComponent>();

            batteryComponent.pauseDrain = true;
            squallVFXComponent.ToggleTrails(false);

            characterModel.invisibilityCount++;

            if(target)
            {
                startPosition = base.transform.position;
                enemyPosition = target.transform.position;
                squallVFXComponent.PlayDashEffect(startPosition, enemyPosition);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            stopwatch += Time.fixedDeltaTime;

            base.rigidbodyMotor.moveVector = Vector3.zero;

            if(stopwatch >= attackInterval && target.healthComponent.alive && base.fixedAge >= diveDuration && !attackFinished)
            {
                stopwatch = 0f;

                if(NetworkServer.active)
                {
                    enemyPosition = target.transform.position;

                    DamageInfo info = new DamageInfo();
                    info.attacker = base.gameObject;
                    info.procCoefficient = 1f;
                    info.crit = isCrit;
                    info.position = enemyPosition;
                    info.damage = base.damageStat * damagePerHit;
                    target.healthComponent.TakeDamage(info);
                    GlobalEventManager.instance.OnHitEnemy(info, target.healthComponent.gameObject);
                    GlobalEventManager.instance.OnHitAll(info, target.healthComponent.gameObject);
                    batteryComponent.Recharge(isCrit ? (2f * chargePerHit) : chargePerHit);

                    EffectManager.SimpleImpactEffect(Assets.squallEvisEffect, enemyPosition, enemyPosition, true);
                    EffectManager.SimpleImpactEffect(GroundLight.comboHitEffectPrefab, enemyPosition, enemyPosition, true);
                }
            }else if ((!target.healthComponent.alive || base.fixedAge >= attackDuration + diveDuration) && base.isAuthority && !attackFinished)
            {
                squallVFXComponent.PlayDashEffect(enemyPosition, startPosition);
                attackFinished = true;
                stopwatch = 0f;
            }

            if(attackFinished && stopwatch >= diveDuration)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            characterModel.invisibilityCount--;
            batteryComponent.pauseDrain = false;
            squallVFXComponent.ToggleTrails(true);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
