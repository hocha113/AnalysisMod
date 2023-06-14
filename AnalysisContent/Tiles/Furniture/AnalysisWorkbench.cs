using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AnalysisMod.AnalysisContent.Tiles.Furniture
{
    public class AnalysisWorkbench : ModTile
    {
        public override void SetStaticDefaults()
        {
            // Properties
            Main.tileTable[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.IgnoredByNpcStepUp[Type] = true; // This line makes NPCs not try to step up this tile during their movement. Only use this for furniture with solid tops.
                                                         // 这行代码可以让NPC在移动时不会尝试踏上此方格。只有当家具顶部是实心的时才使用。

            DustType = ModContent.DustType<Dusts.Sparkle>();
            AdjTiles = new int[] { TileID.WorkBenches };

            // Placement
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
            TileObjectData.newTile.CoordinateHeights = new[] { 18 };
            TileObjectData.addTile(Type);

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);

            // Etc
            AddMapEntry(new Color(200, 200, 200), Language.GetText("ItemName.WorkBench"));
        }

        public override void NumDust(int x, int y, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}