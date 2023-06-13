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
                                                                     // ��Ҫ��¼�ľ�λ�ó���

            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
                                                                 // ¼��ģʽ
        }

        public override void SetDefaults()
        {
            Projectile.width = 8; // The width of projectile hitbox
                                  // �������п���

            Projectile.height = 8; // The height of projectile hitbox
                                   // �������п�߶�

            Projectile.aiStyle = 1; // The ai style of the projectile, please reference the source code of Terraria
                                    // ������AI�����ο�TerrariaԴ���롣

            Projectile.friendly = true; // Can the projectile deal damage to enemies?
                                        // �����ܷ�Ե�������˺���

            Projectile.hostile = false; // Can the projectile deal damage to the player?
                                        // �����ܷ���������˺���

            Projectile.DamageType = DamageClass.Ranged; // Is the projectile shoot by a ranged weapon?
                                                        // �õ����Ƿ���Զ���������䣿

            Projectile.penetrate = 5; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
                                      // �õ������Դ�͸���ٸ������OnTileCollideҲ����ٷ��������

            Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
                                       // �õ�������ʱ�䣨60 = 1�룬���600Ϊ10�룩

            Projectile.alpha = 255; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
                                    // �õ�����͸���ȣ�255��ʾ��ȫ͸������aiStyle 1���ٵ��������ͶӰ�������û��ʹ�õ���Ч������ȷ��ɾ�������ݡ�������ᾪ�ȵط������ͶӰ�ǿ������ġ�

            Projectile.light = 0.5f; // How much light emit around the projectile
                                     // ��ͶӰ��Χ�������ٹ��ߡ�

            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
                                           // ��ͶӰ�Ƿ���ˮ���ٶ�Ӱ�죿

            Projectile.tileCollide = true; // Can the projectile collide with tiles?
                                           // ��ͶӰ�Ƿ�������ש��ײ��

            Projectile.extraUpdates = 1; // Set to above 0 if you want the projectile to update multiple time in a frame
                                         // ���Ҫʹ��Ŀ��һ֡�ڸ��¶�Σ��뽫������Ϊ����0.

            AIType = ProjectileID.Bullet; // Act exactly like default Bullet
                                          // ��Ĭ���ӵ���ȫ��ͬ
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // If collide with tile, reduce the penetrate.
            // So the projectile can reflect at most 5 times

            // ������ש��ײ������ٴ�͸����
            // ��ˣ������Է���5�������ߡ�
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
                // ��������߻���ƽ��ͼ�������Ҳ࣬��תX���ٶȡ�
                if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
                {
                    Projectile.velocity.X = -oldVelocity.X;
                }

                // If the projectile hits the top or bottom side of the tile, reverse the Y velocity
                // ��������߻���ƽ��ͼ��Ķ�����ײ�����תY���ٶȡ�
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
            // ���»�����ɫ���ܹ���Ӱ���ͶӰ
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
            // �˴��뼰OnTileCollide���������ƵĴ��������ש��ײ�ĵط������ҳ��� SoundID.Item10���������ĵ���������
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        }
    }
}