using UnityEngine;
using RoR2;
using EntityStates.GolemMonster;
using System;

namespace Pathfinder.Components
{
    internal class SquallPointer : MonoBehaviour
    {
        private ChildLocator childLocator;
        private InputBankTest inputBank;
        private LineRenderer laserLine;

        private CharacterBody selfBody;

        private float maxAim = 1000f;

        protected void OnEnable()
        {
            childLocator = base.GetComponentInChildren<ChildLocator>();
            laserLine = childLocator.FindChild("Squall").GetComponentInChildren<LineRenderer>();
            inputBank = base.GetComponent<InputBankTest>();
            selfBody = base.GetComponent<CharacterBody>();
        }

        protected void OnDisable()
        {
            laserLine.enabled = false;
        }

        protected void Update()
        {
            Ray aimRay = inputBank.GetAimRay();
            Vector3 origin = selfBody.footPosition;
            Vector3 point = aimRay.GetPoint(maxAim);

            laserLine.SetPosition(0, origin);
            laserLine.SetPosition(1, point);
        }
    }
}
