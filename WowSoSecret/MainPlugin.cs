using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace WowSoSecret
{
    [BepInPlugin(Guid, Name, Version)]
    public class MainPlugin : BaseUnityPlugin
    {
        private const string Guid = "srxd.raoul1808.wowsosecret";
        private const string Name = "Wow So Secret";
        private const string Version = "1.1.0";

        private static ManualLogSource _logger;

        private static string _secretsPath = Path.Combine(Paths.ConfigPath, "SecretTexts.json");

        private static ConfigFile _config = new ConfigFile(Path.Combine(Paths.ConfigPath, "WowSoSecretConfig.cfg"), true);
        
        void Awake()
        {
            _logger = Logger;
            
            if (!File.Exists(_secretsPath))
                File.WriteAllText(_secretsPath, SecretTexts.Default().ToJson());

            var customSecrets = _config.Bind("General",
                "CustomSecrets",
                true,
                "If set to true, the mod will load secret texts configured in the SecretTexts.json file. If it does not exist, a new file will be created.");

            var startingMode = _config.Bind("General",
                "StartingMode",
                SecretModeMode.Disabled,
                "The starting secret mode mode.");

            PresencePatches.CurrentMode = startingMode.Value;
            PresencePatches.LoadSecretTexts(_secretsPath, customSecrets.Value);
            
            Harmony harmony = new Harmony(Guid);
            harmony.PatchAll(typeof(PresencePatches));
            harmony.PatchAll(Assembly.GetExecutingAssembly());  // For some completely utterly unknown f**ing reason, this is required to apply the inner PresencePatch patch class.
        }

        public static void Log(object msg) => _logger.LogMessage(msg);
    }
}
