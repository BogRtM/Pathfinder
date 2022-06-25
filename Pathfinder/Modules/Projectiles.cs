using R2API;
using RoR2;
using RoR2.Projectile;
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

        internal static void RegisterProjectiles()
        {
            CreateBomb();
            //CreateJavelin();
            CreateExplodingJavelin();

            AddProjectile(bombPrefab);
        }

        private static void CreateExplodingJavelin()
        {
            explodingJavelin = CloneProjectilePrefab("ToolbotGrenadeLauncherProjectile", "ExplodingJavelin");
            Rigidbody rb = explodingJavelin.GetComponent<Rigidbody>();
            rb.useGravity = true;

            explodingJavelin.GetComponent<Transform>().localScale = new Vector3(2f, 2f, 2f);

            ProjectileSimple simple = explodingJavelin.GetComponent<ProjectileSimple>();
            simple.desiredForwardSpeed = 200f;

            ProjectileImpactExplosion impactExplosion = explodingJavelin.GetComponent<ProjectileImpactExplosion>();
            impactExplosion.blastRadius = 12f;
            impactExplosion.falloffModel = BlastAttack.FalloffModel.None;
            impactExplosion.impactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Captain/CaptainTazerNova.prefab").WaitForCompletion();
        }

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

        internal static void AddProjectile(GameObject projectileToAdd)
        {
            Modules.Content.AddProjectilePrefab(projectileToAdd);
        }

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

        private static GameObject CreateGhostPrefab(string ghostName)
        {
            GameObject ghostPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>(ghostName);
            if (!ghostPrefab.GetComponent<NetworkIdentity>()) ghostPrefab.AddComponent<NetworkIdentity>();
            if (!ghostPrefab.GetComponent<ProjectileGhostController>()) ghostPrefab.AddComponent<ProjectileGhostController>();

            Modules.Assets.ConvertAllRenderersToHopooShader(ghostPrefab);

            return ghostPrefab;
        }

        private static GameObject CloneProjectilePrefab(string prefabName, string newPrefabName)
        {
            GameObject newPrefab = PrefabAPI.InstantiateClone(RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/" + prefabName), newPrefabName);
            return newPrefab;
        }
    }
}