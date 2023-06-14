using AnalysisMod.AnalysisContent.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AnalysisMod.AnalysisContent.Tiles.Plants
{
    public class AnalysisSapling : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorValidTiles = new[] { ModContent.TileType<AnalysisBlock>(), TileID.Gold };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.DrawFlipHorizontal = true;
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.RandomStyleRange = 3;
            TileObjectData.newTile.StyleMultiplier = 3;

            //TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
            //TileObjectData.newSubTile.AnchorValidTiles = new int[] { TileType<AnalysisSand>() };
            //TileObjectData.addSubTile(1);

            TileObjectData.addTile(Type);

            AddMapEntry(new Color(200, 200, 200), Language.GetText("MapObject.Sapling"));

            TileID.Sets.TreeSapling[Type] = true;
            TileID.Sets.CommonSapling[Type] = true;
            TileID.Sets.SwaysInWindBasic[Type] = true;
            TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]); // Make this tile interact with golf balls in the same way other plants do
                                                                                       // 让这个方块与其他植物一样与高尔夫球互动

            DustType = ModContent.DustType<Sparkle>();

            AdjTiles = new int[] { TileID.Saplings };
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void RandomUpdate(int i, int j)
        {
            // A random chance to slow down growth
            // 有随机几率减缓生长速度
            if (!WorldGen.genRand.NextBool(20))
            {
                return;
            }

            Tile tile = Framing.GetTileSafely(i, j); // Safely get the tile at the given coordinates
                                                     // 安全地获取给定坐标的方块

            bool growSucess; // A bool to see if the tree growing was sucessful.
                             // 一个布尔值来判断树苗是否成功种植。

            // Style 0 is for the AnalysisTree sapling, and style 1 is for AnalysisPalmTree, so here we check frameX to call the correct method.
            // Any pixels before 54 on the tilesheet are for AnalysisTree while any pixels above it are for AnalysisPalmTree

            // 样式0用于AnalysisTree树苗，样式1用于AnalysisPalmTree棕榈树，因此我们检查frameX以调用正确的方法。
            // 在图块表中54之前的像素是为AnalysisTree而上面的像素则是为AnalysisPalmTree。
            if (tile.TileFrameX < 54)
            {
                growSucess = WorldGen.GrowTree(i, j);
            }
            else
            {
                growSucess = WorldGen.GrowPalmTree(i, j);
            }

            // A flag to check if a player is near the sapling
            // 一个标志来检查玩家是否靠近树苗
            bool isPlayerNear = WorldGen.PlayerLOS(i, j);

            //If growing the tree was a sucess and the player is near, show growing effects
            // 如果种植树木成功并且玩家在附近，则显示生长效果。
            if (growSucess && isPlayerNear)
            {
                WorldGen.TreeGrowFXCheck(i, j);
            }
        }

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects effects)
        {
            if (i % 2 == 1)
            {
                effects = SpriteEffects.FlipHorizontally;
            }
        }
    }
}