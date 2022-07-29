using UnityEngine;
using RoR2;
using RoR2.CharacterAI;
using System;
using System.Collections.Generic;
using Pathfinder.Modules.NPC;
using Skillstates.Squall;
using RoR2.UI;
using RoR2.HudOverlay;
using System.Linq;

namespace Pathfinder.Components
{
    internal class SquallController : MonoBehaviour
    {
        internal GameObject owner;
        
        private GameObject masterObject;
        private BaseAI baseAI;
        private AISkillDriver[] aISkillDrivers;
        internal GameObject currentTarget { get { return baseAI.currentEnemy.gameObject; } }

        private EntityStateMachine weaponMachine;
        private EntityStateMachine bodyMachine;
        internal SkillLocator skillLocator;

        internal bool inAttackMode { get { return attackMode; } }

        private bool attackMode = true;

        private SquallVFXComponents squallVFX;
        internal BatteryComponent batteryComponent;

        internal OverlayController overlayController;
        private GameObject overlayInstance;

        private CharacterBody selfBody;

        internal Highlight targetHighlight;

        private bool hasRiskUI;

        private void Awake()
        {
            squallVFX = base.GetComponent<SquallVFXComponents>();
            skillLocator = base.GetComponent<SkillLocator>();
            batteryComponent = base.GetComponent<BatteryComponent>();
        }

        private void Start()
        {
            selfBody = base.GetComponent<CharacterBody>();
            masterObject = selfBody.master.gameObject;
            aISkillDrivers = masterObject.GetComponents<AISkillDriver>();
            baseAI = masterObject.GetComponent<BaseAI>();
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
                CreateHighlight(bodyObject);

                baseAI.currentEnemy.gameObject = bodyObject;
                baseAI.currentEnemy.bestHurtBox = target;
                baseAI.enemyAttention = 1f; //baseAI.enemyAttentionDuration;
                baseAI.BeginSkillDriver(baseAI.EvaluateSkillDrivers());
            }
        }

        internal void EnterAttackMode()
        {
            if (Util.HasEffectiveAuthority(owner))
            {
                Util.PlaySound("BeepAttack", base.gameObject);
                EffectManager.SimpleEffect(Modules.Assets.squallAttackFlash, base.transform.position, Quaternion.identity, false);
            }

            if (attackMode) return;

            attackMode = true;

            foreach (AISkillDriver driver in aISkillDrivers)
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

        internal void EnterFollowMode()
        {
            if (Util.HasEffectiveAuthority(owner))
            {
                Util.PlaySound("BeepFollow", base.gameObject);
                EffectManager.SimpleEffect(Modules.Assets.squallFollowFlash, base.transform.position, Quaternion.identity, false);
            }

            if (!attackMode) return;

            attackMode = false;

            if (targetHighlight) UnityEngine.Object.Destroy(targetHighlight);

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

        internal void DiveToPoint(Vector3 position, float minDistance)
        {
            this.bodyMachine.SetInterruptState(new DiveToPoint() { divePosition = position, minDistanceFromPoint = minDistance }, EntityStates.InterruptPriority.PrioritySkill);
        }

        internal void DoSpecialAttack(HurtBox target)
        {
            if(this.skillLocator.special.ExecuteIfReady())
                this.bodyMachine.SetInterruptState(new SquallEvis() { target = target }, EntityStates.InterruptPriority.PrioritySkill);
        }

        #region UI
        private void FixedUpdate()
        {
            if (selfBody.isPlayerControlled) return;

            if(hasRiskUI && overlayController != null && overlayInstance)
            {
                if(overlayInstance.GetComponent<RectTransform>().anchoredPosition3D != new Vector3(355f, -155f))
                {
                    //Chat.AddMessage("Readjusting icon transform");
                    overlayInstance.GetComponent<RectTransform>().anchoredPosition = new Vector3(355f, -155f);
                    overlayInstance.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(355f, -155f, 0f);
                }
            }

            if (overlayController != null || !owner.GetComponent<CharacterBody>().isPlayerControlled) return;

            var ownerHUD = HUD.readOnlyInstanceList.Where(el => el.targetBodyObject == owner);
            foreach (HUD hud in ownerHUD)
            {
                AddSkillOverlay(hud);
            }
        }

        private void CreateHighlight(GameObject target)
        {
            if (targetHighlight) UnityEngine.Object.Destroy(targetHighlight);

            if (target.GetComponent<Highlight>()) return;

            var modelLocator = target.GetComponent<ModelLocator>();
            if (modelLocator)
            {
                var modelTransform = modelLocator.modelTransform;
                if (modelTransform)
                {
                    var characterModel = modelTransform.GetComponent<CharacterModel>();
                    if (characterModel)
                    {
                        foreach (CharacterModel.RendererInfo rendererInfo in characterModel.baseRendererInfos)
                        {
                            if (!rendererInfo.ignoreOverlays)
                            {
                                targetHighlight = target.AddComponent<Highlight>();
                                targetHighlight.highlightColor = Highlight.HighlightColor.teleporter;
                                targetHighlight.strength = 1.5f;
                                targetHighlight.targetRenderer = rendererInfo.renderer;
                                targetHighlight.isOn = true;
                                return;
                            }
                        }
                    }
                }
            }
        }

        private void AddSkillOverlay(HUD hud)
        {
            GameObject skill4root = Array.Find<SkillIcon>(hud.skillIcons, icon => icon.name == "Skill4Root").gameObject;

            ChildLocator childLocator = hud.GetComponent<ChildLocator>();
            ChildLocator.NameTransformPair[] newArray = new ChildLocator.NameTransformPair[childLocator.transformPairs.Length + 1];
            childLocator.transformPairs.CopyTo(newArray, 0);
            newArray[newArray.Length - 1] = new ChildLocator.NameTransformPair
            {
                name = skill4root.transform.parent.name,
                transform = skill4root.transform.parent
            };
            childLocator.transformPairs = newArray;

            OverlayCreationParams overlayCreationParams = new OverlayCreationParams()
            {
                prefab = skill4root,
                childLocatorEntry = skill4root.transform.parent.name
            };

            overlayController = HudOverlayManager.AddOverlay(owner, overlayCreationParams);
            overlayController.onInstanceAdded += OverlayController_onInstanceAdded;
        }

        private void OverlayController_onInstanceAdded(OverlayController overlayController, GameObject instance)
        {
            overlayInstance = instance;

            if(overlayController.creationParams.childLocatorEntry == "SkillIconContainer")
            {
                hasRiskUI = true;
                instance.transform.Find("BottomContainer").Find("SkillBackgroundPanel").gameObject.SetActive(false);
                instance.GetComponent<RectTransform>().anchoredPosition += new Vector2(80f, 0f);
            } 
            else
            {
                instance.transform.Find("SkillBackgroundPanel").gameObject.SetActive(false);
                instance.GetComponent<RectTransform>().anchoredPosition += new Vector2(0f, 130f);
            }

            instance.name = "SquallSpecialRoot";
            
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
