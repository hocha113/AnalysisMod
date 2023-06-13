using System;
using AnalysisMod.AnalysisContent.Tiles;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisCommon.Systems
{
    public class AnalysisBiomeTileCount : ModSystem
    {
        public int AnalysisBlockCount;

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            AnalysisBlockCount = tileCounts[ModContent.TileType<AnalysisBlock>()];
        }
    }
}
