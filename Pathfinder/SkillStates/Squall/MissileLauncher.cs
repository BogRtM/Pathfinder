using UnityEngine;
using EntityStates;
using EntityStates.Drone.DroneWeapon;
using RoR2;
using RoR2.Orbs;
using Pathfinder.Components;
using Pathfinder.Modules;
using UnityEngine.Networking;

namespace Skillstates.Squall
{
    internal class MissileLauncher : BaseState
    {
        public HurtBox target;
        public bool isCrit;

        private SquallController squallController;

        public static float baseDuration = 0.5f;

        private float duration;
        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / base.attackSpeedStat;
            isCrit = base.RollCrit();
            squallController = base.GetComponent<SquallController>();

            if (!base.characterBody.isPlayerControlled)
                target = squallController.currentBestHurtbox;
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
            if (NetworkServer.active && target)
            {
                if(target.healthComponent.alive)
                {
                    MicroMissileOrb orb = new MicroMissileOrb();
                    orb.target = target;
                    orb.origin = base.characterBody.corePosition;
                    orb.damageValue = Config.SquallMissileDamage.Value * base.damageStat;
                    orb.attacker = base.gameObject;
                    orb.teamIndex = base.teamComponent.teamIndex;
                    orb.isCrit = isCrit;
                    orb.procChainMask = default(ProcChainMask);
                    orb.procCoefficient = 1f;
                    orb.damageColorIndex = DamageColorIndex.Default;
                    orb.damageType = DamageType.Generic;
                    OrbManager.instance.AddOrb(orb);
                }
            }

            EffectManager.SimpleMuzzleFlash(FireMissileBarrage.effectPrefab, base.gameObject, "MissileLauncher", true);
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
