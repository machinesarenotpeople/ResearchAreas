using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace ResearchAreas.Core
{
    /// <summary>
    /// System to remove areas when research becomes locked.
    /// </summary>
    public static class AreaRemover
    {
        /// <summary>
        /// Remove all areas that don't have completed research.
        /// </summary>
        /// <param name="map">The map to check areas on</param>
        /// <returns>List of removed area labels</returns>
        public static List<string> RemoveInvalidAreas(Map map)
        {
            if (map == null || map.areaManager == null)
                return new List<string>();

            List<string> removedAreas = new List<string>();
            List<Area> areasToRemove = new List<Area>();

            // Check all areas except Home (Home is special and should always exist)
            foreach (Area area in map.areaManager.AllAreas)
            {
                // Skip Home area - it's always allowed
                if (area == map.areaManager.Home)
                    continue;

                // Check if area is allowed based on research
                if (!ResearchChecker.IsAreaAllowed(area))
                {
                    areasToRemove.Add(area);
                }
            }

            // Remove invalid areas
            foreach (Area area in areasToRemove)
            {
                string areaLabel = area.Label;
                
                // Check if area is in use
                if (IsAreaInUse(map, area))
                {
                    // Still remove, but log a warning
                    Log.Warning($"ResearchAreas: Removing area '{areaLabel}' that may be in use.");
                }

                // Remove the area
                map.areaManager.Remove(area);
                removedAreas.Add(areaLabel);
            }

            return removedAreas;
        }

        /// <summary>
        /// Remove all zones that don't have completed research.
        /// </summary>
        /// <param name="map">The map to check zones on</param>
        /// <returns>List of removed zone labels</returns>
        public static List<string> RemoveInvalidZones(Map map)
        {
            if (map == null || map.zoneManager == null)
                return new List<string>();

            List<string> removedZones = new List<string>();
            List<Zone> zonesToRemove = new List<Zone>();

            // Check all zones
            foreach (Zone zone in map.zoneManager.AllZones)
            {
                ResearchProjectDef requiredResearch = null;
                string zoneType = null;

                if (zone is Zone_Stockpile)
                {
                    zoneType = "Stockpile";
                    requiredResearch = ResearchChecker.GetRequiredResearch(zoneType);
                }
                else if (zone is Zone_Growing)
                {
                    zoneType = "Growing";
                    requiredResearch = ResearchChecker.GetRequiredResearch(zoneType);
                }

                if (requiredResearch != null && !ResearchChecker.IsResearchCompleted(requiredResearch))
                {
                    zonesToRemove.Add(zone);
                }
            }

            // Remove invalid zones
            foreach (Zone zone in zonesToRemove)
            {
                string zoneLabel = zone.label;
                
                // Check if zone has items/plants
                if (zone.AllContainedThings.Any())
                {
                    Log.Warning($"ResearchAreas: Removing zone '{zoneLabel}' that contains items.");
                }

                // Remove the zone
                map.zoneManager.RemoveZone(zone);
                removedZones.Add(zoneLabel);
            }

            return removedZones;
        }

        /// <summary>
        /// Check if an area is currently in use.
        /// </summary>
        /// <param name="map">The map</param>
        /// <param name="area">The area to check</param>
        /// <returns>True if the area is in use</returns>
        private static bool IsAreaInUse(Map map, Area area)
        {
            if (map == null || area == null)
                return false;

            // Check if any pawns are assigned to this area
            foreach (Pawn pawn in map.mapPawns.AllPawnsSpawned)
            {
                if (pawn.playerSettings != null && pawn.playerSettings.AreaRestriction == area)
                {
                    return true;
                }
            }

            // Check if any zones are using this area
            foreach (Zone zone in map.zoneManager.AllZones)
            {
                if (zone is Zone_Stockpile stockpile && stockpile.GetZoneLabel() == area.Label)
                {
                    // Check if stockpile has items
                    if (stockpile.AllContainedThings.Any())
                    {
                        return true;
                    }
                }
                else if (zone is Zone_Growing growing && growing.GetZoneLabel() == area.Label)
                {
                    // Check if growing zone has plants
                    if (growing.AllContainedThings.Any())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Validate and remove invalid areas and zones on all maps.
        /// </summary>
        /// <returns>Dictionary mapping map names to lists of removed area/zone labels</returns>
        public static Dictionary<string, List<string>> ValidateAllMaps()
        {
            Dictionary<string, List<string>> results = new Dictionary<string, List<string>>();

            if (Current.Game == null || Current.Game.Maps == null)
                return results;

            foreach (Map map in Current.Game.Maps)
            {
                if (map != null)
                {
                    List<string> removed = new List<string>();
                    
                    // Remove invalid areas
                    if (map.areaManager != null)
                    {
                        removed.AddRange(RemoveInvalidAreas(map));
                    }
                    
                    // Remove invalid zones
                    if (map.zoneManager != null)
                    {
                        removed.AddRange(RemoveInvalidZones(map));
                    }
                    
                    if (removed.Count > 0)
                    {
                        results[map.Parent?.Label ?? "Unknown Map"] = removed;
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Notify the player about removed areas.
        /// </summary>
        /// <param name="removedAreas">Dictionary of removed areas by map</param>
        public static void NotifyPlayer(Dictionary<string, List<string>> removedAreas)
        {
            if (removedAreas == null || removedAreas.Count == 0)
                return;

            foreach (var kvp in removedAreas)
            {
                string mapName = kvp.Key;
                List<string> areas = kvp.Value;

                if (areas.Count > 0)
                {
                    string message = $"ResearchAreas: Removed {areas.Count} area(s) from {mapName} due to missing research: {string.Join(", ", areas)}";
                    Messages.Message(message, MessageTypeDefOf.NeutralEvent);
                }
            }
        }
    }
}
