using MadCore.API;
using MadCore.API.World.Command;
using MadCore.API.World.Entity;

namespace MadCore.Example
{
    public static class ExampleCommands
    {
        
        
        public static MadCommand God = CommandRegistry.Instance.Register(MadCore.Id("god"), new MadCommand("/god",
            args =>
            {
                MadIsland.ActivePlayer.GetComponent<PlayerExtension>().ToggleGod();
            })
        );
        
        public static void RegisterAll(){}
    }
}