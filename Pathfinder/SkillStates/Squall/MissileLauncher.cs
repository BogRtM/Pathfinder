using UnityEngine;
using EntityStates;
using RoR2;
using Pathfinder.Components;
using Pathfinder.Modules;

namespace Skillstates.Squall
{
    internal class MissileLauncher : BaseState
    {
        public GameObject target;
        public bool isCrit;

        private GameObject missilePrefab;
        private SquallController squallController;

        public static float baseDuration = 1f;
        public static int maxMissileCount = 4;

        private float duration;
        private float fireTime;
        private float fireStopwatch;
        private int missileCount;
        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / base.attackSpeedStat;
            fireTime = duration / maxMissileCount;
            isCrit = base.RollCrit();
            missilePrefab = GlobalEventManager.CommonAssets.missilePrefab;
            squallController = base.GetComponent<SquallController>();

            target = squallController.currentTarget;

            FireMissile();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            fireStopwatch += Time.fixedDeltaTime;
            if(fireStopwatch >= fireTime && missileCount < maxMissileCount)
            {
                fireStopwatch = 0f;
                FireMissile();
            }

            if (base.fixedAge >= duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        private void FireMissile()
        {
            if (base.isAuthority)
            {
                MissileUtils.FireMissile(base.characterBody.corePosition, base.characterBody, default(ProcChainMask), target,
                    Config.SquallMissileDamage.Value * base.damageStat, isCrit, missilePrefab, DamageColorIndex.Default, Vector3.up, 200f, false);

                missileCount++;
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
