# ResearchAreas Mod - AI Coding Agent Instructions

## Project Overview
ResearchAreas is a RimWorld 1.6 mod that gates area/zone creation (Stockpiles, Growing Zones, Animal Areas, etc.) behind research projects. The core mechanism: Harmony patches intercept area/zone registration, validate research completion via `ResearchChecker`, and block creation with user feedback if required research is incomplete.

## Architecture Pattern: Research Gating Pipeline

The mod follows a specific flow:
1. **User attempts area creation** → 2. **Harmony Prefix patch fires** → 3. **ResearchChecker validates** → 4. **Block (return false) or allow (return true)**

Key components:
- **ResearchChecker** (static utility): Central validation hub. Maintains `areaResearchMap` (area type → ResearchProjectDef) and caches research completion status. Always call `RefreshCache()` after research completion events.
- **ResearchAreasGameComponent** (GameComponent): Lifecycle handler. Calls `AreaRemover.ValidateAllMaps()` on load to purge areas lacking research. Manages periodic cache refresh.
- **Harmony Patches** (ZoneManagerPatches, UIPatches): `ZoneManager.RegisterZone()` Prefix blocks zone creation. UI Postfixes add tooltips/lock icons for UX feedback.
- **AreaRemover**: Scans all maps on load, identifies areas without required research (skips Home), removes them, notifies player.
- **ResearchAreasSettings**: Persists via `ExposeData()` using RimWorld's `Scribe_*` system. User-configurable toggles stored here.

## Adding a New Area Type (Step-by-Step)

1. **Register mapping in ResearchChecker.cs**:
   ```csharp
   areaResearchMap.Add("YourAreaKey", DefDatabase<ResearchProjectDef>.GetNamed("ResearchAreas_YourKey", false));
   ```
   Use the area's C# class name (e.g., "Stockpile" for Zone_Stockpile, "Growing" for Zone_Growing) as the key.

2. **Create ResearchProjectDef in XML** (`Defs/ResearchProjectDef/AreaResearchProjects.xml`):
   ```xml
   <ResearchProjectDef>
       <defName>ResearchAreas_YourKey</defName>
       <label>Your Research Label</label>
       <baseCost>250</baseCost>
       <techLevel>Neolithic</techLevel>
   </ResearchProjectDef>
   ```

3. **Add Harmony patch if needed** (e.g., for custom zone types not covered by `Zone_Stockpile`/`Zone_Growing`):
   ```csharp
   if (newZone is YourCustomZone) {
       areaKey = "YourAreaKey";
       requiredResearch = Core.ResearchChecker.GetRequiredResearch(areaKey);
   }
   ```

## Critical Conventions

### Caching
ResearchChecker maintains:
- `researchCache`: bool map of research def names → completion status
- `areaTypeCache` / `zoneTypeCache`: instance → type key mappings

**Never assume cache is current.** Always call `ResearchChecker.RefreshCache()` after:
- Research completion (via GameComponent.GameComponentTick or event patches)
- Game load (ResearchAreasGameComponent.FinalizeInit)
- Testing in development

### Harmony Prefix Returns
- `return true` → allow original method to execute
- `return false` → block and skip original method (used to prevent invalid area creation)

Return false + `Messages.Message()` for user feedback.

### Settings Persistence
All configuration lives in ResearchAreasSettings. Use `ExposeData()` pattern:
```csharp
Scribe_Values.Look(ref boolField, "xmlKey", defaultValue);
```
RimWorld handles the XML save/load transparently.

## Mod Data (XML) Structure
- Research defs: `Defs/ResearchProjectDef/AreaResearchProjects.xml`
- Game component registration: `Defs/GameComponentDef/ResearchAreasGameComponent.xml`
- These are RimWorld content files, NOT in the C# project

## Build & Test
- **Build**: `dotnet build` in Source/ → outputs to `Assemblies/ResearchAreas.dll`
- **Run**: Load in RimWorld with Harmony mod enabled
- **Debug**: Use `Log.Message()` and RimWorld's in-game debug mode (Ctrl+F12)
- **Cache issues**: If behavior seems stale, call `ResearchChecker.RefreshCache()` manually
