using AnalysisMod.AnalysisContent.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Projectiles
{
    public class AnalysisWhipProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // This makes the projectile use whip collision detection and allows flasks to be applied to it.
            // 这使得抛射物使用鞭子碰撞检测，并允许将药剂瓶应用于其上。
            ProjectileID.Sets.IsAWhip[Type] = true;
        }

        public override void SetDefaults()
        {
            // This method quickly sets the whip's properties.
            // 此方法快速设置鞭子的属性。
            Projectile.DefaultToWhip();

            // use these to change from the vanilla defaults
            // 使用这些来从香草默认值更改

            // Projectile.WhipSettings.Segments = 20;
            // Projectile.WhipSettings.RangeMultiplier = 1f;
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

        // This Analysis uses PreAI to implement a charging mechanic.
        // If you remove this, also remove Item.channel = true from the item's SetDefaults.

        // 此分析使用 PreAI 实现充能机制。
        // 如果您删除此内容，则还要从项目的 SetDefaults 中删除 Item.channel = true。
        public override bool PreAI()
        {
            Player owner = Main.player[Projectile.owner];

            // Like other whips, this whip updates twice per frame (Projectile.extraUpdates = 1), so 120 is equal to 1 second.
            // 像其他鞭子一样，这个鞭子每帧更新两次（Projectile.extraUpdates = 1），因此 120 等于 1 秒钟。
            if (!owner.channel || ChargeTime >= 120)
            {
                return true; // Let the vanilla whip AI run.
                             // 让香草鞭 AI 运行。
            }

            if (++ChargeTime % 12 == 0) // 1 segment per 12 ticks of charge.
                                        // 每 12 次计费一个段落。
                Projectile.WhipSettings.Segments++;

            // Increase range up to 2x for full charge.
            // 在充满电的情况下，甩鞭范围增加到2倍。
            Projectile.WhipSettings.RangeMultiplier += 1 / 120f;

            // Reset the animation and item timer while charging.
            // 在充能时重置动画和物品计时器。
            owner.itemAnimation = owner.itemAnimationMax;
            owner.itemTime = owner.itemTimeMax;

            return false; // Prevent the vanilla whip AI from running.
                          // 防止运行香草鞭 AI。
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<AnalysisWhipDebuff>(), 240);
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
            Projectile.damage = (int)(Projectile.damage * 0.5f); // Multihit penalty. Decrease the damage the more enemies the whip hits.
                                                                 // 多击惩罚。随着鞭子命中敌人数量的增加而减少伤害量。
        }

        // This method draws a line between all points of the whip, in case there's empty space between the sprites.
        // 此方法在所有鞭点之间绘制线条，以防空白处有空间存在精灵图图像之间
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

            // 下面的代码是用于自定义绘图。
            // 如果不需要，请全部删除并调用其中一个香草 DrawWhip 方法，如上所述。
            // 不过，如果这样做，必须遵守它们绘制的方式

            SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.instance.LoadProjectile(Type);
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Vector2 pos = list[0];

            for (int i = 0; i < list.Count - 1; i++)
            {
                // These two values are set to suit this projectile's sprite, but won't necessarily work for your own.
                // You can change them if they don't!

                // 这两个值适合此抛射物的精灵图图像，但不一定适用于您自己的精灵图图像。
                // 如果它们不起作用，可以更改它们！
                Rectangle frame = new Rectangle(0, 0, 10, 26); // The size of the Handle (measured in pixels)
                                                               // 手柄的大小（以像素为单位）

                Vector2 origin = new Vector2(5, 8); // Offset for where the player's hand will start measured from the top left of the image.
                                                    // 从图像左上角开始测量玩家手部起始位置的偏移量。
                float scale = 1;

                // These statements determine what part of the spritesheet to draw for the current segment.
                // They can also be changed to suit your sprite.

                // 这些语句确定当前段要绘制精灵图表中的哪个部分。
                // 它们也可以根据您的精灵图进行更改。
                if (i == list.Count - 2)
                {
                    // This is the head of the whip. You need to measure the sprite to figure out these values.
                    // 这是鞭子的头。您需要测量精灵图来计算这些值
                    frame.Y = 74; // Distance from the top of the sprite to the start of the frame.
                                  // 距离帧开始处到精灵图顶部的距离

                    frame.Height = 18; // Height of the frame.
                                       // 帧图高度。

                    // For a more impactful look, this scales the tip of the whip up when fully extended, and down when curled up.
                    // 为了获得更具影响力的外观，当完全伸展时，它会将鞭尖缩放，并在卷曲时向下缩放。
                    Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
                    float t = Timer / timeToFlyOut;
                    scale = MathHelper.Lerp(0.5f, 1.5f, Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true));
                }
                else if (i > 10)
                {
                    // Third segment
                    // 第三段
                    frame.Y = 58;
                    frame.Height = 16;
                }
                else if (i > 5)
                {
                    // Second Segment
                    // 第二段
                    frame.Y = 42;
                    frame.Height = 16;
                }
                else if (i > 0)
                {
                    // First Segment
                    // 第一段
                    frame.Y = 26;
                    frame.Height = 16;
                }

                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2; // This projectile's sprite faces down, so PiOver2 is used to correct rotation.
                                                                         // 此投射物的精灵图朝下，因此使用PiOver2进行旋转修正。
                Color color = Lighting.GetColor(element.ToTileCoordinates());

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, flip, 0);

                pos += diff;
            }
            return false;
        }
    }
}
