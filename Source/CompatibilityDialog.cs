using RimWorld;
using UnityEngine;
using Verse;

namespace ResearchAreas.UI
{
    /// <summary>
    /// Dialog shown on first load to warn about area removal.
    /// </summary>
    public class CompatibilityDialog : Window
    {
        private bool removeAreas = true;
        private bool dontShowAgain = false;

        public CompatibilityDialog()
        {
            this.doCloseButton = false;
            this.doCloseX = false;
            this.absorbInputAroundWindow = true;
            this.forcePause = true;
        }

        public override Vector2 InitialSize => new Vector2(500f, 300f);

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(0f, 0f, inRect.width, 40f), "Research Areas - Save Compatibility");

            Text.Font = GameFont.Small;
            Rect textRect = new Rect(0f, 50f, inRect.width, inRect.height - 100f);
            string text = "This mod will check all existing areas and zones when loading this save.\n\n" +
                         "Areas and zones that don't have their required research completed will be removed.\n\n" +
                         "If zones contain items, they will be dropped on the ground.\n\n" +
                         "Do you want to proceed with area validation?";
            
            Widgets.Label(textRect, text);

            Rect checkboxRect = new Rect(0f, inRect.height - 80f, inRect.width, 30f);
            Widgets.CheckboxLabeled(checkboxRect, "Don't show this dialog again", ref dontShowAgain);

            Rect buttonRect1 = new Rect(inRect.width / 2f - 120f, inRect.height - 40f, 100f, 30f);
            if (Widgets.ButtonText(buttonRect1, "Proceed"))
            {
                var settings = Settings.ResearchAreasMod.Settings;
                if (settings != null)
                {
                    if (dontShowAgain)
                    {
                        settings.compatibilityDialogShown = true;
                        settings.showCompatibilityDialog = false;
                    }

                    if (removeAreas)
                    {
                        Core.AreaRemover.ValidateAllMapsWithWarnings();
                    }
                }

                Close();
            }

            Rect buttonRect2 = new Rect(inRect.width / 2f + 20f, inRect.height - 40f, 100f, 30f);
            if (Widgets.ButtonText(buttonRect2, "Skip"))
            {
                var settings = Settings.ResearchAreasMod.Settings;
                if (settings != null && dontShowAgain)
                {
                    settings.compatibilityDialogShown = true;
                    settings.showCompatibilityDialog = false;
                }
                Close();
            }
        }
    }
}
