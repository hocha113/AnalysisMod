using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Pets.AnalysisLightPet
{
    public class AnalysisLightPetProjectile : ModProjectile
    {
        private const int DashCooldown = 1000; // How frequently this pet will dash at enemies.
                                               // 宠物会多频繁地向敌人冲刺。

        private const float DashSpeed = 20f; // The speed with which this pet will dash at enemies.
                                             // 宠物向敌人冲刺的速度。

        private const int FadeInTicks = 30;
        private const int FullBrightTicks = 200;
        private const int FadeOutTicks = 30;
        private const float Range = 500f;

        private static readonly float RangeHypoteneus = (float)(Math.Sqrt(2.0) * Range); // This comes from the formula for calculating the diagonal of a square (a * √2)
                                                                                         // 这来自于计算正方形对角线的公式（a * √2）。

        private static readonly float RangeHypoteneusSquared = RangeHypoteneus * RangeHypoteneus;

        // The following 2 lines of code are ref properties (learn about them in google) to the Projectile.ai array entries, which will help us make our code way more readable.
        // We're using the ai array because it's automatically synchronized by the base game in multiplayer, which saves us from writing a lot of boilerplate code.
        // Note that the Projectile.ai array is only 3 entries big. If you need more than 3 synchronized variables - you'll have to use fields and sync them manually.

        // 以下两行代码是 ref 属性（在 Google 中了解），用于 Projectile.ai 数组条目，这将帮助我们使我们的代码更易读。
        // 我们使用 ai 数组，因为它在多人游戏中由基础游戏自动同步，这样可以节省我们编写大量样板代码。
        // 请注意，Projectile.ai 数组仅有3个条目。如果您需要超过3个同步变量-您必须手动使用字段进行同步。
        public ref float AIFadeProgress => ref Projectile.ai[0];
        public ref float AIDashCharge => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.LightPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.timeLeft *= 5;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.scale = 0.8f;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            // If the player is no longer active (online) - deactivate (remove) the projectile.
            // 如果玩家不再活跃（在线）-停用（删除）弹丸。
            if (!player.active)
            {
                Projectile.active = false;
                return;
            }

            // Keep the projectile disappearing as long as the player isn't dead and has the pet buff.
            // 只要玩家没有死亡并拥有宠物增益效果，就保持弹丸消失状态。
            if (!player.dead && player.HasBuff(ModContent.BuffType<AnalysisLightPetBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            UpdateDash(player);
            UpdateFading(player);
            UpdateExtraMovement();

            // Rotates the pet when it moves horizontally.
            // 当水平移动时旋转宠物。
            Projectile.rotation += Projectile.velocity.X / 20f;

            // Lights up area around it.
            // 点亮周围区域。
            if (!Main.dedServ)
            {
                Lighting.AddLight(Projectile.Center, Projectile.Opacity * 0.9f, Projectile.Opacity * 0.1f, Projectile.Opacity * 0.3f);
            }
        }

        private void UpdateDash(Player player)
        {
            // The following code makes our pet dash at enemies when certain conditions are met
            // 下面的代码使我们的宠物在满足某些条件时向敌人冲刺

            AIDashCharge++;

            if (AIDashCharge <= DashCooldown || (int)AIFadeProgress % 100 != 0)
            {
                return;
            }

            // Enumerate
            // 枚举
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                var npc = Main.npc[i];

                // Ignore this npc if it's not active, or if it's friendly.
                // 如果此 NPC 不活跃或友好，则忽略该 NPC。
                if (!npc.active || npc.friendly)
                {
                    continue;
                }

                // Ignore this npc if it's too far away. Note that we're using squared values for our checks, to avoid square root calculations as a small, but effective optimization.
                // 如果 NPC 太远，则忽略该 NPC。请注意，在检查时我们使用平方值以避免开根号计算作为小但有效的优化措施。
                if (player.DistanceSQ(npc.Center) >= RangeHypoteneusSquared)
                {
                    continue;
                }

                Projectile.velocity += Vector2.Normalize(npc.Center - Projectile.Center) * DashSpeed; // Fling the projectile towards the npc.
                                                                                                      // 将弹丸向 NPC 抛出。
                AIDashCharge = 0f; // Reset the charge.
                                   // 重置充电。

                // Play a sound.
                // 播放声音。
                if (!Main.dedServ)
                {
                    SoundEngine.PlaySound(SoundID.Item42, Projectile.Center);
                }

                break;
            }
        }

        private void UpdateFading(Player player)
        {
            //TODO: Comment and clean this up more.
            // TODO：更多注释和清理此代码。

            var playerCenter = player.Center; // Cache the player's center vector to avoid recalculations.
                                              // 缓存玩家的中心向量以避免重新计算。

            AIFadeProgress++;

            if (AIFadeProgress < FadeInTicks)
            {
                Projectile.alpha = (int)(255 - 255 * AIFadeProgress / FadeInTicks);
            }
            else if (AIFadeProgress < FadeInTicks + FullBrightTicks)
            {
                Projectile.alpha = 0;

                if (Main.rand.NextBool(6))
                {
                    var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.PinkFairy, 0f, 0f, 200, default, 0.8f);

                    dust.velocity *= 0.3f;
                }
            }
            else if (AIFadeProgress < FadeInTicks + FullBrightTicks + FadeOutTicks)
            {
                Projectile.alpha = (int)(255 * (AIFadeProgress - FadeInTicks - FullBrightTicks) / FadeOutTicks);
            }
            else
            {
                Projectile.Center = playerCenter + Main.rand.NextVector2Circular(Range, Range);
                AIFadeProgress = 0f;

                Projectile.velocity = 2f * Vector2.Normalize(playerCenter - Projectile.Center);
            }

            if (Vector2.Distance(playerCenter, Projectile.Center) > RangeHypoteneus)
            {
                Projectile.Center = playerCenter + Main.rand.NextVector2Circular(Range, Range);
                AIFadeProgress = 0f;

                Projectile.velocity += 2f * Vector2.Normalize(playerCenter - Projectile.Center);
            }

            if ((int)AIFadeProgress % 100 == 0)
            {
                Projectile.velocity = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(90));
            }
        }

        private void UpdateExtraMovement()
        {
            // Adds some friction to the pet's movement as long as its speed is above 1
            // 只要速度高于1，就为宠物的移动添加一些摩擦力
            if (Projectile.velocity.Length() > 1f)
            {
                Projectile.velocity *= 0.98f;
            }

            // If the pet stops - launch it into a random direction at a low speed.
            // 如果宠物停止-以低速随机方向发射。
            if (Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = Vector2.UnitX.RotatedBy(Main.rand.NextFloat() * MathHelper.TwoPi) * 2f;
            }
        }
    }
}