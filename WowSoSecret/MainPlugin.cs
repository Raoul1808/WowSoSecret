using System;
using System.IO;
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

        private static string _configFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Speen Mods");

        private static string _secretsPath = Path.Combine(_configFolder, "SecretTexts.json");
        
        void Awake()
        {
            _logger = Logger;
            
            if (!Directory.Exists(_configFolder))
                Directory.CreateDirectory(_configFolder);
            if (!File.Exists(_secretsPath))
                File.WriteAllText(_secretsPath, SecretTexts.Default().ToJson());

            PresencePatches.LoadSecretTexts(_secretsPath);
            
            Harmony harmony = new Harmony(Guid);
            harmony.PatchAll(typeof(PresencePatches));
            harmony.PatchAll(Assembly.GetExecutingAssembly());  // For some completely utterly unknown f**ing reason, this is required to apply the inner PresencePatch patch class.
        }

        public static void Log(object msg) => _logger.LogMessage(msg);
    }
}
