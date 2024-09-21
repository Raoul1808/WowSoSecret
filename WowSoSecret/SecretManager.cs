using System;
using System.IO;
using BepInEx;
using BepInEx.Configuration;

namespace WowSoSecret
{
    public static class SecretManager
    {
        private static ConfigFile _config;
        internal static SecretTexts SecretTexts { get; private set; }

        public static readonly string SecretsPath = Path.Combine(Paths.ConfigPath, "SecretTexts.json");
        public static bool CustomSecrets { get; set; }

        public static SecretMode CurrentMode { get; set; }

        internal static void Init(ConfigFile config)
        {
            _config = config;

            var customSecrets = _config.Bind("General",
                "CustomSecrets",
                true,
                "If set to true, the mod will load secret texts configured in the SecretTexts.json file. If it does not exist, a new file will be created.");
            var startingMode = _config.Bind("General",
                "StartingMode",
                SecretMode.Disabled,
                "The starting secret mode mode.");

            CurrentMode = startingMode.Value;
            CustomSecrets = customSecrets.Value;
            LoadSecretsFromDisk();
        }

        internal static void LoadSecretsFromDisk()
        {
            try
            {
                SecretTexts = SecretTexts.FromJson(File.ReadAllText(SecretsPath));
            }
            catch (Exception)
            {
                CustomSecrets = false;
            }
        }

        internal static void SaveSecrets()
        {
            File.WriteAllText(SecretsPath, SecretTexts.ToJson());
        }
        
        public static void DisplayCurrentMode()
        {
            string status;
            switch (CurrentMode)
            {
                case SecretMode.Editing:
                    status = "enabled in EDITOR";
                    break;
                
                case SecretMode.Playing:
                    status = "enabled while PLAYING";
                    break;
                
                case SecretMode.Global:
                    status = "GLOBALLY ENABLED";
                    break;
                
                case SecretMode.Disabled:
                default:
                    status = "DISABLED";
                    break;
            }
            
            NotificationSystemGUI.AddMessage("Secret Mode is currently " + status + ".");
        }

        public static string GetPlayingText() => CustomSecrets ? SecretTexts.Playing.GetRandomOrDefault(SecretTexts.DefaultText) : SecretTexts.DefaultText;
        public static string GetEditorText() => CustomSecrets ? SecretTexts.Editor.GetRandomOrDefault(SecretTexts.DefaultText) : SecretTexts.DefaultText;
        public static string GetResultsText() => CustomSecrets ? SecretTexts.Results.GetRandomOrDefault(SecretTexts.DefaultText) : SecretTexts.DefaultText;
    }
}
