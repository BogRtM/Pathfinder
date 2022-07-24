using EntityStates;
using EntityStates.Engi.EngiMissilePainter;
using RoR2;
using RoR2.UI;
using Pathfinder.Components;
using UnityEngine;
using Pathfinder;
using Pathfinder.Modules;
using System;
using RoR2.HudOverlay;

namespace Skillstates.Pathfinder.Command
{
    internal class CommandMode : BaseState
    {
        private CommandTracker tracker;
        private OverrideController overrideController;
        private FalconerComponent falconerComponent;

        private OverlayController overlayController;

        public override void OnEnter()
        {
            base.OnEnter();
            tracker = base.GetComponent<CommandTracker>();
            overrideController = base.GetComponent<OverrideController>();
            falconerComponent = base.GetComponent<FalconerComponent>();
            tracker.ActivateIndicator();

            overrideController.inCommandMode = true;

            Util.PlaySound(Paint.enterSoundString, base.gameObject);

            overlayController = HudOverlayManager.AddOverlay(base.gameObject, new OverlayCreationParams
            {
                prefab = Assets.commandCrosshair,
                childLocatorEntry = "ScopeContainer"
            });

            overrideController.SetCommandSkills();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void OnExit()
        {
            tracker.DeactivateIndicator();
            overrideController.inCommandMode = false;
            RemoveOverlay();
            Util.PlaySound(Paint.exitSoundString, base.gameObject);
            overrideController.UnsetCommandSkills();
            base.OnExit();
        }

        private void RemoveOverlay()
        {
            if(overlayController != null)
            {
                HudOverlayManager.RemoveOverlay(overlayController);
                overlayController = null;
            }
        }
    }
}