using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2;
using RoR2.UI;
using UnityEngine.UI;

namespace Pathfinder.Components
{
    internal class BatteryComponent : MonoBehaviour
    {
        public float maxCharge = 100f;
        private float currentCharge;

        public static float drainRate = Modules.Config.batteryDrainRate.Value;
        public static float rechargeRate = Modules.Config.batteryRechargeRate.Value;
        public static float rechargeDelay = 1f;
        internal float stopwatch;

        private GameObject batteryUI;
        private Image batteryMeter;
        private Image batteryRings;
        private Image batteryGlow;
        private Image batteryPip;
        private TMPro.TextMeshProUGUI batteryText;

        private Color followColor = Color.green;
        private Color attackColor = Color.red;

        internal bool pauseDrain;

        internal SquallController squallController;

        private void Awake()
        {
            //Do not set currentCharge to 0 on Awake
            currentCharge = 1f;
            stopwatch = rechargeDelay;
            squallController = base.GetComponent<SquallController>();
            Hooks();
        }

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

                Transform transform = self.transform.Find("MainContainer").Find("MainUIArea").Find("CrosshairCanvas").transform;

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

        private void OnDestroy()
        {
            UnityEngine.Object.Destroy(batteryUI);
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
        private void UpdateValues()
        {
            if (!batteryUI) return;

            float fill = currentCharge / 100f;

            batteryMeter.fillAmount = fill;
            batteryRings.fillAmount = fill;
            batteryGlow.fillAmount = fill;
            batteryText.SetText(Mathf.FloorToInt(currentCharge).ToString());
        }

        internal void UpdateColor()
        {
            if (!batteryUI) return;

            if(squallController.inAttackMode)
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
    }
}
