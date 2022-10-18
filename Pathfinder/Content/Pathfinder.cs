using BepInEx.Configuration;
using Pathfinder.Modules.Characters;
using Pathfinder.Components;
using Skillstates.Pathfinder;
using Skillstates.Pathfinder.Command;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using R2API;
using Pathfinder.Modules.CustomSkillDefs;
using UnityEngine.UI;
using EntityStates;
using Pathfinder.Modules.NPC;

namespace Pathfinder.Modules.Survivors
{
    internal class Pathfinder : SurvivorBase
    {
        public override string bodyName => "Pathfinder";

        public const string PF_PREFIX = PathfinderPlugin.DEVELOPER_PREFIX + "_PATHFINDER_BODY_";
        //used when registering your survivor's language tokens
        public override string survivorTokenPrefix => PF_PREFIX;

        internal static float goForThroatCD = 12f;

        public override BodyInfo bodyInfo { get; set; } = new BodyInfo
        {
            bodyName = "PathfinderBody",
            bodyNameToken = PathfinderPlugin.DEVELOPER_PREFIX + "_PATHFINDER_BODY_NAME",
            subtitleNameToken = PathfinderPlugin.DEVELOPER_PREFIX + "_PATHFINDER_BODY_SUBTITLE",

            characterPortrait = Assets.mainAssetBundle.LoadAsset<Texture>("texPathfinderIcon"),
            //bodyColor = new Color(62f / 255f, 162f / 255f, 82f / 255f),
            bodyColor = new Color32(62, 162, 82, 255),

            crosshair = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/StandardCrosshair.prefab").WaitForCompletion(),
            podPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),

            maxHealth = 110f,
            healthGrowth = 33f,
            healthRegen = 1f,
            armor = 0f,
            jumpCount = 1,
        };

        public override CustomRendererInfo[] customRendererInfos { get; set; } = new CustomRendererInfo[]
        {
                new CustomRendererInfo
                {
                    childName = "Pathfinder"
                },
                new CustomRendererInfo
                {
                    childName = "Shaft",
                },
                new CustomRendererInfo
                {
                    childName = "Spearhead",
                },
                new CustomRendererInfo
                {
                    childName = "Vest",
                },
                new CustomRendererInfo
                {
                    childName = "Drape",
                },
                new CustomRendererInfo
                {
                    childName = "Poncho",
                },
                new CustomRendererInfo
                {
                    childName = "BolasMesh",
                }
        };

        public override UnlockableDef characterUnlockableDef => null;

        public override Type characterMainState => typeof(EntityStates.GenericCharacterMain);

        public override ItemDisplaysBase itemDisplays => new PathfinderItemDisplays();

        //if you have more than one character, easily create a config to enable/disable them like this
        public override ConfigEntry<bool> characterEnabledConfig => Modules.Config.CharacterEnableConfig(bodyName);

        private static UnlockableDef masterySkinUnlockableDef;
        public static SkinDef HeadhunterSkin;

        public override void InitializeCharacter()
        {
            base.InitializeCharacter();
            PathfinderPlugin.pathfinderBodyPrefab = this.bodyPrefab;
            Hooks();
            //SetCoreTransform();
        }

        private void Hooks()
        {
            On.RoR2.ModelSkinController.ApplySkin += ModelSkinController_ApplySkin;
        }

        private void ModelSkinController_ApplySkin(On.RoR2.ModelSkinController.orig_ApplySkin orig, ModelSkinController self, int skinIndex)
        {
            if (self.skins[skinIndex].skinIndex == HeadhunterSkin.skinIndex)
            {
                Log.Warning("Changing to Headhunter");
            }
        }

        private void SetCoreTransform()
        {
            ChildLocator childLocator = bodyPrefab.GetComponentInChildren<ChildLocator>();
            GameObject model = childLocator.gameObject;
            CharacterBody characterBody = bodyPrefab.GetComponent<CharacterBody>();

            Transform baseTransform = childLocator.FindChild("BaseBone");
            model.GetComponent<CharacterModel>().coreTransform = baseTransform;
            characterBody.coreTransform = baseTransform;
        }

        public override void InitializeUnlockables()
        {
            //uncomment this when you have a mastery skin. when you do, make sure you have an icon too
            //masterySkinUnlockableDef = Modules.Unlockables.AddUnlockable<Modules.Achievements.MasteryAchievement>();
        }

        protected override void InitializeDisplayPrefab()
        {
            base.InitializeDisplayPrefab();
            CharacterSelectSurvivorPreviewDisplayController CSSpreview = displayPrefab.AddComponent<CharacterSelectSurvivorPreviewDisplayController>();
            CSSpreview.bodyPrefab = this.bodyPrefab;
        }

        public override void InitializeHitboxes()
        {
            ChildLocator childLocator = bodyPrefab.GetComponentInChildren<ChildLocator>();
            GameObject model = childLocator.gameObject;
            
            Transform thrustHitbox = childLocator.FindChild("SpearHitbox");
            Modules.Prefabs.SetupHitbox(model, thrustHitbox, "Spear");
        }

        protected override void AddMyComponents()
        {
            bodyPrefab.AddComponent<FalconerComponent>();
            bodyPrefab.AddComponent<OverrideController>();
            //bodyPrefab.AddComponent<SquallBatteryComponent>();

            bodyPrefab.AddComponent<CommandTracker>();
            GameObject customTracker = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/HuntressTrackingIndicator.prefab").WaitForCompletion(), "CommandTracker");
            customTracker.transform.Find("Core Pip").gameObject.GetComponent<SpriteRenderer>().color = Color.red;
            customTracker.transform.Find("Core, Dark").gameObject.GetComponent<SpriteRenderer>().color = Color.red;
            foreach (SpriteRenderer i in customTracker.transform.Find("Holder").gameObject.GetComponentsInChildren<SpriteRenderer>())
            {
                i.color = Color.red;
            }

            CommandTracker.trackerPrefab = customTracker;

            GameObject commandCrosshair = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Engi/EngiPaintCrosshair.prefab").WaitForCompletion(), "CommandCrosshair");
            var square = commandCrosshair.transform.Find("Holder").Find("Square").GetComponent<Image>();
            square.color = Color.red;

            var holder = commandCrosshair.transform.Find("Holder");
            holder.Find("AmmoArea").gameObject.SetActive(false);
            holder.Find("ReadyContainer").Find("Text").gameObject.SetActive(false);
            holder.Find("ReadyContainer").Find("Square, Outer").GetComponent<Image>().color = new Color(0.579f, 0f, 0f);
            Assets.commandCrosshair = commandCrosshair;
        }

        public override void InitializeSkills()
        {
            Modules.Skills.CreateSkillFamilies(bodyPrefab);
            //Modules.Skills.CreateSquallCommandFamilies(bodyPrefab);
            string prefix = PathfinderPlugin.DEVELOPER_PREFIX;

            #region Primary
            SkillDef primarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_PATHFINDER_BODY_PRIMARY_THRUST_NAME",
                skillNameToken = prefix + "_PATHFINDER_BODY_PRIMARY_THRUST_NAME",
                skillDescriptionToken = prefix + "_PATHFINDER_BODY_PRIMARY_THRUST_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texThrustIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(Thrust)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] {"KEYWORD_PIERCE"}
            });


            Modules.Skills.AddPrimarySkills(bodyPrefab, primarySkillDef);
            #endregion

            #region Secondary
            SkillDef javelinSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_PATHFINDER_BODY_SECONDARY_JAVELIN_NAME",
                skillNameToken = prefix + "_PATHFINDER_BODY_SECONDARY_JAVELIN_NAME",
                skillDescriptionToken = prefix + "_PATHFINDER_BODY_SECONDARY_JAVELIN_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texJavelinIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(JavelinToss)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });

            OverrideController.javelinSkill = javelinSkillDef;
            Modules.Content.AddSkillDef(javelinSkillDef);

            SkillDef dashSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_PATHFINDER_BODY_SECONDARY_DASH_NAME",
                skillNameToken = prefix + "_PATHFINDER_BODY_SECONDARY_DASH_NAME",
                skillDescriptionToken = prefix + "_PATHFINDER_BODY_SECONDARY_DASH_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texEvadeIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(Evade)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 2,
                baseRechargeInterval = Config.dashCD.Value,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = false,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });

            SkillDef diveSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_PATHFINDER_BODY_SECONDARY_DASH_NAME",
                skillNameToken = prefix + "_PATHFINDER_BODY_SECONDARY_DASH_NAME",
                skillDescriptionToken = prefix + "_PATHFINDER_BODY_SECONDARY_DASH_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texEvadeIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(Evade)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 2,
                baseRechargeInterval = Config.dashCD.Value,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = false,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });

            Modules.Skills.AddSecondarySkills(bodyPrefab, dashSkillDef);
            #endregion

            #region Utility
            SkillDef spinSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_PATHFINDER_BODY_UTILITY_SPIN_NAME",
                skillNameToken = prefix + "_PATHFINDER_BODY_UTILITY_SPIN_NAME",
                skillDescriptionToken = prefix + "_PATHFINDER_BODY_UTILITY_SPIN_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSpinIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(AirFlip)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = Config.rendingTalonsCD.Value,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = false,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] {"KEYWORD_UNPOLISHED"}
            });

            SkillDef bolaSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_PATHFINDER_BODY_UTILITY_BOLAS_NAME",
                skillNameToken = prefix + "_PATHFINDER_BODY_UTILITY_BOLAS_NAME",
                skillDescriptionToken = prefix + "_PATHFINDER_BODY_UTILITY_BOLAS_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texBolasIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(AimBolas)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = Config.bolasCD.Value,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = false,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = true,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] {"KEYWORD_SHOCKING", "KEYWORD_ELECTROCUTE"}
            });

            Modules.Skills.AddUtilitySkills(bodyPrefab, bolaSkillDef, spinSkillDef);
            #endregion

            #region Special
            SkillDef commandSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_PATHFINDER_BODY_SPECIAL_COMMAND_NAME",
                skillNameToken = prefix + "_PATHFINDER_BODY_SPECIAL_COMMAND_NAME",
                skillDescriptionToken = prefix + "_PATHFINDER_BODY_SPECIAL_COMMAND_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texCommandIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(CommandMode)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = false,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] { "KEYWORD_ATTACK", "KEYWORD_FOLLOW", "KEYWORD_SQUALL_SPECIAL" }
            });

            SkillDef utilityCommandSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_PATHFINDER_BODY_SPECIAL_COMMAND2_NAME",
                skillNameToken = prefix + "_PATHFINDER_BODY_SPECIAL_COMMAND2_NAME",
                skillDescriptionToken = prefix + "_PATHFINDER_BODY_SPECIAL_COMMAND2_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texCommandIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(CommandMode)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = false,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] { "KEYWORD_ATTACK", "KEYWORD_FOLLOW", "KEYWORD_SQUALL_SPECIAL" }
            });
            OverrideController.utilityCommandSkillDef = utilityCommandSkillDef;
            Modules.Skills.AddSpecialSkills(bodyPrefab, commandSkillDef); //utilityCommandSkillDef);

            AttackCommandSkillDef attackCommand = Modules.Skills.CreateAttackCommandSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_PATHFINDER_BODY_SPECIAL_ATTACK_NAME",
                skillNameToken = prefix + "_PATHFINDER_BODY_SPECIAL_ATTACK_NAME",
                skillDescriptionToken = prefix + "_PATHFINDER_BODY_SPECIAL_ATTACK_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texAttackIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(AttackCommand)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 1f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
            });
            attackCommand.dontAllowPastMaxStocks = true;
            OverrideController.attackCommand = attackCommand;
            Modules.Content.AddSkillDef(attackCommand);

            SkillDef followCommand = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_PATHFINDER_BODY_SPECIAL_FOLLOW_NAME",
                skillNameToken = prefix + "_PATHFINDER_BODY_SPECIAL_FOLLOW_NAME",
                skillDescriptionToken = prefix + "_PATHFINDER_BODY_SPECIAL_FOLLOW_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texFollowIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(FollowCommand)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 1f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
            });
            followCommand.dontAllowPastMaxStocks = true;
            OverrideController.followCommand = followCommand;
            Modules.Content.AddSkillDef(followCommand);


            SkillDef cancelSkill = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Engi/EngiCancelTargetingDummy.asset").WaitForCompletion();
            SkillDef cancelCommand = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_PATHFINDER_BODY_SPECIAL_CANCEL_NAME",
                skillNameToken = prefix + "_PATHFINDER_BODY_SPECIAL_CANCEL_NAME",
                skillDescriptionToken = prefix + "_PATHFINDER_BODY_SPECIAL_CANCEL_DESCRIPTION",
                skillIcon = cancelSkill.icon,
                activationState = new EntityStates.SerializableEntityStateType(typeof(Idle)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 1f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
            });
            cancelCommand.dontAllowPastMaxStocks = true;
            OverrideController.cancelCommand = cancelCommand;
            Modules.Content.AddSkillDef(cancelCommand);

            SpecialCommandSkillDef squallSpecial = Modules.Skills.CreateSpecialSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_SQUALL_SPECIAL_GOFORTHROAT_NAME",
                skillNameToken = prefix + "_SQUALL_SPECIAL_GOFORTHROAT_NAME",
                skillDescriptionToken = prefix + "_SQUALL_SPECIAL_GOFORTHROAT_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSquallEvisIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SpecialCommand)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = Config.goForThroatCD.Value,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = false,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });
            OverrideController.specialCommand = squallSpecial;
            Modules.Content.AddSkillDef(squallSpecial);
            //Modules.Skills.AddSquallSpecial(bodyPrefab, squallSpecial);
            #endregion
        }

        public override void InitializeSkins()
        {
            GameObject model = bodyPrefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();

            ModelSkinController skinController = model.AddComponent<ModelSkinController>();
            ChildLocator childLocator = model.GetComponent<ChildLocator>();

            SkinnedMeshRenderer mainRenderer = characterModel.mainSkinnedMeshRenderer;

            CharacterModel.RendererInfo[] defaultRenderers = characterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();

            #region DefaultSkin
            SkinDef defaultSkin = Modules.Skins.CreateSkinDef(PathfinderPlugin.DEVELOPER_PREFIX + "_PATHFINDER_BODY_DEFAULT_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texMainSkin"),
                defaultRenderers,
                mainRenderer,
                model);

            string meshString = "mesh";
            defaultSkin.meshReplacements = Skins.getMeshReplacements(defaultRenderers, new string[]
            {
                meshString + "Pathfinder",
                meshString + "Shaft",
                meshString + "Spearhead",
                meshString + "Vest",
                meshString + "Drape",
                meshString + "Poncho",
                null
            });

            skins.Add(defaultSkin);
            #endregion
            
            #region MasterySkin
            Material masteryMat = Modules.Materials.CreateHopooMaterial("matHeadhunter");

            Material[] matArray = new Material[defaultRenderers.Length];
            for(int i = 0; i < matArray.Length; i++)
            {
                matArray[i] = masteryMat;
            }

            CharacterModel.RendererInfo[] masteryRendererInfos = Skins.getRendererMaterials(defaultRenderers, matArray);

            SkinDef masterySkin = Modules.Skins.CreateSkinDef(PathfinderPlugin.DEVELOPER_PREFIX + "_PATHFINDER_BODY_MASTERY_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texMasteryAchievement"),
                masteryRendererInfos,
                mainRenderer,
                model,
                masterySkinUnlockableDef);

            string headhunter = "HeadHunter";
            masterySkin.meshReplacements = Skins.getMeshReplacements(masteryRendererInfos, new string[]
            {
                headhunter + "Body",
                headhunter + "Shaft",
                headhunter + "Spearhead",
                headhunter + "Vest",
                headhunter + "Drape",
                headhunter + "Backpack",
                null
            });

            for(int i = 0; i < masterySkin.rendererInfos.Length - 1; i++)
            {
                masterySkin.rendererInfos[i].defaultMaterial = masteryMat;
            }

            masterySkin.minionSkinReplacements = new SkinDef.MinionSkinReplacement[]
            {
                Squall.HHSquallReplacements
            };

            HeadhunterSkin = masterySkin;

            var CSSpreview = displayPrefab.GetComponent<CharacterSelectSurvivorPreviewDisplayController>();

            List<CharacterSelectSurvivorPreviewDisplayController.SkinChangeResponse> responses = new List<CharacterSelectSurvivorPreviewDisplayController.SkinChangeResponse>();

            CharacterSelectSurvivorPreviewDisplayController.SkinChangeResponse skinChangeResponse = new CharacterSelectSurvivorPreviewDisplayController.SkinChangeResponse
            {
                triggerSkin = masterySkin
            };

            responses.Add(skinChangeResponse);

            CSSpreview.skinChangeResponses = responses.ToArray();

            skins.Add(masterySkin);
            #endregion
            
            skinController.skins = skins.ToArray();
        }
    }
}