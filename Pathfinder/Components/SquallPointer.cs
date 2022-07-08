using UnityEngine;
using RoR2;
using System;

namespace Pathfinder.Components
{
    internal class SquallPointer : MonoBehaviour
    {
        private InputBankTest inputBank;
        private LineRenderer laserLine;

        private CharacterBody selfBody;

        private float maxAim = 1000f;

        protected void OnEnable()
        {
            laserLine = base.GetComponentInChildren<LineRenderer>();
            inputBank = base.GetComponent<InputBankTest>();
            selfBody = base.GetComponent<CharacterBody>();
            laserLine.enabled = true;
        }

        protected void OnDisable()
        {
            laserLine.enabled = false;
        }
        
        protected void Update()
        {
            Ray aimRay = inputBank.GetAimRay();
            Vector3 origin = selfBody.corePosition;
            Vector3 point = aimRay.GetPoint(maxAim);

            laserLine.SetPosition(0, origin);
            laserLine.SetPosition(1, point);
        }
        
    }
}
