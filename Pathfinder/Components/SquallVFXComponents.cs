using UnityEngine;
using RoR2;
using Pathfinder.Modules;
using System;

namespace Pathfinder.Components
{
    internal class SquallVFXComponents : MonoBehaviour
    {
        private GameObject dashEffect = PathfinderAssets.squallDashEffect;

        private ModelLocator modelLocator;
        private ChildLocator childLocator;
        private InputBankTest inputBank;
        private LineRenderer laserLine;
        private TrailRenderer[] trails;

        private CharacterBody selfBody;

        private Transform lineStartTransform;

        private float maxAim = 1000f;

        private void Awake()
        {
            modelLocator = base.GetComponent<ModelLocator>();
            childLocator = modelLocator.modelTransform.GetComponent<ChildLocator>();

            laserLine = base.GetComponentInChildren<LineRenderer>();
            
            trails = base.GetComponentsInChildren<TrailRenderer>();

            if (Config.laserLineEnabled.Value)
                laserLine.enabled = true;

            lineStartTransform = childLocator.FindChild("UpperBody");
        }

        private void OnEnable()
        {
            inputBank = base.GetComponent<InputBankTest>();
            selfBody = base.GetComponent<CharacterBody>();
        }

        private void OnDisable()
        {
            laserLine.enabled = false;
        }
        
        private void FixedUpdate()
        {
            if (laserLine.enabled)
            {
                Ray aimRay = inputBank.GetAimRay();
                Vector3 origin = lineStartTransform.position;
                Vector3 point = aimRay.GetPoint(maxAim);

                laserLine.SetPosition(0, origin);
                laserLine.SetPosition(1, point);
            }
        }

        internal void SetTrailColor(Color color)
        {
            foreach(var i in trails)
            {
                i.startColor = color;
                i.endColor = color;
            }
        }

        internal void SetLineColor(Color color)
        {
            if (Config.laserLineEnabled.Value)
            {
                laserLine.startColor = color;
                laserLine.endColor = color;
            }
        }

        internal void ToggleVFX(bool onOff)
        {
            if (Config.laserLineEnabled.Value)
            {
                laserLine.enabled = onOff;
            }

            foreach(var i in trails)
            {
                i.enabled = onOff;
            }
        }
        internal void PlayDashEffect(Vector3 start, Vector3 end)
        {
            dashEffect.GetComponentInChildren<ParticleSystem>().startSpeed = Vector3.Distance(start, end) * 5f;

            EffectData effectData = new EffectData()
            {
                origin = start,
                rotation = Util.QuaternionSafeLookRotation((end - start).normalized)
            };

            EffectManager.SpawnEffect(dashEffect, effectData, true);
        }

    }
}
