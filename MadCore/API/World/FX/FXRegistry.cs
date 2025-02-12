using MadCore.API.Registry;

namespace MadCore.API.World.FX
{
    public class FXRegistry : MadRegistry<MadFX>
    {
        public static readonly FXRegistry Instance = new FXRegistry();

        public FXManager FXManager;

        protected override void OnLoad(ManagersScript managersScript)
        {
            FXManager = managersScript.fxMN;
        }
    }
}