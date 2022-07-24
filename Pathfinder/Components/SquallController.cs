using UnityEngine;
using RoR2;
using RoR2.CharacterAI;
using System;
using System.Collections.Generic;
using Pathfinder.Content;
using Skillstates.Squall;
using RoR2.UI;
using RoR2.HudOverlay;
using System.Linq;

namespace Pathfinder.Components
{
    internal class SquallController : MonoBehaviour
    {
        internal GameObject owner { get; set; }
        
        private GameObject masterPrefab;
        private BaseAI baseAI;
        private AISkillDriver[] aISkillDrivers;
        internal GameObject currentTarget { get { return baseAI.currentEnemy.gameObject; } }

        private EntityStateMachine weaponMachine;
        private EntityStateMachine bodyMachine;
        internal SkillLocator skillLocator;

        internal bool inAttackMode { get { return attackMode; } }

        private bool attackMode = true;

        private SquallVFXComponent squallVFX;
        internal BatteryComponent batteryComponent;

        private OverlayController overlayController;

        private void Awake()
        {
            squallVFX = base.GetComponent<SquallVFXComponent>();
            skillLocator = base.GetComponent<SkillLocator>();
            batteryComponent = base.GetComponent<BatteryComponent>();
            //Hooks();
        }

        private void Start()
        {
            //owner = base.GetComponent<CharacterBody>().master.minionOwnership.ownerMaster.bodyInstanceObject;
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
            if (attackMode) return;

            attackMode = true;
            foreach(AISkillDriver driver in aISkillDrivers)
            {
                if (driver.enabled) continue;
                if (Squall.attackDrivers.Contains(driver.customName))
                {
                    driver.enabled = true;
                }
            }

            squallVFX.SetLineColor(Color.red);
            squallVFX.SetTrailColor(Color.red);

            batteryComponent.UpdateColor();
        }

        internal void DiveToPoint(Vector3 position, float minDistance)
        {
            this.bodyMachine.SetInterruptState(new DiveToPoint() { divePosition = position, minDistanceFromPoint = minDistance }, EntityStates.InterruptPriority.PrioritySkill);
        }

        internal void EnterFollowMode()
        {
            if (!attackMode) return;

            attackMode = false;
            foreach (AISkillDriver driver in aISkillDrivers)
            {
                if (!driver.enabled) continue;
                if (Squall.attackDrivers.Contains(driver.customName))
                {
                    driver.enabled = false;
                }
            }

            baseAI.currentEnemy.gameObject = null;
            baseAI.currentEnemy.bestHurtBox = null;
            baseAI.BeginSkillDriver(baseAI.EvaluateSkillDrivers());

            squallVFX.SetLineColor(Color.green);
            squallVFX.SetTrailColor(Color.green);
            if (batteryComponent)
            {
                batteryComponent.stopwatch = 0f;
                batteryComponent.UpdateColor();
            }
        }

        internal void DoSpecialAttack(HurtBox target)
        {
            if(this.skillLocator.special.ExecuteIfReady())
                this.bodyMachine.SetInterruptState(new SquallEvis() { target = target }, EntityStates.InterruptPriority.PrioritySkill);
        }

        #region UI
        private void FixedUpdate()
        {
            if (overlayController != null || !owner.GetComponent<CharacterBody>().isPlayerControlled) return;

            var ownerHUD = HUD.readOnlyInstanceList.Where(el => el.targetBodyObject == owner);
            foreach (HUD hud in ownerHUD)
            {
                Chat.AddMessage("Found Owner");

                AddSkillOverlay(hud);
                /*
                GameObject newTransform = UnityEngine.Object.Instantiate<GameObject>(skillIcon, scaler);
                newTransform.transform.Find("SkillBackgroundPanel").gameObject.SetActive(false);

                RectTransform oldRect = newTransform.GetComponent<RectTransform>();
                RectTransform newRect = newTransform.GetComponent<RectTransform>();

                newRect.anchoredPosition = new Vector2(oldRect.anchoredPosition.x, 115f);
                newRect.localScale = oldRect.localScale;

                newTransform.name = "SquallSpecialRoot";

                newTransform.GetComponent<SkillIcon>().targetSkill = skillLocator.special;
                */
            }
        }

        private void AddSkillOverlay(HUD hud)
        {
            Transform scaler = hud.transform.Find("MainContainer").Find("MainUIArea").Find("SpringCanvas").Find("BottomRightCluster").Find("Scaler");

            ChildLocator childLocator = hud.GetComponent<ChildLocator>();
            ChildLocator.NameTransformPair[] newArray = new ChildLocator.NameTransformPair[childLocator.transformPairs.Length + 1];
            childLocator.transformPairs.CopyTo(newArray, 0);
            newArray[newArray.Length - 1] = new ChildLocator.NameTransformPair
            {
                name = "Scaler",
                transform = scaler
            };
            childLocator.transformPairs = newArray;

            GameObject skillIcon = scaler.Find("Skill4Root").gameObject;

            OverlayCreationParams overlayCreationParams = new OverlayCreationParams()
            {
                prefab = skillIcon,
                childLocatorEntry = "Scaler"
            };

            overlayController = HudOverlayManager.AddOverlay(owner, overlayCreationParams);
            overlayController.onInstanceAdded += OverlayController_onInstanceAdded;
        }

        private void OverlayController_onInstanceAdded(OverlayController overlayController, GameObject instance)
        {
            instance.name = "SquallSpecialRoot";
            instance.transform.Find("SkillBackgroundPanel").gameObject.SetActive(false);
            instance.GetComponent<RectTransform>().anchoredPosition += new Vector2(0f, 130f);
            instance.GetComponent<SkillIcon>().targetSkill = skillLocator.special;
        }

        private void OnDisable()
        {
            if (overlayController != null)
            {
                overlayController.onInstanceAdded -= OverlayController_onInstanceAdded;
                HudOverlayManager.RemoveOverlay(overlayController);
            }
        }
        #endregion

        #region Junk
        /*
        private void Hooks()
        {
            On.RoR2.UI.HUD.Update += HUD_Update;
        }

        private void HUD_Update(On.RoR2.UI.HUD.orig_Update orig, HUD self)
        {
            orig(self);

            if (self.targetBodyObject == owner)
            {
                Chat.AddMessage("Found Owner");

                Transform scaler = self.transform.Find("MainContainer").Find("MainUIArea").Find("SpringCanvas").Find("BottomRightCluster").Find("Scaler");

                Transform skill4Transform = scaler.Find("Skill4Root");

                Transform newTransform = UnityEngine.Object.Instantiate<Transform>(skill4Transform, scaler);
                newTransform.Find("SkillBackgroundPanel").gameObject.SetActive(false);

                RectTransform oldRect = newTransform.GetComponent<RectTransform>();
                RectTransform newRect = newTransform.GetComponent<RectTransform>();

                newRect.anchoredPosition = new Vector2(oldRect.anchoredPosition.x, 115f);
                newRect.localScale = oldRect.localScale;

                newTransform.name = "SquallSpecialRoot";

                newTransform.GetComponent<SkillIcon>().targetSkill = skillLocator.special;

                On.RoR2.UI.HUD.Update -= this.HUD_Update;
            }
        }
        */
        #endregion
    }
}
