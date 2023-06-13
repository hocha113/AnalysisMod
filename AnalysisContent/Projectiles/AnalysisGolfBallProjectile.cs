using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Projectiles
{
    public class AnalysisGolfBallProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.IsAGolfBall[Type] = true; // Allows the projectile to be placed on the tee.
                                                        // 允许将弹丸放置在球座上。

            ProjectileID.Sets.TrailingMode[Type] = 0; // Creates a trail behind the golf ball.
                                                      // 在高尔夫球后面留下一条轨迹。

            ProjectileID.Sets.TrailCacheLength[Type] = 20; // Sets the length of the trail.
                                                           // 设置轨迹的长度。
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true; // Indicates that this projectile will be synced to a joining player (by default, any projectiles active before the player joins (besides pets) are not synced over).
                                            // 指示此弹丸将与加入玩家同步（默认情况下，任何在玩家加入之前激活的弹丸（除宠物外）都不会被同步）。

            Projectile.width = 7; // The width of the projectile's hitbox.
                                  // 弹丸打击箱的宽度。

            Projectile.height = 7; // The height of the projectile's hitbox.
                                   // 弹丸打击箱的高度。

            Projectile.friendly = true; // Setting this to anything other than true causes an index out of bounds error.
                                        // 将其设置为true以外的任何值都会导致索引越界错误。

            Projectile.penetrate = -1; // Number of times the projectile can penetrate enemies. -1 sets it to infinite penetration.
                                       // 弹丸可以穿透敌人的次数。-1表示无限穿透。

            Projectile.aiStyle = 149; // 149 is the golf ball AI.
                                      // 149是高尔夫球AI编号。

            Projectile.tileCollide = false; // Tile Collision is set to false, as it's handled in the AI.
                                            // Tile Collision被设置为false，因为它由AI处理。
        }
    }
}