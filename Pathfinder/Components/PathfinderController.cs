using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using Pathfinder.Content;
using RoR2.UI;
using System.Linq;
using RoR2.Skills;
using System.Collections.Generic;

namespace Pathfinder.Components
{
    internal class PathfinderController : MonoBehaviour //, IOnDamageDealtServerReceiver
    {
        private GameObject summonPrefab;

        private Animator modelAnimator;
        private SkillLocator skillLocator;

        private CharacterMaster falconMaster;
        private CharacterMaster selfMaster;
        private CharacterBody selfBody;

        private SquallController squallController;

        public static SkillDef javelinSkill;
        

        internal bool javelinReady;

        private void Awake()
        {
            summonPrefab = PathfinderPlugin.squallMasterPrefab;
            modelAnimator = base.GetComponentInChildren<Animator>();
            skillLocator = base.GetComponent<SkillLocator>();
        }

        private void Start()
        {
            selfBody = base.GetComponent<CharacterBody>();
            selfMaster = selfBody.master;

            FindOrSummonSquall();

            Subscriptions();
        }

        private void Subscriptions()
        {
            selfBody.onInventoryChanged += SelfBody_onInventoryChanged;
        }

        private void SelfBody_onInventoryChanged()
        {
            if (falconMaster)
            {
                falconMaster.inventory.CopyItemsFrom(selfBody.inventory);
                CleanSquallInventory(falconMaster.inventory);
            }
        }

        private void FindOrSummonSquall()
        {
            var minions = CharacterMaster.readOnlyInstancesList.Where(el => el.minionOwnership.ownerMaster == selfMaster);
            foreach (CharacterMaster minion in minions)
            {
                if (minion.masterIndex == MasterCatalog.FindMasterIndex(summonPrefab))
                {
                    falconMaster = minion;
                    if (!falconMaster.hasBody) falconMaster.Respawn(base.transform.position + Vector3.up, Quaternion.identity);
                    if (!falconMaster.godMode) falconMaster.ToggleGod();
                    squallController = minion.bodyInstanceObject.GetComponent<SquallController>();
                    squallController.owner = base.gameObject;
                    return;
                }
            }

            if (NetworkServer.active && !falconMaster)
            {
                SpawnFalcon(selfBody);
            }
        }

        private void SpawnFalcon(CharacterBody characterBody)
        {
            MasterSummon minionSummon = new MasterSummon();
            minionSummon.masterPrefab = summonPrefab;
            minionSummon.ignoreTeamMemberLimit = false;
            minionSummon.teamIndexOverride = TeamIndex.Player;
            minionSummon.summonerBodyObject = characterBody.gameObject;
            minionSummon.inventoryToCopy = characterBody.inventory;
            minionSummon.position = characterBody.corePosition + new Vector3(0f, 10f, 0f);
            minionSummon.rotation = Quaternion.identity;
            
            if(falconMaster = minionSummon.Perform())
            {
                if (!falconMaster.godMode) falconMaster.ToggleGod();
                CleanSquallInventory(falconMaster.inventory);
                squallController = falconMaster.bodyInstanceObject.GetComponent<SquallController>();
                squallController.owner = base.gameObject;
            }
        }

        private void CleanSquallInventory(Inventory inventory)
        {
            if (inventory.itemAcquisitionOrder.Count == 0) return;

            foreach(string itemName in Squall.SquallBlackList)
            {
                var itemIndex = ItemCatalog.FindItemIndex(itemName);
                var itemCount = inventory.GetItemCount(itemIndex);
                if (itemCount > 0)
                {
                    inventory.RemoveItem(itemIndex, itemCount);
                }
            }
        }

        internal void AttackOrder(HurtBox target)
        {
            squallController.EnterAttackMode();

            if(target && target.healthComponent && target.healthComponent.alive)
            {
                squallController.SetTarget(target);
            }
        }

        internal void DiveCommand(HurtBox target)
        {
            if (target && target.healthComponent && target.healthComponent.alive)
            {
                squallController.DiveTarget(target.healthComponent.gameObject);
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
            squallController.DoSpecialAttack(target);
        }

        /*
        public void OnDamageDealtServer(DamageReport damageReport)
        {
            if(damageReport.damageDealt >= (5f * damageReport.attackerBody.damage))
            {
                squallController.ShootMissile(damageReport.victim, damageReport.damageInfo.crit);
            }
            else if(damageReport.victim.alive && squallController)
            {
                squallController.ShootTarget(damageReport.victim, damageReport.damageInfo.crit);
            }
        }
        */
        public void ReadyJavelin()
        {
            javelinReady = true;
            skillLocator.primary.SetSkillOverride(base.gameObject, javelinSkill, GenericSkill.SkillOverridePriority.Contextual);
            if (modelAnimator)
            {
                modelAnimator.SetLayerWeight(modelAnimator.GetLayerIndex("JavelinReady"), 1f);
            }
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
    }
}
