using Terraria;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Placeable.Furniture
{
    public class AnalysisPlatform : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 200;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.AnalysisPlatform>());
            Item.width = 8;
            Item.height = 10;
        }

        // Please see Content/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe(2)
                .AddIngredient<AnalysisBlock>()
                .AddTile<Tiles.Furniture.AnalysisWorkbench>()
                .Register();
        }
    }
}