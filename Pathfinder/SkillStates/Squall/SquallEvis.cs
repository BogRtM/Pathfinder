using EntityStates;
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
        public static float duration = 1.3f;
        public static float baseAttackInterval = 0.185f;

        internal HurtBox target;

        private Transform modelTransform;
        private CharacterModel characterModel;
        private SquallController squallController;

        private float attackInterval;
        private float stopwatch;

        private bool isCrit;

        public override void OnEnter()
        {
            base.OnEnter();
            attackInterval = baseAttackInterval / base.attackSpeedStat;
            isCrit = base.RollCrit();
            modelTransform = base.GetModelTransform();
            characterModel = modelTransform.GetComponent<CharacterModel>();
            squallController = base.GetComponent<SquallController>();

            characterModel.invisibilityCount++;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            stopwatch += Time.fixedDeltaTime;

            if(stopwatch >= attackInterval && target.healthComponent.alive)
            {
                stopwatch = 0f;

                if(NetworkServer.active)
                {
                    DamageInfo info = new DamageInfo();
                    info.attacker = squallController.owner;
                    info.procCoefficient = 1f;
                    info.crit = isCrit;
                    info.position = target.transform.position;
                    info.damage = base.damageStat * 0.7f;
                    info.AddModdedDamageType(PathfinderPlugin.marking);
                    target.healthComponent.TakeDamage(info);
                    GlobalEventManager.instance.OnHitEnemy(info, target.healthComponent.gameObject);
                    GlobalEventManager.instance.OnHitAll(info, target.healthComponent.gameObject);

                    EffectManager.SimpleEffect(Assets.squallEvisEffect, target.transform.position, Quaternion.identity, true);
                }
            }else if ((!target.healthComponent.alive || base.fixedAge >= duration) && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            characterModel.invisibilityCount--;
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
