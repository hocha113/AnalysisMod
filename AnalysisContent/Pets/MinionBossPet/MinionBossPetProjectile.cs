using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Pets.MinionBossPet
{
    // You can find a simple pet Analysis in AnalysisMod\Content\Pets\AnalysisPet
    // This pet uses custom AI and drawing to make it more special (It's a Master Mode boss pet after all)
    // It behaves similarly to the Creeper Egg or Suspicious Grinning Eye pets, but takes some visual properties from AnalysisMod's Minion Boss

    // 在AnalysisMod\Content\Pets\AnalysisPet中可以找到一个简单的宠物分析
    // 这个宠物使用自定义AI和绘图使其更加特别（毕竟是大师模式的Boss宠物）
    // 它的行为类似于苦力怕蛋或可疑的咧嘴眼睛宠物，但从AnalysisMod的随从Boss中获取了一些视觉属性
    public class MinionBossPetProjectile : ModProjectile
    {
        // This is a ref property, lets us write Projectile.ai[0] as whatever name we want
        // 这是一个ref属性，让我们可以将Projectile.ai[0]写成任何想要的名称
        public ref float AlphaForVisuals => ref Projectile.ai[0];

        // This projectile uses an additional texture for drawing
        // 此弹幕使用额外纹理进行绘制
        public static Asset<Texture2D> EyeAsset;

        public override void Load()
        {
            // load/cache the additional texture
            // 加载/缓存额外纹理
            if (!Main.dedServ)
            {
                EyeAsset = ModContent.Request<Texture2D>(Texture + "_Eye");
            }
        }

        public override void Unload()
        {
            // Unload the additional texture
            // 卸载额外纹理
            EyeAsset = null;
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
            Main.projPet[Projectile.type] = true;

            // Basics of CharacterPreviewAnimations explained in AnalysisPetProjectile
            // Notice we define our own method to use in .WithCode() below. This technically allows us to animate the projectile manually using frameCounter and frame aswell

            // CharacterPreviewAnimations基础知识在AnalysisPetProjectile中解释
            // 请注意，我们在下面定义了自己的方法以在.WithCode()中使用。这实际上允许我们手动使用frameCounter和frame来对弹幕进行动画处理。
            ProjectileID.Sets.CharacterPreviewAnimations[Projectile.type] = ProjectileID.Sets.SimpleLoop(0, Main.projFrames[Projectile.type], 5)
                .WithOffset(-2, -22f)
                .WithCode(CharacterPreviewCustomization);
        }

        public static void CharacterPreviewCustomization(Projectile proj, bool walking)
        {
            // Modified floating from DelegateMethods.CharacterPreview.Float, this is technically not representative of how the pet actually looks and moves ingame, but the Suspicious Grinning Eye has that too
            // 修改DelegateMethods.CharacterPreview.Float浮动效果，这并不代表宠物实际上看起来和移动方式如何，在游戏内Suspicious Grinning Eye也有此效果。

            // If you don't need to modify it, just call DelegateMethods.CharacterPreview.Float(proj, walking) directly here instead and change properties of your pet after it.
            // You do not need this otherwise and can use the preset directly as showcased in AnalysisPetProjectile

            // 如果您不需要修改，请直接在此处调用DelegateMethods.CharacterPreview.Float(proj, walking)，然后更改您的宠物属性。
            // 否则你可以像AnalysisPetProjectile展示那样直接使用预设值。
            float half = 0.5f;
            float timer = (float)Main.timeForVisualEffects % 60f / 60f;
            float speed = 1f; // This is normally 2
                              // 通常情况下为2.

            proj.position.Y += 0f - half + (float)(Math.Cos(timer * MathHelper.TwoPi * speed) * half * 2f);

            // We are only using this method for one specific projectile, so it's fine to cast the ModProjectile directly like this
            // 我们仅针对一个特定的弹幕使用此方法，因此直接将ModProjectile转换为该类型是可以的。
            MinionBossPetProjectile minion = (MinionBossPetProjectile)proj.ModProjectile;

            // Need to set the alpha to 1f to hide the eyes that would normally draw and show the actual pet
            // 需要将alpha设置为1f以隐藏通常会绘制的眼睛并显示实际宠物
            minion.AlphaForVisuals = 1f;

            // You can use Projectile.isAPreviewDummy in the draw code instead, it depends if you prefer changing the conditions leading up to the drawing, or the drawing itself
            // 您可以在绘图代码中使用Projectile.isAPreviewDummy代替，这取决于您更喜欢改变导致绘图的条件还是绘图本身。
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.EyeOfCthulhuPet); // Copy the stats of the Suspicious Grinning Eye projectile
                                                                    // 复制可疑咧嘴眼睛弹幕的统计数据

            Projectile.aiStyle = -1; // Use custom AI
                                     // 使用自定义AI
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * AlphaForVisuals * Projectile.Opacity;
        }

        public override void PostDraw(Color lightColor)
        {
            // Draw surrounding eyes to mimic the boss
            // 绘制周围眼睛来模仿Boss
            Texture2D eyeTexture = EyeAsset.Value;

            Vector2 offset = new Vector2(0, Projectile.gfxOffY); // Vertical offset when the projectile is changing elevation on tiles (does not apply to this particular projectile because it is always airbone)
                                                                 // 当弹幕在平铺上改变高度时的垂直偏移量（不适用于此特定弹幕，因为它总是空气中）
            Vector2 orbitingCenter = Projectile.Center + offset;

            // Don't need to draw the eyes if the pet is fully faded in
            // 如果宠物完全淡入，则不需要绘制眼睛
            if (AlphaForVisuals >= 1)
            {
                return;
            }

            int eyeCount = 10;
            for (int i = 0; i < eyeCount; i++)
            {
                Vector2 origin = Vector2.Zero; // Using origin as zero because the draw position is the center
                                               // 使用原点作为零点，因为绘图位置是中心点。

                Vector2 rotatedPos = (Vector2.UnitY * 24).RotatedBy(i / (float)eyeCount * MathHelper.TwoPi); // Create a vector of length 24 with a specific rotation based on loop index
                                                                                                             // 创建一个长度为24、具有基于循环索引的特定旋转角度向量。

                Vector2 drawPos = orbitingCenter - Main.screenPosition + origin + rotatedPos; // Always important to substract Main.screenPosition to translate it into screen coordinates
                                                                                              // 始终重要地减去Main.screenPosition以将其转换为屏幕坐标系。

                Color color = Color.White * (1f - AlphaForVisuals) * Projectile.Opacity; // Draw it in reversed alpha to the projectile
                                                                                         // 以相反alpha值向弹丸进行渲染处理

                // Use this instead of Main.spriteBatch.Draw so that dyes apply to it
                // 使用这个而不是Main.spriteBatch.Draw使得染料也能应用到其中.
                Main.EntitySpriteDraw(eyeTexture, drawPos, eyeTexture.Bounds, color, 0f, origin, 1f, SpriteEffects.None, 0);
            }
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            // For organization, the AI is split into several methods defined below
            // They are NOT part of the ModProjectile class!

            // 为了组合性，在下面分别定义了多个AI方法
            // 它们不属于ModProjectile类！
            CheckActive(player);

            bool movesFast = Movement(player);

            Animate(movesFast);

            AlphaForVisuals = GetAlphaForVisuals(player);
        }

        private void CheckActive(Player player)
        {
            // Keep the projectile from disappearing as long as the player isn't dead and has the pet buff
            // 只要玩家没有死亡并且拥有宠物增益，就保持弹道不消失
            if (!player.dead && player.HasBuff(ModContent.BuffType<MinionBossPetBuff>()))
            {
                Projectile.timeLeft = 2;
            }
        }

        private bool Movement(Player player)
        {
            // Handles movement, returns true if moving fast (used for animation)
            // 处理移动，如果快速移动则返回true（用于动画）
            float velDistanceChange = 2f;

            // Calculates the desired resting position, aswell as some vectors used in velocity/rotation calculations
            // 计算所需的静止位置以及在速度/旋转计算中使用的一些向量
            int dir = player.direction;
            Projectile.direction = Projectile.spriteDirection = dir;

            Vector2 desiredCenterRelative = new Vector2(dir * 30, -30f);

            // Add some sine motion
            // 添加一些正弦运动
            desiredCenterRelative.Y += (float)Math.Sin(Main.GameUpdateCount / 120f * MathHelper.TwoPi) * 5;

            Vector2 desiredCenter = player.MountedCenter + desiredCenterRelative;
            Vector2 betweenDirection = desiredCenter - Projectile.Center;
            float betweenSQ = betweenDirection.LengthSquared(); // It is recommended to operate on squares of distances, to save computing time on square-rooting
                                                                // 建议对距离的平方进行操作，以节省平方根计算时间。

            if (betweenSQ > 1000f * 1000f || betweenSQ < velDistanceChange * velDistanceChange)
            {
                // Set position directly if too far away from the player, or when near the desired location
                // 如果与玩家相距太远或接近所需位置，则直接设置位置
                Projectile.Center = desiredCenter;
                Projectile.velocity = Vector2.Zero;
            }

            if (betweenDirection != Vector2.Zero)
            {
                Projectile.velocity = betweenDirection * 0.1f * 2;
            }

            bool movesFast = Projectile.velocity.LengthSquared() > 6f * 6f;

            if (movesFast)
            {
                // If moving very fast, rotate the projectile towards it smoothly
                // 如果移动非常快，则平滑地将弹道朝向其旋转
                float rotationVel = Projectile.velocity.X * 0.08f + Projectile.velocity.Y * Projectile.spriteDirection * 0.02f;
                if (Math.Abs(Projectile.rotation - rotationVel) >= MathHelper.Pi)
                {
                    if (rotationVel < Projectile.rotation)
                    {
                        Projectile.rotation -= MathHelper.TwoPi;
                    }
                    else
                    {
                        Projectile.rotation += MathHelper.TwoPi;
                    }
                }

                float rotationInertia = 12f;
                Projectile.rotation = (Projectile.rotation * (rotationInertia - 1f) + rotationVel) / rotationInertia;
            }
            else
            {
                // If moving at regular speeds, rotate the projectile towards its default rotation (0) smoothly if necessary
                // 如果以普通速度移动，则必要时将弹道平滑地朝向其默认旋转（0）
                if (Projectile.rotation > MathHelper.Pi)
                {
                    Projectile.rotation -= MathHelper.TwoPi;
                }

                if (Projectile.rotation > -0.005f && Projectile.rotation < 0.005f)
                {
                    Projectile.rotation = 0f;
                }
                else
                {
                    Projectile.rotation *= 0.96f;
                }
            }

            return movesFast;
        }

        private void Animate(bool movesFast)
        {
            int animationSpeed = 7;

            if (movesFast)
            {
                // Increase animation speed if projectile moves fast (less is faster)
                // 如果弹道移动得很快，则增加其动画速度（越小越快）
                animationSpeed = 4;
            }

            // Animate all frames from top to bottom, going back to the first
            // 从上到下依次播放所有帧，并回到第一个帧。
            Projectile.frameCounter++;
            if (Projectile.frameCounter > animationSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }

        private static float GetAlphaForVisuals(Player player)
        {
            // 0f on full life, 1f for below half life
            // 完整生命值为0f，半血以下为1f
            float lifeRatio = player.statLife / (float)player.statLifeMax2;
            return Utils.Clamp(2 * (1f - lifeRatio), 0f, 1f);
        }
    }
}
