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
                AOSwitchLogic.PrimaryNeverSelect[selection[0]] = false;
                AOSwitchLogic.PrimaryNeverSelect[selection[1]] = false;

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
                AOSwitchLogic.SecondaryNeverSelect[selection[0]] = false;
                AOSwitchLogic.SecondaryNeverSelect[selection[1]] = false;

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
                    sw.WriteLine(AOControl.primarySwapFlag);
                    sw.WriteLine(AOControl.secondarySwapFlag);
                    sw.WriteLine(AOControl.COswapToHighest);
                    sw.WriteLine(AOControl.patchPrevNext);
                    sw.WriteLine(AOControl.zorc);
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
                    AOControl.last_valid_description = "REPLACES THE `PREV/NEXT WEAPON` FUNCTION WITH `SWAP TO NEXT HIGHER/LOWER PRIORITIZED WEAPONS`";
                    return "REPLACES THE `PREV/NEXT WEAPON` FUNCTION WITH `SWAP TO NEXT HIGHER/LOWER PRIORITIZED WEAPONS`";
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
                if( n == 2103 )
                {
                    AOControl.last_valid_description = "TOGGLES EVERYTHING RELATED TO PRIMARY WEAPONS IN THIS MOD";
                    return "TOGGLES EVERYTHING RELATED TO PRIMARY WEAPONS IN THIS MOD";
                }
                if (n == 2102)
                {
                    AOControl.last_valid_description = "TOGGLES EVERYTHING RELATED TO SECONDARY WEAPONS IN THIS MOD";
                    return "TOGGLES EVERYTHING RELATED TO SECONDARY WEAPONS IN THIS MOD";
                }
                if (n == 2104)
                {
                    // Alert
                    AOControl.last_valid_description = "DISPLAY A WARNING IF A DEVASTATOR GETS AUTOSELECTED";
                    return "DISPLAY A WARNING IF A DEVASTATOR GETS AUTOSELECTED";
                }
                if (n == 2105)
                {
                    // On Swap
                    AOControl.last_valid_description = "SETS WETHER ON PICKUP SHOULD SWAP TO THE PICKED UP (IF VALID) OR THE HIGHEST";
                    return "SETS WETHER ON PICKUP SHOULD SWAP TO THE PICKED UP (IF VALID) OR THE HIGHEST";
                }

                else
                {
                    return AOControl.last_valid_description;
                }  
            }

            public static int getWeaponIconIndex( string weapon )
            {
                if (weapon.Equals("IMPULSE") || weapon.Equals("FALCON")) return 0;
                if (weapon.Equals("CYCLONE") || weapon.Equals("MISSILE_POD")) return 1;
                if (weapon.Equals("REFLEX") || weapon.Equals("HUNTER")) return 2;
                if (weapon.Equals("SHOTGUN") || weapon.Equals("CREEPER")) return 3;
                if (weapon.Equals("DRILLER") || weapon.Equals("NOVA")) return 4;
                if (weapon.Equals("FLAK") || weapon.Equals("DEVASTATOR")) return 5;
                if (weapon.Equals("THUNDERBOLT") || weapon.Equals("TIMEBOMB")) return 6;
                if (weapon.Equals("LANCER") || weapon.Equals("VORTEX")) return 7; 
                else
                {
                    uConsole.Log("-AUTOORDERSELECT- [ERROR] getWeaponIconIndex didnt recognise the given weapon string");
                    return 0;
                }
            }


            public static void DrawPriorityList(UIElement uie)
            {
                //uConsole.Log("x:" +UIManager.m_mouse_pos.x + "| y:"+ UIManager.m_mouse_pos.y);


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


                Color ceas = UIManager.m_col_ui4;

                Vector2 left = position;
                Vector2 right = position;
                left.x += 75;
                right.x += 240;
                
                //Draws the weapon / neverselect buttons
                for (int i = 0; i < 8; i++)
                {
                    //uie.DrawWideBox(position, 100f, 28f, Color.red, 0.2f, 7); //TEST
                    //UIManager.DrawQuadBarHorizontal(position, 100f, 18f, 30, Color.red, 12); //TEST
                    //UIManager.DrawQuadUIInner(position, num, 10f, c, this.m_alpha, 11, 0.75f);

                    int primaryindex = getWeaponIconIndex(AOSwitchLogic.PrimaryPriorityArray[i]); //temporary
                    int secondaryindex = getWeaponIconIndex(AOSwitchLogic.SecondaryPriorityArray[i]);
                    UIManager.DrawSpriteUI(left, 0.15f, 0.15f, ceas, uie.m_alpha, 26+primaryindex);

                    UIManager.DrawSpriteUI(right, 0.15f, 0.15f, ceas, uie.m_alpha, 104+secondaryindex);

                    left.y += 50;
                    right.y += 50;

                    if (AOSwitchLogic.PrimaryNeverSelect[i]) // irgendwas mit i
                    {
                        uie.DrawWideBox(position, 100f, 28f, Color.red, 0.2f, 7);
                        UIManager.DrawQuadBarHorizontal(position, 100f, 18f, 30f, Color.red, 12); 
                    }
                    position.x -= 150f;
                    string s = "";
                    if(!AOSwitchLogic.PrimaryNeverSelect[i])
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
                    if (!AOSwitchLogic.SecondaryNeverSelect[i])
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
                string mod = "";
                string mod1 = "";
                string mod2 = "";
                if (AOControl.primarySwapFlag || AOControl.secondarySwapFlag)
                {
                    mod += "ACTIVE";
                }
                else
                {
                    mod += "INACTIVE";
                }
                if (AOControl.primarySwapFlag )
                {
                    mod1 += "ON";
                }
                else if(!AOControl.primarySwapFlag)
                {
                    mod1 += "OFF";
                }
                if (AOControl.secondarySwapFlag)
                {
                    mod2 += "ON";
                }
                else if (!AOControl.secondarySwapFlag)
                {
                    mod2 += "OFF";
                }
                uie.SelectAndDrawItem("Mod: "+mod, position, 2100, false, 0.3f, 0.45f);
                position.y += 2f;
                Vector2 cust;
                cust.x = 542;
                cust.y = 123;

                /*
                AOControl.drag.x = position.x;//UIManager.m_mouse_pos.x;
                AOControl.drag.y = UIManager.m_mouse_pos.y;
                uConsole.Log("x:" + UIManager.m_mouse_pos.x + "| y:" + UIManager.m_mouse_pos.y);*/


                cust.x = 540;
                cust.y = -123;
                //UIManager.DrawQuadUIInner(cust, 97f, 1f, UIManager.m_col_ui2, 0.6f, 11, 0.75f);
                
                position.y += 50f;
                position.x += 5f;
                //uie.SelectAndDrawItem("Weapon Logic: " + mod1, position, 2103, false, 0.27f, 0.4f);
                position.y += 50f;
                //uie.SelectAndDrawItem("Missile Logic: " + mod2, position, 2102, false, 0.27f, 0.4f);
                cust.x = 540;
                cust.y = -22;
                //UIManager.DrawQuadUIInner(cust, 97f, 1f, UIManager.m_col_ui2, 0.6f, 11, 0.75f);
                // 2101 is reserved for patch prev/next logic

                string a1 = AOControl.patchPrevNext ? "ON" : "OFF";
                string b = AOControl.zorc ? "ON" : "OFF";
                string c = AOControl.COswapToHighest ? "HIGHEST" : "PICKUP";

                position.x -= 5f;
                position.y += 132; //original 147
                //var AOControl.patchPrevNext
                uie.SelectAndDrawItem("REPLACE: "+ a1, position, 2101, false, 0.3f, 0.45f); // Possible Options: DEFAULT/PRIORITY
                position.y += 50;
                //var AOControl.zorc
                uie.SelectAndDrawItem("ALERT: "+ b, position, 2104, false, 0.3f, 0.45f); // Possible Options: OFF/ON
                position.y += 50;
                uie.SelectAndDrawItem("SWAP TO: " +c, position, 2105, false, 0.3f, 0.45f); // Possible Options: DEFAULT/PRIORITY
                
                //this adds a short description of what the button that is currently selected does if its pressed
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
 