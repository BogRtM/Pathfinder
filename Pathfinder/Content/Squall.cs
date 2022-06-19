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

namespace Pathfinder.Content
{
    internal class Squall : CharacterBase
    {
        public override BodyInfo bodyInfo { get; set; } = new BodyInfo()
        {
            bodyNameToClone = "Drone1",
            bodyName = "SquallBody",
            crosshair = Modules.Assets.LoadCrosshair("Standard"),
            moveSpeed = 21f,
            acceleration = 120f
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

        public static List<AISkillDriver> followDrivers = new List<AISkillDriver>();

        public override void InitializeCharacter()
        {
            base.InitializeCharacter();
            InitializeSquall();
            PathfinderPlugin.squallBodyPrefab = this.bodyPrefab;
        }

        public void InitializeSquall()
        {
            bodyPrefab.AddComponent<SquallController>();
            bodyPrefab.AddComponent<SquallTracker>();
            bodyPrefab.AddComponent<SquallAIModes>();
            bodyPrefab.AddComponent<SquallPointer>();

            foreach (var i in bodyPrefab.GetComponents<AkEvent>())
            {
                UnityEngine.Object.DestroyImmediate(i);
            }
            CreateSquallMaster();
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

            SquallAIModes aiModes = bodyPrefab.GetComponent<SquallAIModes>();
            
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
            hardLeash.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            hardLeash.ignoreNodeGraph = false;
            hardLeash.moveInputScale = 1f;
            hardLeash.driverUpdateTimerOverride = 3f;
            hardLeash.shouldSprint = true;
            hardLeash.shouldTapButton = false;
            hardLeash.shouldFireEquipment = false;
            hardLeash.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            hardLeash.minTargetHealthFraction = Mathf.NegativeInfinity;
            hardLeash.maxTargetHealthFraction = Mathf.Infinity;
            hardLeash.minUserHealthFraction = float.NegativeInfinity;
            hardLeash.maxUserHealthFraction = float.PositiveInfinity;
            hardLeash.skillSlot = SkillSlot.None;
            aiModes.followDrivers.Add(hardLeash);

            AISkillDriver softLeash = masterPrefab.AddComponent<AISkillDriver>();
            softLeash.customName = "SoftLeashToLeader";
            softLeash.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            softLeash.moveTargetType = AISkillDriver.TargetType.CurrentLeader;
            softLeash.activationRequiresAimConfirmation = false;
            softLeash.activationRequiresTargetLoS = false;
            softLeash.selectionRequiresTargetLoS = false;
            softLeash.maxTimesSelected = -1;
            softLeash.maxDistance = float.PositiveInfinity;
            softLeash.minDistance = 30f;
            softLeash.requireSkillReady = false;
            softLeash.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            softLeash.ignoreNodeGraph = false;
            softLeash.moveInputScale = 1f;
            softLeash.driverUpdateTimerOverride = 0.05f;
            softLeash.shouldSprint = false;
            softLeash.shouldTapButton = false;
            softLeash.shouldFireEquipment = false;
            softLeash.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            softLeash.minTargetHealthFraction = Mathf.NegativeInfinity;
            softLeash.maxTargetHealthFraction = Mathf.Infinity;
            softLeash.minUserHealthFraction = float.NegativeInfinity;
            softLeash.maxUserHealthFraction = float.PositiveInfinity;
            softLeash.skillSlot = SkillSlot.None;
            aiModes.followDrivers.Add(softLeash);

            AISkillDriver chaseTarget = masterPrefab.AddComponent<AISkillDriver>();
            chaseTarget.customName = "ChaseTarget";
            chaseTarget.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            chaseTarget.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            chaseTarget.activationRequiresAimConfirmation = false;
            chaseTarget.activationRequiresTargetLoS = true;
            chaseTarget.selectionRequiresTargetLoS = true;
            chaseTarget.maxTimesSelected = -1;
            chaseTarget.maxDistance = float.PositiveInfinity;
            chaseTarget.minDistance = 30f;
            chaseTarget.requireSkillReady = true;
            chaseTarget.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            chaseTarget.ignoreNodeGraph = false;
            chaseTarget.moveInputScale = 1f;
            chaseTarget.driverUpdateTimerOverride = -1f;
            chaseTarget.shouldSprint = false;
            chaseTarget.shouldFireEquipment = false;
            chaseTarget.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            chaseTarget.minTargetHealthFraction = Mathf.NegativeInfinity;
            chaseTarget.maxTargetHealthFraction = Mathf.Infinity;
            chaseTarget.minUserHealthFraction = float.NegativeInfinity;
            chaseTarget.maxUserHealthFraction = float.PositiveInfinity;
            chaseTarget.skillSlot = SkillSlot.Primary;
            aiModes.attackDrivers.Add(chaseTarget);

            AISkillDriver attackTarget = masterPrefab.AddComponent<AISkillDriver>();
            attackTarget.customName = "StrafeTarget";
            attackTarget.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            attackTarget.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            attackTarget.activationRequiresAimConfirmation = false;
            attackTarget.activationRequiresTargetLoS = true;
            attackTarget.selectionRequiresTargetLoS = true;
            attackTarget.maxTimesSelected = -1;
            attackTarget.maxDistance = 30f;
            attackTarget.minDistance = 0f;
            attackTarget.requireSkillReady = true;
            attackTarget.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            attackTarget.ignoreNodeGraph = false;
            attackTarget.moveInputScale = 1f;
            attackTarget.driverUpdateTimerOverride = -1f;
            attackTarget.shouldSprint = false;
            attackTarget.shouldFireEquipment = false;
            attackTarget.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            attackTarget.minTargetHealthFraction = Mathf.NegativeInfinity;
            attackTarget.maxTargetHealthFraction = Mathf.Infinity;
            attackTarget.minUserHealthFraction = float.NegativeInfinity;
            attackTarget.maxUserHealthFraction = float.PositiveInfinity;
            attackTarget.skillSlot = SkillSlot.Primary;
            aiModes.attackDrivers.Add(attackTarget);

            /*
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
            hardLeash.driverUpdateTimerOverride = 3f;
            hardLeash.shouldSprint = true;
            hardLeash.shouldTapButton = false;
            hardLeash.shouldFireEquipment = false;
            hardLeash.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            hardLeash.minTargetHealthFraction = Mathf.NegativeInfinity;
            hardLeash.maxTargetHealthFraction = Mathf.Infinity;
            hardLeash.minUserHealthFraction = float.NegativeInfinity;
            hardLeash.maxUserHealthFraction = float.NegativeInfinity;
            hardLeash.skillSlot = SkillSlot.None;
            
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
            softLeash.aimType = AISkillDriver.AimType.AtCurrentLeader;
            softLeash.ignoreNodeGraph = false;
            softLeash.moveInputScale = 1f;
            softLeash.driverUpdateTimerOverride = 0.05f;
            softLeash.shouldSprint = false;
            softLeash.shouldTapButton = false;
            softLeash.shouldFireEquipment = false;
            softLeash.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            softLeash.minTargetHealthFraction = Mathf.NegativeInfinity;
            softLeash.maxTargetHealthFraction = Mathf.Infinity;
            softLeash.minUserHealthFraction = float.NegativeInfinity;
            softLeash.maxUserHealthFraction = float.NegativeInfinity;
            softLeash.skillSlot = SkillSlot.None;

            AISkillDriver idleNearLeader = masterPrefab.AddComponent<AISkillDriver>();
            idleNearLeader.customName = "IdleNearLeader";
            idleNearLeader.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            idleNearLeader.moveTargetType = AISkillDriver.TargetType.CurrentLeader;
            idleNearLeader.activationRequiresAimConfirmation = false;
            idleNearLeader.activationRequiresTargetLoS = false;
            idleNearLeader.selectionRequiresTargetLoS = false;
            idleNearLeader.maxTimesSelected = -1;
            idleNearLeader.maxDistance = 20f;
            idleNearLeader.minDistance = 0f;
            idleNearLeader.requireSkillReady = false;
            idleNearLeader.aimType = AISkillDriver.AimType.AtMoveTarget;
            idleNearLeader.ignoreNodeGraph = false;
            idleNearLeader.moveInputScale = 1f;
            idleNearLeader.driverUpdateTimerOverride = -1f;
            idleNearLeader.shouldSprint = false;
            idleNearLeader.shouldTapButton = false;
            idleNearLeader.shouldFireEquipment = false;
            idleNearLeader.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            idleNearLeader.minTargetHealthFraction = Mathf.NegativeInfinity;
            idleNearLeader.maxTargetHealthFraction = Mathf.Infinity;
            idleNearLeader.minUserHealthFraction = float.NegativeInfinity;
            idleNearLeader.maxUserHealthFraction = float.NegativeInfinity;
            idleNearLeader.skillSlot = SkillSlot.None;
            */
            #endregion

            Modules.Content.AddMasterPrefab(masterPrefab);
            PathfinderPlugin.squallMasterPrefab = masterPrefab;
        }

        public override void InitializeSkills()
        {
            Modules.Skills.CreateSkillFamilies(bodyPrefab);
            string prefix = PathfinderPlugin.DEVELOPER_PREFIX;

            #region Primary
            SkillDef primarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_SQUALL_BODY_PRIMARY_GUN_NAME",
                skillNameToken = prefix + "_SQUALLL_BODY_PRIMARY_GUN_NAME",
                skillDescriptionToken = prefix + "_PATHFINDER_BODY_PRIMARY_GUN_DESCRIPTION",
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
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
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

            Modules.Skills.AddSecondarySkills(bodyPrefab, primarySkillDef);
            Modules.Skills.AddUtilitySkills(bodyPrefab, primarySkillDef);
            Modules.Skills.AddSpecialSkills(bodyPrefab, primarySkillDef);
        }

        public override void InitializeDoppelganger(string clone)
        {
        }
    }
}
