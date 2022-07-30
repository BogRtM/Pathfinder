using UnityEngine;
using EntityStates;
using RoR2;
using Pathfinder.Components;
using Pathfinder.Modules;

namespace Skillstates.Squall
{
    internal class ReworkedMissiles : BaseState
    {
        public GameObject target;
        public bool isCrit;

        private GameObject missilePrefab;
        private SquallController squallController;

        public static float baseDuration = 1f;
        public static int maxMissileCount = 4;

        private float duration;
        private float halfwayPoint;
        private bool pastHalfway;
        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / base.attackSpeedStat;
            halfwayPoint = duration / 2f;
            isCrit = base.RollCrit();
            missilePrefab = GlobalEventManager.CommonAssets.missilePrefab;
            squallController = base.GetComponent<SquallController>();

            if (!base.characterBody.isPlayerControlled)
                target = squallController.currentTarget;
            else
                target = null;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(base.fixedAge >= halfwayPoint && !pastHalfway)
            {
                pastHalfway = true;
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
            }
        }

        public override void OnExit()
        {
            FireMissile();
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
