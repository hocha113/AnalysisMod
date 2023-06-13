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

            // ModContent.WallType<Walls.AnalysisWall>() ���ڼ�������Ʒ��ʹ��ʱӦ���õ�ǽ�� ID��
            // DefaultToPlaceableWall ����ɷ���ǽ����Ʒʹ��ʱ���õĸ��� Item ֵ��
            // �� Visual Studio ����ͣ�� DefaultToPlaceableWall �����Ķ��ĵ���
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
