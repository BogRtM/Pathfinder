using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using EntityStates;
using Pathfinder;

namespace SkillStates.Squall
{
    internal class Piledriver : BaseState
    {
        public static float baseDuration = 0.5f;

        public HurtBox target;

        public override void OnEnter()
        {
            base.OnEnter();
            Log.Warning("Special order at Squall entity state");
            Chat.AddMessage("Special order received");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(base.fixedAge >= baseDuration)
            {
                this.outer.SetNextStateToMain();
            }
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
