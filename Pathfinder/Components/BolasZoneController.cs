using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using Pathfinder.Modules;
using RoR2.Projectile;

namespace Pathfinder.Components
{
    internal class BolasZoneController : MonoBehaviour
    {
        private GameObject owner;
        private TeamFilter teamFilter;
        private SphereSearch search;
        private Vector3 position;

        private DotController.DotIndex electrocute = Buffs.electrocuteDoT;

        private float radius = 18f;
        private float dotDuration = 2f;
        private float pulseInterval = 1.5f;
        private float pulseStopwatch = 1.5f;

        private List<HurtBox> candidates;

        private void Awake()
        {
            teamFilter = base.GetComponent<TeamFilter>();
            search = new SphereSearch();
            position = base.transform.position;
            candidates = new List<HurtBox>();
        }

        private void Start()
        {
            owner = base.GetComponent<ProjectileController>().owner;
        }

        private void FixedUpdate()
        {
            if(NetworkServer.active)
            {
                pulseStopwatch += Time.fixedDeltaTime;
                if(pulseStopwatch >= pulseInterval)
                {
                    pulseStopwatch = 0f;
                    
                    SearchForTargets(candidates);

                    foreach(HurtBox victim in candidates)
                    {
                        if(victim.healthComponent && victim.healthComponent.alive)
                        {
                            InflictElectrocute(victim.healthComponent);
                        }
                    }

                    candidates.Clear();
                }
            }
        }

        private void InflictElectrocute(HealthComponent victim)
        {
            InflictDotInfo dotInfo = new InflictDotInfo();
            dotInfo.attackerObject = owner;
            dotInfo.victimObject = victim.gameObject;
            dotInfo.dotIndex = electrocute;
            dotInfo.duration = dotDuration;
            dotInfo.damageMultiplier = 1f;
            DotController.InflictDot(ref dotInfo);
        }

        protected void SearchForTargets(List<HurtBox> candidates)
		{
            search.origin = position;
            search.mask = LayerIndex.entityPrecise.mask;
            search.radius = radius;
            search.RefreshCandidates();
            search.FilterCandidatesByHurtBoxTeam(TeamMask.GetUnprotectedTeams(teamFilter.teamIndex));
            search.OrderCandidatesByDistance();
            search.FilterCandidatesByDistinctHurtBoxEntities();
            search.GetHurtBoxes(candidates);
            search.ClearCandidates();
        }
    }
}
