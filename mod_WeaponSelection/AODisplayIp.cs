using Harmony;
using Overload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace mod_WeaponSelection
{
    class AODisplayIp
    {
        /*
         * 
         * Not needed anymore
         * 
         * 
        public static string pw = "";

        [HarmonyPatch(typeof(Client), "Connect")]
        internal class OnConnectLogIP
        {
            private static void Postfix(string ip, int port)
            {
                pw = ip + ":" + port;
            }
        }

        [HarmonyPatch(typeof(UIElement), "DrawMpPreMatchMenu")]
        internal class DrawIP
        {
            private static void Postfix(UIElement __instance)
            {    
                Vector2 pos = default(Vector2);
                pos.x = 538.246f;
                pos.y = -161.9803f;
                //uConsole.Log("x =" + pos.x + " y= " + pos.y);
                __instance.DrawStringSmall(pw, pos, 0.5f, StringOffset.CENTER, UIManager.m_col_damage, pos.x / 64, -1f);
            }
        }
        */
    }
}
