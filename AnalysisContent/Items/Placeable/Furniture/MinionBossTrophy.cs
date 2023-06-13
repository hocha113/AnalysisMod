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
            // Vanilla��������������÷�����Ҫ�ú��������ǣ���Щ���������������Item.createTile��Item.placeStyle��������һЩ�����пɷ�����Ʒ�ж�������ֵ��
            Item.DefaultToPlaceableTile(ModContent.TileType<AnalysisContent.Tiles.Furniture.MinionBossTrophy>());

            Item.width = 32;
            Item.height = 32;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(0, 1);
        }
    }
}
