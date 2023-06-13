using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Placeable.Furniture
{
    public class AnalysisWorkbench : ModItem
    {
        public override void SetDefaults()
        {
            // ModContent.TileType<Tiles.Furniture.AnalysisWorkbench>() retrieves the id of the tile that this item should place when used.
            // DefaultToPlaceableTile handles setting various Item values that placeable items use
            // Hover over DefaultToPlaceableTile in Visual Studio to read the documentation!

            // ModContent.TileType<Tiles.Furniture.AnalysisWorkbench>() 检索此物品在使用时应放置的瓷砖的ID。
            // DefaultToPlaceableTile 处理设置可放置物品使用的各种项目值
            // 在 Visual Studio 中悬停在 DefaultToPlaceableTile 上以阅读文档！
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.AnalysisWorkbench>());
            Item.width = 28; // The item texture's width
                             // 该物品纹理的宽度

            Item.height = 14; // The item texture's height
                              // 该物品纹理的高度
            Item.value = 150;
        }

        // Please see Content/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.WorkBench)
                .AddIngredient<AnalysisItem>(10)
                .Register();
        }
    }
}
