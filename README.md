# Research Areas

A RimWorld 1.6 mod that gates area and zone creation behind research projects, adding strategic depth to colony management.

## Overview

Research Areas locks the creation of different area types (stockpiles, growing zones, animal areas, etc.) behind research projects. This mod adds a progression system where players must research area management techniques before they can designate and use different types of zones and areas in their colonies.

## Features

- **Research-Gated Areas**: Each area type requires its own research project to unlock
- **Automatic Cleanup**: Existing areas without required research are automatically removed when loading a save
- **Zone Protection**: Prevents creation of stockpiles and growing zones without proper research
- **User Feedback**: Clear messages explain why area creation is blocked and what research is needed
- **Home Area Exception**: The Home area is always available (system area)

## Requirements

- **RimWorld 1.6**
- **Harmony** (automatically downloaded from Steam Workshop)

## Installation

### Steam Workshop
1. Subscribe to the mod on Steam Workshop
2. Enable it in the RimWorld mod list
3. Ensure Harmony is enabled (it will be automatically subscribed)

### Manual Installation
1. Download the latest release from the [Releases](https://github.com/yourusername/ResearchAreas/releases) page
2. Extract the mod folder to your RimWorld Mods directory:
   - Windows: `C:\Users\[YourName]\AppData\LocalLow\Ludeon Studios\RimWorld by Ludeon Studios\Mods`
   - Linux: `~/.config/unity3d/Ludeon Studios/RimWorld by Ludeon Studios/Mods`
   - macOS: `~/Library/Application Support/RimWorld/Mods`
3. Enable the mod in the RimWorld mod list

## Research Projects

The mod adds the following research projects (all at Neolithic tech level):

| Research Project | Cost | Description |
|-----------------|------|-------------|
| **Stockpile Management** | 100 | Unlocks the ability to create stockpile zones for storing items |
| **Agricultural Planning** | 200 | Unlocks the ability to create growing zones for farming |
| **Settlement Planning** | 200 | Unlocks the Home area for firefighting and defense |
| **Animal Husbandry** | 300 | Unlocks animal sleeping and allowed areas |
| **Complex Roofing** | 300 | Unlocks the ability to designate no-roof areas |
| **Social Organization** | 600 | Unlocks custom allowed areas (requires Settlement Planning) |

## How It Works

1. **Area Creation**: When you try to create a new area or zone, the mod checks if the required research has been completed
2. **Research Validation**: If the research isn't complete, area creation is blocked with a message explaining what research is needed
3. **Save Game Compatibility**: When loading an existing save, the mod automatically removes any areas that don't have their required research completed
4. **Zone Protection**: Stockpile and growing zone creation is also gated behind research

## Compatibility

- Compatible with RimWorld 1.6
- Should work with most mods that add new area types (untested)
- Compatible with research mods like Research Reinvented and ResearchTree
- The Home area is always available regardless of research status

## Building from Source

### Prerequisites
- .NET Framework 4.7.2 or later
- RimWorld 1.6 installation
- Harmony library

### Build Steps

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/ResearchAreas.git
   cd ResearchAreas
   ```

2. Set the `RIMWORLD_DIR` environment variable to your RimWorld installation directory:
   ```bash
   # Windows (PowerShell)
   $env:RIMWORLD_DIR = "C:\Program Files (x86)\Steam\steamapps\common\RimWorld"
   
   # Linux
   export RIMWORLD_DIR="/path/to/rimworld"
   ```

3. Build the project:
   ```bash
   cd Source/ResearchAreas
   dotnet build
   ```

4. The compiled DLL will be in the `Assemblies/` folder

## Mod Structure

```
ResearchAreas/
├── About/
│   └── About.xml                 # Mod metadata
├── Assemblies/
│   └── ResearchAreas.dll         # Compiled mod (after building)
├── Defs/
│   ├── GameComponentDef/
│   │   └── ResearchAreasGameComponent.xml
│   └── ResearchProjectDef/
│       └── AreaResearchProjects.xml
└── Source/
    └── ResearchAreas/            # C# source code
        ├── Core/
        ├── HarmonyPatches/
        └── UI/
```

## Known Issues

- Areas are identified by label matching, so custom-named areas might not be detected correctly
- Zone removal on save load may remove zones with items (items will be dropped)

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Credits

- **Author**: Vec Chromatron
- **Mod ID**: dyscopia.ResearchAreas
- Built with [Harmony](https://github.com/pardeike/Harmony) by Andreas Pardeike

## License

[Specify your license here]

## Support

If you encounter any issues or have suggestions, please open an issue on GitHub.

---

**Note**: This mod modifies core RimWorld functionality. If you experience any conflicts with other mods, please report them so we can investigate compatibility.
