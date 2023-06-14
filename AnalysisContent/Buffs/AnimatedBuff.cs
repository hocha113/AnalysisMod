using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Buffs
{
    // This buff has an extra animation spritesheet, and also showcases PreDraw specifically.
    // (We keep the autoloaded texture as one frame in case other mods need to access the buff sprite directly and aren't aware of it having special draw code).

    // 这个增益效果有一个额外的动画精灵图表，并且特别展示了PreDraw。
    //（我们将自动加载的纹理保留为一帧，以防其他模组需要直接访问增益效果精灵图并不知道它具有特殊绘制代码）。
    public class AnimatedBuff : ModBuff
    {
        // Some constants we define to make our life easier.
        // 为使我们的生活更轻松而定义的一些常量。
        public static readonly int FrameCount = 4; // Amount of frames we have on our animation spritesheet.
                                                   // 我们在动画精灵图表上拥有的帧数。

        public static readonly int AnimationSpeed = 60; // In ticks.
                                                        // 单位：游戏刻。

        public static readonly string AnimationSheetPath = "AnalysisMod/AnalysisContent/Buffs/AnimatedBuff_Animation";

        public static readonly int DamageBonus = 10;

        private Asset<Texture2D> animatedTexture;

        public override LocalizedText Description => base.Description.WithFormatArgs(DamageBonus);

        public override void SetStaticDefaults()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                // Do NOT load textures on the server!
                // 不要在服务器上加载纹理！
                animatedTexture = ModContent.Request<Texture2D>(AnimationSheetPath);
            }
        }

        public override void Unload()
        {
            animatedTexture = null;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, int buffIndex, ref BuffDrawParams drawParams)
        {
            // You can use this hook to make something special happen when the buff icon is drawn (such as reposition it, pick a different texture, etc.).
            // 你可以使用这个钩子来在图标绘制时执行一些特殊操作（例如重新定位、选择不同的纹理等）。

            // We draw our special texture here with a specific animation.
            // 在此处使用我们的特殊纹理并进行动画处理。

            // Use our animation spritesheet.
            // 使用我们的动画精灵图表。
            Texture2D ourTexture = animatedTexture.Value;
            // Choose the frame to display, here based on constants and the game's tick count.
            // 根据常量和游戏计数器选择要显示的帧。
            Rectangle ourSourceRectangle = ourTexture.Frame(verticalFrames: FrameCount, frameY: (int)Main.GameUpdateCount / AnimationSpeed % FrameCount);

            // Other stuff you can do in this hook
            // 此钩子中可以做其他事情
            /*
			// Here we make the icon have a lime green tint.
            // 在这里，我们使图标呈现酸橙绿色调。

			drawParams.drawColor = Color.LimeGreen * Main.buffAlpha[buffIndex];
			*/

            // Be aware of the fact that drawParams.mouseRectangle exists: it defaults to the size of the autoloaded buffs' sprite,
            // it handles mouseovering and clicking on the buff icon. Since our frame in the animation is 32x32 (same as the autoloaded sprite),
            // and we don't change drawParams.position, we don't have to do anything. If you offset the position, or have a non-standard size, change it accordingly.

            // 注意drawParams.mouseRectangle存在：它默认为自动加载缓冲区精灵图的大小，
            // 它处理鼠标悬停和单击缓冲区图标。由于我们在动画中的帧是32x32（与自动加载精灵图相同），
            // 并且我们没有更改drawParams.position，因此无需进行任何操作。如果您偏移位置或具有非标准大小，请相应更改它。

            // We have two options here:
            // Option 1 is the recommended one, as it requires less code.
            // Option 2 allows you to customize drawing even more, but then you are on your own.

            // 我们有两个选项：
            // 选项1是推荐选项，因为它需要较少的代码。
            // 选项2允许您进一步自定义绘制，但那样就只能靠自己了。

            // For demonstration, both options' codes are written down, but the latter is commented out using /* and */.
            // 演示起见，两种选项都写下了代码，但后者被注释掉了/* 和 */.

            // OPTION 1 - Let the game draw it for us. Therefore we have to assign our variables to drawParams:
            // OPTION 1 - 让游戏替我们绘制。 因此，必须将变量分配给drawParams：
            drawParams.Texture = ourTexture;
            drawParams.SourceRectangle = ourSourceRectangle;
            // Return true to let the game draw the buff icon.
            return true;

            /*
			// OPTION 2 - Draw our buff manually:
            // OPTION 2 - 手动绘制我们的缓冲区：

			spriteBatch.Draw(ourTexture, drawParams.position, ourSourceRectangle, drawParams.drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

			// Return false to prevent drawing the icon, since we have already drawn it.
            // 返回false以防止绘制图标，因为我们已经将其绘制出来了。

			return false;
			*/
        }

        public override void Update(Player player, ref int buffIndex)
        {
            // Increase all damage by 10%
            // 将所有伤害增加10％
            player.GetDamage<GenericDamageClass>() += DamageBonus / 100f;
        }
    }
}
