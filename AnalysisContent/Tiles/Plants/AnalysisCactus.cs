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
            GrowsOnTileId = new int[1] { ModContent.TileType<AnalysisOre>() };
        }

        public override Asset<Texture2D> GetTexture()
        {
            return ModContent.Request<Texture2D>("AnalysisMod/AnalysisContent/Tiles/Plants/AnalysisCactus");
        }

        // This would be where the Cactus Fruit Texture would go, if we had one.
        public override Asset<Texture2D> GetFruitTexture()
        {
            return null;
        }
    }
}