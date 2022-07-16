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

        private BulletAttack attack;

        private Vector3 shootVector;
        private Ray aimRay;

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / base.attackSpeedStat;

            aimRay = base.GetAimRay();

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
                    damage = 0.2f * base.damageStat,
                    tracerEffectPrefab = FireBarrage.tracerEffectPrefab,
                    force = 1f,
                    hitEffectPrefab = FirePistol2.hitEffectPrefab,
                    stopperMask = LayerIndex.entityPrecise.mask,
                    isCrit = base.RollCrit(),
                    radius = 1f,
                    damageType = DamageType.Generic,
                    falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                    procCoefficient = 0.3f,
                    maxDistance = 200f,
                    aimVector = aimRay.direction
                };

                FireBullet(attack);
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
            EffectManager.SimpleMuzzleFlash(FireBarrage.effectPrefab, base.gameObject, "GunL", false);
            EffectManager.SimpleMuzzleFlash(FireBarrage.effectPrefab, base.gameObject, "GunR", false);
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
