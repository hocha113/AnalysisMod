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

            // Vanilla��������������÷�����Ҫ�ú����ã�������������������Item.createTile��Item.placeStyle�Լ�����һЩ�����пɷ�����Ʒ��ͨ�õ�ֵ��
            // ���������ö�����ﹲ����ͬ��ͼ�����ͣ��������ͼ���������ݣ�����ô���÷��Ĭ��Ϊ0���ͺ���Ҫ��
            Item.DefaultToPlaceableTile(ModContent.TileType<AnalysisContent.Tiles.Furniture.MinionBossRelic>(), 0);

            Item.width = 30;
            Item.height = 40;
            Item.rare = ItemRarityID.Master;
            Item.master = true; // This makes sure that "Master" displays in the tooltip, as the rarity only changes the item name color
                                // ��ȷ���ˡ�Master����ʾ�ڹ�����ʾ�У���Ϊϡ�ж�ֻ��ı���Ʒ������ɫ��

            Item.value = Item.buyPrice(0, 5);
        }
    }
}
