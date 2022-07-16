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
        private CommandTracker tracker;
        private OverrideController overrideController;
        private FalconerComponent falconerComponent;

        public override void OnEnter()
        {
            base.OnEnter();
            tracker = base.GetComponent<CommandTracker>();
            overrideController = base.GetComponent<OverrideController>();
            falconerComponent = base.GetComponent<FalconerComponent>();
            tracker.ActivateIndicator();

            Util.PlaySound(Paint.enterSoundString, base.gameObject);

            base.characterBody.hideCrosshair = true;
            falconerComponent.AttachCommandCrosshair();

            overrideController.SetCommandSkills();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void OnExit()
        {
            tracker.DeactivateIndicator();
            base.characterBody.hideCrosshair = false;
            falconerComponent.DeactivateCrosshair();
            Util.PlaySound(Paint.exitSoundString, base.gameObject);
            overrideController.UnsetCommandSkills();
            base.OnExit();
        }
    }
}