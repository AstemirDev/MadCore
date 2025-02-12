using System.Collections.Generic;
using HarmonyLib;
using MadCore.API.Registry;
using UnityEngine;
using UnityEngine.UI;

namespace MadCore.API.World.Item.Craft
{
    public class CraftRegistry : MadRegistry<MadCraftData>
    {
        public static readonly CraftRegistry Instance = new CraftRegistry();
        public CraftManager CraftManager;

        protected override void OnLoad(ManagersScript managersScript)
        {
            CraftManager = managersScript.craftMN;
            var rebuildList = new List<CraftManager.CraftData>(CraftManager.craftData);
            foreach (var madCraftData in Values)
            {
                var craftId = rebuildList.Count;
                madCraftData.CraftId = craftId;
                var data = madCraftData.BuildData();
                rebuildList.Add(data);
            }
            CraftManager.craftData = rebuildList.ToArray();
        }
        
        [HarmonyPatch(typeof(InventoryManager), nameof(InventoryManager.SubInventoryLoad))]
        [HarmonyPrefix]
        public static bool SubInventoryLoad(InventorySlot chest, ItemData tmpData, bool subInventoryActive, InventoryManager __instance)
        {
            if (tmpData == null) return true;
            var extraData = tmpData.GetComponent<ItemDataAdditional>();
            if (extraData == null) return true;
            var item = ItemRegistry.Instance.GetRegistered(extraData.ItemId);
            var craftData = item.CraftData.Invoke();
            if (craftData == null) return true;
            if (chest.type != InventorySlot.Type.Bench) return true;
            __instance.tmpSubInventory = chest;
            for (var index = 0; index < __instance.subCount; ++index)
            {
                if (!string.IsNullOrEmpty(__instance.itemSlot[50 + index].itemKey))
                {
                    Debug.Log("SubInventory Override " + (50 + index));
                    __instance.SubInventoryUnLoad();
                    break;
                }
            }
            if (subInventoryActive && !__instance.subInventory.activeSelf)
                __instance.SubInventoryVisible();
            for (int index = 0; index < __instance.subCount; ++index)
            {
                if (index < chest.size)
                {
                    __instance.SlotToSlot(chest.slots[index], __instance.itemSlot[50 + index]);
                    __instance.itemSlot[50 + index].gameObject.SetActive(true);
                }
                else
                    __instance.itemSlot[50 + index].gameObject.SetActive(false);
            }
            var component1 = __instance.subInventory.GetComponent<RectTransform>();
            component1.sizeDelta = new Vector2(component1.sizeDelta.x, 42 + 25 * ((chest.size - 1) / 10.0F));
            __instance.subInventory.transform.Find("InventoryName/Text").GetComponent<Text>().text = tmpData.GetItemName();
            __instance.CraftOpen(craftData.CraftId);
            return false;
        }
    }
}