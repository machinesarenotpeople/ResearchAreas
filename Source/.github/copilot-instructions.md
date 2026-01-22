# ResearchAreas Mod - AI Coding Agent Instructions

## Project Overview
ResearchAreas is a RimWorld mod that gates vanilla area types (Stockpiles, Growing Zones, Animal Areas, Home Zones, etc.) behind research requirements. The mod uses Harmony patching to intercept area creation and validate research prerequisites before allowing players to create new areas.

## Architecture & Key Components

### Core Components
- **ResearchChecker** ([ResearchChecker.cs](ResearchChecker.cs)): Static utility that maintains area-to-research mappings and manages caching. Provides `IsAreaAllowed()` and `GetRequiredResearch()` methods.
- **ResearchAreasGameComponent** ([ResearchAreasGameComponent.cs](ResearchAreasGameComponent.cs)): Game lifecycle handler that initializes on load, validates areas, refreshes caches, and shows compatibility dialogs.
- **ResearchAreasSettings** ([ResearchAreasSettings.cs](ResearchAreasSettings.cs)): Mod settings stored via RimWorld's `ModSettings` system. Includes toggles for research requirements per area type and removal behaviors.

### Harmony Patching System
The mod patches RimWorld's area management:
- **ZoneManagerPatches** ([ZoneManagerPatches.cs](ZoneManagerPatches.cs)): Patches `ZoneManager.RegisterZone()` to block creation of zones (Stockpiles, Growing, etc.) without required research.
- **AreaManagerPatches** ([AreaManagerPatches.cs](AreaManagerPatches.cs)): Reserved for AreaManager-specific patches; area creation primarily occurs through `ZoneManager.RegisterZone()`.
- **UIPatches** ([UIPatches.cs](UIPatches.cs)): Adds tooltips and visual indicators (lock icons, grayed-out buttons) for locked areas.

### Supporting Utilities
- **AreaRemover** ([AreaRemover.cs](AreaRemover.cs)): Removes invalid areas on game load based on settings.
- **ModCompatibility** ([ModCompatibility.cs](ModCompatibility.cs)): Framework for registering custom area types from other mods.
- **CompatibilityDialog** ([CompatibilityDialog.cs](CompatibilityDialog.cs)): Shows save file compatibility warnings.
- **CustomAreaMappingDialog** ([CustomAreaMappingDialog.cs](CustomAreaMappingDialog.cs)): UI for mapping mod-added area types to research.
- **TooltipHandler** ([TooltipHandler.cs](TooltipHandler.cs)): Renders research requirement tooltips.

## Critical Patterns & Conventions

### Area-to-Research Mapping
Located in `ResearchChecker.InitializeMapping()`. Maps area type keys (e.g., "Stockpile", "Growing") to `ResearchProjectDef` names:
```csharp
{ "Stockpile", DefDatabase<ResearchProjectDef>.GetNamed("ResearchAreas_Stockpiles", false) }
```
New area types must be registered here AND have corresponding research defs in XML mod files.

### Caching Strategy
ResearchChecker maintains two caches to avoid repeated lookups:
- `researchCache`: Maps research project names to completion status
- `areaTypeCache` & `zoneTypeCache`: Maps area/zone instances to type keys
Call `ResearchChecker.RefreshCache()` after research completion or game load to update cached state.

### Harmony Patch Pattern
All patches use `[HarmonyPatch]` attributes with `Prefix` to **intercept and prevent** invalid area creation:
```csharp
static bool Prefix(...) { return true/false; } // false blocks execution
```
Return `false` to deny area creation and show `Messages.Message()` feedback.

### Settings & Save Data
Settings persist via `ResearchAreasSettings.ExposeData()` using RimWorld's `Scribe_*` system. All boolean toggles must be explicitly saved/loaded.

## Development Workflows

### Adding a New Area Type
1. Add mapping in `ResearchChecker.InitializeMapping()` with area key and research def name
2. Create research XML definition (mod data, not in code)
3. Add setting toggle in `ResearchAreasSettings` if user-configurable
4. Add UI elements in settings dialog if needed
5. Test with `ResearchChecker.RefreshCache()` after research completion

### Debugging Patches
- Use `[HarmonyPatch]` with specific `typeof()` and method name
- Test with `Harmony harmony = new Harmony("dyscopia.ResearchAreas"); harmony.PatchAll();`
- Log area types with `Log.Message($"Area: {area.GetType().Name}")` to identify type keys

### Compatibility with Mod-Added Areas
Use `ModCompatibility.RegisterModAreaType(defName, researchKey)` to support third-party area mods. Extend `ModCompatibilityPatches` with new `[HarmonyPatch]` entries for specific mods.

## External Dependencies
- **RimWorld API**: `Verse`, `RimWorld` namespaces (game framework)
- **HarmonyLib**: For runtime patching (included in mod package)
- **.NET Framework 4.7.2** (RimWorld standard)

## Project Structure Note
- Build output: `bin/Debug/` and `bin/Release/`
- RimWorld mod data: Expected in separate XML directory (not in this C# project)
- Solution file: `ResearchAreas.slnx` (Visual Studio)
