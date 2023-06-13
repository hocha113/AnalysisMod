using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Placeable.Furniture
{
    public class MinionBossTrophy : ModItem
    {
        public override void SetDefaults()
        {
            // Vanilla has many useful methods like these, use them! This substitutes setting Item.createTile and Item.placeStyle aswell as setting a few values that are common across all placeable items
            // Vanilla有许多这样的有用方法，要好好利用它们！这些方法可以替代设置Item.createTile和Item.placeStyle，并设置一些在所有可放置物品中都常见的值。
            Item.DefaultToPlaceableTile(ModContent.TileType<AnalysisContent.Tiles.Furniture.MinionBossTrophy>());

            Item.width = 32;
            Item.height = 32;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(0, 1);
        }
    }
}
