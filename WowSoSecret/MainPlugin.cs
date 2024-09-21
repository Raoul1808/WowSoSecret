using System.IO;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace WowSoSecret
{
    [BepInPlugin(Guid, Name, Version)]
    [BepInDependency(SpinCoreGuid, BepInDependency.DependencyFlags.HardDependency)]
    public class MainPlugin : BaseUnityPlugin
    {
        private const string Guid = "srxd.raoul1808.wowsosecret";
        private const string Name = "Wow So Secret";
        private const string Version = "2.0.0";

        private const string SpinCoreGuid = "srxd.raoul1808.spincore";

        private static ManualLogSource _logger;

        private static string _secretsPath = Path.Combine(Paths.ConfigPath, "SecretTexts.json");

        void Awake()
        {
            _logger = Logger;

            if (!File.Exists(_secretsPath))
                File.WriteAllText(_secretsPath, SecretTexts.Default().ToJson());

            SecretManager.Init(Config);

            Harmony harmony = new Harmony(Guid);
            harmony.PatchAll(typeof(PresencePatches));
            SpinCoreSupport.Init();
        }

        public static void Log(object msg) => _logger.LogMessage(msg);
    }
}
