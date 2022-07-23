using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2;
using RoR2.UI;
using RoR2.HudOverlay;
using UnityEngine.UI;

namespace Pathfinder.Components
{
    internal class BatteryComponent : MonoBehaviour
    {
        public float maxCharge = 100f;
        private float currentCharge = 0f;

        public static float drainRate = Modules.Config.batteryDrainRate.Value;
        public static float rechargeRate = Modules.Config.batteryRechargeRate.Value;
        public static float rechargeDelay = 1f;
        internal float stopwatch;

        private OverlayController overlayController;

        private Image batteryMeter;
        private Image batteryRings;
        private Image batteryGlow;
        private Image batteryPip;
        private TMPro.TextMeshProUGUI batteryText;

        private Color followColor = Color.green;
        private Color attackColor = Color.red;

        internal bool pauseDrain;
        private bool allCreated;

        internal SquallController squallController;

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
            CreateOverlay();
        }

        private void OnDisable()
        {
            if (overlayController != null)
            {
                overlayController.onInstanceAdded -= OverlayController_onInstanceAdded;
                HudOverlayManager.RemoveOverlay(overlayController);
            }
        }
        
        private void FixedUpdate()
        {
            if (squallController.inAttackMode && !pauseDrain)
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
                    Recharge(rechargeRate * Time.fixedDeltaTime);
            }
        }
        
        public void Recharge(float amount)
        {
            currentCharge = Mathf.Clamp(currentCharge + amount, 0f, maxCharge);
            UpdateValues();
        }

        public void Drain(float amount)
        {
            currentCharge = Mathf.Clamp(currentCharge - amount, 0f, maxCharge);
            UpdateValues();
        }

        #region UI
        private void CreateOverlay()
        {
            OverlayCreationParams overlayCreationParams = new OverlayCreationParams
            {
                prefab = Modules.Assets.BatteryMeter,
                childLocatorEntry = "CrosshairExtras"
            };

            overlayController = HudOverlayManager.AddOverlay(squallController.owner, overlayCreationParams);
            overlayController.onInstanceAdded += OverlayController_onInstanceAdded;

            overlayController.alpha = 0.5f;
        }

        private void OverlayController_onInstanceAdded(OverlayController overlayController, GameObject instance)
        {
            instance.transform.localPosition = new Vector3(-100f, 0f, 0f);
            instance.transform.localScale = new Vector3(0.18f, 0.18f, 0.18f);


            batteryRings = instance.transform.Find("OuterRings").GetComponent<Image>();
            batteryMeter = instance.transform.Find("Meter").GetComponent<Image>();
            batteryGlow = instance.transform.Find("Glow").GetComponent<Image>();
            batteryText = instance.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>();
            batteryPip = instance.transform.Find("Pip").GetComponent<Image>();

            if(batteryRings && batteryMeter && batteryGlow && batteryText && batteryPip) allCreated = true;

            UpdateColor();
        }

        private void UpdateValues()
        {
            if (overlayController == null) return;
            if (!allCreated) return;

            float fill = currentCharge / 100f;

            batteryMeter.fillAmount = fill;
            batteryRings.fillAmount = fill;
            batteryGlow.fillAmount = fill;
            batteryText.SetText(Mathf.FloorToInt(currentCharge).ToString());
        }

        internal void UpdateColor()
        {
            if (overlayController == null) return;

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
            batteryPip.color = color;
            batteryRings.color = color;
            batteryGlow.color = color;
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
