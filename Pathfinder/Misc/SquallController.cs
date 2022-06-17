using UnityEngine;
using RoR2;
using EntityStates.GolemMonster;

namespace Pathfinder.Misc
{
    internal class SquallController : MonoBehaviour
    {
        private ChildLocator childLocator;
        private GameObject laserEffect;
        private InputBankTest inputBank;
        private LineRenderer laserLine;

        private float maxAim = 1000f;

        protected void Start()
        {
            childLocator = base.GetComponentInChildren<ChildLocator>();
            laserLine = childLocator.FindChild("Squall").GetComponentInChildren<LineRenderer>();
            inputBank = base.GetComponent<InputBankTest>();
        }

        protected void Update()
        {
            Ray aimRay = inputBank.GetAimRay();
            Vector3 origin = childLocator.FindChild("MainHurtbox").transform.position;
            Vector3 point = aimRay.GetPoint(maxAim);

            laserLine.SetPosition(0, origin);
            laserLine.SetPosition(1, point);
        }
    }
}
