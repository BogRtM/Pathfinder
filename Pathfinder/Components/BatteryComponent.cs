using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Pathfinder;
using RoR2;

namespace Pathfinder.Components
{
    internal class BatteryComponent : MonoBehaviour
    {
        public float maxCharge = 100f;
        internal float charge;

        internal SquallController squallController;

        private void Awake()
        {
            charge = maxCharge;
        }

        private void FixedUpdate()
        {
            if (squallController.inAttackMode)
                AddCharge();
        }

        private void AddCharge()
        {
            throw new NotImplementedException();
        }

        private void DepleteCharge()
        {

        }
    }
}
