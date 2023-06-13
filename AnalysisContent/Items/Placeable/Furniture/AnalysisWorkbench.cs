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

            // ModContent.TileType<Tiles.Furniture.AnalysisWorkbench>() ��������Ʒ��ʹ��ʱӦ���õĴ�ש��ID��
            // DefaultToPlaceableTile �������ÿɷ�����Ʒʹ�õĸ�����Ŀֵ
            // �� Visual Studio ����ͣ�� DefaultToPlaceableTile �����Ķ��ĵ���
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.AnalysisWorkbench>());
            Item.width = 28; // The item texture's width
                             // ����Ʒ����Ŀ��

            Item.height = 14; // The item texture's height
                              // ����Ʒ����ĸ߶�
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
