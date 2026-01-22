using HarmonyLib;
using RimWorld;
using Verse;

namespace ResearchAreas.HarmonyPatches
{
    /// <summary>
    /// Harmony patches for AreaManager to intercept area creation and validate research requirements.
    /// </summary>
    [StaticConstructorOnStartup]
    public static class AreaManagerPatches
    {
        static AreaManagerPatches()
        {
            var harmony = new Harmony("dyscopia.ResearchAreas");
            harmony.PatchAll();
        }

        /// <summary>
        /// Patch AreaManager.Add() to check research before allowing area creation.
        /// </summary>
        [HarmonyPatch(typeof(AreaManager), nameof(AreaManager.Add))]
        public static class AreaManager_Add_Patch
        {
            static bool Prefix(Area area, ref bool __result)
            {
                if (area == null)
                {
                    __result = false;
                    return false;
                }

                // Check if area is allowed based on research
                if (!Core.ResearchChecker.IsAreaAllowed(area))
                {
                    ResearchProjectDef requiredResearch = Core.ResearchChecker.GetRequiredResearch(area);
                    string message = requiredResearch != null
                        ? $"Cannot create area. Requires research: {requiredResearch.label}."
                        : "Cannot create area. Required research not found.";

                    Messages.Message(message, MessageTypeDefOf.RejectInput);
                    __result = false;
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Patch AreaManager.TryMakeNewAllowed() to check research before creating new allowed areas.
        /// </summary>
        [HarmonyPatch(typeof(AreaManager), nameof(AreaManager.TryMakeNewAllowed))]
        public static class AreaManager_TryMakeNewAllowed_Patch
        {
            static bool Prefix(ref Area __result)
            {
                // Create a temporary area to check research requirements
                // We'll check the research for custom allowed areas
                ResearchProjectDef requiredResearch = Core.ResearchChecker.GetRequiredResearch("Allowed");
                
                if (requiredResearch != null && !Core.ResearchChecker.IsResearchCompleted(requiredResearch))
                {
                    string message = $"Cannot create allowed area. Requires research: {requiredResearch.label}.";
                    Messages.Message(message, MessageTypeDefOf.RejectInput);
                    __result = null;
                    return false;
                }

                return true;
            }
        }
    }
}
