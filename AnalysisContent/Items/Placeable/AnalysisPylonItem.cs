using AnalysisMod.AnalysisContent.Tiles;
using Terraria.Enums;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Placeable
{
    /// <summary>
    /// The coupled item that places the Analysis Pylon tile. For more information on said tile,
    /// see <seealso cref="AnalysisPylonTile"/>.<br/>
    /// 用于放置分析基座瓦片的耦合项。有关该瓦片的更多信息，请参见<seealso cref="AnalysisPylonTile"/>。
    /// </summary>
    public class AnalysisPylonItem : ModItem
    {
        public override void SetDefaults()
        {
            // Basically, this a just a shorthand method that will set all default values necessary to place
            // the passed in tile type; in this case, the Analysis Pylon tile.
            // 基本上，这只是一种简写方法，将设置放置传入的瓷砖类型所需的所有默认值；
            // 在这种情况下，是分析塔瓷砖。
            Item.DefaultToPlaceableTile(ModContent.TileType<AnalysisPylonTile>());

            // Another shorthand method that will set the rarity and how much the item is worth.
            // 另一个简写方法将设置物品的稀有度和价值。
            Item.SetShopValues(ItemRarityColor.Blue1, Terraria.Item.buyPrice(gold: 10));
        }
    }
}
