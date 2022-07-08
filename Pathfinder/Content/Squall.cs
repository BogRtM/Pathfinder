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
            //bodyPrefab.AddComponent<SquallPointer>();

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
            hardLeash.driverUpdateTimerOverride = -1f;
            hardLeash.shouldSprint = false;
            hardLeash.shouldFireEquipment = false;
            hardLeash.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            hardLeash.minTargetHealthFraction = Mathf.NegativeInfinity;
            hardLeash.maxTargetHealthFraction = Mathf.Infinity;
            hardLeash.minUserHealthFraction = float.NegativeInfinity;
            hardLeash.maxUserHealthFraction = float.PositiveInfinity;
            hardLeash.skillSlot = SkillSlot.None;
            followDrivers.Add(hardLeash.customName);

            AISkillDriver diveAttack = masterPrefab.AddComponent<AISkillDriver>();
            diveAttack.customName = "DiveBestTarget";
            diveAttack.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            diveAttack.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            diveAttack.activationRequiresAimConfirmation = false;
            diveAttack.activationRequiresTargetLoS = false;
            diveAttack.selectionRequiresTargetLoS = false;
            diveAttack.maxTimesSelected = -1;
            diveAttack.maxDistance = 60f;
            diveAttack.minDistance = 0f;
            diveAttack.requireSkillReady = true;
            diveAttack.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            diveAttack.ignoreNodeGraph = true;
            diveAttack.moveInputScale = 1f;
            diveAttack.driverUpdateTimerOverride = 3f;
            diveAttack.shouldSprint = false;
            diveAttack.shouldFireEquipment = false;
            diveAttack.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            diveAttack.minTargetHealthFraction = Mathf.NegativeInfinity;
            diveAttack.maxTargetHealthFraction = Mathf.Infinity;
            diveAttack.minUserHealthFraction = float.NegativeInfinity;
            diveAttack.maxUserHealthFraction = float.PositiveInfinity;
            diveAttack.skillSlot = SkillSlot.Primary;


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
            softLeash.driverUpdateTimerOverride = -1f;
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

            AISkillDriver idleStrafe = masterPrefab.AddComponent<AISkillDriver>();
            idleStrafe.customName = "IdleStrafeLeader";
            idleStrafe.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            idleStrafe.moveTargetType = AISkillDriver.TargetType.CurrentLeader;
            idleStrafe.activationRequiresAimConfirmation = false;
            idleStrafe.activationRequiresTargetLoS = false;
            idleStrafe.selectionRequiresTargetLoS = false;
            idleStrafe.maxTimesSelected = -1;
            idleStrafe.maxDistance = float.PositiveInfinity;
            idleStrafe.minDistance = 0f;
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
