using MadCore.API.Registry;

namespace MadCore.API.World.Entity
{
    public class MadEntity : IDHolder
    {
        private ID _id;
        
        public void SetID(ID id)
        {
            _id = id;
        }

        public ID GetID()
        {
            return _id;
        }
    }
}