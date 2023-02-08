using EntityStates;
using EntityStates.Commando;
using UnityEngine;
using Pathfinder.Modules;
using Pathfinder.Components;
using RoR2;
using RoR2.Skills;
using RoR2.Items;

namespace Skillstates.Pathfinder
{
    internal class Evade : BaseState
    {
        public static float baseDuration = 0.2f;
        public static float speedCoefficient = 12f;
        public static float flatBonus = 65f;

        private float bonusFromMovespeed;
        private float totalDashBonus;

        public static SkillDef javelinSkill;

        private OverrideController controller;

        private GameObject dustPrefab;

        private Vector3 dashVector;

        public override void OnEnter()
        {
            base.OnEnter();
            controller = base.GetComponent<OverrideController>();
            dashVector = ((base.inputBank.moveVector == Vector3.zero) ? base.characterDirection.forward : base.inputBank.moveVector).normalized;
            base.characterDirection.forward = dashVector;
            base.characterMotor.velocity *= 0.1f;

            Util.PlaySound(SlideState.soundString, base.gameObject);

            dustPrefab = UnityEngine.Object.Instantiate<GameObject>(SlideState.slideEffectPrefab, base.FindModelChild("Pathfinder"));

            EffectData effectData = new EffectData()
            {
                origin = base.characterBody.corePosition,
                rotation = Util.QuaternionSafeLookRotation(dashVector)
            };

            EffectManager.SpawnEffect(Assets.dashEffect, effectData, false);

            if (!controller.javelinReady)
            {
                PlayAnimation("FullBody, Override", "GroundDashF", "Dash.playbackRate", baseDuration);
                controller.ReadyJavelin();
            } else
            {
                PlayAnimation("FullBody, Override", "JavGroundDash", "Dash.playbackRate", baseDuration);
            }

            if(base.moveSpeedStat <= 0f)
            {
                totalDashBonus = 0f;
            }
            else if (base.moveSpeedStat <= base.characterBody.baseMoveSpeed)
            {
                totalDashBonus = flatBonus * (base.moveSpeedStat / base.characterBody.baseMoveSpeed);
            }
            else
            {
                float bonusMS = base.moveSpeedStat - base.characterBody.baseMoveSpeed;
                bonusFromMovespeed = (bonusMS * base.characterBody.baseMoveSpeed) / (base.moveSpeedStat);
                totalDashBonus = speedCoefficient * bonusFromMovespeed + flatBonus;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(base.isAuthority)
            {
                base.characterDirection.forward = dashVector;
                base.characterMotor.rootMotion += dashVector * totalDashBonus * Time.fixedDeltaTime;
                base.characterMotor.velocity.y = 0f;
            }

            if(base.fixedAge >= baseDuration && base.isAuthority)
            {
                base.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            if (dustPrefab) UnityEngine.Object.Destroy(dustPrefab);
            PlayCrossfade("FullBody, Override", "BufferEmpty", 0.2f);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}