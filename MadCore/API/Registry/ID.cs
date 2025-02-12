using System;

namespace MadCore.API.Registry
{
    public class ID
    {
        public static readonly ID Null = new ID("", "");
        
        public readonly string Name;
        
        public readonly string Path;

        public ID(string name, string path)
        {
            Name = name;
            Path = path;
        }
        
        public bool IsValid(){return !Equals(Null);}

        protected bool Equals(ID other)
        {
            return Name == other.Name && Path == other.Path;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((ID)obj);
        }

        public override string ToString() {
            if (!IsValid())
            {
                return "null";
            }
            if (Name.ToLower().Equals(MadIsland.Namespace.ToLower()))
            {
                return Path;
            }
            return Name+":"+Path;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Path);
        }

        public static ID Of(string name, string path)
        {
            return new ID(name, path);
        }

        public static ID Of(string fullPath)
        {
            if (fullPath == null)
            {
                return Null;
            }
            if (string.IsNullOrEmpty(fullPath))
            {
                return Null;
            }
            if (!fullPath.Contains(":"))
            {
                return new ID(MadIsland.Namespace, fullPath);
            }
            var separatorIndex = fullPath.IndexOf(':');
            return new ID(fullPath.Substring(0, separatorIndex), fullPath.Substring(separatorIndex+1));
        }
    }
}