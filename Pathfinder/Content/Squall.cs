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

namespace Pathfinder.Content.NPC
{
    internal class Squall : CharacterBase
    {
        public override BodyInfo bodyInfo { get; set; } = new BodyInfo()
        {
            bodyNameToClone = "Drone1",
            bodyName = "SquallBody",
            bodyNameToken = PathfinderPlugin.DEVELOPER_PREFIX + "_SQUALL_BODY_NAME",
            crosshair = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/StandardCrosshair.prefab").WaitForCompletion(),
            maxHealth = 100f,
            healthGrowth = 100f * 0.3f,
            healthRegen = 1f,
            armor = 0f,
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

        public override Type characterMainState => typeof(SquallMainState);
        public override Type characterSpawnState => typeof(SquallMainState);

        public override string bodyName => "Squall";

        internal static List<string> followDrivers = new List<string>();
        internal static List<string> attackDrivers = new List<string>();
        internal static List<string> SquallBlackList = new List<string>()
        {
            "FreeChest",
            "TreasureCache",
            "TreasureCacheVoid",
            "FireRing",
            "IceRing",
            //"LunarPrimaryReplacement",
            //"LunarSecondaryReplacement",
            //"LunarSpecialReplacement",
            //"LunarUtilityReplacement",

        };
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
            bodyPrefab.AddComponent<BatteryComponent>();
            bodyPrefab.AddComponent<SquallVFXComponents>();

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

            //SquallController squallController = bodyPrefab.GetComponent<SquallController>();
            //squallController.followDrivers = new List<string>();
            //squallController.attackDrivers = new List<string>();

            AddSkillDrivers(masterPrefab);

            Modules.Content.AddMasterPrefab(masterPrefab);
            FalconerComponent.summonPrefab = masterPrefab;
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
            hardLeash.minDistance = 120f;
            hardLeash.requireSkillReady = false;
            hardLeash.aimType = AISkillDriver.AimType.AtCurrentLeader;
            hardLeash.ignoreNodeGraph = false;
            hardLeash.moveInputScale = 1f;
            hardLeash.driverUpdateTimerOverride = -1f;
            hardLeash.shouldSprint = true;
            hardLeash.shouldFireEquipment = false;
            hardLeash.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            hardLeash.minTargetHealthFraction = Mathf.NegativeInfinity;
            hardLeash.maxTargetHealthFraction = Mathf.Infinity;
            hardLeash.minUserHealthFraction = float.NegativeInfinity;
            hardLeash.maxUserHealthFraction = float.PositiveInfinity;
            hardLeash.skillSlot = SkillSlot.None;

            AISkillDriver strafeMissile = masterPrefab.AddComponent<AISkillDriver>();
            strafeMissile.customName = "ShootMissilesStrafe";
            strafeMissile.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            strafeMissile.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            strafeMissile.activationRequiresAimConfirmation = false;
            strafeMissile.activationRequiresTargetLoS = true;
            strafeMissile.selectionRequiresTargetLoS = false;
            strafeMissile.maxTimesSelected = -1;
            strafeMissile.maxDistance = 35f;
            strafeMissile.minDistance = 0f;
            strafeMissile.requireSkillReady = true;
            strafeMissile.aimType = AISkillDriver.AimType.AtMoveTarget;
            strafeMissile.ignoreNodeGraph = false;
            strafeMissile.moveInputScale = 1f;
            strafeMissile.driverUpdateTimerOverride = -1f;
            strafeMissile.shouldSprint = false;
            strafeMissile.shouldFireEquipment = false;
            strafeMissile.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            strafeMissile.minTargetHealthFraction = Mathf.NegativeInfinity;
            strafeMissile.maxTargetHealthFraction = Mathf.Infinity;
            strafeMissile.minUserHealthFraction = float.NegativeInfinity;
            strafeMissile.maxUserHealthFraction = float.PositiveInfinity;
            strafeMissile.skillSlot = SkillSlot.Secondary;
            if(!attackDrivers.Contains(strafeMissile.customName)) attackDrivers.Add(strafeMissile.customName);

            AISkillDriver chaseMissile = masterPrefab.AddComponent<AISkillDriver>();
            chaseMissile.customName = "ShootMissilesChase";
            chaseMissile.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            chaseMissile.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            chaseMissile.activationRequiresAimConfirmation = false;
            chaseMissile.activationRequiresTargetLoS = true;
            chaseMissile.selectionRequiresTargetLoS = false;
            chaseMissile.maxTimesSelected = -1;
            chaseMissile.maxDistance = 100f;
            chaseMissile.minDistance = 30f;
            chaseMissile.requireSkillReady = true;
            chaseMissile.aimType = AISkillDriver.AimType.AtMoveTarget;
            chaseMissile.ignoreNodeGraph = false;
            chaseMissile.moveInputScale = 1f;
            chaseMissile.driverUpdateTimerOverride = -1f;
            chaseMissile.shouldSprint = true;
            chaseMissile.shouldFireEquipment = false;
            chaseMissile.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            chaseMissile.minTargetHealthFraction = Mathf.NegativeInfinity;
            chaseMissile.maxTargetHealthFraction = Mathf.Infinity;
            chaseMissile.minUserHealthFraction = float.NegativeInfinity;
            chaseMissile.maxUserHealthFraction = float.PositiveInfinity;
            chaseMissile.skillSlot = SkillSlot.Secondary;
            if (!attackDrivers.Contains(chaseMissile.customName)) attackDrivers.Add(chaseMissile.customName);

            AISkillDriver strafeGun = masterPrefab.AddComponent<AISkillDriver>();
            strafeGun.customName = "ShootGunsStrafe";
            strafeGun.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            strafeGun.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            strafeGun.activationRequiresAimConfirmation = true;
            strafeGun.activationRequiresTargetLoS = true;
            strafeGun.selectionRequiresTargetLoS = false;
            strafeGun.maxTimesSelected = -1;
            strafeGun.maxDistance = 35f;
            strafeGun.minDistance = 0f;
            strafeGun.requireSkillReady = true;
            strafeGun.aimType = AISkillDriver.AimType.AtMoveTarget;
            strafeGun.ignoreNodeGraph = false;
            strafeGun.moveInputScale = 1f;
            strafeGun.driverUpdateTimerOverride = -1f;
            strafeGun.shouldSprint = false;
            strafeGun.shouldFireEquipment = false;
            strafeGun.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            strafeGun.minTargetHealthFraction = Mathf.NegativeInfinity;
            strafeGun.maxTargetHealthFraction = Mathf.Infinity;
            strafeGun.minUserHealthFraction = float.NegativeInfinity;
            strafeGun.maxUserHealthFraction = float.PositiveInfinity;
            strafeGun.skillSlot = SkillSlot.Primary;
            if (!attackDrivers.Contains(strafeGun.customName)) attackDrivers.Add(strafeGun.customName);

            AISkillDriver chaseGun = masterPrefab.AddComponent<AISkillDriver>();
            chaseGun.customName = "ShootGunsChase";
            chaseGun.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            chaseGun.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            chaseGun.activationRequiresAimConfirmation = true;
            chaseGun.activationRequiresTargetLoS = true;
            chaseGun.selectionRequiresTargetLoS = false;
            chaseGun.maxTimesSelected = -1;
            chaseGun.maxDistance = 100f;
            chaseGun.minDistance = 30f;
            chaseGun.requireSkillReady = true;
            chaseGun.aimType = AISkillDriver.AimType.AtMoveTarget;
            chaseGun.ignoreNodeGraph = false;
            chaseGun.moveInputScale = 1f;
            chaseGun.driverUpdateTimerOverride = -1f;
            chaseGun.shouldSprint = true;
            chaseGun.shouldFireEquipment = false;
            chaseGun.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            chaseGun.minTargetHealthFraction = Mathf.NegativeInfinity;
            chaseGun.maxTargetHealthFraction = Mathf.Infinity;
            chaseGun.minUserHealthFraction = float.NegativeInfinity;
            chaseGun.maxUserHealthFraction = float.PositiveInfinity;
            chaseGun.skillSlot = SkillSlot.Primary;
            if (!attackDrivers.Contains(chaseGun.customName)) attackDrivers.Add(chaseGun.customName);

            AISkillDriver chaseEnemies = masterPrefab.AddComponent<AISkillDriver>();
            chaseEnemies.customName = "ChaseEnemies";
            chaseEnemies.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            chaseEnemies.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            chaseEnemies.activationRequiresAimConfirmation = false;
            chaseEnemies.activationRequiresTargetLoS = false;
            chaseEnemies.selectionRequiresTargetLoS = false;
            chaseEnemies.maxTimesSelected = -1;
            chaseEnemies.maxDistance = float.PositiveInfinity;
            chaseEnemies.minDistance = 0f;
            chaseEnemies.requireSkillReady = true;
            chaseEnemies.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            chaseEnemies.ignoreNodeGraph = false;
            chaseEnemies.moveInputScale = 1f;
            chaseEnemies.driverUpdateTimerOverride = -1f;
            chaseEnemies.shouldSprint = false;
            chaseEnemies.shouldFireEquipment = false;
            chaseEnemies.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            chaseEnemies.minTargetHealthFraction = Mathf.NegativeInfinity;
            chaseEnemies.maxTargetHealthFraction = Mathf.Infinity;
            chaseEnemies.minUserHealthFraction = float.NegativeInfinity;
            chaseEnemies.maxUserHealthFraction = float.PositiveInfinity;
            chaseEnemies.skillSlot = SkillSlot.None;
            if (!attackDrivers.Contains(chaseEnemies.customName)) attackDrivers.Add(chaseEnemies.customName);

            AISkillDriver doNothing = masterPrefab.AddComponent<AISkillDriver>();
            doNothing.customName = "DoNothing";
            doNothing.movementType = AISkillDriver.MovementType.Stop;
            doNothing.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            doNothing.activationRequiresAimConfirmation = true;
            doNothing.activationRequiresTargetLoS = true;
            doNothing.selectionRequiresTargetLoS = false;
            doNothing.maxTimesSelected = -1;
            doNothing.maxDistance = float.PositiveInfinity;
            doNothing.minDistance = 0f;
            doNothing.requireSkillReady = true;
            doNothing.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            doNothing.ignoreNodeGraph = false;
            doNothing.moveInputScale = 1f;
            doNothing.driverUpdateTimerOverride = -1f;
            doNothing.shouldSprint = false;
            doNothing.shouldFireEquipment = false;
            doNothing.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            doNothing.minTargetHealthFraction = Mathf.NegativeInfinity;
            doNothing.maxTargetHealthFraction = Mathf.Infinity;
            doNothing.minUserHealthFraction = float.NegativeInfinity;
            doNothing.maxUserHealthFraction = float.PositiveInfinity;
            doNothing.skillSlot = SkillSlot.Primary;
            if (!attackDrivers.Contains(doNothing.customName)) attackDrivers.Add(doNothing.customName);

            AISkillDriver softLeash = masterPrefab.AddComponent<AISkillDriver>();
            softLeash.customName = "SoftLeashToLeader";
            softLeash.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            softLeash.moveTargetType = AISkillDriver.TargetType.CurrentLeader;
            softLeash.activationRequiresAimConfirmation = false;
            softLeash.activationRequiresTargetLoS = false;
            softLeash.selectionRequiresTargetLoS = false;
            softLeash.maxTimesSelected = -1;
            softLeash.maxDistance = float.PositiveInfinity;
            softLeash.minDistance = 20f;
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
                skillName = prefix + "_SQUALL_BODY_PRIMARY_GUNS_NAME",
                skillNameToken = prefix + "_SQUALL_BODY_PRIMARY_GUNS_NAME",
                skillDescriptionToken = prefix + "_SQUALL_BODY_PRIMARY_GUNS_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPrimaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(MountedGuns)),
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
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
            });

            Modules.Skills.AddPrimarySkills(bodyPrefab, primarySkillDef);
            #endregion

            #region Secondary
            SkillDef secondarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_SQUALL_BODY_SECONDARY_MISSILE_NAME",
                skillNameToken = prefix + "_SQUALL_BODY_SECONDARY_MISSILE_NAME",
                skillDescriptionToken = prefix + "_SQUALL_BODY_SECONDARY_MISSILE_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPrimaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(MissileLauncher)),
                activationStateMachineName = "Missiles",
                baseMaxStock = 1,
                baseRechargeInterval = 12f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
            });
            #endregion

            Modules.Skills.AddSecondarySkills(bodyPrefab, secondarySkillDef);

            SkillDef utilitySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_SQUALL_BODY_PRIMARY_MISSILE_NAME",
                skillNameToken = prefix + "_SQUALL_BODY_PRIMARY_MISSILE_NAME",
                skillDescriptionToken = prefix + "_SQUALL_BODY_PRIMARY_MISSILE_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPrimaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SquallMainState)),
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

            SkillDef specialSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_SQUALL_BODY_SPECIAL_SQUALLEVIS_NAME",
                skillNameToken = prefix + "_SQUALL_BODY_SPECIAL_SQUALLEVIS_NAME",
                skillDescriptionToken = prefix + "_SQUALL_BODY_SPECIAL_SQUALLEVIS_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSquallEvisIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SquallMainState)),
                activationStateMachineName = "Body",
                baseMaxStock = 1,
                baseRechargeInterval = Modules.Survivors.Pathfinder.goForThroatCD,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
            });

            Modules.Skills.AddSpecialSkills(bodyPrefab, specialSkillDef);
        }

        public override void InitializeDoppelganger(string clone)
        {
        }
    }
}
