using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using Pathfinder.Content;

namespace Pathfinder.Misc
{
    internal class FalconerComponent : MonoBehaviour
    {
        private GameObject bodyPrefab;
        private GameObject summonPrefab;

        private CharacterMaster falconMaster;
        private CharacterBody selfBody;

        private bool falconIsAlive;
        private void Awake()
        {
            this.bodyPrefab = base.GetComponent<GameObject>();
            selfBody = base.GetComponent<CharacterBody>();
            summonPrefab = PathfinderPlugin.squallPrefab;
            Hooks();
        }

        private void Hooks()
        {
            //CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
        }

        private void CharacterBody_onBodyStartGlobal(CharacterBody characterBody)
        {
            
            if (characterBody.bodyIndex == selfBody.bodyIndex && !falconMaster)
            {
                Log.Warning("Attempting summon");
                if (NetworkServer.active)
                {
                    falconMaster = SpawnFalcon(characterBody);
                    falconMaster.godMode = true;
                    falconMaster.inventory.GiveItem(RoR2Content.Items.MinionLeash);
                }
            }
        }

        private CharacterMaster SpawnFalcon(CharacterBody characterBody)
        {
            Log.Warning(summonPrefab.name);
            MasterSummon minionSummon = new MasterSummon();
            minionSummon.masterPrefab = summonPrefab;
            minionSummon.ignoreTeamMemberLimit = false;
            minionSummon.teamIndexOverride = TeamIndex.Player;
            minionSummon.summonerBodyObject = characterBody.gameObject;
            minionSummon.position = characterBody.transform.position;
            minionSummon.rotation = characterBody.transform.rotation;
            return minionSummon.Perform();
        }
    }
}
