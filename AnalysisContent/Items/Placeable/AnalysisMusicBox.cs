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
                                                      // 音乐盒在原版中无法获得前缀。
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox; // recorded music boxes transform into the basic form in shimmer
                                                                        // 录制的音乐盒会转化为基本形式，闪烁时也是如此。

            // The following code links the music box's item and tile with a music track:
            //   When music with the given ID is playing, equipped music boxes have a chance to change their id to the given item type.
            //   When an item with the given item type is equipped, it will play the music that has musicSlot as its ID.
            //   When a tile with the given type and Y-frame is nearby, if its X-frame is >= 36, it will play the music that has musicSlot as its ID.
            // When getting the music slot, you should not add the file extensions!

            //以下代码将物品和图块与音乐轨道链接起来：           
            //当播放具有给定ID的音乐时，装备了音乐盒的玩家有机会将其ID更改为给定物品类型。            
            //当装备具有给定物品类型的物品时，它将播放具有musicSlot作为其ID的音乐。            
            //当附近存在指定类型和Y帧的图块，并且其X帧 >= 36，则它将播放具有musicSlot作为其ID的音乐。           
            //获取音频槽时，请勿添加文件扩展名！
            MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, "Assets/Music/MysteriousMystery"), ModContent.ItemType<AnalysisMusicBox>(), ModContent.TileType<AnalysisMusicBoxTile>());
        }

        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(ModContent.TileType<AnalysisMusicBoxTile>(), 0);
        }
    }
}
