using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace ResearchAreas.HarmonyPatches
{
    /// <summary>
    /// UI patches for tooltips and visual indicators on area creation buttons.
    /// </summary>
    [HarmonyPatch]
    public static class UIPatches
    {
        /// <summary>
        /// Patch to add tooltips to area selection buttons.
        /// This patches the area allowed GUI to show research requirements.
        /// </summary>
        [HarmonyPatch(typeof(AreaAllowedGUI), "DoAreaRow")]
        [HarmonyPostfix]
        public static void AreaAllowedGUI_DoAreaRow_Postfix(Rect rect, Area area)
        {
            if (area == null)
                return;

            var settings = Settings.ResearchAreasMod.Settings;
            if (settings == null || !settings.showTooltips)
                return;

            // Check if area is locked
            if (!Core.ResearchChecker.IsAreaAllowed(area))
            {
                ResearchProjectDef requiredResearch = Core.ResearchChecker.GetRequiredResearch(area);
                if (requiredResearch != null)
                {
                    string tooltip = $"Requires research: {requiredResearch.label}";
                    if (Mouse.IsOver(rect))
                    {
                        TooltipHandler.TipRegion(rect, tooltip);
                    }
                }
            }
        }

        /// <summary>
        /// Patch to gray out locked area buttons.
        /// </summary>
        [HarmonyPatch(typeof(AreaAllowedGUI), "DoAreaRow")]
        [HarmonyPostfix]
        public static void AreaAllowedGUI_DoAreaRow_Visual_Postfix(Rect rect, Area area)
        {
            if (area == null)
                return;

            var settings = Settings.ResearchAreasMod.Settings;
            if (settings == null || !settings.showVisualIndicators)
                return;

            // Check if area is locked
            if (!Core.ResearchChecker.IsAreaAllowed(area))
            {
                // Gray out the button
                GUI.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
                
                // Draw lock icon
                Texture2D lockIcon = ContentFinder<Texture2D>.Get("UI/Overlays/Locked", false);
                if (lockIcon != null)
                {
                    Rect iconRect = new Rect(rect.xMax - 20f, rect.y, 16f, 16f);
                    GUI.DrawTexture(iconRect, lockIcon);
                }
                
                GUI.color = Color.white;
            }
        }

        /// <summary>
        /// Patch zone creation designators to show tooltips.
        /// </summary>
        [HarmonyPatch(typeof(Designator_ZoneAddStockpile), "GizmoOnGUI")]
        [HarmonyPostfix]
        public static void Designator_ZoneAddStockpile_GizmoOnGUI_Postfix(Rect topRect)
        {
            var settings = Settings.ResearchAreasMod.Settings;
            if (settings == null || !settings.showTooltips)
                return;

            ResearchProjectDef requiredResearch = Core.ResearchChecker.GetRequiredResearch("Stockpile");
            if (requiredResearch != null && !Core.ResearchChecker.IsResearchCompleted(requiredResearch))
            {
                if (Mouse.IsOver(topRect))
                {
                    string tooltip = $"Requires research: {requiredResearch.label}";
                    TooltipHandler.TipRegion(topRect, tooltip);
                }
            }
        }

        /// <summary>
        /// Patch growing zone designator to show tooltips.
        /// </summary>
        [HarmonyPatch(typeof(Designator_ZoneAdd_Growing), "GizmoOnGUI")]
        [HarmonyPostfix]
        public static void Designator_ZoneAdd_Growing_GizmoOnGUI_Postfix(Rect topRect)
        {
            var settings = Settings.ResearchAreasMod.Settings;
            if (settings == null || !settings.showTooltips)
                return;

            ResearchProjectDef requiredResearch = Core.ResearchChecker.GetRequiredResearch("Growing");
            if (requiredResearch != null && !Core.ResearchChecker.IsResearchCompleted(requiredResearch))
            {
                if (Mouse.IsOver(topRect))
                {
                    string tooltip = $"Requires research: {requiredResearch.label}";
                    TooltipHandler.TipRegion(topRect, tooltip);
                }
            }
        }
    }
}
