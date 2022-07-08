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
        public static float baseDuration = 0.1f;

        public HealthComponent target;
        public bool isCrit;

        private BulletAttack attack;

        private Vector3 shootVector;

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / base.attackSpeedStat;

            shootVector = (target.body.corePosition - base.transform.position).normalized;

            if (base.isAuthority)
            {
                attack = new BulletAttack()
                {
                    owner = base.gameObject,
                    weapon = base.gameObject,
                    origin = base.transform.position,
                    muzzleName = "GunL",
                    minSpread = 0f,
                    maxSpread = 0f,
                    bulletCount = 2U,
                    damage = 0.2f * base.damageStat,
                    tracerEffectPrefab = FireBarrage.tracerEffectPrefab,
                    force = 1f,
                    hitEffectPrefab = FirePistol2.hitEffectPrefab,
                    isCrit = this.isCrit,
                    radius = 1f,
                    damageType = DamageType.Generic,
                    falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                    procCoefficient = 0.3f,
                    maxDistance = 200f,
                    aimVector = shootVector
                };

                FireBullet(attack);
            }
        }

        public void FireBullet(BulletAttack attack)
        {
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
