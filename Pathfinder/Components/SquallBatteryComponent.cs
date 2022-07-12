using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Pathfinder;
using RoR2;
using RoR2.UI;

namespace Pathfinder.Components
{
    internal class SquallBatteryComponent : MonoBehaviour
    {
        public float maxCharge = 100f;
        internal float currentCharge;

        private float drainRate = 1f;
        private float rechargeRate = 2f;

        public bool canEnterAttackMode { get { return (currentCharge >= (maxCharge / 2f)); } }

        internal SquallController squallController;

        private void Awake()
        {
            currentCharge = maxCharge;
            squallController = base.GetComponent<SquallController>();
        }

        private void FixedUpdate()
        {
            if (squallController.inAttackMode)
            {
                Drain();
                if(currentCharge <= 0f)
                {
                    Chat.AddMessage("Battery depleted");
                    squallController.EnterFollowMode();
                }
            }
            else if(currentCharge < maxCharge)
            {
                Recharge();
            }
        }

        private void Recharge()
        {
            currentCharge = Mathf.Clamp(currentCharge + (rechargeRate * Time.fixedDeltaTime), 0f, maxCharge);
        }

        private void Drain()
        {
            currentCharge = Mathf.Clamp(currentCharge - (drainRate * Time.fixedDeltaTime), 0f, maxCharge);
        }
    }
}
