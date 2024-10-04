# Change Log
`0.5.3`
- Updated for SotS patch
- Fixed Squall spawning for Player team only
- Rewrote some skill descriptions + keywords
- Turned down emissive strength of Headhunter skin
  
<i>I do not own the SotS DLC; if there are any compatability issues with the new DLC content, then I am not aware of them.</i>

<details>
    <summary>Previous patches</summary>
  
`0.5.2`
```
• Fixed game crashing if attempting to use Fleetfoot while Nullified
    - I can't believe I didn't realize this sooner
```
    
`0.5.1`
```
• Massively reduced emission strength of the Thrust VFX

• Changed Pathfinder's default sort order in the character select lobby to be right after Loader
    - This can be configured
```
    
`0.5.0`
```
• Added lore entry
```
    
`0.4.3`
```
• Created new config option to enable/disable Squall's laser pointer

• Fixed camera not following the ragdoll post-death

• Shock Bolas' initial explosion now applies slight downward force to airborne enemies

• Squall will no longer leash to you while in Attack mode

• Minor code optimization
```
    
`0.4.2`
```
• Base duration of Thrust reduced from 0.8 to 0.7 seconds

• Slightly altered jump + descent animations

• Icons do not generate mipmaps anymore

• Added important disclaimer to ReadMe
```
    
`0.4.1`
```
• Updated cachedName in SurvivorDef

• Shock Bolas now always has a minimum 0.1 second aim duration

• Improved logic for RecalculateStats() hook
```
    
`0.4.0`
```
• Added Mastery skin

• Squall's missile launcher is back to firing projectiles instead of orbs
    - The orb missiles had poor synergy with Backup Mags, so I am reverting the change.
    
• Fleetfoot's dash distance will now receive diminishing returns from movement speed
    - To compensate, the base distance has been increased.
    - This will result in a greater dash distance at below ~185% movement speed, and lower distance at greater values.
    
• Rending Talons default cooldown reduced from 8 to 6 seconds.
    
• Rending Talons now grants a movement speed buff and 100% air control while airborne.

• Added a VFX trail to Rending Talons to better reflect its hitbox.
    - This is a temporary measure while I work on real VFX.
    - But I might just keep it as is.
```
    
`0.3.2`
```
• Improved logic for Go for the Throat
    - Should no longer interfere with Nemmando's Gouge (let me know if it still does)
```
    
`0.3.1`
```
• Item displays for Trophy Hunter's Tricorn and Sawmerang were producing wonky bugs so I'm temporarily disabling them while I work on a fix
• Spare Drone Parts added to Squall's item blacklist
    - He will still benefit from their effects should you pick up the item
```

`0.3.0`
```
• Added item displays for vanilla + SotV items
• Squall's missile launcher now fires an orb instead of a projectile
    - This should result in better performance and 100% accuracy, but missiles will no longer retarget
• Removed Kjaro's and Runald's bands from Squall's item blacklist
• Config should now update automatically with new patches
```

`0.2.5`
```
• (Hopefully) Fixed issue where Go for the Throat would not recharge Squall's battery as a guest in multiplayer
    - Might require further testing. Please contact me if the issue persists
• Minor code optimization
```
`0.2.4`
```
• This time for sure (please)
```
`0.2.3`
```
• Emergency update cause I'm a dumbass and broke some serious shit
```
`0.2.2`
```
• Allied Tesla Troopers' Tesla Gauntlet now recharges Squall's battery
```
`0.2.1`
```
• Thrust
    - Base damage increased 200% > 250%
    - Piercing damage multiplier reduced from 1.5 > 1.3
• Fixed some config inconsistencies
• Slightly adjusted Squall's missile launcher to fire a missile at the start instead of the halfway point
• Dio's Best Friend and Pluripotent Larva added to Squall's item blacklist
• Fixed issue where reviving with Dio's Best Friend or Pluripotent Larva would cause Squall's battery and skill icons to disappear
```
`0.2.0`

Pathfinder
```
• Thrust : New functionality
    - Piercing. Thrust your spear forward for 200% damage.
    - Piercing Keyword: Striking with the tip of the spear deals 300% damage and bypasses armor instead.
• Explosive Javelin
    - Damage reduced 900% > 800%
    - Explosion now has "sweetspot" type damage falloff
• Shock Bolas
    - No longer deals damage on initial impact
    - CD increased 16s > 18s
    - Flight speed reduced 200 > 150
• Rending Talons
    - Added 20% movement speed buff while spinning
```
Squall
```
• Missile Launcher
    - Cooldown reduced 12s > 3s
    - Number of missiles reduced 4 > 2
    - Missiles now fire at the halfway point and end of the skill cast
• Attempted to network Squall's battery gauge. Let me know how this works in multiplayer.
• Fixed bug where, at extremely high attack speeds, Go for the Throat would strike infinitely
```
`0.1.4`
```
• Forgot to actually implement the config changes last time
```

`0.1.3`
```
• Activating Follow mode now teleports Squall to you if the distance is great enough
    - this should fix Squall not appearing in Voidling's phases 2 and 3
• Issuing an Attack Command will no longer cancel Go For the Throat
• Added config options for some skill CDs
• In multiplayer, Squall's beeping sounds should now only be audible to his owner
• Fixed issue with Rending Talons where being frozen would cause you to get stuck mid spinning animation
• Fixed more language tokens
```

`0.1.2`
```
• Actually included the right readme this time
```

`0.1.1`
```
• Squall now inherits your equipment (but won't use it)
• Added Gesture of the Drowned to Squall's item blacklist
• Fixed some of Squall's language tokens
• Fixed issue where holding down M1 would not throw shurikens properly
• Fixed issue with RiskUI where Squall's special stock was not shown in the skill icon panel
• Slightly thickened Squall's laser pointer for better visibility
```

`0.1.0`
```
• Initial Release
```
</details>
