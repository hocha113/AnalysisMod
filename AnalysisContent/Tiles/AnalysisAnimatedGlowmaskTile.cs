using AnalysisMod.AnalysisContent.Tiles.Furniture;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using AnalysisMod.AnalysisContent.Items;

namespace AnalysisMod.AnalysisContent.Tiles
{
    public class AnalysisAnimatedGlowmaskTile : ModTile
    {
        // If you want to know more about tiles, please follow this link
        // 如果您想了解更多关于瓷砖的信息，请点击此链接：

        // https://github.com/tModLoader/tModLoader/wiki/Basic-Tile
        public override void SetStaticDefaults()
        {
            // This changes a Framed tile to a FrameImportant tile
            // For modders, just remember to set this to true when you make a tile that uses a TileObjectData
            // Or basically all tiles that aren't like dirt, ores, or other basic building tiles

            // 这将把一个带框架的瓷砖变成一个重要框架的瓷砖
            // 对于mod制作者，当你制作使用TileObjectData的地图块时，请记得将其设置为true
            // 或者基本上所有不像泥土、矿物或其他基本建筑块一样的地图块
            Main.tileFrameImportant[Type] = true;
            // Use this to utilize an existing template
            // The names of styles are self explanatory usually (you can see all existing templates at the link mentioned earlier)

            // 使用这个来利用现有模板
            // 样式名称通常是自我说明性质（您可以在前面提到过的链接中查看所有现有模板）
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            // Before adding the new tile you can make some changes to newTile like height, origin and etc.
            // Changing the Height because the template is for 1x2 not 1x3

            // 在添加新地图块之前，您可以对newTile进行一些更改，如高度、原点等。
            // 更改高度是因为该模板适用于1x2而不是1x3
            TileObjectData.newTile.Height = 3;

            // Modifies which part of the tile is centered on the mouse, in tile coordinates, from the top right corner
            // 修改以鼠标为中心部分所占比例，在平铺坐标系下从右上角开始计算。
            TileObjectData.newTile.Origin = new Point16(1, 2);

            // Setting the height of the tiles individually for each
            // 为每个单元格单独设置高度
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 18 };

            // Finally adding newTile
            // 最后添加newTile
            TileObjectData.addTile(Type);

            // AddMapEntry is for setting the color and optional text associated with the Tile when viewed on the map
            // AddMapEntry用于在查看地图时设置与方格相关联的颜色和可选文本。
            AddMapEntry(new Color(75, 139, 166));

            // The height of a group of animation frames for this tile
            // Defaults to 0, which disables animations

            // 此方格动画帧组合的高度，
            // 默认值为0，表示禁用动画效果。
            AnimationFrameHeight = 56;
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            // We can change frames manually, but since we are just simulating a different tile, we can just use the same value
            // 我们可以手动更改帧数，但由于我们只是模拟不同类型方格，因此可以使用相同值。
            frame = Main.tileFrame[TileID.LunarMonolith];
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            Texture2D texture = ModContent.Request<Texture2D>("AnalysisMod/AnalysisContent/Tiles/AnalysisAnimatedGlowmaskTile").Value;
            Texture2D glowTexture = ModContent.Request<Texture2D>("AnalysisMod/AnalysisContent/Tiles/AnalysisAnimatedGlowmaskTile_Glow").Value;

            // If you are using ModTile.SpecialDraw or PostDraw or PreDraw, use this snippet and add zero to all calls to spriteBatch.Draw
            // The reason for this is to accommodate the shift in drawing coordinates that occurs when using the different Lighting mode
            // Press Shift+F9 to change lighting modes quickly to verify your code works for all lighting modes

            // 如果您使用ModTile.SpecialDraw或PostDraw或PreDraw，请使用此代码片段，并将所有对spriteBatch.Draw的调用添加为零。
            // 这样做的原因是适应在使用不同照明模式时发生的绘图坐标移位
            // 按Shift + F9快速更改照明模式，以验证您的代码是否适用于所有照明模式
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

            // Because height of third tile is different we change it
            // 由于第三个方格高度不同，我们需要进行更改。
            int height = tile.TileFrameY % AnimationFrameHeight == 36 ? 18 : 16;

            // Offset along the Y axis depending on the current frame
            // 根据当前帧沿Y轴偏移量
            int frameYOffset = Main.tileFrame[Type] * AnimationFrameHeight;

            // Firstly we draw the original texture and then glow mask texture
            // 首先绘制原始纹理，然后再绘制发光蒙版纹理。
            spriteBatch.Draw(
                texture,
                new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero,
                new Rectangle(tile.TileFrameX, tile.TileFrameY + frameYOffset, 16, height),
                Lighting.GetColor(i, j), 0f, default, 1f, SpriteEffects.None, 0f);
            // Make sure to draw with Color.White or at least a color that is fully opaque
            // Achieve opaqueness by increasing the alpha channel closer to 255. (lowering closer to 0 will achieve transparency)

            // 确保使用Color.White或至少完全不透明的颜色进行绘制
            // 通过增加接近255的alpha通道来实现不透明度。（降低到接近0会导致透明）
            spriteBatch.Draw(
                glowTexture,
                new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero,
                new Rectangle(tile.TileFrameX, tile.TileFrameY + frameYOffset, 16, height),
                Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            // Return false to stop vanilla draw
            // 返回false以停止vanilla draw
            return false;
        }
    }

    internal class AnalysisAnimatedGlowmaskTileItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.VoidMonolith);
            Item.createTile = ModContent.TileType<AnalysisAnimatedGlowmaskTile>();
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
