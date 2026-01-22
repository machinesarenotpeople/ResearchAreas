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
        public bool requireHomeResearch = true;
        public bool requireNoRoofResearch = true;
        public bool requireAllowedResearch = true;
        public bool requireSnowRemovalResearch = true;
        public bool requirePollutionRemovalResearch = true;

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

        // Configurable research costs
        public int costPottery = 100;
        public int costDomestication = 150;
        public int costAgriculture = 150;
        public int costShoveling = 200;
        public int costComplexRoofing = 300;
        public int costSanitation = 300;
        public int costGovernment = 400;

        public override void ExposeData()
        {
            base.ExposeData();
            
            // Research toggles
            Scribe_Values.Look(ref requireStockpileResearch, "requireStockpileResearch", true);
            Scribe_Values.Look(ref requireGrowingResearch, "requireGrowingResearch", true);
            Scribe_Values.Look(ref requireHomeResearch, "requireHomeResearch", true);
            Scribe_Values.Look(ref requireNoRoofResearch, "requireNoRoofResearch", true);
            Scribe_Values.Look(ref requireAllowedResearch, "requireAllowedResearch", true);
            Scribe_Values.Look(ref requireSnowRemovalResearch, "requireSnowRemovalResearch", true);
            Scribe_Values.Look(ref requirePollutionRemovalResearch, "requirePollutionRemovalResearch", true);

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

            // Research costs
            Scribe_Values.Look(ref costPottery, "costPottery", 100);
            Scribe_Values.Look(ref costDomestication, "costDomestication", 150);
            Scribe_Values.Look(ref costAgriculture, "costAgriculture", 150);
            Scribe_Values.Look(ref costShoveling, "costShoveling", 200);
            Scribe_Values.Look(ref costComplexRoofing, "costComplexRoofing", 300);
            Scribe_Values.Look(ref costSanitation, "costSanitation", 300);
            Scribe_Values.Look(ref costGovernment, "costGovernment", 400);
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

            listing.CheckboxLabeled("Require Stockpile Research", ref Settings.requireStockpileResearch);
            listing.CheckboxLabeled("Require Growing Research", ref Settings.requireGrowingResearch);
            listing.CheckboxLabeled("Require Home Research", ref Settings.requireHomeResearch);
            listing.CheckboxLabeled("Require No Roof Research", ref Settings.requireNoRoofResearch);
            listing.CheckboxLabeled("Require Allowed Research", ref Settings.requireAllowedResearch);
            listing.CheckboxLabeled("Require Snow Removal Research", ref Settings.requireSnowRemovalResearch);
            listing.CheckboxLabeled("Require Pollution Removal Research", ref Settings.requirePollutionRemovalResearch);

            listing.Gap();
            listing.Label("Area Removal Settings");
            listing.GapLine();

            listing.CheckboxLabeled("Remove Invalid Areas on Load", ref Settings.removeInvalidAreasOnLoad);
            listing.CheckboxLabeled("Show Removal Warnings", ref Settings.showRemovalWarnings);

            listing.Gap();
            listing.Label("Research Costs");
            listing.GapLine();

            listing.Label($"Pottery: {Settings.costPottery}");
            Settings.costPottery = (int)listing.Slider(Settings.costPottery, 1, 1000);
            
            listing.Label($"Domestication: {Settings.costDomestication}");
            Settings.costDomestication = (int)listing.Slider(Settings.costDomestication, 1, 1000);
            
            listing.Label($"Agriculture: {Settings.costAgriculture}");
            Settings.costAgriculture = (int)listing.Slider(Settings.costAgriculture, 1, 1000);
            
            listing.Label($"Shoveling: {Settings.costShoveling}");
            Settings.costShoveling = (int)listing.Slider(Settings.costShoveling, 1, 1000);
            
            listing.Label($"Complex Roofing: {Settings.costComplexRoofing}");
            Settings.costComplexRoofing = (int)listing.Slider(Settings.costComplexRoofing, 1, 1000);
            
            listing.Label($"Sanitation: {Settings.costSanitation}");
            Settings.costSanitation = (int)listing.Slider(Settings.costSanitation, 1, 1000);
            
            listing.Label($"Government: {Settings.costGovernment}");
            Settings.costGovernment = (int)listing.Slider(Settings.costGovernment, 1, 1000);

            listing.Gap();
            listing.Label("UI Settings");
            listing.GapLine();

            listing.CheckboxLabeled("Show Tooltips", ref Settings.showTooltips);
            listing.CheckboxLabeled("Show Visual Indicators", ref Settings.showVisualIndicators);

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
