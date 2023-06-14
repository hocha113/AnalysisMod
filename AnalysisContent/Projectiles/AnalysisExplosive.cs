using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Projectiles
{
    // This projectile demonstrates exploding tiles (like a bomb or dynamite), spawning child projectiles, and explosive visual effects.
    // TODO: This projectile does not currently damage the owner, or damage other players on the For the worthy secret seed.

    // 这个弹射物展示了爆炸的方块（像炸弹或炸药），生成子弹，以及爆炸视觉效果。
    // TODO: 这个弹射物目前不会对拥有者造成伤害，也不会在“FTW”的秘密种子中对其他玩家造成伤害。
    public class AnalysisExplosive : ModProjectile
    {
        private const int DefaultWidthHeight = 15;
        private const int ExplosionWidthHeight = 250;

        public override void SetDefaults()
        {
            // While the sprite is actually bigger than 15x15, we use 15x15 since it lets the projectile clip into tiles as it bounces. It looks better.
            // 虽然精灵图实际上比15x15大，但我们使用15x15是因为这样可以让弹射物在反弹时裁剪到方块内。看起来更好。
            Projectile.width = DefaultWidthHeight;
            Projectile.height = DefaultWidthHeight;
            Projectile.friendly = true;
            Projectile.penetrate = -1;

            // 5 second fuse.
            // 5秒引线。
            Projectile.timeLeft = 300;

            // These help the projectile hitbox be centered on the projectile sprite.
            // 这些帮助使得弹射物的碰撞框居中于其精灵图之上。
            DrawOffsetX = -2;
            DrawOriginOffsetY = -5;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // Vanilla explosions do less damage to Eater of Worlds in expert mode, so we will too.
            // 在专家模式下，馋嘴蛇受到普通爆炸的伤害较小，所以我们也一样做。
            if (Main.expertMode)
            {
                if (target.type >= NPCID.EaterofWorldsHead && target.type <= NPCID.EaterofWorldsTail)
                {
                    modifiers.FinalDamage /= 5;
                }
            }
        }

        // The projectile is very bouncy, but the spawned children projectiles shouldn't bounce at all.
        // 弹射物非常有反弹力，但生成的子项目不应该反弹。
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Die immediately if ai[1] isn't 0 (We set this to 1 for the 5 extra explosives we spawn in Kill)
            // 如果ai[1]不等于0，则立即死亡（我们将其设置为1，在Kill函数中用于产生额外的5个爆炸）。
            if (Projectile.ai[1] != 0)
            {
                return true;
            }
            // OnTileCollide can trigger quite frequently, so using soundDelay helps prevent the sound from overlapping too much.
            // OnTileCollide可能会频繁触发，因此使用soundDelay可以防止声音重叠太多次数
            if (Projectile.soundDelay == 0)
            {
                // We adjust Volume since the sound is a bit too loud. PitchVariance gives the sound some random pitch variance.
                // 我们调整Volume值因为声音有点太响了。PitchVariance给声音一些随机的音高变化。
                SoundStyle impactSound = new SoundStyle($"{nameof(AnalysisMod)}/Assets/Sounds/Items/BananaImpact")
                {
                    Volume = 0.7f,
                    PitchVariance = 0.5f,
                };
                SoundEngine.PlaySound(impactSound);
            }
            Projectile.soundDelay = 10;

            // This code makes the projectile very bouncy.
            // 这段代码使得弹射物非常有反弹力。
            if (Projectile.velocity.X != oldVelocity.X && Math.Abs(oldVelocity.X) > 1f)
            {
                Projectile.velocity.X = oldVelocity.X * -0.9f;
            }
            if (Projectile.velocity.Y != oldVelocity.Y && Math.Abs(oldVelocity.Y) > 1f)
            {
                Projectile.velocity.Y = oldVelocity.Y * -0.9f;
            }
            return false;
        }

        public override void AI()
        {
            // The projectile is in the midst of exploding during the last 3 updates.
            // 在最后3次更新期间，弹射物正在爆炸中。
            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 3)
            {
                Projectile.tileCollide = false;
                // Set to transparent. This projectile technically lives as transparent for about 3 frames
                // 设置为透明。这个弹射物在技术上作为透明状态存在约3帧时间
                Projectile.alpha = 255;

                // change the hitbox size, centered about the original projectile center. This makes the projectile damage enemies during the explosion.
                // 更改碰撞框大小，以原始弹射物中心为中心。这样可以让爆炸时的弹射物对敌人造成伤害。
                Projectile.Resize(ExplosionWidthHeight, ExplosionWidthHeight);

                Projectile.damage = 250;
                Projectile.knockBack = 10f;
            }
            else
            {
                // Smoke and fuse dust spawn. The position is calculated to spawn the dust directly on the fuse.
                // 产生烟雾和引线灰尘。位置计算是直接在引线上生成灰尘。
                if (Main.rand.NextBool())
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 1f);
                    dust.scale = 0.1f + Main.rand.Next(5) * 0.1f;
                    dust.fadeIn = 1.5f + Main.rand.Next(5) * 0.1f;
                    dust.noGravity = true;
                    dust.position = Projectile.Center + new Vector2(1, 0).RotatedBy(Projectile.rotation - 2.1f, default) * 10f;

                    dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1f);
                    dust.scale = 1f + Main.rand.Next(5) * 0.1f;
                    dust.noGravity = true;
                    dust.position = Projectile.Center + new Vector2(1, 0).RotatedBy(Projectile.rotation - 2.1f, default) * 10f;
                }
            }
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 10f)
            {
                Projectile.ai[0] = 10f;
                // Roll speed dampening. 
                // 滚动速度减缓
                if (Projectile.velocity.Y == 0f && Projectile.velocity.X != 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X * 0.96f;

                    if (Projectile.velocity.X > -0.01 && Projectile.velocity.X < 0.01)
                    {
                        Projectile.velocity.X = 0f;
                        Projectile.netUpdate = true;
                    }
                }
                // Delayed gravity
                // 延迟重力
                Projectile.velocity.Y = Projectile.velocity.Y + 0.2f;
            }
            // Rotation increased by velocity.X 
            // 根据velocity.X增加旋转角度
            Projectile.rotation += Projectile.velocity.X * 0.1f;
        }

        public override void Kill(int timeLeft)
        {
            // If we are the original projectile running on the owner, spawn the 5 child projectiles.
            // 如果我们是运行在拥有者身上的原始弹射物，则生成5个子项目。
            if (Projectile.owner == Main.myPlayer && Projectile.ai[1] == 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    // Random upward vector.
                    // 随机向上向量。
                    Vector2 launchVelocity = new Vector2(Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-10, -8));
                    // Importantly, ai1 is set to 1 here. This is checked in OnTileCollide to prevent bouncing and here in Kill to prevent an infinite chain of splitting projectiles.
                    // 重要的是，在此处设置ai1等于1. OnTileCollide函数会检查此值以防止反复分裂，并且Kill函数也会检查此值以防止无限链式分裂子项目发生。
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, launchVelocity, Projectile.type, Projectile.damage, Projectile.knockBack, Main.myPlayer, 0, 1);
                }
            }

            // Play explosion sound
            // 播放爆炸声音
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            // Smoke Dust spawn
            // 产生烟雾粉末
            for (int i = 0; i < 50; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
                dust.velocity *= 1.4f;
            }

            // Fire Dust spawn
            // 产生火焰粉末
            for (int i = 0; i < 80; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3f);
                dust.noGravity = true;
                dust.velocity *= 5f;
                dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 2f);
                dust.velocity *= 3f;
            }

            // Large Smoke Gore spawn
            // 大型烟雾Gore出现
            for (int g = 0; g < 2; g++)
            {
                var goreSpawnPosition = new Vector2(Projectile.position.X + Projectile.width / 2 - 24f, Projectile.position.Y + Projectile.height / 2 - 24f);
                Gore gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
                gore.scale = 1.5f;
                gore.velocity.X += 1.5f;
                gore.velocity.Y += 1.5f;
                gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
                gore.scale = 1.5f;
                gore.velocity.X -= 1.5f;
                gore.velocity.Y += 1.5f;
                gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
                gore.scale = 1.5f;
                gore.velocity.X += 1.5f;
                gore.velocity.Y -= 1.5f;
                gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
                gore.scale = 1.5f;
                gore.velocity.X -= 1.5f;
                gore.velocity.Y -= 1.5f;
            }
            // reset size to normal width and height.
            // 尺寸恢复到正常宽度和高度。
            Projectile.Resize(DefaultWidthHeight, DefaultWidthHeight);

            // Finally, actually explode the tiles and walls. Run this code only for the owner
            // 最后，实际上爆炸方块和墙壁。仅为拥有者运行此代码
            if (Projectile.owner == Main.myPlayer)
            {
                int explosionRadius = 7;
                int minTileX = (int)(Projectile.Center.X / 16f - explosionRadius);
                int maxTileX = (int)(Projectile.Center.X / 16f + explosionRadius);
                int minTileY = (int)(Projectile.Center.Y / 16f - explosionRadius);
                int maxTileY = (int)(Projectile.Center.Y / 16f + explosionRadius);

                // Ensure that all tile coordinates are within the world bounds
                // 确保所有的方块坐标都在世界范围内
                Utils.ClampWithinWorld(ref minTileX, ref minTileY, ref maxTileX, ref maxTileY);

                // These 2 methods handle actually mining the tiles and walls while honoring tile explosion conditions
                // 这两个方法处理实际挖掘方块和墙壁，并遵守方块爆炸条件
                bool explodeWalls = Projectile.ShouldWallExplode(Projectile.Center, explosionRadius, minTileX, maxTileX, minTileY, maxTileY);
                Projectile.ExplodeTiles(Projectile.Center, explosionRadius, minTileX, maxTileX, minTileY, maxTileY, explodeWalls);
            }
        }
    }
}
