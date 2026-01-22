using System;
using RimWorld;
using ResearchAreas.Core;
using ResearchAreas.Settings;
using UnityEngine;
using Verse;

namespace ResearchAreas.UI
{
    /// <summary>
    /// Dialog for adding custom area name mappings.
    /// </summary>
    public class CustomAreaMappingDialog : Window
    {
        private string areaName = "";
        private string areaType = "Stockpile";
        private readonly string[] areaTypeOptions = { "Stockpile", "Growing", "AnimalSleeping", "AnimalAllowed", "Home", "NoRoof", "Allowed" };

        public CustomAreaMappingDialog()
        {
            this.doCloseButton = true;
            this.absorbInputAroundWindow = true;
            this.forcePause = true;
        }

        public override Vector2 InitialSize => new Vector2(400f, 200f);

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(0f, 0f, inRect.width, 30f), "Add Custom Area Mapping");

            Text.Font = GameFont.Small;
            float y = 40f;

            // Area name input
            Widgets.Label(new Rect(0f, y, 150f, 30f), "Area Name:");
            areaName = Widgets.TextField(new Rect(160f, y, inRect.width - 160f, 30f), areaName);
            y += 40f;

            // Area type dropdown
            Widgets.Label(new Rect(0f, y, 150f, 30f), "Area Type:");
            Rect dropdownRect = new Rect(160f, y, inRect.width - 160f, 30f);
            if (Widgets.ButtonText(dropdownRect, areaType))
            {
                var options = new System.Collections.Generic.List<FloatMenuOption>();
                foreach (string option in areaTypeOptions)
                {
                    string currentOption = option;
                    options.Add(new FloatMenuOption(currentOption, () => areaType = currentOption));
                }
                Find.WindowStack.Add(new FloatMenu(options));
            }
            y += 50f;

            // Add button
            Rect addButtonRect = new Rect(inRect.width / 2f - 50f, y, 100f, 30f);
            if (Widgets.ButtonText(addButtonRect, "Add"))
            {
                if (string.IsNullOrWhiteSpace(areaName))
                {
                    Messages.Message("Area name cannot be empty.", MessageTypeDefOf.RejectInput);
                }
                else
                {
                    var settings = Settings.ResearchAreasMod.Settings;
                    if (settings != null && settings.customAreaMappings != null)
                    {
                        string normalizedName = areaName.Trim().ToLower();
                        if (settings.customAreaMappings.ContainsKey(normalizedName))
                        {
                            Messages.Message($"Mapping for '{areaName}' already exists. Remove it first or use a different name.", MessageTypeDefOf.RejectInput);
                        }
                        else
                        {
                            settings.customAreaMappings[normalizedName] = areaType;
                            // Clear area detection cache since mappings changed
                            Core.ResearchChecker.ClearCaches();
                            Messages.Message($"Added mapping: {areaName} → {areaType}", MessageTypeDefOf.PositiveEvent);
                            Close();
                        }
                    }
                }
            }
        }
    }
}
