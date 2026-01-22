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
        
        // Cache zone-to-area mappings for performance
        private static Dictionary<Area, string> areaTypeCache = new Dictionary<Area, string>();
        private static Dictionary<Zone, string> zoneTypeCache = new Dictionary<Zone, string>();
        private static int lastMapUpdateTick = -1;

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

            // Home area is always allowed - it's a special system area
            Map map = Find.CurrentMap;
            if (map != null && map.areaManager != null && area == map.areaManager.Home)
                return true;

            string areaKey = GetAreaKey(area);
            if (string.IsNullOrEmpty(areaKey))
                return true; // Unknown area types are allowed by default

            // Check if research is required via settings
            if (Settings.ResearchAreasMod.Settings != null && !IsResearchRequired(areaKey))
                return true;

            ResearchProjectDef requiredResearch = GetRequiredResearch(areaKey);
            if (requiredResearch == null)
                return true; // No research required for this area type

            return IsResearchCompleted(requiredResearch);
        }

        /// <summary>
        /// Check if research is required for an area type based on settings.
        /// </summary>
        private static bool IsResearchRequired(string areaKey)
        {
            if (Settings.ResearchAreasMod.Settings == null)
                return true; // Default to requiring research if settings not loaded

            var settings = Settings.ResearchAreasMod.Settings;
            
            switch (areaKey)
            {
                case "Stockpile":
                    return settings.requireStockpileResearch;
                case "Growing":
                    return settings.requireGrowingResearch;
                case "AnimalSleeping":
                case "AnimalAllowed":
                    return settings.requireAnimalResearch;
                case "Home":
                    return settings.requireHomeResearch;
                case "NoRoof":
                    return settings.requireNoRoofResearch;
                case "Allowed":
                    return settings.requireAllowedResearch;
                default:
                    return true;
            }
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
        /// Get the area type key for an area (with caching).
        /// Prioritizes zone type checking over label matching for better reliability.
        /// </summary>
        /// <param name="area">The area</param>
        /// <returns>The area type key</returns>
        private static string GetAreaKey(Area area)
        {
            if (area == null)
                return null;

            // Check cache first
            if (areaTypeCache.TryGetValue(area, out string cachedKey))
            {
                return cachedKey;
            }

            Map map = Find.CurrentMap;
            if (map == null)
                return null;

            // Home area is handled separately in IsAreaAllowed
            if (map.areaManager != null && area == map.areaManager.Home)
            {
                areaTypeCache[area] = "Home";
                return "Home";
            }

            string areaKey = null;
            string label = area.Label?.ToLower() ?? "";

            // Step 1: Check custom area name mappings first (highest priority)
            if (Settings.ResearchAreasMod.Settings != null && 
                Settings.ResearchAreasMod.Settings.customAreaMappings != null &&
                Settings.ResearchAreasMod.Settings.customAreaMappings.Count > 0)
            {
                if (Settings.ResearchAreasMod.Settings.customAreaMappings.TryGetValue(label, out string mappedType))
                {
                    areaTypeCache[area] = mappedType;
                    return mappedType;
                }
            }

            // Step 2: Check zone types directly (most reliable method)
            if (map.zoneManager != null)
            {
                try
                {
                    // Update zone cache if needed
                    UpdateZoneCache(map);
                    
                    // Check if this area is associated with a zone by matching label
                    foreach (Zone zone in map.zoneManager.AllZones)
                    {
                        if (zone.label == area.Label)
                        {
                            // Check zone type cache first
                            if (zoneTypeCache.TryGetValue(zone, out string zoneKey))
                            {
                                areaKey = zoneKey;
                                break;
                            }
                            
                            // Check zone type directly
                            if (zone is Zone_Stockpile)
                            {
                                areaKey = "Stockpile";
                                zoneTypeCache[zone] = "Stockpile";
                                break;
                            }
                            if (zone is Zone_Growing)
                            {
                                areaKey = "Growing";
                                zoneTypeCache[zone] = "Growing";
                                break;
                            }
                        }
                    }
                }
                catch
                {
                    // If we can't access zones, continue with label-based detection
                }
            }

            // Step 3: Fall back to label-based detection (less reliable but necessary for areas without zones)
            if (areaKey == null)
            {
                // Home area (by label)
                if (label == "home")
                    areaKey = "Home";
                // Stockpile areas
                else if (label.Contains("stockpile"))
                    areaKey = "Stockpile";
                // Growing zones
                else if (label.Contains("growing"))
                    areaKey = "Growing";
                // Animal sleeping areas
                else if (label.Contains("animal") && label.Contains("sleeping"))
                    areaKey = "AnimalSleeping";
                // Animal allowed areas
                else if (label.Contains("animal") && label.Contains("allowed"))
                    areaKey = "AnimalAllowed";
                // No roof areas
                else if (label.Contains("no roof") || label.Contains("noroof") || label.Contains("no-roof"))
                    areaKey = "NoRoof";
            }

            // Step 4: Default to custom allowed area if still unknown
            if (areaKey == null)
                areaKey = "Allowed";

            // Cache the result
            areaTypeCache[area] = areaKey;
            return areaKey;
        }

        /// <summary>
        /// Update the zone type cache for a map.
        /// </summary>
        private static void UpdateZoneCache(Map map)
        {
            if (map == null || map.zoneManager == null)
                return;

            int currentTick = Find.TickManager?.TicksGame ?? 0;
            
            // Only update cache if map has changed or cache is stale
            if (lastMapUpdateTick == currentTick && zoneTypeCache.Count > 0)
                return;

            lastMapUpdateTick = currentTick;
            zoneTypeCache.Clear();

            foreach (Zone zone in map.zoneManager.AllZones)
            {
                if (zone is Zone_Stockpile)
                    zoneTypeCache[zone] = "Stockpile";
                else if (zone is Zone_Growing)
                    zoneTypeCache[zone] = "Growing";
            }
        }

        /// <summary>
        /// Clear all caches (call when areas/zones are added/removed or custom mappings change).
        /// </summary>
        public static void ClearCaches()
        {
            areaTypeCache.Clear();
            zoneTypeCache.Clear();
            lastMapUpdateTick = -1;
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
            ClearCaches();
        }

        /// <summary>
        /// Refresh the research completion cache.
        /// </summary>
        public static void RefreshCache()
        {
            if (researchCache == null)
            {
                researchCache = new Dictionary<string, bool>();
            }
            else
            {
                researchCache.Clear();
            }

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
        }
    }
}
