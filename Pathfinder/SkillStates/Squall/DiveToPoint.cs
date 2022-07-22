using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RoR2;
using R2API;
using EntityStates;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Pathfinder;
using Pathfinder.Components;

namespace Skillstates.Squall
{
    internal class DiveToPoint : BaseState
    {
        public static float giveUpDuration = 3f;
        public static float speedCoefficient = 6f;

        public Vector3 divePosition;

        public float minDistanceFromPoint;

        public override void OnEnter()
        {
            base.OnEnter();
            base.characterBody.isSprinting = true;
            if (minDistanceFromPoint <= 0f) minDistanceFromPoint = 1f;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            bool flag1 = Vector3.Distance(divePosition, base.transform.position) <= minDistanceFromPoint;
            bool flag2 = base.fixedAge >= giveUpDuration;
            
            if ((flag1 || flag2) && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
             
            if (base.isAuthority)
            {
                base.rigidbodyDirection.aimDirection = (divePosition - base.transform.position).normalized;
                base.rigidbodyMotor.rootMotion += (divePosition - base.transform.position).normalized * (speedCoefficient * base.moveSpeedStat * Time.fixedDeltaTime);
            }
        }

        public override void OnExit()
        {
            base.characterBody.isSprinting = false;
            base.PlayCrossfade("FullBody, Override", "BufferEmpty", 0.2f);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
