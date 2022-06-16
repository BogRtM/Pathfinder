using EntityStates;
using UnityEngine;
using Pathfinder.Misc;
using Pathfinder.Modules;
using RoR2.Projectile;
using RoR2;

namespace Pathfinder.SkillStates.Empower
{
    internal class JavelinToss : BaseState
    {
        private Animator animator;
        private ChildLocator childLocator;
        private GameObject shaft;
        private GameObject spearhead;
        private EmpowerComponent empowerComponent;
        private Ray aimRay;
        private Transform leftHand;

        private float fireTime;
        private float throwForce = 150f;
        private bool hasFired = false;

        public static float baseDuration = 0.8f;
        public float duration;
        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / base.attackSpeedStat;
            fireTime = duration * 0.2f;
            base.StartAimMode(baseDuration + 0.1f, true);

            animator = base.GetModelAnimator();
            aimRay = base.GetAimRay();
            childLocator = base.GetModelChildLocator();
            empowerComponent = base.GetComponent<EmpowerComponent>();

            leftHand = childLocator.FindChild("HandL");

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
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(base.fixedAge >= fireTime && !hasFired)
            {
                shaft.SetActive(false);
                spearhead.SetActive(false);
                this.FireJavelin();
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
            fireProjectileInfo.damage = 8f * base.damageStat;
            fireProjectileInfo.force = throwForce;
            fireProjectileInfo.owner = base.gameObject;
            fireProjectileInfo.position = leftHand.position;
            fireProjectileInfo.rotation = Util.QuaternionSafeLookRotation(aimRay.direction);
            fireProjectileInfo.projectilePrefab = Pathfinder.Modules.Projectiles.javelinPrefab;
            ProjectileManager.instance.FireProjectile(fireProjectileInfo);
        }

        public override void OnExit()
        {
            empowerComponent.ResetPrimary(base.skillLocator);
            shaft.SetActive(true);
            spearhead.SetActive(true);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}