namespace MadCore.API.World.Entity
{
    public class PlayerExtension : NPCExtension
    {
        public static void Inject()
        {
            foreach (var player in MadIsland.Players)
            {
                var madPlayer = player.gameObject.AddComponent<PlayerExtension>();
                madPlayer.CommonState = player;
            }
        }

        public override bool IsPlayer()
        {
            return true;
        }
    }
}