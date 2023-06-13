using AnalysisMod.AnalysisContent.Items;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Tiles.Plants
{
    public class AnalysisPalmTree : ModPalmTree
    {
        // This is a blind copy-paste from Vanilla's PurityPalmTree settings.
        //TODO: This needs some explanations
        public override TreePaintingSettings TreeShaderSettings => new TreePaintingSettings
        {
            UseSpecialGroups = true,
            SpecialGroupMinimalHueValue = 11f / 72f,
            SpecialGroupMaximumHueValue = 0.25f,
            SpecialGroupMinimumSaturationValue = 0.88f,
            SpecialGroupMaximumSaturationValue = 1f
        };

        public override void SetStaticDefaults()
        {
            // Makes Analysis Palm Tree grow on Gold Ore
            GrowsOnTileId = new int[1] { TileID.Gold };
        }

        // This is the primary texture for the trunk. Branches and foliage use different settings.
        // The first row will be the Ocean textures, the second row will be Oasis Textures.
        public override Asset<Texture2D> GetTexture()
        {
            return ModContent.Request<Texture2D>("AnalysisMod/AnalysisContent/Tiles/Plants/AnalysisPalmTree");
        }

        public override int SaplingGrowthType(ref int style)
        {
            style = 1;
            return ModContent.TileType<Plants.AnalysisSapling>();
        }

        public override Asset<Texture2D> GetOasisTopTextures()
        {
            // Palm Trees come in an Oasis variant. The Top Textures for it:
            return ModContent.Request<Texture2D>("AnalysisMod/AnalysisContent/Tiles/Plants/AnalysisPalmOasisTree_Tops");
        }

        public override Asset<Texture2D> GetTopTextures()
        {
            // Palm Trees come in a Beach variant. The Top Textures for it:
            return ModContent.Request<Texture2D>("AnalysisMod/AnalysisContent/Tiles/Plants/AnalysisPalmTree_Tops");
        }

        public override int DropWood()
        {
            return ModContent.ItemType<Items.Placeable.AnalysisOre>();
        }
    }
}