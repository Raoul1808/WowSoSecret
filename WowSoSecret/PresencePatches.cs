using System.Reflection;
using HarmonyLib;

namespace WowSoSecret
{
    public static class PresencePatches
    {
        private static bool _secretMode = false;

        [HarmonyPatch]
        private class PresencePatch
        {
            private static MethodBase TargetMethod()
            {
                var type = AccessTools.TypeByName("SpinDiscord");
                return AccessTools.Method(type, "UpdateActivityPresence");
            }

            private static void Prefix(ref string state, ref string details, ref string coverArt, ref string trackArtist, ref string trackTitle, ref long endTime)
            {
                details = "i am become mew";
                state = "destroyer of presences";
                coverArt = "wrong game";
                trackArtist = "destroyer of worlds";
                trackTitle = "not eater";
                // if (GameStates.EditingTrack.IsActive)
                // {
                //     details = "Editing <SECRET>";
                //     state = "Secret Mode Enabled!";
                //     trackArtist = "Secret";
                //     trackTitle = "Secret";
                //     coverArt = "Secret";
                //     endTime = 0;
                // }
            }
        }
    }
}
