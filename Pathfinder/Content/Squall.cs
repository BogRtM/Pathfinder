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
            crosshair = Modules.Assets.LoadCrosshair("Standard"),
            moveSpeed = 24f,
            acceleration = 150f
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

            SquallController squallController = bodyPrefab.GetComponent<SquallController>();
            squallController.followDrivers = new List<string>();
            squallController.attackDrivers = new List<string>();
            
            #region AI
            foreach (AISkillDriver i in masterPrefab.GetComponentsInChildren<AISkillDriver>())
            {
                UnityEngine.Object.DestroyImmediate(i);
            }

            AISkillDriver hardLeash = masterPrefab.AddComponent<AISkillDriver>();
            hardLeash.customName = "FollowHardLeashToLeader";
            hardLeash.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            hardLeash.moveTargetType = AISkillDriver.TargetType.CurrentLeader;
            hardLeash.activationRequiresAimConfirmation = false;
            hardLeash.activationRequiresTargetLoS = false;
            hardLeash.selectionRequiresTargetLoS = false;
            hardLeash.maxTimesSelected = -1;
            hardLeash.maxDistance = float.PositiveInfinity;
            hardLeash.minDistance = 60f;
            hardLeash.requireSkillReady = false;
            hardLeash.aimType = AISkillDriver.AimType.AtMoveTarget;
            hardLeash.ignoreNodeGraph = false;
            hardLeash.moveInputScale = 1f;
            hardLeash.driverUpdateTimerOverride = 1f;
            hardLeash.shouldSprint = false;
            hardLeash.shouldTapButton = false;
            hardLeash.shouldFireEquipment = false;
            hardLeash.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            hardLeash.minTargetHealthFraction = Mathf.NegativeInfinity;
            hardLeash.maxTargetHealthFraction = Mathf.Infinity;
            hardLeash.minUserHealthFraction = float.NegativeInfinity;
            hardLeash.maxUserHealthFraction = float.PositiveInfinity;
            hardLeash.skillSlot = SkillSlot.None;
            followDrivers.Add(hardLeash.customName);

            
            AISkillDriver softLeash = masterPrefab.AddComponent<AISkillDriver>();
            softLeash.customName = "FollowSoftLeashToLeader";
            softLeash.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            softLeash.moveTargetType = AISkillDriver.TargetType.CurrentLeader;
            softLeash.activationRequiresAimConfirmation = false;
            softLeash.activationRequiresTargetLoS = false;
            softLeash.selectionRequiresTargetLoS = false;
            softLeash.maxTimesSelected = -1;
            softLeash.maxDistance = float.PositiveInfinity;
            softLeash.minDistance = 40f;
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
            followDrivers.Add(softLeash.customName);
            

            AISkillDriver missileChase = masterPrefab.AddComponent<AISkillDriver>();
            missileChase.customName = "AttackMissileChaseM2";
            missileChase.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            missileChase.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            missileChase.activationRequiresAimConfirmation = true;
            missileChase.activationRequiresTargetLoS = true;
            missileChase.selectionRequiresTargetLoS = false;
            missileChase.maxTimesSelected = -1;
            missileChase.maxDistance = 150f;
            missileChase.minDistance = 30f;
            missileChase.requireSkillReady = true;
            missileChase.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            missileChase.ignoreNodeGraph = false;
            missileChase.moveInputScale = 1f;
            missileChase.driverUpdateTimerOverride = -1f;
            missileChase.shouldSprint = false;
            missileChase.shouldFireEquipment = false;
            missileChase.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            missileChase.minTargetHealthFraction = Mathf.NegativeInfinity;
            missileChase.maxTargetHealthFraction = Mathf.Infinity;
            missileChase.minUserHealthFraction = float.NegativeInfinity;
            missileChase.maxUserHealthFraction = float.PositiveInfinity;
            missileChase.skillSlot = SkillSlot.Secondary;
            attackDrivers.Add(missileChase.customName);

            AISkillDriver missileStrafe = masterPrefab.AddComponent<AISkillDriver>();
            missileStrafe.customName = "AttackMissileStrafeM2";
            missileStrafe.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            missileStrafe.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            missileStrafe.activationRequiresAimConfirmation = true;
            missileStrafe.activationRequiresTargetLoS = true;
            missileStrafe.selectionRequiresTargetLoS = false;
            missileStrafe.maxTimesSelected = -1;
            missileStrafe.maxDistance = 30f;
            missileStrafe.minDistance = 0f;
            missileStrafe.requireSkillReady = true;
            missileStrafe.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            missileStrafe.ignoreNodeGraph = false;
            missileStrafe.moveInputScale = 1f;
            missileStrafe.driverUpdateTimerOverride = -1f;
            missileStrafe.shouldSprint = false;
            missileStrafe.shouldFireEquipment = false;
            missileStrafe.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            missileStrafe.minTargetHealthFraction = Mathf.NegativeInfinity;
            missileStrafe.maxTargetHealthFraction = Mathf.Infinity;
            missileStrafe.minUserHealthFraction = float.NegativeInfinity;
            missileStrafe.maxUserHealthFraction = float.PositiveInfinity;
            missileStrafe.skillSlot = SkillSlot.Secondary;
            attackDrivers.Add(missileStrafe.customName);

            AISkillDriver chaseTarget = masterPrefab.AddComponent<AISkillDriver>();
            chaseTarget.customName = "AttackChaseTargetM1";
            chaseTarget.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            chaseTarget.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            chaseTarget.activationRequiresAimConfirmation = true;
            chaseTarget.activationRequiresTargetLoS = true;
            chaseTarget.selectionRequiresTargetLoS = false;
            chaseTarget.maxTimesSelected = -1;
            chaseTarget.maxDistance = 150f;
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
            attackDrivers.Add(chaseTarget.customName);

            AISkillDriver attackTarget = masterPrefab.AddComponent<AISkillDriver>();
            attackTarget.customName = "AttackStrafeTargetM1";
            attackTarget.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            attackTarget.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            attackTarget.activationRequiresAimConfirmation = true;
            attackTarget.activationRequiresTargetLoS = true;
            attackTarget.selectionRequiresTargetLoS = false;
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
            attackDrivers.Add(attackTarget.customName);

            AISkillDriver idleStrafe = masterPrefab.AddComponent<AISkillDriver>();
            idleStrafe.customName = "IdleStrafeLeader";
            idleStrafe.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            idleStrafe.moveTargetType = AISkillDriver.TargetType.CurrentLeader;
            idleStrafe.activationRequiresAimConfirmation = false;
            idleStrafe.activationRequiresTargetLoS = false;
            idleStrafe.selectionRequiresTargetLoS = false;
            idleStrafe.maxTimesSelected = -1;
            idleStrafe.maxDistance = float.PositiveInfinity;
            idleStrafe.minDistance = float.NegativeInfinity;
            idleStrafe.requireSkillReady = false;
            idleStrafe.aimType = AISkillDriver.AimType.MoveDirection;
            idleStrafe.ignoreNodeGraph = false;
            idleStrafe.moveInputScale = 1f;
            idleStrafe.driverUpdateTimerOverride = -1f;
            idleStrafe.shouldSprint = false;
            idleStrafe.shouldTapButton = false;
            idleStrafe.shouldFireEquipment = false;
            idleStrafe.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            idleStrafe.minTargetHealthFraction = Mathf.NegativeInfinity;
            idleStrafe.maxTargetHealthFraction = Mathf.Infinity;
            idleStrafe.minUserHealthFraction = float.NegativeInfinity;
            idleStrafe.maxUserHealthFraction = float.PositiveInfinity;
            idleStrafe.skillSlot = SkillSlot.None;
            followDrivers.Add(idleStrafe.customName);

            AISkillDriver seekOut = masterPrefab.AddComponent<AISkillDriver>();
            seekOut.customName = "SeekOutEnemies";
            seekOut.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            seekOut.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            seekOut.activationRequiresAimConfirmation = false;
            seekOut.activationRequiresTargetLoS = false;
            seekOut.selectionRequiresTargetLoS = false;
            seekOut.maxTimesSelected = -1;
            seekOut.maxDistance = float.PositiveInfinity;
            seekOut.minDistance = 0f;
            seekOut.requireSkillReady = true;
            seekOut.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            seekOut.ignoreNodeGraph = false;
            seekOut.moveInputScale = 1f;
            seekOut.driverUpdateTimerOverride = -1f;
            seekOut.shouldSprint = false;
            seekOut.shouldFireEquipment = false;
            seekOut.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            seekOut.minTargetHealthFraction = Mathf.NegativeInfinity;
            seekOut.maxTargetHealthFraction = Mathf.Infinity;
            seekOut.minUserHealthFraction = float.NegativeInfinity;
            seekOut.maxUserHealthFraction = float.PositiveInfinity;
            seekOut.skillSlot = SkillSlot.None;
            attackDrivers.Add(seekOut.customName);

            /*
            AISkillDriver idleAttackMode = masterPrefab.AddComponent<AISkillDriver>();
            idleAttackMode.customName = "IdleInAttackMode";
            idleAttackMode.movementType = AISkillDriver.MovementType.Stop;
            idleAttackMode.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            idleAttackMode.activationRequiresAimConfirmation = false;
            idleAttackMode.activationRequiresTargetLoS = false;
            idleAttackMode.selectionRequiresTargetLoS = false;
            idleAttackMode.maxTimesSelected = -1;
            idleAttackMode.maxDistance = float.PositiveInfinity;
            idleAttackMode.minDistance = 0f;
            idleAttackMode.requireSkillReady = true;
            idleAttackMode.aimType = AISkillDriver.AimType.None;
            idleAttackMode.ignoreNodeGraph = false;
            idleAttackMode.moveInputScale = 1f;
            idleAttackMode.driverUpdateTimerOverride = -1f;
            idleAttackMode.shouldSprint = false;
            idleAttackMode.shouldFireEquipment = false;
            idleAttackMode.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            idleAttackMode.minTargetHealthFraction = Mathf.NegativeInfinity;
            idleAttackMode.maxTargetHealthFraction = Mathf.Infinity;
            idleAttackMode.minUserHealthFraction = float.NegativeInfinity;
            idleAttackMode.maxUserHealthFraction = float.PositiveInfinity;
            idleAttackMode.skillSlot = SkillSlot.None;
            attackDrivers.Add(idleAttackMode.customName);
            */

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
            Modules.Skills.AddUtilitySkills(bodyPrefab, primarySkillDef);
            Modules.Skills.AddSpecialSkills(bodyPrefab, primarySkillDef);
        }

        public override void InitializeDoppelganger(string clone)
        {
        }
    }
}
