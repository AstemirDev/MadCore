using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using MadCore.API.Registry;
using UnityEngine;

namespace MadCore.API.World.Entity
{
    public class EntityRegistry : MadRegistry<MadEntity>
    {
        public static readonly EntityRegistry Instance = new EntityRegistry();
        
        public NPCManager NpcManager;

        protected override void OnLoad(ManagersScript managersScript)
        {
            NpcManager = managersScript.npcMN;
        }
        
        [HarmonyPatch(typeof(CommonStates), nameof(CommonStates.MoveSpeed))]
        [HarmonyPrefix]
        private static bool MoveSpeed(bool limit, CommonStates __instance, ref float __result)
        {
            __result = __instance.GetComponent<PlayerExtension>().GetMaxSpeed(limit, true);
            return false;
        }
        
        [HarmonyPatch(typeof(CommonStates), nameof(CommonStates.MoveSpeedNPC))]
        [HarmonyPrefix]
        private static bool MoveSpeedNPC(CommonStates __instance, ref float __result)
        {
            var npcExtension = NPCExtension.GetOrCreate(__instance);
            __result = npcExtension.GetMaxSpeed();
            return false;
        }
    }
}