using HarmonyLib;
using RimWorld;
using Verse;

namespace ResearchAreas.HarmonyPatches
{
    /// <summary>
    /// Harmony patches for AreaManager to intercept area creation and validate research requirements.
    /// Note: Area creation primarily occurs through ZoneManager (RegisterZone) and designators.
    /// This class is reserved for AreaManager-specific patches if needed.
    /// </summary>
    [StaticConstructorOnStartup]
    public static class AreaManagerPatches
    {
        static AreaManagerPatches()
        {
            var harmony = new Harmony("dyscopia.ResearchAreas");
            harmony.PatchAll();
        }

        // Area creation is handled via ZoneManager.RegisterZone() in ZoneManagerPatches.cs
        // and through designator-based creation patched in ResearchUIPatches.cs
    }
}
