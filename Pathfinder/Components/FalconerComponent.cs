using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using Pathfinder.Content;

namespace Pathfinder.Components
{
    internal class FalconerComponent : MonoBehaviour
    {
        private GameObject bodyPrefab;
        private GameObject summonPrefab;

        private CharacterMaster falconMaster;
        private CharacterBody selfBody;

        private SquallController squallController;

        private bool falconIsAlive;
        private void Awake()
        {
            this.bodyPrefab = base.gameObject;
            selfBody = base.GetComponent<CharacterBody>();
            summonPrefab = PathfinderPlugin.squallMasterPrefab;
            Hooks();
        }

        private void OnEnable()
        {
            if (GameObject.Find("SquallBody(Clone)")) return;

            Log.Warning("Attempting summon");
            if (NetworkServer.active)
            {
                falconMaster = SpawnFalcon();
                //falconIsAlive = true;
                //falconMaster.godMode = true;
                squallController = summonPrefab.GetComponent<SquallController>();
                falconMaster.inventory.GiveItem(RoR2Content.Items.MinionLeash);
            }
        }

        private CharacterMaster SpawnFalcon()
        {
            Log.Warning(summonPrefab.name);
            MasterSummon minionSummon = new MasterSummon();
            minionSummon.masterPrefab = summonPrefab;
            minionSummon.ignoreTeamMemberLimit = false;
            minionSummon.teamIndexOverride = TeamIndex.Player;
            minionSummon.summonerBodyObject = bodyPrefab;
            minionSummon.position = base.transform.position;
            minionSummon.rotation = base.transform.rotation;
            return minionSummon.Perform();
        }

        private void Hooks()
        {
            //CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
        }

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody obj)
        {
            if(obj.bodyIndex == selfBody.bodyIndex && falconMaster)
            {
                falconMaster.inventory.CopyItemsFrom(obj.inventory);
            }
        }
    }
}
