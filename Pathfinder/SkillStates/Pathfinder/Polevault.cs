using EntityStates;

namespace Pathfinder.SkillStates
{
    internal class Polevault : BaseState
    {
        public static float baseDuration = 1.5f;
        public override void OnEnter()
        {
            base.OnEnter();
            base.PlayAnimation("FullBody, Override", "Polevault", "Flip.playbackRate", baseDuration);
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
            return InterruptPriority.Pain;
        }
    }
}