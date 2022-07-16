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
//using Pathfinder.Modules.Misc;

namespace Pathfinder.Components
{
    internal class FalconerComponent : MonoBehaviour //, IOnDamageDealtServerReceiver
    {
        internal static GameObject summonPrefab;

        private CharacterMaster falconMaster;
        private CharacterMaster selfMaster;
        private CharacterBody selfBody;

        internal SquallController squallController;

        internal GameObject commandCrosshair;

        private void Start()
        {
            selfBody = base.GetComponent<CharacterBody>();
            selfMaster = selfBody.master;

            //AttachCommandCrosshair();

            FindOrSummonSquall();

            Subscriptions();
        }

        internal void AttachCommandCrosshair()
        {
            if(commandCrosshair)
            {
                commandCrosshair.SetActive(true);
                return;
            }

            var selfHUD = HUD.readOnlyInstanceList.Where(el => el.targetBodyObject == base.gameObject);
            foreach(HUD i in selfHUD)
            {
                Transform crosshairArea = i.transform.Find("MainContainer").Find("MainUIArea").Find("CrosshairCanvas");
                if (!crosshairArea) return;
                commandCrosshair = UnityEngine.Object.Instantiate(PathfinderPlugin.commandCrosshair);
                commandCrosshair.transform.SetParent(crosshairArea, false);
                commandCrosshair.SetActive(true);
            }
        }

        internal void DeactivateCrosshair()
        {
            if(commandCrosshair) commandCrosshair.SetActive(false);
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
            minionSummon.rotation = characterBody.transform.rotation;
            
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

        internal void FollowOrder()
        {
            Vector3 teleportPosition = selfBody.corePosition + new Vector3(0f, 10f, 0f);
            squallController.DiveToPoint(teleportPosition);
            //TeleportHelper.TeleportBody(falconMaster.GetBody(), teleportPosition);
            //EffectManager.SimpleEffect(Run.instance.GetTeleportEffectPrefab(falconMaster.bodyInstanceObject), teleportPosition, Quaternion.identity, true);
            squallController.EnterFollowMode();
        }

        internal void SpecialOrder(HurtBox target)
        {
            squallController.DoSpecialAttack(target);
        }
    }
}
