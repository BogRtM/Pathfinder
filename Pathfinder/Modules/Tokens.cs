using R2API;
using System;
using Pathfinder.Components;
using Skillstates.Pathfinder;
using Skillstates.Squall;

namespace Pathfinder.Modules
{
    internal static class Tokens
    {
        internal static void AddTokens()
        {
            #region Pathfinder
            string prefix = PathfinderPlugin.DEVELOPER_PREFIX + "_PATHFINDER_BODY_";
            string squallPrefix = PathfinderPlugin.DEVELOPER_PREFIX + "_SQUALL_BODY_";

            string desc = "The Pathfinder is a crafty, nimble hunter who fights alongside his trusty falcon, Squall.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc += "< ! > Squall is a versatile companion who can deal high sustained damage, create distractions, and debuff your foes. Command him liberally." + Environment.NewLine + Environment.NewLine;
            desc += "< ! > Make sure to alternate between Hunter's Pursuit and your javelin toss to maximize your damage." + Environment.NewLine + Environment.NewLine;
            desc += "< ! > Rending Talons can deal extremely high sustained damage when coupled with items that increase your air time, like Hopoo Feather or Milky Chrysalis." + Environment.NewLine + Environment.NewLine;
            desc += "< ! > You are quite fragile compared to other melee survivors; stay evasive, and always be prepared to disengage." + Environment.NewLine + Environment.NewLine + Environment.NewLine;
            
            desc += "<style=cShrine>Modder's Note:</style> <style=cUserSetting>Thank you so much for showing interest in The Pathfinder! " +
                "This survivor is still in active development, thus many things are liable to change, and your feedback is highly requested;" +
                "Please feel free to DM <style=cIsUtility>Bog#4770</style> on Discord, or find me on the official Risk of Rain 2 Modding server.</style>";


            string outro = "..and so they left, searching for a new identity.";
            string outroFailure = "..and so they vanished, forever lost to the uncaring wilderness.";

            #region Squall
            LanguageAPI.Add(squallPrefix + "NAME", "Squall");
            #endregion

            #region Pathfinder
            LanguageAPI.Add(prefix + "NAME", "Pathfinder");
            LanguageAPI.Add(prefix + "DESCRIPTION", desc);
            LanguageAPI.Add(prefix + "SUBTITLE", "Birds of Prey");
            LanguageAPI.Add(prefix + "LORE", "");
            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", outro);
            LanguageAPI.Add(prefix + "OUTRO_FAILURE", outroFailure);
            #endregion

            #region Skins
            LanguageAPI.Add(prefix + "DEFAULT_SKIN_NAME", "Default");
            LanguageAPI.Add(prefix + "MASTERY_SKIN_NAME", "Taiga");
            #endregion

            #region Passive
            LanguageAPI.Add(prefix + "PASSIVE_NAME", "Falconer");
            LanguageAPI.Add(prefix + "PASSIVE_DESCRIPTION", "You are accompanied by your robot falcon, Squall. Squall is immune to damage, but runs on a <style=cIsUtility>battery</style>.");
            #endregion

            #region Keywords
            LanguageAPI.Add("KEYWORD_BATTERY", "<style=cKeywordName>Battery></style><style=cSub>In <style=cIsHealing>Follow Mode</style>, Squall regenerates 2% battery per second. " +
                "In <style=cDeath>Attack Mode</style>, Squall drains 10% battery per second. If the battery reaches 0%, Squall is forced into <style=cIsHealing>Follow Mode</style>.");

            LanguageAPI.Add("KEYWORD_ELECTROCUTE", $"<style=cKeywordName>Electrocute</style><style=cSub>Targets have their movespeed reduced by {100f * Config.electrocuteSlowAmount.Value}%, " +
                $"armor reduced by {Config.electrocuteArmorShred.Value}, and take <style=cIsDamage>{100f * Config.electrocuteDPS.Value}% damage</style> per second.</style>");
            
            LanguageAPI.Add("KEYWORD_ATTACK", "<style=cDeath[ Attack ]</style>" + Environment.NewLine +
                "<style=cSub>Redirect Squall's attention to target enemy, and activate <style=cDeath>Attack Mode</style>, " + 
                $" granting access to machine guns that deal <style=cIsDamage>2x{100f * Config.SquallGunDamage.Value}% damage</style>, " +
                $"and a missile launcher that deals <style=cIsDamage>{MissileLauncher.maxMissileCount}x{100f * Config.SquallMissileDamage.Value}% damage.</style>");

            LanguageAPI.Add("KEYWORD_FOLLOW", "<style=cIsHealing>[ Follow ]</style>" + Environment.NewLine +
                $"<style=cSub>Return Squall to yourself, and set him to <style=cIsHealing>Follow Mode</style>.");

            LanguageAPI.Add("KEYWORD_SQUALL_UTILITY", "<style>c=IsUtility>[ Utility ]</style>" + Environment.NewLine +
                "<style=cSub>Order Squall to use his <style=cIsUtility>Utility</style> skill. Check the Loadout panel for details.</style>");

            LanguageAPI.Add("KEYWORD_SQUALL_SPECIAL", "<style>c=Shrine>[ Special ]</style>" + Environment.NewLine +
                "<style=cSub>Order Squall to use his <style=cShrine>Special</style> skill. Check the Loadout panel for details.</style>");
            #endregion

            #region Primary
            LanguageAPI.Add(prefix + "PRIMARY_THRUST_NAME", "Thrust");
            LanguageAPI.Add(prefix + "PRIMARY_THRUST_DESCRIPTION", $"Thrust your spear forward for <style=cIsDamage>{100f * Config.ThrustDamage.Value}% damage</style>.");
            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "SECONDARY_PURSUIT_NAME", "Hunter's Pursuit");
            LanguageAPI.Add(prefix + "SECONDARY_PURSUIT_DESCRIPTION", $"<style=cIsUtility>Dash</style> a short distance. The next time you use your primary skill, throw an " +
                $"<style=cIsUtility>exploding</style> javelin for <style=cIsDamage>{100f * Config.JavelinDamage.Value}% damage</style>.");
            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_SPIN_NAME", "Rending Talons");
            LanguageAPI.Add(prefix + "UTILITY_SPIN_DESCRIPTION", $"Rise into the air, spinning rapidly for <style=cIsDamage>{100f * Config.AirSpinDamage.Value}% damage</style>. " +
                $"Upon landing, perform a horizontal sweep for <style=cIsDamage>{100f * Config.GroundSpinDamage.Value}% damage</style>.");

            LanguageAPI.Add(prefix + "UTILITY_BOLAS_NAME", "Shock Bolas");
            LanguageAPI.Add(prefix + "UTILITY_BOLAS_DESCRIPTION", "Throw electrically charged bolas, which <style=cIsUtility>shock</style> nearby enemies, and leave behind an <style=cIsUtility>electrocuting</style> field for <style=cIsUtility>10 seconds</style>.");
            #endregion

            #region Special
            LanguageAPI.Add(prefix + "SPECIAL_COMMAND_NAME", "Issue Command");
            LanguageAPI.Add(prefix + "SPECIAL_COMMAND_DESCRIPTION", "Prepare a command for Squall. You can issue an <style=cDeath>Attack</style>, <style=cIsHealing>Follow</style>, " +
                "<style=cIsUtility>Utility</style>, or <style=cShrine>Special</style> command." +
                "<style");
            #endregion

            #region SquallSpecial
            LanguageAPI.Add(prefix + "SQUALL_SPECIAL_GOFORTHROAT_NAME", "Go for the Throat!");
            LanguageAPI.Add(prefix + "SQUALL_SPECIAL_GOFORTHROAT_DESCRIPTION", "Order Squall to strike target enemy X times");
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