using UnityEngine;
using RoR2;
using RoR2.CharacterAI;
using EntityStates.GolemMonster;
using System;
using System.Collections.Generic;

namespace Pathfinder.Components
{
    internal class SquallController : MonoBehaviour
    {

        private GameObject bodyPrefab;
        //private GameObject masterPrefab;

        private BaseAI baseAI;

        public List<AISkillDriver> followDrivers = new List<AISkillDriver>();

        private CharacterBody selfBody;

        private float maxAim = 1000f;

        protected void OnEnable()
        {
            bodyPrefab = base.GetComponent<CharacterMaster>().bodyPrefab;
            baseAI = base.GetComponent<BaseAI>();
            Hooks();
            EnterAttackMode();
        }

        private void EnterAttackMode()
        {
            foreach(AISkillDriver driver in followDrivers)
            {
                driver.enabled = false;
            }
        }

        private void Hooks()
        {
            CharacterMaster.onStartGlobal += CharacterMaster_onStartGlobal;
        }

        private void CharacterMaster_onStartGlobal(CharacterMaster obj)
        {
            if(obj.gameObject.GetComponent<SquallController>())
            {
                obj.GetBody().healthComponent.godMode = true;
            }
        }
    }
}
