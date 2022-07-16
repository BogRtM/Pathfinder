using UnityEngine;
using RoR2;
using RoR2.CharacterAI;
using System;
using System.Collections.Generic;
using Pathfinder.Content;
using Skillstates.Squall;

namespace Pathfinder.Components
{
    internal class SquallController : MonoBehaviour
    {
        internal GameObject owner { get; set; }
        private GameObject masterPrefab;

        private BaseAI baseAI;
        internal GameObject currentTarget { get { return baseAI.currentEnemy.gameObject; } }

        private EntityStateMachine weaponMachine;
        private EntityStateMachine bodyMachine;
        //private EntityStateMachine missileMachine;
        internal SkillLocator skillLocator;

        private bool attackMode;

        internal bool inAttackMode { get { return attackMode; } }

        private SquallVFXComponent squallVFX;

        internal List<string> followDrivers = Squall.followDrivers;
        internal List<string> attackDrivers = Squall.attackDrivers;

        private AISkillDriver[] aISkillDrivers;
        private void Awake()
        {
            squallVFX = base.GetComponent<SquallVFXComponent>();
            skillLocator = base.GetComponent<SkillLocator>();
        }

        private void Start()
        {
            masterPrefab = base.GetComponent<CharacterBody>().master.gameObject;
            aISkillDrivers = masterPrefab.GetComponents<AISkillDriver>();
            baseAI = masterPrefab.GetComponent<BaseAI>();
            weaponMachine = EntityStateMachine.FindByCustomName(base.gameObject, "Weapon");
            bodyMachine = EntityStateMachine.FindByCustomName(base.gameObject, "Body");
            EnterFollowMode();
        }

        internal void SetTarget(HurtBox target)
        {
            HealthComponent healthComponent = target.healthComponent;
            GameObject bodyObject = healthComponent.gameObject;
            if(target && healthComponent && healthComponent.alive && bodyObject)
            {
                baseAI.currentEnemy.gameObject = bodyObject;
                baseAI.currentEnemy.bestHurtBox = target;
                baseAI.enemyAttention = baseAI.enemyAttentionDuration;
                baseAI.BeginSkillDriver(baseAI.EvaluateSkillDrivers());
            }
        }

        internal void EnterAttackMode()
        {
            if (inAttackMode) return;

            attackMode = true;
            Chat.AddMessage("Entering Attack Mode");
            foreach(AISkillDriver driver in aISkillDrivers)
            {
                if (driver.enabled) continue;
                if (attackDrivers.Contains(driver.customName))
                {
                    driver.enabled = true;
                }
            }

            squallVFX.SetTrailColor(Color.red);
        }

        internal void DiveToPoint(Vector3 position)
        {
            this.bodyMachine.SetInterruptState(new DivePoint() { divePosition = position }, EntityStates.InterruptPriority.PrioritySkill);
        }

        internal void EnterFollowMode()
        {
            attackMode = false;
            Chat.AddMessage("Entering Follow Mode");
            foreach (AISkillDriver driver in aISkillDrivers)
            {
                if (!driver.enabled) continue;
                if (attackDrivers.Contains(driver.customName))
                {
                    driver.enabled = false;
                }
            }

            baseAI.currentEnemy.gameObject = null;
            baseAI.currentEnemy.bestHurtBox = null;
            baseAI.BeginSkillDriver(baseAI.EvaluateSkillDrivers());

            squallVFX.SetTrailColor(Color.blue);
        }

        internal void DoSpecialAttack(HurtBox target)
        {
            this.skillLocator.special.ExecuteIfReady();
            this.bodyMachine.SetInterruptState(new SquallEvis() { target = target }, EntityStates.InterruptPriority.PrioritySkill);
        }

        internal GameObject GetCurrentTarget()
        {
            if(baseAI)
            {
                GameObject target = baseAI.currentEnemy.gameObject;
                if (target) return target;
            }
            return null;
        }
    }
}
