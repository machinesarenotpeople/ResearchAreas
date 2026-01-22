using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ResearchAreas.Core
{
    /// <summary>
    /// Game component to handle initialization, area validation on load, and research completion events.
    /// </summary>
    public class ResearchAreasGameComponent : GameComponent
    {
        private bool areasValidated = false;

        public ResearchAreasGameComponent()
        {
        }

        public ResearchAreasGameComponent(Game game) : base()
        {
        }

        /// <summary>
        /// Called when the game is initialized.
        /// </summary>
        public override void FinalizeInit()
        {
            base.FinalizeInit();
            
            // Refresh research cache
            ResearchChecker.RefreshCache();
            
            // Validate areas on first load
            if (!areasValidated)
            {
                ValidateAreasOnLoad();
                areasValidated = true;
            }
        }

        /// <summary>
        /// Called when the game is loaded.
        /// </summary>
        public override void LoadedGame()
        {
            base.LoadedGame();
            
            // Refresh research cache
            ResearchChecker.RefreshCache();
            
            // Show compatibility dialog if needed
            var settings = Settings.ResearchAreasMod.Settings;
            if (settings != null && settings.showCompatibilityDialog && !settings.compatibilityDialogShown)
            {
                Find.WindowStack.Add(new UI.CompatibilityDialog());
            }
            else
            {
                // Validate and remove invalid areas
                ValidateAreasOnLoad();
            }
        }

        /// <summary>
        /// Validate areas on game load and remove those without required research.
        /// </summary>
        private void ValidateAreasOnLoad()
        {
            Dictionary<string, List<string>> removedAreas = AreaRemover.ValidateAllMaps();
            
            if (removedAreas.Count > 0)
            {
                AreaRemover.NotifyPlayer(removedAreas);
            }
        }

        /// <summary>
        /// Called every game tick (optional - can be used for periodic validation).
        /// </summary>
        public override void GameComponentTick()
        {
            base.GameComponentTick();
            
            // Periodically refresh cache (every 250 ticks = ~4 seconds)
            if (Find.TickManager.TicksGame % 250 == 0)
            {
                ResearchChecker.RefreshCache();
            }
        }

        /// <summary>
        /// Expose data for saving/loading.
        /// </summary>
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref areasValidated, "areasValidated", false);
        }
    }
}
