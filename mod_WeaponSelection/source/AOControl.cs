﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Overload;
using Harmony;

namespace mod_WeaponSelection
{
    class AOControl
    {

        public static bool isCurrentlyInLobby = false;

    }


    // To-Do:
    // - add "never select" logic
    // - add "never select" gui
    // - add options file
    // - add commands to control this mod
    // - debug.log min every overload method possibly related to missile switches in order to find a good jumppoint
    // - add "missile switch on running empty" onto the jumppoint
    // - add swap to next higher/lower weapon/missile based on priority
    // - add a gui to define a button for that stuff
    //
    // To-Do but unrelated to this mod:
    // - Serverbrowser based on json file
    // - audio replacement in menu for havok 
    // - guided observer mode
    // - static team colors (Anarchy/Team-Anarchy(<=2)/CTF) ENEMY = red, ALLY = blue

    [HarmonyPatch(typeof(UIElement), "DrawMpPreMatchMenu")]
    internal class ClientInLobby
    {
        public static void Postfix()
        {
            AOControl.isCurrentlyInLobby = true;
        }
    }

    [HarmonyPatch(typeof(UIElement), "DrawMpMenu")]
    internal class ClientNotInLobby
    {
        public static void Postfix()
        {      
            AOControl.isCurrentlyInLobby = false;         
        }
    }



}
