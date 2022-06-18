﻿using UnityEngine;
using EntityStates;
using EntityStates.Commando.CommandoWeapon;
using EntityStates.VoidSurvivor.Weapon;
using RoR2;

namespace Skillstates.Squall
{
    internal class MountedGuns : BaseState
    {
        public static float baseDuration = 0.3f;

        private BulletAttack attack;
        private BulletAttack rightAttack;

        private ChildLocator childLocator;
        private Ray aimRay;

        private float duration;
        private bool isCrit;

        public override void OnEnter()
        {
            base.OnEnter();
            childLocator = base.GetModelChildLocator();
            aimRay = base.GetAimRay();
            isCrit = base.RollCrit();
            duration = baseDuration / base.attackSpeedStat;

            if (base.isAuthority)
            {
                attack = new BulletAttack()
                {
                    owner = base.gameObject,
                    weapon = base.gameObject,
                    origin = aimRay.origin,
                    muzzleName = "GunL",
                    minSpread = 0f,
                    maxSpread = 0f,
                    bulletCount = 2U,
                    damage = 0.4f * base.damageStat,
                    tracerEffectPrefab = FireBarrage.tracerEffectPrefab,
                    force = 1f,
                    hitEffectPrefab = FirePistol2.hitEffectPrefab,
                    isCrit = this.isCrit,
                    radius = 2f,
                    damageType = DamageType.Generic,
                    falloffModel = BulletAttack.FalloffModel.None,
                    procCoefficient = 0.3f,
                    aimVector = aimRay.direction
                };

                FireBullet(attack);
            }
        }

        public void FireBullet(BulletAttack attack)
        {
            Util.PlaySound(FireBarrage.fireBarrageSoundString, base.gameObject);
            Util.PlaySound(FireBarrage.fireBarrageSoundString, base.gameObject);
            EffectManager.SimpleMuzzleFlash(FireBarrage.effectPrefab, base.gameObject, "GunL", false);
            EffectManager.SimpleMuzzleFlash(FireBarrage.effectPrefab, base.gameObject, "GunR", false);
            attack.Fire();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(base.fixedAge >= duration)
            {
                this.outer.SetNextStateToMain();
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
