using BepInEx.Configuration;
using UnityEngine;

namespace Pathfinder.Modules
{
    public static class Config
    {
        public static ConfigEntry<float> ThrustDamage;
        public static ConfigEntry<float> JavelinDamage;
        public static ConfigEntry<float> AirSpinDamage;
        public static ConfigEntry<float> GroundSpinDamage;

        public static ConfigEntry<float> SquallGunDamage;
        public static ConfigEntry<float> SquallGunProc;
        public static ConfigEntry<float> SquallMissileDamage;
        public static ConfigEntry<float> SquallMissileProc;
        public static void ReadConfig(PathfinderPlugin plugin)
        {
            ThrustDamage = plugin.Config.Bind<float>("Pathfinder", "Thrust Damage Coefficient", 2.8f, "Damage coefficient of Thrust");
            JavelinDamage = plugin.Config.Bind<float>("Pathfinder", "Javelin Damage Coefficient", 8f, "Damage coefficient of your javelin toss");
            AirSpinDamage = plugin.Config.Bind<float>("Pathfinder", "Air Spin Damage Coefficient", 2f, "Damage coefficient of your aerial spin attack");
            GroundSpinDamage = plugin.Config.Bind<float>("Pathfinder", "Ground Spin Damage Coefficient", 8f, "Damage coefficient of your ground spin attack");

            //SquallGunDamage = plugin.Config.Bind<float>("Squall", "Machine Guns Damage Coefficient", 0.3f, "Damage coefficient of Squall's machine guns. Each attack fires two bullets.");
            //SquallGunProc = plugin.Config.Bind<float>("Squall", "Machine Guns Proc Coefficient", 0.2f, "Proc coefficient of Squall's machine guns. Each attack fires two bullets.");
            //SquallMissileDamage = plugin.Config.Bind<float>("Squall", "Missile Launcher Damage Coefficient", 0.8f, "Damage coefficient of Squall's missile launcher. Each volley fires four missiles.");
        }

        // this helper automatically makes config entries for disabling survivors
        public static ConfigEntry<bool> CharacterEnableConfig(string characterName, string description = "Set to false to disable this character", bool enabledDefault = true) {
            return PathfinderPlugin.instance.Config.Bind<bool>("General",
                                                          "Enable " + characterName,
                                                          enabledDefault,
                                                          description);
        }
    }
}