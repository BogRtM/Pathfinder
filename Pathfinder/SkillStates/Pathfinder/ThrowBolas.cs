using EntityStates;
using UnityEngine;
using RoR2;
using RoR2.Projectile;
using Pathfinder.Components;
using Pathfinder.Modules;

namespace Skillstates.Pathfinder
{
    internal class ThrowBolas : BaseState
    {
        private GameObject bolas;

        public static float loopDuration = 0.4f;
        public static float throwForce = 60f;

        private OverrideController controller;
        private ChildLocator childLocator;
        private Ray aimRay;

        private string javReady = "";

        public override void OnEnter()
        {
            base.OnEnter();
            controller = base.GetComponent<OverrideController>();
            childLocator = base.GetModelChildLocator();
            if (controller.javelinReady) javReady = "Jav";
            bolas = childLocator.FindChild("Bolas").gameObject;
            bolas.SetActive(true);

            base.PlayAnimation("Gesture, Override", javReady + "BolasLoop", "Hand.playbackRate", loopDuration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            base.StartAimMode(0.1f, false);

            if (base.isAuthority && !base.inputBank.skill3.down)
            {
                this.outer.SetNextStateToMain();
            }
        }
        private void FireBolas()
        {
            aimRay = base.GetAimRay();
            
            FireProjectileInfo fireProjectileInfo = new FireProjectileInfo();
            fireProjectileInfo.crit = false;
            fireProjectileInfo.damage = 0f;
            fireProjectileInfo.force = throwForce;
            fireProjectileInfo.owner = base.gameObject;
            fireProjectileInfo.position = aimRay.origin;
            fireProjectileInfo.rotation = Util.QuaternionSafeLookRotation(aimRay.direction);
            fireProjectileInfo.projectilePrefab = Projectiles.bolas;
            ProjectileManager.instance.FireProjectile(fireProjectileInfo);
        }

        public override void OnExit()
        {
            base.PlayAnimation("Gesture, Override", javReady + "BolasThrow", "Hand.playbackRate", loopDuration);
            FireBolas();
            bolas.SetActive(false);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
