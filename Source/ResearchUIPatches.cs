using System.Collections.Generic;
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
        // Icon size for area unlock indicators
        private const float IconSize = 16f;
        private const float IconSpacing = 2f;

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
        /// Get the list of area type keys unlocked by a research project.
        /// </summary>
        private static List<string> GetUnlockedAreaTypes(ResearchProjectDef research)
        {
            if (research == null)
                return null;

            List<string> unlockedTypes = new List<string>();
            string[] areaTypes = { "Stockpile", "Growing", "AnimalSleeping", "AnimalAllowed", "Home", "NoRoof", "Allowed" };

            foreach (string areaType in areaTypes)
            {
                ResearchProjectDef requiredResearch = Core.ResearchChecker.GetRequiredResearch(areaType);
                if (requiredResearch != null && requiredResearch.defName == research.defName)
                {
                    unlockedTypes.Add(areaType);
                }
            }

            return unlockedTypes.Count > 0 ? unlockedTypes : null;
        }

        /// <summary>
        /// Get icon texture for an area type.
        /// </summary>
        private static Texture2D GetAreaIcon(string areaType)
        {
            // Try to find appropriate icons - these are RimWorld's built-in icons
            // If not found, we'll use a generic icon or create a simple colored square
            string iconPath = null;
            
            switch (areaType)
            {
                case "Stockpile":
                    iconPath = "UI/Designators/ZoneCreate_Stockpile";
                    break;
                case "Growing":
                    iconPath = "UI/Designators/ZoneCreate_Growing";
                    break;
                case "AnimalSleeping":
                case "AnimalAllowed":
                    iconPath = "UI/Designators/ZoneCreate_Pen";
                    break;
                case "Home":
                    iconPath = "UI/Overlays/HomeArea";
                    break;
                case "NoRoof":
                    iconPath = "UI/Overlays/NoRoofArea";
                    break;
                case "Allowed":
                    iconPath = "UI/Overlays/AllowedArea";
                    break;
            }

            if (!string.IsNullOrEmpty(iconPath))
            {
                Texture2D icon = ContentFinder<Texture2D>.Get(iconPath, false);
                if (icon != null)
                    return icon;
            }

            // Fallback: return null (we'll draw a simple colored square instead)
            return null;
        }

        /// <summary>
        /// Draw a simple colored icon for an area type.
        /// </summary>
        private static void DrawAreaTypeIcon(Rect rect, string areaType)
        {
            Color iconColor = Color.white;
            
            switch (areaType)
            {
                case "Stockpile":
                    iconColor = new Color(0.8f, 0.6f, 0.2f); // Brown/orange
                    break;
                case "Growing":
                    iconColor = new Color(0.2f, 0.8f, 0.2f); // Green
                    break;
                case "AnimalSleeping":
                case "AnimalAllowed":
                    iconColor = new Color(0.6f, 0.4f, 0.8f); // Purple
                    break;
                case "Home":
                    iconColor = new Color(0.8f, 0.2f, 0.2f); // Red
                    break;
                case "NoRoof":
                    iconColor = new Color(0.4f, 0.6f, 0.9f); // Blue
                    break;
                case "Allowed":
                    iconColor = new Color(0.7f, 0.7f, 0.7f); // Gray
                    break;
            }

            // Draw a simple colored square with border
            Color oldColor = GUI.color;
            GUI.color = iconColor;
            Widgets.DrawBoxSolid(rect, iconColor);
            GUI.color = Color.black;
            Widgets.DrawBox(rect, 1);
            GUI.color = oldColor;
        }

        /// <summary>
        /// Patch research node drawing to add tooltip with unlocked areas and icons.
        /// </summary>
        [HarmonyPatch(typeof(MainTabWindow_Research), "DrawResearchNode")]
        [HarmonyPostfix]
        public static void MainTabWindow_Research_DrawResearchNode_Postfix(Rect rect, ResearchProjectDef research)
        {
            if (research == null)
                return;

            var settings = Settings.ResearchAreasMod.Settings;
            if (settings == null)
                return;

            List<string> unlockedTypes = GetUnlockedAreaTypes(research);
            if (unlockedTypes == null || unlockedTypes.Count == 0)
                return;

            // Build tooltip text
            string unlockedAreas = GetUnlockedAreas(research);
            string tooltip = unlockedAreas != null ? $"Unlocks: {unlockedAreas}" : null;

            // Draw icons on the right side of the research node
            if (settings.showVisualIndicators)
            {
                float iconX = rect.xMax - (unlockedTypes.Count * (IconSize + IconSpacing)) - 4f;
                float iconY = rect.y + 2f;

                for (int i = 0; i < unlockedTypes.Count; i++)
                {
                    Rect iconRect = new Rect(iconX + (i * (IconSize + IconSpacing)), iconY, IconSize, IconSize);
                    
                    // Try to get an icon texture
                    Texture2D icon = GetAreaIcon(unlockedTypes[i]);
                    if (icon != null)
                    {
                        GUI.DrawTexture(iconRect, icon);
                    }
                    else
                    {
                        // Draw a simple colored square as fallback
                        DrawAreaTypeIcon(iconRect, unlockedTypes[i]);
                    }

                    // Add tooltip to icon
                    if (Mouse.IsOver(iconRect) && tooltip != null)
                    {
                        TooltipHandler.TipRegion(iconRect, tooltip);
                    }
                }
            }

            // Add tooltip to entire research node
            if (tooltip != null && Mouse.IsOver(rect))
            {
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
