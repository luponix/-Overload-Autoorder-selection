using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Overload;
using Harmony;

namespace mod_WeaponSelection
{
    class IdioticExperiments
    {
    }


    [HarmonyPatch(typeof(GameManager), "Start")]
    internal class ConsolePatch3
    {

        private static void Postfix(GameManager __instance)
        {
            uConsole.RegisterCommand("spawnrobot_10", "", new uConsole.DebugCommand(ConsolePatch3.Cmdspawnrobot));
        }

        public static void Cmdspawnrobot()
        {
            for( int i = 0; i < 10; i++ )
            {
                ChallengeManager.SpawnRobot();
            }
            
        }
    }
}
