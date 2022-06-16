using EntityStates;
using EntityStates.Merc;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using Pathfinder.Misc;
using System;

namespace Pathfinder.SkillStates.Empower
{
    internal class Sweep : BaseState
    {
        private OverlapAttack sweepAttack;

        public static float baseDuration = 0.5f;

        private float fireTime;
        private float duration;
        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / base.attackSpeedStat;
            fireTime = duration * 0.1f;
            base.StartAimMode(duration + 0.1f, true);

            base.PlayAnimation("Gesture, Override", "Sweep", "Thrust.playbackRate", duration);
            Util.PlayAttackSpeedSound(GroundLight.finisherAttackSoundString, base.gameObject, GroundLight.slashPitch);

            Transform modelTransform = base.GetModelTransform();
            HitBoxGroup hitBoxGroup = null;

            if (modelTransform)
            {
                hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "Spear");
            }

            this.sweepAttack = new OverlapAttack();
            sweepAttack.attacker = base.gameObject;
            sweepAttack.inflictor = base.gameObject;
            sweepAttack.damageType = DamageType.Generic;
            sweepAttack.procCoefficient = 1f;
            sweepAttack.teamIndex = base.GetTeam();
            sweepAttack.isCrit = base.RollCrit();
            sweepAttack.forceVector = Vector3.zero;
            sweepAttack.pushAwayForce = 1f;
            sweepAttack.damage = 9f * base.damageStat;
            sweepAttack.hitBoxGroup = hitBoxGroup;
            sweepAttack.hitEffectPrefab = GroundLight.finisherHitEffectPrefab;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(base.fixedAge >= fireTime) sweepAttack.Fire();

            if (base.fixedAge >= duration)
            {
                base.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}