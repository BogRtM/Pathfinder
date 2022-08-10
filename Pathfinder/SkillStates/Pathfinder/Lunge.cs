using UnityEngine;
using RoR2;
using EntityStates;

namespace Skillstates.Pathfinder
{
    internal class Lunge : BaseState
    {
        public static float giveUpDuration = 3f;
        public static float maxYAngle = 0f;
        public static float lungeSpeed = 70f;

        private Transform rootBone;

        private Ray aimRay;
        private Vector3 lungeDirection;
        public override void OnEnter()
        {
            base.OnEnter();
            aimRay = base.GetAimRay();
            lungeDirection = aimRay.direction;

            base.characterDirection.enabled = false;

            base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;

            base.PlayAnimation("FullBody, Override", "Lunge");

            base.characterMotor.velocity = Vector3.zero;
            base.characterMotor.velocity += lungeDirection * lungeSpeed;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            //base.GetModelTransform().rotation = Util.QuaternionSafeLookRotation(lungeDirection);
            //base.characterDirection.forward = lungeDirection;
            base.gameObject.transform.localRotation = Util.QuaternionSafeLookRotation(lungeDirection);

            if ((base.characterMotor.isGrounded || base.fixedAge >= giveUpDuration) && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.gameObject.transform.localRotation = Quaternion.identity;
            base.characterDirection.enabled = true;
            base.characterBody.bodyFlags &= CharacterBody.BodyFlags.IgnoreFallDamage;
            base.PlayAnimation("FullBody, Override", "BufferEmpty");
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}