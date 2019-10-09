using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using Overload;
using System.IO;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine.Networking;

namespace mod_WeaponSelection
{
    //add a trigger on UIElement Draw (Prefix)
    //that checks wether it should draw customise next if so -> set a bool to true
    // it also checks wether bool is true and wether it should draw sth else
    // if so bool = false and an initialisation for some values


    // github test (REEE)

    [HarmonyPatch(typeof(GameManager), "Start")]
    internal class ConsolePatch2
    {

        private static void Postfix(GameManager __instance)
        {
            //uConsole.RegisterCommand("menustate", "Missile ammo", new uConsole.DebugCommand(ConsolePatch2.CmdMenuState));
            uConsole.RegisterCommand("missile_ammo", "Missile ammo", new uConsole.DebugCommand(ConsolePatch2.CmdShowMissileAmmo));
            uConsole.RegisterCommand("weaponorder", "shows weapon priorization", new uConsole.DebugCommand(ConsolePatch2.CmdShowWeaponOrder));
            uConsole.RegisterCommand("missileorder", "shows missile priorization", new uConsole.DebugCommand(ConsolePatch2.CmdShowMissileOrder));
            uConsole.RegisterCommand("toggleprimaryorder", "", new uConsole.DebugCommand(ConsolePatch2.CmdTogglePrimary));
            uConsole.RegisterCommand("togglesecondaryorder", "", new uConsole.DebugCommand(ConsolePatch2.CmdToggleSecondary));
            uConsole.RegisterCommand("showneverselect", "", new uConsole.DebugCommand(ConsolePatch2.CmdShowNeverSelect));

            

            uConsole.RegisterCommand("menustate", "", new uConsole.DebugCommand(ConsolePatch2.CmdMenuState));
            
            AOSwitchLogic.Initialise();
            

           

        }

       

        private static void CmdShowNeverSelect()
        {
            uConsole.Log("Ignored Primaries:");
            int counter = 0;
            for (int i = 0; i < 8; i++)
            {
                if(AOSwitchLogic.PrimaryNeverSelect[i])
                {
                    counter++;
                    uConsole.Log(" "+counter+" - "+AOSwitchLogic.PrimaryPriorityArray[i]);
                }
            }
            uConsole.Log(" ");

           uConsole.Log("Ignored Secondaries:");
            counter = 0;
            for (int i = 0; i < 8; i++)
            {
                if (AOSwitchLogic.SecondaryNeverSelect[i])
                {
                    counter++;
                    uConsole.Log(" " + counter + " - " + AOSwitchLogic.SecondaryPriorityArray[i]);
                }
            }
        }

        private static void CmdTogglePrimary()
        {         
            AOControl.primarySwapFlag = !AOControl.primarySwapFlag;
            uConsole.Log("[WPS] Primary weapon swapping: " + AOControl.primarySwapFlag);
            AOUIElement.DrawMpAutoselectOrderingScreen.saveToFile();
        }

        private static void CmdToggleSecondary()
        {
            AOControl.secondarySwapFlag = !AOControl.secondarySwapFlag;
            uConsole.Log("[WPS] Secondary weapon swapping: " + AOControl.secondarySwapFlag);
            AOUIElement.DrawMpAutoselectOrderingScreen.saveToFile();
        }

        private static void CmdShowMissileAmmo()
        {
            if (GameManager.m_local_player != null)
            {
                uConsole.Log("Missile_Ammo:");
                for (int i = 0; i < 8; i++)
                {
                    uConsole.Log("missile " + i + " :" + ((int)GameManager.m_local_player.m_missile_ammo[i]).ToString());
                }
                uConsole.Log("");
            }
            else
            {
                uConsole.Log("Missile Ammo cannot be displayed, local player is null");
            }
        }

        private static void CmdShowWeaponOrder()
        {
            uConsole.Log("Weapons");
            for(int i = 0; i < 8; i++)
            {
                uConsole.Log(" - "+i+": "+ AOSwitchLogic.PrimaryPriorityArray[i]);
            }
        }

        private static void CmdShowMissileOrder()
        {
            uConsole.Log("Missiles");
            for (int i = 0; i < 8; i++)
            {
                uConsole.Log(" - " + i + ": " + AOSwitchLogic.SecondaryPriorityArray[i]);
            }
        }


        // outcomment this command in the public versions to prevent conflicts with Tobias weapon mods
        private static void CmdMenuState()
        {
            uConsole.Log("Debugging Platform...");
            uConsole.Log("m_menu_state: " + MenuManager.m_menu_state.ToString());
            uConsole.Log("m_menu_substate: " + MenuManager.m_menu_sub_state.ToString());
            uConsole.Log("m_menu_selection: " + UIManager.m_menu_selection.ToString());
            uConsole.Log("m_menu_micro_state: " + MenuManager.m_menu_micro_state.ToString());
        }


        
       

       

        

       

    }
}
