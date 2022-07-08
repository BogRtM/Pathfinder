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

        public static float baseDuration = 0.1f;
        public static int maxMissileCount = 4;

        private float duration;
        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / base.attackSpeedStat;
            isCrit = base.RollCrit();
            missilePrefab = GlobalEventManager.CommonAssets.missilePrefab;

            FireMissile();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= duration)
            {
                this.outer.SetNextStateToMain();
            }
        }

        private void FireMissile()
        {
            if (base.isAuthority)
            {
                MissileUtils.FireMissile(base.characterBody.corePosition, base.characterBody, default(ProcChainMask), target,
                    1f * base.damageStat, isCrit, missilePrefab, DamageColorIndex.Default, Vector3.up, 200f, false);
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
