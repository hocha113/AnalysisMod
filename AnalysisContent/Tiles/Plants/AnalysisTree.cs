using AnalysisMod.AnalysisContent.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Tiles.Plants
{
    public class AnalysisTree : ModTree
    {
        // This is a blind copy-paste from Vanilla's PurityPalmTree settings.
        //TODO: This needs some explanations

        // 这是从Vanilla的PurityPalmTree设置中盲目复制粘贴的。
        //TODO: 这需要一些解释
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
            // Makes Analysis Tree grow on AnalysisBlock
            // 使Analysis Tree在AnalysisBlock上生长
            GrowsOnTileId = new int[1] { ModContent.TileType<AnalysisBlock>() };
        }

        // This is the primary texture for the trunk. Branches and foliage use different settings.
        // 这是树干的主要纹理。分支和叶子使用不同的设置。
        public override Asset<Texture2D> GetTexture()
        {
            return ModContent.Request<Texture2D>("AnalysisMod/AnalysisContent/Tiles/Plants/AnalysisTree");
        }

        public override int SaplingGrowthType(ref int style)
        {
            style = 0;
            return ModContent.TileType<AnalysisSapling>();
        }

        public override void SetTreeFoliageSettings(Tile tile, ref int xoffset, ref int treeFrame, ref int floorY, ref int topTextureFrameWidth, ref int topTextureFrameHeight)
        {
            // This is where fancy code could go, but let's save that for an advanced Analysis
            // 这里可以放置花哨的代码，但让我们将其保留给高级分析。
        }

        // Branch Textures
        // 分支纹理
        public override Asset<Texture2D> GetBranchTextures()
        {
            return ModContent.Request<Texture2D>("AnalysisMod/AnalysisContent/Tiles/Plants/AnalysisTree_Branches");
        }

        // Top Textures
        // 顶部纹理
        public override Asset<Texture2D> GetTopTextures()
        {
            return ModContent.Request<Texture2D>("AnalysisMod/AnalysisContent/Tiles/Plants/AnalysisTree_Tops");
        }

        public override int DropWood()
        {
            return ModContent.ItemType<AnalysisDye>();
        }

        public override bool Shake(int x, int y, ref bool createLeaves)
        {
            Item.NewItem(WorldGen.GetItemSource_FromTreeShake(x, y), new Vector2(x, y) * 16, ModContent.ItemType<Items.Placeable.AnalysisBlock>());
            return false;
        }

        public override int TreeLeaf()
        {
            return ModContent.GoreType<AnalysisTreeLeaf>();
        }
    }
}