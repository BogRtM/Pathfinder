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
        private GameObject masterPrefab;
        private TrailRenderer[] trails;

        private BaseAI baseAI { get; set; }
        private EntityStateMachine weaponMachine;
        private EntityStateMachine bodyMachine;
        private EntityStateMachine missileMachine;

        private bool inFollowMode;
        public bool inAttackMode;

        //internal PathfinderController ownerController;
        internal GameObject owner;

        internal List<string> followDrivers = Squall.followDrivers; //= new List<string>();
        internal List<string> attackDrivers = Squall.attackDrivers; //= new List<string>();

        private AISkillDriver[] aISkillDrivers;
        private void Awake()
        {
            trails = base.GetComponentsInChildren<TrailRenderer>();
        }

        private void Start()
        {
            masterPrefab = base.GetComponent<CharacterBody>().master.gameObject;
            aISkillDrivers = masterPrefab.GetComponents<AISkillDriver>();
            baseAI = masterPrefab.GetComponent<BaseAI>();
            weaponMachine = EntityStateMachine.FindByCustomName(base.gameObject, "Weapon");
            bodyMachine = EntityStateMachine.FindByCustomName(base.gameObject, "Body");
            missileMachine = EntityStateMachine.FindByCustomName(base.gameObject, "Missiles");

            EnterFollowMode();
        }

        internal void ShootTarget(HealthComponent victim, bool isCrit)
        {
            //weaponMachine.SetInterruptState(new MountedGuns() { target = victim, isCrit = isCrit }, EntityStates.InterruptPriority.Skill);
        }

        internal void ShootMissile(HealthComponent victim, bool isCrit)
        {
            missileMachine.SetInterruptState(new MissileLauncher() { target = victim.gameObject, isCrit = isCrit }, EntityStates.InterruptPriority.Any);
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
                    //Log.Warning("Disabling driver: " + driver.customName);
                    driver.enabled = false;
                }
            }
            /*
            foreach(var i in trails)
            {
                i.startColor = Color.red;
                i.endColor = Color.red;
            }
            */
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
                    //Log.Warning("Enabling driver: " + driver.customName);
                    driver.enabled = true;
                }
            }

            baseAI.currentEnemy.gameObject = null;
            baseAI.currentEnemy.bestHurtBox = null;
            baseAI.BeginSkillDriver(baseAI.EvaluateSkillDrivers());

            /*
            foreach(var i in trails)
            {
                i.startColor = Color.blue;
                i.endColor = Color.blue;
            }
            */
        }

        internal void DiveTarget(GameObject target)
        {
            bodyMachine.SetInterruptState(new DiveAttack() { target = target }, EntityStates.InterruptPriority.PrioritySkill);
        }

        internal void DoSpecialAttack(HurtBox target)
        {
            Log.Warning("Special order at squallcontroller");
            this.weaponMachine.SetInterruptState(new Piledriver() { target = target }, EntityStates.InterruptPriority.Pain);
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
