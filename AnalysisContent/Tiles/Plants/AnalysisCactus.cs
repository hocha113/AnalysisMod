using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Tiles.Plants
{
    public class AnalysisCactus : ModCactus
    {
        public override void SetStaticDefaults()
        {
            // Makes Analysis Cactus grow on AnalysisOre
            // 让分析仙人掌在分析矿石上生长
            GrowsOnTileId = new int[1] { ModContent.TileType<AnalysisOre>() };
        }

        public override Asset<Texture2D> GetTexture()
        {
            return ModContent.Request<Texture2D>("AnalysisMod/AnalysisContent/Tiles/Plants/AnalysisCactus");
        }

        // This would be where the Cactus Fruit Texture would go, if we had one.
        // 如果我们有的话，这将是仙人掌果纹理所在的位置。
        public override Asset<Texture2D> GetFruitTexture()
        {
            return null;
        }
    }
}