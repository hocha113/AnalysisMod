using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Tiles
{
    public class AnalysisSlopeTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.CanBeSloped[Type] = true; // allow this tile to be sloped, because it isn't solid
                                                  // 允许这个瓦片倾斜，因为它不是实心的
        }

        public override bool Slope(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            tile.TileFrameX = (short)((tile.TileFrameX + 18) % 72);
            SoundEngine.PlaySound(SoundID.MenuTick);
            return false;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            return false;
        }
    }
}
