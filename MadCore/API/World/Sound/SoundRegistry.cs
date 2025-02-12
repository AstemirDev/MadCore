using System.Numerics;
using MadCore.API.Registry;
using Vector3 = UnityEngine.Vector3;

namespace MadCore.API.World.Sound
{
    public class SoundRegistry : MadRegistry<MadSound>
    {
        public static readonly SoundRegistry Instance = new SoundRegistry();
        public SoundManager SoundManager;
        
        protected override void OnLoad(ManagersScript managersScript)
        {
            SoundManager = managersScript.sound;
            foreach (var madSound in Values)
            {
                MadCore.Logger.LogInfo("Building Sound Source "+madSound);
                madSound.BuildSoundSource();
            }
        }


        public static void PlaySound(SoundEffectId soundEffectId, Vector3 position, bool randomPitch = true)
        {
            Instance.SoundManager.Go3DSound((int)soundEffectId, position, randomPitch, Instance.SoundManager.soundBaseDist);
        }
    }
}