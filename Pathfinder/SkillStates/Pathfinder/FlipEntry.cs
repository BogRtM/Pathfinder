using EntityStates;
using UnityEngine;

namespace Pathfinder.SkillStates
{
    internal class FlipEntry : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            EntityState nextState = new AirFlip();
            if(base.characterMotor.isGrounded)
            {
                nextState = new Polevault();
            }
            
            this.outer.SetNextState(nextState);
        }
    }
}
