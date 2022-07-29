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
            string devPrefix = PathfinderPlugin.DEVELOPER_PREFIX;
            string prefix = devPrefix + "_PATHFINDER_BODY_";
            string squallPrefix = devPrefix + "_SQUALL_BODY_";

            string modderNote = "<style=cShrine>Modder's Note:</style> <style=cUserSetting>Thank you so much for showing interest in <color=#3ea252>The Pathfinder</color>! " +
                "This survivor is still in active development, thus many things are liable to change, and your feedback is highly requested. " +
                "Please feel free to DM <style=cIsUtility>Bog#4770</style> on Discord, or find me on the official Risk of Rain 2 Modding server.</style>";

            string desc = "The Pathfinder is a crafty, evasive skirmisher who fights alongside his trusty falcon, Squall.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc += "< ! > You are quite fragile compared to other melee survivors; use your quick feet, long reach, and Squall's distractions to stay alive." + Environment.NewLine + Environment.NewLine;
            desc += "< ! > Alternate between Fleetfoot and your javelin toss to get as many javelins as possible, but make sure to reserve a skill charge to dodge when necessary." + Environment.NewLine + Environment.NewLine;
            desc += "< ! > Shock Bolas are a great way to lock down large groups of enemies at once, while Rending Talons can deal brutal damage when combined with items that increase your air time." + Environment.NewLine + Environment.NewLine;
            desc += "< ! > Squall is a highly capable fighter; command him liberally and prioritize attack speed and critical chance items to help speed up the battery meter." + Environment.NewLine + Environment.NewLine + Environment.NewLine;

            desc += modderNote;

            string lore = "...";

            string outro = "..and so they left, the new conquerers of yet another food chain.";
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

            #region Skins
            LanguageAPI.Add(prefix + "DEFAULT_SKIN_NAME", "Default");
            LanguageAPI.Add(prefix + "MASTERY_SKIN_NAME", "Taiga");
            #endregion

            #region Passive
            LanguageAPI.Add(prefix + "PASSIVE_NAME", "Falconer");
            LanguageAPI.Add(prefix + "PASSIVE_DESCRIPTION", "You are accompanied by your robot falcon, Squall. Squall inherits <style=cIsDamage>most</style> of " +
                "your items and is immune to damage, but runs on a <style=cIsUtility>battery</style>.");
            #endregion

            #region Keywords
            LanguageAPI.Add("KEYWORD_BATTERY", "<style=cKeywordName>Battery</style><style=cSub>" +
                "Squall has two modes: <color=#FF0000>Attack</color>, and <color=#00FF00>Follow</color>. " +
                $"In <color=#FF0000>Attack Mode</color>, Squall <style=cIsDamage>drains {Config.batteryDrainRate.Value}%</style> battery per second. " +
                $"In <color=#00FF00>Follow Mode</color>, Squall <style=cIsHealing>regenerates {Config.batteryRechargeRate.Value}%</style> battery per second, " +
                $"scaling with <style=cIsUtility>his attack speed</style>. " +
                $"If the meter hits 0, Squall is forced into <color=#00FF00>Follow Mode</color>.</style>");

            LanguageAPI.Add("KEYWORD_ELECTROCUTE", $"<style=cKeywordName>Electrocute</style><style=cSub>Targets have their movespeed reduced by {100f * Config.electrocuteSlowAmount.Value}%, " +
                $"and take <style=cIsDamage>{100f * Config.electrocuteDPS.Value}% damage</style> per second.</style>");

            LanguageAPI.Add("KEYWORD_ATTACK", "<style=cKeywordName><color=#FF0000>Attack</color></style>" +
                "<style=cSub>Direct Squall's attention to the targeted enemy, and activate <color=#FF0000>Attack Mode</color>, " +
                $"granting access to machine guns that deal <style=cIsDamage>2x{100f * Config.SquallGunDamage.Value}% damage</style>, " +
                $"and a missile launcher that deals <style=cIsDamage>{MissileLauncher.maxMissileCount}x{100f * Config.SquallMissileDamage.Value}% damage</style>.</style>");

            LanguageAPI.Add("KEYWORD_FOLLOW", "<style=cKeywordName><color=#00FF00>Follow</color></style>" +
                $"<style=cSub>Return Squall to yourself, and activate <color=#00FF00>Follow Mode</color>, causing him to stay close to you.</style>");

            LanguageAPI.Add("KEYWORD_SQUALL_UTILITY", "<style=cKeywordName><color=#87b9cf>Utility</color></style>" +
                "<style=cSub>Order Squall to use his <style=cIsUtility>Utility</style> skill.</style>");

            LanguageAPI.Add("KEYWORD_SQUALL_SPECIAL", "<style=cKeywordName><color=#efeb1c>Special - Go for the Throat!</color></style>" +
                $"<style=cSub>Order Squall to repeatedly strike the targeted enemy for <style=cIsDamage>{100f * Config.specialDamageCoefficient.Value}% damage</style>. " +
                $"Each strike briefly shreds <style=cIsDamage>armor</style> by <style=cIsDamage>{Config.specialArmorShred.Value}</style>, " +
                $"and <style=cIsUtility>regenerates {Config.specialRechargeAmount.Value}%</style> battery, doubled on Critical Strikes. " +
                $"This skill can overcharge the battery up to <style=cIsUtility>120%</style>.</style>");

            LanguageAPI.Add("KEYWORD_UNPOLISHED", "<style=cKeywordName>Unpolished</style> " +
                "<style=cSub>This skill is missing VFX and SFX, and is also probably buggier than the others.");
            #endregion

            #region Primary
            LanguageAPI.Add(prefix + "PRIMARY_THRUST_NAME", "Thrust");
            LanguageAPI.Add(prefix + "PRIMARY_THRUST_DESCRIPTION", $"Thrust your spear forward for <style=cIsDamage>{100f * Config.ThrustDamage.Value}% damage</style>.");
            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "SECONDARY_DASH_NAME", "Fleetfoot");
            LanguageAPI.Add(prefix + "SECONDARY_DASH_DESCRIPTION", $"<style=cIsUtility>Dash</style> a short distance. The next time you use your primary skill, throw an " +
                $"<style=cIsDamage>explosive</style> javelin for <style=cIsDamage>{100f * Config.JavelinDamage.Value}% damage</style>.");

            LanguageAPI.Add(prefix + "SECONDARY_JAVELIN_NAME", "Explosive Javelin");
            LanguageAPI.Add(prefix + "SECONDARY_JAVELIN_DESCRIPTION", "Throw an <style=cIsDamage>explosive</style> javelin for " +
                $"<style=cIsDamage>{100f * Config.JavelinDamage.Value}% damage</style>.");
            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_SPIN_NAME", "Rending Talons");
            LanguageAPI.Add(prefix + "UTILITY_SPIN_DESCRIPTION", $"<style=cWorldEvent>UNPOLISHED</style>. Rise into the air, spinning rapidly for <style=cIsDamage>{100f * Config.AirSpinDamage.Value}% damage</style>. " +
                $"Upon landing, perform a horizontal sweep for <style=cIsDamage>{100f * Config.GroundSpinDamage.Value}% damage</style>.");

            LanguageAPI.Add(prefix + "UTILITY_BOLAS_NAME", "Shock Bolas");
            LanguageAPI.Add(prefix + "UTILITY_BOLAS_DESCRIPTION", $"Throw electrically charged bolas, which <style=cIsUtility>shock</style> " +
                $"nearby enemies, and leave behind an <style=cIsUtility>electrocuting</style> field for <style=cIsUtility>10 seconds</style>.");
            #endregion

            #region Special
            LanguageAPI.Add(prefix + "SPECIAL_COMMAND_NAME", "Issue Command");
            LanguageAPI.Add(prefix + "SPECIAL_COMMAND2_NAME", "Issue Command - Utility Override");
            LanguageAPI.Add(prefix + "SPECIAL_COMMAND_DESCRIPTION", "Prepare a command for Squall. You can issue an <color=#FF0000>Attack</color>, <color=#00FF00>Follow</color>, " +
                "or <color=#efeb1c>Special</color> command.");
            LanguageAPI.Add(prefix + "SPECIAL_COMMAND2_DESCRIPTION", "This is the same skill as <color=#3ea252>Issue Command</color>. However, Squall's <color=#efeb1c>Special</color> " +
                "command now overrides your <style=cIsUtility>Utility</style> skill instead.");

            LanguageAPI.Add(prefix + "SPECIAL_ATTACK_NAME", "Attack Command");
            LanguageAPI.Add(prefix + "SPECIAL_ATTACK_DESCRIPTION", "Direct Squall's attention to the targeted enemy, and activate <color=#FF0000>Attack Mode</color>, " +
                $"granting access to machine guns that deal <style=cIsDamage>2x{100f * Config.SquallGunDamage.Value}% damage</style>, " +
                $"and a missile launcher that deals <style=cIsDamage>{MissileLauncher.maxMissileCount}x{100f * Config.SquallMissileDamage.Value}% damage</style>.");

            LanguageAPI.Add(prefix + "SPECIAL_FOLLOW_NAME", "Follow Command");
            LanguageAPI.Add(prefix + "SPECIAL_FOLLOW_DESCRIPTION", $"Return Squall to yourself, and activate <color=#00FF00>Follow Mode</color>, " +
                $"causing him to stay close to you.");

            LanguageAPI.Add(prefix + "SPECIAL_CANCEL_NAME", "Cancel Command Mode");
            LanguageAPI.Add(prefix + "SPECIAL_CANCEL_DESCRIPTION", "Cancel Command Mode.");

            LanguageAPI.Add(devPrefix + "_SQUALL_SPECIAL_GOFORTHROAT_NAME", "Go for the Throat!");
            LanguageAPI.Add(devPrefix + "_SQUALL_SPECIAL_GOFORTHROAT_DESCRIPTION", "Order Squall to repeatedly strike the targeted enemy for " +
                $"<style=cIsDamage>{100f * Config.specialDamageCoefficient.Value}% damage</style>. " +
                $"Each strike briefly shreds <style=cIsDamage>armor</style> by <style=cIsDamage>{Config.specialArmorShred.Value}</style>, " +
                $"and <style=cIsUtility>regenerates {Config.specialRechargeAmount.Value}%</style> battery, doubled on Critical Strikes. " +
                $"This skill can overcharge the battery up to <style=cIsUtility>120%</style>.");
            #endregion
            #endregion

            #region Squall
            LanguageAPI.Add(devPrefix + "_SQUALL_PRIMARY_GUNS_NAME", "Mounted Guns");
            LanguageAPI.Add(devPrefix + "_SQUALL_PRIMARY_GUNS_DESCRIPTION",
                $"Fire your turrets for <style=cIsDamage>2x{100f * Config.SquallGunDamage.Value}% damage</style>.");

            LanguageAPI.Add(devPrefix + "_SQUALL_SECONDARY_MISSILE_NAME", "Missile Launcher");
            LanguageAPI.Add(devPrefix + "_SQUALL_SECONDARY_MISSILE_DESCRIPTION", "Fire a volley of 4 missiles for <style=cIsDamage>150% damage</style> each.");

            LanguageAPI.Add(devPrefix + "_SQUALL_UTILITY_DONOTHING_NAME", "Do Nothing");
            LanguageAPI.Add(devPrefix + "_SQUALL_UTILITY_DONOTHING_DESCRIPTION", "This skill does nothing.");

            LanguageAPI.Add(devPrefix + "_SQUALL_SPECIAL_GOFORTHROAT_NAME", "Go for the Throat!");
            LanguageAPI.Add(devPrefix + "_SQUALL_SPECIAL_GOFORTHROAT_DESCRIPTION", "Order Squall to repeatedly strike the targeted enemy for " +
                $"<style=cIsDamage>{100f * Config.specialDamageCoefficient.Value}% damage</style>. " +
                $"Each strike briefly shreds <style=cIsDamage>armor</style> by <style=cIsDamage>{Config.specialArmorShred.Value}</style>, " +
                $"and <style=cIsUtility>regenerates {Config.specialRechargeAmount.Value}%</style> battery, doubled on Critical Strikes. " +
                $"This skill can overcharge the battery up to <style=cIsUtility>120%</style>.");
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