using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Placeable.Furniture
{
    public class AnalysisTable : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.AnalysisTable>());
            Item.width = 38;
            Item.height = 24;
            Item.value = 150;
        }

        // Please see Content/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.WoodenTable)
                .AddIngredient<AnalysisItem>(10)
                .Register();
        }
    }
}
