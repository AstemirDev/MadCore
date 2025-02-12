using MadCore.API.World.Item.Craft;
using MadCore.Example.Item;

namespace MadCore.Example
{
    public class ExampleCraft
    {
        public static MadCraftData Gunbench = CraftRegistry.Instance.Register(MadCore.Id("gunbench"), new MadCraftData(
            ()=>ExampleItems.Rpg
        ));
        
        public static void RegisterAll(){}
    }
}