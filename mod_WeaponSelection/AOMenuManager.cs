using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using Overload;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.Networking;

namespace mod_WeaponSelection
{

    // github test

    // BUGS:
    // 1. loadouts get affected when clicking the autoselect menu button with the mouse //HAHHAHAHA FIXED AFTER AN EMBARASSING LONG TIME, GET SOME CODING SKILLS FUCKER
    // 2. exit button is not working // FIXED
    // 3. page up / down doesnt work // FUCK THAT
    // 4. unnecessary code in parts  // BETTER
    // 5. AOIUElement calls Initialise far to often
    // 
    // To-Add:
    // 1. necessary arrays/ variables should be in this class and also only get changed here 
    //    to keep AUIElement as a pure specify "how to draw elements" class
    // 2. add missile swap to highest priority into the switch logic
    // 3. add a distinct sound if autoselect swaps to a devastator
    // 4. initialise the mod/textfile when overload gets started
    //
    // Figure-Out:
    // how to handle/check for inputs
    // how to access private methods
    //
    //
  




    [HarmonyPatch(typeof(MenuManager), "MpCustomizeUpdate")]
    class MpCustomizeMenuLogic
    {
        public static int selected;
        public static int selected2;
        public static int loadout1LastTick;
        public static int loadout2LastTick;

        public static void Postfix()
        {
            selected = AOUIElement.DrawMpAutoselectOrderingScreen.returnPrimarySelected();
            selected2 = AOUIElement.DrawMpAutoselectOrderingScreen.returnSecondarySelected();
            switch (MenuManager.m_menu_sub_state)
            {
                case MenuSubState.ACTIVE:
                    if (MenuManager.m_menu_micro_state == 3)
                    {
                        switch (UIManager.m_menu_selection)
                        {
                            case 200:
                            case 201:
                            case 202:
                            case 203:
                                if (UIManager.PushedSelect(100))
                                {
                                    MenuManager.m_menu_micro_state = UIManager.m_menu_selection - 200;
                                    MenuManager.UIPulse(1f);
                                    GameManager.m_audio.PlayCue2D(364, 0.4f, 0.07f, 0f, false);
                                }
                                break;
                            case 1720:
                                if (UIManager.PushedSelect(100)) doSelectedStuffForPrimary(0);
                                break;
                            case 1721:
                                if (UIManager.PushedSelect(100)) doSelectedStuffForPrimary(1);
                                break;
                            case 1722:
                                if (UIManager.PushedSelect(100)) doSelectedStuffForPrimary(2);
                                break;
                            case 1723:
                                if (UIManager.PushedSelect(100)) doSelectedStuffForPrimary(3);
                                break;
                            case 1724:
                                if (UIManager.PushedSelect(100)) doSelectedStuffForPrimary(4);
                                break;
                            case 1725:
                                if (UIManager.PushedSelect(100)) doSelectedStuffForPrimary(5);
                                break;
                            case 1726:
                                if (UIManager.PushedSelect(100)) doSelectedStuffForPrimary(6);
                                break;
                            case 1727:
                                if (UIManager.PushedSelect(100)) doSelectedStuffForPrimary(7);
                                break;
                            case 1728:
                                if (UIManager.PushedSelect(100)) doSelectedStuffForSecondary(0);
                                break;
                            case 1729:
                                if (UIManager.PushedSelect(100)) doSelectedStuffForSecondary(1);
                                break;
                            case 1730:
                                if (UIManager.PushedSelect(100)) doSelectedStuffForSecondary(2);
                                break;
                            case 1731:
                                if (UIManager.PushedSelect(100)) doSelectedStuffForSecondary(3);
                                break;
                            case 1732:
                                if (UIManager.PushedSelect(100)) doSelectedStuffForSecondary(4);
                                break;
                            case 1733:
                                if (UIManager.PushedSelect(100)) doSelectedStuffForSecondary(5);
                                break;
                            case 1734:
                                if (UIManager.PushedSelect(100)) doSelectedStuffForSecondary(6);
                                break;
                            case 1735:
                                if (UIManager.PushedSelect(100)) doSelectedStuffForSecondary(7);
                                break;
                            case 2000:
                                if (UIManager.PushedSelect(100)) doNeverSelectStuffForPrimary(0);
                                break;
                            case 2001:
                                if (UIManager.PushedSelect(100)) doNeverSelectStuffForPrimary(1);
                                break;
                            case 2002:
                                if (UIManager.PushedSelect(100)) doNeverSelectStuffForPrimary(2);
                                break;
                            case 2003:
                                if (UIManager.PushedSelect(100)) doNeverSelectStuffForPrimary(3);
                                break;
                            case 2004:
                                if (UIManager.PushedSelect(100)) doNeverSelectStuffForPrimary(4);
                                break;
                            case 2005:
                                if (UIManager.PushedSelect(100)) doNeverSelectStuffForPrimary(5);
                                break;
                            case 2006:
                                if (UIManager.PushedSelect(100)) doNeverSelectStuffForPrimary(6);
                                break;
                            case 2007:
                                if (UIManager.PushedSelect(100)) doNeverSelectStuffForPrimary(7);
                                break;
                            case 2010:
                                if (UIManager.PushedSelect(100)) doNeverSelectStuffForSecondary(0);
                                break;
                            case 2011:
                                if (UIManager.PushedSelect(100)) doNeverSelectStuffForSecondary(1);
                                break;
                            case 2012:
                                if (UIManager.PushedSelect(100)) doNeverSelectStuffForSecondary(2);
                                break;
                            case 2013:
                                if (UIManager.PushedSelect(100)) doNeverSelectStuffForSecondary(3);
                                break;
                            case 2014:
                                if (UIManager.PushedSelect(100)) doNeverSelectStuffForSecondary(4);
                                break;
                            case 2015:
                                if (UIManager.PushedSelect(100)) doNeverSelectStuffForSecondary(5);
                                break;
                            case 2016:
                                if (UIManager.PushedSelect(100)) doNeverSelectStuffForSecondary(6);
                                break;
                            case 2017:
                                if (UIManager.PushedSelect(100)) doNeverSelectStuffForSecondary(7);
                                break;
                            case 2100:
                                if(UIManager.PushedSelect(100))
                                {
                                    if (AOControl.primarySwapFlag || AOControl.secondarySwapFlag)
                                    {
                                        AOControl.primarySwapFlag = false;
                                        AOControl.secondarySwapFlag = false;
                                        SFXCueManager.PlayCue2D(SFXCue.enemy_detonatorA_death_roll, 0.8f, 0f, 0f, false);
                                        //SFXCueManager.PlayRawSoundEffect2D(SoundEffect.door_close2, 1f, -0.2f, 0.25f, false);
                                    }
                                    else
                                    {
                                        AOControl.primarySwapFlag = true;
                                        AOControl.secondarySwapFlag = true;
                                        SFXCueManager.PlayCue2D(SFXCue.enemy_detonatorB_alert, 0.8f, 0f, 0f, false);
                                       // SFXCueManager.PlayRawSoundEffect2D(SoundEffect.door_open2, 1f, -0.2f, 0.25f, false);
                                    }
                                    AOUIElement.DrawMpAutoselectOrderingScreen.saveToFile();
                                }                          
                                break;
                            case 2101: //REPLACE
                                if (UIManager.PushedSelect(100))
                                {
                                    if(AOControl.patchPrevNext)
                                    {
                                        AOControl.patchPrevNext = false;
                                        SFXCueManager.PlayCue2D(SFXCue.guidebot_response_negative, 0.8f, 0f, 0f, false);
                                    }
                                    else
                                    {
                                        AOControl.patchPrevNext = true;
                                        SFXCueManager.PlayCue2D(SFXCue.guidebot_objective_found, 0.8f, 0f, 0f, false);
                                    }
                                    AOUIElement.DrawMpAutoselectOrderingScreen.saveToFile();
                                }
                                break;
                            case 2102:
                                if (UIManager.PushedSelect(100))
                                {
                                    if (AOControl.secondarySwapFlag)
                                    {
                                        AOControl.secondarySwapFlag = false;
                                        SFXCueManager.PlayCue2D(SFXCue.guidebot_response_negative, 0.8f, 0f, 0f, false);
                                    }
                                    else
                                    {
                                        AOControl.secondarySwapFlag = true;
                                        SFXCueManager.PlayCue2D(SFXCue.guidebot_objective_found, 0.8f, 0f, 0f, false);
                                    }
                                    AOUIElement.DrawMpAutoselectOrderingScreen.saveToFile();
                                }
                                break;
                            case 2103:
                                if (UIManager.PushedSelect(100))
                                {
                                    if (AOControl.primarySwapFlag)
                                    {
                                        AOControl.primarySwapFlag = false;
                                        SFXCueManager.PlayCue2D(SFXCue.guidebot_response_negative, 0.8f, 0f, 0f, false);
                                    }
                                    else
                                    {
                                        AOControl.primarySwapFlag = true;
                                        SFXCueManager.PlayCue2D(SFXCue.guidebot_objective_found, 0.8f, 0f, 0f, false);
                                    }
                                    AOUIElement.DrawMpAutoselectOrderingScreen.saveToFile();
                                }
                                break;
                            case 2104: //
                                if (UIManager.PushedSelect(100))
                                {
                                    if (AOControl.zorc)
                                    {
                                        AOControl.zorc = false;
                                        SFXCueManager.PlayCue2D(SFXCue.guidebot_response_negative, 0.8f, 0f, 0f, false);
                                    }
                                    else
                                    {
                                        AOControl.zorc = true;
                                        SFXCueManager.PlayCue2D(SFXCue.guidebot_objective_found, 0.8f, 0f, 0f, false);
                                    }
                                    AOUIElement.DrawMpAutoselectOrderingScreen.saveToFile();
                                }
                                break;
                            case 2105: //
                                if (UIManager.PushedSelect(100))
                                {
                                    if (AOControl.COswapToHighest)
                                    {
                                        AOControl.COswapToHighest = false;
                                        SFXCueManager.PlayCue2D(SFXCue.guidebot_response_negative, 0.8f, 0f, 0f, false);
                                    }
                                    else
                                    {
                                        AOControl.COswapToHighest = true;
                                        SFXCueManager.PlayCue2D(SFXCue.guidebot_objective_found, 0.8f, 0f, 0f, false);
                                    }
                                    AOUIElement.DrawMpAutoselectOrderingScreen.saveToFile();
                                }
                                break;

                      

                            default:
                                if (UIManager.PushedSelect(100) && UIManager.m_menu_selection == 100)
                                {
                                    uConsole.Log("Definitly 203 " + Player.Mp_loadout1 + " : " + Player.Mp_loadout2);
                                    UIManager.DestroyAll(false);
                                    MenuManager.PlaySelectSound(1f);
                                    if( AOControl.isCurrentlyInLobby )
                                    {
                                        MenuManager.ChangeMenuState(MenuState.MP_PRE_MATCH_MENU, false);
                                        
                                    }
                                    else
                                    {
                                        MenuManager.ChangeMenuState(MenuState.MP_MENU, false);
                                    }
                                    AOUIElement.DrawMpAutoselectOrderingScreen.isInitialised = false;
                                    
                                }
                                break;

                        }
                    }
                    else
                    {
                        //uConsole.Log("NOT 203 "+Player.Mp_loadout1 + " : " + Player.Mp_loadout2);
                        if(Player.Mp_loadout1 == 203 || Player.Mp_loadout2 == 203)
                        {
                            Player.Mp_loadout1 = loadout1LastTick;
                            Player.Mp_loadout2 = loadout2LastTick;
                        }
                        else
                        {
                            loadout1LastTick = Player.Mp_loadout1;
                            loadout2LastTick = Player.Mp_loadout2;
                        }
                        if(UIManager.PushedSelect(100) && UIManager.m_menu_selection == 203)
                        {
                            //MenuManager.SetDefaultSelection(-1);
                            MenuManager.m_menu_micro_state = 3;
                            MenuManager.UIPulse(1f);
                            GameManager.m_audio.PlayCue2D(364, 0.4f, 0.07f, 0f, false);
                        }
                        
                    }



                    break;
            }
        }


        private static void doNeverSelectStuffForPrimary(int i)
        {
            AOSwitchLogic.PrimaryNeverSelect[i] = !AOSwitchLogic.PrimaryNeverSelect[i];
            if (!AOSwitchLogic.PrimaryNeverSelect[i])
            {
                SFXCueManager.PlayCue2D(SFXCue.hud_weapon_cycle_close, 0.8f, 0f, 0f, false);
            }
            else
            {
                SFXCueManager.PlayCue2D(SFXCue.hud_weapon_cycle_picker, 0.8f, 0f, 0f, false);
            }
            AOUIElement.DrawMpAutoselectOrderingScreen.saveToFile();
        }

        private static void doNeverSelectStuffForSecondary(int i)
        {
            AOSwitchLogic.SecondaryNeverSelect[i] = !AOSwitchLogic.SecondaryNeverSelect[i];
            if (!AOSwitchLogic.SecondaryNeverSelect[i])
            {
                SFXCueManager.PlayCue2D(SFXCue.hud_weapon_cycle_close, 0.8f, 0f, 0f, false);
            }
            else
            {
                SFXCueManager.PlayCue2D(SFXCue.hud_weapon_cycle_picker, 0.8f, 0f, 0f, false);
            }
            AOUIElement.DrawMpAutoselectOrderingScreen.saveToFile();
        }

        private static void doSelectedStuffForPrimary(int i)
        {
            if (selected < 1)
            {
                AOUIElement.DrawMpAutoselectOrderingScreen.isPrimarySelected[i] = true;
                GameManager.m_audio.PlayCue2D(364, 0.4f, 0.07f, 0f, false);
            }
            else
            {
                AOUIElement.DrawMpAutoselectOrderingScreen.isPrimarySelected[i] = true;
                AOUIElement.DrawMpAutoselectOrderingScreen.SwapSelectedPrimary();
                SFXCueManager.PlayCue2D(SFXCue.ui_upgrade, 0.8f, 0f, 0f, false);
                
            }
        }


        private static void doSelectedStuffForSecondary(int i)
        {
            if (selected2 < 1)
            {
                AOUIElement.DrawMpAutoselectOrderingScreen.isSecondarySelected[i] = true;
                GameManager.m_audio.PlayCue2D(364, 0.4f, 0.07f, 0f, false);
            }
            else
            {
                AOUIElement.DrawMpAutoselectOrderingScreen.isSecondarySelected[i] = true;
                AOUIElement.DrawMpAutoselectOrderingScreen.SwapSelectedSecondary();
                SFXCueManager.PlayCue2D(SFXCue.ui_upgrade, 0.8f, 0f, 0f, false);
            }
        }
    }

}




