using UnityEngine;
using EntityStates;
using EntityStates.Commando.CommandoWeapon;
using EntityStates.VoidSurvivor.Weapon;
using RoR2;
using Pathfinder.Modules;
using Pathfinder;

namespace Skillstates.Squall
{
    internal class MountedGuns : BaseState
    {
        public static float baseDuration = 0.2f;

        internal HealthComponent target;

        private BulletAttack leftAttack;
        private BulletAttack rightAttack;

        private Ray aimRay;

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / base.attackSpeedStat;

            aimRay = base.GetAimRay();

            if (base.isAuthority)
            {
                leftAttack = new BulletAttack()
                {
                    owner = base.gameObject,
                    weapon = base.gameObject,
                    origin = aimRay.origin,
                    muzzleName = "GunL",
                    minSpread = 0f,
                    maxSpread = 0f,
                    bulletCount = 1U,
                    damage = Config.SquallGunDamage.Value * base.damageStat,
                    tracerEffectPrefab = FireBarrage.tracerEffectPrefab,
                    force = 1f,
                    hitEffectPrefab = FirePistol2.hitEffectPrefab,
                    stopperMask = LayerIndex.entityPrecise.mask,
                    isCrit = base.RollCrit(),
                    radius = 1f,
                    damageType = DamageType.Generic,
                    falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                    procCoefficient = Config.SquallGunProc.Value,
                    maxDistance = 200f,
                    aimVector = aimRay.direction
                };

                rightAttack = new BulletAttack()
                {
                    owner = base.gameObject,
                    weapon = base.gameObject,
                    origin = aimRay.origin,
                    muzzleName = "GunR",
                    minSpread = 0f,
                    maxSpread = 0f,
                    bulletCount = 1U,
                    damage = Config.SquallGunDamage.Value * base.damageStat,
                    tracerEffectPrefab = FireBarrage.tracerEffectPrefab,
                    force = 1f,
                    hitEffectPrefab = FirePistol2.hitEffectPrefab,
                    stopperMask = LayerIndex.entityPrecise.mask,
                    isCrit = base.RollCrit(),
                    radius = 1f,
                    damageType = DamageType.Generic,
                    falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                    procCoefficient = Config.SquallGunProc.Value,
                    maxDistance = 200f,
                    aimVector = aimRay.direction
                };

                FireBullet(leftAttack);
                FireBullet(rightAttack);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public void FireBullet(BulletAttack attack)
        {
            Util.PlaySound(FireBarrage.fireBarrageSoundString, base.gameObject);
            EffectManager.SimpleMuzzleFlash(FireBarrage.effectPrefab, base.gameObject, attack.muzzleName, false);
            
            if(base.isAuthority)
                attack.Fire();
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
