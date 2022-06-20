using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using Pathfinder.Content;
using RoR2.UI;
using System.Linq;

namespace Pathfinder.Components
{
    internal class PathfinderController : MonoBehaviour
    {
        private GameObject summonPrefab;

        private CharacterMaster falconMaster;
        private CharacterMaster selfMaster;
        private CharacterBody selfBody;

        private SquallController squallController;

        private void Awake()
        {
            summonPrefab = PathfinderPlugin.squallMasterPrefab;
            //Hooks();
        }

        private void Start()
        {
            selfBody = base.GetComponent<CharacterBody>();
            Hooks();
            selfMaster = selfBody.master;
            var minions = CharacterMaster.readOnlyInstancesList.Where(el => el.minionOwnership.ownerMaster == selfMaster);
            foreach(CharacterMaster minion in minions)
            {
                Log.Warning(minion.name);
                if (minion.masterIndex == MasterCatalog.FindMasterIndex(summonPrefab))
                {
                    Log.Warning("Squall is alive");
                    falconMaster = minion;
                    squallController = minion.bodyInstanceObject.GetComponent<SquallController>();
                    return;
                }
            }
            SpawnFalcon(selfBody);
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
                falconMaster.godMode = true;
                squallController = falconMaster.bodyInstanceObject.GetComponent<SquallController>();
                falconMaster.inventory.CopyItemsFrom(characterBody.inventory);
                falconMaster.inventory.GiveItem(RoR2Content.Items.MinionLeash);
            }
        }

        public void ChooseTarget(HurtBox target)
        {
            if(target && target.healthComponent && target.healthComponent.alive)
            {
                squallController.SetTarget(target);
            }
        }

        private void Hooks()
        {
            selfBody.onInventoryChanged += SelfBody_onInventoryChanged;
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
