using HarmonyLib;
using MadCore.API.Event;
using MadCore.API.Utils;
using UnityEngine;

namespace MadCore.API.Gui
{
    public static class MadTitleScreen
    {
        public static event EventTitleScreen.Load OnLoad;
        
        public static void Load()
        {
            OnLoad?.DynamicInvoke(GameUtils.FindInRoot("TitleCanvas").GetComponent<Canvas>());
        }
        
        [HarmonyPatch(typeof(TitleScript), "Start")]
        [HarmonyPostfix]
        private static void TitleStart()
        {
            Load();
        }
    }
}