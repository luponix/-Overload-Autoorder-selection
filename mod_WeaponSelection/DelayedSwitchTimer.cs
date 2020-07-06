using System.Timers;
using Harmony;
using Overload;
using UnityEngine;

namespace mod_WeaponSelection
{
    class DelayedSwitchTimer
    {
        
        Timer timer;


        public DelayedSwitchTimer()
        {}

        public void Awake()
        {
            //Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " Monitor     Awake");
            var ntimer = new Timer(250);
            ntimer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            ntimer.Enabled = true;
            ntimer.AutoReset = false;
            timer = ntimer;
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //uConsole.Log("Delayed Switch Timer Elapsed");
            //AOSwitchLogic.maybeSwapMissiles();

            int weapon_num = AOSwitchLogic.findHighestPrioritizedUseableMissile();


            if (GameManager.m_local_player.m_missile_level[weapon_num] == WeaponUnlock.LOCKED || GameManager.m_local_player.m_missile_ammo[weapon_num] == 0)
            {
                return;
            }
            if (GameManager.m_local_player.m_missile_type != (MissileType)weapon_num)
            {
                GameManager.m_local_player.Networkm_missile_type = (MissileType)weapon_num;
                GameManager.m_local_player.CallCmdSetCurrentMissile(GameManager.m_local_player.m_missile_type);
                //GameManager.m_local_player.c_player_ship.MissileSelectFX();
                GameManager.m_local_player.UpdateCurrentMissileName();
            }



        }
    }
}