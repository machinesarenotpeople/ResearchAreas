# ResearchAreas Mod - AI Coding Agent Instructions

## Project Overview
ResearchAreas is a RimWorld 1.6 mod that gates zone creation (Stockpiles, Growing Zones) behind research projects. The core mechanism: Harmony patches intercept zone registration via `ZoneManager.RegisterZone()`, validate research completion via `ResearchChecker`, and block creation with user feedback if required research is incomplete.

## Current Status

**✅ WORKING:**
- Stockpile Zone research gating
- Growing Zone research gating

**❌ NOT WORKING (Requires RimWorld Assembly Decompilation):**
- Home Area, Allowed Area, NoRoof Area (designator methods/classes undefined in RimWorld 1.6)
- Animal Zones (do not exist in RimWorld 1.6)

## Architecture Pattern: Research Gating Pipeline

The mod follows a specific flow for zones:
1. **User designates zone** → 2. **ZoneManager.RegisterZone() called** → 3. **Harmony Prefix patch fires** → 4. **ResearchChecker validates** → 5. **Block (return false) or allow (return true)**

### Core Components

- **ResearchChecker** (`Source/ResearchChecker.cs`): Central validation hub. Maintains `areaResearchMap` (area type string → ResearchProjectDef) and caches research completion status via `researchCache`.
  - Key methods: `GetRequiredResearch(areaKey)`, `IsResearchCompleted(research)`, `RefreshCache()`
  - Always call `RefreshCache()` after research completion events

- **ResearchAreasGameComponent** (`Source/ResearchAreasGameComponent.cs`): Lifecycle handler.
  - `FinalizeInit()`: Called on game load; calls `AreaRemover.ValidateAllMaps()` to purge zones lacking research

- **ZoneManagerPatches** (`Source/ZoneManagerPatches.cs`): Main Harmony patch.
  - Patches: `ZoneManager.RegisterZone()` with Prefix
  - Checks incoming zone type (Zone_Stockpile, Zone_Growing)
  - Blocks creation (return false) if research incomplete; allows (return true) otherwise

- **AreaRemover** (`Source/AreaRemover.cs`): Cleanup on load.
  - Scans all zones on all maps
  - Removes zones lacking required research
  - Sends player notification

- **ResearchAreasSettings** (`Source/ResearchAreasSettings.cs`): Configuration persistence.
  - Per-area toggles (requireStockpileResearch, requireGrowingResearch, etc.)
  - Persisted via `ExposeData()` using RimWorld's `Scribe_Values.Look(ref field, "xmlKey", default)`

## Adding Zone Types (For Future Work)

To gate additional zone types:

1. **Register mapping in ResearchChecker.cs** `InitializeMapping()`:
   ```csharp
   areaResearchMap.Add("YourZoneKey", DefDatabase<ResearchProjectDef>.GetNamed("ResearchAreas_YourKey", false));
   ```
   Use the zone's C# class name (e.g., "Stockpile" for Zone_Stockpile) as the key.

2. **Create ResearchProjectDef** in `Defs/ResearchProjectDef/AreaResearchProjects.xml`:
   ```xml
   <ResearchProjectDef>
       <defName>ResearchAreas_YourKey</defName>
       <label>Your Zone Research Label</label>
       <baseCost>250</baseCost>
       <techLevel>Neolithic</techLevel>
       <tab>ResearchAreas_AreaManagement</tab>
   </ResearchProjectDef>
   ```

3. **Add zone type check in ZoneManagerPatches.cs** `Prefix()` method:
   ```csharp
   else if (newZone is Zone_YourType) {
       areaKey = "YourZoneKey";
       requiredResearch = Core.ResearchChecker.GetRequiredResearch(areaKey);
   }
   ```

## Adding Area Types (Requires Decompilation)

For non-zone areas (Home, Allowed, NoRoof), the designator method names could not be found through normal means.

**Required Next Steps:**
1. Decompile `Assembly-CSharp.dll` using dnSpy or ILSpy
2. Search for classes: `Designator_AreaAllowedGrow`, `Designator_AreaNoRoof`, `Designator_AreaHomeToggle`
3. Identify the method called when these designators create/modify areas
4. Update AreaManagerPatches.cs with the correct class and method names
5. Implement Prefix patches similar to the zone patches

**Known Challenge**: RimWorld 1.6's designator API differs significantly from earlier versions. Direct patching points are not documented.

## Critical Conventions

### Caching
ResearchChecker maintains two caches:
- `researchCache`: Map of research def names → bool (completion status)
- `areaTypeCache`: Zone instance → string (area type key)

**Cache Staleness Risk**: Cache is initialized on load and only refreshed explicitly.
- Always call `ResearchChecker.RefreshCache()` after research events
- GameComponent calls this on load via `FinalizeInit()`

### Harmony Prefix Returns
- `return true` → Allow original method to execute
- `return false` → Block/skip original method (prevents zone creation)

When blocking, pair with `Messages.Message()` for user feedback.

### Settings Persistence Pattern
Use RimWorld's `Scribe_*` system in `ExposeData()`:
```csharp
Scribe_Values.Look(ref requireStockpileResearch, "requireStockpileResearch", true);
```
RimWorld handles XML serialization automatically.

## Mod Data (XML) Structure
- Research definitions: `Defs/ResearchProjectDef/AreaResearchProjects.xml`
- Research tab definition: `Defs/ResearchTabDef/ResearchTabs.xml` (custom "Area Management" tab)
- These are RimWorld content files (NOT in Source/); loaded by RimWorld on startup

## Build & Test Workflow
- **Build**: `dotnet build` in `Source/` → outputs to `Assemblies/ResearchAreas.dll`
- **Run**: Load mod in RimWorld (requires Harmony mod as dependency)
- **Debug**: Use `Log.Message()` for console output; RimWorld debug mode (Ctrl+F12) for in-game inspection
- **Cache debugging**: Call `ResearchChecker.RefreshCache()` manually if behavior seems stale
- **Harmony errors**: Check RimWorld log if patch fails to apply; verify method signatures match exactly

## Known Limitations (RimWorld 1.6 API Constraints)
- **Animal zone types**: `Zone_AnimalSleeping` and `Zone_AnimalAllowed` do not appear to exist as separate classes in RimWorld 1.6
- **Area designators**: Home, Allowed, NoRoof areas use designator-based creation, not `ZoneManager.RegisterZone()`
- **Home area**: Intentionally NOT gated (always available per RimWorld convention)
- **UI patches disabled**: Method signatures on designators/research UI don't match RimWorld 1.6 API expectations

## Development References
- RimWorld 1.6.4633 rev1261
- .NET Framework 4.7.2
- HarmonyLib 0Harmony (from Steam Workshop)
- Target assembly: `Assembly-CSharp.dll` (RimWorld core)
