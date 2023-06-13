using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Placeable
{
    public class AnalysisBar : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
            ItemID.Sets.SortingPriorityMaterials[Item.type] = 59; // Influences the inventory sort order. 59 is PlatinumBar, higher is more valuable.
                                                                  // 影响库存排序顺序。59是铂金条，数值越高价值越大。

            // The Chlorophyte Extractinator can exchange items. Here we tell it to allow a one-way exchanging of 5 AnalysisBar for 2 ChlorophyteBar.
            // 叶绿萃取器可以交换物品。在这里我们告诉它允许以5个分析锭换取叶绿锭的单向交换。
            ItemTrader.ChlorophyteExtractinator.AddOption_OneWay(Type, 5, ItemID.ChlorophyteBar, 2);
        }

        public override void SetDefaults()
        {
            // ModContent.TileType returns the ID of the tile that this item should place when used. ModContent.TileType<T>() method returns an integer ID of the tile provided to it through its generic type argument (the type in angle brackets)
            // ModContent.TileType返回该项使用时应放置的图块ID。ModContent.TileType<T>()方法通过其泛型类型参数（尖括号中的类型）提供给它一个整数ID。
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.AnalysisBar>());
            Item.width = 20;
            Item.height = 20;
            Item.value = 750; // The cost of the item in copper coins. (1 = 1 copper, 100 = 1 silver, 1000 = 1 gold, 10000 = 1 platinum)
                              // 该项在铜币中的成本。（1 = 1铜币，100 = 1银币，1000 = 1金币，10000 = 1白金币）
        }

        // Please see AnalysisContent/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AnalysisOre>(4)
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}
