using AnalysisMod.AnalysisContent.Tiles.Furniture;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using AnalysisMod.AnalysisContent.Items;

namespace AnalysisMod.AnalysisContent.Tiles
{
    internal class AnalysisAnimatedTile : ModTile
    {
        // If you want to know more about tiles, please follow this link
        // 如果您想了解更多关于瓷砖的信息，请点击此链接：

        // https://github.com/tModLoader/tModLoader/wiki/Basic-Tile
        public override void SetStaticDefaults()
        {
            // If a tile is a light source
            // 如果一个瓷砖是光源
            Main.tileLighted[Type] = true;
            // This changes a Framed tile to a FrameImportant tile
            // For modders, just remember to set this to true when you make a tile that uses a TileObjectData
            // Or basically all tiles that aren't like dirt, ores, or other basic building tiles

            // 这将把一个框架式的瓷砖变成一个重要框架式的瓷砖
            // 对于modders，当你制作使用TileObjectData的图块时，请记得将其设置为true
            // 或者基本上所有不像泥土、矿物或其他基本建筑图块的图块
            Main.tileFrameImportant[Type] = true;

            // Set to True if you'd like your tile to die if hit by lava
            // 如果您希望您的图块在被岩浆击中后死亡，则设置为True
            Main.tileLavaDeath[Type] = true;

            // Use this to utilize an existing template
            // The names of styles are self explanatory usually (you can see all existing templates at the link mentioned earlier)

            // 使用此功能来利用现有模板
            // 样式名称通常是自我说明性质（您可以在前面提到过的链接中查看所有现有模板）
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);

            // This last call adds a new tile
            // Before that, you can make some changes to newTile like height, origin and etc.

            // 最后一次调用添加新图块
            // 在那之前，您可以对newTile进行一些更改，如高度、原点等。
            TileObjectData.addTile(Type);

            // AddMapEntry is for setting the color and optional text associated with the Tile when viewed on the map
            // AddMapEntry 用于在地图上查看该 Tile 时设置颜色和可选文本。
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(238, 145, 105), name);

            // Can't use this since texture is vertical
            // 无法使用这个因为纹理是垂直方向的。

            // AnimationFrameHeight = 56;
        }

        // Our textures animation frames are arranged horizontally, which isn't typical, so here we specify animationFrameWidth which we use later in AnimateIndividualTile
        // 我们的纹理动画帧按水平排列，这不是典型情况，所以我们在这里指定 animationFrameWidth ，稍后我们会在 AnimateIndividualTile 中使用它。
        private readonly int animationFrameWidth = 18;

        // This method allows you to determine how much light this block emits
        // 此方法允许您确定此方块发出多少光线。
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.93f;
            g = 0.11f;
            b = 0.12f;
        }

        // This method allows you to determine whether or not the tile will draw itself flipped in the world
        // 此方法允许您确定图块是否会在世界中自行翻转。
        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            // Flips the sprite if x coord is odd. Makes the tile more interesting
            // 如果x坐标是奇数，则翻转精灵图。使图块更有趣
            if (i % 2 == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            // Tweak the frame drawn by x position so tiles next to each other are off-sync and look much more interesting
            // 调整由x位置绘制的帧，以便相邻的图块不同步并看起来更加有趣
            int uniqueAnimationFrame = Main.tileFrame[Type] + i;
            if (i % 2 == 0)
                uniqueAnimationFrame += 3;
            if (i % 3 == 0)
                uniqueAnimationFrame += 3;
            if (i % 4 == 0)
                uniqueAnimationFrame += 3;
            uniqueAnimationFrame %= 6;

            // frameYOffset = modTile.animationFrameHeight * Main.tileFrame [type] will already be set before this hook is called
            // But we have a horizontal animated texture, so we use frameXOffset instead of frameYOffset

            // frameYOffset = modTile.animationFrameHeight * Main.tileFrame [type] 将在调用此钩子之前已经设置
            // 但我们有一个水平动画纹理，所以我们使用frameXOffset而不是frameYOffset
            frameXOffset = uniqueAnimationFrame * animationFrameWidth;
        }

        // This method allows you to change the sound a tile makes when hit
        // 这个方法允许你改变瓷砖被敲击时发出的声音
        public override bool KillSound(int i, int j, bool fail)
        {
            // Play the glass shattering sound instead of the normal digging sound if the tile is destroyed on this hit
            // 如果在这次敲击中瓷砖被摧毁，播放玻璃碎裂声而不是正常的挖掘声
            if (!fail)
            {
                SoundEngine.PlaySound(SoundID.Shatter, new Vector2(i, j).ToWorldCoordinates());
                return false;
            }
            return base.KillSound(i, j, fail);
        }

        //TODO: It's better to have an actual class for this Analysis, instead of comments
        //TODO: 最好为此分析创建一个实际的类，而不是注释

        // Below is an Analysis completely manually drawing a tile. It shows some interesting concepts that may be useful for more advanced things
        // 下面是完全手动绘制图块的分析。它展示了一些有趣的概念，可能对更高级的事情有用。
        /*public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
			// Instead of SetSpriteEffects
			// Flips the sprite if x coord is odd. Makes the tile more interesting

            // 不使用SetSpriteEffects
            // 如果x坐标为奇数，则反转精灵图。使图块更加有趣。

			SpriteEffects effects = SpriteEffects.None;
			if (i % 2 == 1)
				effects = SpriteEffects.FlipHorizontally;

			// Instead of AnimateIndividualTile
			// Tweak the frame drawn by x position so tiles next to each other are off-sync and look much more interesting

            // 不使用AnimateIndividualTile
            // 通过x位置调整绘制帧以使相邻图块失去同步并显得更加有趣。

			int uniqueAnimationFrame = Main.tileFrame[Type] + i % 6;
			if (i % 2 == 0)
				uniqueAnimationFrame += 3;
			if (i % 3 == 0)
				uniqueAnimationFrame += 3;
			if (i % 4 == 0)
				uniqueAnimationFrame += 3;
			uniqueAnimationFrame %= 6;

			int frameXOffset = uniqueAnimationFrame * animationFrameWidth;


			Tile tile = Main.tile[i, j];
			Texture2D texture = ModContent.Request<Texture2D>("AnalysisMod/AnalysisContent/Tiles/AnalysisAnimatedTileTile").Value;

			// If you are using ModTile.SpecialDraw or PostDraw or PreDraw, use this snippet and add zero to all calls to spriteBatch.Draw
			// The reason for this is to accommodate the shift in drawing coordinates that occurs when using the different Lighting mode
			// Press Shift+F9 to change lighting modes quickly to verify your code works for all lighting modes

            // 如果您正在使用ModTile.SpecialDraw或PostDraw或PreDraw，请使用此代码片段，并将所有对spriteBatch.Draw 的调用添加零。
            // 原因是要适应在使用不同 Lighting 模式时发生的绘制坐标偏移。
            // 按Shift+F9 快速切换 Lighting 模式以验证您的代码是否适用于所有 Lighting 模式。

			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

			Main.spriteBatch.Draw(
				texture,
				new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero,
				new Rectangle(tile.frameX + frameXOffset, tile.frameY, 16, 16),
				Lighting.GetColor(i, j), 0f, default, 1f, effects, 0f);

			return false; // return false to stop vanilla draw
                          // 返回false停止原始绘制
		}*/

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            /*
			// Spend 9 ticks on each of 6 frames, looping
            // 在6个帧中每个花费9个滴答声，循环

			frameCounter++;
			if (frameCounter >= 9) {
				frameCounter = 0;
				if (++frame >= 6) {
					frame = 0;
				}
			}

			// Or, more compactly:
            // 或者更简洁地说：
			if (++frameCounter >= 9) {
				frameCounter = 0;
				frame = ++frame % 6;
			}*/

            // Above code works, but since we are just mimicking another tile, we can just use the same value
            // 上述代码有效，但由于我们只是模仿另一个图块，因此可以使用相同的值
            frame = Main.tileFrame[TileID.FireflyinaBottle];
        }
    }

    internal class AnalysisAnimatedTileItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.FireflyinaBottle);
            Item.createTile = ModContent.TileType<AnalysisAnimatedTile>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AnalysisItem>()
                .AddTile<AnalysisWorkbench>()
                .Register();
        }
    }
}
