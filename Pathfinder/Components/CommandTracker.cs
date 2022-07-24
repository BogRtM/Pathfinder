using System.Linq;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Pathfinder.Components
{
    internal class CommandTracker : MonoBehaviour
    {
        internal static GameObject trackerPrefab;

        private Indicator indicator;
        private InputBankTest inputBank;
        private TeamComponent teamComponent;
        private HurtBox trackingTarget;

        public float maxTrackingDistance = 120f;
        public float maxTrackingAngle = 15f;
        public float trackerUpdateFrequency = 10f;
        private float trackerUpdateStopwatch;

        private readonly BullseyeSearch search = new BullseyeSearch();

        private void Awake()
        {
            this.indicator = new Indicator(base.gameObject, trackerPrefab);
        }
        
        private void OnDisable()
        {
            DeactivateIndicator();
        }

        private void Start()
        {
            //this.characterBody = base.GetComponent<CharacterBody>();
            this.inputBank = base.GetComponent<InputBankTest>();
            this.teamComponent = base.GetComponent<TeamComponent>();
        }

        public void ActivateIndicator()
        {
            indicator.active = true;
        }

        public void DeactivateIndicator()
        {
            indicator.active = false;
        }

        public HurtBox GetTrackingTarget()
        {
            return this.trackingTarget;
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
