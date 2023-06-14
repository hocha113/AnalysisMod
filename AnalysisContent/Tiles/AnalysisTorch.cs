using AnalysisMod.AnalysisContent.Biomes;
using AnalysisMod.AnalysisContent.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AnalysisMod.AnalysisContent.Tiles
{
    // Torches are special tiles that support the block swap feature and the biome torch feature. AnalysisSurfaceBiome shows how the biome torch is assigned.
    //火把是支持方块交换和生物群系火把功能的特殊瓷砖。AnalysisSurfaceBiome显示了如何分配生物群系火把。
    public class AnalysisTorch : ModTile
    {
        private Asset<Texture2D> flameTexture;

        public override void SetStaticDefaults()
        {
            // Properties
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileNoAttach[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileWaterDeath[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.DisableSmartInteract[Type] = true;
            TileID.Sets.Torch[Type] = true;

            DustType = ModContent.DustType<Sparkle>();
            AdjTiles = new int[] { TileID.Torches };

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

            // Placement
            TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.Torches, 0));
            /*  This is what is copied from the Torches tile
			TileObjectData.newTile.CopyFrom(TileObjectData.StyleTorch);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.StyleTorch);
			TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.Tree | AnchorType.AlternateTile, TileObjectData.newTile.Height, 0);
			TileObjectData.newAlternate.AnchorAlternateTiles = new[] { 124, 561, 574, 575, 576, 577, 578 };
			TileObjectData.addAlternate(1);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.StyleTorch);
			TileObjectData.newAlternate.AnchorRight = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.Tree | AnchorType.AlternateTile, TileObjectData.newTile.Height, 0);
			TileObjectData.newAlternate.AnchorAlternateTiles = new[] { 124, 561, 574, 575, 576, 577, 578 };
			TileObjectData.addAlternate(2);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.StyleTorch);
			TileObjectData.newAlternate.AnchorWall = true;
			TileObjectData.addAlternate(0);
			*/

            // This code adds style-specific properties to style 1. Style 1 is used by AnalysisWaterTorch. This code allows the tile to be placed in liquids. More info can be found in the guide:
            //此代码将样式特定属性添加到样式1中。样式1由AnalysisWaterTorch使用。此代码允许在液体中放置该瓷砖。更多信息可以在指南中找到：
            // https://github.com/tModLoader/tModLoader/wiki/Basic-Tile#newsubtile-and-newalternate
            TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
            TileObjectData.newSubTile.LinkedAlternates = true;
            TileObjectData.newSubTile.WaterDeath = false;
            TileObjectData.newSubTile.LavaDeath = false;
            TileObjectData.newSubTile.WaterPlacement = LiquidPlacement.Allowed;
            TileObjectData.newSubTile.LavaPlacement = LiquidPlacement.Allowed;
            TileObjectData.addSubTile(1);

            TileObjectData.addTile(Type);

            // Etc
            AddMapEntry(new Color(200, 200, 200), Language.GetText("ItemName.Torch"));

            // Assets
            if (!Main.dedServ)
            {
                flameTexture = ModContent.Request<Texture2D>("AnalysisMod/AnalysisContent/Tiles/AnalysisTorch_Flame");
            }
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;

            // We can determine the item to show on the cursor by getting the tile style and looking up the corresponding item drop.
            //我们可以通过获取图块样式并查找相应的掉落物品来确定要在光标上显示的项目。
            int style = TileObjectData.GetTileStyle(Main.tile[i, j]);
            player.cursorItemIconID = TileLoader.GetItemDropFromTypeAndStyle(Type, style);
        }

        public override float GetTorchLuck(Player player)
        {
            // GetTorchLuck is called when there is an AnalysisTorch nearby the client player
            // In most use-cases you should return 1f for a good luck torch, or -1f for a bad luck torch.
            // You can also add a smaller amount (eg 0.5) for a smaller postive/negative luck impact.
            // Remember that the overall torch luck is decided by every torch around the player, so it may be wise to have a smaller amount of luck impact.
            // Multiple Analysis torches on screen will have no additional effect.

            // 我们可以通过获取图块样式并查找相应的掉落物品来确定要在光标上显示的项目。
            // 当客户端玩家附近有AnalysisTorch时，会调用GetTorchLuck
            // 在大多数情况下，您应返回1f以获得好运火把，或-1f以获得不幸运火把。
            // 您还可以添加较小的金额（例如0.5）以获得较小的正面/负面运气影响。
            // 请记住，整个火把运气是由玩家周围所有火把决定的，因此可能明智地具有较小数量的幸运影响。
            // 屏幕上出现多个分析灯笼将没有额外效果。

            // Positive and negative luck are accumulated separately and then compared to some fixed limits in vanilla to determine overall torch luck.
            // Postive luck is capped at 1, any value higher won't make any difference and negative luck is capped at 2.
            // A negative luck of 2 will cancel out all torch luck bonuses.

            //积极和消极运气分别累积，然后与香草中一些固定限制进行比较，以确定整体火把运气。
            //积极的幸运值上限为1，任何更高的值都不会有任何影响，而负面幸运值上限为2。
            //负2点运气将取消所有火把幸运奖金。

            // The influence positive torch luck can have overall is 0.1 (if positive luck is any number less than 1) or 0.2 (if positive luck is greater than or equal to 1)
            //正面火把幸运可以产生0.1（如果正面幸运是小于1的任意数字）或0.2（如果正面幸运大于或等于1）

            bool inAnalysisUndergroundBiome = player.InModBiome<AnalysisUndergroundBiome>();
            return inAnalysisUndergroundBiome ? 1f : -0.1f; // AnalysisTorch gives maximum positive luck when in Analysis biome, otherwise a small negative luck
                                                            // 在分析生物群系中时，AnalysisTorch提供最大的积极性好运；否则会有一些小消极性不利|
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = Main.rand.Next(1, 3);

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];

            // If the torch is on
            // 如果火炬处于开启状态
            if (tile.TileFrameX < 66)
            {
                int style = TileObjectData.GetTileStyle(Main.tile[i, j]);
                // Make it emit the following light.
                // 让它发出以下光线。
                if (style == 0)
                {
                    r = 0.9f;
                    g = 0.9f;
                    b = 0.9f;
                }
                else if (style == 1)
                {
                    r = 0.5f;
                    g = 1.5f;
                    b = 0.5f;
                }
            }
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            // This code slightly lowers the draw position if there is a solid tile above, so the flame doesn't overlap that tile. Terraria torches do this same logic.
            // 如果上方有实心图块，则此代码会稍微降低绘制位置，以便火焰不重叠该图块。 Terraria 火把使用相同逻辑。
            offsetY = 0;

            if (WorldGen.SolidTile(i, j - 1))
            {
                offsetY = 4;
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            // The following code draws multiple flames on top our placed torch.
            // 以下代码在我们放置的灯笼顶部绘制多个火焰。

            int offsetY = 0;

            if (WorldGen.SolidTile(i, j - 1))
            {
                offsetY = 4;
            }

            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);

            if (Main.drawToScreen)
            {
                zero = Vector2.Zero;
            }

            ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (uint)i); // Don't remove any casts.
                                                                                    // 请勿删除任何转换。
            Color color = new Color(100, 100, 100, 0);
            int width = 20;
            int height = 20;
            var tile = Main.tile[i, j];
            int frameX = tile.TileFrameX;
            int frameY = tile.TileFrameY;
            int style = TileObjectData.GetTileStyle(Main.tile[i, j]);
            if (style == 1)
            {
                // AnalysisWaterTorch should be a bit greener.
                // AnalysisWaterTorch应该再绿一点。
                color.G = 255;
            }

            for (int k = 0; k < 7; k++)
            {
                float xx = Utils.RandomInt(ref randSeed, -10, 11) * 0.15f;
                float yy = Utils.RandomInt(ref randSeed, -10, 1) * 0.35f;

                spriteBatch.Draw(flameTexture.Value, new Vector2(i * 16 - (int)Main.screenPosition.X - (width - 16f) / 2f + xx, j * 16 - (int)Main.screenPosition.Y + offsetY + yy) + zero, new Rectangle(frameX, frameY, width, height), color, 0f, default, 1f, SpriteEffects.None, 0f);
            }
        }
    }
}