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

        private BaseAI baseAI { get; set; }
        private EntityStateMachine weaponMachine;
        private EntityStateMachine bodyMachine;

        private bool inFollowMode;
        private bool inAttackMode;

        internal PathfinderController ownerController;

        internal List<string> followDrivers = Squall.followDrivers; //= new List<string>();
        internal List<string> attackDrivers = Squall.attackDrivers; //= new List<string>();

        private AISkillDriver[] aISkillDrivers;

        /*
        private void Awake()
        {
            followDrivers = Squall.followDrivers;
            attackDrivers = Squall.attackDrivers;
        }
        */

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
                baseAI.targetRefreshTimer = 5f;
                if(!inAttackMode) EnterAttackMode();
                baseAI.BeginSkillDriver(baseAI.EvaluateSkillDrivers());
            }
        }

        internal void EnterAttackMode()
        {
            inFollowMode = false;
            inAttackMode = true;
            Chat.AddMessage("Entering Attack Mode");
            foreach(AISkillDriver driver in aISkillDrivers)
            {
                if (!driver.enabled) continue;
                if (followDrivers.Contains(driver.customName))
                {
                    Log.Warning("Disabling driver: " + driver.customName);
                    driver.enabled = false;
                }
            }
        }

        internal void EnterFollowMode()
        {
            inAttackMode = false;
            inFollowMode = true;
            Chat.AddMessage("Entering Follow Mode");
            foreach (AISkillDriver driver in aISkillDrivers)
            {
                if (driver.enabled) continue;
                if (followDrivers.Contains(driver.customName))
                {
                    Log.Warning("Enabling driver: " + driver.customName);
                    driver.enabled = true;
                }
            }
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
