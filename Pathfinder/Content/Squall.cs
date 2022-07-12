using BepInEx.Configuration;
using Pathfinder.Modules.Characters;
using Pathfinder.Modules;
using Pathfinder.Components;
using RoR2;
using RoR2.Skills;
using RoR2.CharacterAI;
using R2API;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

using Skillstates.Squall;
using EntityStates;
using System.Linq;

namespace Pathfinder.Content
{
    internal class Squall : CharacterBase
    {
        public override BodyInfo bodyInfo { get; set; } = new BodyInfo()
        {
            bodyNameToClone = "Drone1",
            bodyName = "SquallBody",
            bodyNameToken = PathfinderPlugin.DEVELOPER_PREFIX + "_SQUALL_BODY_NAME",
            crosshair = Modules.Assets.LoadCrosshair("Standard"),
            moveSpeed = 24f,
            acceleration = 150f,
            characterPortrait = Assets.mainAssetBundle.LoadAsset<Texture>("texBirdIcon")
        };

        public override CustomRendererInfo[] customRendererInfos { get; set; } = new CustomRendererInfo[]
        {
            new CustomRendererInfo
                {
                    childName = "Squall"
                }
        };

        public override Type characterMainState => typeof(EntityStates.FlyState);
        //public override Type characterSpawnState => typeof(EntityStates.FlyState);

        public override string bodyName => "Squall";

        internal static List<string> followDrivers = new List<string>();
        internal static List<string> attackDrivers = new List<string>();

        public override void InitializeCharacter()
        {
            base.InitializeCharacter();
            InitializeSquall();
            PathfinderPlugin.squallBodyPrefab = this.bodyPrefab;
        }

        public void InitializeSquall()
        {
            EntityStateMachine missileMachine = bodyPrefab.AddComponent<EntityStateMachine>();
            missileMachine.customName = "Missiles";

            NetworkStateMachine networkStateMachine = bodyPrefab.GetComponent<NetworkStateMachine>();
            networkStateMachine.stateMachines = networkStateMachine.stateMachines.Append(missileMachine).ToArray<EntityStateMachine>();

            SerializableEntityStateType idleStateType = new SerializableEntityStateType(typeof(Idle));
            missileMachine.initialStateType = idleStateType;
            missileMachine.mainStateType = idleStateType;

            bodyPrefab.AddComponent<SquallController>();
            bodyPrefab.AddComponent<SquallBatteryComponent>();
            bodyPrefab.AddComponent<SquallVFXComponent>();

            foreach (var i in bodyPrefab.GetComponents<AkEvent>())
            {
                UnityEngine.Object.DestroyImmediate(i);
            }

            CharacterBody squallBody = bodyPrefab.GetComponent<CharacterBody>();

            squallBody.bodyFlags |= CharacterBody.BodyFlags.Mechanical;

            CreateSquallMaster();
        }

        public override void InitializeHitboxes()
        {
            ChildLocator childLocator = bodyPrefab.GetComponentInChildren<ChildLocator>();
            GameObject model = childLocator.gameObject;

            //example of how to create a hitbox
            //Transform hitboxTransform = childLocator.FindChild("SwordHitbox");
            //Modules.Prefabs.SetupHitbox(model, hitboxTransform, "Sword");

            Transform diveHitbox = childLocator.FindChild("DiveHitbox");
            Modules.Prefabs.SetupHitbox(model, diveHitbox, "Dive");
        }

        private void CreateSquallMaster()
        {
            var masterPrefab = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Drones/Drone1Master.prefab").WaitForCompletion(), "SquallMaster");

            var newMaster = masterPrefab.GetComponent<CharacterMaster>();
            newMaster.bodyPrefab = this.bodyPrefab;

            var baseAI = masterPrefab.GetComponent<BaseAI>();
            baseAI.aimVectorMaxSpeed = 5000f;
            baseAI.aimVectorDampTime = 0.01f;
            //baseAI.enemyAttentionDuration = float.PositiveInfinity;

            SquallController squallController = bodyPrefab.GetComponent<SquallController>();
            squallController.followDrivers = new List<string>();
            squallController.attackDrivers = new List<string>();

            AddSkillDrivers(masterPrefab);

            Modules.Content.AddMasterPrefab(masterPrefab);
            PathfinderPlugin.squallMasterPrefab = masterPrefab;
        }

        private void AddSkillDrivers(GameObject masterPrefab)
        {
            #region AI
            foreach (AISkillDriver i in masterPrefab.GetComponentsInChildren<AISkillDriver>())
            {
                UnityEngine.Object.DestroyImmediate(i);
            }

            AISkillDriver hardLeash = masterPrefab.AddComponent<AISkillDriver>();
            hardLeash.customName = "HardLeashToLeader";
            hardLeash.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            hardLeash.moveTargetType = AISkillDriver.TargetType.CurrentLeader;
            hardLeash.activationRequiresAimConfirmation = false;
            hardLeash.activationRequiresTargetLoS = false;
            hardLeash.selectionRequiresTargetLoS = false;
            hardLeash.maxTimesSelected = -1;
            hardLeash.maxDistance = float.PositiveInfinity;
            hardLeash.minDistance = 60f;
            hardLeash.requireSkillReady = false;
            hardLeash.aimType = AISkillDriver.AimType.AtCurrentLeader;
            hardLeash.ignoreNodeGraph = false;
            hardLeash.moveInputScale = 1f;
            hardLeash.driverUpdateTimerOverride = -1f;
            hardLeash.shouldSprint = false;
            hardLeash.shouldFireEquipment = false;
            hardLeash.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            hardLeash.minTargetHealthFraction = Mathf.NegativeInfinity;
            hardLeash.maxTargetHealthFraction = Mathf.Infinity;
            hardLeash.minUserHealthFraction = float.NegativeInfinity;
            hardLeash.maxUserHealthFraction = float.PositiveInfinity;
            hardLeash.skillSlot = SkillSlot.None;

            AISkillDriver shootMissile = masterPrefab.AddComponent<AISkillDriver>();
            shootMissile.customName = "SoftLeashToLeader";
            shootMissile.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            shootMissile.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            shootMissile.activationRequiresAimConfirmation = false;
            shootMissile.activationRequiresTargetLoS = true;
            shootMissile.selectionRequiresTargetLoS = false;
            shootMissile.maxTimesSelected = -1;
            shootMissile.maxDistance = 70f;
            shootMissile.minDistance = 0f;
            shootMissile.requireSkillReady = true;
            shootMissile.aimType = AISkillDriver.AimType.AtMoveTarget;
            shootMissile.ignoreNodeGraph = false;
            shootMissile.moveInputScale = 1f;
            shootMissile.driverUpdateTimerOverride = -1f;
            shootMissile.shouldSprint = false;
            shootMissile.shouldFireEquipment = false;
            shootMissile.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            shootMissile.minTargetHealthFraction = Mathf.NegativeInfinity;
            shootMissile.maxTargetHealthFraction = Mathf.Infinity;
            shootMissile.minUserHealthFraction = float.NegativeInfinity;
            shootMissile.maxUserHealthFraction = float.PositiveInfinity;
            shootMissile.skillSlot = SkillSlot.Secondary;
            attackDrivers.Add(shootMissile.customName);

            AISkillDriver softLeash = masterPrefab.AddComponent<AISkillDriver>();
            softLeash.customName = "SoftLeashToLeader";
            softLeash.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            softLeash.moveTargetType = AISkillDriver.TargetType.CurrentLeader;
            softLeash.activationRequiresAimConfirmation = false;
            softLeash.activationRequiresTargetLoS = false;
            softLeash.selectionRequiresTargetLoS = false;
            softLeash.maxTimesSelected = -1;
            softLeash.maxDistance = 40f;
            softLeash.minDistance = 0f;
            softLeash.requireSkillReady = false;
            softLeash.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            softLeash.ignoreNodeGraph = false;
            softLeash.moveInputScale = 1f;
            softLeash.driverUpdateTimerOverride = -1f;
            softLeash.shouldSprint = false;
            softLeash.shouldFireEquipment = false;
            softLeash.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            softLeash.minTargetHealthFraction = Mathf.NegativeInfinity;
            softLeash.maxTargetHealthFraction = Mathf.Infinity;
            softLeash.minUserHealthFraction = float.NegativeInfinity;
            softLeash.maxUserHealthFraction = float.PositiveInfinity;
            softLeash.skillSlot = SkillSlot.None;

            AISkillDriver idleNear = masterPrefab.AddComponent<AISkillDriver>();
            idleNear.customName = "IdleNearLeader";
            idleNear.movementType = AISkillDriver.MovementType.Stop;
            idleNear.moveTargetType = AISkillDriver.TargetType.CurrentLeader;
            idleNear.activationRequiresAimConfirmation = false;
            idleNear.activationRequiresTargetLoS = false;
            idleNear.selectionRequiresTargetLoS = false;
            idleNear.maxTimesSelected = -1;
            idleNear.maxDistance = float.PositiveInfinity;
            idleNear.minDistance = 0f;
            idleNear.requireSkillReady = false;
            idleNear.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            idleNear.ignoreNodeGraph = false;
            idleNear.moveInputScale = 1f;
            idleNear.driverUpdateTimerOverride = -1f;
            idleNear.shouldSprint = false;
            idleNear.shouldFireEquipment = false;
            idleNear.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            idleNear.minTargetHealthFraction = Mathf.NegativeInfinity;
            idleNear.maxTargetHealthFraction = Mathf.Infinity;
            idleNear.minUserHealthFraction = float.NegativeInfinity;
            idleNear.maxUserHealthFraction = float.PositiveInfinity;
            idleNear.skillSlot = SkillSlot.None;
            #endregion
        }

        public override void InitializeSkills()
        {
            Modules.Skills.CreateSkillFamilies(bodyPrefab);
            string prefix = PathfinderPlugin.DEVELOPER_PREFIX;

            #region Primary
            SkillDef primarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_SQUALL_BODY_PRIMARY_DIVE_NAME",
                skillNameToken = prefix + "_SQUALLL_BODY_PRIMARY_DIVE_NAME",
                skillDescriptionToken = prefix + "_PATHFINDER_BODY_PRIMARY_DIVE_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPrimaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(DiveAttack)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 8f,
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
            SkillDef secondarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_SQUALL_BODY_PRIMARY_MISSILE_NAME",
                skillNameToken = prefix + "_SQUALLL_BODY_PRIMARY_MISSILE_NAME",
                skillDescriptionToken = prefix + "_PATHFINDER_BODY_PRIMARY_MISSILE_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPrimaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(MissileLauncher)),
                activationStateMachineName = "Missiles",
                baseMaxStock = 1,
                baseRechargeInterval = 6f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
            });
            #endregion

            Modules.Skills.AddSecondarySkills(bodyPrefab, secondarySkillDef);

            SkillDef utilitySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_SQUALL_BODY_PRIMARY_MISSILE_NAME",
                skillNameToken = prefix + "_SQUALLL_BODY_PRIMARY_MISSILE_NAME",
                skillDescriptionToken = prefix + "_PATHFINDER_BODY_PRIMARY_MISSILE_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPrimaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(MissileLauncher)),
                activationStateMachineName = "Missiles",
                baseMaxStock = 1,
                baseRechargeInterval = 6f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
            });

            Modules.Skills.AddUtilitySkills(bodyPrefab, primarySkillDef);
            Modules.Skills.AddSpecialSkills(bodyPrefab, primarySkillDef);
        }

        public override void InitializeDoppelganger(string clone)
        {
        }
    }
}
