using R2API;
using System;

namespace Pathfinder.Modules
{
    internal static class Tokens
    {
        internal static void AddTokens()
        {
            #region Pathfinder
            string prefix = PathfinderPlugin.DEVELOPER_PREFIX + "_PATHFINDER_BODY_";
            string squallPrefix = PathfinderPlugin.DEVELOPER_PREFIX + "_SQUALL_BODY_";

            string desc = "The Pathfinder is a crafty, evasive hunter who fights alongside his trusty falcon, Squall.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc += "< ! > Squall is a versatile companion who can deal high sustained damage, create distractions, and debuff your foes. Command him liberally." + Environment.NewLine + Environment.NewLine;
            desc += "< ! > Make sure to alternate between Hunter's Pursuit and your javelin toss to maximize your damage." + Environment.NewLine + Environment.NewLine;
            desc += "< ! > Rending Talons can deal extremely high sustained damage when coupled with items that increase your air time, like Hopoo Feather or Milky Chrysalis." + Environment.NewLine + Environment.NewLine;
            desc += "< ! > You are quite fragile compared to other melee survivors; stay evasive, and always be prepared to disengage." + Environment.NewLine + Environment.NewLine + Environment.NewLine;
            desc += "<style=cShrine>Modder's Note:</style> <style=cUserSetting>Thank you so much for showing interest in The Pathfinder! " +
                "This survivor is still in active development, thus your feedback is highly requested. " +
                "Please feel free to contact Bog#4770 on Discord with feedback, suggestions, or inquiries. You can also find me on the official Risk of Rain 2 Modding server. " +
                "Additionally, a config file has been provided for you to play around with his damage values.</style>";


            string outro = "..and so they left, searching for a new identity.";
            string outroFailure = "..and so he vanished, forever a blank slate.";

            LanguageAPI.Add(prefix + "NAME", "Pathfinder");
            LanguageAPI.Add(prefix + "DESCRIPTION", desc);
            LanguageAPI.Add(prefix + "SUBTITLE", "Birds of Prey");
            LanguageAPI.Add(prefix + "LORE", "sample lore");
            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", outro);
            LanguageAPI.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            LanguageAPI.Add(prefix + "DEFAULT_SKIN_NAME", "Default");
            LanguageAPI.Add(prefix + "MASTERY_SKIN_NAME", "Taiga");
            #endregion

            #region Passive
            LanguageAPI.Add(prefix + "PASSIVE_NAME", "Robot Falcon: Squall");
            LanguageAPI.Add(prefix + "PASSIVE_DESCRIPTION", "You are accompanied by your falcon: Squall. Squall inherits your items, " +
                "and comes equipped with <style=cIsDamage>machine guns</style> and a <style=cIsDamage>missile launcher</style>.");
            #endregion

            #region Keywords
            LanguageAPI.Add("KEYWORD_EMPOWER", "<style=cKeywordName>Empower</style><style=cSub>Upgrades the next usage of your primary skill.</style>");
            //LanguageAPI.Add("KEYWORD_MACHINEGUN", $"<style=cKeywordName>Machine Gun</style><style=cSub>Shoot for 2x<style=cIsDamage>{Config.SquallGunDamage.Value}% damage</style>. </style>");
            //LanguageAPI.Add("KEYWORD_MISSILELAUNCHER", $"<style=cKeywordName>Missile Launcher</style><style=cSub>Fire a volley of missilles for 4xx<style=cIsDamage>{Config.SquallMissileDamage.Value}% damage</style>. </style>");
            LanguageAPI.Add("KEYWORD_ATTACK", "<style=cIsDamage>[ Attack ]</style>" + Environment.NewLine +
                "<style=cSub>Redirect Squall's attention to target enemy, and set him to Attack Mode. " +
                "In Attack Mode, Squall will attack more aggressively, and seek out enemies by himself.</style>");
            LanguageAPI.Add("KEYWORD_FOLLOW", "<style=cIsUtility>[ Follow ]</style>" + Environment.NewLine +
                "<style=cSub>Teleport Squall to yourself, and set him to Follow Mode. In Follow Mode, Squall will prioritize staying close to you.");
            #endregion

            #region Primary
            LanguageAPI.Add(prefix + "PRIMARY_THRUST_NAME", "Thrust");
            LanguageAPI.Add(prefix + "PRIMARY_THRUST_DESCRIPTION", $"Thrust your spear forward for <style=cIsDamage>{100f * Config.ThrustDamage.Value}% damage</style>.");
            #endregion

            #region Empower
            LanguageAPI.Add(prefix + "SECONDARY_JAVELIN_NAME", "Javelin Toss");
            LanguageAPI.Add(prefix + "EMPOWER_JAVELIN_DESCRIPTION", "Throw an <style=cIsUtility>exploding</style> javelin for <style=cIsDamage>{}% damage</style>.");

            LanguageAPI.Add(prefix + "EMPOWER_COMBO_NAME", "Rending Talons");
            LanguageAPI.Add(prefix + "EMPOWER_COMBO_DESCRIPTION", "Perform a 3-hit combo for <style=cIsDamage>2x200% + 1x300% damage</style>.");

            LanguageAPI.Add(prefix + "EMPOWER_LUNGE_NAME", "Impale");
            LanguageAPI.Add(prefix + "EMPOWER_LUNGE_DESCRIPTION", "Swiftly lunge forward for <style=cIsDamage>500% damage</style>.");
            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "SECONDARY_PURSUIT_NAME", "Hunter's Pursuit");
            LanguageAPI.Add(prefix + "SECONDARY_PURSUIT_DESCRIPTION", $"<style=cIsUtility>Dash</style> a short distance. The next primary skill used within three seconds will throw an " +
                $"<style=cIsUtility>exploding</style> javelin for <style=cIsDamage>{100f * Config.JavelinDamage.Value}% damage</style>.");
            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_SPIN_NAME", "Rending Talons");
            LanguageAPI.Add(prefix + "UTILITY_SPIN_DESCRIPTION", $"Rise into the air, spinning rapidly for <style=cIsDamage>{100f * Config.AirSpinDamage.Value}% damage</style>. " +
                $"Upon landing, perform a horizontal sweep for <style=cIsDamage>{100f * Config.GroundSpinDamage.Value}% damage</style>.");
            #endregion

            #region Special
            LanguageAPI.Add(prefix + "SPECIAL_COMMAND_NAME", "Issue Command");
            LanguageAPI.Add(prefix + "SPECIAL_COMMAND_DESCRIPTION", "Prepare an order for Squall. You can issue an " +
                "<style=cIsDamage>Attack</style>, <style=cIsUtility>Follow</style>, or <style=cShrine>Special</style> command.");
            #endregion

            #region Achievements
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_NAME", "Pathfinder: Mastery");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_DESC", "As Pathfinder, beat the game or obliterate on Monsoon.");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_UNLOCKABLE_NAME", "Pathfinder: Mastery");
            #endregion
            #endregion

            /*
            #region Henry
            string prefix = PathfinderPlugin.DEVELOPER_PREFIX + "_HENRY_BODY_";

            string desc = "Henry is a skilled fighter who makes use of a wide arsenal of weaponry to take down his foes.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Sword is a good all-rounder while Boxing Gloves are better for laying a beatdown on more powerful foes." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Pistol is a powerful anti air, with its low cooldown and high damage." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Roll has a lingering armor buff that helps to use it aggressively." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Bomb can be used to wipe crowds with ease." + Environment.NewLine + Environment.NewLine;

            string outro = "..and so he left, searching for a new identity.";
            string outroFailure = "..and so he vanished, forever a blank slate.";

            LanguageAPI.Add(prefix + "NAME", "Henry");
            LanguageAPI.Add(prefix + "DESCRIPTION", desc);
            LanguageAPI.Add(prefix + "SUBTITLE", "The Chosen One");
            LanguageAPI.Add(prefix + "LORE", "sample lore");
            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", outro);
            LanguageAPI.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            LanguageAPI.Add(prefix + "DEFAULT_SKIN_NAME", "Default");
            LanguageAPI.Add(prefix + "MASTERY_SKIN_NAME", "Alternate");
            #endregion

            #region Passive
            LanguageAPI.Add(prefix + "PASSIVE_NAME", "Henry passive");
            LanguageAPI.Add(prefix + "PASSIVE_DESCRIPTION", "Sample text.");
            #endregion

            #region Primary
            LanguageAPI.Add(prefix + "PRIMARY_SLASH_NAME", "Sword");
            LanguageAPI.Add(prefix + "PRIMARY_SLASH_DESCRIPTION", Helpers.agilePrefix + $"Swing forward for <style=cIsDamage>{100f * StaticValues.swordDamageCoefficient}% damage</style>.");
            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "SECONDARY_GUN_NAME", "Handgun");
            LanguageAPI.Add(prefix + "SECONDARY_GUN_DESCRIPTION", Helpers.agilePrefix + $"Fire a handgun for <style=cIsDamage>{100f * StaticValues.gunDamageCoefficient}% damage</style>.");
            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_ROLL_NAME", "Roll");
            LanguageAPI.Add(prefix + "UTILITY_ROLL_DESCRIPTION", "Roll a short distance, gaining <style=cIsUtility>300 armor</style>. <style=cIsUtility>You cannot be hit during the roll.</style>");
            #endregion

            #region Special
            LanguageAPI.Add(prefix + "SPECIAL_BOMB_NAME", "Bomb");
            LanguageAPI.Add(prefix + "SPECIAL_BOMB_DESCRIPTION", $"Throw a bomb for <style=cIsDamage>{100f * StaticValues.bombDamageCoefficient}% damage</style>.");
            #endregion

            #region Achievements
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_NAME", "Henry: Mastery");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_DESC", "As Henry, beat the game or obliterate on Monsoon.");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_UNLOCKABLE_NAME", "Henry: Mastery");
            #endregion
            #endregion
            */
        }
    }
}