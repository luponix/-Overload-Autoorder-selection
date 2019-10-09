using Overload;
using Harmony;
using System.IO;
using UnityEngine;
using System;

namespace mod_WeaponSelection
{
    class AOControl
    {
        //used to compare weapon options config
        public const int VersionNumber = 138;

        //public static Vector2 drag = Vector2.zero;
        //public static bool dragt = true;

            /// <summary>
            /// could it be that overload broadcasts entire playerstates in one big packet instead of short updates about changed game states
            /// 
            /// </summary>

        public static bool isCurrentlyInLobby = false;
        public static bool isInitialised = false;

        public static string last_valid_description = "CHANGE THE ORDER BY CLICKING AT THE TWO WEAPONS YOU WANT TO SWAP";

        //path to config file // not needed anymore, delete on sight
        public static string OptionFilePath = Path.Combine(Application.persistentDataPath, "WPS-ModOptions-File.txt");

        //VARIABLES SET IN THE CONFIG FILE
        public static bool primarySwapFlag = true; //toggles the whole primary selection logic
        public static bool secondarySwapFlag = true; // toggles the whole secondary selection logic
        public static bool COswapToHighest = false; // toggles wether on pickup  the logic should switch to the highest weapon or the picked up weapon if its higher
        public static bool patchPrevNext = true;  // toggles wether the default prev/next weapon swap methods should be replaced with a priority based prev/next
        public static bool zorc = false; // extra alert for old men when the devastator gets autoselected, still need to find an annoying sound for that

     


    }
 

    // To-Do:
    // - patch the previous/next logic
    // - 
    // - 
    // - 
    // - add commands to control this mod
    // - debug.log min every overload method possibly related to missile switches in order to find a good jumppoint
    // - add "missile switch on running empty" onto the jumppoint
    // - add swap to next higher/lower weapon/missile based on priority
    // - add a gui to define a button for that stuff
    //
    // To-Do but unrelated to this mod:
    // - Serverbrowser based on json file
    // - 
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
