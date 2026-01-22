using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace ResearchAreas.HarmonyPatches
{
    /// <summary>
    /// UI patches for research tree to show unlocked areas.
    /// </summary>
    [HarmonyPatch]
    public static class ResearchUIPatches
    {
        /// <summary>
        /// Get the list of areas unlocked by a research project.
        /// </summary>
        private static string GetUnlockedAreas(ResearchProjectDef research)
        {
            if (research == null)
                return null;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            bool first = true;

            // Check each area type
            string[] areaTypes = { "Stockpile", "Growing", "AnimalSleeping", "AnimalAllowed", "Home", "NoRoof", "Allowed" };
            string[] areaLabels = { "Stockpile zones", "Growing zones", "Animal areas", "Animal areas", "Home area", "No-roof areas", "Custom allowed areas" };

            for (int i = 0; i < areaTypes.Length; i++)
            {
                ResearchProjectDef requiredResearch = Core.ResearchChecker.GetRequiredResearch(areaTypes[i]);
                if (requiredResearch != null && requiredResearch.defName == research.defName)
                {
                    if (!first)
                        sb.Append(", ");
                    sb.Append(areaLabels[i]);
                    first = false;
                }
            }

            return sb.Length > 0 ? sb.ToString() : null;
        }

        /// <summary>
        /// Patch research node drawing to add tooltip with unlocked areas.
        /// </summary>
        [HarmonyPatch(typeof(MainTabWindow_Research), "DrawResearchNode")]
        [HarmonyPostfix]
        public static void MainTabWindow_Research_DrawResearchNode_Postfix(Rect rect, ResearchProjectDef research)
        {
            if (research == null)
                return;

            // Check if this research unlocks any areas
            string unlockedAreas = GetUnlockedAreas(research);
            if (unlockedAreas != null && Mouse.IsOver(rect))
            {
                string tooltip = $"Unlocks: {unlockedAreas}";
                TooltipHandler.TipRegion(rect, tooltip);
            }
        }

        /// <summary>
        /// Alternative patch for research tree mods that might use different UI.
        /// </summary>
        [HarmonyPatch(typeof(ResearchProjectDef), "GetDescription")]
        [HarmonyPostfix]
        public static void ResearchProjectDef_GetDescription_Postfix(ResearchProjectDef __instance, ref string __result)
        {
            if (__instance == null)
                return;

            string unlockedAreas = GetUnlockedAreas(__instance);
            if (unlockedAreas != null)
            {
                __result += $"\n\nUnlocks: {unlockedAreas}";
            }
        }
    }
}
