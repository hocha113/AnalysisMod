using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AnalysisMod.AnalysisContent.Tiles
{
    internal class AnalysisFishingCrate : ModTile
    {
        public override void SetStaticDefaults()
        {
            // Properties
            Main.tileFrameImportant[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileTable[Type] = true;

            // Placement
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.CoordinateHeights = new int[2] { 16, 18 };
            TileObjectData.newTile.StyleHorizontal = true; // Optional, if you add more placeStyles for the item
                                                           // 如果您为该项目添加了更多的placeStyles，则此项为可选。
            TileObjectData.addTile(Type);

            // Etc
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(200, 200, 200), name);
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            return false;
        }
    }
}
