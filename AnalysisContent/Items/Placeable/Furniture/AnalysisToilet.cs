using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Placeable.Furniture
{
    public class AnalysisToilet : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.AnalysisToilet>());
            Item.width = 16;
            Item.height = 24;
            Item.value = 150;
        }

        // Please see Content/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Toilet)
                .AddIngredient<AnalysisItem>(10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
