using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace AnalysisMod.AnalysisContent.Projectiles
{
    public class AnalysisPaperAirplaneProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 10; // The width of the projectile
            Projectile.height = 10; // The height of the projectile

            Projectile.aiStyle = -1; // We are setting the aiStyle to -1 to use the custom AI below. If just want the vanilla behavior, you can set the aiStyle to 159.
                                     // 我们将aiStyle设置为-1以使用下面的自定义AI。如果只想要香草行为，可以将aiStyle设置为159。

            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.DamageType = DamageClass.Ranged; // Set the damage type to ranged damage.

            // Setting this to true will stop the projectile from automatically flipping its sprite when changing directions.
            // The vanilla paper airplanes have this set to true.
            // If this is true the projectile won't flip its sprite vertically while doing a loop, but the paper airplane can be upside down if it is shot one direction and then turns around on its own.
            // Set to false if you want the projectile to always be right side up.

            // 将此设置为true将阻止弹丸在改变方向时自动翻转其精灵图。
            // 香草纸飞机已将其设置为true。
            // 如果这是真的，则弹丸不会在执行循环时垂直翻转其精灵图，但如果它朝一个方向射击然后自己掉头，则纸飞机可能颠倒。
            // 如果您希望弹丸始终正面朝上，请将其设置为false。
            Projectile.manualDirectionChange = true;

            // If you are using Projectile.aiStyle = 159, setting the AIType isn't necessary here because the two types of vanilla paper airplanes aiStyles have the same AI.
            // 如果您正在使用Projectile.aiStyle = 159，则在此处设置AIType并不必要，因为两种类型的香草纸飞机aiStyles具有相同的AI。

            // AIType = ProjectileID.PaperAirplaneA;
        }

        // This is the behavior of the paper airplane.
        // If you just want the same vanilla behavior, you can instead set Projectile.aiStyle = 159 in SetDefaults and remove this AI() section.

        // 这是纸飞机的行为方式。
        // 如果您只想要相同的香草行为，则可以在SetDefaults中代替将Projectile.aiStyle = 159，并删除此AI（）部分。
        public override void AI()
        {
            // All projectiles have timers that help to delay certain events
            // 所有投射物都有延迟某些事件所需的计时器

            // Projectile.ai[0], Projectile.ai[1] — timers that are automatically synchronized on the client and server

            // This will run only once as soon as the projectile spawns.
            // 这仅会运行一次，就像弹丸生成一样。
            if (Projectile.ai[1] == 0f)
            {
                Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt(); // If it is moving right, then set Projectile.direction to 1. If it is moving left, then set Projectile.direction to -1.
                                                                                     // 如果它向右移动，则将Projectile.direction设​​置为1。如果它向左移动，则将Projectile.direction设​​置为-1。

                Projectile.rotation = Projectile.velocity.ToRotation(); // Set the rotation based on the velocity.
                                                                        // 根据速度进行旋转。

                Projectile.ai[1] = 1f; // Set Projectile.ai[1] to 1. This is only used to make this section of code run only once.
                                       // 将Projectile.ai [1]设置​​为1。这仅用于使此代码部分仅运行一次。

                Projectile.ai[0] = -Main.rand.Next(30, 80); // Set Projectile.ai[0] to a random number from -30 to -79.
                                                            // 将Projectile.ai [0]设置​​为从-30到-79的随机数字。

                Projectile.netUpdate = true; // Sync the projectile in a multiplayer game.
                                             // 在多人游戏中同步弹丸。
            }

            // Kill the projectile if it touches a liquid. (It will automatically get killed by touching a tile. You can change that by returning false in OnTileCollide())
            // 如果它接触液体，则杀死弹丸。 （它会自动被接触瓷砖杀死。您可以通过在OnTileCollide（）中返回false来更改这一点）
            if (Projectile.wet && Projectile.owner == Main.myPlayer)
            {
                Projectile.Kill();
            }

            Projectile.ai[0] += 1f; // Increase Projectile.ai[0] by 1 every tick. Remember, there are 60 ticks per second.
                                    // 每个tick都将Projectile.ai [0]增加1。请记住，每秒有60个ticks。

            Vector2 rotationVector = Projectile.rotation.ToRotationVector2() * 8f; // Get the rotation of the projectile.
                                                                                   // 获取弹丸的旋转角度。

            float ySinModifier = (float)Math.Sin((float)Math.PI * 2f * (float)(Main.timeForVisualEffects % 90.0 / 90.0)) * Projectile.direction * Main.WindForVisuals; // This will make the projectile fly in a sine wave fashion.
                                                                                                                                                                       // 这将使弹丸以正弦波方式飞行。

            Vector2 newVelocity = rotationVector + new Vector2(Main.WindForVisuals, ySinModifier); // Create a new velocity using the rotation and wind.
                                                                                                   // 使用旋转和风创建新速度。

            bool directionSameAsWind = Projectile.direction == Math.Sign(Main.WindForVisuals) && Projectile.velocity.Length() > 3f; // True if the projectile is moving the same direction as the wind and is not moving slowly.
                                                                                                                                    // 如果投射物与风向相同并且不慢动作移动，则为True。

            bool readyForFlip = Projectile.ai[0] >= 20f && Projectile.ai[0] <= 69f; // True if Projectile.ai[0] is between 20 and 69
                                                                                    // 如果Projectile.ai [0]在20到69之间，则为True


            // Once Projectile.ai[0] reaches 70...
            // Projectile.ai [0]达到70后...
            if (Projectile.ai[0] == 70f)
            {
                Projectile.ai[0] = -Main.rand.Next(120, 600); // Set it back to a random number from -120 to -599.
                                                              // 将其设置回从-120到-599的随机数字。
            }

            // Do a flip! This will cause the projectile to fly in a loop if directionSameAsWind and readyForFlip are true.
            // 翻转！ 如果directionSameAsWind和readyForFlip为true，则会导致弹丸飞行循环。
            if (readyForFlip && directionSameAsWind)
            {
                float lerpValue = Utils.GetLerpValue(0f, 30f, Projectile.ai[0], clamped: true);
                newVelocity = rotationVector.RotatedBy(-Projectile.direction * ((float)Math.PI * 2f) * 0.02f * lerpValue);
            }

            Projectile.velocity = newVelocity.SafeNormalize(Vector2.UnitY) * Projectile.velocity.Length(); // Set the velocity to the value we calculated above.
                                                                                                           // 将速度设置为我们上面计算的值。

            // If it is flying normally. i.e. not flying a loop.
            // 如果正常飞行。 即不要飞一个循环。
            if (!(readyForFlip && directionSameAsWind))
            {
                float yModifier = MathHelper.Lerp(0.15f, 0.05f, Math.Abs(Main.WindForVisuals));

                // Half of time, decrease the y velocity a little.
                // 一半时间，略微降低y速度。
                if (Projectile.timeLeft % 40 < 20)
                {
                    Projectile.velocity.Y -= yModifier;
                }
                // The other half of time, increase the y velocity a little.
                // 另一半时间，略微增加y速度。
                else
                {
                    Projectile.velocity.Y += yModifier;
                }

                // Cap the y velocity so the projectile falls slowly and doesn't rise too quickly.
                // MathHelper.Clamp() allows you to set a minimum and maximum value. In this case, the result will always be between -2f and 2f (inclusive).

                // 限制y速度，使弹丸缓慢下落并且不会过快上升。
                // MathHelper.Clamp（）允许您设置最小值和最大值。 在这种情况下，结果始终在-2f和2f之间（包括边界）。
                Projectile.velocity.Y = MathHelper.Clamp(Projectile.velocity.Y, -2f, 2f);

                // Set the x velocity.
                // MathHelper.Clamp() allows you to set a minimum and maximum value. In this case, the result will always be between -6f and 6f (inclusive).

                // 设置x速度。
                // MathHelper.Clamp（）允许您设置最小值和最大值。 在这种情况下，结果始终在-6f和6f之间（包括边界）。
                Projectile.velocity.X = MathHelper.Clamp(Projectile.velocity.X + Main.WindForVisuals * 0.006f, -6f, 6f);

                // Switch direction when the current velocity and the oldVelocity have different signs.
                // 当当前速度和oldVelocity具有不同符号时切换方向。
                if (Projectile.velocity.X * Projectile.oldVelocity.X < 0f)
                {
                    Projectile.direction *= -1; // Reverse the direction
                                                // 反转方向

                    Projectile.ai[0] = -Main.rand.Next(120, 300); // Set Projectile.ai[0] to a random number from -120 to -599.
                                                                  // 将Projectile.ai [0]设为从-120到-599的随机数字。

                    Projectile.netUpdate = true; // Sync the projectile in a multiplayer game.
                                                 // 在多人游戏中同步弹丸状态.
                }
            }

            // Set the rotation and spriteDirection
            // 设置旋转角度和spriteDirection
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.spriteDirection = Projectile.direction;

            // Let's add some dust for special effect. In this case, it runs every other tick (30 ticks per second).
            // 让我们添加一些特效灰尘。 在这种情况下，它每隔一个刻度运行一次（每秒30个刻度）。
            if (Projectile.timeLeft % 2 == 0)
            {
                Dust.NewDustPerfect(new Vector2(Projectile.Center.X - Projectile.width * Projectile.direction, Projectile.Center.Y), ModContent.DustType<Dusts.Sparkle>(), null, 0, default, 0.5f); //Here we spawn the dust at the back of the projectile with half scale.
            }
        }

        // We need to draw the projectile manually. If you don't include this, the projectile will be facing the wrong direction when flying left.
        // 我们需要手动绘制弹丸。 如果不包括此内容，则在向左飞行时，弹丸将面朝错误的方向。
        public override bool PreDraw(ref Color lightColor)
        {
            // This is where we specify which way to flip the sprite. If the projectile is moving to the left, then flip it vertically.
            // 这里我们指定翻转精灵图的方向。如果弹射物正在向左移动，则垂直翻转它。
            SpriteEffects spriteEffects = Projectile.spriteDirection <= 0 ? SpriteEffects.FlipVertically : SpriteEffects.None;

            // Getting texture of projectile
            // 获取弹射物的纹理
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            // Get the currently selected frame on the texture.
            // 获取当前选择的纹理帧。
            Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Type], frameY: Projectile.frame);

            Vector2 origin = sourceRectangle.Size() / 2f;

            // Applying lighting and draw our projectile
            // 应用光照并绘制我们的弹射物
            Color drawColor = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            // It's important to return false, otherwise we also draw the original texture.
            // 重要提示：必须返回false，否则还会绘制原始纹理。
            return false;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position); // Play a sound when the projectile dies. In this case, that is when it hits a block or a liquid.
                                                                        // 当弹丸死亡时播放声音。在这种情况下，即它撞击到方块或液体时。

            if (Projectile.owner == Main.myPlayer && !Projectile.noDropItem)
            {
                int dropItemType = ModContent.ItemType<Items.AnalysisPaperAirplane>(); // This the item we want the paper airplane to drop.
                                                                                       // 这是我们希望纸飞机掉落的物品。

                int newItem = Item.NewItem(Projectile.GetSource_DropAsItem(), Projectile.Hitbox, dropItemType); // Create a new item in the world.
                                                                                                                // 在世界中创建一个新物品。

                Main.item[newItem].noGrabDelay = 0; // Set the new item to be able to be picked up instantly
                                                    // 将新物品设置为可以立即拾取

                // Here we need to make sure the item is synced in multiplayer games.
                // 在多人游戏中需要确保该物品同步。
                if (Main.netMode == NetmodeID.MultiplayerClient && newItem >= 0)
                {
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);
                }
            }

            // Let's add some dust for special effect.
            // 让我们添加一些特效灰尘。
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, ModContent.DustType<Dusts.Sparkle>());
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.type == NPCID.GraniteGolem)
            {
                // Paper beats Rock!
                // Use FinalDamage since the projectile isn't conceptually stronger, the target is weaker to this weapon.

                // 石头被纸打败了！
                // 使用FinalDamage，因为弹丸并不是概念上更强大，而是目标对此武器更脆弱。
                modifiers.FinalDamage *= 20f; // 20x damage...isn't much since defense is high and damage is low.
                                              // 20倍伤害...由于防御力高而伤害低，并不算太多。
            }
        }
    }
}
