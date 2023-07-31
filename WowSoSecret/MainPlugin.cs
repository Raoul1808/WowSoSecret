using BepInEx;

namespace WowSoSecret
{
    [BepInPlugin("srxd.raoul1808.wowsosecret", "Wow So Secret", "0.1.0")]
    public class MainPlugin : BaseUnityPlugin
    {
        void Awake()
        {
            Logger.LogMessage("Wow So...back?");
        }
    }
}
