﻿using UnityEngine;
using RoR2;
using EntityStates;
using Pathfinder.Components;
using Pathfinder;

namespace Skillstates.Pathfinder.Command
{
    internal class SpecialCommand : BaseIssueCommand
    {
        public HurtBox target;

        private string javString = "";

        public override void OnEnter()
        {
            base.OnEnter();
            target = commandTracker.GetTrackingTarget();

            //overrideController.squallSpecialCurrentStock--;

            if (overrideController.javelinReady) javString = "Jav";

            base.PlayCrossfade("Gesture, Override", javString + "Point", "Hand.playbackRate", duration, 0.1f);

            falconerComponent.SpecialOrder(target);
        }
    }
}
