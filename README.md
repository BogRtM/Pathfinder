## Known issues
https://github.com/BogRtM/Pathfinder/issues

Due to the fact that summons are controlled by the server in multiplayer, it is highly recommended that the Pathfinder player be the host, otherwise there will be latency in commanding Squall.

# The Pathfinder 
The Pathfinder is a glass cannon melee survivor who commands a robotic falcon, Squall. Together, they can dish out tremendous DPS, and take down any game!

• Squall can be commanded to focus on selected enemies, or return to you when necessary.<br/>
• Unlike other melee survivors, you have low HP and no armor; poke from afar when necessary and choose your engagements wisely.<br/>
• The Pathfinder prefers items that increase his burst damage potential, while Squall benefits greatly from attack speed and critical chance.<br/>
• Lysate Cell is highly recommended!

This survivor was inspired by Insect Glaive from Monster Hunter, and Rexxar from the Warcraft series.

![image](https://user-images.githubusercontent.com/55299061/182016018-01f87dca-f87a-4b41-a582-06d1b4d28c5b.png)

![PF_Image3](https://user-images.githubusercontent.com/55299061/181102681-2d86e7df-3009-4755-83aa-750fe811c9e6.png)
![PF_Image2](https://user-images.githubusercontent.com/55299061/181102712-5d287fe8-1e36-4504-b19c-397af44fa6a2.png)
![PF_Image1](https://user-images.githubusercontent.com/55299061/181102728-f211865b-aee2-4930-82af-c6302d006d23.png)

## Important
This is an early access, alpha release of a WIP survivor. He is still incomplete, and in active development. As such, please understand that he may be buggy, and that many things are still liable to change for the 1.0 release, even his gameplay and kit design. My intent on releasing him as he is now is to find bugs and to gather external feedback. Please feel free to contact `Bog#4770` on discord with bug reports or feedback. For now, a config file has been provided for you to play around with *some* of his values.

## Credits
```
• bruh
    - Base skin
    - Squall design
    
• Mr. Bones
    - Mastery skin
    
• rob + TheTimeSweeper
    - Providing the Henry character base template
    
• Bog
    - Character concept + design
    - coding
    - animations
    - skill icons
    - custom VFX
    - custom UI
```
Custom SFX sourced from
```
• mixkit.co
• zapsplat.com
```

## To-Do List and Planned Updates
### High priority
```
• Fix bugs and incompatibilities
• Ensure working multiplayer functionality
• Write lore
• Make custom master prefab for Pathfinder NPC
• Implement an alt secondary skill that encourages a more melee-focused playstyle
• VFX + SFX for Rending Talons
• alt skills for Squall
```
### Medium priority
```
• Improve animations as necessary
• Compatibility for Ancient Scepter mod
```
### Low priority
```
• Achievements + unlockables
• Make more custom SFX
```
And some other stuff I'm probably forgetting

## Contact Me
Reach out to `Bog#4770` on Discord with feedback, or find me on the official Risk of Rain 2 modding server.

## Special Thanks
```
• First and foremost, EnforcerGang, for inspiring me to do this in the first place
• The good folk of the RoR2 modding server, for teaching me how to code and for being a great source of motivation
• Vibe and Scruff
• My friends from the netherweebs server
```
----
![PathfinderPortrait](https://user-images.githubusercontent.com/55299061/181103931-0f2a2d6a-53fd-4346-929c-1210799c735b.png)
![Concept1](https://user-images.githubusercontent.com/55299061/181116317-8ae8084a-a07a-42b5-8508-ad3ecd54b14f.png)
![Concept2](https://user-images.githubusercontent.com/55299061/181116345-9c446691-d1a0-43c0-93b4-ba81e9415dbc.png)

## DISCLAIMER

In the case that this mod becomes deprecated or unplayable, and I have completely disappeared for an extended period of time and cannot be contacted, I grant full permission to update and maintain this mod to anybody who wishes to do so, as long as proper credit is given to myself and all others who have worked on Pathfinder. I will do my best to keep the github repo updated with all the latest project files.

## Change Log

### Latest Patch

`0.4.2`
```
• Base duration of Thrust reduced from 0.8 to 0.7 seconds

• Slightly altered jump + descent animations

• Icons do not generate mipmaps anymore

• Added important disclaimer to ReadMe
```

<details>
    <summary>Previous patches</summary>
    
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
