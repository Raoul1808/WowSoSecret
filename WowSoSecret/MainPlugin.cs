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
        private const string Version = "1.0.0";

        private static ManualLogSource _logger;
        
        void Awake()
        {
            _logger = Logger;
            Harmony harmony = new Harmony(Guid);
            harmony.PatchAll(typeof(PresencePatches));
            harmony.PatchAll(Assembly.GetExecutingAssembly());  // For some completely utterly unknown f**ing reason, this is required to apply the inner PresencePatch patch class.
        }

        public static void Log(object msg) => _logger.LogMessage(msg);
    }
}
