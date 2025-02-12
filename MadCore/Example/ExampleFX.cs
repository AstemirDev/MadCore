using MadCore.API.World.FX;
using UnityEngine;

namespace MadCore.Example
{
    public static class ExampleFX
    {
        public static MadFX SemenFX = FXRegistry.Instance.Register(MadCore.Id("semen"), new MadFX(()=>ExampleInit.AssetBundle.LoadAsset<GameObject>("assets/modassets/fx/FX_Semen.prefab")));
        public static MadFX NuclearFX = FXRegistry.Instance.Register(MadCore.Id("nuclear"), new MadFX(()=>ExampleInit.AssetBundle.LoadAsset<GameObject>("assets/modassets/fx/FX_NuclearExplosion.prefab")));
        public static MadFX MissleFX = FXRegistry.Instance.Register(MadCore.Id("missle"), new MadFX(()=>ExampleInit.AssetBundle.LoadAsset<GameObject>("assets/modassets/fx/FX_Missle.prefab")));

        public static void RegisterAll(){}
    }
}