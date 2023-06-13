using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Placeable.Furniture
{
    public class MinionBossRelic : ModItem
    {
        public override void SetDefaults()
        {
            // Vanilla has many useful methods like these, use them! This substitutes setting Item.createTile and Item.placeStyle aswell as setting a few values that are common across all placeable items
            // The place style (here by default 0) is important if you decide to have more than one relic share the same tile type (more on that in the tiles' code)

            // Vanilla有许多这样的有用方法，要好好利用！这个方法可以替代设置Item.createTile和Item.placeStyle以及设置一些在所有可放置物品中通用的值。
            // 如果你决定让多个文物共享相同的图块类型（更多关于图块代码的内容），那么放置风格（默认为0）就很重要。
            Item.DefaultToPlaceableTile(ModContent.TileType<AnalysisContent.Tiles.Furniture.MinionBossRelic>(), 0);

            Item.width = 30;
            Item.height = 40;
            Item.rare = ItemRarityID.Master;
            Item.master = true; // This makes sure that "Master" displays in the tooltip, as the rarity only changes the item name color
                                // 这确保了“Master”显示在工具提示中，因为稀有度只会改变物品名称颜色。

            Item.value = Item.buyPrice(0, 5);
        }
    }
}
