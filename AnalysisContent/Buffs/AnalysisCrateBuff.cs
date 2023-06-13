using AnalysisMod.AnalysisCommon.Players;
using Terraria;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Buffs
{
    public class AnalysisCrateBuff : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            // Use a ModPlayer to keep track of the buff being active
            // 使用 ModPlayer 来跟踪增益是否处于激活状态
            player.GetModPlayer<AnalysisFishingPlayer>().hasAnalysisCrateBuff = true;
        }
    }
}
