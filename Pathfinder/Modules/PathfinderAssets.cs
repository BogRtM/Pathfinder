﻿using System.Reflection;
using R2API;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using RoR2;
using System.IO;
using System.Collections.Generic;
using RoR2.UI;
using System;
using TMPro;

namespace Pathfinder.Modules
{
    internal static class PathfinderAssets
    {
        // the assetbundle to load assets from
        internal static AssetBundle mainAssetBundle;

        //UI
        internal static GameObject BatteryMeter;
        internal static GameObject commandCrosshair;

        /* particle effects
        internal static GameObject swordSwingEffect;
        internal static GameObject swordHitImpactEffect;
        internal static GameObject bombExplosionEffect;
        */

        internal static GameObject thrustEffect;
        internal static GameObject thrustTipImpact;

        internal static GameObject dashEffect;
        internal static GameObject javEffect;

        internal static GameObject vaultEffect;

        internal static GameObject lightningRingEffect;
        internal static GameObject lineVisualizer;
        internal static GameObject explosionVisualizer;

        internal static GameObject squallEvisEffect;
        internal static GameObject squallDashEffect;
        internal static GameObject squallFollowFlash;
        internal static GameObject squallAttackFlash;

        // networked hit sounds
        internal static NetworkSoundEventDef swordHitSoundEvent;

        // cache these and use to create our own materials
        internal static Shader hotpoo = RoR2.LegacyResourcesAPI.Load<Shader>("Shaders/Deferred/HGStandard");
        internal static Material commandoMat;
        private static string[] assetNames = new string[0];

        // CHANGE THIS
        private const string assetFolder = "PF_Assets";
        private const string assetbundleName = "pathfinderassets";
        private const string soundbankFolder = "PF_Soundbanks";
        private const string soundbankName = "PathfinderBank.bnk";
        //change this to your project's name if/when you've renamed it
        private const string csProjName = "Pathfinder";

        public static string AssetBundlePath
        {
            get
            {
                return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(PathfinderPlugin.PInfo.Location), assetFolder, assetbundleName);
            }
        }

        internal static void Initialize()
        {
            if (assetbundleName == "myassetbundle")
            {
                Log.Error("AssetBundle name hasn't been changed. not loading any assets to avoid conflicts");
                return;
            }

            LoadAssetBundle();
            PopulateAssets();
        }

        internal static void LoadAssetBundle()
        {
            try
            {
                if (mainAssetBundle == null)
                {
                    mainAssetBundle = AssetBundle.LoadFromFile(AssetBundlePath);
                }

                if (mainAssetBundle)
                {
                    Log.Warning("Morris asset bundle loaded successfully");
                }
            }
            catch (Exception e)
            {
                Log.Error("Failed to load assetbundle. Make sure your assetbundle name is setup correctly\n" + e);
                return;
            }

            assetNames = mainAssetBundle.GetAllAssetNames();
        }

        internal static void PopulateAssets()
        {
            if (!mainAssetBundle)
            {
                Log.Error("There is no AssetBundle to load assets from.");
                return;
            }

            // feel free to delete everything in here and load in your own assets instead
            // it should work fine even if left as is- even if the assets aren't in the bundle
            /*
            swordHitSoundEvent = CreateNetworkSoundEventDef("HenrySwordHit");

            bombExplosionEffect = LoadEffect("BombExplosionEffect", "HenryBombExplosion");

            if (bombExplosionEffect)
            {
                ShakeEmitter shakeEmitter = bombExplosionEffect.AddComponent<ShakeEmitter>();
                shakeEmitter.amplitudeTimeDecay = true;
                shakeEmitter.duration = 0.5f;
                shakeEmitter.radius = 200f;
                shakeEmitter.scaleShakeRadiusWithLocalScale = false;

                shakeEmitter.wave = new Wave
                {
                    amplitude = 1f,
                    frequency = 40f,
                    cycleOffset = 0f
                };
            }

            swordSwingEffect = Assets.LoadEffect("HenrySwordSwingEffect", true);
            swordHitImpactEffect = Assets.LoadEffect("ImpactHenrySlash");
            */

            //Thrust effect
            thrustEffect = PathfinderAssets.LoadEffect("SpearThrust", false);
            thrustTipImpact = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/ImpactMercFocusedAssault.prefab").WaitForCompletion();

            //Javelin effects
            dashEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/SprintOutOfCombat/SprintActivate.prefab").WaitForCompletion();
            javEffect = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Vagrant/VagrantTrackingBombExplosion.prefab").WaitForCompletion(),
                "JavelinExplosionPrefab");
            if(!javEffect.GetComponent<NetworkIdentity>()) javEffect.AddComponent<NetworkIdentity>();
            AddNewEffectDef(javEffect, "Play_mage_m2_impact");

            //Vault effect
            vaultEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Feather/FeatherEffect.prefab").WaitForCompletion();

            //Bolas Zone effect
            lightningRingEffect = mainAssetBundle.LoadAsset<GameObject>("LightningRing");
            Texture2D blots = mainAssetBundle.LoadAsset<Texture2D>("Blots");
            Texture2D lightningCloud = Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/texCloudLightning1.png").WaitForCompletion();
            Material zoneMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/TPHealingNova/matGlowFlowerAreaIndicator.mat").WaitForCompletion();

            lightningRingEffect.AddComponent<NetworkIdentity>();

            var innerRing = lightningRingEffect.transform.GetChild(0).GetComponent<ParticleSystemRenderer>();
            innerRing.material = zoneMaterial;
            innerRing.material.SetTexture("_Cloud1Tex", lightningCloud);
            innerRing.material.SetTexture("_Cloud2Tex", lightningCloud);
            innerRing.material.SetColor("_TintColor", Color.blue);
            innerRing.material.SetFloat("_RimPower", 6f);
            innerRing.material.SetFloat("_RimStrength", 1f);
            
            var outerRing = lightningRingEffect.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystemRenderer>();
            outerRing.material = zoneMaterial;
            outerRing.material.SetTexture("_Cloud1Tex", lightningCloud);
            outerRing.material.SetTexture("_Cloud2Tex", lightningCloud);
            outerRing.material.SetFloat("_RimPower", 7f);
            outerRing.material.SetColor("_TintColor", Color.green);

            //Battery UI
            BatteryMeter = mainAssetBundle.LoadAsset<GameObject>("BatteryMeter");
            TextMeshProUGUI text = BatteryMeter.GetComponentInChildren<TextMeshProUGUI>();
            text.font = Addressables.LoadAssetAsync<TMP_FontAsset>("RoR2/Base/Common/Fonts/Bombardier/tmpBombDropshadow.asset").WaitForCompletion();

            squallFollowFlash = PathfinderAssets.LoadEffect("FlashFollow");
            squallAttackFlash = PathfinderAssets.LoadEffect("FlashAttack");
            squallEvisEffect = PathfinderAssets.LoadEffect("SquallEvisEffect");
            squallDashEffect = PathfinderAssets.LoadEffect("SquallDash");

            lineVisualizer = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/BasicThrowableVisualizer.prefab").WaitForCompletion();
            explosionVisualizer = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/HuntressArrowRainIndicator.prefab").WaitForCompletion();
        }

        private static GameObject CreateTracer(string originalTracerName, string newTracerName)
        {
            if (RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/" + originalTracerName) == null) return null;

            GameObject newTracer = PrefabAPI.InstantiateClone(RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/" + originalTracerName), newTracerName, true);

            if (!newTracer.GetComponent<EffectComponent>()) newTracer.AddComponent<EffectComponent>();
            if (!newTracer.GetComponent<VFXAttributes>()) newTracer.AddComponent<VFXAttributes>();
            if (!newTracer.GetComponent<NetworkIdentity>()) newTracer.AddComponent<NetworkIdentity>();

            newTracer.GetComponent<Tracer>().speed = 250f;
            newTracer.GetComponent<Tracer>().length = 50f;

            AddNewEffectDef(newTracer);

            return newTracer;
        }

        internal static NetworkSoundEventDef CreateNetworkSoundEventDef(string eventName)
        {
            NetworkSoundEventDef networkSoundEventDef = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            networkSoundEventDef.akId = AkSoundEngine.GetIDFromString(eventName);
            networkSoundEventDef.eventName = eventName;

            Modules.Content.AddNetworkSoundEventDef(networkSoundEventDef);

            return networkSoundEventDef;
        }

        internal static void ConvertAllRenderersToHopooShader(GameObject objectToConvert)
        {
            if (!objectToConvert) return;

            foreach (Renderer i in objectToConvert.GetComponentsInChildren<Renderer>())
            {
                i?.material?.SetHopooMaterial();
            }
        }

        internal static CharacterModel.RendererInfo[] SetupRendererInfos(GameObject obj)
        {
            MeshRenderer[] meshes = obj.GetComponentsInChildren<MeshRenderer>();
            CharacterModel.RendererInfo[] rendererInfos = new CharacterModel.RendererInfo[meshes.Length];

            for (int i = 0; i < meshes.Length; i++)
            {
                rendererInfos[i] = new CharacterModel.RendererInfo
                {
                    defaultMaterial = meshes[i].material,
                    renderer = meshes[i],
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false
                };
            }

            return rendererInfos;
        }


        public static GameObject LoadSurvivorModel(string modelName) {
            GameObject model = mainAssetBundle.LoadAsset<GameObject>(modelName);
            if (model == null) {
                Log.Error("Trying to load a null model- check to see if the name in your code matches the name of the object in Unity");
                return null;
            }

            return PrefabAPI.InstantiateClone(model, model.name, false);
        }

        internal static Texture LoadCharacterIconGeneric(string characterName)
        {
            return mainAssetBundle.LoadAsset<Texture>("tex" + characterName + "Icon");
        }

        internal static GameObject LoadCrosshair(string crosshairName)
        {
            if (RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Crosshair/" + crosshairName + "Crosshair") == null) return RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Crosshair/StandardCrosshair");
            return RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Crosshair/" + crosshairName + "Crosshair");
        }

        private static GameObject LoadEffect(string resourceName)
        {
            return LoadEffect(resourceName, "", false);
        }

        private static GameObject LoadEffect(string resourceName, string soundName)
        {
            return LoadEffect(resourceName, soundName, false);
        }

        private static GameObject LoadEffect(string resourceName, bool parentToTransform)
        {
            return LoadEffect(resourceName, "", parentToTransform);
        }

        private static GameObject LoadEffect(string resourceName, string soundName, bool parentToTransform)
        {
            bool assetExists = false;
            for (int i = 0; i < assetNames.Length; i++)
            {
                if (assetNames[i].Contains(resourceName.ToLowerInvariant()))
                {
                    assetExists = true;
                    i = assetNames.Length;
                }
            }

            if (!assetExists)
            {
                Log.Error("Failed to load effect: " + resourceName + " because it does not exist in the AssetBundle");
                return null;
            }

            GameObject newEffect = mainAssetBundle.LoadAsset<GameObject>(resourceName);

            newEffect.AddComponent<DestroyOnTimer>().duration = 12;
            newEffect.AddComponent<NetworkIdentity>();
            newEffect.AddComponent<VFXAttributes>().vfxPriority = VFXAttributes.VFXPriority.Always;
            var effect = newEffect.AddComponent<EffectComponent>();
            effect.applyScale = false;
            effect.effectIndex = EffectIndex.Invalid;
            effect.parentToReferencedTransform = parentToTransform;
            effect.positionAtReferencedTransform = true;
            effect.soundName = soundName;

            AddNewEffectDef(newEffect, soundName);

            return newEffect;
        }

        private static void AddNewEffectDef(GameObject effectPrefab)
        {
            AddNewEffectDef(effectPrefab, "");
        }

        private static void AddNewEffectDef(GameObject effectPrefab, string soundName)
        {
            EffectDef newEffectDef = new EffectDef();
            newEffectDef.prefab = effectPrefab;
            newEffectDef.prefabEffectComponent = effectPrefab.GetComponent<EffectComponent>();
            newEffectDef.prefabName = effectPrefab.name;
            newEffectDef.prefabVfxAttributes = effectPrefab.GetComponent<VFXAttributes>();
            newEffectDef.spawnSoundEventName = soundName;

            Modules.Content.AddEffectDef(newEffectDef);
        }
    }
}