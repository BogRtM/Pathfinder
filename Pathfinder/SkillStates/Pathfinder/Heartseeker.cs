using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using RoR2;
using EntityStates;
using Pathfinder.Modules;

namespace Skillstates.Pathfinder
{
    internal class Heartseeker : BaseState
    {
        public static float baseDuration = 1f;
        public static float jumpPower = 3f;

        private Vector3 jumpVector;
        private Vector3 aimDirection;
        private Ray aimRay;
        public override void OnEnter()
        {
            base.OnEnter();

            aimRay = base.GetAimRay();
            aimDirection = aimRay.direction;
            aimDirection.y = 0f;

            jumpVector = ((base.inputBank.moveVector == Vector3.zero) ? base.characterDirection.forward : base.inputBank.moveVector).normalized;

            if (NetworkServer.active)
                base.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);

            jumpVector += Vector3.up;
            if (base.isAuthority)
            {
                base.characterMotor.Motor.ForceUnground();
                base.characterMotor.velocity.y = 0f;
                base.characterMotor.velocity += jumpVector * base.moveSpeedStat * jumpPower;
            }

            EffectData effectData = new EffectData
            {
                origin = base.characterBody.footPosition
            };

            EffectManager.SpawnEffect(Assets.vaultEffect, effectData, false);

            base.StartAimMode(baseDuration + 0.1f, true);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= baseDuration && base.isAuthority)
                this.outer.SetNextState(new Lunge());
        }

        public override void OnExit()
        {
            if (NetworkServer.active)
                base.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);

            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        public void GetInputDirection()
        {
            Vector3 cross = Vector3.Cross(aimDirection, Vector3.up);

            switch (Mathf.Round(Vector3.Dot(jumpVector, aimDirection)))
            {
                case 1:
                    Chat.AddMessage("Forward");
                    break;

                case -1:
                    Chat.AddMessage("Backward");
                    break;

                case 0:
                    {
                        switch (Mathf.Round(Vector3.Dot(jumpVector, cross)))
                        {
                            case 1:
                                Chat.AddMessage("Left");
                                break;

                            case -1:
                                Chat.AddMessage("Right");
                                break;

                            default:
                                break;
                        }

                        break;
                    }

                default:
                    break;
            }
        }
    }
}