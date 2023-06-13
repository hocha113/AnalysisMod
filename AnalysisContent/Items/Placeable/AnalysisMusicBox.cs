using AnalysisMod.AnalysisContent.Tiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Placeable
{
    public class AnalysisMusicBox : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.CanGetPrefixes[Type] = false; // music boxes can't get prefixes in vanilla
                                                      // ���ֺ���ԭ�����޷����ǰ׺��
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox; // recorded music boxes transform into the basic form in shimmer
                                                                        // ¼�Ƶ����ֺл�ת��Ϊ������ʽ����˸ʱҲ����ˡ�

            // The following code links the music box's item and tile with a music track:
            //   When music with the given ID is playing, equipped music boxes have a chance to change their id to the given item type.
            //   When an item with the given item type is equipped, it will play the music that has musicSlot as its ID.
            //   When a tile with the given type and Y-frame is nearby, if its X-frame is >= 36, it will play the music that has musicSlot as its ID.
            // When getting the music slot, you should not add the file extensions!

            //���´��뽫��Ʒ��ͼ�������ֹ������������           
            //�����ž��и���ID������ʱ��װ�������ֺе�����л��Ὣ��ID����Ϊ������Ʒ���͡�            
            //��װ�����и�����Ʒ���͵���Ʒʱ���������ž���musicSlot��Ϊ��ID�����֡�            
            //����������ָ�����ͺ�Y֡��ͼ�飬������X֡ >= 36�����������ž���musicSlot��Ϊ��ID�����֡�           
            //��ȡ��Ƶ��ʱ����������ļ���չ����
            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Assets/Music/MysteriousMystery"), ModContent.ItemType<AnalysisMusicBox>(), ModContent.TileType<AnalysisMusicBoxTile>());
        }

        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(ModContent.TileType<AnalysisMusicBoxTile>(), 0);
        }
    }
}
