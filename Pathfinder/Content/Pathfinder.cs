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
            bodyColor = new Color(62f / 255f, 162f / 255f, 82f / 255f),

            crosshair = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/SimpleDotCrosshair.prefab").WaitForCompletion(),
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
        public override ConfigEntry<bool> characterEnabledConfig => null; //Modules.Config.CharacterEnableConfig(bodyName);

        private static UnlockableDef masterySkinUnlockableDef;

        public override void InitializeCharacter()
        {
            base.InitializeCharacter();
            PathfinderPlugin.pathfinderBodyPrefab = this.bodyPrefab;
            //SetCoreTransform();
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

            #region Empower
            SkillDef javelinSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_PATHFINDER_BODY_EMPOWER_JAVELIN_NAME",
                skillNameToken = prefix + "_PATHFINDER_BODY_EMPOWER_JAVELIN_NAME",
                skillDescriptionToken = prefix + "_PATHFINDER_BODY_EMPOWER_JAVELIN_DESCRIPTION",
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
            #endregion

            #region Primary
            //Creates a skilldef for a typical primary 
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
            });


            Modules.Skills.AddPrimarySkills(bodyPrefab, primarySkillDef);
            #endregion

            #region Secondary
            SkillDef pursuitSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_PATHFINDER_BODY_SECONDARY_DASH_NAME",
                skillNameToken = prefix + "_PATHFINDER_BODY_SECONDARY_DASH_NAME",
                skillDescriptionToken = prefix + "_PATHFINDER_BODY_SECONDARY_DASH_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texEvadeIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(Evade)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 2,
                baseRechargeInterval = 6f,
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

            Modules.Skills.AddSecondarySkills(bodyPrefab, pursuitSkillDef);
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
                baseRechargeInterval = 8f,
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
                stockToConsume = 1
            });

            SkillDef bolaSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_PATHFINDER_BODY_UTILITY_BOLAS_NAME",
                skillNameToken = prefix + "_PATHFINDER_BODY_UTILITY_BOLAS_NAME",
                skillDescriptionToken = prefix + "_PATHFINDER_BODY_UTILITY_BOLAS_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texBolasIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(ThrowBolas)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 16f,
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
                keywordTokens = new string[] {"KEYWORD_ELECTROCUTE"}
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

            Modules.Skills.AddSpecialSkills(bodyPrefab, commandSkillDef);

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
                skillName = prefix + "_PATHFINDER_BODY_SQUALL_SPECIAL_GOFORTHROAT_NAME",
                skillNameToken = prefix + "_PATHFINDER_BODY_SQUALL_SPECIAL_GOFORTHROAT_NAME",
                skillDescriptionToken = prefix + "_PATHFINDER_BODY_SQUALL_SPECIAL_GOFORTHROAT_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSquallEvisIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SpecialCommand)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = goForThroatCD,
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
            OverrideController.squallSpecial = squallSpecial;
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

            defaultSkin.meshReplacements = new SkinDef.MeshReplacement[]
            {
                //place your mesh replacements here
                //unnecessary if you don't have multiple skins
                //new SkinDef.MeshReplacement
                //{
                //    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshPATHFINDERSword"),
                //    renderer = defaultRenderers[0].renderer
                //},
                //new SkinDef.MeshReplacement
                //{
                //    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshPATHFINDERGun"),
                //    renderer = defaultRenderers[1].renderer
                //},
                //new SkinDef.MeshReplacement
                //{
                //    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshPATHFINDER"),
                //    renderer = defaultRenderers[2].renderer
                //}
            };

            skins.Add(defaultSkin);
            #endregion

            //uncomment this when you have a mastery skin
            #region MasterySkin
            /*
            Material masteryMat = Modules.Materials.CreateHopooMaterial("matPATHFINDERAlt");
            CharacterModel.RendererInfo[] masteryRendererInfos = SkinRendererInfos(defaultRenderers, new Material[]
            {
                masteryMat,
                masteryMat,
                masteryMat,
                masteryMat
            });

            SkinDef masterySkin = Modules.Skins.CreateSkinDef(PATHFINDERPlugin.DEVELOPER_PREFIX + "_PATHFINDER_BODY_MASTERY_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texMasteryAchievement"),
                masteryRendererInfos,
                mainRenderer,
                model,
                masterySkinUnlockableDef);

            masterySkin.meshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshPATHFINDERSwordAlt"),
                    renderer = defaultRenderers[0].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshPATHFINDERAlt"),
                    renderer = defaultRenderers[2].renderer
                }
            };

            skins.Add(masterySkin);
            */
            #endregion

            skinController.skins = skins.ToArray();
        }
    }
}