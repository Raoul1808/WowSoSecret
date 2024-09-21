using HarmonyLib;
using Steamworks;

namespace WowSoSecret
{
    [HarmonyPatch]
    public static class PresencePatches
    {
        [HarmonyPatch(typeof(SpinDiscord), nameof(SpinDiscord.UpdateActivityPresence))]
        [HarmonyPrefix]
        private static void Prefix(ref string state, ref string details, ref string coverArt, ref string trackArtist, ref string trackTitle, ref long endTime)
        {
            if (SecretManager.CurrentMode == SecretMode.Disabled) return;

            bool hideEdit = SecretManager.CurrentMode == SecretMode.Editing ||
                            SecretManager.CurrentMode == SecretMode.Global;
            bool hidePlay = SecretManager.CurrentMode == SecretMode.Playing ||
                            SecretManager.CurrentMode == SecretMode.Global;

            if (hidePlay)
                coverArt = "";

            if (GameStates.EditingTrack.IsActive && hideEdit)
            {
                details = SecretManager.GetEditorText();
                trackArtist = "Secret";
                trackTitle = "Secret";
                coverArt = "";
                endTime = 0;
            }

            if ((GameStates.PlayingTrack.IsActive || GameStates.PausedTrack.IsActive) && hidePlay)
            {
                details = SecretManager.GetPlayingText();
                trackArtist = "Secret";
                trackTitle = "Secret";
                coverArt = "";
                endTime = 0;
            }

            if ((GameStates.Failed.IsActive || GameStates.CompleteSequence.IsActive ||
                 GameStates.LevelComplete.IsActive || GameStates.SongCompleted.IsActive) && hidePlay)
            {
                details = SecretManager.GetResultsText();
                trackArtist = "Secret";
                trackTitle = "Secret";
                coverArt = "";
                endTime = 0;
            }
        }

        [HarmonyPatch(typeof(SteamFriends), nameof(SteamFriends.SetRichPresence))]
        [HarmonyPrefix]
        private static void PatchSteamPresence(string pchKey, ref string pchValue)
        {
            if (SecretManager.CurrentMode == SecretMode.Disabled) return;

            bool hideEdit = SecretManager.CurrentMode == SecretMode.Editing || SecretManager.CurrentMode == SecretMode.Global;
            bool hidePlay = SecretManager.CurrentMode == SecretMode.Playing || SecretManager.CurrentMode == SecretMode.Global;

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
                        pchValue = text + " - " + SecretManager.GetPlayingText();
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
                        pchValue = "In Level Editor - " + SecretManager.GetEditorText();
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
                        pchValue = "Results Screen - " + SecretManager.GetResultsText();
                        break;
                }
            }
        }

        [HarmonyPatch(typeof(XDMainMenu), nameof(XDMainMenu.OpenMenu))]
        [HarmonyPostfix]
        private static void DisplayTextOnMainMenuOpen()
        {
            SecretManager.DisplayCurrentMode();
            if (!SpinCoreSupport.Enabled)
                NotificationSystemGUI.AddMessage("Press F8 in the main menu to cycle between Secret Mode modes.");
        }
    }
}
