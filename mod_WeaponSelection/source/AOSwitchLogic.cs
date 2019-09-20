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

        /////////////////////////////////////////////////////////////////////////////////////
        //              PUBLIC VARIABLES                  
        /////////////////////////////////////////////////////////////////////////////////////


        public static String[] PrimaryPriorityArray = new String[8];
        public static String[] SecondaryPriorityArray = new String[8];

        public static Player playerObject = null;
        public static string textFile = Path.Combine(Application.persistentDataPath, "Weapon-Priority-List.txt");

        public static bool initialised = false; // should get set back to false by the exit weapon screen list

        public static string[] EnergyWeapons = { "IMPULSE", "CYCLONE", "REFLEX", "THUNDERBOLT", "LANCER", };
        public static string[] AmmoWeapons = { "CRUSHER", "FLAK", "DRILLER" };

        public static bool isManuallySwapping = false;
        public static int currentMissile = -1;
        public static bool negativeAmmo = false;

        public static bool swapThisGoddamnMissile = false;

        //public static int[] missile_ammo = new int[8];
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
        }

        private static void createDefaultPriorityFile()
        {
            using (StreamWriter sw = File.CreateText(textFile))
            {
                sw.WriteLine("THUNDERBOLT");
                sw.WriteLine("CYCLONE");
                sw.WriteLine("DRILLER");
                sw.WriteLine("REFLEX");
                sw.WriteLine("FLAK");
                sw.WriteLine("SHOTGUN");
                sw.WriteLine("LANCER");
                sw.WriteLine("IMPULSE");
                sw.WriteLine("DEVASTATOR");
                sw.WriteLine("NOVA");
                sw.WriteLine("TIMEBOMB");
                sw.WriteLine("VORTEX");
                sw.WriteLine("HUNTER");
                sw.WriteLine("FALCON");
                sw.WriteLine("CREEPER");
                sw.WriteLine("MISSILE_POD");
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

                    if (counter < 8)
                    {
                        if (ln == "THUNDERBOLT" | ln == "IMPULSE" | ln == "CYCLONE" | ln == "DRILLER" | ln == "LANCER" | ln == "REFLEX" | ln == "FLAK" | ln == "SHOTGUN")
                        {
                            PrimaryPriorityArray[counter] = ln;
                        }
                        else
                        {
                            uConsole.Log("ERROR(1) while reading File, unexpected line content : " + ln);
                            Debug.Log("[WPS] ERROR(1) unexpected line content -> (content: " + ln + " )");

                            return;
                        }

                    }
                    else if (counter < 16)
                    {
                        if (ln == "DEVASTATOR" | ln == "TIMEBOMB" | ln == "VORTEX" | ln == "NOVA" | ln == "HUNTER" | ln == "FALCON" | ln == "CREEPER" | ln == "MISSILE_POD")
                        {
                            SecondaryPriorityArray[counter - 8] = ln;
                        }
                        else
                        {
                            uConsole.Log("ERROR(2) while reading File, unexpected line content : " + ln);
                            Debug.Log("[WPS] ERROR(2) unexpected line content -> (content: " + ln + " )");

                            return;
                        }
                    }
                    else
                    {
                        // uConsole.Log("ERROR(3) while reading File, unexpected line content : " + ln);
                        // Debug.Log("[WPS] ERROR(3) unexpected line content -> (content: " + ln + " )");

                        return;
                    }
                    counter++;
                }
                file.Close();

            }
        }



        /////////////////////////////////////////////////////////////////////////////////////
        //              PRIMARY WEAPONS                    
        /////////////////////////////////////////////////////////////////////////////////////
        ///Primary Weapon Selection works as intended

        public static void maybeTryToSwapWeapons()
        {
            if (GameManager.m_local_player.m_energy > 0 && !(GameManager.m_local_player.m_ammo > 0))
            {
                swapToWeapon(findHighestPrioritizedUseableWeapon(EnergyWeapons));
                return;
            }
            if (!(GameManager.m_local_player.m_energy > 0) && GameManager.m_local_player.m_ammo > 0)
            {
                swapToWeapon(findHighestPrioritizedUseableWeapon(AmmoWeapons));
                return;
            }
            swapToWeapon(findHighestPrioritizedUseableWeapon(PrimaryPriorityArray));
        }

        private static string findHighestPrioritizedUseableWeapon(string[] array)
        {
            foreach (string priorityWeapon in PrimaryPriorityArray)
            {
                if (isWeaponAccessible(priorityWeapon))
                {
                    foreach (string maybeRestrictedWeaponPoolWeapon in array)
                    {
                        if (priorityWeapon.Equals(maybeRestrictedWeaponPoolWeapon))
                        {
                            return priorityWeapon;
                        }
                    }
                }
            }
            return "";
        }

        private static void swapToWeapon(string weaponName)
        {
           // uConsole.Log("swapping to " + weaponName);
            if (!(GameManager.m_local_player.m_weapon_type.Equals(stringToWeaponType(weaponName))))
            {
                GameManager.m_local_player.Networkm_weapon_type = stringToWeaponType(weaponName);
                GameManager.m_local_player.CallCmdSetCurrentWeapon(GameManager.m_local_player.m_weapon_type);
                GameManager.m_player_ship.WeaponSelectFX();
            }
            SFXCueManager.PlayRawSoundEffect2D(SoundEffect.hud_notify_message1, 1f, 0.15f, 0.1f, false);
        }

        public static WeaponType stringToWeaponType(string weapon)
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

        public static bool isWeaponAccessible(string weapon)
        {
            if (weapon.Equals("IMPULSE")) return !(GameManager.m_local_player.m_weapon_level[0].ToString().Equals("LOCKED"));
            if (weapon.Equals("CYCLONE")) return !(GameManager.m_local_player.m_weapon_level[1].ToString().Equals("LOCKED"));
            if (weapon.Equals("REFLEX")) return !(GameManager.m_local_player.m_weapon_level[2].ToString().Equals("LOCKED"));
            if (weapon.Equals("CRUSHER")) return !(GameManager.m_local_player.m_weapon_level[3].ToString().Equals("LOCKED"));
            if (weapon.Equals("DRILLER")) return !(GameManager.m_local_player.m_weapon_level[4].ToString().Equals("LOCKED"));
            if (weapon.Equals("FLAK")) return !(GameManager.m_local_player.m_weapon_level[5].ToString().Equals("LOCKED"));
            if (weapon.Equals("THUNDERBOLT")) return !(GameManager.m_local_player.m_weapon_level[6].ToString().Equals("LOCKED"));
            if (weapon.Equals("LANCER")) return !(GameManager.m_local_player.m_weapon_level[7].ToString().Equals("LOCKED"));
            else
            {
                return false;
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

        public static void maybeSwapMissiles()
        {
            int highestMissile = findHighestPrioritizedUseableMissile();
            if (highestMissile == -1)
            {
                uConsole.Log("findHighestPrioritizedUseableMissile returned no possible missile, if this shit actually happens, ping me(luponix) on discord");

            }
            else
            {

              //  uConsole.Log("want to swap: highest missile is :" + highestMissile);
                swapMissile(highestMissile);

            }

        }

       

        public static int findHighestPrioritizedUseableMissile()
        {
            foreach (string missile in SecondaryPriorityArray)
            {
                int var = missileStringToInt(missile);
                /*if ( var == current_missile && missile_ammo[var] > 0)
                {
                    return var;
                }*/
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
                /*if ( var == current_missile && missile_ammo[var] > 0)
                {
                    return var;
                }*/
                if (GameManager.m_local_player.m_missile_ammo[var] > 0 && var != currentMissile)
                {
                    return var;
                }

            }
            return -1;
        }



        private static void swapMissile(int weapon_num)
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
                GameManager.m_local_player.m_missile_type = GameManager.m_local_player.Networkm_missile_type;
                GameManager.m_player_ship.MissileSelectFX();
                GameManager.m_local_player.UpdateCurrentMissileName();
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


        //Experiment for triggering swap to highest missile on empty missile
        // this method doesnt work
        // Conclusion: the coding is annoying, server probably expects me to change to a different weapon
        /*[HarmonyPatch(typeof(Player), "SwitchToNextMissileWithAmmo")]
        internal class OnEmptyMissile0
        {
            public static bool Prefix(Player __instance)
            {
                if (MenuManager.opt_primary_autoswitch == 0)
                {
                    if (GameplayManager.IsMultiplayerActive && NetworkMatch.InGameplay() && __instance == GameManager.m_local_player)
                    {
                        if (__instance.NumUnlockedMissilesWithAmmo() > 1 || (__instance.NumUnlockedMissilesWithAmmo() == 1 && __instance.m_missile_ammo[(int)__instance.m_missile_type] == 0))
                        {
                            maybeSwapMissiles();
                            __instance.Networkm_missile_type = __instance.m_missile_type;
                            __instance.c_player_ship.MissileSelectFX();
                            __instance.UpdateCurrentMissileName();
                            
                            __instance.m_missile_type_prev = IntToMissileType(findHighestPrevMissile());
                            uConsole.Log("UPDATE (´missile empty): "+__instance.m_missile_type_prev.ToString());
                            return false;
                        }
                        return true;
                    }
                    return true;
                }
                return true;
            }
        }*/
        
        /*[HarmonyPatch(typeof(Player), "FindBestPrevMissile")]
        internal class OnEmptyMissile
        {
            public static void Postfix(Player __instance)
            {
                if (MenuManager.opt_primary_autoswitch == 0)
                {
                    if (GameplayManager.IsMultiplayerActive && NetworkMatch.InGameplay() && __instance == GameManager.m_local_player)
                    {
                        swapThisGoddamnMissile = true;
                       
                    }
                    
                }
                
            }
        }

        [HarmonyPatch(typeof(PlayerShip), "MaybeFireMissile")]
        internal class OnEmptyMissile2
        {
            public static void Postfix()
            {
                if (swapThisGoddamnMissile)
                {
                    if (MenuManager.opt_primary_autoswitch == 0)
                    {
                        if (GameplayManager.IsMultiplayerActive && NetworkMatch.InGameplay() )
                        {
                            maybeSwapMissiles();
                            uConsole.Log("WE SWAP THIS GODDAMN MISSILE NOW");
                            swapThisGoddamnMissile = false;
                        }

                    }
                }
            }
        }*/


        //Weapon switch sound herausschreiben
        
            // 1.
            
        [HarmonyPatch(typeof(Player), "UnlockWeaponClient")]
        internal class WeaponPickup
        {
            public static void Postfix(WeaponType wt, bool silent, Player __instance)
            {
                if (MenuManager.opt_primary_autoswitch == 0)
                {
                    if (GameplayManager.IsMultiplayerActive && NetworkMatch.InGameplay() && __instance == GameManager.m_local_player)
                    {
                        
                        if (!(wt.Equals(__instance.m_weapon_type)))
                        {
                           AOSwitchLogic.maybeTryToSwapWeapons();
                        }
                    }
                }
            }
        }
        
        // 2.
        [HarmonyPatch(typeof(Player), "RpcSetMissileAmmo")]
        internal class SecondaryPickup
        {//if auto select option on "never" . check needs to be added later
            public static void Postfix(int missile_type, int ammo, Player __instance)
            {
                if (MenuManager.opt_primary_autoswitch == 0)
                {
                    if (GameplayManager.IsMultiplayerActive && NetworkMatch.InGameplay() && __instance == GameManager.m_local_player)
                    {   
                        maybeSwapMissiles();
                        //__instance.m_missile_type_prev = IntToMissileType(findHighestPrevMissile());
                        //uConsole.Log("UPDATE (´missile pickup): "+__instance.m_missile_type_prev.ToString());
                    }
                }
            }
        }
        /*
        [HarmonyPatch(typeof(PlayerShip), "MaybeFireMissile")]
        internal class OnMissileFire
        {
            public static void Postfix()
            {
                if (MenuManager.opt_primary_autoswitch == 0)
                {
                    if (GameplayManager.IsMultiplayerActive && NetworkMatch.InGameplay())
                    {
                        GameManager.m_local_player.m_missile_type_prev = IntToMissileType(findHighestPrevMissile());
                    }
                }
            }
        }*/

        [HarmonyPatch(typeof(UIElement), "DrawMpOverlayLoadout")]
        internal class OnRespawnCheckForHunterInCurrentLoadout
        {
            public static void Postfix()
            {
                if (MenuManager.opt_primary_autoswitch == 0)
                {
                    if (GameplayManager.IsMultiplayerActive && NetworkMatch.InGameplay())
                    {
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
        }
        
        /*
        [HarmonyPatch(typeof(Player), "SwitchToNextMissileWithAmmo")]
        internal class OutOfMissiles
        {
            public static bool Prefix(Player __instance)
            {
                bool flag = true;
                if (MenuManager.opt_primary_autoswitch == 0 && GameplayManager.IsMultiplayerActive && NetworkMatch.InGameplay() && __instance == GameManager.m_local_player && flag) 
                {
                    maybeSwapMissiles();
                    return false;
                    //__instance.CallCmdSetCurrentMissile(__instance.m_missile_type);
                }
                return true;
            }
        }
        */


        // try intercepting CallCMDSetMissile at some point
        // or check for what happens in the methods before oldmissile -> missile -> missile_prev
        /*[HarmonyPatch(typeof(Player), "CallCmdSetCurrentMissile")]
        internal class OutOfMissiles
        {

            public bool Prefix(Player __instance)
            {
                if (!isManuallySwapping)
                {
                    if (MenuManager.opt_primary_autoswitch == 0 && GameplayManager.IsMultiplayerActive && NetworkMatch.InGameplay() && __instance == GameManager.m_local_player)
                    {
                        if (!NetworkClient.active)
                        {
                            Debug.LogError("Command function CmdSetCurrentMissile called on server.");
                            return;
                        }
                        if (base.isServer)
                        {
                            this.CmdSetCurrentMissile(missile_type);
                            return;
                        }
                        NetworkWriter networkWriter = new NetworkWriter();
                        networkWriter.Write(0);
                        networkWriter.Write((short)((ushort)5));
                        networkWriter.WritePackedUInt32((uint)Player.kCmdCmdSetCurrentMissile);
                        networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
                        networkWriter.Write((int)missile_type);
                        base.SendCommandInternal(networkWriter, 0, "CmdSetCurrentMissile");


                    }
                }
                else return true;

            }
        }
        */
        /*
                        if (__instance.m_missile_ammo[missileTypeToInt(__instance.m_missile_type)] == 0)
                        {
                            int var = findHighestPrioritizedUseableMissile();
                            int weapon_num = 0;
                            if (var != -1)
                            {
                                weapon_num = var;
                            }

                            if (__instance.m_missile_level[weapon_num] == WeaponUnlock.LOCKED || __instance.m_missile_ammo[weapon_num] == 0)
                            {
                                return false;
                            }
                            if (__instance.m_missile_type != (MissileType)weapon_num)
                            {
                                __instance.Networkm_missile_type = (MissileType)weapon_num;
                                __instance.CallCmdSetCurrentMissile(__instance.m_missile_type);
                                GameManager.m_player_ship.MissileSelectFX();
                                __instance.UpdateCurrentMissileName();
                            }
                            return false;
                        }*/

        //3.
        //lets test wether this can work
        // add a lot of tracker uconsole logs to suspicious methods to figure out what the best call would be starting with maybe fire missile 
        /*[HarmonyPatch(typeof(Player), "GetNextMissileWithAmmo")]
        internal class OutOfMissiles
        {
            public static bool Prefix(MissileType mt, Player __instance, ref MissileType __result)
            {
                if (MenuManager.opt_primary_autoswitch == 0 && GameplayManager.IsMultiplayerActive && NetworkMatch.InGameplay() && __instance == GameManager.m_local_player)
                {

                    int var = findHighestPrioritizedUseableMissile();
                    if (var != -1)
                    {
                        uConsole.Log("GetNextMissileWithAmmo successful IF CASE : "+ IntToMissileType(var));
                        __result = IntToMissileType(var);
                        return false;
                    }
                    uConsole.Log(" [WSM] <GetNextMissileWithAmmo> findHighestPrioritizedUseableMissile returned -1");
                    Debug.Log(" [WSM] <GetNextMissileWithAmmo> findHighestPrioritizedUseableMissile returned -1");
                    return true;

                }
                else
                {
                    uConsole.Log(" [WSM] <GetNextMissileWithAmmo> ELSE CASE");
                    return true;
                }
            }
        }
        */


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

        [HarmonyPatch(typeof(Player), "SwitchToAmmoWeapon")]
        internal class OutOfAmmo
        {
            private static bool Prefix(Player __instance)
            {
                if (MenuManager.opt_primary_autoswitch == 0)
                {
                    if (GameplayManager.IsMultiplayerActive && NetworkMatch.InGameplay() && __instance == GameManager.m_local_player)
                    {            
                        AOSwitchLogic.maybeTryToSwapWeapons();
                        return false;
                    }
                    return false;
                }
                return false;
            }
        }

        //5. NEEEDS TO BE TESTED AGAIN
        [HarmonyPatch(typeof(Player), "SwitchToEnergyWeapon")]
        internal class OutOfEnergy
        {
            private static bool Prefix(Player __instance)
            {
                if (MenuManager.opt_primary_autoswitch == 0)
                {
                    if (GameplayManager.IsMultiplayerActive && NetworkMatch.InGameplay() && __instance == GameManager.m_local_player)
                    {
                        
                        AOSwitchLogic.maybeTryToSwapWeapons();
                        return false;
                    }
                    return false;
                }
                return false;
            }
        }

        /*
       [HarmonyPatch(typeof(Player), "MaybeSwitchToNextMissile")]
       internal class ShutDownAutomaticMissileSwitching
       {

           private static bool Prefix()
           {
               uConsole.Log("WANT TO MAYBE SWITCH");
               if (MenuManager.opt_primary_autoswitch == 0)
               { 
                   if( GameplayManager.IsMultiplayerActive && NetworkMatch.InGameplay() )
                   {
                       uConsole.Log("DENIED THE MAYBE SWITCH");
                       return false;
                   }

                   return true;
               }
               return true;
           }
       }

       [HarmonyPatch(typeof(Player), "SwitchToNextMissileWithAmmo")]
       internal class ShutDownAutomaticMissileSwitching2
       {

           private static bool Prefix()
           {
               uConsole.Log("WANT TO SWITCH TO NEXT MISSILE WITH AMMO");
               if (MenuManager.opt_primary_autoswitch == 0)
               {
                   if (GameplayManager.IsMultiplayerActive && NetworkMatch.InGameplay())
                   {
                       uConsole.Log("DENIED SWITCH TO NEXT MISSILE WITH AMMO");
                       return false;
                   }

                   return true;
               }
               return true;
           }
       }
       */

        //next idea is to disable both methods but to ask for user input and also actualise the weapon selected


    }
}

