using HarmonyLib;
using RimWorld;
using Verse;

namespace ResearchAreas.HarmonyPatches
{
    /// <summary>
    /// Harmony patches for ZoneManager to intercept zone creation and validate research requirements.
    /// </summary>
    [HarmonyPatch(typeof(ZoneManager), nameof(ZoneManager.RegisterZone))]
    public static class ZoneManager_RegisterZone_Patch
    {
        static bool Prefix(Zone newZone)
        {
            if (newZone == null)
                return true;

            // Check research requirements based on zone type
            ResearchProjectDef requiredResearch = null;
            string areaKey = null;

            if (newZone is Zone_Stockpile)
            {
                areaKey = "Stockpile";
                requiredResearch = Core.ResearchChecker.GetRequiredResearch(areaKey);
            }
            else if (newZone is Zone_Growing)
            {
                areaKey = "Growing";
                requiredResearch = Core.ResearchChecker.GetRequiredResearch(areaKey);
            }

            if (requiredResearch != null && !Core.ResearchChecker.IsResearchCompleted(requiredResearch))
            {
                string message = $"Cannot create {areaKey.ToLower()} zone. Requires research: {requiredResearch.label}.";
                Messages.Message(message, MessageTypeDefOf.RejectInput);
                return false;
            }

            return true;
        }
    }
}
