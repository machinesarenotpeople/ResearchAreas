using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace ResearchAreas.Settings
{
    /// <summary>
    /// Mod settings for Research Areas mod.
    /// </summary>
    public class ResearchAreasSettings : ModSettings
    {
        // Toggle research requirements per area type
        public bool requireStockpileResearch = true;
        public bool requireGrowingResearch = true;
        public bool requireAnimalResearch = true;
        public bool requireHomeResearch = true;
        public bool requireNoRoofResearch = true;
        public bool requireAllowedResearch = true;

        // Area removal settings
        public bool removeInvalidAreasOnLoad = true;
        public bool showRemovalWarnings = true;

        // Save compatibility
        public bool showCompatibilityDialog = true;
        public bool compatibilityDialogShown = false;

        // UI settings
        public bool showTooltips = true;
        public bool showVisualIndicators = true;

        // Custom area name mappings (area label -> area type key)
        public Dictionary<string, string> customAreaMappings = new Dictionary<string, string>();

        public override void ExposeData()
        {
            base.ExposeData();
            
            // Research toggles
            Scribe_Values.Look(ref requireStockpileResearch, "requireStockpileResearch", true);
            Scribe_Values.Look(ref requireGrowingResearch, "requireGrowingResearch", true);
            Scribe_Values.Look(ref requireAnimalResearch, "requireAnimalResearch", true);
            Scribe_Values.Look(ref requireHomeResearch, "requireHomeResearch", true);
            Scribe_Values.Look(ref requireNoRoofResearch, "requireNoRoofResearch", true);
            Scribe_Values.Look(ref requireAllowedResearch, "requireAllowedResearch", true);

            // Removal settings
            Scribe_Values.Look(ref removeInvalidAreasOnLoad, "removeInvalidAreasOnLoad", true);
            Scribe_Values.Look(ref showRemovalWarnings, "showRemovalWarnings", true);

            // Compatibility
            Scribe_Values.Look(ref showCompatibilityDialog, "showCompatibilityDialog", true);
            Scribe_Values.Look(ref compatibilityDialogShown, "compatibilityDialogShown", false);

            // UI settings
            Scribe_Values.Look(ref showTooltips, "showTooltips", true);
            Scribe_Values.Look(ref showVisualIndicators, "showVisualIndicators", true);

            // Custom area mappings
            Scribe_Collections.Look(ref customAreaMappings, "customAreaMappings", LookMode.Value, LookMode.Value);
            if (customAreaMappings == null)
            {
                customAreaMappings = new Dictionary<string, string>();
            }
        }
    }

    /// <summary>
    /// Mod class to register settings.
    /// </summary>
    public class ResearchAreasMod : Mod
    {
        public static ResearchAreasSettings Settings { get; private set; }

        public ResearchAreasMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<ResearchAreasSettings>();
        }

        public override void DoSettingsWindowContents(UnityEngine.Rect inRect)
        {
            var listing = new Listing_Standard();
            listing.Begin(inRect);

            listing.Label("Research Requirements");
            listing.GapLine();

            Settings.requireStockpileResearch = listing.CheckboxLabeled("Require Stockpile Research", Settings.requireStockpileResearch, "Require research to create stockpile zones");
            Settings.requireGrowingResearch = listing.CheckboxLabeled("Require Growing Research", Settings.requireGrowingResearch, "Require research to create growing zones");
            Settings.requireAnimalResearch = listing.CheckboxLabeled("Require Animal Research", Settings.requireAnimalResearch, "Require research to create animal areas");
            Settings.requireHomeResearch = listing.CheckboxLabeled("Require Home Research", Settings.requireHomeResearch, "Require research to use Home area (note: Home area always exists)");
            Settings.requireNoRoofResearch = listing.CheckboxLabeled("Require No Roof Research", Settings.requireNoRoofResearch, "Require research to create no-roof areas");
            Settings.requireAllowedResearch = listing.CheckboxLabeled("Require Allowed Research", Settings.requireAllowedResearch, "Require research to create custom allowed areas");

            listing.Gap();
            listing.Label("Area Removal Settings");
            listing.GapLine();

            Settings.removeInvalidAreasOnLoad = listing.CheckboxLabeled("Remove Invalid Areas on Load", Settings.removeInvalidAreasOnLoad, "Automatically remove areas without required research when loading saves");
            Settings.showRemovalWarnings = listing.CheckboxLabeled("Show Removal Warnings", Settings.showRemovalWarnings, "Show warnings when removing areas that contain items (items will be dropped on the ground)");

            listing.Gap();
            listing.Label("UI Settings");
            listing.GapLine();

            Settings.showTooltips = listing.CheckboxLabeled("Show Tooltips", Settings.showTooltips, "Show tooltips explaining research requirements");
            Settings.showVisualIndicators = listing.CheckboxLabeled("Show Visual Indicators", Settings.showVisualIndicators, "Show visual indicators (grayed out buttons, lock icons) for locked areas");

            listing.Gap();
            listing.Label("Custom Area Name Mappings");
            listing.GapLine();
            listing.Label("Map custom area names to area types. Examples: MyStockpile=Stockpile, FarmZone=Growing", -1f);
            listing.Gap(4f);

            // Display current mappings
            if (Settings.customAreaMappings != null && Settings.customAreaMappings.Count > 0)
            {
                var mappingsToRemove = new List<string>();
                foreach (var kvp in Settings.customAreaMappings)
                {
                    Rect rowRect = listing.GetRect(24f);
                    Rect labelRect = new Rect(rowRect.x, rowRect.y, rowRect.width - 110f, 24f);
                    Rect buttonRect = new Rect(rowRect.xMax - 100f, rowRect.y, 100f, 24f);
                    
                    Widgets.Label(labelRect, $"{kvp.Key} → {kvp.Value}");
                    if (Widgets.ButtonText(buttonRect, "Remove"))
                    {
                        mappingsToRemove.Add(kvp.Key);
                    }
                    listing.Gap(2f);
                }
                foreach (var key in mappingsToRemove)
                {
                    Settings.customAreaMappings.Remove(key);
                    // Clear area detection cache since mappings changed
                    Core.ResearchChecker.ClearCaches();
                }
                listing.Gap(4f);
            }

            if (listing.ButtonText("Add Custom Mapping"))
            {
                Find.WindowStack.Add(new UI.CustomAreaMappingDialog());
            }

            listing.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Research Areas";
        }
    }
}
