using System.Collections.Generic;
using System.Reflection;
using SpinCore;
using SpinCore.Translation;
using SpinCore.UI;
using UnityEngine;
using UnityEngine.UI;

namespace WowSoSecret
{
    internal static class SpinCoreSupport
    {
        private static CustomGroup _editorGroup;
        private static CustomGroup _playingGroup;
        private static CustomGroup _resultsGroup;
        private static int _currentCategory;
        private static CustomInputField _secretInput;

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
                var customGroup = UIHelper.CreateGroup(pageParent, "Custom");

                UIHelper.CreateSectionHeader(
                    generalGroup.Transform,
                    "Header",
                    "WowSoSecret_GeneralSection",
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
                    val =>
                    {
                        SecretManager.CustomSecrets = val;
                        customGroup.Active = val;
                    });

                UIHelper.CreateSectionHeader(
                    customGroup.Transform,
                    "Header",
                    "WowSoSecret_CustomSecrets_Header",
                    true
                );
                {
                    var group = UIHelper.CreateGroup(customGroup.Transform, "LoadSaveButtons", Axis.Horizontal);
                    UIHelper.CreateButton(
                        group.Transform,
                        "LoadButton",
                        "WowSoSecret_CustomSecrets_LoadButton",
                        () =>
                        {
                            ReloadSecrets();
                            NotificationSystemGUI.AddMessage("Reloaded custom secrets");
                        }
                    );
                    UIHelper.CreateButton(
                        group.Transform,
                        "SaveButton",
                        "WowSoSecret_CustomSecrets_SaveButton",
                        () =>
                        {
                            SecretManager.SaveSecrets();
                            NotificationSystemGUI.AddMessage("Saved custom secrets");
                        }
                    );
                }
                _secretInput = UIHelper.CreateInputField(
                    customGroup.Transform,
                    "SecretInput",
                    (oldVal, newVal) => AddNewSecret(newVal)
                );
                {
                    var group = UIHelper.CreateGroup(customGroup.Transform, "CategoryButtons", Axis.Horizontal);
                    var buttons = new CustomButton[3];
                    var categories = new[] { "Editor", "Playing", "Results" };
                    for (int i = 0; i < 3; i++)
                    {
                        string category = categories[i];
                        int index = i;
                        buttons[i] = UIHelper.CreateButton(
                            group.Transform,
                            $"{category}Button",
                            $"WowSoSecret_{category}Category",
                            () =>
                            {
                                _currentCategory = index;
                                for (int j = 0; j < categories.Length; j++)
                                {
                                    _editorGroup.Active = index == 0;
                                    _playingGroup.Active = index == 1;
                                    _resultsGroup.Active = index == 2;
                                    buttons[j].GameObject.GetComponent<XDNavigable>().forceExpanded = index == j;
                                }
                            }
                        );
                    }
                    buttons[0].GameObject.GetComponent<XDNavigable>().forceExpanded = true;
                }
                _editorGroup = UIHelper.CreateGroup(customGroup.Transform, "Editor");
                _playingGroup = UIHelper.CreateGroup(customGroup.Transform, "Playing");
                _resultsGroup = UIHelper.CreateGroup(customGroup.Transform, "Results");
                ReloadSecrets();
            };
            UIHelper.RegisterMenuInModSettingsRoot("WowSoSecret_Name", page);
        }

        private static void ReloadSecrets()
        {
            SecretManager.LoadSecretsFromDisk();
            if (!SecretManager.CustomSecrets) return;
            ReloadGroup(SecretManager.SecretTexts.Editor, _editorGroup);
            ReloadGroup(SecretManager.SecretTexts.Playing, _playingGroup);
            ReloadGroup(SecretManager.SecretTexts.Results, _resultsGroup);
            _editorGroup.Active = _currentCategory == 0;
            _playingGroup.Active = _currentCategory == 1;
            _resultsGroup.Active = _currentCategory == 2;
        }

        private static void ReloadGroup(List<string> list, CustomGroup affectedGroup)
        {
            affectedGroup.Transform.RemoveAllChildren();
            for (int i = 0; i < list.Count; i++)
            {
                string text = list[i];
                var group = UIHelper.CreateGroup(affectedGroup.Transform, text, Axis.Horizontal);
                var layoutGroup = (HorizontalOrVerticalLayoutGroup) group.LayoutGroup;
                layoutGroup.childForceExpandWidth = false;
                layoutGroup.childForceExpandHeight = false;
                layoutGroup.spacing = 10;
                var button = UIHelper.CreateButton(
                    group.Transform,
                    "Delete",
                    "WowSoSecret_DeleteButtonLabel",
                    () =>
                    {
                        list.Remove(text);
                        Object.DestroyImmediate(group.GameObject);
                    }
                );
                button.GameObject.GetComponent<LayoutElement>().preferredWidth = 80;
                button.GameObject.GetComponent<LayoutElement>().preferredHeight = 60;
                var label = UIHelper.CreateLabel(
                    group.Transform,
                    "Label",
                    ""
                );
                label.ExtraText = text;
                label.GameObject.GetComponent<LayoutElement>().preferredHeight = 60;
            }
        }

        private static void AddNewSecret(string secret)
        {
            List<string> listRef;
            CustomGroup groupRef;
            switch (_currentCategory)
            {
                case 0:
                    listRef = SecretManager.SecretTexts.Editor;
                    groupRef = _editorGroup;
                    break;
                case 1:
                    listRef = SecretManager.SecretTexts.Playing;
                    groupRef = _playingGroup;
                    break;
                case 2:
                    listRef = SecretManager.SecretTexts.Results;
                    groupRef = _resultsGroup;
                    break;
                default:
                    return;
            }

            if (listRef.Contains(secret)) return;
            listRef.Add(secret);
            ReloadGroup(listRef, groupRef);
            _secretInput.InputField.SetText(string.Empty, true);
        }
    }
}
