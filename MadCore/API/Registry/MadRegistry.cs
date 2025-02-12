using System;
using System.Collections.Generic;
using System.Linq;
using MadCore.API.Misc;

namespace MadCore.API.Registry
{
    
    public abstract class MadRegistry<T> where T : class, IDHolder
    {
        
        public readonly Dictionary<ID, T> Entries = new Dictionary<ID, T>();

        public void Load(ManagersScript managersScript)
        {
            OnLoad(managersScript);
        }

        public T Register(ID id, T entry, bool overwrite = false)
        {
            entry.SetID(id);
            return Register(entry, overwrite);
        }

        private T Register(T entry, bool overwrite = false)
        {
            if (!Entries.ContainsKey(entry.GetID()) || overwrite)
            {
                Entries.Add(entry.GetID(), entry);
            }
            return entry;
        }

        public bool HasRegistered(ID id)
        {
            return Entries.ContainsKey(id);
        }
        
        public Optional<T> GetRegisteredSafe(ID id)
        {
            return Optional<T>.Of(GetRegistered(id));
        }
        
        public T GetRegistered(ID id)
        {
            T entry;
            return Entries.TryGetValue(id, out entry) ? entry : GetNull();
        }

        public virtual T GetNull()
        {
            return null;
        }

        public T[] Values => Entries.Values.ToArray();
        
        protected virtual void OnLoad(ManagersScript managersScript){}
    }
}