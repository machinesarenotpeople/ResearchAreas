using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ResearchAreas.Core
{
    /// <summary>
    /// Utility class to check if areas are allowed based on research completion.
    /// </summary>
    public static class ResearchChecker
    {
        private static Dictionary<string, ResearchProjectDef> areaResearchMap;
        private static Dictionary<string, bool> researchCache;
        private static bool cacheInitialized = false;

        /// <summary>
        /// Initialize the mapping between area types and research projects.
        /// </summary>
        static ResearchChecker()
        {
            InitializeMapping();
        }

        /// <summary>
        /// Initialize the area-to-research mapping.
        /// </summary>
        private static void InitializeMapping()
        {
            areaResearchMap = new Dictionary<string, ResearchProjectDef>
            {
                { "Stockpile", DefDatabase<ResearchProjectDef>.GetNamed("ResearchAreas_Stockpiles", false) },
                { "Growing", DefDatabase<ResearchProjectDef>.GetNamed("ResearchAreas_GrowingZones", false) },
                { "AnimalSleeping", DefDatabase<ResearchProjectDef>.GetNamed("ResearchAreas_AnimalAreas", false) },
                { "AnimalAllowed", DefDatabase<ResearchProjectDef>.GetNamed("ResearchAreas_AnimalAreas", false) },
                { "Home", DefDatabase<ResearchProjectDef>.GetNamed("ResearchAreas_Home", false) },
                { "NoRoof", DefDatabase<ResearchProjectDef>.GetNamed("ResearchAreas_NoRoof", false) },
                { "Allowed", DefDatabase<ResearchProjectDef>.GetNamed("ResearchAreas_Allowed", false) }
            };

            researchCache = new Dictionary<string, bool>();
        }

        /// <summary>
        /// Check if an area is allowed based on research completion.
        /// </summary>
        /// <param name="area">The area to check</param>
        /// <returns>True if the area is allowed, false otherwise</returns>
        public static bool IsAreaAllowed(Area area)
        {
            if (area == null)
                return true;

            string areaKey = GetAreaKey(area);
            if (string.IsNullOrEmpty(areaKey))
                return true; // Unknown area types are allowed by default

            ResearchProjectDef requiredResearch = GetRequiredResearch(areaKey);
            if (requiredResearch == null)
                return true; // No research required for this area type

            return IsResearchCompleted(requiredResearch);
        }

        /// <summary>
        /// Get the research project required for an area type.
        /// </summary>
        /// <param name="areaKey">The area type key</param>
        /// <returns>The required research project, or null if none required</returns>
        public static ResearchProjectDef GetRequiredResearch(string areaKey)
        {
            if (areaResearchMap == null)
                InitializeMapping();

            areaResearchMap.TryGetValue(areaKey, out ResearchProjectDef research);
            return research;
        }

        /// <summary>
        /// Get the research project required for an area.
        /// </summary>
        /// <param name="area">The area</param>
        /// <returns>The required research project, or null if none required</returns>
        public static ResearchProjectDef GetRequiredResearch(Area area)
        {
            if (area == null)
                return null;

            string areaKey = GetAreaKey(area);
            return GetRequiredResearch(areaKey);
        }

        /// <summary>
        /// Check if a research project is completed.
        /// </summary>
        /// <param name="research">The research project to check</param>
        /// <returns>True if completed, false otherwise</returns>
        public static bool IsResearchCompleted(ResearchProjectDef research)
        {
            if (research == null)
                return true;

            // Check cache first
            if (researchCache != null && researchCache.TryGetValue(research.defName, out bool cached))
            {
                return cached;
            }

            // Check if research is finished
            bool completed = research.IsFinished;
            
            // Update cache
            if (researchCache != null)
            {
                researchCache[research.defName] = completed;
            }

            return completed;
        }

        /// <summary>
        /// Get the area type key for an area.
        /// </summary>
        /// <param name="area">The area</param>
        /// <returns>The area type key</returns>
        private static string GetAreaKey(Area area)
        {
            if (area == null)
                return null;

            // Check area label - Rimworld areas are identified by their labels
            string label = area.Label?.ToLower() ?? "";
            
            // Home area
            if (label == "home" || area == Find.CurrentMap?.areaManager?.Home)
                return "Home";
            
            // Stockpile areas
            if (label.Contains("stockpile"))
                return "Stockpile";
            
            // Growing zones
            if (label.Contains("growing"))
                return "Growing";
            
            // Animal sleeping areas
            if (label.Contains("animal") && label.Contains("sleeping"))
                return "AnimalSleeping";
            
            // Animal allowed areas
            if (label.Contains("animal") && label.Contains("allowed"))
                return "AnimalAllowed";
            
            // No roof areas
            if (label.Contains("no roof") || label.Contains("noroof"))
                return "NoRoof";

            // Check if this is a zone-based area by checking the map's zones
            // Note: This might not work in all contexts, but it's a fallback
            try
            {
                Map map = Find.CurrentMap;
                if (map != null && map.zoneManager != null)
                {
                    foreach (Zone zone in map.zoneManager.AllZones)
                    {
                        if (zone is Zone_Stockpile stockpile && stockpile.label == area.Label)
                            return "Stockpile";
                        if (zone is Zone_Growing growing && growing.label == area.Label)
                            return "Growing";
                    }
                }
            }
            catch
            {
                // If we can't access the map, continue with default logic
            }

            // Default: treat as custom allowed area (requires Allowed research)
            return "Allowed";
        }

        /// <summary>
        /// Clear the research completion cache.
        /// </summary>
        public static void ClearCache()
        {
            if (researchCache != null)
            {
                researchCache.Clear();
            }
            cacheInitialized = false;
        }

        /// <summary>
        /// Refresh the research completion cache.
        /// </summary>
        public static void RefreshCache()
        {
            ClearCache();
            if (areaResearchMap != null)
            {
                foreach (var kvp in areaResearchMap)
                {
                    if (kvp.Value != null)
                    {
                        researchCache[kvp.Value.defName] = kvp.Value.IsFinished;
                    }
                }
            }
            cacheInitialized = true;
        }
    }
}
