using HarmonyLib;
using RimWorld;
using Verse;

namespace ResearchAreas.UI
{
    /// <summary>
    /// Handles tooltip integration to show research requirements for areas.
    /// </summary>
    public static class TooltipHandler
    {

        /// <summary>
        /// Get tooltip text for an area type.
        /// </summary>
        /// <param name="areaKey">The area type key</param>
        /// <returns>Tooltip text, or null if no research required</returns>
        public static string GetTooltipText(string areaKey)
        {
            ResearchProjectDef requiredResearch = Core.ResearchChecker.GetRequiredResearch(areaKey);
            
            if (requiredResearch == null)
                return null;

            bool completed = Core.ResearchChecker.IsResearchCompleted(requiredResearch);
            
            if (completed)
                return null; // No tooltip needed if research is completed

            return $"Requires research: {requiredResearch.label}";
        }

        /// <summary>
        /// Get tooltip text for an area.
        /// </summary>
        /// <param name="area">The area</param>
        /// <returns>Tooltip text, or null if no research required</returns>
        public static string GetTooltipText(Area area)
        {
            if (area == null)
                return null;

            ResearchProjectDef requiredResearch = Core.ResearchChecker.GetRequiredResearch(area);
            
            if (requiredResearch == null)
                return null;

            bool completed = Core.ResearchChecker.IsResearchCompleted(requiredResearch);
            
            if (completed)
                return null; // No tooltip needed if research is completed

            return $"Requires research: {requiredResearch.label}";
        }

        /// <summary>
        /// Patch to add tooltips to area selection UI.
        /// This is a placeholder - actual UI patching would depend on Rimworld's UI structure.
        /// </summary>
        [HarmonyPatch]
        public static class AreaUITooltipPatch
        {
            // Note: Actual UI patching would require knowledge of Rimworld's specific UI classes
            // This is a framework that can be extended based on Rimworld's UI implementation
            // Common targets might be:
            // - AreaAllowedGUI
            // - ZoneManager
            // - ArchitectCategoryTab
            
            // For now, the tooltip functionality is available through the static methods above
            // and can be called from appropriate UI patches when needed.
        }
    }
}
