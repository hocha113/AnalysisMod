using AnalysisMod.AnalysisContent.Dusts;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Walls
{
	public class AnalysisWallUnsafe : ModWall
	{
		public override void SetStaticDefaults() {
            // As an Analysis of an unsafe wall, "Main.wallHouse[Type] = true;" is omitted.
            // 对于一堵不安全的墙壁，"Main.wallHouse[Type] = true;" 被省略了。

            DustType = ModContent.DustType<Sparkle>();

			AddMapEntry(new Color(150, 150, 150));

            // We need to manually register the item drop, since no item places this wall. This wall can only be obtained by using AnalysisSolution on natural spider walls.
            // 我们需要手动注册物品掉落，因为没有任何物品会放置这种墙。只有通过对自然蜘蛛墙使用分析溶液才能获取到这种墙。
            RegisterItemDrop(ModContent.ItemType<Items.Placeable.AnalysisWall>());
		}

		public override void NumDust(int i, int j, bool fail, ref int num) {
			num = fail ? 1 : 3;
		}
	}
}