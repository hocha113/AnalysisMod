using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Biomes
{
    public class AnalysisWaterfallStyle : ModWaterfallStyle
    {
        // Makes the waterfall provide light
        // Learn how to make a waterfall: https://terraria.wiki.gg/wiki/Waterfall

        // 让瀑布发光
        // 学习如何制作瀑布：https://terraria.wiki.gg/wiki/Waterfall
        public override void AddLight(int i, int j) =>
            Lighting.AddLight(new Vector2(i, j).ToWorldCoordinates(), Color.White.ToVector3() * 0.5f);
    }
}