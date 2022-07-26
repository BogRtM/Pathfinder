using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using EntityStates;
using EntityStates.Toolbot;
using RoR2;
using Pathfinder.Components;
using Pathfinder.Modules;

namespace Skillstates.Pathfinder
{
    internal class AimBolas : AimThrowableBase
    {
        public static float loopDuration = 0.4f;
        public static float throwForce = 60f;

        private GameObject bolas;
        private string javReady = "";

        private OverrideController controller;
        private ChildLocator childLocator;

        public override void OnEnter()
        {
            controller = base.GetComponent<OverrideController>();
            if (controller.javelinReady) javReady = "Jav";
            bolas = base.FindModelChild("Bolas").gameObject;
            bolas.SetActive(true);

            base.PlayAnimation("Gesture, Override", javReady + "BolasLoop", "Hand.playbackRate", loopDuration);

            projectilePrefab = Projectiles.shockBolas;
            arcVisualizerPrefab = Assets.lineVisualizer;
            endpointVisualizerPrefab = Assets.explosionVisualizer;
            endpointVisualizerRadiusScale = 18f;
            detonationRadius = 18f;
            damageCoefficient = Config.bolasExplosionDamage.Value;
            setFuse = false;
            projectileBaseSpeed = 200f;
            maxDistance = float.PositiveInfinity;
            baseMinimumDuration = 0.1f;

            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.StartAimMode(0.1f, false);

            base.FixedUpdate();
        }

        public override void OnExit()
        {
            base.PlayAnimation("Gesture, Override", javReady + "BolasThrow", "Hand.playbackRate", loopDuration);
            bolas.SetActive(false);
            base.OnExit();
        }
    }
}
