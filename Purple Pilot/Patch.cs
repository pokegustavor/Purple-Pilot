using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using PulsarModLoader;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Linq;
using PulsarModLoader.Patches;
namespace Purple_Pilot
{
    public class Mod : PulsarMod
    {
        public override string Version => "1.3";

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
            __result = ModConfig.getColorForClassID(inID);
        }
    }
    [HarmonyPatch(typeof(PLUIClassSelectionMenu), "Update")]
    class MenuPilotColorChange 
    {
        static void Postfix(PLUIClassSelectionMenu __instance) 
        {
            if (PLGlobal.Instance.ClassColors.Length > 1)
            {
                for (int i = 0; i < 5; i++)
                {
                    PLGlobal.Instance.ClassColors[i] = ModConfig.getColorForClassID(i);
                }
            }
            if (__instance.ClassButtons.Length > 1)
            {
                for(int i = 0; i < 5; i++) 
                {
                    Color color = ModConfig.getColorForClassID(i);
                    ColorBlock colors = __instance.ClassButtons[i].colors;
                    colors.normalColor = color * 0.9f;
                    colors.highlightedColor = color;
                    colors.selectedColor = color;
                    __instance.ClassButtons[i].colors = colors;
                    foreach (Text text in __instance.ClassButtons[i].GetComponentsInChildren<Text>())
                    {
                        text.color = color;
                    }
                }
            }
        }
    }
    [HarmonyPatch(typeof(PLInGameUI),"Update")]
    class HealthBarFix 
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> targetSequence = new List<CodeInstruction>
            {
                new CodeInstruction(OpCodes.Ldelem_Ref),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Color),"get_black")),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(Graphic),"set_color"))
            };
            List<CodeInstruction> patchSequence = new List<CodeInstruction>
            {
                new CodeInstruction(OpCodes.Ldelem_Ref),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Color),"get_white")),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(Graphic),"set_color"))
            };

            return HarmonyHelpers.PatchBySequence(instructions, targetSequence, patchSequence, HarmonyHelpers.PatchMode.REPLACE, HarmonyHelpers.CheckMode.NONNULL, false);
        }

        /*
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions)
        {
            List<CodeInstruction> instructionsList = Instructions.ToList();
            instructionsList[477].operand = AccessTools.Method(typeof(Color),"get_white");
            return instructionsList.AsEnumerable();
        }

        static void Postfix(PLInGameUI __instance) 
        {
            if (__instance.CrewStatusRoot.activeSelf) 
            {
                int num = 0;
                foreach (GameObject gameObject in __instance.CrewStatusSlots) 
                {
                    PLPlayer plplayer2 = null;
                    if (__instance.relevantPlayersForCrewStatus.Count > num)
                    {
                        plplayer2 = __instance.relevantPlayersForCrewStatus[num];
                    }
                    else 
                    {
                        break;
                    }
                    if (plplayer2.GetPawn() != null && Time.time - plplayer2.GetPawn().LastDamageTakenTime < 0.33f && Time.time * 18f % 1f < 0.5f)
                    {
                        __instance.CrewStatusSlots_BGs[num].color = Color.red * 0.8f;
                        __instance.CrewStatusSlots_Fills[num].color = Color.red;
                        __instance.CrewStatusSlots_Names[num].color = Color.red;
                        __instance.CrewStatusSlots_TalkingImages[num].color = Color.red;
                    }
                    else
                    {
                        __instance.CrewStatusSlots_BGs[num].color = PLInGameUI.FromAlpha(PLGlobal.Instance.ClassColors[plplayer2.GetClassID()] * 0.35f, 1f);
                        __instance.CrewStatusSlots_Fills[num].color = PLInGameUI.FromAlpha(PLGlobal.Instance.ClassColors[plplayer2.GetClassID()] * 0.7f, 1f);
                        __instance.CrewStatusSlots_Names[num].color = __instance.CrewStatusSlots_Fills[num].color;
                        __instance.CrewStatusSlots_TalkingImages[num].color = __instance.CrewStatusSlots_Fills[num].color;
                        Color color = PLGlobal.Instance.ClassColors[plplayer2.GetClassID()];
                        if (color.b >= 0.75 && color.g >= 0.75 && color.r >= 0.75)
                        {
                            __instance.CrewStatusSlots_HPs[num].color = Color.black;
                        }
                        else
                        {
                            __instance.CrewStatusSlots_HPs[num].color = Color.white;
                        }
                    }
                    num++;
                }
            }
        }
    }
    [HarmonyPatch(typeof(PLTeleportationScreen), "Update")]
    class IconFix 
    {
        static void Postfix(PLTeleportationScreen __instance,ref UITexture[] ___ClassTargets) 
        {
            if (!__instance.UIIsSetup()) return;
            for(int i = 0; i < 5; i++) 
            {
                ___ClassTargets[i].color = ModConfig.getColorForClassID(i);
            }
        }
    }

    class ModConfig : ModSettingsMenu 
    {
        public static SaveValue<string> captanValue = new SaveValue<string>("CaptanColour", "#0066ffff");
        public static SaveValue<string> pilotValue = new SaveValue<string>("PilotColour", "#6600b2ff");
        public static SaveValue<string> scienceValue = new SaveValue<string>("ScienceColour", "#00ff00ff");
        public static SaveValue<string> weaponsValue = new SaveValue<string>("WeaponsColour", "#ff0000ff");
        public static SaveValue<string> engineValue = new SaveValue<string>("EngineColour", "#ff6600ff");
        public override void Draw()
        {
            GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
            CreateBox("Captain", captanValue);
            CreateBox("Pilot", pilotValue);
            CreateBox("Scientist", scienceValue);
            CreateBox("Weapons", weaponsValue);
            GUILayout.EndHorizontal();
            CreateBox("Engineer", engineValue);
            
        }
        public override string Name()
        {
            return "Purple Pilot Settings";
        }
        public static Color getColorForClassID(int ID) 
        {
            Color color;
            switch (ID) 
            {
                default:
                case 0:
                    ColorUtility.TryParseHtmlString(ModConfig.captanValue.Value, out color);
                    break;
                case 1:
                    ColorUtility.TryParseHtmlString(ModConfig.pilotValue.Value, out color);
                    break;
                case 2:
                    ColorUtility.TryParseHtmlString(ModConfig.scienceValue.Value, out color);
                    break;
                case 3:
                    ColorUtility.TryParseHtmlString(ModConfig.weaponsValue.Value, out color);
                    break;
                case 4:
                    ColorUtility.TryParseHtmlString(ModConfig.engineValue.Value, out color);
                    break;
            }
            return color;
        }
        static void CreateBox(string name, SaveValue<string> saveValue) 
        {
            GUILayout.BeginVertical("Box", new GUILayoutOption[]
        {
            GUILayout.Width(240f),
            GUILayout.Height(160f)
        });
            Color color;
            ColorUtility.TryParseHtmlString(saveValue.Value, out color);
            GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
            GUILayout.Label($"{name} Colour", Array.Empty<GUILayoutOption>());
            GUILayout.Label(saveValue.Value, Array.Empty<GUILayoutOption>());
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
            GUILayout.BeginVertical("Box", Array.Empty<GUILayoutOption>());
            GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
            GUILayout.Label("R", new GUILayoutOption[] { GUILayout.Width(10f) });
            color.r = GUILayout.HorizontalSlider(color.r, 0f, 1f, Array.Empty<GUILayoutOption>());
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
            GUILayout.Label("G", new GUILayoutOption[] { GUILayout.Width(10f) });
            color.g = GUILayout.HorizontalSlider(color.g, 0f, 1f, Array.Empty<GUILayoutOption>());
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
            GUILayout.Label("B", new GUILayoutOption[] { GUILayout.Width(10f) });
            color.b = GUILayout.HorizontalSlider(color.b, 0f, 1f, Array.Empty<GUILayoutOption>());
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
            GUILayout.Label("A", new GUILayoutOption[] { GUILayout.Width(10f) });
            color.a = GUILayout.HorizontalSlider(color.a, 0f, 1f, Array.Empty<GUILayoutOption>());
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.BeginVertical("Box", new GUILayoutOption[]
            {
            GUILayout.Width(44f),
            GUILayout.Height(44f)
            });
            GUI.color = color;
            saveValue.Value = "#" + ColorUtility.ToHtmlStringRGBA(color);
            GUILayout.Label(new Texture2D(60, 40), Array.Empty<GUILayoutOption>());
            GUI.color = Color.white;
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.Label(string.Format("{0},{1},{2},{3}", new object[]
            {
            (int)(color.r * 255f),
            (int)(color.g * 255f),
            (int)(color.b * 255f),
            (int)(color.a * 255f)
            }), Array.Empty<GUILayoutOption>());
            GUILayout.EndVertical();
        }
    }

    
}
