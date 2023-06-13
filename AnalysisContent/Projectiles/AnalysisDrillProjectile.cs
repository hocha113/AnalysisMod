using AnalysisMod.AnalysisContent.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Projectiles
{
    public class AnalysisDrillProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // Prevents jitter when steping up and down blocks and half blocks
            // 防止在上下方块和半方块时抖动
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ownerHitCheck = true;
            Projectile.aiStyle = -1; // Replace with 20 if you do not want custom code
                                     // 如果您不想使用自定义代码，请替换为20

            Projectile.hide = true; // Hides the projectile, so it will draw in the player's hand when we set the player's heldProj to this one.
                                    // 隐藏弹射物，这样当我们将玩家的heldProj设置为此项目时，它将绘制在玩家手中。
        }

        // This code is adapted and simplified from aiStyle 20 to use a different dust and more noises. If you want to use aiStyle 20, you do not need to do any of this.
        // It should be noted that this projectile has no effect on mining and is mostly visual.

        // 此代码是从aiStyle 20简化并改编而来，以使用不同的粉尘和更多噪音。 如果要使用aiStyle 20，则无需执行任何操作。
        // 应注意，此弹射物对采矿没有影响，并且大多数是视觉效果。
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.timeLeft = 60;

            // Animation code could go here if the projectile was animated. 
            // 如果弹射物有动画，则可以在此处放置动画代码。

            // Plays a sound every 20 ticks. In aiStyle 20, soundDelay is set to 30 ticks.
            // 每20个ticks播放一次声音。 在aiStyle 20中，soundDelay设置为30 ticks。
            if (Projectile.soundDelay <= 0)
            {
                SoundEngine.PlaySound(SoundID.Item22, Projectile.Center);
                Projectile.soundDelay = 20;
            }

            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter);
            if (Main.myPlayer == Projectile.owner)
            {
                // This code must only be ran on the client of the projectile owner
                // 此代码必须仅在弹射物所有者的客户端上运行
                if (player.channel)
                {
                    float holdoutDistance = player.HeldItem.shootSpeed * Projectile.scale;
                    // Calculate a normalized vector from player to mouse and multiply by holdoutDistance to determine resulting holdoutOffset
                    // 计算从玩家到鼠标的归一化向量，并乘以holdoutDistance以确定结果holdoutOffset
                    Vector2 holdoutOffset = holdoutDistance * Vector2.Normalize(Main.MouseWorld - playerCenter);
                    if (holdoutOffset.X != Projectile.velocity.X || holdoutOffset.Y != Projectile.velocity.Y)
                    {
                        // This will sync the projectile, most importantly, the velocity.
                        // 这将同步弹射物，最重要的是速度。
                        Projectile.netUpdate = true;
                    }

                    // Projectile.velocity acts as a holdoutOffset for held projectiles.
                    // Projectile.velocity充当持有投影的偏移量。
                    Projectile.velocity = holdoutOffset;
                }
                else
                {
                    Projectile.Kill();
                }
            }

            if (Projectile.velocity.X > 0f)
            {
                player.ChangeDir(1);
            }
            else if (Projectile.velocity.X < 0f)
            {
                player.ChangeDir(-1);
            }

            Projectile.spriteDirection = Projectile.direction;
            player.ChangeDir(Projectile.direction); // Change the player's direction based on the projectile's own
                                                    // 根据投影本身更改玩家的方向

            player.heldProj = Projectile.whoAmI; // We tell the player that the drill is the held projectile, so it will draw in their hand
                                                 // 我们告诉玩家钻头是被握住的投影，在他们手中绘制出来

            player.SetDummyItemTime(2); // Make sure the player's item time does not change while the projectile is out
                                        // 确保在发射投影时不会更改玩家道具时间

            Projectile.Center = playerCenter; // Centers the projectile on the player. Projectile.velocity will be added to this in later Terraria code causing the projectile to be held away from the player at a set distance.
                                              // 使投影居中于玩家。 Projectile.velocity将在后续的Terraria代码中添加到此处，从而使投影以一定距离远离玩家。

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();

            // Gives the drill a slight jiggle
            // 给钻头一个轻微的摇晃
            Projectile.velocity.X *= 1f + Main.rand.Next(-3, 4) * 0.01f;

            // Spawning dust
            // 生成粉尘
            if (Main.rand.NextBool(10))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position + Projectile.velocity * Main.rand.Next(6, 10) * 0.15f, Projectile.width, Projectile.height, ModContent.DustType<Sparkle>(), 0f, 0f, 80, Color.White, 1f);
                dust.position.X -= 4f;
                dust.noGravity = true;
                dust.velocity.X *= 0.5f;
                dust.velocity.Y = -Main.rand.Next(3, 8) * 0.1f;
            }
        }
    }
}
