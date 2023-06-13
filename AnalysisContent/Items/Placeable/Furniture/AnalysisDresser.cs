using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Placeable.Furniture
{
    public class AnalysisDresser : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.AnalysisDresser>());

            Item.width = 26;
            Item.height = 22;
            Item.value = 500;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Dresser)
                .AddIngredient<AnalysisBlock>(10)
                .AddTile<Tiles.Furniture.AnalysisWorkbench>()
                .Register();
        }
    }
}