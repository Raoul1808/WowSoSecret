using System.IO;
using System.Reflection;
using HarmonyLib;
using Steamworks;
using UnityEngine;

namespace WowSoSecret
{
    public static class PresencePatches
    {
        public static SecretModeMode CurrentMode = SecretModeMode.Disabled;
        private static SecretTexts _secrets = SecretTexts.Default();

        public static void LoadSecretTexts(string path, bool customSecrets)
        {
            if (customSecrets)
                _secrets = SecretTexts.FromJson(File.ReadAllText(path));
        }

        [HarmonyPatch]
        private class DiscordPresencePatch
        {
            private static MethodBase TargetMethod()
            {
                var type = AccessTools.TypeByName("SpinDiscord");
                return AccessTools.Method(type, "UpdateActivityPresence");
            }

            private static void Prefix(ref string state, ref string details, ref string coverArt, ref string trackArtist, ref string trackTitle, ref long endTime)
            {
                if (CurrentMode == SecretModeMode.Disabled) return;

                bool hideEdit = CurrentMode == SecretModeMode.Editing || CurrentMode == SecretModeMode.Global;
                bool hidePlay = CurrentMode == SecretModeMode.Playing || CurrentMode == SecretModeMode.Global;

                if (hidePlay)
                    coverArt = "";
                
                if (GameStates.EditingTrack.IsActive && hideEdit)
                {
                    details = _secrets.Editor.GetRandomOrDefault("Secret Mode Enabled!");
                    trackArtist = "Secret";
                    trackTitle = "Secret";
                    coverArt = "";
                    endTime = 0;
                }

                if ((GameStates.PlayingTrack.IsActive || GameStates.PausedTrack.IsActive) && hidePlay)
                {
                    details = _secrets.Playing.GetRandomOrDefault("Secret Mode Enabled!");
                    trackArtist = "Secret";
                    trackTitle = "Secret";
                    coverArt = "";
                    endTime = 0;
                }

                if ((GameStates.Failed.IsActive || GameStates.CompleteSequence.IsActive || GameStates.LevelComplete.IsActive || GameStates.SongCompleted.IsActive) && hidePlay)
                {
                    details = _secrets.Results.GetRandomOrDefault("Secret Mode Enabled!");
                    trackArtist = "Secret";
                    trackTitle = "Secret";
                    coverArt = "";
                    endTime = 0;
                }
            }
        }

        [HarmonyPatch(typeof(SteamFriends), nameof(SteamFriends.SetRichPresence))]
        [HarmonyPrefix]
        private static void PatchSteamPresence(string pchKey, ref string pchValue)
        {
            if (CurrentMode == SecretModeMode.Disabled) return;

            bool hideEdit = CurrentMode == SecretModeMode.Editing || CurrentMode == SecretModeMode.Global;
            bool hidePlay = CurrentMode == SecretModeMode.Playing || CurrentMode == SecretModeMode.Global;

            if ((GameStates.PlayingTrack.IsActive || GameStates.PausedTrack.IsActive) && hidePlay)
            {
                string text = GameStates.PausedTrack.IsActive ? "Paused" : "Playing";
                
                switch (pchKey)
                {
                    case "currentTrack":
                    case "currentArtist":
                        pchValue = "Secret";
                        break;
                    
                    case "generalStatus":
                        pchValue = text + " - " + _secrets.Playing.GetRandomOrDefault("Secret Mode Enabled!");
                        break;
                }
            }

            if (GameStates.EditingTrack.IsActive && hideEdit)
            {
                switch (pchKey)
                {
                    case "currentTrack":
                    case "currentArtist":
                        pchValue = "Secret";
                        break;
                    
                    case "generalStatus":
                        pchValue = "In Level Editor - " + _secrets.Editor.GetRandomOrDefault("Secret Mode Enabled!");
                        break;
                }
            }

            if ((GameStates.Failed.IsActive || GameStates.CompleteSequence.IsActive || GameStates.LevelComplete.IsActive || GameStates.SongCompleted.IsActive) && hidePlay)
            {
                switch (pchKey)
                {
                    case "currentTrack":
                    case "currentArtist":
                        pchValue = "Secret";
                        break;
                    
                    case "generalStatus":
                        pchValue = "Results Screen - " + _secrets.Results.GetRandomOrDefault("Secret Mode Enabled!");
                        break;
                }
            }
        }

        [HarmonyPatch(typeof(XDMainMenu), nameof(XDMainMenu.OpenMenu))]
        [HarmonyPostfix]
        private static void DisplayTextOnMainMenuOpen()
        {
            DisplayCurrentMode();
            NotificationSystemGUI.AddMessage("Press F8 in the main menu to cycle between Secret Mode modes.");
        }

        private static void DisplayCurrentMode()
        {
            string status;
            switch (CurrentMode)
            {
                case SecretModeMode.Editing:
                    status = "enabled in EDITOR";
                    break;
                
                case SecretModeMode.Playing:
                    status = "enabled while PLAYING";
                    break;
                
                case SecretModeMode.Global:
                    status = "GLOBALLY ENABLED";
                    break;
                
                case SecretModeMode.Disabled:
                default:
                    status = "DISABLED";
                    break;
            }
            
            NotificationSystemGUI.AddMessage("Secret Mode is currently " + status + ".");
        }

        [HarmonyPatch(typeof(XDMainMenu), "Update")]
        [HarmonyPostfix]
        private static void UpdateSecretMode()
        {
            if (Input.GetKeyDown(KeyCode.F8))
            {
                CurrentMode++;
                if (CurrentMode > SecretModeMode.Global)
                    CurrentMode = SecretModeMode.Disabled;
                DisplayCurrentMode();
            }
        }
    }
}
