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
using System.Linq;

namespace Skillstates.Squall
{
    public class SquallEvis : BaseState
    {
        public static float diveDuration = 0.2f;
        public static float attackDuration = 1.3f;
        public static float baseAttackInterval = 0.129f;
        public static float damageCoefficient = Config.specialDamageCoefficient.Value;
        public static float chargePerHit = Config.specialRechargeAmount.Value;

        internal HurtBox target;

        private Transform modelTransform;
        private CharacterModel characterModel;
        private BatteryComponent batteryComponent;
        private SquallVFXComponents squallVFXComponent;

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
            squallVFXComponent = base.GetComponent<SquallVFXComponents>();

            batteryComponent.pauseDrain = true;
            squallVFXComponent.ToggleVFX(false);

            characterModel.invisibilityCount++;

            if (target)
            {
                startPosition = base.transform.position;
                enemyPosition = target.transform.position;
                squallVFXComponent.PlayDashEffect(startPosition, enemyPosition);
                Util.PlaySound(EvisDash.endSoundString, base.gameObject);
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
                    info.damage = base.damageStat * damageCoefficient;
                    info.AddModdedDamageType(PathfinderPlugin.shredding);

                    target.healthComponent.TakeDamage(info);
                    GlobalEventManager.instance.OnHitEnemy(info, target.healthComponent.gameObject);
                    GlobalEventManager.instance.OnHitAll(info, target.healthComponent.gameObject);
                    
                    float chargeAmount = isCrit ? (2f * chargePerHit) : chargePerHit;
                    batteryComponent.Recharge(chargeAmount, true);

                    //GroundLight.comboHitEffectPrefab
                    EffectManager.SimpleImpactEffect(Assets.squallEvisEffect, enemyPosition, enemyPosition, true);
                    EffectManager.SimpleImpactEffect(Assaulter.hitEffectPrefab, enemyPosition, enemyPosition, true);
                }
            }
            else if ((!target.healthComponent.alive || base.fixedAge >= attackDuration + diveDuration) && base.isAuthority && !attackFinished)
            {
                FinishAttack();
            }

            if(attackFinished && stopwatch >= diveDuration)
            {
                this.outer.SetNextStateToMain();
            }
        }

        private void FinishAttack()
        {
            squallVFXComponent.PlayDashEffect(enemyPosition, startPosition);
            Util.PlaySound(EvisDash.endSoundString, base.gameObject);
            attackFinished = true;
            stopwatch = 0f;
        }

        public override void OnExit()
        {
            characterModel.invisibilityCount--;
            batteryComponent.pauseDrain = false;
            squallVFXComponent.ToggleVFX(true);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
