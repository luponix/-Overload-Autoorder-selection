using Harmony;
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


        // github test



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
                    sw.WriteLine(AOSwitchLogic.PrimaryNeverSelect[0]);
                    sw.WriteLine(AOSwitchLogic.PrimaryNeverSelect[1]);
                    sw.WriteLine(AOSwitchLogic.PrimaryNeverSelect[2]);
                    sw.WriteLine(AOSwitchLogic.PrimaryNeverSelect[3]);
                    sw.WriteLine(AOSwitchLogic.PrimaryNeverSelect[4]);
                    sw.WriteLine(AOSwitchLogic.PrimaryNeverSelect[5]);
                    sw.WriteLine(AOSwitchLogic.PrimaryNeverSelect[6]);
                    sw.WriteLine(AOSwitchLogic.PrimaryNeverSelect[7]);
                    sw.WriteLine(AOSwitchLogic.SecondaryNeverSelect[0]);
                    sw.WriteLine(AOSwitchLogic.SecondaryNeverSelect[1]);
                    sw.WriteLine(AOSwitchLogic.SecondaryNeverSelect[2]);
                    sw.WriteLine(AOSwitchLogic.SecondaryNeverSelect[3]);
                    sw.WriteLine(AOSwitchLogic.SecondaryNeverSelect[4]);
                    sw.WriteLine(AOSwitchLogic.SecondaryNeverSelect[5]);
                    sw.WriteLine(AOSwitchLogic.SecondaryNeverSelect[6]);
                    sw.WriteLine(AOSwitchLogic.SecondaryNeverSelect[7]);
                }
            }

            public static string selectionToDescription(int n)
            {
                if( n == 2100 )
                {
                    AOControl.last_valid_description = "TOGGLES WETHER THE WHOLE MOD SHOULD BE ACTIVE";
                    return "TOGGLES WETHER THE WHOLE MOD SHOULD BE ACTIVE";
                }
                if (n == 2101)
                {
                    AOControl.last_valid_description = "REPLACES THE `PREV//NEXT WEAPON` FUNCTION WITH `SWAP TO NEXT HIGHER//LOWER PRIORITIZED WEAPONS`";
                    return "REPLACES THE `PREV//NEXT WEAPON` FUNCTION WITH `SWAP TO NEXT HIGHER//LOWER PRIORITIZED WEAPONS`";
                }
                if ( n <= 2017 && n >= 2000 )
                {
                    AOControl.last_valid_description = "TOGGLES WETHER THIS WEAPON SHOULD NEVER BE SELECTED";
                    return "TOGGLES WETHER THIS WEAPON SHOULD NEVER BE SELECTED";
                }
                if (n <= 1735 && n >= 1720)
                {
                    AOControl.last_valid_description = "CHANGE THE ORDER BY CLICKING AT THE TWO WEAPONS YOU WANT TO SWAP";
                    return "CHANGE THE ORDER BY CLICKING AT THE TWO WEAPONS YOU WANT TO SWAP";
                }
                else
                {
                    return AOControl.last_valid_description;
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
               

                position.x -= 160f;
                position2.x += 160f;
                Vector2 temp_pos = position;

                //this would be a good candidate to put into another method
                //(string s, Vector2 pos, int selection, bool fade




                //Draws the weapon / neverselect buttons
                for (int i = 0; i < 8; i++)
                {
                   /* uie.DrawWideBox(position, 100f, 28f, Color.red, 0.2f, 7);
                    UIManager.DrawQuadBarHorizontal(position, 100f, 18f, 30, Color.red, 12);*/
                    //UIManager.DrawQuadUIInner(position, num, 10f, c, this.m_alpha, 11, 0.75f);
                    if(AOSwitchLogic.PrimaryNeverSelect[i]) // irgendwas mit i
                    {
                        uie.DrawWideBox(position, 100f, 28f, Color.red, 0.2f, 7);
                        UIManager.DrawQuadBarHorizontal(position, 100f, 18f, 30, Color.red, 12);
                    }
                    position.x -= 150f;
                    string s = "";
                    if(AOSwitchLogic.PrimaryNeverSelect[i])
                    {
                        s += "+";
                    }
                    else
                    {
                        s += "-";
                    }
                    uie.SelectAndDrawItem(s,position,2000+i, false, 0.022f, 1f);               
                    position.x += 150f;
                    uie.SelectAndDrawHalfItem(Primary[i], position, 1720+i, false);
                    position.y += 50;
                    if(AOSwitchLogic.SecondaryNeverSelect[i])
                    {
                        uie.DrawWideBox(position2, 100f, 28f, Color.red, 0.2f, 7);
                        UIManager.DrawQuadBarHorizontal(position2, 100f, 18f, 30, Color.red, 12);
                    }
                    position2.x += 150f;
                    string a = "";
                    if (AOSwitchLogic.SecondaryNeverSelect[i])
                    {
                        a += "+";
                    }
                    else
                    {
                        a += "-";
                    }
                    uie.SelectAndDrawItem(a, position2, 2010 + i, false, 0.022f, 1f);
                    position2.x -= 150f;
                    uie.SelectAndDrawHalfItem(Secondary[i], position2, 1728 + i, false);
                    position2.y += 50;
                }

                position = Vector2.up * (UIManager.UI_TOP + 70f);
                position.y += 164f;
                position.x += 540f;
                //position.y += 350f;
                string mod = "";
                if(AOControl.primarySwapFlag || AOControl.secondarySwapFlag)
                {
                    mod += "ACTIVE";
                }
                else
                {
                    mod += "INACTIVE";
                }
                uie.SelectAndDrawItem("Mod: "+mod, position, 2100, false, 0.3f, 0.45f);
                //position.y += 50;
                //uie.SelectAndDrawItem("NOT ADDED YET", position, 2101, false, 0.3f, 0.45f);
                //add a different decription based on what button is selected

                //recenters position2
                position2.x -= 160f;
                position2.y -= 14f;

                string k = selectionToDescription(UIManager.m_menu_selection);
                uie.DrawLabelSmall(position2, k, 500f);

               /*
                Color color3 = UIManager.m_col_ub0;
                Vector2 temp_pos2 = position;

                
                //UIManager.DrawBoxOutline(temp_pos2 , temp_pos ,color3,3,7);

                temp_pos.x -= 95f;
                UIManager.DrawQuadBarHorizontal(temp_pos, 1.2f, 1.2f, 195f, color3, 41);
               /*
                temp_pos.x -= 95f;
                temp_pos.y -= 95f;
                uie.DrawDigitsThree(temp_pos, 032, 0.45f, StringOffset.RIGHT, UIManager.m_col_ui2, uie.m_alpha);

                //temp_pos.y = pos.y;
                temp_pos.x -= 95f;
                UIManager.DrawSpriteUI(temp_pos, 0.2f, 0.2f, UIManager.m_col_ub1, uie.m_alpha, 41);
                temp_pos.x += 95f;
                UIManager.DrawSpriteUI(temp_pos, 0.2f, 0.2f, UIManager.m_col_ub1, uie.m_alpha, 41);
                temp_pos.x -= 80f;

                uie.DrawStringSmall(Loc.LS("IDK"), temp_pos, 0.4f, StringOffset.LEFT, UIManager.m_col_ui2, uie.m_alpha, -1f);

                   */


            }


        }



    
    }
}
 