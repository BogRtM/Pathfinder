using EntityStates;
using UnityEngine;
using UnityEngine.Networking;
using Pathfinder.Components;
using Pathfinder.Modules;
using RoR2.Projectile;
using RoR2;
using RoR2.Skills;

namespace Skillstates.Pathfinder
{
    internal class JavelinToss : BaseState
    {
        private Animator animator;
        private ChildLocator childLocator;
        private GameObject shaft;
        private GameObject spearhead;
        private Ray aimRay;
        private OverrideController controller;

        private float fireTime;
        private float throwForce = 150f;
        private bool hasFired = false;

        public static float baseDuration = 0.8f;
        public float duration;
        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / base.attackSpeedStat;
            fireTime = duration * 0.15f;
            base.StartAimMode(baseDuration + 0.1f, true);
            controller = base.GetComponent<OverrideController>();
            animator = base.GetModelAnimator();
            aimRay = base.GetAimRay();
            childLocator = base.GetModelChildLocator();

            shaft = childLocator.FindChild("Shaft").gameObject;
            spearhead = childLocator.FindChild("Spearhead").gameObject;

            if (animator)
            {
                if (!animator.GetBool("isMoving") && animator.GetBool("isGrounded"))
                {
                    base.PlayAnimation("FullBody, Override", "JavelinToss", "Hand.playbackRate", duration);
                }
                else
                {
                    base.PlayAnimation("Gesture, Override", "JavelinToss", "Hand.playbackRate", duration);
                }
            }

            controller.UnreadyJavelin();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(base.fixedAge >= fireTime && !hasFired)
            {
                shaft.SetActive(false);
                spearhead.SetActive(false);
                
                if(base.isAuthority) this.FireJavelin();
            }

            if (base.fixedAge >= this.duration)
            {
                base.outer.SetNextStateToMain();
            }
        }

        private void FireJavelin()
        {
            hasFired = true;
            FireProjectileInfo fireProjectileInfo = new FireProjectileInfo();
            fireProjectileInfo.crit = base.RollCrit();
            fireProjectileInfo.damage = Config.JavelinDamage.Value * base.damageStat;
            fireProjectileInfo.force = throwForce;
            fireProjectileInfo.owner = base.gameObject;
            fireProjectileInfo.position = aimRay.origin; //leftHand.position;
            fireProjectileInfo.rotation = Util.QuaternionSafeLookRotation(aimRay.direction);
            fireProjectileInfo.projectilePrefab = Projectiles.explodingJavelin; //Projectiles.javelinPrefab;
            ProjectileManager.instance.FireProjectile(fireProjectileInfo);
        }

        public override void OnExit()
        {
            shaft.SetActive(true);
            spearhead.SetActive(true);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}