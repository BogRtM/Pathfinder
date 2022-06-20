using UnityEngine;
using EntityStates;
using EntityStates.Drone.DroneWeapon;
using RoR2;
using Pathfinder.Components;
using System;
using System.Linq;
using Pathfinder;

namespace Skillstates.Squall
{
    internal class MissileLauncher : BaseState
    {
        private GameObject target;
        private GameObject missilePrefab;
        private SquallController squallController;

        public static float baseDuration = 0.6f;
        public static int maxMissileCount = 4;

        private float duration;
        private float fireTimer;
        private float fireStopWatch;
        private int missileCount;
        private bool isCrit;
        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / base.attackSpeedStat;
            fireTimer = duration / (maxMissileCount - 1);
            isCrit = base.RollCrit();
            missilePrefab = GlobalEventManager.CommonAssets.missilePrefab;
            squallController = base.GetComponent<SquallController>();

            if(squallController)
            {
                target = squallController.GetCurrentTarget();
                if (target) Log.Warning("Firing missile at: " + target);
            }

            FireMissile();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            fireStopWatch += Time.fixedDeltaTime;
            if(fireStopWatch >= fireTimer && missileCount < maxMissileCount)
            {
                fireStopWatch = 0f;
                FireMissile();
            }

            if (base.fixedAge >= duration && missileCount >= maxMissileCount)
            {
                this.outer.SetNextStateToMain();
            }
        }

        private void FireMissile()
        {
            if (base.isAuthority)
            {
                MissileUtils.FireMissile(base.characterBody.corePosition, base.characterBody, default(ProcChainMask), target,
                    2.5f * base.damageStat, isCrit, missilePrefab, DamageColorIndex.Default, Vector3.up, 200f, false);
            }

            missileCount++;
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
