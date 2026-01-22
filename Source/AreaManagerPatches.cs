using HarmonyLib;
using RimWorld;
using Verse;

namespace ResearchAreas.HarmonyPatches
{
    /// <summary>
    /// Harmony patches for area creation through designators.
    /// Areas are created via DesignateSingleCell on area designators.
    /// </summary>
    [StaticConstructorOnStartup]
    public static class AreaManagerPatches
    {
        static AreaManagerPatches()
        {
            var harmony = new Harmony("dyscopia.ResearchAreas");
            harmony.PatchAll();
        }
    }

    /// <summary>
    /// Patch Designator_AreaAllowedExpand.DesignateSingleCell to check research.
    /// </summary>
    [HarmonyPatch(typeof(Designator_AreaAllowedExpand), nameof(Designator_AreaAllowedExpand.DesignateSingleCell))]
    public static class Designator_AreaAllowedExpand_Patch
    {
        static bool Prefix(IntVec3 c)
        {
            ResearchProjectDef requiredResearch = Core.ResearchChecker.GetRequiredResearch("Allowed");
            if (requiredResearch != null && !Core.ResearchChecker.IsResearchCompleted(requiredResearch))
            {
                Messages.Message("Zoning research needed.", MessageTypeDefOf.RejectInput);
                return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Patch Designator_AreaAllowedClear.DesignateSingleCell to check research.
    /// </summary>
    [HarmonyPatch(typeof(Designator_AreaAllowedClear), nameof(Designator_AreaAllowedClear.DesignateSingleCell))]
    public static class Designator_AreaAllowedClear_Patch
    {
        static bool Prefix(IntVec3 c)
        {
            ResearchProjectDef requiredResearch = Core.ResearchChecker.GetRequiredResearch("Allowed");
            if (requiredResearch != null && !Core.ResearchChecker.IsResearchCompleted(requiredResearch))
            {
                Messages.Message("Zoning research needed.", MessageTypeDefOf.RejectInput);
                return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Patch Designator_AreaHome.DesignateSingleCell to check research.
    /// This patches the abstract base, catching both add and clear home area operations.
    /// </summary>
    [HarmonyPatch(typeof(Designator_AreaHome), nameof(Designator_AreaHome.DesignateSingleCell))]
    public static class Designator_AreaHome_Patch
    {
        static bool Prefix(IntVec3 c)
        {
            ResearchProjectDef requiredResearch = Core.ResearchChecker.GetRequiredResearch("Home");
            if (requiredResearch != null && !Core.ResearchChecker.IsResearchCompleted(requiredResearch))
            {
                Messages.Message("Zoning research needed.", MessageTypeDefOf.RejectInput);
                return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Patch Designator_AreaNoRoof.DesignateSingleCell to check research for roof management.
    /// </summary>
    [HarmonyPatch(typeof(Designator_AreaNoRoof), nameof(Designator_AreaNoRoof.DesignateSingleCell))]
    public static class Designator_AreaNoRoof_Patch
    {
        static bool Prefix(IntVec3 c)
        {
            ResearchProjectDef requiredResearch = Core.ResearchChecker.GetRequiredResearch("NoRoof");
            if (requiredResearch != null && !Core.ResearchChecker.IsResearchCompleted(requiredResearch))
            {
                Messages.Message("Zoning research needed.", MessageTypeDefOf.RejectInput);
                return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Patch Designator_AreaBuildRoof.DesignateSingleCell to check research for roof management.
    /// </summary>
    [HarmonyPatch(typeof(Designator_AreaBuildRoof), nameof(Designator_AreaBuildRoof.DesignateSingleCell))]
    public static class Designator_AreaBuildRoof_Patch
    {
        static bool Prefix(IntVec3 c)
        {
            ResearchProjectDef requiredResearch = Core.ResearchChecker.GetRequiredResearch("NoRoof");
            if (requiredResearch != null && !Core.ResearchChecker.IsResearchCompleted(requiredResearch))
            {
                Messages.Message("Zoning research needed.", MessageTypeDefOf.RejectInput);
                return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Patch Designator_AreaIgnoreRoof.DesignateSingleCell to check research for roof management.
    /// </summary>
    [HarmonyPatch(typeof(Designator_AreaIgnoreRoof), nameof(Designator_AreaIgnoreRoof.DesignateSingleCell))]
    public static class Designator_AreaIgnoreRoof_Patch
    {
        static bool Prefix(IntVec3 c)
        {
            ResearchProjectDef requiredResearch = Core.ResearchChecker.GetRequiredResearch("NoRoof");
            if (requiredResearch != null && !Core.ResearchChecker.IsResearchCompleted(requiredResearch))
            {
                Messages.Message("Zoning research needed.", MessageTypeDefOf.RejectInput);
                return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Patch Designator_AreaSnowClear.DesignateSingleCell to check research for snow/sand removal.
    /// This patches the abstract base, catching both add and remove snow clear operations.
    /// </summary>
    [HarmonyPatch(typeof(Designator_AreaSnowClear), nameof(Designator_AreaSnowClear.DesignateSingleCell))]
    public static class Designator_AreaSnowClear_Patch
    {
        static bool Prefix(IntVec3 c)
        {
            ResearchProjectDef requiredResearch = Core.ResearchChecker.GetRequiredResearch("SnowClear");
            if (requiredResearch != null && !Core.ResearchChecker.IsResearchCompleted(requiredResearch))
            {
                Messages.Message("Zoning research needed.", MessageTypeDefOf.RejectInput);
                return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Patch Designator_AreaPollutionClear.DesignateSingleCell to check research for pollution removal.
    /// This patches the abstract base, catching both add and remove pollution clear operations.
    /// </summary>
    [HarmonyPatch(typeof(Designator_AreaPollutionClear), nameof(Designator_AreaPollutionClear.DesignateSingleCell))]
    public static class Designator_AreaPollutionClear_Patch
    {
        static bool Prefix(IntVec3 c)
        {
            ResearchProjectDef requiredResearch = Core.ResearchChecker.GetRequiredResearch("Pollution");
            if (requiredResearch != null && !Core.ResearchChecker.IsResearchCompleted(requiredResearch))
            {
                Messages.Message("Zoning research needed.", MessageTypeDefOf.RejectInput);
                return false;
            }
            return true;
        }
    }
}
