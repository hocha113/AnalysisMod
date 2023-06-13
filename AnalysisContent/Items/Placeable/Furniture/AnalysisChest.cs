using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Placeable.Furniture
{
    public class AnalysisChest : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.AnalysisChest>());
            // Item.placeStyle = 1; // Use this to place the chest in its locked style
            // Item.placeStyle = 1; // 使用此代码将箱子放置为锁定状态
            Item.width = 26;
            Item.height = 22;
            Item.value = 500;
        }

        // Please see AnalysisContent/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AnalysisItem>()
                .AddTile<Tiles.Furniture.AnalysisWorkbench>()
                .Register();
        }
    }

    public class AnalysisChestKey : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 3; // Biome keys usually take 1 item to research instead.
                                          // 生态圈钥匙通常只需要1个物品进行研究。
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.GoldenKey);
        }
    }
}
