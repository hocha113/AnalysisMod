using AnalysisMod.AnalysisContent;
using AnalysisMod.AnalysisContent.Items.Accessories;
using Terraria;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisCommon.Players
{
    /// <summary>
    /// ModPlayer class coupled with <seealso cref="AnalysisInfoDisplay"/> and <seealso cref="AnalysisInfoAccessory"/> to show off how to properly add a
    /// new info accessory (such as a Radar, Lifeform Analyzer, etc.)
    /// </summary>
    public class AnalysisInfoDisplayPlayer : ModPlayer
    {
        // Flag checking when information display should be activated
        public bool showMinionCount;

        // Make sure to use the right Reset hook. This one is unique, as it will still be
        // called when the game is paused; this allows for info accessories to keep updating properly.
        public override void ResetInfoAccessories()
        {
            showMinionCount = false;
        }

        // If we have another nearby player on our team, we want to get their info accessories working on us,
        // just like in vanilla. This is what this hook is for.
        public override void RefreshInfoAccessoriesFromTeamPlayers(Player otherPlayer)
        {
            if (otherPlayer.GetModPlayer<AnalysisInfoDisplayPlayer>().showMinionCount)
            {
                showMinionCount = true;
            }
        }
    }
}
