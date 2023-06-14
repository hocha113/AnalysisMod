using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Projectiles
{
    // Shortsword projectiles are handled in a special way with how they draw and damage things
    // The "hitbox" itself is closer to the player, the sprite is centered on it
    // However the interactions with the world will occur offset from this hitbox, closer to the sword's tip (CutTiles, Colliding)
    // Values chosen mostly correspond to Iron Shortword

    // 短剑投射物的处理方式与其绘制和伤害对象有关
    // “命中框”本身更靠近玩家，精灵图居中于其中
    // 但是与世界的交互将发生在该命中框偏移处，更接近剑尖（CutTiles、Colliding）
    // 选择的值大多对应于Iron Shortword
    public class AnalysisShortswordProjectile : ModProjectile
    {
        public const int FadeInDuration = 7;
        public const int FadeOutDuration = 4;

        public const int TotalDuration = 16;

        // The "width" of the blade
        // 刀刃“宽度”
        public float CollisionWidth => 10f * Projectile.scale;

        public int Timer
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(18); // This sets width and height to the same value (important when projectiles can rotate)
                                               // 这将宽度和高度设置为相同的值（当投射物可以旋转时很重要）

            Projectile.aiStyle = -1; // Use our own AI to customize how it behaves, if you don't want that, keep this at ProjAIStyleID.ShortSword. You would still need to use the code in SetVisualOffsets() though
                                     // 使用我们自己的 AI 来自定义其行为，如果不想这样做，请保持 ProjAIStyleID.ShortSword。然而您仍需要使用 SetVisualOffsets() 中的代码

            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.scale = 1f;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ownerHitCheck = true; // Prevents hits through tiles. Most melee weapons that use projectiles have this
                                             // 防止穿过瓷砖击打。大多数使用投射物的近战武器都具有此功能

            Projectile.extraUpdates = 1; // Update 1+extraUpdates times per tick
                                         // 每个 tick 更新 1+extraUpdates 次

            Projectile.timeLeft = 360; // This value does not matter since we manually kill it earlier, it just has to be higher than the duration we use in AI
                                       // 由于我们稍后手动杀死它，因此此值无关紧要，只需比 AI 中使用的持续时间长即可。

            Projectile.hide = true; // Important when used alongside player.heldProj. "Hidden" projectiles have special draw conditions
                                    // 当与 player.heldProj 结合使用时非常重要。“隐藏”的投射物具有特殊绘制条件。
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Timer += 1;
            if (Timer >= TotalDuration)
            {
                // Kill the projectile if it reaches it's intented lifetime
                // 如果达到预定寿命，则杀死该投射物
                Projectile.Kill();
                return;
            }
            else
            {
                // Important so that the sprite draws "in" the player's hand and not fully infront or behind the player
                // 非常重要，以便精灵图“描绘”在玩家手中，并且不完全出现在玩家前面或后面。
                player.heldProj = Projectile.whoAmI;
            }

            // Fade in and out
            // GetLerpValue returns a value between 0f and 1f - if clamped is true - representing how far Timer got along the "distance" defined by the first two parameters
            // The first call handles the fade in, the second one the fade out.
            // Notice the second call's parameters are swapped, this means the result will be reverted

            // 渐入渐出
            // GetLerpValue 返回一个值在 0f 和 1f 之间 - 如果 clamped 是 true - 表示 Timer 沿着由前两个参数定义的“距离”走了多远
            // 第一次调用处理淡入，第二次调用处理淡出。
            // 注意第二个调用的参数已交换，这意味着结果将被反转
            Projectile.Opacity = Utils.GetLerpValue(0f, FadeInDuration, Timer, clamped: true) * Utils.GetLerpValue(TotalDuration, TotalDuration - FadeOutDuration, Timer, clamped: true);

            // Keep locked onto the player, but extend further based on the given velocity (Requires ShouldUpdatePosition returning false to work)
            // 锁定到玩家，但根据给定速度进一步延伸（需要 ShouldUpdatePosition 返回 false 才能工作）
            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, reverseRotation: false, addGfxOffY: false);
            Projectile.Center = playerCenter + Projectile.velocity * (Timer - 1f);

            // Set spriteDirection based on moving left or right. Left -1, right 1
            // 根据向左或向右移动设置 spriteDirection。左-1、右1
            Projectile.spriteDirection = (Vector2.Dot(Projectile.velocity, Vector2.UnitX) >= 0f).ToDirectionInt();

            // Point towards where it is moving, applied offset for top right of the sprite respecting spriteDirection
            // 指向其移动方向，并应用偏移以尊重 spriteDirection 的精灵图顶部右侧
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 - MathHelper.PiOver4 * Projectile.spriteDirection;

            // The code in this method is important to align the sprite with the hitbox how we want it to
            // 此方法中的代码对于使精灵图与命中框按我们想要的方式对齐非常重要
            SetVisualOffsets();
        }

        private void SetVisualOffsets()
        {
            // 32 is the sprite size (here both width and height equal)
            // 32是精灵图大小（宽度和高度相等）
            const int HalfSpriteWidth = 32 / 2;
            const int HalfSpriteHeight = 32 / 2;

            int HalfProjWidth = Projectile.width / 2;
            int HalfProjHeight = Projectile.height / 2;

            // Vanilla configuration for "hitbox in middle of sprite"
            //“碰撞框在精灵图中间”的原始配置
            DrawOriginOffsetX = 0;
            DrawOffsetX = -(HalfSpriteWidth - HalfProjWidth);
            DrawOriginOffsetY = -(HalfSpriteHeight - HalfProjHeight);

            // Vanilla configuration for "hitbox towards the end"
            // “碰撞框靠近末端”的原始配置

            //if (Projectile.spriteDirection == 1) {
            //	DrawOriginOffsetX = -(HalfProjWidth - HalfSpriteWidth);
            //	DrawOffsetX = (int)-DrawOriginOffsetX * 2;
            //	DrawOriginOffsetY = 0;
            //}
            //else {
            //	DrawOriginOffsetX = (HalfProjWidth - HalfSpriteWidth);
            //	DrawOffsetX = 0;
            //	DrawOriginOffsetY = 0;
            //}
        }

        public override bool ShouldUpdatePosition()
        {
            // Update Projectile.Center manually
            // 手动更新Projectile.Center
            return false;
        }

        public override void CutTiles()
        {
            // "cutting tiles" refers to breaking pots, grass, queen bee larva, etc.
            // “切割图块”指的是打破罐子、草、蜜王幼虫等。
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 start = Projectile.Center;
            Vector2 end = start + Projectile.velocity.SafeNormalize(-Vector2.UnitY) * 10f;
            Utils.PlotTileLine(start, end, CollisionWidth, DelegateMethods.CutTiles);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // "Hit anything between the player and the tip of the sword"
            // shootSpeed is 2.1f for reference, so this is basically plotting 12 pixels ahead from the center

            //“击中玩家和剑尖之间的任何东西”
            // shootSpeed为2.1f，因此基本上从中心向前绘制12个像素
            Vector2 start = Projectile.Center;
            Vector2 end = start + Projectile.velocity * 6f;
            float collisionPoint = 0f; // Don't need that variable, but required as parameter
                                       // 不需要该变量，但作为参数是必需的

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, CollisionWidth, ref collisionPoint);
        }
    }
}
