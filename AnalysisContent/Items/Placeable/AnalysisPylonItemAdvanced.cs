using AnalysisMod.AnalysisContent.Tiles;
using Terraria.Enums;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Placeable
{
    /// <summary>
    /// The coupled item that places the Advanced Analysis Pylon tile. For more information on said tile,
    /// see <seealso cref="AnalysisPylonTileAdvanced"/>.<br/>
    /// 用于放置高级分析塔砖块的耦合物品。有关该砖块的更多信息，<br/>
    /// 请参见<seealso cref="AnalysisPylonTileAdvanced"/>。
    /// </summary>
    public class AnalysisPylonItemAdvanced : ModItem
    {
        public override void SetDefaults()
        {
            // Basically, this a just a shorthand method that will set all default values necessary to place
            // the passed in tile type; in this case, the Advanced Analysis Pylon tile.

            // 基本上，这只是一种简写方法，将设置放置传入的砖块类型所需的所有默认值
            // 在这种情况下，是高级分析塔砖块。
            Item.DefaultToPlaceableTile(ModContent.TileType<AnalysisPylonTileAdvanced>());

            // Another shorthand method that will set the rarity and how much the item is worth.
            // 另一个简写方法，将设置物品稀有度和价值。
            Item.SetShopValues(ItemRarityColor.LightRed4, Terraria.Item.buyPrice(gold: 20));
        }
    }
}
