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
    internal class DivePoint : BaseState
    {
        public static float giveUpDuration = 3f;
        public static float speedCoefficient = 6f;

        //public HurtBox target;
        public Vector3 divePosition;

        private bool startDive;

        public override void OnEnter()
        {
            base.OnEnter();

            //base.PlayAnimation("FullBody, Override", "DiveLoop");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(base.isAuthority)
            {
                base.rigidbodyDirection.aimDirection = (divePosition - base.transform.position).normalized;
                base.rigidbodyMotor.rootMotion += (divePosition - base.transform.position).normalized * (speedCoefficient * base.moveSpeedStat * Time.fixedDeltaTime);
            }
            
            if(Vector3.Distance(divePosition, base.transform.position) <= 2f && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            } 
            else if(base.fixedAge >= giveUpDuration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.PlayCrossfade("FullBody, Override", "BufferEmpty", 0.2f);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
