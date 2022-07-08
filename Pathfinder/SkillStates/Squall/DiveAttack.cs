using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RoR2;
using R2API;
using EntityStates;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Pathfinder;
using Pathfinder.Components;

namespace Skillstates.Squall
{
    internal class DiveAttack : BaseState
    {
        public static float prepDuration = 0.5f;
        public static float giveUpDuration = 3f;
        public static float speedCoefficient = 5f;
        public static float searchRadius = 40f;

        public GameObject target;
        private GameObject effect;
        private SphereSearch search;
        private Vector3 divePosition;
        private SquallController squallController;

        private bool startDive;

        public override void OnEnter()
        {
            base.OnEnter();
            squallController = base.GetComponent<SquallController>();

            Chat.AddMessage("Diving now");

            if (target) divePosition = target.transform.position;

            base.PlayAnimation("FullBody, Override", "DivePrep", "Wing.playbackRate", prepDuration);
            effect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LemurianBruiser/OmniExplosionVFXLemurianBruiserFireballImpact.prefab").WaitForCompletion();
        }

        private GameObject GetBestTarget()
        {
            Chat.AddMessage("Searching for target now");
            List<HurtBox> candidates = new List<HurtBox>();

            search = new SphereSearch();
            search.origin = squallController.owner.transform.position;
            search.mask = LayerIndex.entityPrecise.mask;
            search.radius = searchRadius;
            search.RefreshCandidates();
            search.FilterCandidatesByHurtBoxTeam(TeamMask.GetEnemyTeams(base.teamComponent.teamIndex));
            search.FilterCandidatesByDistinctHurtBoxEntities();
            search.GetHurtBoxes(candidates);
            search.ClearCandidates();

            List<HurtBox> orderedList = candidates.OrderBy(e => e.healthComponent.health).ToList();

            GameObject bestTarget = orderedList.Last().healthComponent.gameObject;

            Chat.AddMessage("Diving: " + bestTarget + " - Distance: " + Vector3.Distance(bestTarget.transform.position, search.origin));

            return bestTarget;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(base.fixedAge >= prepDuration && !startDive)
            {
                startDive = true;
            }

            if(startDive && base.isAuthority && target)
            {
                base.rigidbodyMotor.rootMotion += (divePosition - base.transform.position).normalized * (speedCoefficient * base.moveSpeedStat * Time.fixedDeltaTime);
            }
            
            if(Vector3.Distance(divePosition, base.transform.position) <= 1f && base.isAuthority)
            {
                BlastAttack attack = new BlastAttack()
                {
                    attacker = squallController.owner,
                    attackerFiltering = AttackerFiltering.NeverHitSelf,
                    crit = base.RollCrit(),
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = DamageType.Generic,
                    falloffModel = BlastAttack.FalloffModel.None,
                    position = base.transform.position,
                    baseDamage = base.damageStat * 3f,
                    impactEffect = EffectCatalog.FindEffectIndexFromPrefab(effect),
                    radius = 20f,
                    teamIndex = base.teamComponent.teamIndex
                };

                attack.AddModdedDamageType(PathfinderPlugin.marking);

                attack.Fire();

                this.outer.SetNextStateToMain();
            } 
            else if(base.fixedAge >= giveUpDuration + prepDuration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.PlayCrossfade("FullBody, Override", "BufferEmpty", 0.2f);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
