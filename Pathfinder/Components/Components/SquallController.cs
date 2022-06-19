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

        private List<AISkillDriver> followDrivers;
        private List<AISkillDriver> attackDrivers;

        protected void Start()
        {
            followDrivers = base.GetComponent<SquallAIModes>().followDrivers;
            attackDrivers = base.GetComponent<SquallAIModes>().attackDrivers;
            weaponMachine = EntityStateMachine.FindByCustomName(base.gameObject, "Weapon");
            masterPrefab = base.GetComponent<CharacterBody>().master.gameObject;
            baseAI = masterPrefab.GetComponent<BaseAI>();
            bodyMachine = EntityStateMachine.FindByCustomName(base.gameObject, "Body");
        }

        public void SetTarget(HurtBox target)
        {
            Log.Warning("Order received");
            HealthComponent healthComponent = target.healthComponent;
            GameObject bodyObject = healthComponent.gameObject;
            if(target && healthComponent && healthComponent.alive && bodyObject)
            {
                Log.Warning("Changing target to: " + bodyObject.name);
                baseAI.currentEnemy.gameObject = bodyObject;
                baseAI.currentEnemy.bestHurtBox = target;
                baseAI.enemyAttention = baseAI.enemyAttentionDuration;
                baseAI.targetRefreshTimer = 5f;
                EnterAttackMode();
                Log.Warning("Squall target is now: " + baseAI.currentEnemy.gameObject);
                baseAI.BeginSkillDriver(baseAI.EvaluateSkillDrivers());
            }
        }

        private void EnterAttackMode()
        {
            Log.Warning("Squall entering Attack Mode");
            foreach(AISkillDriver driver in followDrivers)
            {
                driver.enabled = false;
            }
        }
    }
}
