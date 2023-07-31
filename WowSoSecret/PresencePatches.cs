using System.Reflection;
using HarmonyLib;
using Steamworks;
using UnityEngine;

namespace WowSoSecret
{
    public static class PresencePatches
    {
        private static SecretModeMode _currentMode;

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
                if (_currentMode == SecretModeMode.Disabled) return;

                bool hideEdit = _currentMode == SecretModeMode.Editing || _currentMode == SecretModeMode.Both;
                bool hidePlay = _currentMode == SecretModeMode.Playing || _currentMode == SecretModeMode.Both;

                if (hidePlay)
                    coverArt = "";
                
                if (GameStates.EditingTrack.IsActive && hideEdit)
                {
                    details = "Editing <SECRET>";
                    state = "Secret Mode Enabled!";
                    trackArtist = "Secret";
                    trackTitle = "Secret";
                    coverArt = "";
                    endTime = 0;
                }

                if ((GameStates.PlayingTrack.IsActive || GameStates.PausedTrack.IsActive) && hidePlay)
                {
                    details = "Playing <SECRET>";
                    state = "Secret Mode Enabled!";
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
            if (_currentMode == SecretModeMode.Disabled) return;

            bool hideEdit = _currentMode == SecretModeMode.Editing || _currentMode == SecretModeMode.Both;
            bool hidePlay = _currentMode == SecretModeMode.Playing || _currentMode == SecretModeMode.Both;

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
                        pchValue = text + " - <SECRET>";
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
                        pchValue = "In Level Editor - <SECRET>";
                        break;
                }
            }
        }

        [HarmonyPatch(typeof(XDMainMenu), nameof(XDMainMenu.OpenMenu))]
        [HarmonyPostfix]
        private static void DisplayTextOnMainMenuOpen()
        {
            DisplayCurrentMode();
            NotificationSystemGUI.AddMessage("Press F8 to cycle between Secret Mode modes.");
        }

        private static void DisplayCurrentMode()
        {
            string status;
            switch (_currentMode)
            {
                case SecretModeMode.Editing:
                    status = "enabled in EDITOR";
                    break;
                
                case SecretModeMode.Playing:
                    status = "enabled while PLAYING";
                    break;
                
                case SecretModeMode.Both:
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
                _currentMode++;
                if (_currentMode > SecretModeMode.Both)
                    _currentMode = SecretModeMode.Disabled;
                DisplayCurrentMode();
            }
        }
    }
}
