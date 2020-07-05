﻿using System;
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
    [HarmonyPatch(typeof(GameManager), "Start")]
    internal class ConsolePatch2
    {

        private static void Postfix(GameManager __instance)
        {     
            uConsole.RegisterCommand("missile_ammo", "Missile ammo", new uConsole.DebugCommand(ConsolePatch2.CmdShowMissileAmmo));
            uConsole.RegisterCommand("weaponorder", "shows weapon priorization", new uConsole.DebugCommand(ConsolePatch2.CmdShowWeaponOrder));
            uConsole.RegisterCommand("missileorder", "shows missile priorization", new uConsole.DebugCommand(ConsolePatch2.CmdShowMissileOrder));
            uConsole.RegisterCommand("toggleprimaryorder", "", new uConsole.DebugCommand(ConsolePatch2.CmdTogglePrimary));
            uConsole.RegisterCommand("togglesecondaryorder", "", new uConsole.DebugCommand(ConsolePatch2.CmdToggleSecondary));
            uConsole.RegisterCommand("showneverselect", "", new uConsole.DebugCommand(ConsolePatch2.CmdShowNeverSelect));
            uConsole.RegisterCommand("toggle_hud", "Toggles some hud elements", new uConsole.DebugCommand(ConsolePatch2.CmdToggleHud));
            //uConsole.RegisterCommand("camera_set", "allows setting other graphic settings", new uConsole.DebugCommand(ConsolePatch2.SetGraphics));
            uConsole.RegisterCommand("version", "prints the AO version", new uConsole.DebugCommand(ConsolePatch2.CmdVersion));
            uConsole.RegisterCommand("setfov", "prints the AO version", new uConsole.DebugCommand(ConsolePatch2.CmdSetFov));

            AOSwitchLogic.Initialise();
        }

        private static void CmdSetFov()
        {

            int fov = uConsole.GetInt();
            if (fov == -1)
            {
                fov = 90;
            }

            float num = (float)Screen.width / (float)Screen.height;
            num = Mathf.Clamp(num, 1f, 3f);
            float num2 = num / 1.77777779f;
            float rel_aspect_ratio = num2;
            num2 = ((num2 <= 1f) ? ((float)Math.Sqrt((double)num2)) : Mathf.Pow(num2, 0.25f));

            Viewer.SCREEN_FOV = fov / num2;

            if (!GameplayManager.VRActive)
            {
                GameManager.GOAL_FOV = Viewer.SCREEN_FOV;
                UIManager.gm.m_ui_collision_camera.fieldOfView = Viewer.SCREEN_FOV;
            }
            uConsole.Log("Set FOV to " + fov);
        }



        private static void CmdVersion()
        {
            uConsole.Log("version: " + AOControl.version);
        }

        private static void SetGraphics()
        {
            GameManager.m_viewer.SetCameraGraphicsSettings(false, 0, false, 0, false, 0);
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

        private static void CmdToggleHud()
        {
            AOControl.miasmic = !AOControl.miasmic;
            uConsole.Log("Toggled HUD ! current state : " + AOControl.miasmic);
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

        


    }
}
