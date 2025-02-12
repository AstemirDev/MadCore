namespace MadCore.API.Event
{
    public class EventWorld
    {
        public delegate void WorldLoad(ManagersScript managers);
        
        public delegate void WorldUpdate(float deltaTime);
    }
}