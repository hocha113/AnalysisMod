using AnalysisMod.AnalysisContent.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Projectiles
{
    public class AnalysisWhipProjectileAdvanced : ModProjectile
    {
        // The texture doesn't have the same name as the item, so this property points to it.
        // 纹理名称与物品名称不同，因此该属性指向纹理。
        public override string Texture => "AnalysisMod/AnalysisContent/Projectiles/AnalysisWhipProjectile";

        public override void SetStaticDefaults()
        {
            // This makes the projectile use whip collision detection and allows flasks to be applied to it.
            // 这使得抛射物使用鞭子碰撞检测，并允许将烧瓶应用于其上。
            ProjectileID.Sets.IsAWhip[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true; // This prevents the projectile from hitting through solid tiles.
                                             // 这可以防止抛射物穿过实心方块。
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.WhipSettings.Segments = 10;
            Projectile.WhipSettings.RangeMultiplier = 1.5f;
        }

        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private float ChargeTime
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2; // Without PiOver2, the rotation would be off by 90 degrees counterclockwise.
                                                                                         // 如果没有 PiOver2，则旋转会逆时针偏移90度。

            Projectile.Center = Main.GetPlayerArmPosition(Projectile) + Projectile.velocity * Timer;
            // Vanilla uses Vector2.Dot(Projectile.velocity, Vector2.UnitX) here. Dot Product returns the difference between two vectors, 0 meaning they are perpendicular.
            // However, the use of UnitX basically turns it into a more complicated way of checking if the projectile's velocity is above or equal to zero on the X axis.

            // Vanilla 在此处使用 Vector2.Dot(Projectile.velocity, Vector2.UnitX)。点积返回两个向量之间的差异，0表示它们垂直。
            // 但是，UnitX的使用基本上将其变成了一种更复杂的方式来检查抛射物速度在X轴上是否大于或等于零。
            Projectile.spriteDirection = Projectile.velocity.X >= 0f ? 1 : -1;

            // remove these 3 lines if you don't want the charging mechanic
            // 如果您不想要充能机制，请删除这3行
            if (!Charge(owner))
            {
                return; // timer doesn't update while charging, freezing the animation at the start.
                        // 计时器在充能时不更新，在开始时冻结动画。
            }

            Timer++;

            float swingTime = owner.itemAnimationMax * Projectile.MaxUpdates;
            if (Timer >= swingTime || owner.itemAnimation <= 0)
            {
                Projectile.Kill();
                return;
            }

            owner.heldProj = Projectile.whoAmI;
            if (Timer == swingTime / 2)
            {
                // Plays a whipcrack sound at the tip of the whip.
                // 在鞭子顶端播放鞭打声音。
                List<Vector2> points = Projectile.WhipPointsForCollision;
                Projectile.FillWhipControlPoints(Projectile, points);
                SoundEngine.PlaySound(SoundID.Item153, points[points.Count - 1]);
            }

            // Spawn Dust along the whip path
            // This is the dust code used by Durendal. Consult the Terraria source code for even more Analysiss, found in Projectile.AI_165_Whip.

            // 沿着鞭路径生成灰尘
            // 这是Durendal使用的灰尘代码。有关更多分析，请参考Terraria源代码，位于Projectile.AI_165_Whip中找到。
            float swingProgress = Timer / swingTime;
            // This code limits dust to only spawn during the the actual swing.
            // 此代码限制粉尘仅在实际摆动期间产生。
            if (Utils.GetLerpValue(0.1f, 0.7f, swingProgress, clamped: true) * Utils.GetLerpValue(0.9f, 0.7f, swingProgress, clamped: true) > 0.5f && !Main.rand.NextBool(3))
            {
                List<Vector2> points = Projectile.WhipPointsForCollision;
                points.Clear();
                Projectile.FillWhipControlPoints(Projectile, points);
                int pointIndex = Main.rand.Next(points.Count - 10, points.Count);
                Rectangle spawnArea = Utils.CenteredRectangle(points[pointIndex], new Vector2(30f, 30f));
                int dustType = DustID.Enchanted_Gold;
                if (Main.rand.NextBool(3))
                    dustType = DustID.TintableDustLighted;

                // After choosing a randomized dust and a whip segment to spawn from, dust is spawned.
                // 在选择随机化粉末和鞭节进行生成后，会产生粉末。
                Dust dust = Dust.NewDustDirect(spawnArea.TopLeft(), spawnArea.Width, spawnArea.Height, dustType, 0f, 0f, 100, Color.White);
                dust.position = points[pointIndex];
                dust.fadeIn = 0.3f;
                Vector2 spinningpoint = points[pointIndex] - points[pointIndex - 1];
                dust.noGravity = true;
                dust.velocity *= 0.5f;
                // This math causes these dust to spawn with a velocity perpendicular to the direction of the whip segments, giving the impression of the dust flying off like sparks.
                // 此数学公式导致这些灰尘以垂直于鞭节方向的速度生成，给人一种灰尘像火花飞舞的印象。
                dust.velocity += spinningpoint.RotatedBy(owner.direction * ((float)Math.PI / 2f));
                dust.velocity *= 0.5f;
            }
        }

        // This method handles a charging mechanic.
        // If you remove this, also remove Item.channel = true from the item's SetDefaults.
        // Returns true if fully charged

        // 此方法处理充能机制。
        // 如果您删除此内容，请从物品的SetDefaults中也删除Item.channel = true。
        // 充能完全时返回true
        private bool Charge(Player owner)
        {
            // Like other whips, this whip updates twice per frame (Projectile.extraUpdates = 1), so 120 is equal to 1 second.
            // 像其他鞭子一样，这个鞭子每帧更新两次（Projectile.extraUpdates = 1），因此120等于1秒。
            if (!owner.channel || ChargeTime >= 120)
            {
                return true; // finished charging
                             // 充能完成
            }

            ChargeTime++;

            if (ChargeTime % 12 == 0) // 1 segment per 12 ticks of charge.
                                      // 每12个tick的充能增加一个段落。

                Projectile.WhipSettings.Segments++;

            // Increase range up to 2x for full charge.

            Projectile.WhipSettings.RangeMultiplier += 1 / 120f;

            // Reset the animation and item timer while charging.
            // 充能时将动画和物品计时器重置。
            owner.itemAnimation = owner.itemAnimationMax;
            owner.itemTime = owner.itemTimeMax;

            return false; // still charging
                          // 仍在充能中
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<AnalysisWhipAdvancedDebuff>(), 240);
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
            Projectile.damage = (int)(Projectile.damage * 0.7f); // Multihit penalty. Decrease the damage the more enemies the whip hits.
                                                                 // 多次打击惩罚。随着鞭子击中敌人数量的增加，减少伤害。
        }

        // This method draws a line between all points of the whip, in case there's empty space between the sprites.
        // 此方法在鞭子所有点之间绘制线条，以防精灵图之间有空白区域。
        private void DrawLine(List<Vector2> list)
        {
            Texture2D texture = TextureAssets.FishingLine.Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = new Vector2(frame.Width / 2, 2);

            Vector2 pos = list[0];
            for (int i = 0; i < list.Count - 1; i++)
            {
                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2;
                Color color = Lighting.GetColor(element.ToTileCoordinates(), Color.White);
                Vector2 scale = new Vector2(1, (diff.Length() + 2) / frame.Height);

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0);

                pos += diff;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            List<Vector2> list = new List<Vector2>();
            Projectile.FillWhipControlPoints(Projectile, list);

            DrawLine(list);

            //Main.DrawWhip_WhipBland(Projectile, list);

            // The code below is for custom drawing.
            // If you don't want that, you can remove it all and instead call one of vanilla's DrawWhip methods, like above.
            // However, you must adhere to how they draw if you do.

            // 下面的代码用于自定义绘图。
            // 如果您不需要，请删除所有内容并调用其中一个vanilla's DrawWhip方法。
            // 但是，如果这样做，则必须遵守它们的绘制方式。

            SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.instance.LoadProjectile(Type);
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Vector2 pos = list[0];

            for (int i = 0; i < list.Count - 1; i++)
            {
                // These two values are set to suit this projectile's sprite, but won't necessarily work for your own.
                // You can change them if they don't!

                // 这两个值适合此projectile的sprite设置，但不一定适合您自己的sprite。
                // 如果不行可以更改！
                Rectangle frame = new Rectangle(0, 0, 10, 26); // The size of the Handle (measured in pixels)
                                                               // 手柄大小（以像素为单位）

                Vector2 origin = new Vector2(5, 8); // Offset for where the player's hand will start measured from the top left of the image.
                                                    // 从图像左上角开始测量玩家手部起始位置的偏移量。

                float scale = 1;

                // These statements determine what part of the spritesheet to draw for the current segment.
                // They can also be changed to suit your sprite.

                // 这些语句确定当前段落要绘制spritesheet 的哪个部分。
                // 它们也可以更改以适应您的sprite.
                if (i == list.Count - 2)
                {
                    // This is the head of the whip. You need to measure the sprite to figure out these values.
                    // 这是鞭头。 您需要测量精灵图才能找出这些值。
                    frame.Y = 74; // Distance from the top of the sprite to the start of the frame.
                                  // 距离精灵图顶部到框架开始的距离。

                    frame.Height = 18; // Height of the frame.

                    // For a more impactful look, this scales the tip of the whip up when fully extended, and down when curled up.
                    // 为了更具冲击力，当鞭子完全伸展时，它会将鞭尖缩放，并在卷曲时将其缩小。
                    Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
                    float t = Timer / timeToFlyOut;
                    scale = MathHelper.Lerp(0.5f, 1.5f, Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true));
                }
                else if (i > 10)
                {
                    // Third segment
                    frame.Y = 58;
                    frame.Height = 16;
                }
                else if (i > 5)
                {
                    // Second Segment
                    frame.Y = 42;
                    frame.Height = 16;
                }
                else if (i > 0)
                {
                    // First Segment
                    frame.Y = 26;
                    frame.Height = 16;
                }

                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2; // This projectile's sprite faces down, so PiOver2 is used to correct rotation.
                Color color = Lighting.GetColor(element.ToTileCoordinates());

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, flip, 0);

                pos += diff;
            }
            return false;
        }
    }
}
