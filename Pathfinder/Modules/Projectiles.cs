using R2API;
using RoR2;
using RoR2.Projectile;
using Pathfinder.Components;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace Pathfinder.Modules
{
    internal static class Projectiles
    {
        internal static GameObject bombPrefab;
        internal static GameObject javelinPrefab;
        internal static GameObject explodingJavelin;
        internal static GameObject bolas;
        internal static GameObject bolasZone;

        internal static void RegisterProjectiles()
        {
            CreateExplodingJavelin();
            CreateBolasZone();
            CreateBolas();
            AddProjectile(explodingJavelin);
            AddProjectile(bolas);
            AddProjectile(bolasZone);
        }

        private static void CreateBolas()
        {
            bolas = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/CommandoGrenadeProjectile.prefab").WaitForCompletion(), "ShockBolas");
            //DestroyOnTimer destroyOnTimer = shockNet.AddComponent<DestroyOnTimer>();
            //destroyOnTimer.duration = 10f;

            ProjectileController bolasController = bolas.GetComponent<ProjectileController>();
            bolasController.ghostPrefab = CreateGhostPrefab("BolasGhost");

            ProjectileSimple simple = bolasController.GetComponent<ProjectileSimple>();
            simple.desiredForwardSpeed = 200f;

            ProjectileDamage projectileDamage = bolas.GetComponent<ProjectileDamage>();
            projectileDamage.damageType = DamageType.Shock5s;
            projectileDamage.force = 0f;

            ApplyTorqueOnStart torque = bolas.GetComponent<ApplyTorqueOnStart>();
            torque.randomize = false;
            torque.localTorque = new Vector3(0f, -600f, 0f);

            ProjectileImpactExplosion impactExplosion = bolas.GetComponent<ProjectileImpactExplosion>();
            impactExplosion.lifetimeAfterImpact = 0f;
            impactExplosion.blastRadius = 18f;
            impactExplosion.falloffModel = BlastAttack.FalloffModel.None;
            impactExplosion.impactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Captain/CaptainTazerNova.prefab").WaitForCompletion();

            impactExplosion.fireChildren = true;
            impactExplosion.childrenProjectilePrefab = bolasZone;
            impactExplosion.childrenCount = 1;
            impactExplosion.childrenDamageCoefficient = 0f;
        }
        private static void CreateBolasZone()
        {
            float zoneRadius = 18f;

            bolasZone = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/MiniMushroom/SporeGrenadeProjectileDotZone.prefab").WaitForCompletion(), "BolasZone");
            UnityEngine.Object.Destroy(bolasZone.GetComponent<ProjectileDotZone>());

            UnityEngine.Object.Destroy(bolasZone.transform.GetChild(0).gameObject);

            bolasZone.AddComponent<BolasZoneController>();

            DestroyOnTimer destroyOnTimer = bolasZone.AddComponent<DestroyOnTimer>();
            destroyOnTimer.duration = 10f;

            GameObject bolasZoneEffect = Assets.lightningRingEffect.InstantiateClone("BolasZoneEffect");

            foreach(var PSR in bolasZoneEffect.GetComponentsInChildren<ParticleSystemRenderer>())
            {
                PSR.transform.localScale = new Vector3(zoneRadius, zoneRadius, zoneRadius);
            }
            
            bolasZoneEffect.transform.parent = bolasZone.transform;

            /*
            BuffWard bolasWard = bolasZone.AddComponent<BuffWard>();
            bolasWard.radius = 18f;
            bolasWard.buffDef = Buffs.paralyze; //Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Common/bdCripple.asset").WaitForCompletion();
            bolasWard.interval = 0.75f;
            bolasWard.buffDuration = 1f;
            bolasWard.rangeIndicator = null;
            bolasWard.invertTeamFilter = true;
            bolasWard.floorWard = false;
            */
        }

        private static void CreateExplodingJavelin()
        {
            explodingJavelin = CloneProjectilePrefab("ToolbotGrenadeLauncherProjectile", "ExplodingJavelin");
            Rigidbody rb = explodingJavelin.GetComponent<Rigidbody>();
            rb.useGravity = true;

            explodingJavelin.GetComponent<Transform>().localScale = new Vector3(2.5f, 2.5f, 2.5f);

            ProjectileController javelinController = explodingJavelin.GetComponent<ProjectileController>();
            javelinController.ghostPrefab = CreateGhostPrefab("JavelinGhost");

            ProjectileSimple simple = explodingJavelin.GetComponent<ProjectileSimple>();
            simple.desiredForwardSpeed = 200f;

            ProjectileImpactExplosion impactExplosion = explodingJavelin.GetComponent<ProjectileImpactExplosion>();
            impactExplosion.blastRadius = Config.JavelinExplosionRadius.Value;
            impactExplosion.falloffModel = BlastAttack.FalloffModel.None;
            //impactExplosion.impactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/OmniImpactVFXLightningMage.prefab").WaitForCompletion();
            impactExplosion.impactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteLightning/LightningStakeNova.prefab").WaitForCompletion();
        }
        /*
        private static void CreateJavelin()
        {
            javelinPrefab = CloneProjectilePrefab("FMJ", "JavelinProjectile");
            ProjectileController controllerComponent = javelinPrefab.GetComponent<ProjectileController>();
            ProjectileOverlapAttack overlapComponent = javelinPrefab.GetComponent<ProjectileOverlapAttack>();

            //prefabs/projectileghosts/ArrowGhost
            controllerComponent.ghostPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Huntress/ArrowGhost.prefab").WaitForCompletion();

            javelinPrefab.GetComponent<Rigidbody>().useGravity = true;
            javelinPrefab.GetComponent<ProjectileDamage>().damageType = DamageType.Generic;
            UnityEngine.Object.Destroy(javelinPrefab.transform.Find("SweetSpotBehavior").gameObject);

            overlapComponent.impactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/ImpactMercFocusedAssault.prefab").WaitForCompletion();

            javelinPrefab.transform.GetChild(0).transform.localScale = new Vector3(3f, 3f, 3f);
        }
        /*
        private static void CreateBomb()
        {
            bombPrefab = CloneProjectilePrefab("CommandoGrenadeProjectile", "HenryBombProjectile");

            ProjectileImpactExplosion bombImpactExplosion = bombPrefab.GetComponent<ProjectileImpactExplosion>();
            InitializeImpactExplosion(bombImpactExplosion);

            bombImpactExplosion.blastRadius = 16f;
            bombImpactExplosion.destroyOnEnemy = true;
            bombImpactExplosion.lifetime = 12f;
            bombImpactExplosion.impactEffect = Modules.Assets.bombExplosionEffect;
            //bombImpactExplosion.lifetimeExpiredSound = Modules.Assets.CreateNetworkSoundEventDef("HenryBombExplosion");
            bombImpactExplosion.timerAfterImpact = true;
            bombImpactExplosion.lifetimeAfterImpact = 0.1f;

            ProjectileController bombController = bombPrefab.GetComponent<ProjectileController>();
            if (Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("HenryBombGhost") != null) bombController.ghostPrefab = CreateGhostPrefab("HenryBombGhost");
            bombController.startSound = "";
        }

        private static void InitializeImpactExplosion(ProjectileImpactExplosion projectileImpactExplosion)
        {
            projectileImpactExplosion.blastDamageCoefficient = 1f;
            projectileImpactExplosion.blastProcCoefficient = 1f;
            projectileImpactExplosion.blastRadius = 1f;
            projectileImpactExplosion.bonusBlastForce = Vector3.zero;
            projectileImpactExplosion.childrenCount = 0;
            projectileImpactExplosion.childrenDamageCoefficient = 0f;
            projectileImpactExplosion.childrenProjectilePrefab = null;
            projectileImpactExplosion.destroyOnEnemy = false;
            projectileImpactExplosion.destroyOnWorld = false;
            projectileImpactExplosion.falloffModel = RoR2.BlastAttack.FalloffModel.None;
            projectileImpactExplosion.fireChildren = false;
            projectileImpactExplosion.impactEffect = null;
            projectileImpactExplosion.lifetime = 0f;
            projectileImpactExplosion.lifetimeAfterImpact = 0f;
            projectileImpactExplosion.lifetimeRandomOffset = 0f;
            projectileImpactExplosion.offsetForLifetimeExpiredSound = 0f;
            projectileImpactExplosion.timerAfterImpact = false;

            projectileImpactExplosion.GetComponent<ProjectileDamage>().damageType = DamageType.Generic;
        }
        */
        internal static void AddProjectile(GameObject projectileToAdd)
        {
            Modules.Content.AddProjectilePrefab(projectileToAdd);
        }

        private static GameObject CreateGhostPrefab(string ghostName)
        {
            GameObject ghostPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>(ghostName);
            if (!ghostPrefab.GetComponent<NetworkIdentity>()) ghostPrefab.AddComponent<NetworkIdentity>();
            if (!ghostPrefab.GetComponent<ProjectileGhostController>()) ghostPrefab.AddComponent<ProjectileGhostController>();

            //Modules.Assets.ConvertAllRenderersToHopooShader(ghostPrefab);

            return ghostPrefab;
        }

        private static GameObject CloneProjectilePrefab(string prefabName, string newPrefabName)
        {
            GameObject newPrefab = PrefabAPI.InstantiateClone(RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/" + prefabName), newPrefabName);
            return newPrefab;
        }
    }
}