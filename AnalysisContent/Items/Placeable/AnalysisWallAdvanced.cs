using AnalysisMod.AnalysisContent.Tiles.Furniture;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Placeable
{
    public class AnalysisWallAdvanced : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<Walls.AnalysisWallAdvanced>());
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
