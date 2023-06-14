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

    // �̽�Ͷ����Ĵ���ʽ������ƺ��˺������й�
    // �����п򡱱����������ң�����ͼ����������
    // ����������Ľ����������ڸ����п�ƫ�ƴ������ӽ����⣨CutTiles��Colliding��
    // ѡ���ֵ����Ӧ��Iron Shortword
    public class AnalysisShortswordProjectile : ModProjectile
    {
        public const int FadeInDuration = 7;
        public const int FadeOutDuration = 4;

        public const int TotalDuration = 16;

        // The "width" of the blade
        // ���С���ȡ�
        public float CollisionWidth => 10f * Projectile.scale;

        public int Timer
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(18); // This sets width and height to the same value (important when projectiles can rotate)
                                               // �⽫��Ⱥ͸߶�����Ϊ��ͬ��ֵ����Ͷ���������תʱ����Ҫ��

            Projectile.aiStyle = -1; // Use our own AI to customize how it behaves, if you don't want that, keep this at ProjAIStyleID.ShortSword. You would still need to use the code in SetVisualOffsets() though
                                     // ʹ�������Լ��� AI ���Զ�������Ϊ������������������뱣�� ProjAIStyleID.ShortSword��Ȼ��������Ҫʹ�� SetVisualOffsets() �еĴ���

            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.scale = 1f;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ownerHitCheck = true; // Prevents hits through tiles. Most melee weapons that use projectiles have this
                                             // ��ֹ������ש���򡣴����ʹ��Ͷ����Ľ�ս���������д˹���

            Projectile.extraUpdates = 1; // Update 1+extraUpdates times per tick
                                         // ÿ�� tick ���� 1+extraUpdates ��

            Projectile.timeLeft = 360; // This value does not matter since we manually kill it earlier, it just has to be higher than the duration we use in AI
                                       // ���������Ժ��ֶ�ɱ��������˴�ֵ�޹ؽ�Ҫ��ֻ��� AI ��ʹ�õĳ���ʱ�䳤���ɡ�

            Projectile.hide = true; // Important when used alongside player.heldProj. "Hidden" projectiles have special draw conditions
                                    // ���� player.heldProj ���ʹ��ʱ�ǳ���Ҫ�������ء���Ͷ��������������������
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Timer += 1;
            if (Timer >= TotalDuration)
            {
                // Kill the projectile if it reaches it's intented lifetime
                // ����ﵽԤ����������ɱ����Ͷ����
                Projectile.Kill();
                return;
            }
            else
            {
                // Important so that the sprite draws "in" the player's hand and not fully infront or behind the player
                // �ǳ���Ҫ���Ա㾫��ͼ����桱��������У����Ҳ���ȫ���������ǰ�����档
                player.heldProj = Projectile.whoAmI;
            }

            // Fade in and out
            // GetLerpValue returns a value between 0f and 1f - if clamped is true - representing how far Timer got along the "distance" defined by the first two parameters
            // The first call handles the fade in, the second one the fade out.
            // Notice the second call's parameters are swapped, this means the result will be reverted

            // ���뽥��
            // GetLerpValue ����һ��ֵ�� 0f �� 1f ֮�� - ��� clamped �� true - ��ʾ Timer ������ǰ������������ġ����롱���˶�Զ
            // ��һ�ε��ô����룬�ڶ��ε��ô�������
            // ע��ڶ������õĲ����ѽ���������ζ�Ž��������ת
            Projectile.Opacity = Utils.GetLerpValue(0f, FadeInDuration, Timer, clamped: true) * Utils.GetLerpValue(TotalDuration, TotalDuration - FadeOutDuration, Timer, clamped: true);

            // Keep locked onto the player, but extend further based on the given velocity (Requires ShouldUpdatePosition returning false to work)
            // ��������ң������ݸ����ٶȽ�һ�����죨��Ҫ ShouldUpdatePosition ���� false ���ܹ�����
            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, reverseRotation: false, addGfxOffY: false);
            Projectile.Center = playerCenter + Projectile.velocity * (Timer - 1f);

            // Set spriteDirection based on moving left or right. Left -1, right 1
            // ��������������ƶ����� spriteDirection����-1����1
            Projectile.spriteDirection = (Vector2.Dot(Projectile.velocity, Vector2.UnitX) >= 0f).ToDirectionInt();

            // Point towards where it is moving, applied offset for top right of the sprite respecting spriteDirection
            // ָ�����ƶ����򣬲�Ӧ��ƫ�������� spriteDirection �ľ���ͼ�����Ҳ�
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 - MathHelper.PiOver4 * Projectile.spriteDirection;

            // The code in this method is important to align the sprite with the hitbox how we want it to
            // �˷����еĴ������ʹ����ͼ�����п�������Ҫ�ķ�ʽ����ǳ���Ҫ
            SetVisualOffsets();
        }

        private void SetVisualOffsets()
        {
            // 32 is the sprite size (here both width and height equal)
            // 32�Ǿ���ͼ��С����Ⱥ͸߶���ȣ�
            const int HalfSpriteWidth = 32 / 2;
            const int HalfSpriteHeight = 32 / 2;

            int HalfProjWidth = Projectile.width / 2;
            int HalfProjHeight = Projectile.height / 2;

            // Vanilla configuration for "hitbox in middle of sprite"
            //����ײ���ھ���ͼ�м䡱��ԭʼ����
            DrawOriginOffsetX = 0;
            DrawOffsetX = -(HalfSpriteWidth - HalfProjWidth);
            DrawOriginOffsetY = -(HalfSpriteHeight - HalfProjHeight);

            // Vanilla configuration for "hitbox towards the end"
            // ����ײ�򿿽�ĩ�ˡ���ԭʼ����

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
            // �ֶ�����Projectile.Center
            return false;
        }

        public override void CutTiles()
        {
            // "cutting tiles" refers to breaking pots, grass, queen bee larva, etc.
            // ���и�ͼ�顱ָ���Ǵ��ƹ��ӡ��ݡ������׳�ȡ�
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 start = Projectile.Center;
            Vector2 end = start + Projectile.velocity.SafeNormalize(-Vector2.UnitY) * 10f;
            Utils.PlotTileLine(start, end, CollisionWidth, DelegateMethods.CutTiles);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // "Hit anything between the player and the tip of the sword"
            // shootSpeed is 2.1f for reference, so this is basically plotting 12 pixels ahead from the center

            //��������Һͽ���֮����κζ�����
            // shootSpeedΪ2.1f����˻����ϴ�������ǰ����12������
            Vector2 start = Projectile.Center;
            Vector2 end = start + Projectile.velocity * 6f;
            float collisionPoint = 0f; // Don't need that variable, but required as parameter
                                       // ����Ҫ�ñ���������Ϊ�����Ǳ����

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, CollisionWidth, ref collisionPoint);
        }
    }
}
