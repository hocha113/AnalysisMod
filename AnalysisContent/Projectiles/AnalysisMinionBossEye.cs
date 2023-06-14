using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Projectiles
{
    public class AnalysisMinionBossEye : ModProjectile
    {
        public bool FadedIn
        {
            get => Projectile.localAI[0] == 1f;
            set => Projectile.localAI[0] = value ? 1f : 0f;
        }

        public bool PlayedSpawnSound
        {
            get => Projectile.localAI[1] == 1f;
            set => Projectile.localAI[1] = value ? 1f : 0f;
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.alpha = 255;
            Projectile.timeLeft = 90;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            Projectile.aiStyle = -1;
            CooldownSlot = ImmunityCooldownID.Bosses; // use the boss immunity cooldown counter, to prevent ignoring boss attacks by taking damage from other sources
                                                      // 使用BOSS免疫冷却计数器，防止因其他来源的伤害而忽略BOSS攻击
        }

        public override Color? GetAlpha(Color lightColor)
        {
            // When overriding GetAlpha, you usually want to take the projectiles alpha into account. As it is a value between 0 and 255,
            // it's annoying to convert it into a float to multiply. Luckily the Opacity property handles that for us (0f transparent, 1f opaque)

            //当覆盖GetAlpha时，通常需要考虑弹道透明度。由于其值介于0和255之间，
            //将其转换为浮点数进行乘法运算很麻烦。幸运的是Opacity属性已经处理了这个问题（0f表示透明，1f表示不透明）
            return Color.White * Projectile.Opacity;
        }

        private void FadeInAndOut()
        {
            // Fade in (we have Projectile.alpha = 255 in SetDefaults which means it spawns transparent)
            // 淡入（我们在SetDefaults中设置Projectile.alpha = 255，这意味着它生成时是透明的）
            int fadeSpeed = 10;
            if (!FadedIn && Projectile.alpha > 0)
            {
                Projectile.alpha -= fadeSpeed;
                if (Projectile.alpha < 0)
                {
                    FadedIn = true;
                    Projectile.alpha = 0;
                }
            }
            else if (FadedIn && Projectile.timeLeft < 255f / fadeSpeed)
            {
                // Fade out so it aligns with the projectile despawning
                // 淡出以便与弹丸消失对齐
                Projectile.alpha += fadeSpeed;
                if (Projectile.alpha > 255)
                {
                    Projectile.alpha = 255;
                }
            }
        }

        public override void AI()
        {
            FadeInAndOut();

            if (!PlayedSpawnSound)
            {
                PlayedSpawnSound = true;

                // Common practice regarding spawn sounds for projectiles is to put them into AI, playing sounds in the same place where they are spawned
                // is not multiplayer compatible (either no one will hear it, or only you and not others)

                //关于发射声音的常见做法是将它们放入AI中，在生成它们的地方播放声音
                //不支持多人游戏（要么没有人会听到它，要么只有你自己听到而别人不能听到）
                SoundEngine.PlaySound(SoundID.Item8, Projectile.position);
            }

            // Accelerate
            //加速
            Projectile.velocity *= 1.01f;

            // If the sprite points upwards, this will make it point towards the move direction (for other sprite orientations, change MathHelper.PiOver2)
            //如果精灵图朝上，则使其指向移动方向（对于其他精灵图方向，请更改MathHelper.PiOver2）
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
    }
}
