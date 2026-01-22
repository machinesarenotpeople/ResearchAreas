using HarmonyLib;
using RimWorld;
using Verse;

namespace ResearchAreas.HarmonyPatches
{
    /// <summary>
    /// Harmony patches for ZoneManager to intercept zone creation and validate research requirements.
    /// Currently gates: Stockpile zones and Growing zones
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

            // If no research was found or determined for this zone type, allow creation
            if (areaKey == null || requiredResearch == null)
                return true;

            // Block creation if research is not completed
            if (!Core.ResearchChecker.IsResearchCompleted(requiredResearch))
            {
                Messages.Message("Zoning research needed.", MessageTypeDefOf.RejectInput);
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Patch Designator_ZoneAddStockpile to disable button when research incomplete.
    /// </summary>
    [HarmonyPatch(typeof(Designator_ZoneAddStockpile))]
    public static class Designator_ZoneAddStockpile_Patch
    {
        [HarmonyPatch("CanDesignateCell")]
        [HarmonyPostfix]
        static void CanDesignateCell_Postfix(ref AcceptanceReport __result)
        {
            ResearchProjectDef requiredResearch = Core.ResearchChecker.GetRequiredResearch("Stockpile");
            if (requiredResearch != null && !Core.ResearchChecker.IsResearchCompleted(requiredResearch))
            {
                __result = new AcceptanceReport($"Requires research: {requiredResearch.label}");
            }
        }
    }

    /// <summary>
    /// Patch Designator_ZoneAdd_Growing to disable button when research incomplete.
    /// </summary>
    [HarmonyPatch(typeof(Designator_ZoneAdd_Growing))]
    public static class Designator_ZoneAdd_Growing_Patch
    {
        [HarmonyPatch("CanDesignateCell")]
        [HarmonyPostfix]
        static void CanDesignateCell_Postfix(ref AcceptanceReport __result)
        {
            ResearchProjectDef requiredResearch = Core.ResearchChecker.GetRequiredResearch("Growing");
            if (requiredResearch != null && !Core.ResearchChecker.IsResearchCompleted(requiredResearch))
            {
                __result = new AcceptanceReport($"Requires research: {requiredResearch.label}");
            }
        }
    }
}
