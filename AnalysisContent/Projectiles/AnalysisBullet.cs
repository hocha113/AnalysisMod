using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Projectiles
{
    public class AnalysisBullet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; // The length of old position to be recorded
                                                                     // 需要记录的旧位置长度

            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
                                                                 // 录制模式
        }

        public override void SetDefaults()
        {
            Projectile.width = 8; // The width of projectile hitbox
                                  // 弹道命中框宽度

            Projectile.height = 8; // The height of projectile hitbox
                                   // 弹道命中框高度

            Projectile.aiStyle = 1; // The ai style of the projectile, please reference the source code of Terraria
                                    // 弹道的AI风格，请参考Terraria源代码。

            Projectile.friendly = true; // Can the projectile deal damage to enemies?
                                        // 弹道能否对敌人造成伤害？

            Projectile.hostile = false; // Can the projectile deal damage to the player?
                                        // 弹道能否对玩家造成伤害？

            Projectile.DamageType = DamageClass.Ranged; // Is the projectile shoot by a ranged weapon?
                                                        // 该弹道是否由远程武器发射？

            Projectile.penetrate = 5; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
                                      // 该弹道可以穿透多少个怪物。（OnTileCollide也会减少反射次数）

            Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
                                       // 该弹道存在时间（60 = 1秒，因此600为10秒）

            Projectile.alpha = 255; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
                                    // 该弹道的透明度，255表示完全透明。（aiStyle 1快速淡化了这个投影）如果您没有使用淡入效果，则确保删除此内容。否则你会惊讶地发现你的投影是看不见的。

            Projectile.light = 0.5f; // How much light emit around the projectile
                                     // 在投影周围发出多少光线。

            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
                                           // 该投影是否受水流速度影响？

            Projectile.tileCollide = true; // Can the projectile collide with tiles?
                                           // 该投影是否可以与瓷砖碰撞？

            Projectile.extraUpdates = 1; // Set to above 0 if you want the projectile to update multiple time in a frame
                                         // 如果要使项目在一帧内更新多次，请将其设置为大于0.

            AIType = ProjectileID.Bullet; // Act exactly like default Bullet
                                          // 与默认子弹完全相同
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // If collide with tile, reduce the penetrate.
            // So the projectile can reflect at most 5 times

            // 如果与瓷砖碰撞，则减少穿透力。
            // 因此，最多可以反射5次抛物线。
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
            {
                Projectile.Kill();
            }
            else
            {
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

                // If the projectile hits the left or right side of the tile, reverse the X velocity
                // 如果抛物线击中平铺图块左侧或右侧，则反转X轴速度。
                if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
                {
                    Projectile.velocity.X = -oldVelocity.X;
                }

                // If the projectile hits the top or bottom side of the tile, reverse the Y velocity
                // 如果抛物线击中平铺图块的顶部或底部，则反转Y轴速度。
                if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
                {
                    Projectile.velocity.Y = -oldVelocity.Y;
                }
            }

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            // Redraw the projectile with the color not influenced by light
            // 重新绘制颜色不受光线影响的投影
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return true;
        }

        public override void Kill(int timeLeft)
        {
            // This code and the similar code above in OnTileCollide spawn dust from the tiles collided with. SoundID.Item10 is the bounce sound you hear.
            // 此代码及OnTileCollide中上面类似的代码会从与瓷砖碰撞的地方产生灰尘。 SoundID.Item10是您听到的弹跳声音。
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        }
    }
}