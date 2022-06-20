using UnityEngine;
using RoR2;
using RoR2.CharacterAI;
using System;
using System.Collections.Generic;
using Pathfinder.Content;

namespace Pathfinder.Components
{
    internal class SquallController : MonoBehaviour
    {
        private GameObject masterPrefab;

        private BaseAI baseAI;
        private EntityStateMachine weaponMachine;
        private EntityStateMachine bodyMachine;

        public List<string> followDrivers = new List<string>();
        public List<string> attackDrivers = new List<string>();

        private AISkillDriver[] aISkillDrivers;

        protected void Start()
        {
            masterPrefab = base.GetComponent<CharacterBody>().master.gameObject;
            aISkillDrivers = masterPrefab.GetComponents<AISkillDriver>();
            baseAI = masterPrefab.GetComponent<BaseAI>();
            weaponMachine = EntityStateMachine.FindByCustomName(base.gameObject, "Weapon");
            bodyMachine = EntityStateMachine.FindByCustomName(base.gameObject, "Body");
        }

        public void SetTarget(HurtBox target)
        {
            HealthComponent healthComponent = target.healthComponent;
            GameObject bodyObject = healthComponent.gameObject;
            if(target && healthComponent && healthComponent.alive && bodyObject)
            {
                baseAI.currentEnemy.gameObject = bodyObject;
                baseAI.currentEnemy.bestHurtBox = target;
                baseAI.enemyAttention = baseAI.enemyAttentionDuration;
                baseAI.targetRefreshTimer = 5f;
                EnterAttackMode();
                baseAI.BeginSkillDriver(baseAI.EvaluateSkillDrivers());
            }
        }

        private void EnterAttackMode()
        {
            Log.Warning("Squall entering Attack Mode");
            foreach(AISkillDriver driver in aISkillDrivers)
            {
                if (followDrivers.Contains(driver.customName))
                {
                    Log.Warning("Disabling driver: " + driver.customName);
                    driver.enabled = false;
                }
            }
        }
    }
}
