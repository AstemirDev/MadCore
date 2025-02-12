using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MadCore.API.Utils;

namespace MadCore.API.Misc
{
    public static class DLLManager
    {
        private static readonly string[] DllNames = {
            "System.Memory",
            "System.Runtime.CompilerServices.Unsafe",
            "NVorbis"
        };

        private static List<string> _existingDlls;
        
        public static void Load()
        {
            _existingDlls = AppDomain.CurrentDomain.GetAssemblies().Select(assembly => assembly.GetName().Name).ToList();
            foreach (var dllName in DllNames) {
                LoadDll(dllName);
            }
        }
        
        public static void LoadDll(string libName)
        {
            if (IsDllLoaded(libName)) return;
            // if (IsManagedAssembly(libName)) return;
            var dllStream = AssetUtils.GetEmbeddedAsset($"Assets/libs/{libName}.dll");
            var bytes = AssetUtils.ReadAllBytes(dllStream);
            Assembly.Load(bytes);
            MadCore.Logger.LogInfo($"Load DLL {libName}.dll");
        }
        
       
        public static bool IsManagedAssembly(string fileName)
        {
            using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            using (var binaryReader = new BinaryReader(fileStream))
            {
                if (fileStream.Length < 64)
                {
                    return false;
                }
                fileStream.Position = 0x3C;
                var peHeaderPointer = binaryReader.ReadUInt32();
                if (peHeaderPointer == 0)
                {
                    peHeaderPointer = 0x80;
                }
                if (peHeaderPointer > fileStream.Length - 256)
                {
                    return false;
                }
                fileStream.Position = peHeaderPointer;
                var peHeaderSignature = binaryReader.ReadUInt32();
                if (peHeaderSignature != 0x00004550)
                {
                    return false;
                }
                fileStream.Position += 20;
                const ushort PE32 = 0x10b;
                const ushort PE32Plus = 0x20b;
                var peFormat = binaryReader.ReadUInt16();
                if (peFormat != PE32 && peFormat != PE32Plus)
                {
                    return false;
                }
                var dataDictionaryStart = (ushort)(peHeaderPointer + (peFormat == PE32 ? 232 : 248));
                fileStream.Position = dataDictionaryStart;
                var cliHeaderRva = binaryReader.ReadUInt32();
                return cliHeaderRva != 0;
            }
        }

        
        
        public static bool IsDllLoaded(string libName)
        {
            return _existingDlls.Contains(libName);
        }
    }
}