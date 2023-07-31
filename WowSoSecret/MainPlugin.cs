using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace WowSoSecret
{
    [BepInPlugin(Guid, Name, Version)]
    public class MainPlugin : BaseUnityPlugin
    {
        private const string Guid = "srxd.raoul1808.wowsosecret";
        private const string Name = "Wow So Secret";
        private const string Version = "0.1.0";

        private static ManualLogSource _logger;
        
        void Awake()
        {
            _logger = Logger;
            Log("Wow So Secret");
            Harmony harmony = new Harmony(Guid);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public static void Log(object msg) => _logger.LogMessage(msg);
    }
}
