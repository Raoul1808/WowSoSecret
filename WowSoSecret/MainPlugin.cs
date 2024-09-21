using System.IO;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace WowSoSecret
{
    [BepInPlugin(Guid, Name, Version)]
    [BepInDependency(SpinCoreSupport.Guid, BepInDependency.DependencyFlags.SoftDependency)]
    public class MainPlugin : BaseUnityPlugin
    {
        private const string Guid = "srxd.raoul1808.wowsosecret";
        private const string Name = "Wow So Secret";
        private const string Version = "1.1.0";
        
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
            //harmony.PatchAll(Assembly.GetExecutingAssembly());  // For some completely utterly unknown f**ing reason, this is required to apply the inner PresencePatch patch class.
            if (SpinCoreSupport.Enabled)
                SpinCoreSupport.Init();
            else
                harmony.PatchAll(typeof(CorelessPatch));
        }

        private class CorelessPatch
        {
            [HarmonyPatch(typeof(XDMainMenu), "Update")]
            [HarmonyPostfix]
            private static void UpdateSecretMode()
            {
                if (Input.GetKeyDown(KeyCode.F8))
                {
                    SecretManager.CurrentMode++;
                    if (SecretManager.CurrentMode > SecretMode.Global)
                        SecretManager.CurrentMode = SecretMode.Disabled;
                    SecretManager.DisplayCurrentMode();
                }
            }
        }

        public static void Log(object msg) => _logger.LogMessage(msg);
    }
}
