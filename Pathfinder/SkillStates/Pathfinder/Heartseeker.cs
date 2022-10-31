/*
 * using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using RoR2;
using EntityStates;
using Pathfinder.Modules;
using Pathfinder.Components;

namespace Skillstates.Pathfinder
{
    internal class Heartseeker : BaseState
    {
        public static float baseDuration = 0.5f;
        public static float jumpPower = 3f;

        private Animator animator;

        private OverrideController overrideController;

        private Vector3 jumpVector;
        private Vector3 aimDirection;
        private Ray aimRay;
        public override void OnEnter()
        {
            base.OnEnter();

            animator = base.GetModelAnimator();

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

            base.PlayAnimation("FullBody, Override", "HS_Flip", "Flip.playbackRate", baseDuration);
            //PlayFlipAnimation();

            //base.StartAimMode(baseDuration + 0.1f, true);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(base.isAuthority && base.fixedAge >= baseDuration + 0.1f)
            {
                this.outer.SetNextStateToMain();
            }
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

        public void PlayFlipAnimation()
        {
            Vector3 cross = Vector3.Cross(aimDirection, Vector3.up);

            switch (Mathf.Round(Vector3.Dot(jumpVector, aimDirection)))
            {
                case 1:
                    Chat.AddMessage("Forward");
                    animator.SetFloat("jumpIndex", 0.2f);
                    break;

                case -1:
                    Chat.AddMessage("Backward");
                    animator.SetFloat("jumpIndex", 0.4f);
                    break;

                case 0:
                    {
                        switch (Mathf.Round(Vector3.Dot(jumpVector, cross)))
                        {
                            case 1:
                                Chat.AddMessage("Left");
                                animator.SetFloat("jumpIndex", 0.6f);
                                break;

                            case -1:
                                Chat.AddMessage("Right");
                                animator.SetFloat("jumpIndex", 0.8f);
                                break;

                            default:
                                break;
                        }

                        break;
                    }

                default:
                    Chat.AddMessage("Backward");
                    animator.SetFloat("jumpIndex", 0f);
                    break;
            }

            base.PlayCrossfade("FullBody, Override", "HS_Flip", "Flip.playbackRate", baseDuration, 0.1f);
        }
    }
}*/