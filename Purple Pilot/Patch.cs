using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using PulsarModLoader;
namespace Purple_Pilot
{
    public class Mod : PulsarMod
    {
        public override string Version => "1.0";

        public override string Author => "pokegustavo";

        public override string ShortDescription => "Makes most pilot stuff purple";

        public override string Name => "Purple Pilot";

        public override string HarmonyIdentifier()
        {
            return "pokegustavo.purplepilot";
        }
    }
    [HarmonyPatch(typeof(PLPlayer), "GetClassColorFromID")]
    class PilotColorChanger
    {
        static void Postfix(ref Color __result, int inID)
        {
            if (inID == 1)
            {
                __result = new Color(0.41f, 0f, 0.58f, 0.93f);
            }
        }
    }
    [HarmonyPatch(typeof(PLUIClassSelectionMenu), "Update")]
    class MenuPilotColorChange 
    {
        static void Postfix(PLUIClassSelectionMenu __instance) 
        {
            if (PLGlobal.Instance.ClassColors.Length > 1) PLGlobal.Instance.ClassColors[1] = new Color(0.41f, 0f, 0.58f, 0.93f);
            if (__instance.ClassButtons.Length > 1)
            {
                ColorBlock colors = __instance.ClassButtons[1].colors;
                colors.normalColor = new Color(0.41f, 0f, 0.58f, 0.93f) * 0.9f;
                colors.highlightedColor = new Color(0.41f, 0f, 0.58f, 0.93f);
                colors.selectedColor = new Color(0.41f, 0f, 0.58f, 0.93f);
                __instance.ClassButtons[1].colors = colors;
                foreach (Text text in __instance.ClassButtons[1].GetComponentsInChildren<Text>())
                {
                    text.color = new Color(0.41f, 0f, 0.58f, 0.93f);
                }
            }
        }
    }
}
