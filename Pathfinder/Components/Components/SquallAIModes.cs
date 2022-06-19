using UnityEngine;
using System.Collections.Generic;
using RoR2.CharacterAI;

namespace Pathfinder.Components
{
    internal class SquallAIModes : MonoBehaviour
    {
        public List<AISkillDriver> followDrivers = new List<AISkillDriver>();
        public List<AISkillDriver> attackDrivers = new List<AISkillDriver>();
    }
}
