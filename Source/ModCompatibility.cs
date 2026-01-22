using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ResearchAreas.Compatibility
{
    /// <summary>
    /// Compatibility layer for popular area mods.
    /// </summary>
    public static class ModCompatibility
    {
        private static Dictionary<string, string> modAreaMappings = new Dictionary<string, string>();

        /// <summary>
        /// Initialize compatibility mappings for popular mods.
        /// </summary>
        public static void Initialize()
        {
            // Check for popular area mods and register their area types
            // This is a framework that can be extended

            // Example: If a mod adds custom zones, we can map them here
            // modAreaMappings["ModName_AreaType"] = "Stockpile"; // or "Growing", etc.
        }

        /// <summary>
        /// Register a custom area type from another mod.
        /// </summary>
        /// <param name="areaDefName">The defName of the area type</param>
        /// <param name="researchKey">The research key to require (e.g., "Stockpile", "Growing")</param>
        public static void RegisterModAreaType(string areaDefName, string researchKey)
        {
            if (!string.IsNullOrEmpty(areaDefName) && !string.IsNullOrEmpty(researchKey))
            {
                modAreaMappings[areaDefName] = researchKey;
            }
        }

        /// <summary>
        /// Get the research key for a modded area type.
        /// </summary>
        public static string GetResearchKeyForModArea(string areaDefName)
        {
            modAreaMappings.TryGetValue(areaDefName, out string researchKey);
            return researchKey;
        }

        /// <summary>
        /// Check if a modded area type is allowed.
        /// </summary>
        public static bool IsModAreaAllowed(string areaDefName)
        {
            string researchKey = GetResearchKeyForModArea(areaDefName);
            if (string.IsNullOrEmpty(researchKey))
                return true; // Unknown mod areas are allowed by default

            ResearchProjectDef requiredResearch = Core.ResearchChecker.GetRequiredResearch(researchKey);
            if (requiredResearch == null)
                return true;

            return Core.ResearchChecker.IsResearchCompleted(requiredResearch);
        }
    }

    /// <summary>
    /// Harmony patches for popular area mod compatibility.
    /// </summary>
    [HarmonyLib.HarmonyPatch]
    public static class ModCompatibilityPatches
    {
        /// <summary>
        /// Patch for mods that add custom zones.
        /// This is a template - specific patches would need to be added for each mod.
        /// </summary>
        // Example patch structure:
        // [HarmonyPatch(typeof(SomeModZoneClass), "RegisterZone")]
        // public static class SomeMod_RegisterZone_Patch
        // {
        //     static bool Prefix(Zone newZone)
        //     {
        //         if (newZone == null)
        //             return true;
        //
        //         string researchKey = ModCompatibility.GetResearchKeyForModArea(newZone.GetType().Name);
        //         if (!string.IsNullOrEmpty(researchKey))
        //         {
        //             ResearchProjectDef requiredResearch = Core.ResearchChecker.GetRequiredResearch(researchKey);
        //             if (requiredResearch != null && !Core.ResearchChecker.IsResearchCompleted(requiredResearch))
        //             {
        //                 Messages.Message($"Cannot create zone. Requires research: {requiredResearch.label}.", MessageTypeDefOf.RejectInput);
        //                 return false;
        //             }
        //         }
        //
        //         return true;
        //     }
        // }
    }
}
