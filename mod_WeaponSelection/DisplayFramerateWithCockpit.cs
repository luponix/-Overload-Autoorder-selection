using Harmony;
using Overload;
using UnityEngine;

namespace mod_WeaponSelection
{
    class DisplayFramerateWithCockpit
    {
        /*
        [HarmonyPatch(typeof(UIElement), "DrawHUD")]
        internal class DisplayFramerate
        {
            public static void Postfix( UIElement __instance)
            {
                if (GameManager.m_display_fps)
                {
                    Vector2 vector;
                    vector.x = 0f;
                    vector.y = UIManager.UI_TOP - 188f;
                    UIElement.average_fps = UIElement.average_fps * 0.95f + 1f / RUtility.FRAMETIME_UI * 0.05f;
                    __instance.DrawStringSmall("FPS: " + ((int)UIElement.average_fps).ToString(), vector, 0.5f, StringOffset.CENTER, UIManager.m_col_ui4, 1f, -1f);
                }
            }
        }*/
    }
}

