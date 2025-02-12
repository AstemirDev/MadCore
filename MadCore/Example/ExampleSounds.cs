using MadCore.API.World.Sound;

namespace MadCore.Example
{
    public static class ExampleSounds
    {
        public static MadSound Missle = SoundRegistry.Instance.Register(MadCore.Id("missle"), new MadSound("missle", "Assets/sounds/missle.ogg"));
        public static MadSound Explosion = SoundRegistry.Instance.Register(MadCore.Id("explosion"), new MadSound("explosion", "Assets/sounds/explosion.ogg"));

        public static void RegisterAll(){}
    }
}