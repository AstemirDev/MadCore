using System.Collections;
using HarmonyLib;
using MadCore.API.Misc;
using MadCore.API.Registry;
using MadCore.API.Utils;
using MadCore.API.World.Entity;
using UnityEngine;

namespace MadCore.API.World.Item
{
    public class ItemRegistry : MadRegistry<MadItem>
    {
        public static readonly ItemRegistry Instance = new ItemRegistry();
        
        public static readonly MadItem NullItem = Instance.Register(MadCore.Id("null"), new MadItem());

        public ItemManager ItemManager;

        protected override void OnLoad(ManagersScript managersScript)
        {
            ItemManager = managersScript.itemMN;
            foreach (var madItem in Values)
            {
                MadCore.Logger.LogInfo("Building Item Data "+madItem);
                madItem.BuildData();
            }
        }
        

        [HarmonyPatch(typeof(PlayerMove), "HeavyAttack")]
        [HarmonyPrefix]
        private static bool HeavyAttack(PlayerMove __instance, ref IEnumerator __result)
        {
            var id = ID.Of(__instance.common.equip[1].itemKey);
            if (!id.IsValid()) return true;
            if (!Instance.HasRegistered(id)) return true;
            var item = Instance.GetRegistered(id);
            if (item.IsNull) return true;
            __result = PlayerUtils.HeavyAttack(item, __instance);
            return false;
        }
        
        [HarmonyPatch(typeof(RandomCharacter), "Equip")]
        [HarmonyPrefix]
        private static bool Equip(CommonStates common, ItemSlot tmpSlot, RandomCharacter __instance)
        {
            var id = ID.Of(tmpSlot.itemKey);
            if (!id.IsValid()) { return true; }
            if (!Instance.HasRegistered(id)){ return true; }
            var item = Instance.GetRegistered(id);
            item.Attachment?.AttachToSkeleton(common.anim.skeleton, (NPCId)common.npcID);
            __instance.SlotColorChange(common, new []{ item.Attachment?.SlotName}, tmpSlot.itemColor, tmpSlot.hsb);
            return false;
        }
        
        [HarmonyPatch(typeof(ItemManager), "FindItem")]
        [HarmonyPrefix]
        private static bool StartFindItem(string itemKey, ref ItemData __result)
        {
            var id = ID.Of(itemKey);
            if (!id.IsValid()) { return true; }
            if (!Instance.HasRegistered(id)) { return true; }
            __result = Instance.GetRegistered(id).ItemData;
            return false;
        }
        
        [HarmonyPatch(typeof(ItemManager), "FindItem")]
        [HarmonyPostfix]
        private static void EndFindItem(string itemKey, ref ItemData __result)
        {
            if (__result == null)
            {
                __result = NullItem.ItemData;
            }
        }
        
        [HarmonyPatch(typeof(ItemManager), "GetItem")]
        [HarmonyPrefix]
        private static bool StartFindItem(ItemData tmpItem, int count)
        {
            if (tmpItem != NullItem.ItemData) return true;
            Debug.LogError("item not found");
            return false;
        }
        
        public override MadItem GetNull() {
            return NullItem;
        }
    }
}