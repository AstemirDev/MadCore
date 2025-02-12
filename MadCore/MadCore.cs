using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MadCore.API;
using MadCore.API.Gui;
using MadCore.API.Misc;
using MadCore.API.Registry;
using MadCore.API.World.Command;
using MadCore.API.World.Entity;
using MadCore.API.World.Entity.Patches;
using MadCore.API.World.FX;
using MadCore.API.World.Item;
using MadCore.API.World.Item.Craft;
using MadCore.API.World.Sound;
using MadCore.Example;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace MadCore
{
    [BepInPlugin(Guid, Name, Version)]
    [BepInProcess(MadIsland.ExecutableName)]
    public class MadCore : BaseUnityPlugin
    {
        public const string Guid = "MadCore";
        public const string Name = "MadCore";
        public const string Version = "1.0.0";
        internal new static ManualLogSource Logger;
        
        private void Awake()
        {
            Logger = base.Logger;
            DLLManager.Load();
            ExampleInit.Load();
            Harmony.CreateAndPatchAll(typeof(MadCore));
            Harmony.CreateAndPatchAll(typeof(MadTitleScreen));
            Harmony.CreateAndPatchAll(typeof(ItemRegistry));
            Harmony.CreateAndPatchAll(typeof(EntityRegistry));
            Harmony.CreateAndPatchAll(typeof(CraftRegistry));
            Harmony.CreateAndPatchAll(typeof(CommandRegistry));
            // EntityMaxLifePatches.PathAll();
            MadTitleScreen.Load();
            Logger.LogInfo("Initialization complete.");
        }
        
        private void OnDestroy()
        {
            Harmony.UnpatchAll();
        }
        
        [HarmonyPatch(typeof(GameManager), "Start")]
        [HarmonyPostfix]
        private static void Start()
        {
            var managers = GameObject.Find("Managers").GetComponent<ManagersScript>();
            FXRegistry.Instance.Load(managers);
            SoundRegistry.Instance.Load(managers);
            CommandRegistry.Instance.Load(managers);
            ItemRegistry.Instance.Load(managers);
            CraftRegistry.Instance.Load(managers);
            EntityRegistry.Instance.Load(managers);
            MadIsland.WorldStart(managers);
        }

        [HarmonyPatch(typeof(GameManager), "Update")]
        [HarmonyPostfix]
        private static void Update()
        {
            MadIsland.WorldUpdate(Time.deltaTime);
        }
        
        public static ID Id(string path)
        {
            return ID.Of(Name, path);
        }
    }   
}