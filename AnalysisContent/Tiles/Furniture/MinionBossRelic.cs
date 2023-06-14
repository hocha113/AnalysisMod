using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Enums;
using System;
using ReLogic.Content;
using Terraria.Localization;

namespace AnalysisMod.AnalysisContent.Tiles.Furniture
{
    // Common code for a Master Mode boss relic
    // Supports optional Item.placeStyle handling if you wish to add more relics but use the same tile type (then it would be wise to name this class something more generic like BossRelic)
    // If you want to add more relics but don't want to use the Item.placeStyle approach, see the inheritance Analysis at the bottom of the file

    // 大师模式Boss遗物的通用代码
    // 如果您想添加更多遗物但使用相同的图块类型，则支持可选的Item.placeStyle处理（那么将智能地命名此类为更通用的名称，如BossRelic）
    // 如果要添加更多遗物但不想使用Item.placeStyle方法，请参见文件底部的继承分析
    public class MinionBossRelic : ModTile
    {
        public const int FrameWidth = 18 * 3;
        public const int FrameHeight = 18 * 4;
        public const int HorizontalFrames = 1;
        public const int VerticalFrames = 1; // Optional: Increase this number to match the amount of relics you have on your extra sheet, if you choose to use the Item.placeStyle approach
                                             // 可选：增加此数字以匹配额外表格上拥有的遗物数量，如果选择使用Item.placeStyle方法

        public Asset<Texture2D> RelicTexture;

        // Every relic has its own extra floating part, should be 50x50. Optional: Expand this sheet if you want to add more, stacked vertically
        // If you do not use the Item.placeStyle approach, and you extend from this class, you can override this to point to a different texture

        // 每个遗物都有自己额外浮动部分，应为50x50。 可选：如果要添加更多内容，则可以展开此工作表，并垂直堆叠
        // 如果您不使用Item.placeStyle方法，并且从该类扩展，则可以覆盖它以指向不同的纹理
        public virtual string RelicTextureName => "AnalysisMod/AnalysisContent/Tiles/Furniture/MinionBossRelic";

        // All relics use the same pedestal texture, this one is copied from vanilla
        // 所有遗物都使用相同的基座纹理，这个是从原版复制过来的
        public override string Texture => "AnalysisMod/AnalysisContent/Tiles/Furniture/RelicPedestal";

        public override void Load()
        {
            if (!Main.dedServ)
            {
                RelicTexture = ModContent.Request<Texture2D>(RelicTextureName);
            }
        }

        public override void Unload()
        {
            // Unload the extra texture displayed on the pedestal
            // 卸载显示在基座上额外纹理
            RelicTexture = null;
        }

        public override void SetStaticDefaults()
        {
            Main.tileShine[Type] = 400; // Responsible for golden particles
                                        // 负责金色粒子效果

            Main.tileFrameImportant[Type] = true; // Any multitile requires this
                                                  // 任何多图块都需要这个东西。

            TileID.Sets.InteractibleByNPCs[Type] = true; // Town NPCs will palm their hand at this tile
                                                         // 城镇NPC会在这个图块处做出抚摸动作。

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4); // Relics are 3x4
                                                                      // 遗物大小为3x4.

            TileObjectData.newTile.LavaDeath = false; // Does not break when lava touches it
                                                      // 当岩浆接触时不会破裂。

            TileObjectData.newTile.DrawYOffset = 2; // So the tile sinks into the ground
                                                    // 因此该图块会陷入地面。

            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft; // Player faces to the left
                                                                              // 玩家朝向左侧

            TileObjectData.newTile.StyleHorizontal = false; // Based on how the alternate sprites are positioned on the sprite (by default, true)
                                                            // 基于备用精灵图在精灵图上的位置（默认为true）

            // This controls how styles are laid out in the texture file. This tile is special in that all styles will use the same texture section to draw the pedestal.
            // 这控制了样式在纹理文件中的布局。这个图块很特殊，因为所有样式都将使用相同的纹理部分来绘制基座。
            TileObjectData.newTile.StyleWrapLimitVisualOverride = 2;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.styleLineSkipVisualOverride = 0; // This forces the tile preview to draw as if drawing the 1st style.
                                                                    // 这强制图块预览绘制为绘制第一个样式。

            // Register an alternate tile data with flipped direction
            // 注册一个翻转方向的替代图块数据
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile); // Copy everything from above, saves us some code
                                                                          // 复制以上所有内容，可以节省一些代码

            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight; // Player faces to the right
                                                                                    // 玩家面对右侧
            TileObjectData.addAlternate(1);

            // Register the tile data itself
            // 注册图块数据本身
            TileObjectData.addTile(Type);

            // Register map name and color
            // "MapObject.Relic" refers to the translation key for the vanilla "Relic" text

            // 注册地图名称和颜色
            // "MapObject.Relic"是指原版“Relic”文本的翻译键
            AddMapEntry(new Color(233, 207, 94), Language.GetText("MapObject.Relic"));
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            return false;
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            // This forces the tile to draw the pedestal even if the placeStyle differs. 
            // 这将强制绘制基座，即使placeStyle不同。
            tileFrameX %= FrameWidth; // Clamps the frameX
                                      // 夹紧frameX
            tileFrameY %= FrameHeight * 2; // Clamps the frameY (two horizontally aligned place styles, hence * 2)
                                           // 夹紧frameY（两个水平对齐的放置样式，因此* 2）
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            // Since this tile does not have the hovering part on its sheet, we have to animate it ourselves
            // Therefore we register the top-left of the tile as a "special point"
            // This allows us to draw things in SpecialDraw

            //由于该图块在其表中没有悬停部分，因此我们必须自己进行动画处理
            // 因此，我们将左上角的瓷砖注册为“特殊点”
            //这允许我们在SpecialDraw中绘制东西
            if (drawData.tileFrameX % FrameWidth == 0 && drawData.tileFrameY % FrameHeight == 0)
            {
                Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
            }
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            // This is lighting-mode specific, always include this if you draw tiles manually
            //如果您手动绘制图块，则与灯光模式有关，请始终包括此内容
            Vector2 offScreen = new Vector2(Main.offScreenRange);
            if (Main.drawToScreen)
            {
                offScreen = Vector2.Zero;
            }

            // Take the tile, check if it actually exists

            Point p = new Point(i, j);
            Tile tile = Main.tile[p.X, p.Y];
            if (tile == null || !tile.HasTile)
            {
                return;
            }

            // Get the initial draw parameters
            //获取初始绘制参数
            Texture2D texture = RelicTexture.Value;

            int frameY = tile.TileFrameX / FrameWidth; // Picks the frame on the sheet based on the placeStyle of the item
                                                       // 根据物品的placeStyle选择板上的框架

            Rectangle frame = texture.Frame(HorizontalFrames, VerticalFrames, 0, frameY);

            Vector2 origin = frame.Size() / 2f;
            Vector2 worldPos = p.ToWorldCoordinates(24f, 64f);

            Color color = Lighting.GetColor(p.X, p.Y);

            bool direction = tile.TileFrameY / FrameHeight != 0; // This is related to the alternate tile data we registered before
                                                                 // 这与我们之前注册的备用瓷砖数据相关

            SpriteEffects effects = direction ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // Some math magic to make it smoothly move up and down over time
            //一些数学技巧使它能够随着时间平稳地上下移动
            const float TwoPi = (float)Math.PI * 2f;
            float offset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 5f);
            Vector2 drawPos = worldPos + offScreen - Main.screenPosition + new Vector2(0f, -40f) + new Vector2(0f, offset * 4f);

            // Draw the main texture
            // 绘制主纹理
            spriteBatch.Draw(texture, drawPos, frame, color, 0f, origin, 1f, effects, 0f);

            // Draw the periodic glow effect
            // 绘制周期性发光效果
            float scale = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 2f) * 0.3f + 0.7f;
            Color effectColor = color;
            effectColor.A = 0;
            effectColor = effectColor * 0.1f * scale;
            for (float num5 = 0f; num5 < 1f; num5 += 355f / (678f * (float)Math.PI))
            {
                spriteBatch.Draw(texture, drawPos + (TwoPi * num5).ToRotationVector2() * (6f + offset * 2f), frame, effectColor, 0f, origin, 1f, effects, 0f);
            }
        }
    }

    // If you want to make more relics but do not use the Item.placeStyle approach, you can use inheritance to avoid using duplicate code:
    // Your tile code would then inherit from the MinionBossRelic class (which you should make abstract) and should look like this:

    // 如果要创建更多文物但不使用Item.placeStyle方法，则可以使用继承来避免重复代码：
    //然后你的tile代码会从MinionBossRelic类（你应该抽象化）中继承，并且应该像这样：
    /*
	public class MyBossRelic : MinionBossRelic
	{
		public override string RelicTextureName => "AnalysisMod/AnalysisContent/Tiles/Furniture/MyBossRelic";

		public override void SetStaticDefaults() {
			base.SetStaticDefaults();
		}
	}
	*/

    // Your item code would then just use the MyBossRelic tile type, and keep placeStyle on 0
    // The textures for MyBossRelic item/tile have to be supplied separately

    // 然后您的项目代码只需使用MyBossRelic tile类型，并保持placeStyle为0
    // MyBossRelic item / tile 的纹理必须单独提供
}
