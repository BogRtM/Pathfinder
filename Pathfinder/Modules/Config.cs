using BepInEx.Configuration;
using UnityEngine;

namespace Pathfinder.Modules
{
    public static class Config
    {
        private static string pathfinderPrefix = "Pathfinder - ";
        private static string squallPrefix = "Squall - ";

        #region Pathfinder Primary
        private static string primarySectionTitle = pathfinderPrefix + "Primary";
        public static ConfigEntry<float> ThrustDamage;
        #endregion

        #region Pathfinder Secondary
        private static string secondarySectionTitle = pathfinderPrefix + "Secondary";
        public static ConfigEntry<float> dashCD;
        public static ConfigEntry<float> JavelinDamage;
        #endregion

        #region Pathfinder Utility
        private static string utilitySectionTitle = pathfinderPrefix + "Utility";
        public static ConfigEntry<float> rendingTalonsCD;
        public static ConfigEntry<float> AirSpinDamage;
        public static ConfigEntry<float> GroundSpinDamage;

        public static ConfigEntry<float> bolasCD;
        //public static ConfigEntry<float> bolasExplosionDamage;
        public static ConfigEntry<float> electrocuteDPS;
        public static ConfigEntry<float> electrocuteSlowAmount;
        #endregion

        #region Squall General
        private static string squallBatteryTitle = squallPrefix + "Battery";
        public static ConfigEntry<float> batteryDrainRate;
        public static ConfigEntry<float> batteryRechargeRate;
        #endregion

        #region Squall Attack
        private static string squallAttackTitle = squallPrefix + "Attack Mode";
        public static ConfigEntry<float> SquallGunDamage;
        public static ConfigEntry<float> SquallGunProc;
        public static ConfigEntry<float> SquallMissileDamage;
        public static ConfigEntry<float> SquallMissileProc;
        #endregion

        #region Squall Special
        public static string squallSpecialTitle = squallPrefix + "Special";
        public static ConfigEntry<float> goForThroatCD;
        public static ConfigEntry<float> specialDamageCoefficient;
        public static ConfigEntry<float> specialRechargeAmount;
        public static ConfigEntry<float> specialArmorShred;
        #endregion

        public static void ReadConfig(PathfinderPlugin plugin)
        {
            #region Pathfinder Primary
            ThrustDamage = plugin.Config.Bind<float>(primarySectionTitle, "Thrust Damage Coefficient", 2.5f, "Damage coefficient of Thrust");
            #endregion

            #region Secondary
            dashCD = plugin.Config.Bind<float>(secondarySectionTitle, "Fleetfoot Cooldown", 6f, "Cooldown of Fleetfoot");

            JavelinDamage = plugin.Config.Bind<float>(secondarySectionTitle, "Javelin Damage Coefficient", 8f, "Damage coefficient of javelin toss");
            #endregion

            #region Utility
            bolasCD = plugin.Config.Bind<float>(utilitySectionTitle, "Shock Bolas Cooldown", 18f, "Cooldown of Shock Bolas");
            electrocuteDPS = plugin.Config.Bind<float>(utilitySectionTitle, "Electrocute Damage per Second", 1.2f, "Damage % per second of Electrocute DoT");
            electrocuteSlowAmount = plugin.Config.Bind<float>(utilitySectionTitle, "Electrocute Slow Multiplier", 0.5f, "Movespeed multiplier of Electrocute DoT");

            rendingTalonsCD = plugin.Config.Bind<float>(utilitySectionTitle, "Rending Talons Cooldown", 8f, "Cooldown of Rending Talons");
            AirSpinDamage = plugin.Config.Bind<float>(utilitySectionTitle, "Air Spin Damage Coefficient", 3f, "Damage coefficient of Rending Talons aerial spin attack");
            GroundSpinDamage = plugin.Config.Bind<float>(utilitySectionTitle, "Ground Spin Damage Coefficient", 8f, "Damage coefficient of Rending Talons ground spin attack");
            #endregion

            #region Squall General
            batteryDrainRate = plugin.Config.Bind<float>(squallBatteryTitle, "Battery Drain Rate", 8f, "Amount battery drains per second while Squall is in Attack Mode");
            batteryRechargeRate = plugin.Config.Bind<float>(squallBatteryTitle, "Battery Recharge Rate", 1f, "Base battery recharge rate while Squall is in Follow Mode");
            #endregion

            #region Squall Attack
            SquallGunDamage = plugin.Config.Bind<float>(squallAttackTitle, "Machine Guns Damage Coefficient", 0.3f, "Damage coefficient of Squall's machine guns. Each attack fires two bullets.");
            SquallGunProc = plugin.Config.Bind<float>(squallAttackTitle, "Machine Guns Proc Coefficient", 0.3f, "Proc coefficient of Squall's machine guns. Each attack fires two bullets.");
            SquallMissileDamage = plugin.Config.Bind<float>(squallAttackTitle, "Missile Launcher Damage Coefficient", 1.5f, "Damage coefficient of Squall's missile launcher.");
            #endregion

            #region Squall Special
            goForThroatCD = plugin.Config.Bind<float>(squallSpecialTitle, "Go for the Throat Cooldown", 12f, "Cooldown of Go for the Throat.");
            specialDamageCoefficient = plugin.Config.Bind<float>(squallSpecialTitle, "Go for the Throat Damage Coefficient", 0.7f, "Damage coefficient per strike of Go for the Throat.");
            specialRechargeAmount = plugin.Config.Bind<float>(squallSpecialTitle, "Go for the Throat Recharge Amount", 1f, "Percentage of battery recharged per strike of Go for the Throat.");
            specialArmorShred = plugin.Config.Bind<float>(squallSpecialTitle, "Go for the Throat Armor Shred Amount", 2f, "Flat amount of armor deducted per strike of Go for the Throat.");
            #endregion
        }

        // this helper automatically makes config entries for disabling survivors
        public static ConfigEntry<bool> CharacterEnableConfig(string characterName, string description = "Set to false to disable this character", bool enabledDefault = true)
        {
            return PathfinderPlugin.instance.Config.Bind<bool>("General",
                                                          "Enable " + characterName,
                                                          enabledDefault,
                                                          description);
        }
    }
}