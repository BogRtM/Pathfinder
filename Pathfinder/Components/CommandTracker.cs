﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Pathfinder.Components
{
    internal class CommandTracker : MonoBehaviour
    {
        private GameObject trackerPrefab;

        private CharacterBody characterBody;
        private Indicator indicator;
        private InputBankTest inputBank;
        private TeamComponent teamComponent;
        private HurtBox trackingTarget;

        public float maxTrackingDistance = 100f;
        public float maxTrackingAngle = 45f;
        public float trackerUpdateFrequency = 10f;
        private float trackerUpdateStopwatch;

        private readonly BullseyeSearch search = new BullseyeSearch();

        private void Awake()
        {
            trackerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/HuntressTrackingIndicator.prefab").WaitForCompletion();
            trackerPrefab.transform.Find("Core Pip").gameObject.GetComponent<SpriteRenderer>().color = new Color(6f / 255f, 50f / 255f, 15f / 255f);
            trackerPrefab.transform.Find("Core, Dark").gameObject.GetComponent<SpriteRenderer>().color = Color.green;
            foreach(SpriteRenderer i in trackerPrefab.transform.Find("Holder").gameObject.GetComponentsInChildren<SpriteRenderer>())
            {
                i.color = new Color(6f / 255f, 50f / 255f, 15f / 255f);
            }
            this.indicator = new Indicator(base.gameObject, trackerPrefab);
        }

        private void Start()
        {
            this.characterBody = base.GetComponent<CharacterBody>();
            this.inputBank = base.GetComponent<InputBankTest>();
            this.teamComponent = base.GetComponent<TeamComponent>();
        }

        public HurtBox GetTrackingTarget()
        {
            return this.trackingTarget;
        }

        private void OnEnable()
        {
            this.indicator.active = false;
        }

        private void OnDisable()
        {
            this.indicator.active = false;
        }

        private void OnDestroy()
        {
            this.indicator.active = false;
        }

        public void ActivateIndicator()
        {
            this.indicator.active = true;
        }

        public void DeactivateIndicator()
        {
            this.indicator.active = false;
        }

        private void FixedUpdate()
        {
            this.trackerUpdateStopwatch += Time.fixedDeltaTime;
            bool flag = this.trackerUpdateStopwatch >= 1f / this.trackerUpdateFrequency;
            if (flag)
            {
                this.trackerUpdateStopwatch -= 1f / this.trackerUpdateFrequency;
                Ray aimRay = new Ray(this.inputBank.aimOrigin, this.inputBank.aimDirection);
                this.SearchForTarget(aimRay);
                this.indicator.targetTransform = (this.trackingTarget ? this.trackingTarget.transform : null);
            }
        }

        private void SearchForTarget(Ray aimRay)
        {
            this.search.teamMaskFilter = TeamMask.GetUnprotectedTeams(this.teamComponent.teamIndex);
            this.search.filterByLoS = true;
            this.search.searchOrigin = aimRay.origin;
            this.search.searchDirection = aimRay.direction;
            this.search.sortMode = BullseyeSearch.SortMode.Angle;
            this.search.maxDistanceFilter = this.maxTrackingDistance;
            this.search.maxAngleFilter = this.maxTrackingAngle;
            this.search.RefreshCandidates();
            this.search.FilterOutGameObject(base.gameObject);
            this.trackingTarget = this.search.GetResults().FirstOrDefault<HurtBox>();
        }
    }
}
