using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.Utilities;

namespace AnalysisMod.AnalysisContent.Tiles
{
    // Campfires are special tiles that support the block swap feature and the biome torch feature. AnalysisSurfaceBiome shows how the biome campfire is assigned.
    //篝火是支持方块交换和生物群系火把功能的特殊图块。AnalysisSurfaceBiome 显示了如何分配生物群系篝火。
    public class AnalysisCampfire : ModTile
    {
        private Asset<Texture2D> flameTexture;

        public override void SetStaticDefaults()
        {
            // Properties
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileWaterDeath[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.InteractibleByNPCs[Type] = true;
            TileID.Sets.Campfire[Type] = true;

            DustType = -1; // No dust when mined.
                           // 挖掘时不会产生灰尘。

            AdjTiles = new int[] { TileID.Campfire };

            // Placement
            TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.Campfire, 0));
            /*  This is what is copied from the Campfire tile
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.StyleWrapLimit = 16;
			TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
			TileObjectData.newTile.LavaPlacement = LiquidPlacement.NotAllowed;
			TileObjectData.newTile.WaterDeath = true;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.newTile.DrawYOffset = 2;
			*/
            TileObjectData.newTile.StyleLineSkip = 9; // This needs to be added to work for modded tiles.
                                                      // 需要添加以适用于模组图块。
            TileObjectData.addTile(Type);

            // Etc
            AddMapEntry(new Color(254, 121, 2), Language.GetText("ItemName.Campfire"));

            // Assets
            if (!Main.dedServ)
            {
                flameTexture = ModContent.Request<Texture2D>(Texture + "_Flame");
            }
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            Main.SceneMetrics.HasCampfire = true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;

            int style = TileObjectData.GetTileStyle(Main.tile[i, j]);
            player.cursorItemIconID = TileLoader.GetItemDropFromTypeAndStyle(Type, style);
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        {
            return true;
        }

        public override bool RightClick(int i, int j)
        {
            SoundEngine.PlaySound(SoundID.Mech, new Vector2(i * 16, j * 16));
            ToggleTile(i, j);
            return true;
        }

        public override void HitWire(int i, int j)
        {
            ToggleTile(i, j);
        }

        // ToggleTile is a method that contains code shared by HitWire and RightClick, since they both toggle the state of the tile.
        // Note that TileFrameY doesn't necessarily match up with the image that is drawn, AnimateTile and AnimateIndividualTile contribute to the drawing decisions.

        //ToggleTile 是一个包含 HitWire 和 RightClick 共享代码的方法，因为它们都切换图块状态。
        //请注意，TileFrameY 不一定与绘制的图片相匹配，AnimateTile 和 AnimateIndividualTile 对绘制决策做出贡献。
        public void ToggleTile(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            int topX = i - tile.TileFrameX % 54 / 18;
            int topY = j - tile.TileFrameY % 36 / 18;

            short frameAdjustment = (short)(tile.TileFrameY >= 36 ? -36 : 36);

            for (int x = topX; x < topX + 3; x++)
            {
                for (int y = topY; y < topY + 2; y++)
                {
                    Main.tile[x, y].TileFrameY += frameAdjustment;

                    if (Wiring.running)
                    {
                        Wiring.SkipWire(x, y);
                    }
                }
            }

            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                NetMessage.SendTileSquare(-1, topX, topY, 3, 2);
            }
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            if (++frameCounter >= 4)
            {
                frameCounter = 0;
                frame = ++frame % 8;
            }
        }
        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            var tile = Main.tile[i, j];
            if (tile.TileFrameY < 36)
            {
                frameYOffset = Main.tileFrame[type] * 36;
            }
            else
            {
                frameYOffset = 252;
            }
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (Main.gamePaused || !Main.instance.IsActive)
            {
                return;
            }
            if (!Lighting.UpdateEveryFrame || new FastRandom(Main.TileFrameSeed).WithModifier(i, j).Next(4) == 0)
            {
                Tile tile = Main.tile[i, j];
                // Only emit dust from the top tiles, and only if toggled on. This logic limits dust spawning under different conditions.
                // 仅从顶部图块发射灰尘，并且仅在开启时才发射。此逻辑限制了在不同条件下生成灰尘的情况。
                if (tile.TileFrameY == 0 && Main.rand.NextBool(3) && (Main.drawToScreen && Main.rand.NextBool(4) || !Main.drawToScreen))
                {
                    Dust dust = Dust.NewDustDirect(new Vector2(i * 16 + 2, j * 16 - 4), 4, 8, DustID.Smoke, 0f, 0f, 100);
                    if (tile.TileFrameX == 0)
                        dust.position.X += Main.rand.Next(8);

                    if (tile.TileFrameX == 36)
                        dust.position.X -= Main.rand.Next(8);

                    dust.alpha += Main.rand.Next(100);
                    dust.velocity *= 0.2f;
                    dust.velocity.Y -= 0.5f + Main.rand.Next(10) * 0.1f;
                    dust.fadeIn = 0.5f + Main.rand.Next(10) * 0.1f;
                }
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameY < 36)
            {
                float pulse = Main.rand.Next(28, 42) * 0.005f;
                pulse += (270 - Main.mouseTextColor) / 700f;
                r = 0.1f + pulse;
                g = 0.9f + pulse;
                b = 0.3f + pulse;
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            var tile = Main.tile[i, j];
            if (tile.TileFrameY < 36)
            {
                Color color = new Color(255, 255, 255, 0);

                Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
                if (Main.drawToScreen)
                {
                    zero = Vector2.Zero;
                }

                int width = 16;
                int offsetY = 0;
                int height = 16;
                short frameX = tile.TileFrameX;
                short frameY = tile.TileFrameY;
                int addFrX = 0;
                int addFrY = 0;

                TileLoader.SetDrawPositions(i, j, ref width, ref offsetY, ref height, ref frameX, ref frameY); // calculates the draw offsets
                                                                                                               // 计算绘制偏移量

                TileLoader.SetAnimationFrame(Type, i, j, ref addFrX, ref addFrY); // calculates the animation offsets
                                                                                  // 计算动画偏移量

                Rectangle drawRectangle = new Rectangle(tile.TileFrameX, tile.TileFrameY + addFrY, 16, 16);

                // The flame is manually drawn separate from the tile texture so that it can be drawn at full brightness.
                // 火焰手动单独绘制而不是与图块纹理混合在一起，以便可以完全亮度地绘制。
                Main.spriteBatch.Draw(flameTexture.Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y + offsetY) + zero, drawRectangle, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }
    }
}
