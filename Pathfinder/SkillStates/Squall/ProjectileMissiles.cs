using UnityEngine;
using EntityStates;
using RoR2;
using Pathfinder.Components;
using Pathfinder.Modules;

namespace Skillstates.Squall
{
    internal class ProjectileMissiles : BaseState
    {
        public GameObject target;
        public bool isCrit;

        private GameObject missilePrefab;
        private SquallController squallController;

        public static float baseDuration = 0.5f;

        private float duration;
        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / base.attackSpeedStat;
            isCrit = base.RollCrit();
            missilePrefab = GlobalEventManager.CommonAssets.missilePrefab;
            squallController = base.GetComponent<SquallController>();

            if (!base.characterBody.isPlayerControlled)
                target = squallController.currentTarget;
            else
                target = null;

            FireMissile();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

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
            }
        }

        public override void OnExit()
        {
            FireMissile();
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
