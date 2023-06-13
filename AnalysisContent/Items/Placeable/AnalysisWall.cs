using AnalysisMod.AnalysisContent.Tiles.Furniture;
using Terraria;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Placeable
{
    public class AnalysisWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }

        public override void SetDefaults()
        {
            // ModContent.WallType<Walls.AnalysisWall>() retrieves the id of the wall that this item should place when used.
            // DefaultToPlaceableWall handles setting various Item values that placeable wall items use.
            // Hover over DefaultToPlaceableWall in Visual Studio to read the documentation!

            // ModContent.WallType<Walls.AnalysisWall>() 用于检索此物品在使用时应放置的墙壁 ID。
            // DefaultToPlaceableWall 处理可放置墙壁物品使用时设置的各种 Item 值。
            // 在 Visual Studio 中悬停在 DefaultToPlaceableWall 上以阅读文档！
            Item.DefaultToPlaceableWall(ModContent.WallType<Walls.AnalysisWall>());
        }

        // Please see Content/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient<AnalysisBlock>()
                .AddTile<AnalysisWorkbench>()
                .Register();
        }
    }
}
