using System;
using System.IO;
using Harmony;
using Overload;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine.Networking;

namespace mod_WeaponSelection
{



    public static class AOSwitchLogic

    {
        //////////////////////////////////////////////////////////////////////////////////////////////
        ///                       STATUS :
        ///                       - primary swaps need testing on no ammo/no energy
        ///
        ///
        /// 
        //////////////////////////////////////////////////////////////////////////////////////////////

        /*
         * uConsole.Log("UnlockWeaponClient called :" + Player.WeaponNames[wt]);
         * GameplayManager.AddHUDMessage(Loc.LS("UnlockWeaponClient") + " [" + Player.WeaponNames[wt] + "]", -1, true);
         */

        /////////////////////////////////////////////////////////////////////////////////////
        //              PUBLIC VARIABLES                  
        /////////////////////////////////////////////////////////////////////////////////////

        // github test

        public static String[] PrimaryPriorityArray = new String[8];
        public static String[] SecondaryPriorityArray = new String[8];
        public static bool[] PrimaryNeverSelect = new bool[8];
        public static bool[] SecondaryNeverSelect = new bool[8];

        public static Player playerObject = null;
        public static string textFile = Path.Combine(Application.persistentDataPath, "Weapon-Priority-List.txt");

        public static bool initialised = false; // should get set back to false by the exit weapon screen list

        public static string[] EnergyWeapons = { "IMPULSE", "CYCLONE", "REFLEX", "THUNDERBOLT", "LANCER", };
        public static string[] AmmoWeapons = { "CRUSHER", "FLAK", "DRILLER" };

        public static bool isManuallySwapping = false;
        public static int currentMissile = -1;
        public static bool negativeAmmo = false;

        public static bool swapThisGoddamnMissile = false;

        public static bool swap_failed = false;

        public static int frames_to_wait = 5;

        public static int[] missile_ammo = { 0,0,0,0, 0,0,0,0, 999};

        // public static int[] missile_ammo_per_pickup = new int[8];
        // public static int current_missile;

        /////////////////////////////////////////////////////////////////////////////////////
        //              SHARED METHODS                   
        /////////////////////////////////////////////////////////////////////////////////////

        //generally needed improvements:
        // on respawn search highest weapons/missiles
        // interrupt missile switches some methods higher  in order to allow manual switches and check for multiplayer
        // check missiles. name is sometimes not set correctly and  update doesnt get called in some methods ?
        // alle enumeratoren durchgehen welche typen sie haben und ob die methoden sie richtig nutzen NOVA/smart ?

        //if missiles stay funky create your own missile ammo array 

        public static void Initialise()
        {
            MenuManager.opt_primary_autoswitch = 0; 
            if (File.Exists(textFile))
            {
                readContent();
            }
            else
            {
                uConsole.Log("-AUTOSELECTORDER- [ERROR] File does not exist. Creating default priority list");
                Debug.Log("-AUTOSELECTORDER- [ERROR] File does not exist. Creating default priority list");
                createDefaultPriorityFile();
                readContent();
            }
            AOControl.isInitialised = true;
        }

        private static void createDefaultPriorityFile()
        {
            using (StreamWriter sw = File.CreateText(textFile))
            {
                sw.WriteLine("THUNDERBOLT");
                sw.WriteLine("CYCLONE");
                sw.WriteLine("DRILLER");
                sw.WriteLine("IMPULSE");
                sw.WriteLine("FLAK");
                sw.WriteLine("SHOTGUN");
                sw.WriteLine("LANCER");
                sw.WriteLine("REFLEX");
                sw.WriteLine("DEVASTATOR");
                sw.WriteLine("NOVA");
                sw.WriteLine("TIMEBOMB");
                sw.WriteLine("HUNTER");
                sw.WriteLine("VORTEX");
                sw.WriteLine("FALCON");
                sw.WriteLine("MISSILE_POD");
                sw.WriteLine("CREEPER");
                sw.WriteLine(PrimaryNeverSelect[0]);
                sw.WriteLine(PrimaryNeverSelect[1]);
                sw.WriteLine(PrimaryNeverSelect[2]);
                sw.WriteLine(PrimaryNeverSelect[3]);
                sw.WriteLine(PrimaryNeverSelect[4]);
                sw.WriteLine(PrimaryNeverSelect[5]);
                sw.WriteLine(PrimaryNeverSelect[6]);
                sw.WriteLine(PrimaryNeverSelect[7]);
                sw.WriteLine(SecondaryNeverSelect[0]);
                sw.WriteLine(SecondaryNeverSelect[1]);
                sw.WriteLine(SecondaryNeverSelect[2]);
                sw.WriteLine(SecondaryNeverSelect[3]);
                sw.WriteLine(SecondaryNeverSelect[4]);
                sw.WriteLine(SecondaryNeverSelect[5]);
                sw.WriteLine(SecondaryNeverSelect[6]);
                sw.WriteLine(SecondaryNeverSelect[7]);
                sw.WriteLine(AOControl.primarySwapFlag);
                sw.WriteLine(AOControl.secondarySwapFlag);
                sw.WriteLine(AOControl.COswapToHighest);
                sw.WriteLine(AOControl.patchPrevNext);
                sw.WriteLine(AOControl.zorc);
                sw.WriteLine(AOControl.miasmic);
            }
        }

        private static bool stringToBool(string b)
        {
            if (b == "True")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void readContent()
        {
            using (StreamReader file = new StreamReader(textFile))
            {
                int counter = 0;
                string ln;

                //To-Do use default values if encountering sth unexpected in the file
                // (3) is not critical

                while ((ln = file.ReadLine()) != null)
                {
                    ///<summary>
                    /// Contains the priorities of the primary weapons
                    ///</summary>
                    if (counter < 8)
                    {
                        if (ln == "THUNDERBOLT" | ln == "IMPULSE" | ln == "CYCLONE" | ln == "DRILLER" | ln == "LANCER" | ln == "REFLEX" | ln == "FLAK" | ln == "SHOTGUN")
                        {
                            PrimaryPriorityArray[counter] = ln;
                        }
                        else
                        {
                            uConsole.Log("ERROR(1) while reading File, unexpected line content : " + ln);
                            Debug.Log("-AUTOSELECTORDER- [ERROR](1) unexpected line content -> (content: " + ln + " )");

                            return;
                        }

                    }
                    ///<summary>
                    /// Contains the priorities of the secondary weapons
                    ///</summary>
                    else if (counter < 16)
                    {
                        if (ln == "DEVASTATOR" | ln == "TIMEBOMB" | ln == "VORTEX" | ln == "NOVA" | ln == "HUNTER" | ln == "FALCON" | ln == "CREEPER" | ln == "MISSILE_POD")
                        {
                            SecondaryPriorityArray[counter - 8] = ln;
                        }
                        else
                        {
                            uConsole.Log("ERROR(2) while reading File, unexpected line content : " + ln);
                            Debug.Log("-AUTOSELECTORDER- [ERROR](2) unexpected line content -> (content: " + ln + " )");

                            return;
                        }
                    }
                    ///<summary>
                    /// Contains true/false whether primary priorities are neverselected
                    ///</summary>
                    else if (counter < 24)
                    {
                        if (ln == "True" || ln == "False")
                        {
                            AOSwitchLogic.PrimaryNeverSelect[counter - 16] = stringToBool(ln);
                        }
                        else
                        {
                            //if we got here, the data before is fine we just need to generate default for this
                            for (int i = 0; i < 8; i++)
                            {
                                uConsole.Log("REEEEEEEEEEEEEEEEEEEEEEE(1)");
                                AOSwitchLogic.PrimaryNeverSelect[i] = false;
                            }
                        }
                    }
                    ///<summary>
                    /// Contains true/false whether secondary priorities are neverselected
                    ///</summary>
                    else if (counter < 32)
                    {
                        if (ln == "True" || ln == "False")
                        {
                            AOSwitchLogic.SecondaryNeverSelect[counter - 24] = stringToBool(ln);
                        }
                        else
                        {
                            //if we got here, the data before is fine we just need to generate default for this
                            for (int i = 0; i < 8; i++)
                            {
                                uConsole.Log("REEEEEEEEEEEEEEEEEEEEEEE(2)");
                                AOSwitchLogic.SecondaryNeverSelect[i] = false;
                            }
                        }
                    }
                    else if (counter == 32)
                    {
                        if (ln == "True" || ln == "False") { AOControl.primarySwapFlag = stringToBool(ln);  }              
                    }
                    else if (counter == 33)
                    {
                        if (ln == "True" || ln == "False") { AOControl.secondarySwapFlag = stringToBool(ln); }       
                    }
                    else if (counter == 34)
                    {
                        if (ln == "True" || ln == "False") { AOControl.COswapToHighest = stringToBool(ln); }  
                    }
                    else if (counter == 35)
                    {
                        if (ln == "True" || ln == "False") { AOControl.patchPrevNext = stringToBool(ln); }   
                    }
                    else if (counter == 36)
                    {
                        if (ln == "True" || ln == "False") { AOControl.zorc = stringToBool(ln); }   
                    }
                    else if (counter == 37)
                    {
                        if (ln == "True" || ln == "False") { AOControl.miasmic = stringToBool(ln); }
                    }

                    else
                    {
                        // uConsole.Log("ERROR(3) while reading File, unexpected line content : " + ln);
                        Debug.Log("-AUTOSELECTORDER- [ERROR](3) unexpected line content -> (content: " + ln+ " : "+ counter + " )");

                        return;
                    }
                    counter++;
                }
                file.Close();

            }
        }



        /////////////////////////////////////////////////////////////////////////////////////
        //              PRIMARY WEAPONS CHAIN                   
        /////////////////////////////////////////////////////////////////////////////////////
        // Rewritten (1.3.8)

        // before calling this method make sure that this is not triggered by picking up the currently equipped weapon

        public static void maybeSwapPrimary()
        {
            //is there even a potential static option to switch to
            if ( areThereAllowedPrimaries() )
            {
                // case 0: energy but no ammo
                if (GameManager.m_local_player.m_energy > 0 && !(GameManager.m_local_player.m_ammo > 0))
                {
                    //is there an unlocked energy weapon
                    string[] candidates = returnArrayOfUnlockedPrimaries(EnergyWeapons);
                    if (candidates.Length > 0)
                    {
                        string a = returnHighestPrimary(candidates);
                        if (!a.Equals("a")) swapToWeapon(a);
                    }
                    else
                    {
                        swap_failed = true;
                    }
                    return;
                }
                // case 1: ammo but no energy
                if (!(GameManager.m_local_player.m_energy > 0) && GameManager.m_local_player.m_ammo > 0)
                {
                    //is there an unlocked ammo weapon
                    string[] candidates = returnArrayOfUnlockedPrimaries(AmmoWeapons);
                    if (candidates.Length > 0)
                    {
                        string a = returnHighestPrimary(candidates);
                        if (!a.Equals("a")) swapToWeapon(a);
                    }
                    else
                    {
                        swap_failed = true;
                    }
                    return;
                }
                // case 2: ammo and energy
                //give me the highest unlocked weapon
                string[] candidates1 = returnArrayOfUnlockedPrimaries(PrimaryPriorityArray);
                if (candidates1.Length > 0)
                {
                    string a = returnHighestPrimary(candidates1);
                    if (!a.Equals("a")) swapToWeapon(a);
                }  
                return;
            }
        }

        private static bool areThereAllowedPrimaries()
        {
            for( int i = 0; i < 8; i++)
            {
                if (PrimaryNeverSelect[i] == false) return true;
            }
            return false;
        }

        private static string[] returnArrayOfUnlockedPrimaries(string[] arr )
        {          
            int len = arr.Length;
            if( len > 0)
            {
                int counter = 0;
                string[] temp = new string[len];
                for ( int i = 0; i < len; i++)
                {
                    if (isWeaponAccessibleAndNotNeverselect(arr[i]))
                    {
                        temp[counter] = arr[i];
                        counter++;
                    }
                }
                string[] result = new string[counter];
                for( int j = 0; j < counter; j++)
                {
                    result[j] = temp[j];
                }
                return result;
            }
            return new string[0];   
        }

        private static bool isWeaponAccessibleAndNotNeverselect(string weapon)
        {
            if (weapon.Equals("IMPULSE")) return !(GameManager.m_local_player.m_weapon_level[0].ToString().Equals("LOCKED")) && !isPrimaryOnNeverSelectList("IMPULSE");
            if (weapon.Equals("CYCLONE")) return !(GameManager.m_local_player.m_weapon_level[1].ToString().Equals("LOCKED")) && !isPrimaryOnNeverSelectList("CYCLONE");
            if (weapon.Equals("REFLEX")) return !(GameManager.m_local_player.m_weapon_level[2].ToString().Equals("LOCKED")) && !isPrimaryOnNeverSelectList("REFLEX");
            if (weapon.Equals("CRUSHER")) return !(GameManager.m_local_player.m_weapon_level[3].ToString().Equals("LOCKED")) && !isPrimaryOnNeverSelectList("SHOTGUN");
            if (weapon.Equals("DRILLER")) return !(GameManager.m_local_player.m_weapon_level[4].ToString().Equals("LOCKED")) && !isPrimaryOnNeverSelectList("DRILLER");
            if (weapon.Equals("FLAK")) return !(GameManager.m_local_player.m_weapon_level[5].ToString().Equals("LOCKED")) && !isPrimaryOnNeverSelectList("FLAK");
            if (weapon.Equals("THUNDERBOLT")) return !(GameManager.m_local_player.m_weapon_level[6].ToString().Equals("LOCKED")) && !isPrimaryOnNeverSelectList("THUNDERBOLT");
            if (weapon.Equals("LANCER")) return !(GameManager.m_local_player.m_weapon_level[7].ToString().Equals("LOCKED")) && !isPrimaryOnNeverSelectList("LANCER");
            else
            {
                return false;
            }

        }

        private static bool isPrimaryOnNeverSelectList( string weapon )
        {
            for( int i = 0; i < 8; i++ )
            {
                if (weapon.Equals(PrimaryPriorityArray[i])) return PrimaryNeverSelect[i];
            }
            return false;
        }

        private static string returnHighestPrimary( string[] arr )
        {
            for( int i = 0; i < 8; i++ )
            {
                foreach (string sel in arr)
                {
                    if (sel.Equals(PrimaryPriorityArray[i])) return PrimaryPriorityArray[i];
                }
            }
            uConsole.Log("AUTOORDER-  This Case shouldnt be possible [Error 0]");
            return "a";
  
        }

        private static void swapToWeapon(string weaponName)
        {
            if (!(GameManager.m_local_player.m_weapon_type.Equals(stringToWeaponType(weaponName))))
            {
                GameManager.m_local_player.Networkm_weapon_type = stringToWeaponType(weaponName);
                GameManager.m_local_player.CallCmdSetCurrentWeapon(GameManager.m_local_player.m_weapon_type);
                GameManager.m_player_ship.WeaponSelectFX();
            }
            SFXCueManager.PlayRawSoundEffect2D(SoundEffect.hud_notify_message1, 1f, 0.15f, 0.1f, false);
        }

        private static WeaponType stringToWeaponType(string weapon)
        {
            if (weapon.Equals("IMPULSE")) return WeaponType.IMPULSE;
            if (weapon.Equals("CYCLONE")) return WeaponType.CYCLONE;
            if (weapon.Equals("REFLEX")) return WeaponType.REFLEX;
            if (weapon.Equals("CRUSHER")) return WeaponType.CRUSHER;
            if (weapon.Equals("DRILLER")) return WeaponType.DRILLER;
            if (weapon.Equals("FLAK")) return WeaponType.FLAK;
            if (weapon.Equals("THUNDERBOLT")) return WeaponType.THUNDERBOLT;
            if (weapon.Equals("LANCER")) return WeaponType.LANCER;
            else
            {
                return WeaponType.NUM;
            }
        }

        // not part of the chain but helpful
        public static int getWeaponPriority(WeaponType primary)
        {
            if (AOControl.isInitialised)
            {
                string wea = primary.ToString();

                for (int i = 0; i < 8; i++)
                {
                    if (wea.Equals(PrimaryPriorityArray[i]))
                    {
                        return i;
                    }
                    if (wea.Equals("CRUSHER") && PrimaryPriorityArray[i].Equals("SHOTGUN"))
                    {
                        return i;
                    }
                }
                uConsole.Log("-AUTOSELECTORDER- [WARN]: getWeaponPriority:-1, primary wasnt in array");
                return -1;
            }
            else
            {
                uConsole.Log("-AUTOSELECTORDER- [WARN]: getWeaponPriority:-1, priority didnt get initialised");
                return -1;
            }
        }




        /////////////////////////////////////////////////////////////////////////////////////
        //              MISSILES                 
        /////////////////////////////////////////////////////////////////////////////////////

        //needed improvements:
        // 
        // check for current weapon if current weapon equal to pickup dont switch
        // missiles are sometimes not correctly updated. missed commands from submethods that do not get called due to prefix methods ?
        // the problems with hunters could be related to that it shoots 2 hunters per 1 ammo
        // Bug persists

        // NEUER PLAN: du versuchst nach dem natürlichen missileswap deinen eigenen missile swap anzubringen
        // testen ob FindBestPrevMissile eine gute swap methode ist oder ob ich darin eine bool setzen sollte und dann am ende von MaybeFireMissile tatsächlich swappe

        public static int getMissilePriority(MissileType missile)
        {
            //check differences of weapon type to stuff in prioritySecondary
            if (AOControl.isInitialised)
            {
                string mis = missile.ToString();
                for(int i = 0; i < 8; i++)
                {
                    if (mis.Equals(SecondaryPriorityArray[i]))
                    {
                        return i;
                    }
                }
                uConsole.Log("-AUTOSELECTORDER- [WARN]: getMissilePriority:-1, primary wasnt in array");
                return -1;
            }
            else
            {
                uConsole.Log("-AUTOSELECTORDER- [WARN]: getMissilePriority:-1, priority didnt get initialised");
                return -1;
            }

        }

        private static bool areThereAllowedSecondaries()
        {
            for (int i = 0; i < 8; i++)
            {
                if (SecondaryNeverSelect[i] == false) return true;
            }
            return false;
        }

        //REWRITE (1.3.8)


        public static void maybeSwapMissiles()
        {
            int highestMissile = findHighestPrioritizedUseableMissile();
            if (highestMissile == -1)
            {
                //uConsole.Log("findHighestPrioritizedUseableMissile returned no possible missile, if this shit actually happens, ping me(luponix) on discord");
                return;
            }
            else
            {
              //  uConsole.Log("want to swap: highest missile is :" + highestMissile);
                swapToMissile(highestMissile);
                return;
            }

        }
 
        public static int findHighestPrioritizedUseableMissile()
        {
            foreach (string missile in SecondaryPriorityArray)
            {
                int var = missileStringToInt(missile);
                if (GameManager.m_local_player.m_missile_ammo[var] > 0 )
                {
                    return var;
                }
            }
            return -1;
        }

        public static int findHighestPrevMissile()
        {
            int currentMissile = (int)GameManager.m_local_player.m_missile_type;
            foreach (string missile in SecondaryPriorityArray)
            {
                int var = missileStringToInt(missile);
                if (GameManager.m_local_player.m_missile_ammo[var] > 0 && var != currentMissile)
                {
                    return var;
                }
            }
            return -1;
        }

        private static void swapToMissile(int weapon_num)
        {
            //current_missile = weapon_num;
            if (GameManager.m_local_player.m_missile_level[weapon_num] == WeaponUnlock.LOCKED || GameManager.m_local_player.m_missile_ammo[weapon_num] == 0)//GameManager.m_local_player.m_missile_ammo[weapon_num] == 0)
            {
                //uConsole.Log("Did not swap , missile: " + GameManager.m_local_player.m_missile_ammo[weapon_num]);
                return;
            }
            if (GameManager.m_local_player.m_missile_type != (MissileType)weapon_num)
            {
                currentMissile = weapon_num;
                negativeAmmo = false;
                GameManager.m_local_player.Networkm_missile_type = (MissileType)weapon_num;
                // GameManager.m_local_player.CallCmdSetCurrentMissile(GameManager.m_local_player.m_missile_type);
                GameManager.m_local_player.CallCmdSetCurrentMissile(GameManager.m_local_player.Networkm_missile_type);
                //GameManager.m_local_player.m_missile_type = GameManager.m_local_player.Networkm_missile_type; ÄNDERUNG 05.10.2019
                GameManager.m_player_ship.MissileSelectFX();
                GameManager.m_local_player.UpdateCurrentMissileName();
            }
            if( AOControl.zorc )
            {
                if (IntToMissileType(weapon_num).Equals(MissileType.DEVASTATOR))
                {
                    SFXCueManager.PlayCue2D(SFXCue.enemy_boss1_alert, 1f, 0f, 0f, false);
                    GameplayManager.AlertPopup(Loc.LS("DEVASTATOR SELECTED"), string.Empty, 5f);
                }
            }
            
        }

        public static int missileStringToInt(string missile)
        {

            if (missile.Equals("FALCON")) return 0;
            if (missile.Equals("MISSILE_POD")) return 1;
            if (missile.Equals("HUNTER")) return 2;
            if (missile.Equals("CREEPER")) return 3;
            if (missile.Equals("NOVA")) return 4;
            if (missile.Equals("DEVASTATOR")) return 5;
            if (missile.Equals("TIMEBOMB")) return 6;
            if (missile.Equals("VORTEX")) return 7;
            else
            {
                uConsole.Log("<ERROR> |: (missileStringToInt) string missile had unexpected type: " + missile);
                return -1;
            }
        }

        public static int missileProjectileTypeToInt(ProjPrefab missile)
        {
            if (missile == ProjPrefab.missile_falcon) return 0;
            if (missile == ProjPrefab.missile_pod) return 1;
            if (missile == ProjPrefab.missile_hunter) return 2;
            if (missile == ProjPrefab.missile_creeper) return 3;
            if (missile == ProjPrefab.missile_smart) return 4;
            if (missile == ProjPrefab.missile_devastator) return 5;
            if (missile == ProjPrefab.missile_timebomb) return 6;
            if (missile == ProjPrefab.missile_vortex) return 7;
            else
            {
                uConsole.Log("||ERROR IN TRIGGER (helper) hasMissileAmmo(ProjPrefab missile) missile: " + missile);
                return -1;
            }
        }

        public static int missileTypeToInt(MissileType missile)
        {
            if (missile == MissileType.FALCON) return 0;
            if (missile == MissileType.MISSILE_POD) return 1;
            if (missile == MissileType.HUNTER) return 2;
            if (missile == MissileType.CREEPER) return 3;
            if (missile == MissileType.NOVA) return 4;
            if (missile == MissileType.DEVASTATOR) return 5;
            if (missile == MissileType.TIMEBOMB) return 6;
            if (missile == MissileType.VORTEX) return 7;
            else
            {
                uConsole.Log("||ERROR IN TRIGGER (helper) missileTypeToInt(MissileType missile) missile: " + missile);
                return -1;
            }
        }

        public static MissileType IntToMissileType(int missile)
        {
            if (missile == 0) return MissileType.FALCON;
            if (missile == 1) return MissileType.MISSILE_POD;
            if (missile == 2) return MissileType.HUNTER;
            if (missile == 3) return MissileType.CREEPER;
            if (missile == 4) return MissileType.NOVA;
            if (missile == 5) return MissileType.DEVASTATOR;
            if (missile == 6) return MissileType.TIMEBOMB;
            if (missile == 7) return MissileType.VORTEX;
            else
            {
                return MissileType.NUM;
            }
        }

        

        /////////////////////////////////////////////////////////////////////////////////////
        //              HARMONY TRIGGER                 
        /////////////////////////////////////////////////////////////////////////////////////
            
        [HarmonyPatch(typeof(Player), "UnlockWeaponClient")]
        internal class WeaponPickup
        {       
            public static void Postfix(WeaponType wt, bool silent, Player __instance)
            {
                if (MenuManager.opt_primary_autoswitch == 0 && AOControl.primarySwapFlag)
                {
                    if (GameplayManager.IsMultiplayerActive && NetworkMatch.InGameplay() && __instance == GameManager.m_local_player)
                    {
                        int new_weapon = getWeaponPriority(wt);
                        int current_weapon = getWeaponPriority(GameManager.m_local_player.m_weapon_type);

                        if(new_weapon < current_weapon && !PrimaryNeverSelect[new_weapon])
                        {            
                            if(AOControl.COswapToHighest)
                            {
                                AOSwitchLogic.maybeSwapPrimary();
                            }
                            else {    
                                // this method doesnt need to check wether there is ammo or energy as weapon pickups always come with a small amount of it
                                swapToWeapon(wt.ToString());
                            }   
                        }       
                    }
                }
            }
        }
        
        [HarmonyPatch(typeof(UIElement), "DrawHUDArmor")]
        internal class MaybeDrawHUDElementYes
        {
            public static bool Prefix(UIElement __instance)
            {

                // TEST CODE
                Vector2 pos = default(Vector2);
                pos.x = 538.246f;
                pos.y = -161.9803f;
                for( int i = 0; i < 8; i++ )
                {
                    __instance.DrawStringSmall(Player.WeaponNames[i] +" " + GameManager.m_local_player.m_weapon_level[i], pos, 0.5f, StringOffset.CENTER, UIManager.m_col_damage, pos.x / 64, -1f);
                    pos.x += 50f;
                    __instance.DrawStringSmall(Player.MissileNames[i]+" " + GameManager.m_local_player.m_missile_level[i], pos, 0.5f, StringOffset.CENTER, UIManager.m_col_damage, pos.x / 64, -1f);
                    pos.x -= 50f;
                    pos.y -= 20f;
                }
               // REMOVE AFTER TESTING

                if (!AOControl.miasmic)
                {
                    return true;
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(UIElement), "DrawHUDEnergyAmmo")]
        internal class MaybeDrawHUDElementAnd
        {
            public static bool Prefix()
            {
                if (!AOControl.miasmic)
                {
                    return true;
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(UIElement), "DrawHUDIndicators")]
        internal class MaybeDrawHUDElementOr
        {
            public static bool Prefix(UIElement __instance)
            {


                //Debug Marker 
                /*
                Vector2 pos = default(Vector2);
                pos.x = 538.246f;
                pos.y = -161.9803f;
                for (int i = 0; i < 8; i++)
                {
                    int ammo = GameManager.m_local_player.m_missile_ammo[i];
                    __instance.DrawStringSmall(IntToMissileType(i)+": "+ammo, pos, 0.5f, StringOffset.CENTER, UIManager.m_col_damage, pos.x / 64, -1f);
                    pos.y += 20f;
                }*/

                //this part is needed for the weapon selection
                if (frames_to_wait == 0)
                {
                    frames_to_wait = 5;
                    if( missile_ammo[8] != 999)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            if (GameManager.m_local_player.m_missile_ammo[i] < missile_ammo[i] && GameManager.m_local_player.m_missile_ammo[i] == 0)
                            {
                                DelayedSwitchTimer a = new DelayedSwitchTimer();
                                a.Awake();
                                break;
                            }
                        }
                    }
                    else
                    {
                        missile_ammo[8] = 0;
                    }

                    for (int i = 0; i < 8; i++)
                    {
                        missile_ammo[i] = GameManager.m_local_player.m_missile_ammo[i];
                    }
                }
                else
                {
                    frames_to_wait--;
                }

                if (!AOControl.miasmic)
                {
                    return true;
                }
                return false;
            }
        }
        
        // 2.
        [HarmonyPatch(typeof(Player), "RpcSetMissileAmmo")]
        internal class SecondaryPickup
        {//if auto select option on "never" . check needs to be added later
            public static void Postfix(int missile_type, int ammo, Player __instance)
            {
                if (MenuManager.opt_primary_autoswitch == 0 && AOControl.secondarySwapFlag)
                {
                    if (GameplayManager.IsMultiplayerActive && NetworkMatch.InGameplay() && __instance == GameManager.m_local_player)
                    {          

                        if( areThereAllowedSecondaries() )
                        {       
                            int new_missile = getMissilePriority(IntToMissileType(missile_type));
                            int current_missile = getMissilePriority(GameManager.m_local_player.m_missile_type);

                            if (new_missile < current_missile && !SecondaryNeverSelect[new_missile])
                            {
                                if (AOControl.COswapToHighest)
                                {
                                    AOSwitchLogic.maybeSwapMissiles();          
                                }
                                else
                                {
                                    swapToMissile(missile_type);  
                                }
                            }
                        }                      
                    }
                }
            }
        }
       
        //if (NetworkManager.IsServer() && NetworkMatch.InGameplay() && player_ship.m_ready_to_respawn)			 
        /*
        [HarmonyPatch(typeof(UIElement), "DrawMpOverlayLoadout")]
        internal class OnRespawnCheckForHunterInCurrentLoadout
        {
            public static void Postfix()
            {
                if (MenuManager.opt_primary_autoswitch == 0 && AOControl.secondarySwapFlag)
                {
                    if (GameplayManager.IsMultiplayerActive && NetworkMatch.InGameplay())
                    {
                        uConsole.Log("DrawMpOverlayLoadout called");
                        if (!initialised)
                        {
                            Initialise();
                            initialised = true;
                            AOControl.isCurrentlyInLobby = false;
                        }
                        maybeSwapMissiles();
                    }
                }
            }
        }*/


        /*
         * Der Bug besteht also darin das er eine rakete zu früh swappt und dann nicht die rakete wiederfindet bzw vermutlich direkt wieder zurückschaltet
         * Das scheint nicht an unseren methoden und daten zu liegen sondern an unserem trigger
         * 
         * TargetUpdateCurrentMissileName
         * 
         * [SyncVar]
           public MissileType m_missile_type_prev;
           es ist möglich das der server immer im vorraus wissen will zu welcher waffe der spieler als nächtes wechseln wird
           prev also nicht previous sondern preview
         *
         * ///////////////////////////////////////////////////////////////////////////////////////////
         * --> statt PlayerFire GetNextMissileWithAmmo benutzen um eine andere rakete einzusetzen <--------------------
         * ///////////////////////////////////////////////////////////////////////////////////////////
         */

        // WORKS (1.3.8)
        // last change: stopped prefix return false if the weapon couldnt get swapped because of an empty weapons array
        [HarmonyPatch(typeof(Player), "SwitchToAmmoWeapon")]
        internal class OutOfAmmo
        {
            private static bool Prefix(Player __instance)
            {
                if (MenuManager.opt_primary_autoswitch == 0 && AOControl.primarySwapFlag)
                {
                    if (GameplayManager.IsMultiplayerActive && NetworkMatch.InGameplay() && __instance == GameManager.m_local_player)
                    {            
                        AOSwitchLogic.maybeSwapPrimary();
                        if (swap_failed)
                        {
                            uConsole.Log("-AUTOORDER- [EB] swap failed on trying to switch to an ammo weapon");
                            swap_failed = false;
                            return true;
                        }
                        else
                        {
                            uConsole.Log(" - Denied Execution of original Method");
                            return false;
                        }
                    }
                    return true;
                }
                return true;
            }
        }

        // WORKS (1.3.8)
        // last change: stopped prefix return false if the weapon couldnt get swapped because of an empty weapons array
        [HarmonyPatch(typeof(Player), "SwitchToEnergyWeapon")]
        internal class OutOfEnergy
        {
            private static bool Prefix(Player __instance)
            {
                if (MenuManager.opt_primary_autoswitch == 0 && AOControl.primarySwapFlag)
                {
                    if (GameplayManager.IsMultiplayerActive && NetworkMatch.InGameplay() && __instance == GameManager.m_local_player)
                    {
                        
                        AOSwitchLogic.maybeSwapPrimary();
                        if(swap_failed)
                        {
                            uConsole.Log("-AUTOORDER- [EB] swap failed on trying to switch to an energy weapon");
                            swap_failed = false;
                            return true;
                        }
                        else
                        {
                            uConsole.Log(" - Denied Execution of original Method");
                            return false;
                        }   
                    }
                    return true;
                }
                return true;
            }
        }

      


    }
}

