using System.Reflection;
using System.Runtime.CompilerServices;
using BepInEx.Bootstrap;
using SpinCore.Translation;
using SpinCore.UI;

namespace WowSoSecret
{
    internal static class SpinCoreSupport
    {
        public const string Guid = "srxd.raoul1808.spincore";
        public static bool Enabled => Chainloader.PluginInfos.ContainsKey(Guid);
        
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void Init()
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WowSoSecret.locale.json");
            TranslationHelper.LoadTranslationsFromStream(stream);
            
            UIHelper.RegisterGroupInQuickModSettings(quickParent =>
            {
                var group = UIHelper.CreateGroup(quickParent, "WowSoSecret");
                UIHelper.CreateSectionHeader(
                    group.Transform,
                    "Header",
                    "WowSoSecret_Name",
                    false
                );
                UIHelper.CreateSmallMultiChoiceButton(
                    group.Transform,
                    "SecretModeToggle",
                    "WowSoSecret_SecretModeOption",
                    SecretManager.CurrentMode,
                    val => SecretManager.CurrentMode = val
                );
            });
            
            var page = UIHelper.CreateCustomPage("WowSoSecret");
            page.OnPageLoad += pageParent =>
            {
                var generalGroup = UIHelper.CreateGroup(pageParent, "General");

                UIHelper.CreateSectionHeader(
                    generalGroup.Transform,
                    "Header",
                    "WowSoSecret_Name",
                    false
                );
                UIHelper.CreateLargeMultiChoiceButton(
                    generalGroup.Transform,
                    "SecretModeToggle",
                    "WowSoSecret_SecretModeOption",
                    SecretManager.CurrentMode,
                    val => SecretManager.CurrentMode = val
                );
                UIHelper.CreateLargeToggle(
                    generalGroup.Transform,
                    "CustomSecretsToggle",
                    "WowSoSecret_CustomSecretsToggle",
                    SecretManager.CustomSecrets,
                    val => SecretManager.CustomSecrets = val
                );
            };
            UIHelper.RegisterMenuInModSettingsRoot("WowSoSecret_Name", page);
        }
    }
}
