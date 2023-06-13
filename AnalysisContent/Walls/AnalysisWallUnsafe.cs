using AnalysisMod.AnalysisContent.Dusts;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Walls
{
	public class AnalysisWallUnsafe : ModWall
	{
		public override void SetStaticDefaults() {
			// As an Analysis of an unsafe wall, "Main.wallHouse[Type] = true;" is omitted.

			DustType = ModContent.DustType<Sparkle>();

			AddMapEntry(new Color(150, 150, 150));

			// We need to manually register the item drop, since no item places this wall. This wall can only be obtained by using AnalysisSolution on natural spider walls.
			RegisterItemDrop(ModContent.ItemType<Items.Placeable.AnalysisWall>());
		}

		public override void NumDust(int i, int j, bool fail, ref int num) {
			num = fail ? 1 : 3;
		}
	}
}