using BepInEx.Configuration;
using Pathfinder.Modules.Characters;
using Pathfinder.Misc;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;
using Pathfinder.Modules;

namespace Pathfinder.Content
{
    internal class Squall : CharacterBase
    {
        public override BodyInfo bodyInfo { get; set; } = new BodyInfo()
        {
            bodyNameToClone = "FlyingVermin",
            bodyName = "SquallBody",

            moveSpeed = 70f,
            acceleration = 120f
        };

        public override CustomRendererInfo[] customRendererInfos { get; set; } = new CustomRendererInfo[]
        {
            new CustomRendererInfo
                {
                    childName = "Squall"
                }
        };

        public override Type characterMainState => typeof(EntityStates.GenericCharacterMain);
        //public override Type characterSpawnState => typeof(EntityStates.GenericCharacterMain);

        public override string bodyName => "Squall";
        public override void InitializeCharacter()
        {
            base.InitializeCharacter();
            bodyPrefab.GetComponent<HealthComponent>().godMode = true;
            bodyPrefab.GetComponent<CharacterMotor>().airControl = 1f;
            bodyPrefab.AddComponent<SquallController>();

            PathfinderPlugin.squallPrefab = this.bodyPrefab;
        }

        public override void InitializeSkills()
        {
            
        }

        public override void InitializeDoppelganger(string clone)
        {
        }
    }
}
