using AnalysisMod.AnalysisContent;
using AnalysisMod.AnalysisContent.Items.Accessories;
using Terraria;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisCommon.Players
{
    /// <summary>
    /// ModPlayer类与<seealso cref="AnalysisInfoDisplay"/>和<seealso cref="AnalysisInfoAccessory"/>配合使用，
    /// 展示如何正确添加新的信息附件（例如雷达、生物分析仪等）。
    /// </summary>
    public class AnalysisInfoDisplayPlayer : ModPlayer
    {
        // 标志检查何时应激活信息显示
        public bool showMinionCount;

        // 确保使用正确的重置钩子。这个是独特的，因为它在游戏暂停时,
        // 仍然会被调用；这允许信息附件保持更新。
        public override void ResetInfoAccessories()
        {
            showMinionCount = false;
        }

        // 如果我们有另一个在我们队伍中附近的玩家，则希望他们的信息附件也能对我们起作用，
        // 就像原版一样。这就是此钩子所用之处。
        public override void RefreshInfoAccessoriesFromTeamPlayers(Player otherPlayer)
        {
            if (otherPlayer.GetModPlayer<AnalysisInfoDisplayPlayer>().showMinionCount)
            {
                showMinionCount = true;
            }
        }
    }
}
