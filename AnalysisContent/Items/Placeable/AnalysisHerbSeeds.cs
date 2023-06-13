using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Placeable
{
    public class AnalysisHerbSeeds : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.DisableAutomaticPlaceableDrop[Type] = true; // This prevents this item from being automatically dropped from AnalysisHerb tile.
                                                                    // 这可以防止该物品被自动从草药分析界面中删除。
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.AnalysisHerb>());
            Item.width = 12;
            Item.height = 14;
            Item.value = 80;
        }

        // Please see AnalysisContent/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<AnalysisBlock>(), 1)
                .Register();
        }
    }
}
