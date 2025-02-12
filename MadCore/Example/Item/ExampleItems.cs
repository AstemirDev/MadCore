using MadCore.API.Misc;
using MadCore.API.World.Item;
using UnityEngine;

namespace MadCore.Example.Item
{
    public static class ExampleItems
    {
        public static readonly MadItem Rpg = ItemRegistry.Instance.Register(MadCore.Id("rpg"), new ItemRpg());

        public static readonly MadItem Gunbench = ItemRegistry.Instance.Register(MadCore.Id("gunbench"), new MadItem()
        {
            PlacePrefab = ExampleInit.AssetBundle.LoadAsset<GameObject>("assets/modassets/building/Gunbench.prefab"),
            Type = ItemData.ItemType.Prop,
            ToolType = ItemData.ToolType.None,
            SubType = ItemData.SubType.Chest,
            Name = LangString.Text("<color=green>Gun Workbench</color>"),
            Lore = LangString.Text("Used to craft Rpg"),
            CraftData = ()=>ExampleCraft.Gunbench,
            Durability = 800
        });
        
        public static void RegisterAll(){}
    }
}