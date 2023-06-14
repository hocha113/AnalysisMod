using AnalysisMod.AnalysisContent.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Projectiles
{
    // This file shows an animated projectile
    // This file also shows advanced drawing to center the drawn projectile correctly

    // 这个文件展示了一个动画弹射物
    // 这个文件还展示了高级绘图，以正确地将弹射物居中绘制
    public class AnalysisAdvancedAnimatedProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // Total count animation frames
            // 动画帧总数
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40; // The width of projectile hitbox
                                   // 弹射物碰撞盒的宽度

            Projectile.height = 40; // The height of projectile hitbox
                                    // 弹射物碰撞盒的高度

            Projectile.friendly = true; // Can the projectile deal damage to enemies?
                                        // 该弹射物能否对敌人造成伤害？

            Projectile.DamageType = DamageClass.Melee; // Is the projectile shoot by a ranged weapon?
                                                       // 该弹射物是否由远程武器发出？

            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
                                           // 该弹射物速度是否受水影响？

            Projectile.tileCollide = false; // Can the projectile collide with tiles?
                                            // 该弹射物能否与方块碰撞？

            Projectile.penetrate = -1; // Look at comments AnalysisPiercingProjectile
                                       // 查看注释AnalysisPiercingProjectile

            Projectile.alpha = 255; // How transparent to draw this projectile. 0 to 255. 255 is completely transparent.
                                    // 如何透明地绘制此投掷体。0到255。 255是完全透明。
        }

        // Allows you to determine the color and transparency in which a projectile is drawn
        // Return null to use the default color (normally light and buff color)
        // Returns null by default.

        // 允许您确定以哪种颜色和透明度绘制投掷体
        // 返回null以使用默认颜色（通常为浅色和缓冲颜色）
        // 默认情况下返回null。

        public override Color? GetAlpha(Color lightColor)
        {
            // return Color.White;
            return new Color(255, 255, 255, 0) * Projectile.Opacity;
        }

        public override void AI()
        {
            // All projectiles have timers that help to delay certain events
            // Projectile.ai[0], Projectile.ai[1] — timers that are automatically synchronized on the client and server
            // Projectile.localAI[0], Projectile.localAI[0] — only on the client
            // In this Analysis, a timer is used to control the fade in / out and despawn of the projectile

            // 所有投掷体都有计时器，用于延迟某些事件
            // Projectile.ai [0]，Projectile.ai [1] - 自动在客户端和服务器上同步的计时器
            // Projectile.localAI [0]，Projectile.localAI [1] - 仅在客户端上
            // 在此分析中，使用定时器来控制淡入/淡出和消失时间
            Projectile.ai[0] += 1f;

            FadeInAndOut();

            // Slow down
            // 减慢速度
            Projectile.velocity *= 0.98f;

            // Loop through the 4 animation frames, spending 5 ticks on each
            // Projectile.frame — index of current frame

            // 循环遍历4个动画帧，每个花费5个刻度
            // Projectile.frame - 当前帧的索引
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                // Or more compactly Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];

                // 或者更紧凑地Projectile.frame = ++ Projectile.frame％Main.projFrames [Projectile.type];
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }

            // Despawn this projectile after 1 second (60 ticks)
            // You can use Projectile.timeLeft = 60f in SetDefaults() for same goal

            // 在1秒后（60 ticks）消失此投掷体
            // 您可以在SetDefaults（）中使用Projectile.timeLeft = 60f来实现相同的目标
            if (Projectile.ai[0] >= 60f)
                Projectile.Kill();

            // Set both direction and spriteDirection to 1 or -1 (right and left respectively)
            // Projectile.direction is automatically set correctly in Projectile.Update, but we need to set it here or the textures will draw incorrectly on the 1st frame.

            // 将方向和spriteDirection都设置为1或-1（分别为右和左）
            // Projectile.direction会在Projectile.Update中自动正确设置，但我们需要在这里设置它，否则纹理将在第一帧上不正确绘制。
            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;

            Projectile.rotation = Projectile.velocity.ToRotation();
            // Since our sprite has an orientation, we need to adjust rotation to compensate for the draw flipping
            // 由于我们的精灵图具有方向性，因此我们需要调整旋转以补偿绘图翻转
            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation += MathHelper.Pi;
                // For vertical sprites use MathHelper.PiOver2
                // 对于垂直精灵图，请使用MathHelper.PiOver2
            }
        }

        // Many projectiles fade in so that when they spawn they don't overlap the gun muzzle they appear from
        // 许多投掷物淡入以便当它们生成时不重叠枪口从中出现的位置。
        public void FadeInAndOut()
        {
            // If last less than 50 ticks — fade in, than more — fade out
            // 如果持续时间少于50 tick，则淡入，然后更多——淡出
            if (Projectile.ai[0] <= 50f)
            {
                // Fade in
                // 淡入
                Projectile.alpha -= 25;
                // Cap alpha before timer reaches 50 ticks
                // 在计时器达到50 tick之前限制alpha值
                if (Projectile.alpha < 100)
                    Projectile.alpha = 100;

                return;
            }

            // Fade out
            // 淡出
            Projectile.alpha += 25;
            // Cal alpha to the maximum 255(complete transparent)
            // Cal alpha至最大255（完全透明）
            if (Projectile.alpha > 255)
                Projectile.alpha = 255;
        }

        // Some advanced drawing because the texture image isn't centered or symetrical
        // If you dont want to manually drawing you can use vanilla projectile rendering offsets
        // Here you can check it https://github.com/tModLoader/tModLoader/wiki/Basic-Projectile#horizontal-sprite-Analysis

        // 由于纹理图像未居中或对称而进行了一些高级绘图
        // 如果您不想手动绘制，则可以使用香草弹射物渲染偏移量
        // 在这里，您可以检查它 https://github.com/tModLoader/tModLoader/wiki/Basic-Projectile#horizontal-sprite-Analysis
        public override bool PreDraw(ref Color lightColor)
        {
            // SpriteEffects helps to flip texture horizontally and vertically
            // SpriteEffects可以水平和垂直翻转纹理
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            // Getting texture of projectile
            // 获取弹射物的纹理
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            // Calculating frameHeight and current Y pos dependence of frame
            // If texture without animation frameHeight is always texture.Height and startY is always 0

            // 计算帧高度和当前Y位置与帧的依赖关系
            // 如果没有动画的纹理，frameHeight始终为texture.Height，startY始终为0
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            // 在纹理上获取此帧
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);

            // Alternatively, you can skip defining frameHeight and startY and use this:
            // 或者，您可以跳过定义frameHeight和startY并使用以下内容：

            // Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);

            Vector2 origin = sourceRectangle.Size() / 2f;

            // If image isn't centered or symmetrical you can specify origin of the sprite
            // (0,0) for the upper-left corner

            // 如果图像不居中或对称，则可以指定精灵图的原点
            // 左上角为(0,0)
            float offsetX = 20f;
            origin.X = Projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX;

            // If sprite is vertical
            // float offsetY = 20f;

            // 如果精灵图是垂直的
            // float offsetY = 20f;

            // origin.Y = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Height - offsetY : offsetY);


            // Applying lighting and draw current frame
            // 应用光照并绘制当前帧
            Color drawColor = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            // It's important to return false, otherwise we also draw the original texture.
            // 很重要返回false，否则我们也会绘制原始纹理。
            return false;
        }
    }

    // This is a simple item that is based on the NebulaBlaze and shoots AnalysisAdvancedAnimatedProjectile to showcase it.
    // 这是一个简单的物品，基于NebulaBlaze，并发射AnalysisAdvancedAnimatedProjectile以展示它。
    internal class AnalysisAdvancedAnimatedProjectileItem : ModItem
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.NebulaBlaze}";

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.NebulaBlaze);
            Item.mana = 3;
            Item.damage = 3;
            Item.shoot = ModContent.ProjectileType<AnalysisAdvancedAnimatedProjectile>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AnalysisItem>()
                .AddTile<Tiles.Furniture.AnalysisWorkbench>()
                .Register();
        }
    }
}
