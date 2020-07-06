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
                    sw.WriteLine(AOControl.miasmic);
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
                if (weapon.Equals("CRUSHER") || weapon.Equals("CREEPER")) return 3;
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
                uie.SelectAndDrawItem("Weapon Logic: " + mod1, position, 2103, false, 0.27f, 0.4f);
                position.y += 50f;
                uie.SelectAndDrawItem("Missile Logic: " + mod2, position, 2102, false, 0.27f, 0.4f);
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

            /*
            [HarmonyPatch(typeof(UIElement), "DrawHUDSecondaryWeapon")]
            internal class AlterHUDSecondaryIconOrder
            {
                public static bool Prefix(UIElement __instance, Vector2 pos)
                {

                    if (MenuManager.opt_primary_autoswitch == 0 && AOControl.secondarySwapFlag)
                    {
                        if (GameplayManager.IsMultiplayerActive && NetworkMatch.InGameplay() )
                        {


                            float num = 210f;
                            float num2 = 125f;
                            Color col_ub = UIManager.m_col_ub1;
                            Color col_ui = UIManager.m_col_ui2;
                            Color col_ui2 = UIManager.m_col_ui5;
                            Color color = Color.Lerp(UIManager.m_col_hi4, UIManager.m_col_hi5, UnityEngine.Random.value * UIElement.FLICKER);
                            PlayerShip player_ship = GameManager.m_player_ship;
                            if (player_ship.m_wheel_select_state == WheelSelectState.MISSILE)
                            {
                                Vector2 temp_pos;
                                pos.x += ((MenuManager.opt_hud_weapons != 0) ? -6f : 6f);
                                num -= 70f;
                                num2 -= 5f;
                                float num3 = 0f;
                                temp_pos.y = pos.y - 86f;
                                temp_pos.x = pos.x;
                                __instance.DrawStringSmall(Loc.LS("SECONDARY WEAPON SELECT"), temp_pos, 0.4f, StringOffset.CENTER, col_ui, 1f, -1f);
                                temp_pos.x = temp_pos.x - 150f;
                                UIManager.DrawSpriteUI(temp_pos, 0.2f, 0.2f, col_ub, __instance.m_alpha, 42);
                                temp_pos.x = temp_pos.x + 300f;
                                UIManager.DrawSpriteUI(temp_pos, 0.2f, 0.2f, col_ub, __instance.m_alpha, 42);
                                temp_pos.x = pos.x;
                                temp_pos.y = pos.y - 72f;
                                UIManager.DrawQuadBarHorizontal(temp_pos, 1.2f, 1.2f, 380f, col_ui, 41);
                                temp_pos.y = pos.y + 72f;
                                UIManager.DrawQuadBarHorizontal(temp_pos, 1.2f, 1.2f, 380f, col_ui, 41);
                                temp_pos.y = pos.y + 58f;
                                temp_pos.x = pos.x - 179f;
                                UIManager.DrawSpriteUI(temp_pos, 0.2f, 0.2f, col_ub, __instance.m_alpha, 41);
                                temp_pos.x = pos.x + 179f;
                                UIManager.DrawSpriteUI(temp_pos, 0.2f, 0.2f, col_ub, __instance.m_alpha, 41);
                                temp_pos.y = pos.y - 58f;
                                UIManager.DrawSpriteUI(temp_pos, 0.2f, 0.2f, col_ub, __instance.m_alpha, 41);
                                temp_pos.x = pos.x - 179f;
                                UIManager.DrawSpriteUI(temp_pos, 0.2f, 0.2f, col_ub, __instance.m_alpha, 41);
                                Vector2 vector = RUtility.AngleToVector(player_ship.m_move_dir_angle) * 8f;
                                UIManager.DrawSpriteUIRotated(pos + vector, 0.12f, 0.12f, player_ship.m_move_dir_angle + 1.57079637f, col_ui, __instance.m_alpha, 81);
                                for (int i = 0; i < 8; i++)
                                {
                                    vector = RUtility.AngleToVector(num3) * 20f;
                                    bool flag = GameManager.m_local_player.m_missile_level[i] != WeaponUnlock.LOCKED;
                                    bool flag2 = GameManager.m_local_player.m_missile_type == (MissileType)i;
                                    if (flag2)
                                    {
                                        UIManager.DrawSpriteUIRotated(pos + vector, 0.25f, 0.25f, num3 + 1.57079637f, col_ui, __instance.m_alpha, 41);
                                    }
                                    else
                                    {
                                        UIManager.DrawSpriteUIRotated(pos + vector, 0.2f, 0.2f, num3 + 1.57079637f, col_ub, __instance.m_alpha * ((!flag) ? 0.3f : 1f), 41);
                                    }
                                    vector = pos + RUtility.AngleToVector(num3) * 30f;
                                    vector.x += __instance.WHEEL_OFFSET_H[i];
                                    vector.y += __instance.WHEEL_OFFSET_V[i];
                                    if (flag)
                                    {
                                        if (flag2)
                                        {
                                            col_ui.a = __instance.m_alpha;
                                            UIManager.DrawQuadBarHorizontal(vector, 12f, 12f, num, col_ui, 8);
                                        }
                                        else
                                        {
                                            col_ub.a = __instance.m_alpha * 0.5f;
                                            temp_pos.x = vector.x;
                                            temp_pos.y = vector.y - 11f;
                                            UIManager.DrawQuadBarHorizontal(temp_pos, 1.2f, 1.2f, num + 15f, col_ub, 41);
                                            temp_pos.y = vector.y + 11f;
                                            UIManager.DrawQuadBarHorizontal(temp_pos, 1.2f, 1.2f, num + 15f, col_ub, 41);
                                        }
                                        temp_pos.x = vector.x - (num * 0.5f - 4f);
                                        temp_pos.y = vector.y;
                                        __instance.DrawStringSmall(GameManager.m_local_player.GetMissileName((MissileType)i, false), temp_pos, 0.4f, StringOffset.LEFT, (!flag2) ? col_ui : col_ui2, 1f, 110f);
                                        temp_pos.x = temp_pos.x - 6f;
                                        UIManager.DrawSpriteUI(temp_pos, 0.16f, 0.16f, (!flag2) ? col_ub : col_ui2, __instance.m_alpha, 104 + i);
                                        if (i == 1)
                                        {
                                            temp_pos.x = vector.x + (num * 0.5f + 2f);
                                            __instance.DrawDigitsThree(temp_pos, GameManager.m_local_player.m_missile_ammo[i], 0.5f, StringOffset.RIGHT, col_ui, __instance.m_alpha);
                                        }
                                        else
                                        {
                                            temp_pos.x = vector.x + (num * 0.5f + 2f);
                                            __instance.DrawDigitsTwo(temp_pos, GameManager.m_local_player.m_missile_ammo[i], 0.5f, StringOffset.RIGHT, col_ui, __instance.m_alpha, false);
                                        }
                                    }
                                    num3 += 0.7853982f;
                                }
                            }
                            else
                            {
                                Vector2 temp_pos;
                                col_ui.a = __instance.m_alpha;
                                pos.y += ((!GameplayManager.VRActive) ? 50f : 25f);
                                pos.x -= ((MenuManager.opt_hud_weapons != 0) ? -35f : 35f);
                                if (GameplayManager.IsMultiplayerActive && GameManager.m_local_player.m_mp_team != MpTeam.ANARCHY)
                                {
                                    __instance.DrawMPWeaponOutline(pos, num, 14.5f, 37f);
                                }
                                temp_pos.x = pos.x;
                                temp_pos.y = pos.y - 14f;
                                UIManager.DrawQuadBarHorizontal(temp_pos, 1.2f, 1.2f, num2 * 2f + 5f, col_ui, 41);
                                UIManager.DrawQuadBarHorizontal(pos, 12f, 12f, num, col_ui, 8);
                                temp_pos.y = pos.y;
                                temp_pos.x = pos.x - num * 0.5f;
                                int missile_type = (int)GameManager.m_local_player.m_missile_type;
                                if (GameManager.m_local_player.m_missile_level.Length >= 0 && missile_type < GameManager.m_local_player.m_missile_level.Length)
                                {
                                    UIManager.DrawSpriteUI(temp_pos, 0.18f, 0.18f, color, __instance.m_alpha, (int)(104 + GameManager.m_local_player.m_missile_type));
                                    temp_pos.x = temp_pos.x + 7f;
                                    __instance.DrawStringSmall(GameManager.m_local_player.CurrentMissileName, temp_pos, 0.5f, StringOffset.LEFT, color, 1f, 145f);
                                    if (GameManager.m_local_player.m_missile_level[(int)GameManager.m_local_player.m_missile_type] > WeaponUnlock.LEVEL_1)
                                    {
                                        col_ui2.a = __instance.m_alpha;
                                        temp_pos.x = pos.x + (num * 0.5f - 40f);
                                        UIManager.DrawQuadBarHorizontal(temp_pos, 9f, 9f, 8f, col_ui2, 13);
                                        temp_pos.x = temp_pos.x - 1f;
                                        __instance.DrawStringSmall(GameManager.m_local_player.GetMissileTag(GameManager.m_local_player.m_missile_type), temp_pos, 0.45f, StringOffset.CENTER, UIManager.m_col_ub3, 1f, -1f);
                                    }
                                    temp_pos.x = pos.x + (num * 0.5f + 2f);
                                    __instance.DrawDigitsThree(temp_pos, GameManager.m_local_player.m_missile_ammo[(int)GameManager.m_local_player.m_missile_type], 0.5f, StringOffset.RIGHT, col_ui, __instance.m_alpha);
                                }
                                temp_pos.x = pos.x - num2;
                                UIManager.DrawSpriteUI(temp_pos, 0.2f, 0.2f, col_ub, __instance.m_alpha, 41);
                                temp_pos.x = pos.x + num2;
                                UIManager.DrawSpriteUI(temp_pos, 0.2f, 0.2f, col_ub, __instance.m_alpha, 41);
                                pos.y += 25f;
                                temp_pos.y = pos.y;
                                temp_pos.x = pos.x - num2;
                                UIManager.DrawSpriteUI(temp_pos, 0.2f, 0.2f, col_ub, __instance.m_alpha, 41);
                                temp_pos.x = pos.x + num2;
                                UIManager.DrawSpriteUI(temp_pos, 0.2f, 0.2f, col_ub, __instance.m_alpha, 41);
                                temp_pos.x = pos.x;
                                temp_pos.y = pos.y + 18f;
                                UIManager.DrawQuadBarHorizontal(temp_pos, 1.2f, 1.2f, num2 * 2f + 5f, col_ui, 41);
                                pos.x -= num * 0.5f;
                                for (int j = 0; j < 8; j++)
                                {
                                    bool flag3 = j == (int)GameManager.m_local_player.m_missile_type;
                                    bool flag4 = GameManager.m_local_player.m_missile_level[j] != WeaponUnlock.LOCKED;
                                    int f = AOSwitchLogic.getMissilePriority(AOSwitchLogic.IntToMissileType(j));

                                    UIManager.DrawQuadUI(pos, 10f, 10f, (!flag3) ? col_ub : col_ui2, __instance.m_alpha * ((!flag4) ? 0.4f : ((GameManager.m_local_player.m_missile_ammo[getWeaponIconIndex(AOSwitchLogic.SecondaryPriorityArray[f])] <= 0) ? 0.5f : 1f)), 8);
                                    if (flag4)
                                    {
                                        
                                        UIManager.DrawSpriteUI(pos, 0.17f, 0.17f, (!flag3) ? col_ui : color, __instance.m_alpha * ((GameManager.m_local_player.m_missile_ammo[getWeaponIconIndex(AOSwitchLogic.SecondaryPriorityArray[f])] <= 0) ? 0.15f : 1f), 104 + getWeaponIconIndex(AOSwitchLogic.SecondaryPriorityArray[f]));
                                        pos.y += 13f;
                                        UIManager.DrawQuadUI(pos, 11f, 1.5f, col_ub, 0.5f * __instance.m_alpha, 11);
                                        float num4 = Mathf.Min(1f, (float)GameManager.m_local_player.m_missile_ammo[getWeaponIconIndex(AOSwitchLogic.SecondaryPriorityArray[f])] / (float)GameManager.m_local_player.GetMaxMissileAmmo((MissileType)getWeaponIconIndex(AOSwitchLogic.SecondaryPriorityArray[f])));
                                        UIManager.DrawQuadUI(pos, 11f * num4, 1.5f, col_ui2, __instance.m_alpha, 11);
                                        pos.y -= 13f;
                                    }
                                    pos.x += 30f;
                                }
                            }














                            return false;
                        }

                    }
                    return true;
                }
            }


            */














        }



    
    }
}
 