using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace MadCore.API.World.Entity.Patches
{
    public class EntityMaxLifePatches
    {

        public static void PathAll()
        {
            Harmony.CreateAndPatchAll(typeof(EntityMaxLifePatches));
            Harmony.CreateAndPatchAll(typeof(NPCDamagePatch));
            Harmony.CreateAndPatchAll(typeof(CommonLifeChangePatch));
            Harmony.CreateAndPatchAll(typeof(DamageTriggerPatch));
        }
              
        public static double GetMaxHealth(CommonStates commonStates, double maxHealth) {
            return 2000000;
        }
        
        [HarmonyPatch(typeof(GameManager), nameof(GameManager.LifeImageChange))]
        [HarmonyPrefix]
        private static bool LifeImageChange(GameManager __instance)
        {
            __instance.lifeImage.fillAmount = (float) (__instance.playerCommons[GameManager.selectPlayer].life / GetMaxHealth(__instance.playerCommons[GameManager.selectPlayer], __instance.playerCommons[GameManager.selectPlayer].maxLife));
            __instance.lifeImage.transform.parent.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = __instance.playerCommons[GameManager.selectPlayer].life.ToString("0") + "/" + GetMaxHealth(__instance.playerCommons[GameManager.selectPlayer], __instance.playerCommons[GameManager.selectPlayer].maxLife).ToString("0");
            return false;
        }
        
        [HarmonyPatch(typeof(CommonStates))]
        [HarmonyPatch(nameof(CommonStates.NPCDamage))]
        private static class NPCDamagePatch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var instructionsList = instructions.ToList();
                ReplaceMaxLifeThisCommon(instructionsList);
                return instructionsList;
            }
        }
        
        [HarmonyPatch(typeof(CommonStates))]
        [HarmonyPatch(nameof(CommonStates.CommonLifeChange))]
        private static class CommonLifeChangePatch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var instructionsList = instructions.ToList();
                ReplaceMaxLifeThisCommon(instructionsList);
                return instructionsList;
            }
        }

        [HarmonyPatch(typeof(DamageTrigger))]
        [HarmonyPatch("FixedUpdate")]
        private static class DamageTriggerPatch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var instructionsList = instructions.ToList();
                var maxLife = AccessTools.Field(typeof(CommonStates), nameof(CommonStates.maxLife));
                var targets = AccessTools.Field(typeof(DamageTrigger), nameof(DamageTrigger.targets));
                var getItem = AccessTools.Method(typeof(List<CommonStates>), "get_Item", new [] { typeof(int) });
                var index = instructionsList.FindIndex(instruction => instruction.operand == maxLife);
                instructionsList.InsertRange(index-4, new [] {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, targets),
                        new CodeInstruction(OpCodes.Ldloc_0),
                        new CodeInstruction(OpCodes.Callvirt, getItem)
                    }
                );
                instructionsList.Insert(index+5, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EntityMaxLifePatches), nameof(GetMaxHealth), new []{typeof(CommonStates), typeof(double)})));
                return instructionsList;
            }
        }
        
        private static void ReplaceMaxLifeThisCommon(List<CodeInstruction> instructionsList)
        {
            var maxLife = AccessTools.Field(typeof(CommonStates), nameof(CommonStates.maxLife));
            var replaceIndexes = new List<int>();
            var offset = 0;
            for (var i = 0; i < instructionsList.Count; i++)
            {
                if (instructionsList[i].operand == maxLife)
                {
                    replaceIndexes.Add(i);
                }
            }
            foreach (var replaceIndex in replaceIndexes)
            {
                instructionsList.Insert(replaceIndex+offset-1, new CodeInstruction(OpCodes.Ldarg_0));
                instructionsList.Insert(replaceIndex+offset+2, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EntityMaxLifePatches), nameof(GetMaxHealth), new []{typeof(CommonStates), typeof(double)})));
                offset += 2;
            }
        }
    }
}