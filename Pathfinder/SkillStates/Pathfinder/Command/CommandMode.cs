using EntityStates;
using EntityStates.Engi.EngiMissilePainter;
using RoR2;
using RoR2.UI;
using Pathfinder.Components;
using UnityEngine;
using Pathfinder;
using System;

namespace Skillstates.Pathfinder.Command
{
    internal class CommandMode : BaseState
    {
        public static float minDuration = 0.1f;
        public static GameObject crosshairPrefab;

        private CommandTracker tracker;
        private OverrideController overrideController;
        private CrosshairUtils.OverrideRequest overrideRequest;

        public override void OnEnter()
        {
            base.OnEnter();
            tracker = base.GetComponent<CommandTracker>();
            overrideController = base.GetComponent<OverrideController>();
            tracker.ActivateIndicator();

            Util.PlaySound(Paint.enterSoundString, base.gameObject);

            if(crosshairPrefab)
            {
                overrideRequest = CrosshairUtils.RequestOverrideForBody(base.characterBody, crosshairPrefab, CrosshairUtils.OverridePriority.Skill);
            }

            overrideController.SetCommandSkills();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void OnExit()
        {
            tracker.DeactivateIndicator();
            Util.PlaySound(Paint.exitSoundString, base.gameObject);
            if (overrideRequest != null) overrideRequest.Dispose(); 
            overrideController.UnsetCommandSkills();
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}