using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace AnalysisMod.AnalysisContent.Dusts
{
    // This dust shows off custom drawing. By default, the dust sprite is drawn once. This Analysis uses custom drawing to draw a trail,
    // it is an exact clone of DustID.Electric, aside from some code cleanup. One place Terraria uses DustID.Electric is when a player is suffering from BuffID.Electrified.

    // 这个尘埃展示了自定义绘制。默认情况下，尘埃精灵只会被绘制一次。这个分析使用自定义绘制来画出一个轨迹，
    // 它是 DustID.Electric 的完全克隆，除了一些代码清理。Terraria 在玩家受到 BuffID.Electrified 影响时使用 DustID.Electric。
    public class AnalysisCustomDrawDust : ModDust
    {
        public override string Texture => null;

        public override void OnSpawn(Dust dust)
        {
            int desiredVanillaDustTexture = DustID.Electric;
            int frameX = desiredVanillaDustTexture * 10 % 1000;
            int frameY = desiredVanillaDustTexture * 10 / 1000 * 30 + Main.rand.Next(3) * 10;
            dust.frame = new Rectangle(frameX, frameY, 8, 8);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            lightColor = Color.Lerp(lightColor, Color.White, 0.8f);
            return new Color(lightColor.R, lightColor.G, lightColor.B, 25);
        }

        public override bool PreDraw(Dust dust)
        {
            // Here we draw a trail by drawing the dust many times at different scales and offsets. 
            // 在这里，我们通过在不同的比例和偏移下多次绘制灰尘来绘制路径。
            if (dust.fadeIn == 0f)
            {
                float trailLength = Math.Abs(dust.velocity.X) + Math.Abs(dust.velocity.Y);
                trailLength *= 3f;
                if (trailLength > 10f)
                    trailLength = 10f;

                Color drawColor = Lighting.GetColor((int)(dust.position.X + 4) / 16, (int)(dust.position.Y + 4) / 16);
                drawColor = dust.GetAlpha(drawColor);
                for (int i = 0; i < trailLength; i++)
                {
                    Vector2 trailPosition = dust.position - dust.velocity * i;
                    float trailScale = dust.scale * (1f - i / 10f);
                    Main.spriteBatch.Draw(Texture2D.Value, trailPosition - Main.screenPosition, dust.frame, drawColor, dust.rotation, new Vector2(4f, 4f), trailScale, SpriteEffects.None, 0f);
                }
            }

            // By returning true, the default dust drawing will occur, drawing the final full scale dust.
            // 通过返回 true，将会发生默认的灰尘绘制，从而绘制最终的全比例灰尘。
            return true;
        }
    }
}
