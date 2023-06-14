using AnalysisMod.AnalysisContent.Dusts;
using AnalysisMod.AnalysisContent.Items.Placeable.Furniture;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AnalysisMod.AnalysisContent.Tiles.Furniture
{
    public class AnalysisDoorOpen : ModTile
    {
        public override void SetStaticDefaults()
        {
            // Properties
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileLavaDeath[Type] = true;
            Main.tileNoSunLight[Type] = true;
            TileID.Sets.HousingWalls[Type] = true; // needed for non-solid blocks to count as walls
                                                   // 需要让非实心方块被视为墙壁

            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.CloseDoorID[Type] = ModContent.TileType<AnalysisDoorClosed>();

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);

            DustType = ModContent.DustType<Sparkle>();
            AdjTiles = new int[] { TileID.OpenDoor };
            // Tiles usually drop their corresponding item automatically, but RegisterItemDrop is needed here since the AnalysisDoor item places AnalysisDoorClosed, not this tile.
            // 瓷砖通常会自动掉落相应的物品，但由于 AnalysisDoor 物品放置的是 AnalysisDoorClosed 而不是这个瓷砖，因此需要使用 RegisterItemDrop。
            RegisterItemDrop(ModContent.ItemType<AnalysisDoor>(), 0);
            TileID.Sets.CloseDoorID[Type] = ModContent.TileType<AnalysisDoorClosed>();

            // Names
            AddMapEntry(new Color(200, 200, 200), Language.GetText("MapObject.Door"));

            // Placement
            // In addition to copying from the TileObjectData.Something templates, modders can copy from specific tile types. CopyFrom won't copy subtile data, so style specific properties won't be copied, such as how Obsidian doors are immune to lava.
            // 除了从 TileObjectData.Something 模板中复制外，modder 还可以从特定的瓷砖类型中复制。CopyFrom 不会复制子图块数据，因此风格特定属性不会被复制，例如黑曜石门对岩浆免疫。
            TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.OpenDoor, 0));
            /* This is what is copied from the OpenDoor tile
             * 以下内容是从 OpenDoor 瓷砖中复制而来：
             * 
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.Origin = new Point16(0, 0);
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 0);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.StyleMultiplier = 2;
			TileObjectData.newTile.StyleWrapLimit = 2;
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(0, 1);
			TileObjectData.addAlternate(0);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(0, 2);
			TileObjectData.addAlternate(0);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(1, 0);
			TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 1);
			TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 1);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.addAlternate(1);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(1, 1);
			TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 1);
			TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 1);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.addAlternate(1);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(1, 2);
			TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 1);
			TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 1);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.addAlternate(1);
			*/
            TileObjectData.addTile(Type);
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        {
            return true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 1;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<AnalysisDoor>();
        }
    }
}