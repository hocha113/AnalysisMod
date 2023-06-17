using AnalysisMod.AnalysisContent.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Projectiles
{
    public class AnalysisSpearProjectile : ModProjectile
    {
        // Define the range of the Spear Projectile. These are overrideable properties, in case you'll want to make a class inheriting from this one.
        //定义长矛投射物的范围。这些属性可以被覆盖，以便您想要从此类继承。
        protected virtual float HoldoutRangeMin => 24f;
        protected virtual float HoldoutRangeMax => 96f;

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Spear); // Clone the default values for a vanilla spear. Spear specific values set for width, height, aiStyle, friendly, penetrate, tileCollide, scale, hide, ownerHitCheck, and melee.
                                                          // 克隆香草长矛的默认值。为长矛设置特定的宽度、高度、aiStyle、友好性、穿透力、碰撞检测类型、比例尺寸和隐藏状态等数值。
        }

        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner]; // Since we access the owner player instance so much, it's useful to create a helper local variable for this
                                                           // 由于我们经常访问所有者玩家实例，因此创建一个帮助器本地变量非常有用

            int duration = player.itemAnimationMax; // Define the duration the projectile will exist in frames
                                                    // 定义投射物存在的时间长度（以帧为单位）

            player.heldProj = Projectile.whoAmI; // Update the player's held projectile id
                                                 // 更新玩家手持投掷武器ID

            // Reset projectile time left if necessary
            // 必要时重置弹幕剩余时间
            if (Projectile.timeLeft > duration)
            {
                Projectile.timeLeft = duration;
            }

            Projectile.velocity = Vector2.Normalize(Projectile.velocity); // Velocity isn't used in this spear implementation, but we use the field to store the spear's attack direction.
                                                                          // 在这个长矛实现中没有使用速度，但是我们使用该字段来存储长矛攻击方向。

            float halfDuration = duration * 0.5f;
          
            float progress;

            // Here 'progress' is set to a value that goes from 0.0 to 1.0 and back during the item use animation.
            // 这里“progress”被设置为一个从0.0到1.0并在项目使用动画期间返回的值。
            if (Projectile.timeLeft < halfDuration)
            {
                progress = Projectile.timeLeft / halfDuration;
            }
            else
            {
                progress = (duration - Projectile.timeLeft) / halfDuration;
            }

            // Move the projectile from the HoldoutRangeMin to the HoldoutRangeMax and back, using SmoothStep for easing the movement
            // 使用SmoothStep缓解移动将弹丸从HoldoutRangeMin移动到HoldoutRangeMax再返回

            //【可以将 value1 看作起始点的位置向量，value2 看作终点的位置向量。当 amount 为 0 时，平滑插值的结果就是起始点的位置向量 value1。当 amount 为 1 时，平滑插值的结果就是终点的位置向量 value2】
            //【而当 amount 在 0 到 1 之间时，平滑插值的结果是起始点和终点之间的一个位置向量，表示两个向量之间的平滑过渡】
            //【可以将平滑插值想象成一个在起始点和终点之间按照插值比例 amount 进行平滑移动的点，而不是一个向量箭头在两个向量之间来回摆动】
            Projectile.Center = player.MountedCenter + Vector2.SmoothStep(Projectile.velocity * HoldoutRangeMin, Projectile.velocity * HoldoutRangeMax, progress);

            // Apply proper rotation to the sprite.
            // 对精灵图应用正确的旋转角度。
            if (Projectile.spriteDirection == -1)
            {
                // If sprite is facing left, rotate 45 degrees
                // 如果精灵图面向左，则旋转45度
                Projectile.rotation += MathHelper.ToRadians(45f);
            }
            else
            {
                // If sprite is facing right, rotate 135 degrees
                // 如果精灵图面向右，则旋转135度
                Projectile.rotation += MathHelper.ToRadians(135f);
            }

            // Avoid spawning dusts on dedicated servers
            // 避免在专用服务器上生成灰尘效果
            if (!Main.dedServ)
            {
                // These dusts are added later, for the 'AnalysisMod' effect
                // 这些灰尘稍后添加，“AnalysisMod”效果需要使用它们
                if (Main.rand.NextBool(3))
                {
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Sparkle>(), Projectile.velocity.X * 2f, Projectile.velocity.Y * 2f, Alpha: 128, Scale: 1.2f);
                }

                if (Main.rand.NextBool(4))
                {
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Sparkle>(), Alpha: 128, Scale: 0.3f);
                }
            }

            return false; // Don't execute vanilla AI.
                          // 不执行原始AI。
        }
    }
}
