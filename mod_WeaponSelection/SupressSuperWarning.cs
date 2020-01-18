using System;
using System.Collections.Generic;
using System.IO;
using Harmony;
using Overload;

namespace mod_WeaponSelection
{
    class SupressSuperWarning
    {     

        /* Useless with olmod 2.7.90
        [HarmonyPatch(typeof(Player), "RpcShowWarningMessage")]
        internal class MaybeDenySuperWarning
        {
            private static bool Prefix()
            {
                int num = GameManager.m_level_data.m_item_spawn_points.Length;
                bool is_there_a_super_in_current_level = false;
                for (int i = 0; i < num; i++)
                {
                    if (GameManager.m_level_data.m_item_spawn_points[i].multiplayer_team_association_mask == 1)
                    {
                        is_there_a_super_in_current_level = true;
                        break;
                    }
                }
                if (is_there_a_super_in_current_level) uConsole.Log("Level contains a super spawn, letting RpcShowWarningMessage pass");
                else uConsole.Log("Didnt find any item with super flag, Surpressing RpcShowWarningMessage");
                return is_there_a_super_in_current_level;   
            }
        }*/
    }
}

