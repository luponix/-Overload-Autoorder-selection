﻿using Harmony;
using Overload;
using UnityEngine;
using Tobii.Gaming;
using UnityEngine.XR;
using System;
using System.IO;

namespace mod_WeaponSelection
{
    class AOUIElement
    {






        [HarmonyPatch(typeof(UIElement), "DrawMpTabs")]
        internal class AddFourthTab
        {
            public static bool Prefix(Vector2 pos, int tab_selected, UIElement __instance)
            {
                float w = 378f; // 511
                __instance.DrawWideBox(pos, w, 22f, UIManager.m_col_ub2, __instance.m_alpha, 7);
                string[] array = new string[]
                {
                __instance.GetMpTabName(0),
                __instance.GetMpTabName(1),
                __instance.GetMpTabName(2),
                "AUTOSELECT"
                };

                

                for (int i = 0; i < array.Length; i++)
                {
                    pos.x = (((float)i - 1f) * 198f)-99f ;//265 -132
                    __instance.TestMouseInRect(pos, 84f, 16f, 200 + i, false); // original value = 112
                    if (UIManager.m_menu_selection == 200 + i)
                    {
                        __instance.DrawWideBox(pos, 84f, 19f, UIManager.m_col_ui4, __instance.m_alpha, 7);
                    }
                    if (i == tab_selected)
                    {
                        __instance.DrawWideBox(pos, 84f, 16f, UIManager.m_col_ui4, __instance.m_alpha, 12);
                        __instance.DrawStringSmall(array[i], pos, 0.6f, StringOffset.CENTER, UIManager.m_col_ub3, 1f, -1f);
                    }
                    else
                    {
                        __instance.DrawWideBox(pos, 84f, 16f, UIManager.m_col_ui0, __instance.m_alpha, 8);
                        __instance.DrawStringSmall(array[i], pos, 0.6f, StringOffset.CENTER, UIManager.m_col_ui1, 1f, -1f);
                    }
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(UIElement), "DrawMpCustomize")]
        internal class DrawMpAutoselectOrderingScreen
        {
            static string[] PrimaryPriorityArray = new string[8];
            static string[] SecondaryPriorityArray = new string[8];

            static void Postfix(UIElement __instance)
            {
                
                //AOSwitchLogic.Initialise();
                if( isInitialised == false )
                {
                    Initialise();
                    isInitialised = true; //should be set to false when leaving the MpCustomize Menu
                }      

                int menu_micro_state = MenuManager.m_menu_micro_state;
                if (menu_micro_state == 3)
                {
                    // Draw the Autoselect Ordering menu
                    DrawPriorityList(__instance);
                }
            }

            public static void Initialise()
            {
                Primary[0] = AOSwitchLogic.PrimaryPriorityArray[0];
                Primary[1] = AOSwitchLogic.PrimaryPriorityArray[1];
                Primary[2] = AOSwitchLogic.PrimaryPriorityArray[2];
                Primary[3] = AOSwitchLogic.PrimaryPriorityArray[3];
                Primary[4] = AOSwitchLogic.PrimaryPriorityArray[4];
                Primary[5] = AOSwitchLogic.PrimaryPriorityArray[5];
                Primary[6] = AOSwitchLogic.PrimaryPriorityArray[6];
                Primary[7] = AOSwitchLogic.PrimaryPriorityArray[7];

                Secondary[0] =AOSwitchLogic.SecondaryPriorityArray[0];
                Secondary[1] = AOSwitchLogic.SecondaryPriorityArray[1];
                Secondary[2] = AOSwitchLogic.SecondaryPriorityArray[2];
                Secondary[3] = AOSwitchLogic.SecondaryPriorityArray[3];
                Secondary[4] = AOSwitchLogic.SecondaryPriorityArray[4];
                Secondary[5] = AOSwitchLogic.SecondaryPriorityArray[5];
                Secondary[6] = AOSwitchLogic.SecondaryPriorityArray[6];
                Secondary[7] = AOSwitchLogic.SecondaryPriorityArray[7];
            }

            public static bool isInitialised = false;

            public static string[] Primary = {
                AOSwitchLogic.PrimaryPriorityArray[0],
                AOSwitchLogic.PrimaryPriorityArray[1],
                AOSwitchLogic.PrimaryPriorityArray[2],
                AOSwitchLogic.PrimaryPriorityArray[3],
                AOSwitchLogic.PrimaryPriorityArray[4],
                AOSwitchLogic.PrimaryPriorityArray[5],
                AOSwitchLogic.PrimaryPriorityArray[6],
                AOSwitchLogic.PrimaryPriorityArray[7],
            };
            public static string[] Secondary = {
                AOSwitchLogic.SecondaryPriorityArray[0],
                AOSwitchLogic.SecondaryPriorityArray[1],
                AOSwitchLogic.SecondaryPriorityArray[2],
                AOSwitchLogic.SecondaryPriorityArray[3],
                AOSwitchLogic.SecondaryPriorityArray[4],
                AOSwitchLogic.SecondaryPriorityArray[5],
                AOSwitchLogic.SecondaryPriorityArray[6],
                AOSwitchLogic.SecondaryPriorityArray[7]
            };

            public static bool[] isPrimarySelected = new bool[8];

            public static bool[] isSecondarySelected = new bool[8];
            

            public static int returnPrimarySelected()
            {
                int counter = 0;
                for(int i = 0; i < 8; i++)
                {
                    if(isPrimarySelected[i])
                    {
                        counter++;
                    }
                }
                return counter;
            }

            public static int returnSecondarySelected()
            {
                int counter = 0;
                for (int i = 0; i < 8; i++)
                {
                    if (isSecondarySelected[i])
                    {
                        counter++;
                    }
                }
                return counter;
            }

            public static void SwapSelectedPrimary()
            {
                int counter = 0;
                int[] selection = { 0, 0 };   
                for( int i = 0; i < 8; i++ )
                {
                    if(isPrimarySelected[i])
                    {
                        selection[counter] = i;
                        counter++;
                    } 
                    if(counter == 2)
                    {
                        break;
                    }
                }
                string temp = Primary[selection[0]];
                Primary[selection[0]] = Primary[selection[1]];
                Primary[selection[1]] = temp;

                isPrimarySelected[selection[0]] = false;
                isPrimarySelected[selection[1]] = false;

                saveToFile();
                AOSwitchLogic.Initialise();
            }

            public static void SwapSelectedSecondary()
            {
                int counter = 0;
                int[] selection = { 0, 0 };
                for (int i = 0; i < 8; i++)
                {
                    if (isSecondarySelected[i])
                    {
                        selection[counter] = i;
                        counter++;
                    }
                    if (counter == 2)
                    {
                        break;
                    }
                }
                string temp = Secondary[selection[0]];
                Secondary[selection[0]] = Secondary[selection[1]];
                Secondary[selection[1]] = temp;

                isSecondarySelected[selection[0]] = false;
                isSecondarySelected[selection[1]] = false;

                saveToFile();
                AOSwitchLogic.Initialise();
            }

            public static void saveToFile()
            {
                using (StreamWriter sw = File.CreateText(AOSwitchLogic.textFile))
                {
                    
                    sw.WriteLine(Primary[0]);
                    sw.WriteLine(Primary[1]);
                    sw.WriteLine(Primary[2]);
                    sw.WriteLine(Primary[3]);
                    sw.WriteLine(Primary[4]);
                    sw.WriteLine(Primary[5]);
                    sw.WriteLine(Primary[6]);
                    sw.WriteLine(Primary[7]);
                    sw.WriteLine(Secondary[0]);
                    sw.WriteLine(Secondary[1]);
                    sw.WriteLine(Secondary[2]);
                    sw.WriteLine(Secondary[3]);
                    sw.WriteLine(Secondary[4]);
                    sw.WriteLine(Secondary[5]);
                    sw.WriteLine(Secondary[6]);
                    sw.WriteLine(Secondary[7]);

                }
            }


            public static void DrawPriorityList(UIElement uie)
            {
                UIManager.X_SCALE = 0.2f;
                UIManager.ui_bg_dark = true;
                uie.DrawMenuBG();
                Vector2 position = Vector2.up * (UIManager.UI_TOP + 70f);

                position.y += 164f;
                Vector2 position2 = position;

                position.x -= 300f;
                //this would be a good candidate to put into another method
                uie.SelectAndDrawCheckboxItem(Primary[0], position, 1720, isPrimarySelected[0], false, 1f, -1);
                position.y += 50f;
                uie.SelectAndDrawCheckboxItem(Primary[1], position, 1721, isPrimarySelected[1], false, 1f, -1);
                position.y += 50f;
                uie.SelectAndDrawCheckboxItem(Primary[2], position, 1722, isPrimarySelected[2], false, 1f, -1);
                position.y += 50f;
                uie.SelectAndDrawCheckboxItem(Primary[3], position, 1723, isPrimarySelected[3], false, 1f, -1);
                position.y += 50f;
                uie.SelectAndDrawCheckboxItem(Primary[4], position, 1724, isPrimarySelected[4], false, 1f, -1);
                position.y += 50f;
                uie.SelectAndDrawCheckboxItem(Primary[5], position, 1725, isPrimarySelected[5], false, 1f, -1);
                position.y += 50f;
                uie.SelectAndDrawCheckboxItem(Primary[6], position, 1726, isPrimarySelected[6], false, 1f, -1);
                position.y += 50f;
                uie.SelectAndDrawCheckboxItem(Primary[7], position, 1727, isPrimarySelected[7], false, 1f, -1);
                position.y += 50f;

                position2.x += 300f;
                uie.SelectAndDrawCheckboxItem(Secondary[0], position2, 1728, isSecondarySelected[0], false, 1f, -1);
                position2.y += 50f;
                uie.SelectAndDrawCheckboxItem(Secondary[1], position2, 1729, isSecondarySelected[1], false, 1f, -1);
                position2.y += 50f;
                uie.SelectAndDrawCheckboxItem(Secondary[2], position2, 1730, isSecondarySelected[2], false, 1f, -1);
                position2.y += 50f;
                uie.SelectAndDrawCheckboxItem(Secondary[3], position2, 1731, isSecondarySelected[3], false, 1f, -1);
                position2.y += 50f;
                uie.SelectAndDrawCheckboxItem(Secondary[4], position2, 1732, isSecondarySelected[4], false, 1f, -1);
                position2.y += 50f;
                uie.SelectAndDrawCheckboxItem(Secondary[5], position2, 1733, isSecondarySelected[5], false, 1f, -1);
                position2.y += 50f;
                uie.SelectAndDrawCheckboxItem(Secondary[6], position2, 1734, isSecondarySelected[6], false, 1f, -1);
                position2.y += 50f;
                uie.SelectAndDrawCheckboxItem(Secondary[7], position2, 1735, isSecondarySelected[7], false, 1f, -1);

                position2.x -= 300f;
                position2.y += 36f;
                uie.DrawLabelSmall(position2, "CHANGE THE ORDER BY CLICKING AT THE TWO WEAPONS YOU WANT TO SWAP", 500f);

            }


        }



    
    }
}
 