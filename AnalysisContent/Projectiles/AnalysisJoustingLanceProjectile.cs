using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Projectiles
{
    public class AnalysisJoustingLanceProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // This will cause the player to dismount if they are hit by another Jousting Lance.
            // Since no enemies use Jousting Lances, this will only cause the player to dismount in PVP.

            // 如果玩家被另一支长矛击中，这将导致其卸下坐骑。
            // 由于没有敌人使用长矛，因此这只会在PVP中使玩家卸下坐骑。
            ProjectileID.Sets.DismountsPlayersOnHit[Type] = true;

            // This will make sure the velocity of the projectile will always be the shoot speed set in the item.
            // Since the velocity of the projectile affects how far out the jousting lance will spawn, we want the
            // velocity to always be the same even if the player has increased attack speed.

            // 这将确保弹射物的速度始终是道具设置的发射速度。
            // 由于弹射物的速度影响到长矛产生的距离，我们希望即使玩家增加了攻击速度，速度也始终相同。
            ProjectileID.Sets.NoMeleeSpeedVelocityScaling[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true; // Sync this projectile if a player joins mid game.
                                            // 如果有玩家在游戏进行时加入，请同步此弹射物。

            // The width and height do not affect the collision of the Jousting Lance because we calculate that separately (see Colliding() below)
            // 宽度和高度不影响长矛的碰撞，因为我们单独计算（见下面的Colliding()）
            Projectile.width = 25;
            Projectile.height = 25;

            // aiStyle 19 is the AI for Spears. Jousting Lances use the Spear AI. If you set the aiStyle to 19, make sure to set the AIType so it actually behaves like a Jousting Lance.
            // Since we are using custom AI below, we set the aiStyle to -1.

            // aiStyle 19 是标枪AI。 长矛使用标枪AI。 如果您将aiStyle设置为19，请确保设置AIType以使其实际上像一个长矛。
            // 由于我们在下面使用自定义AI，所以将aiStyle设置为-1。
            Projectile.aiStyle = -1;

            Projectile.alpha = 255; // The transparency of the projectile, 255 for completely transparent. Our projectile will fade in (see the AI() below).
                                    // 弹射物透明度为255表示完全透明。 我们的弹射物会淡入（请参见以下AI）。

            Projectile.friendly = true; // Player shot projectile. Does damage to enemies but not to friendly Town NPCs.
                                        // 玩家发出的弹射物。 对敌人造成伤害但不对友好城镇NPC造成伤害。

            Projectile.penetrate = -1; // Infinite penetration. The projectile can hit an infinite number of enemies.
                                       // 无限穿透力。 弹丸可以打中无限数量的敌人。

            Projectile.tileCollide = false; // Don't kill the projectile if it hits a tile.
                                            // 如果弹射物击中瓷砖，请不要将其杀死。

            Projectile.scale = 1f; // The scale of the projectile. This only effects the drawing and the width of the collision.
                                   // 弹射物的比例。 这仅影响绘图和碰撞宽度。

            Projectile.hide = true; // We are drawing the projectile ourself. See PreDraw() below.
                                    // 我们自己绘制弹射物。 请参见以下PreDraw()。

            Projectile.ownerHitCheck = true; // Make sure the owner of the projectile has line of sight to the target (aka can't hit things through tile).
                                             // 确保弹射物所有者对目标具有视线（即不能通过瓷砖击中东西）。

            Projectile.DamageType = DamageClass.MeleeNoSpeed; // Set the damage to melee damage.
                                                              // 将伤害设置为近战伤害。

            // Act like the normal Jousting Lance. Use this if you set the aiStyle to 19.
            // 表现得像普通长矛一样。 如果您将aiStyle设置为19，请使用此选项。

            // AIType = ProjectileID.JoustingLance; 
        }

        // This is the behavior of the Jousting Lances.
        // 这是长枪的行为方式。
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner]; // Get the owner of the projectile.
                                                          // 获取弹丸所有者。

            Projectile.direction = owner.direction; // Direction will be -1 when facing left and +1 when facing right.
                                                    // 当面向左时，方向为-1；当面向右时，方向为+1.

            owner.heldProj = Projectile.whoAmI; // Set the owner's held projectile to this projectile. heldProj is used so that the projectile will be killed when the player drops or swap items.
                                                // 将所有者手持的投掷武器设置为此投射物。 heldProj用于在玩家放下或交换道具时杀死该投射物

            int itemAnimationMax = owner.itemAnimationMax;
            // Remember, frames count down from itemAnimationMax to 0
            // Frame at which the lance is fully extended. Hold at this frame before retracting.
            // Scale factor (0.34f) means the last 34% of the animation will be used for retracting.

            // 请记住，帧从itemAnimationMax倒数到0
            // 长枪完全展开的帧数。 在缩回之前保持这个框架。
            // 比例因子（0.34f）表示最后34％的动画用于收缩
            int holdOutFrame = (int)(itemAnimationMax * 0.34f);
            if (owner.channel && owner.itemAnimation < holdOutFrame)
            {
                owner.SetDummyItemTime(holdOutFrame); // This makes it so the projectile never dies while we are holding it (except when we take damage, see AnalysisJoustingLancePlayer).
                                                      // 这使得在我们拿着它时弹丸永远不会消失（除非我们受到损伤，请参见AnalysisJoustingLancePlayer）。
            }

            // If the Jousting Lance is no longer being used, kill the projectile.
            // 如果不再使用长枪，则杀死弹丸。
            if (owner.ItemAnimationEndingOrEnded)
            {
                Projectile.Kill();
                return;
            }

            int itemAnimation = owner.itemAnimation;
            // extension and retraction factors (0-1). As the animation plays out, extension goes from 0-1 and stays at 1 while holding, then retraction goes from 0-1.
            // 扩展和收缩因子（0-1）。 随着动画的播放，扩展从0到1，并在持有时保持为1，然后收缩从0到1。
            float extension = 1 - Math.Max(itemAnimation - holdOutFrame, 0) / (float)(itemAnimationMax - holdOutFrame);
            float retraction = 1 - Math.Min(itemAnimation, holdOutFrame) / (float)holdOutFrame;

            // Distances are in pixels
            // 距离以像素为单位
            float extendDist = 24; // How far to fly out during extension
                                   // 延伸期间飞行多远

            float retractDist = extendDist / 2; // How far to fly back during retraction
                                                // 回缩期间飞行多远

            float tipDist = 98 + extension * extendDist - retraction * retractDist; // If your Jousting Lance is larger or smaller than the standard size, it is recommended to change the shoot speed of the item instead of this value.
                                                                                    // 如果您的长矛比标准尺寸大或小，则建议更改道具的发射速度而不是此值。

            Vector2 center = owner.RotatedRelativePoint(owner.MountedCenter); // Get the center of the owner. This accounts for the player being shifted up or down while riding a mount, sitting in a chair, etc.
                                                                              // 获取所有者中心。 这考虑了玩家骑乘坐骑，坐在椅子上等情况下向上或向下移位。

            Projectile.Center = center; // Set the center of the projectile to the center of the owner. Projectile.Center is now actually the tip of the Jousting Lance.
                                        // 将弹丸中心设置为所有者中心。 Projectile.Center现在实际上是长矛的顶端。

            Projectile.position += Projectile.velocity * tipDist; // The projectile velocity contains the orientation of the lance, multiply it by the tipDist to position the tip.
                                                                  // 弹丸速度包含长矛的方向，将其乘以tipDist来定位尖端。

            // Set the rotation of the projectile.
            // For reference, 0 is the top left, 180 degrees or pi radians is the bottom right.

            // 设置弹丸的旋转。
            // 参考：0为左上角，180度或π弧度为右下角。
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + (float)Math.PI * 3 / 4f;

            // Fade the projectile in when it first spawns
            // 当它第一次生成时淡入弹丸
            Projectile.alpha -= 40;
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }

            // The Hallowed and Shadow Jousting Lance spawn dusts when the player is moving above a certain speed.
            // 神圣和暗影枪战长矛在玩家移动到一定速度以上时会产生灰尘。
            float minimumDustVelocity = 6f;

            // This Vector2.Dot is the dot product between the projectile's velocity and the player's velocity normalized to be between -1 and 1.
            // What this means in this context is that the speed value will be closer to positive 1 if the player is moving in the same direction as the direction the lance was shot.
            // Analysis: if the lance is shot up and to the right, the value here will be closer to 1 if the player is also moving up and to the right.

            // 这个Vector2.Dot是弹丸速度与玩家速度之间点积，并归一化到-1至1之间。
            // 在这种情况下，意味着如果玩家朝着标枪发射方向移动，则速度值将更接近于正1。
            // 分析：如果标枪向上和向右发射，则此处的值将更接近于1，如果玩家也朝上和向右移动，则此处的值将更接近于1。
            float movementInLanceDirection = Vector2.Dot(Projectile.velocity.SafeNormalize(Vector2.UnitX * owner.direction), owner.velocity.SafeNormalize(Vector2.UnitX * owner.direction));

            float playerVelocity = owner.velocity.Length();

            if (playerVelocity > minimumDustVelocity && movementInLanceDirection > 0.8f)
            {
                // The chance for the dust to spawn. The actual chance (see below) is 1/dustChance. We make the chance higher the faster the player is moving by making the denominator smaller.
                // 产生灰尘的机会。实际机会（见下文）是1/dustChance。我们通过使分母变小来增加玩家移动得越快的机会。
                int dustChance = 8;
                if (playerVelocity > minimumDustVelocity + 1f)
                {
                    dustChance = 5;
                }
                if (playerVelocity > minimumDustVelocity + 2f)
                {
                    dustChance = 2;
                }

                // Set your dust types here.
                // 在这里设置您的灰尘类型。
                int dustTypeCommon = ModContent.DustType<Dusts.Sparkle>();
                int dustTypeRare = DustID.WhiteTorch;

                int offset = 4; // This offset will affect how much the dust spreads out.
                                // 该偏移量将影响灰尘扩散程度。

                // Spawn the dusts based on the dustChance. The dusts are spawned at the tip of the Jousting Lance.
                // 根据dustChance生成粉尘。粉末在Jousting Lance顶部生成。
                if (Main.rand.NextBool(dustChance))
                {
                    int newDust = Dust.NewDust(Projectile.Center - new Vector2(offset, offset), offset * 2, offset * 2, dustTypeCommon, Projectile.velocity.X * 0.2f + Projectile.direction * 3, Projectile.velocity.Y * 0.2f, 100, default, 1.2f);
                    Main.dust[newDust].noGravity = true;
                    Main.dust[newDust].velocity *= 0.25f;
                    newDust = Dust.NewDust(Projectile.Center - new Vector2(offset, offset), offset * 2, offset * 2, dustTypeCommon, 0f, 0f, 150, default, 1.4f);
                    Main.dust[newDust].velocity *= 0.25f;
                }

                if (Main.rand.NextBool(dustChance + 3))
                {
                    Dust.NewDust(Projectile.Center - new Vector2(offset, offset), offset * 2, offset * 2, dustTypeRare, 0f, 0f, 150, default, 1.4f);
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // This will increase or decrease the knockback of the Jousting Lance depending on how fast the player is moving.
            // 这将根据玩家移动速度的快慢增加或减少Jousting Lance的击退。
            modifiers.Knockback *= Main.player[Projectile.owner].velocity.Length() / 7f;

            // This will increase or decrease the damage of the Jousting Lance depending on how fast the player is moving.
            // 这将根据玩家移动速度的快慢增加或减少Jousting Lance的伤害。
            modifiers.SourceDamage *= 0.1f + Main.player[Projectile.owner].velocity.Length() / 7f * 0.9f;
        }

        // This is the custom collision that Jousting Lances uses. 
        // 这是Jousting Lances使用的自定义碰撞。
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float rotationFactor = Projectile.rotation + (float)Math.PI / 4f; // The rotation of the Jousting Lance.
                                                                              // 标枪旋转角度。

            float scaleFactor = 95f; // How far back the hit-line will be from the tip of the Jousting Lance. You will need to modify this if you have a longer or shorter Jousting Lance. Vanilla uses 95f
                                     // 从Jousting Lance尖端开始，打击线会向后延伸多远。如果您有更长或更短的Jousting Lance，则需要修改此值。Vanilla使用95f

            float widthMultiplier = 23f; // How thick the hit-line is. Increase or decrease this value if your Jousting Lance is thicker or thinner. Vanilla uses 23f
                                         // 打击线有多粗。如果您的标枪较厚或较薄，请增加或减小此值。Vanilla使用23f

            float collisionPoint = 0f; // collisionPoint is needed for CheckAABBvLineCollision(), but it isn't used for our collision here. Keep it at 0f.
                                       // CheckAABBvLineCollision()需要collisionPoint，但在这里不会用到它。将其保留为0f。

            // This Rectangle is the width and height of the Jousting Lance's hitbox which is used for the first step of collision.
            // You will need to modify the last two numbers if you have a bigger or smaller Jousting Lance.
            // Vanilla uses (0, 0, 300, 300) which that is quite large for the size of the Jousting Lance.
            // The size doesn't matter too much because this rectangle is only a basic check for the collision (the hit-line is much more important).

            // 这个矩形是比武长枪碰撞检测的宽度和高度，用于第一步的碰撞检测。
            // 如果你有一个更大或更小的比武长枪，则需要修改最后两个数字。
            // 原版使用 (0, 0, 300, 300)，对于比武长枪来说相当大。
            // 碰撞检测只是基本检查（命中线路更重要），所以大小并不太重要。
            Rectangle lanceHitboxBounds = new Rectangle(0, 0, 300, 300);

            // Set the position of the large rectangle.
            // 设置大矩形的位置。
            lanceHitboxBounds.X = (int)Projectile.position.X - lanceHitboxBounds.Width / 2;
            lanceHitboxBounds.Y = (int)Projectile.position.Y - lanceHitboxBounds.Height / 2;

            // This is the back of the hit-line with Projectile.Center being the tip of the Jousting Lance.
            // 这是命中线路的背面，Projectile.Center 是比武长枪尖端。
            Vector2 hitLineEnd = Projectile.Center + rotationFactor.ToRotationVector2() * scaleFactor;

            // The following is for debugging the size of the hit line. This will allow you to easily see where it starts and ends.
            // 下面代码用于调试命中线路大小。这将使您轻松地看到其起点和终点。

            // Dust.NewDustPerfect(Projectile.Center, DustID.Pixie, Velocity: Vector2.Zero, Scale: 0.5f);
            // Dust.NewDustPerfect(hitLineEnd, DustID.Pixie, Velocity: Vector2.Zero, Scale: 0.5f);

            // First check that our large rectangle intersects with the target hitbox.
            // Then we check to see if a line from the tip of the Jousting Lance to the "end" of the lance intersects with the target hitbox.

            // 首先检查我们的大矩形是否与目标碰撞框相交。
            // 然后我们检查从比武长枪尖端到“末端”的直线是否与目标碰撞框相交。
            if (lanceHitboxBounds.Intersects(targetHitbox)
                && Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, hitLineEnd, widthMultiplier * Projectile.scale, ref collisionPoint))
            {
                return true;
            }
            return false;
        }

        // We need to draw the projectile manually. If you don't include this, the Jousting Lance will not be aligned with the player.
        // 我们需要手动绘制弹幕。如果不包括此内容，则会导致比武长枪与玩家不对齐。
        public override bool PreDraw(ref Color lightColor)
        {

            // SpriteEffects change which direction the sprite is drawn.
            // SpriteEffects 更改精灵绘制方向。
            SpriteEffects spriteEffects = SpriteEffects.None;

            // Get texture of projectile.
            // 获取弹幕纹理
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            // Get the currently selected frame on the texture.
            // 获取纹理上当前选择帧
            Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Type], frameY: Projectile.frame);

            // The origin in this case is (0, 0) of our projectile because Projectile.Center is the tip of our Jousting Lance.
            // 在这种情况下，原点为我们弹幕的(0, 0)，因为 Projectile.Center 是我们比武长枪的尖端。
            Vector2 origin = Vector2.Zero;

            // The rotation of the projectile.
            // 弹幕旋转
            float rotation = Projectile.rotation;

            // If the projectile is facing right, we need to rotate it by -90 degrees, move the origin, and flip the sprite horizontally.
            // This will make it so the bottom of the sprite is correctly facing down when shot to the right.

            // 如果弹幕面向右，则需要将其旋转-90度，移动原点并水平翻转精灵。
            // 这样可以使精灵底部在向右发射时正确朝下。
            if (Projectile.direction > 0)
            {
                rotation -= (float)Math.PI / 2f;
                origin.X += sourceRectangle.Width;
                spriteEffects = SpriteEffects.FlipHorizontally;
            }

            // The position of the sprite. Not subtracting Main.player[Projectile.owner].gfxOffY will cause the sprite to bounce when walking up blocks.
            // 精灵位置。不减去 Main.player[Projectile.owner].gfxOffY 将导致走上方块时精灵反弹。
            Vector2 position = new(Projectile.Center.X, Projectile.Center.Y - Main.player[Projectile.owner].gfxOffY);

            // Apply lighting and draw our projectile
            // 应用光照并绘制我们的弹幕
            Color drawColor = Projectile.GetAlpha(lightColor);

            Main.EntitySpriteDraw(texture,
                position - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, rotation, origin, Projectile.scale, spriteEffects, 0);

            // The following is for debugging the size of the collision rectangle. Set this to the same size as the one you have in Colliding().
            // 下面代码用于调试碰撞矩形大小。将其设置为与 Colliding() 中相同的大小。

            // Rectangle lanceHitboxBounds = new Rectangle(0, 0, 300, 300);
            // Main.EntitySpriteDraw(TextureAssets.MagicPixel.Value,
            // 	new Vector2((int)Projectile.Center.X - lanceHitboxBounds.Width / 2, (int)Projectile.Center.Y - lanceHitboxBounds.Height / 2) - Main.screenPosition,
            // 	lanceHitboxBounds, Color.Orange * 0.5f, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);

            // It's important to return false, otherwise we also draw the original texture.
            // 重要：必须返回 false，否则还会绘制原始纹理。
            return false;
        }
    }
}