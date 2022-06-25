using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using Pathfinder.Content;
using RoR2.UI;
using System.Linq;
using RoR2.Skills;

namespace Pathfinder.Components
{
    internal class PathfinderController : MonoBehaviour
    {
        private GameObject summonPrefab;

        private Animator modelAnimator;
        private SkillLocator skillLocator;

        private CharacterMaster falconMaster;
        private HealthComponent falconHP;
        private CharacterMaster selfMaster;
        private CharacterBody selfBody;

        private SquallController squallController;

        public static SkillDef javelinSkill;

        internal bool javelinReady;

        private float javelinDuration = 3f;
        private float javelinTimer;

        private void Awake()
        {
            summonPrefab = PathfinderPlugin.squallMasterPrefab;
            modelAnimator = base.GetComponentInChildren<Animator>();
            skillLocator = base.GetComponent<SkillLocator>();
            //Hooks();
        }

        private void Start()
        {
            selfBody = base.GetComponent<CharacterBody>();
            selfMaster = selfBody.master;
            Hooks();
            var minions = CharacterMaster.readOnlyInstancesList.Where(el => el.minionOwnership.ownerMaster == selfMaster);
            foreach(CharacterMaster minion in minions)
            {
                Log.Warning(minion.name);
                if (minion.masterIndex == MasterCatalog.FindMasterIndex(summonPrefab))
                {
                    Log.Warning("Squall is alive");
                    falconMaster = minion;
                    if (!falconMaster.godMode) falconMaster.ToggleGod();
                    squallController = minion.bodyInstanceObject.GetComponent<SquallController>();
                    squallController.ownerController = this;
                    return;
                }
            }
            SpawnFalcon(selfBody);
        }

        private void FixedUpdate()
        {
            if (javelinReady)
            {
                javelinTimer -= Time.fixedDeltaTime;
                if(javelinTimer <= 0)
                {
                    UnreadyJavelin();
                }
            }
        }

        public void ReadyJavelin()
        {
            javelinReady = true;
            skillLocator.primary.SetSkillOverride(base.gameObject, javelinSkill, GenericSkill.SkillOverridePriority.Contextual);
            if(modelAnimator)
            {
                modelAnimator.SetLayerWeight(modelAnimator.GetLayerIndex("JavelinReady"), 1f);
            }
            javelinTimer = javelinDuration;
        }

        public void UnreadyJavelin()
        {
            javelinReady = false;
            skillLocator.primary.UnsetSkillOverride(base.gameObject, javelinSkill, GenericSkill.SkillOverridePriority.Contextual);
            if (modelAnimator)
            {
                modelAnimator.SetLayerWeight(modelAnimator.GetLayerIndex("JavelinReady"), 0f);
            }
        }

        private void SpawnFalcon(CharacterBody characterBody)
        {
            MasterSummon minionSummon = new MasterSummon();
            minionSummon.masterPrefab = summonPrefab;
            minionSummon.ignoreTeamMemberLimit = false;
            minionSummon.teamIndexOverride = TeamIndex.Player;
            minionSummon.summonerBodyObject = characterBody.gameObject;
            minionSummon.position = characterBody.corePosition;
            minionSummon.rotation = characterBody.transform.rotation;
            
            if(falconMaster = minionSummon.Perform())
            {
                if (!falconMaster.godMode) falconMaster.ToggleGod();
                squallController = falconMaster.bodyInstanceObject.GetComponent<SquallController>();
                squallController.ownerController = this;
                falconMaster.inventory.CopyItemsFrom(characterBody.inventory);
                falconMaster.inventory.GiveItem(RoR2Content.Items.MinionLeash);
            }
        }

        internal void AttackOrder(HurtBox target)
        {
            if(target && target.healthComponent && target.healthComponent.alive)
            {
                squallController.SetTarget(target);
            }
        }

        internal void FollowOrder()
        {
            Vector3 teleportPosition = selfBody.corePosition + new Vector3(0f, 10f, 0f);
            TeleportHelper.TeleportBody(falconMaster.GetBody(), teleportPosition);
            EffectManager.SimpleEffect(Run.instance.GetTeleportEffectPrefab(falconMaster.bodyInstanceObject), teleportPosition, Quaternion.identity, true);
            squallController.EnterFollowMode();
        }

        internal void SpecialOrder(HurtBox target)
        {
            Log.Warning("Special order at pathfindercontroller");
            squallController.DoSpecialAttack(target);
        }

        private void Hooks()
        {
            //On.RoR2.PrimarySkillShurikenBehavior.OnSkillActivated += PrimarySkillShurikenBehavior_OnSkillActivated;
            selfBody.onInventoryChanged += SelfBody_onInventoryChanged;
            selfMaster.onBodyDestroyed += SelfMaster_onBodyDestroyed;
        }

        private void SelfMaster_onBodyDestroyed(CharacterBody obj)
        {
            if(falconMaster) falconMaster.godMode = false;
            falconMaster.TrueKill();
        }

        private void SelfBody_onInventoryChanged()
        {
            if(falconMaster)
                falconMaster.inventory.CopyItemsFrom(selfBody.inventory);
        }

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody obj)
        {
            if(obj.gameObject.GetComponent<PathfinderController>() && falconMaster)
            {
                
            }
        }
    }
}
