using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using RoR2.UI;
using RoR2.HudOverlay;
using UnityEngine.UI;
using System.Linq;

namespace Pathfinder.Components
{
    [RequireComponent(typeof(SquallController))]
    internal class BatteryComponent : NetworkBehaviour
    {
        public float maxCharge = 100f;
        public float maxOvercharge = 20f;

        [SyncVar]
        private float currentCharge;

        public static float drainRate = Modules.Config.batteryDrainRate.Value;
        public static float baseRechargeRate = Modules.Config.batteryRechargeRate.Value;
        public static float rechargeDelay = 1f;

        internal float rechargeRate;
        internal float stopwatch;

        private OverlayController overlayController;

        private Image batteryMeter;
        //private Image batteryRings;
        //private Image batteryGlow;
        //private Image batteryPip;
        private TMPro.TextMeshProUGUI batteryText;

        private Color followColor = new Color(0f, 1f, 0f, 0.6f);
        private Color attackColor = new Color(1f, 0f, 0f, 0.6f);

        internal bool pauseDrain;
        private bool allCreated;

        internal SquallController squallController;

        private CharacterBody selfBody;

        private void Awake()
        {
            //Do not set currentCharge to 0 on Awake
            currentCharge = 1f;
            stopwatch = rechargeDelay;
            squallController = base.GetComponent<SquallController>();
            //Hooks();
        }

        private void Start()
        {
            selfBody = base.GetComponent<CharacterBody>();
            rechargeRate = baseRechargeRate * selfBody.attackSpeed;
        }

        private void FixedUpdate()
        {
            if (selfBody)
            {
                if (selfBody.isPlayerControlled)
                {
                    UnityEngine.Object.Destroy(this);
                }
            }

            if ((squallController.inAttackMode || currentCharge > maxCharge) && !pauseDrain)
            {
                Drain(drainRate * Time.fixedDeltaTime);
                if(currentCharge <= 0f)
                {
                    squallController.EnterFollowMode();
                }
            }
            else 
            {
                stopwatch += Time.fixedDeltaTime;

                if (currentCharge < maxCharge && stopwatch >= rechargeDelay)
                    Recharge(rechargeRate * Time.fixedDeltaTime, false);
            }

            if (!squallController.owner) return;
            if (overlayController != null || !squallController.owner.GetComponent<CharacterBody>().isPlayerControlled) return;

            var ownerHUD = HUD.readOnlyInstanceList.Where(el => el.targetBodyObject == squallController.owner);
            foreach (HUD hud in ownerHUD)
            {
                CreateOverlay();
            }
        }
        
        public void Recharge(float amount, bool canOvercharge)
        {
            if(canOvercharge)
                currentCharge = Mathf.Clamp(currentCharge + amount, 0f, maxCharge + maxOvercharge);
            else
                currentCharge = Mathf.Clamp(currentCharge + amount, 0f, maxCharge);

            UpdateValues();
        }

        public void Drain(float amount)
        {
            currentCharge = Mathf.Clamp(currentCharge - amount, 0f, maxCharge + maxOvercharge);
            UpdateValues();
        }

        #region UI
        private void OnDisable()
        {
            if (overlayController != null)
            {
                overlayController.onInstanceAdded -= OverlayController_onInstanceAdded;
                HudOverlayManager.RemoveOverlay(overlayController);
            }

        }

        private void CreateOverlay()
        {
            OverlayCreationParams overlayCreationParams = new OverlayCreationParams
            {
                prefab = Modules.PathfinderAssets.BatteryMeter,
                childLocatorEntry = "CrosshairExtras"
            };

            overlayController = HudOverlayManager.AddOverlay(squallController.owner, overlayCreationParams);
            overlayController.onInstanceAdded += OverlayController_onInstanceAdded;

            UpdateValues();
            //overlayController.alpha = 0.5f;
        }

        private void OverlayController_onInstanceAdded(OverlayController overlayController, GameObject instance)
        {
            instance.transform.localPosition = new Vector3(-100f, 70, 0f);
            float sizeScale = 0.2f;
            instance.transform.localScale = new Vector3(sizeScale, sizeScale, sizeScale);


            //batteryRings = instance.transform.Find("OuterRings").GetComponent<Image>();
            batteryMeter = instance.transform.Find("Meter").GetComponent<Image>();
            //batteryGlow = instance.transform.Find("Glow").GetComponent<Image>();
            batteryText = instance.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>();
            //batteryPip = instance.transform.Find("Pip").GetComponent<Image>();

            if(batteryMeter && batteryText) allCreated = true;

            UpdateColor();
        }

        private void UpdateValues()
        {
            if (overlayController == null) return;
            if (!batteryMeter || !batteryText) return;

            float fill = Mathf.Clamp01(currentCharge / 100f);

            batteryMeter.fillAmount = fill;
            //batteryRings.fillAmount = fill;
            //batteryGlow.fillAmount = fill;
            batteryText.SetText(Mathf.FloorToInt(currentCharge).ToString());
        }

        internal void UpdateColor()
        {
            if (overlayController == null) return;
            if (!batteryMeter || !batteryText) return;

            if (squallController.inAttackMode)
            {
                UpdateColor(attackColor);
            }else
            {
                UpdateColor(followColor);
            }
        }

        private void UpdateColor(Color color)
        {
            //batteryGlow.color = color;
            batteryMeter.color = color;
            batteryText.color = color;
        }
        #endregion

        #region Junk
        /*
        private void Hooks()
        {
            On.RoR2.UI.HUD.Update += this.HUD_Update;
        }

        private void HUD_Update(On.RoR2.UI.HUD.orig_Update orig, HUD self)
        {
            orig(self);

            if (self.targetBodyObject == squallController.owner)
            {
                Chat.AddMessage("Found Squall Owner");

                Transform transform = self.transform.Find("MainContainer").Find("MainUIArea").Find("CrosshairCanvas");

                batteryUI = Instantiate(Modules.Assets.BatteryMeter, transform);
                batteryUI.transform.localPosition = new Vector3(-100f, 0f, 0f);
                batteryUI.transform.localScale = new Vector3(0.18f, 0.18f, 0.18f);
                batteryText = batteryUI.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>();
                batteryRings = batteryUI.transform.Find("OuterRings").GetComponent<Image>();
                batteryMeter = batteryUI.transform.Find("Meter").GetComponent<Image>();
                batteryGlow = batteryUI.transform.Find("Glow").GetComponent<Image>();
                batteryPip = batteryUI.transform.Find("Pip").GetComponent<Image>();

                UpdateColor();
                On.RoR2.UI.HUD.Update -= this.HUD_Update;
            }
        }
        */
        #endregion
    }
}
