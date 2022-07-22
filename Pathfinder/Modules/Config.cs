using BepInEx.Configuration;
using UnityEngine;

namespace Pathfinder.Modules
{
    public static class Config
    {
        private static string pathfinderPrefix = "Pathfinder - ";
        private static string squallPrefix = "Squall - ";

        #region Debuffs
        private static string debuffSectionTitle = "Debuffs";
        public static ConfigEntry<float> electrocuteDPS;
        public static ConfigEntry<float> electrocuteSlowAmount;
        public static ConfigEntry<float> electrocuteArmorShred;

        public static ConfigEntry<float> raptorMarkDuration;
        public static ConfigEntry<float> raptorMarkDamageMult;
        #endregion

        #region Pathfinder Primary
        private static string primarySectionTitle = pathfinderPrefix + "Primary";
        public static ConfigEntry<float> ThrustDamage;
        #endregion

        #region Pathfinder Secondary
        private static string secondarySectionTitle= pathfinderPrefix + "Secondary";
        public static ConfigEntry<float> JavelinDamage;
        public static ConfigEntry<float> JavelinExplosionRadius;
        #endregion

        #region Utility
        public static ConfigEntry<float> AirSpinDamage;
        public static ConfigEntry<float> GroundSpinDamage;
        #endregion

        public static ConfigEntry<float> SquallGunDamage;
        public static ConfigEntry<float> SquallGunProc;
        public static ConfigEntry<float> SquallMissileDamage;
        public static ConfigEntry<float> SquallMissileProc;
        public static void ReadConfig(PathfinderPlugin plugin)
        {
            #region Debuffs
            electrocuteDPS = plugin.Config.Bind<float>(debuffSectionTitle, "Electrocute Damage per Second", 1.2f, "Damage % per second of Electrocute DoT");
            electrocuteSlowAmount = plugin.Config.Bind<float>(debuffSectionTitle, "Electrocute Slow Multiplier", 0.5f, "Movespeed multiplier of Electrocute DoT");
            electrocuteArmorShred = plugin.Config.Bind<float>(debuffSectionTitle, "Electrocute Armor Shred", 20f, "Flat amount of armor deducted by Electrocute DoT");
            raptorMarkDuration = plugin.Config.Bind<float>(debuffSectionTitle, "Raptor Mark Duration", 5f, "Duration of Raptor's Mark");
            raptorMarkDamageMult = plugin.Config.Bind<float>(debuffSectionTitle, "Raptor Mark Damage Multipliler", 1.2f, "Duration of Raptor's Mark");
            #endregion

            #region Pathfinder Primary
            ThrustDamage = plugin.Config.Bind<float>(primarySectionTitle, "Thrust Damage Coefficient", 2.8f, "Damage coefficient of Thrust");
            #endregion

            #region Secondary
            JavelinDamage = plugin.Config.Bind<float>(secondarySectionTitle, "Javelin Damage Coefficient", 8f, "Damage coefficient of javelin toss");
            JavelinExplosionRadius = plugin.Config.Bind<float>(secondarySectionTitle, "Javelin Explosion Radius", 8f, "Explosion radius of javelin toss");
            #endregion

            #region Utility
            AirSpinDamage = plugin.Config.Bind<float>("Pathfinder", "Air Spin Damage Coefficient", 3f, "Damage coefficient of Rending Talons aerial spin attack");
            GroundSpinDamage = plugin.Config.Bind<float>("Pathfinder", "Ground Spin Damage Coefficient", 8f, "Damage coefficient of Rending Talons ground spin attack");
            #endregion
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