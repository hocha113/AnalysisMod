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
            // Item.placeStyle = 1; // ʹ�ô˴��뽫���ӷ���Ϊ����״̬
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
                                          // ��̬ȦԿ��ͨ��ֻ��Ҫ1����Ʒ�����о���
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.GoldenKey);
        }
    }
}
