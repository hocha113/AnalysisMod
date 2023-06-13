using AnalysisMod.AnalysisContent.Dusts;
using AnalysisMod.AnalysisContent.Items.Weapons;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace AnalysisMod.AnalysisContent.Projectiles
{
    // This projectile showcases advanced AI code. Of particular note is a showcase on how projectiles can stick to NPCs in a manner similar to the behavior of vanilla weapons such as Bone Javelin, Daybreak, Blood Butcherer, Stardust Cell Minion, and Tentacle Spike. This code is modeled closely after Bone Javelin.
    // 这个弹射物展示了先进的AI代码。特别值得注意的是，它展示了弹射物如何像骨矛、黎明、血屠夫、星尘细胞仆从和触手钉等基本武器一样粘附在NPC上的行为方式。这段代码紧密地模仿了骨矛。
    public class AnalysisJavelinProjectile : ModProjectile
    {
        // These properties wrap the usual ai arrays for cleaner and easier to understand code.
        // Are we sticking to a target?

        // 这些属性包装了通常用于更清晰易懂的代码的ai数组。
        // 我们是否粘附到目标上？
        public bool IsStickingToTarget
        {
            get => Projectile.ai[0] == 1f;
            set => Projectile.ai[0] = value ? 1f : 0f;
        }

        // Index of the current target
        // 当前目标的索引
        public int TargetWhoAmI
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public int GravityDelayTimer
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }

        public float StickTimer
        {
            get => Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16; // The width of projectile hitbox
            Projectile.height = 16; // The height of projectile hitbox
            Projectile.aiStyle = 0; // The ai style of the projectile (0 means custom AI). For more please reference the source code of Terraria
                                    // 弹射物的AI风格（0表示自定义AI）。更多信息请参考Terraria源代码。

            Projectile.friendly = true; // Can the projectile deal damage to enemies?
                                        // 弹射物能否对敌人造成伤害？

            Projectile.hostile = false; // Can the projectile deal damage to the player?
                                        // 弹射物能否对玩家造成伤害？

            Projectile.DamageType = DamageClass.Ranged; // Makes the projectile deal ranged damage. You can set in to DamageClass.Throwing, but that is not used by any vanilla items
                                                        // 使弹射物造成远程伤害。你可以将其设置为DamageClass.Throwing，但没有任何原版道具使用该选项。

            Projectile.penetrate = 2; // How many monsters the projectile can penetrate.
                                      // 弹射物可以穿透多少敌人 。

            Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
                                       // 弹射物存在时间（60 = 1秒，因此600就是10秒）

            Projectile.alpha = 255; // The transparency of the projectile, 255 for completely transparent. Our custom AI below fades our projectile in. Make sure to delete this if you aren't usinga an aiStyle that fades in.
                                    // 弹射物透明度，255表示完全透明。我们下面自定义的AI会淡入我们的投影。如果您不使用淡入效果，请确保删除此内容。
            Projectile.light = 0.5f; // How much light emit around the projectile
                                     // 围绕着弹幕发出多少光亮

            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
                                           // 弹幕速度是否受水流影响？

            Projectile.tileCollide = true; // Can the projectile collide with tiles?
                                           // 弹射物是否可以与瓷砖碰撞？

            Projectile.hide = true; // Makes the projectile completely invisible. We need this to draw our projectile behind enemies/tiles in DrawBehind()
                                    // 使弹射物完全不可见。我们需要这个来在DrawBehind()中将投影绘制在敌人/瓷砖后面。
        }

        private const int GravityDelay = 45;

        public override void AI()
        {
            UpdateAlpha();
            // Run either the Sticky AI or Normal AI
            // Separating into different methods helps keeps your AI clean

            // 运行粘性AI或普通AI
            // 将其分成不同的方法有助于保持您的AI清洁
            if (IsStickingToTarget)
            {
                StickyAI();
            }
            else
            {
                NormalAI();
            }
        }

        private void NormalAI()
        {
            GravityDelayTimer++; // doesn't make sense.
                                 // 没有意义。

            // For a little while, the javelin will travel with the same speed, but after this, the javelin drops velocity very quickly.
            // 一段时间内，标枪将以相同的速度移动，但之后，标枪会非常快地降低速度。
            if (GravityDelayTimer >= GravityDelay)
            {
                GravityDelayTimer = GravityDelay;

                // wind resistance
                // 风阻力
                Projectile.velocity.X *= 0.98f;
                // gravity
                // 重力
                Projectile.velocity.Y += 0.35f;
            }

            // Offset the rotation by 90 degrees because the sprite is oriented vertiacally.
            // 将旋转偏移90度，因为精灵是垂直定向的。
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);

            // Spawn some random dusts as the javelin travels
            // 标枪行进时产生一些随机灰尘
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.height, Projectile.width, ModContent.DustType<Sparkle>(), Projectile.velocity.X * .2f, Projectile.velocity.Y * .2f, 200, Scale: 1.2f);
                dust.velocity += Projectile.velocity * 0.3f;
                dust.velocity *= 0.2f;
            }
            if (Main.rand.NextBool(4))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.height, Projectile.width, ModContent.DustType<Sparkle>(),
                    0, 0, 254, Scale: 0.3f);
                dust.velocity += Projectile.velocity * 0.5f;
                dust.velocity *= 0.5f;
            }
        }

        private const int StickTime = 60 * 15; // 15 seconds
        private void StickyAI()
        {
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            StickTimer += 1f;

            // Every 30 ticks, the javelin will perform a hit effect
            // 每30个tick，标枪都会执行一个打击效果
            bool hitEffect = StickTimer % 30f == 0f;
            int npcTarget = TargetWhoAmI;
            if (StickTimer >= StickTime || npcTarget < 0 || npcTarget >= 200)
            {   // If the index is past its limits, kill it
                // 如果索引超过了它的限制，请杀死它
                Projectile.Kill();
            }
            else if (Main.npc[npcTarget].active && !Main.npc[npcTarget].dontTakeDamage)
            {
                // If the target is active and can take damage
                // Set the projectile's position relative to the target's center

                // 如果目标处于活动状态并且能够受到伤害
                // 设置弹射物位置相对于目标中心
                Projectile.Center = Main.npc[npcTarget].Center - Projectile.velocity * 2f;
                Projectile.gfxOffY = Main.npc[npcTarget].gfxOffY;
                if (hitEffect)
                {
                    // Perform a hit effect here, causing the npc to react as if hit.
                    // 在此执行打击效果，导致NPC反应好像被击中了。

                    // Note that this does NOT damage the NPC, the damage is done through the debuff.
                    //请注意，这不会损坏NPC,伤害通过debuff完成。
                    Main.npc[npcTarget].HitEffect(0, 1.0);
                }
            }
            else
            {   // Otherwise, kill the projectile
                // 否则就杀死投影。
                Projectile.Kill();
            }
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position); // Play a death sound
                                                                     // 播放死亡声音

            Vector2 usePos = Projectile.position; // Position to use for dusts
                                                  // 用于dusts的位置使用Position

            // Offset the rotation by 90 degrees because the sprite is oriented vertiacally.
            // 将旋转偏移90度，因为精灵是垂直定向的。
            Vector2 rotationVector = (Projectile.rotation - MathHelper.ToRadians(90f)).ToRotationVector2(); // rotation vector to use for dust velocity
                                                                                                            // 用于dust速度的旋转向量
            usePos += rotationVector * 16f;

            // Spawn some dusts upon javelin death
            // 标枪死亡时产生一些灰尘
            for (int i = 0; i < 20; i++)
            {
                // Create a new dust
                // 创建一个新的dust
                Dust dust = Dust.NewDustDirect(usePos, Projectile.width, Projectile.height, DustID.Tin);
                dust.position = (dust.position + Projectile.Center) / 2f;
                dust.velocity += rotationVector * 2f;
                dust.velocity *= 0.5f;
                dust.noGravity = true;
                usePos -= rotationVector * 8f;
            }

            // Make sure to only spawn items if you are the projectile owner.
            // This is an important check as Kill() is called on clients, and you only want the item to drop once

            // 确保只有在您是弹射物所有者时才会生成物品。
            // 这是一个重要的检查，因为Kill()在客户端上调用，而您只希望该项一次掉落。
            if (Projectile.owner == Main.myPlayer)
            {
                // Drop a javelin item, 1 in 18 chance (~5.5% chance)
                // 丢下标枪道具，1/18机率（约5.5%机率）
                int item = 0;
                if (Main.rand.NextBool(18))
                {
                    item = Item.NewItem(Projectile.GetSource_DropAsItem(), Projectile.getRect(), ModContent.ItemType<AnalysisJavelin>());
                }

                // Sync the drop for multiplayer
                // Note the usage of Terraria.ID.MessageID, please use this!

                // 同步多人游戏中的掉落
                //请注意使用Terraria.ID.MessageID！
                if (Main.netMode == NetmodeID.MultiplayerClient && item >= 0)
                {
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, item, 1f);
                }
            }
        }

        private const int MaxStickingJavelin = 6; // This is the max amount of javelins able to be attached to a single NPC
                                                  // 这是单个NPC可以附着的最大标枪数量

        private readonly Point[] stickingJavelins = new Point[MaxStickingJavelin]; // The point array holding for sticking javelins
                                                                                   // 持有粘性标枪数组的点阵列

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            IsStickingToTarget = true; // we are sticking to a target
                                       // 我们是否粘附到目标上？

            TargetWhoAmI = target.whoAmI; // Set the target whoAmI
                                          // 设置目标whoAmI

            Projectile.velocity = (target.Center - Projectile.Center) *
                0.75f; // Change velocity based on delta center of targets (difference between entity centers)
                       // 根据目标实体中心的差异（即实体中心之间的差异）改变速度

            Projectile.netUpdate = true; // netUpdate this javelin
                                         // 更新此标枪的网络状态

            Projectile.damage = 0; // Makes sure the sticking javelins do not deal damage anymore
                                   // 确保粘在敌人身上的标枪不再造成伤害

            // AnalysisJavelinBuff handles the damage over time (DoT)
            // AnalysisJavelinBuff 处理持续伤害（DoT）
            target.AddBuff(ModContent.BuffType<Buffs.AnalysisJavelinDebuff>(), 900);

            // KillOldestJavelin will kill the oldest projectile stuck to the specified npc.
            // It only works if ai[0] is 1 when sticking and ai[1] is the target npc index, which is what IsStickingToTarget and TargetWhoAmI correspond to.

            // KillOldestJavelin 将杀死附着在指定 NPC 上最老的抛射物。
            // 它仅在附着时 ai[0] 为1，ai[1] 是目标 npc 索引时才起作用，这就是 IsStickingToTarget 和 TargetWhoAmI 对应的内容。
            Projectile.KillOldestJavelin(Projectile.whoAmI, Type, target.whoAmI, stickingJavelins);
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            // For going through platforms and such, javelins use a tad smaller size
            // 为了穿过平台等障碍物，标枪使用稍小一点的大小
            width = height = 10; // notice we set the width to the height, the height to 10. so both are 10
                                 // 请注意我们将宽度设置为高度，将高度设置为10。因此两者都是10。
            return true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // By shrinking target hitboxes by a small amount, this projectile only hits if it more directly hits the target.
            // This helps the javelin stick in a visually appealing place within the target sprite.

            // 缩小目标命中框一小部分，使得该抛射物只有更直接地击中目标时才会命中。
            // 这有助于让标枪停留在一个视觉上吸引人的位置，在目标精灵内部。
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
            {
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            }
            // Return if the hitboxes intersects, which means the javelin collides or not
            // 如果命中框相交，则返回 true 表示 javelin 发生碰撞；否则返回 false。
            return projHitbox.Intersects(targetHitbox);
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            // If attached to an NPC, draw behind tiles (and the npc) if that NPC is behind tiles, otherwise just behind the NPC.
            // 如果连接到 NPC，则如果该 NPC 在瓷砖后面，则绘制在瓷砖（和 npc）后面；否则仅绘制在 NPC 后面。
            if (IsStickingToTarget)
            {
                int npcIndex = TargetWhoAmI;
                if (npcIndex >= 0 && npcIndex < 200 && Main.npc[npcIndex].active)
                {
                    if (Main.npc[npcIndex].behindTiles)
                    {
                        behindNPCsAndTiles.Add(index);
                    }
                    else
                    {
                        behindNPCsAndTiles.Add(index);
                    }

                    return;
                }
            }
            // Since we aren't attached, add to this list
            // 由于我们没有连接，因此将其添加到此列表中
            behindNPCsAndTiles.Add(index);
        }

        // Change this number if you want to alter how the alpha changes
        // 如果要更改 alpha 的变化方式，请更改此数字
        private const int AlphaFadeInSpeed = 25;

        private void UpdateAlpha()
        {
            // Slowly remove alpha as it is present
            // 当存在时缓慢减少 alpha 值
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= AlphaFadeInSpeed;
            }

            // If alpha gets lower than 0, set it to 0
            // 如果 alpha 小于 0，则将其设置为 0
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }
        }
    }
}
