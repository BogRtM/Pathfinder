using UnityEngine;
using RoR2;
using EntityStates;

namespace Skillstates.Squall
{
    internal class MissileLauncher : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
