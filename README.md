# News
Unfortunately you will have to update your config to receive the updated values.

First balance patch is out. Pathfinder came out hitting a bit too hard, so I will be dialing his strength back a tad. The javelin + bolas kit that I dropped was intended to be a melee + ranged hybrid build, but it was too much ranged and not enough melee. I shifted some power from his javelin and bolas into his Thrust, to reward you greater for accuracy, and for fighting in melee range. Additionally, Squall's missile launcher has received a rework that spreads out its volley of missiles more over time, so that he must remain in Attack mode longer to get the same amount of missiles. Check the change log for more detailed notes.

I do not yet have an opinion on Squall's power or battery gauge. It seems to me that he is scaling extremely well into the late game far too consistently, but I will hold off and wait before making any more changes to him.

## Known issues
https://github.com/BogRtM/Pathfinder/issues

Seems like the two primary concerns in multiplayer are that as a guest, sometimes Squall does not follow orders, and Go for the Throat does not recharge Squall's battery. I am looking into this, but if you encounter one of these two scenarios, please don't hesitate to send me the log. For now, it is highly recommended to be the host yourself if you plan to play him in multiplayer.

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
    - 3D modeling
    - rigging
    - Squall design
    
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
• Item displays
• Write lore
• Make custom master prefab for Pathfinder NPC
• Mastery skin
• Implement an alt secondary skill that encourages a more melee-focused playstyle
• VFX + SFX for Rending Talons
• alt skills for Squall
```
### Medium priority
```
• Improve animations as necessary
• Mod compatibility
• Improve Fleetfoot VFX
• Make custom UI elements for command mode
```
### Low priority
```
• Achievements + unlockables
• Make more custom SFX
```
And some other stuff I'm probably forgetting

## Volunteers Wanted
I am still very much a beginner when it comes to coding and modding, and am requesting the aid of more experienced coders who can help me diagnose bugs, optimize code, and help ensure working multiplayer functionality. If you have advice or suggestions on how to improve my code, please reach out to me.

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

## Change Log
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
