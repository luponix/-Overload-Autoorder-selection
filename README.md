# -Overload-Autoorder-selection
 Info: 
 This is a mod meant to be used along with olmod (https://github.com/arbruijn/olmod/releases/latest)
 It aims to give you more control about when overload should swap to what weapon
 If you are using an old version, update to the latest or deinstall your current one
 
 ________________________________________________________________________________________________________________________________
![1](https://github.com/luponix/-Overload-Autoorder-selection/blob/master/github-ReadMe.PNG)
Menu Explanation:
 The Menu is split into two Columns
  * The left handles the Primary Weapons
  * The right handles the Secondary Weapons
 The higher a weapon is placed the higher is its priority
 You swap Weapons by clicking at the names of the two weapons
 you want to swap.
 with the "+", "-" buttons you can set wether a weapons should never be automatically selected
 Weapons that will never be selected are marked red.
 
 Additional Options:
 * Mod:   -> turns this mod on or off, when turning the mod on it also sets "Primary Autoselect" to "Never" 
 * Alert: -> Displays a warning and plays a sound if a Devastator gets autoselected as its the weapon that kills
             you easily if spamming the fire button @Zorc #Prevent Old Dads to blow themselves up
 * Swap To: -> This sets wether autoorder swaps to the highest available(HIGHEST) or the picked up(PICKUP) weapon
               if the swap is valid (the picked up weapon is higher than the current one)            
 //Is not in the current version
 * Replace: -> If turned on it will modify the functions that allow you to swap weapons manually to swap 
               based on Priority instead of Overloads fixed weapon Order 
               ("Next Weapon" would swap to the next higher prioritized weapon)
               ("Prev Weapon" would swap to the next lower prioritized weapon)
               and so on for missiles

________________________________________________________________________________________________________________________________

Commands:
 * weaponorder : shows the priority of the primarys
 * missileorder : -||-
 * toggleprimaryorder : disables all logic related to primary weapons
 * togglesecondaryorder :  disables all logic related to secondary weapons
 * showneverselect : displays all weapons that are on the neverselect list
 
________________________________________________________________________________________________________________________________ 

Completed Features: 
  * added a priority list
  * added a menu under Multiplayer/Customize/Autoorder
  * added Neverselect option
  * Mod swaps weapon on pickup/empty energy/empty ammo
  * Warning if a Devastator gets selected
  
To Add:
  * swap to next highest missile when shot empty
  * swap to highest weapon and missile on respawn
  * Modify the (Prev Weapon/next Weapon)/Missile Functions to 
    swap based on Priority
  * set Primary Autoselect to Never   
    
To Look At:
  * changing the icon order of DrawHud to show the Priority
    

