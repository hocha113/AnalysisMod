using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using AnalysisMod.AnalysisContent.Dusts;
using Terraria.GameContent;
using ReLogic.Content;

namespace AnalysisMod.AnalysisContent.Projectiles
{
    // Analysis Advanced Flail is a complete adaption of Ball O' Hurt projectile. The code has been rewritten a bit to make it easier to follow. Compare this code against the decompiled Terraria code for an Analysis of adapting vanilla code. A few comments and extra code snippets show features from other vanilla flails as well.
    // Analysis Advanced Flail shows a plethora of advanced AI and collision topics.
    // See AnalysisFlail for a simpler but less customizable flail projectile Analysis.

    // Analysis Advanced Flail是Ball O' Hurt弹射物的完整适应。代码已经进行了一些重写，使其更易于理解。将此代码与反编译的Terraria代码进行比较，以分析如何适应基础代码。一些注释和额外的代码片段展示了其他基础链球武器的特性。
    // Analysis Advanced Flail展示了大量高级AI和碰撞主题。
    // 有关简单但不太可定制化的链球弹射物分析，请参见AnalysisFlail。
    public class AnalysisAdvancedFlailProjectile : ModProjectile
    {
        private const string ChainTexturePath = "AnalysisMod/AnalysisContent/Projectiles/AnalysisAdvancedFlailProjectileChain"; // The folder path to the flail chain sprite
                                                                                                                                // 链条精灵文件夹路径

        private const string ChainTextureExtraPath = "AnalysisMod/AnalysisContent/Projectiles/AnalysisAdvancedFlailProjectileChainExtra";  // This texture and related code is optional and used for a unique effect
                                                                                                                                           // 这个纹理及相关代码是可选项，用于产生独特效果

        private enum AIState
        {
            Spinning,
            LaunchingForward,
            Retracting,
            UnusedState,
            ForcedRetracting,
            Ricochet,
            Dropping
        }

        // These properties wrap the usual ai and localAI arrays for cleaner and easier to understand code.
        // 这些属性包装常规ai和localAI数组，使得代码更加清晰易懂。
        private AIState CurrentAIState
        {
            get => (AIState)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        public ref float StateTimer => ref Projectile.ai[1];
        public ref float CollisionCounter => ref Projectile.localAI[0];
        public ref float SpinningStateTimer => ref Projectile.localAI[1];

        public override void SetStaticDefaults()
        {
            // These lines facilitate the trail drawing
            // 这些行方便拖尾绘制
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true; // This ensures that the projectile is synced when other players join the world.
                                            // 确保当其他玩家加入世界时该弹射物被同步。

            Projectile.width = 24; // The width of your projectile
                                   // 你的弹射物宽度

            Projectile.height = 24; // The height of your projectile
                                    // 你的弹射物高度

            Projectile.friendly = true; // Deals damage to enemies
                                        // 对敌人造成伤害

            Projectile.penetrate = -1; // Infinite pierce
                                       // 无限穿透力

            Projectile.DamageType = DamageClass.Melee; // Deals melee damage
                                                       // 近战伤害类型

            Projectile.usesLocalNPCImmunity = true; // Used for hit cooldown changes in the ai hook
                                                    // 用于ai钩子中打击冷却时间变化

            Projectile.localNPCHitCooldown = 10; // This facilitates custom hit cooldown logic
                                                 // 这有助于自定义打击冷却逻辑

            // Vanilla flails all use aiStyle 15, but the code isn't customizable so an adaption of that aiStyle is used in the AI method
            // 所有基础链球武器都使用aiStyle 15, 但该代码不可定制化，因此在AI方法中使用了一个改编版aiStyle
        }

        // This AI code was adapted from vanilla code: Terraria.Projectile.AI_015_Flails() 

        // 此AI代码是从基础代码中改编而来：Terraria.Projectile.AI_015_Flails()
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            // Kill the projectile if the player dies or gets crowd controlled
            // 如果玩家死亡或被控制，则杀死弹射物
            if (!player.active || player.dead || player.noItems || player.CCed || Vector2.Distance(Projectile.Center, player.Center) > 900f)
            {
                Projectile.Kill();
                return;
            }
            if (Main.myPlayer == Projectile.owner && Main.mapFullscreen)
            {
                Projectile.Kill();
                return;
            }

            Vector2 mountedCenter = player.MountedCenter;
            bool doFastThrowDust = false;
            bool shouldOwnerHitCheck = false;
            int launchTimeLimit = 15;  // How much time the projectile can go before retracting (speed and shootTimer will set the flail's range)
                                       // 弹射物在收回之前可以飞行的时间长度（速度和shootTimer将设置链球的范围）

            float launchSpeed = 14f; // How fast the projectile can move
                                     // 弹射物可以移动的速度

            float maxLaunchLength = 800f; // How far the projectile's chain can stretch before being forced to retract when in launched state
                                          // 发射状态下，链条能够伸展多远才会被迫缩回

            float retractAcceleration = 3f; // How quickly the projectile will accelerate back towards the player while retracting
                                            // 向玩家收回时，弹射物加速的快慢程度

            float maxRetractSpeed = 10f; // The max speed the projectile will have while retracting
                                         // 在收回时，弹射物最大速度

            float forcedRetractAcceleration = 6f; // How quickly the projectile will accelerate back towards the player while being forced to retract
                                                  // 当被迫缩回时，弹射物加速返回玩家的快慢程度

            float maxForcedRetractSpeed = 15f; // The max speed the projectile will have while being forced to retract
                                               // 在被迫缩回时，弹射物最大速度
            float unusedRetractAcceleration = 1f;
            float unusedMaxRetractSpeed = 14f;
            int unusedChainLength = 60;
            int defaultHitCooldown = 10; // How often your flail hits when resting on the ground, or retracting
                                         // 静止或正在收回状态下你的链球攻击频率

            int spinHitCooldown = 20; // How often your flail hits when spinning
                                      // 旋转状态下你的链球攻击频率

            int movingHitCooldown = 10; // How often your flail hits when moving
                                        // 移动状态下你的链球攻击频率

            int ricochetTimeLimit = launchTimeLimit + 5;

            // Scaling these speeds and accelerations by the players melee speed makes the weapon more responsive if the player boosts it or general weapon speed
            // 通过玩家近战攻击力调整这些速度和加速度使得该武器更具响应性。
            float meleeSpeedMultiplier = player.GetTotalAttackSpeed(DamageClass.Melee);
            launchSpeed *= meleeSpeedMultiplier;
            unusedRetractAcceleration *= meleeSpeedMultiplier;
            unusedMaxRetractSpeed *= meleeSpeedMultiplier;
            retractAcceleration *= meleeSpeedMultiplier;
            maxRetractSpeed *= meleeSpeedMultiplier;
            forcedRetractAcceleration *= meleeSpeedMultiplier;
            maxForcedRetractSpeed *= meleeSpeedMultiplier;
            float launchRange = launchSpeed * launchTimeLimit;
            float maxDroppedRange = launchRange + 160f;
            Projectile.localNPCHitCooldown = defaultHitCooldown;

            switch (CurrentAIState)
            {
                case AIState.Spinning:
                    {
                        shouldOwnerHitCheck = true;
                        if (Projectile.owner == Main.myPlayer)
                        {
                            Vector2 unitVectorTowardsMouse = mountedCenter.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.UnitX * player.direction);
                            player.ChangeDir((unitVectorTowardsMouse.X > 0f).ToDirectionInt());
                            if (!player.channel) // If the player releases then change to moving forward mode
                                                 // 如果玩家释放则切换到向前模式。
                            {
                                CurrentAIState = AIState.LaunchingForward;
                                StateTimer = 0f;
                                Projectile.velocity = unitVectorTowardsMouse * launchSpeed + player.velocity;
                                Projectile.Center = mountedCenter;
                                Projectile.netUpdate = true;
                                Projectile.ResetLocalNPCHitImmunity();
                                Projectile.localNPCHitCooldown = movingHitCooldown;
                                break;
                            }
                        }
                        SpinningStateTimer += 1f;
                        // This line creates a unit vector that is constantly rotated around the player. 10f controls how fast the projectile visually spins around the player
                        // 这一行创建了一个单位向量，在玩家周围不断旋转。10f控制了项目视觉上绕着玩家旋转的快慢程度。
                        Vector2 offsetFromPlayer = new Vector2(player.direction).RotatedBy((float)Math.PI * 10f * (SpinningStateTimer / 60f) * player.direction);

                        offsetFromPlayer.Y *= 0.8f;
                        if (offsetFromPlayer.Y * player.gravDir > 0f)
                        {
                            offsetFromPlayer.Y *= 0.5f;
                        }
                        Projectile.Center = mountedCenter + offsetFromPlayer * 30f + new Vector2(0, player.gfxOffY);
                        Projectile.velocity = Vector2.Zero;
                        Projectile.localNPCHitCooldown = spinHitCooldown; // set the hit speed to the spinning hit speed
                                                                          // 将打击速度设为旋转打击速度
                        break;
                    }
                case AIState.LaunchingForward:
                    {
                        doFastThrowDust = true;
                        bool shouldSwitchToRetracting = StateTimer++ >= launchTimeLimit;
                        shouldSwitchToRetracting |= Projectile.Distance(mountedCenter) >= maxLaunchLength;
                        if (player.controlUseItem) // If the player clicks, transition to the Dropping state
                                                   // 如果玩家点击，则转换到下落状态
                        {
                            CurrentAIState = AIState.Dropping;
                            StateTimer = 0f;
                            Projectile.netUpdate = true;
                            Projectile.velocity *= 0.2f;
                            // This is where Drippler Crippler spawns its projectile
                            // 这是Drippler Crippler生成其弹射物的位置。
                            /*
							if (Main.myPlayer == Projectile.owner)
								Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, Projectile.velocity, 928, Projectile.damage, Projectile.knockBack, Main.myPlayer);
							*/
                            break;
                        }
                        if (shouldSwitchToRetracting)
                        {
                            CurrentAIState = AIState.Retracting;
                            StateTimer = 0f;
                            Projectile.netUpdate = true;
                            Projectile.velocity *= 0.3f;
                            // This is also where Drippler Crippler spawns its projectile, see above code.
                            // 这也是Drippler Crippler生成其弹射物的位置，请参见上面的代码。
                        }
                        player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());
                        Projectile.localNPCHitCooldown = movingHitCooldown;
                        break;
                    }
                case AIState.Retracting:
                    {
                        Vector2 unitVectorTowardsPlayer = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);
                        if (Projectile.Distance(mountedCenter) <= maxRetractSpeed)
                        {
                            Projectile.Kill(); // Kill the projectile once it is close enough to the player
                                               // 当它靠近玩家时，杀死该弹射物。
                            return;
                        }
                        if (player.controlUseItem) // If the player clicks, transition to the Dropping state
                                                   // 如果玩家点击，则转换到下落状态
                        {
                            CurrentAIState = AIState.Dropping;
                            StateTimer = 0f;
                            Projectile.netUpdate = true;
                            Projectile.velocity *= 0.2f;
                        }
                        else
                        {
                            Projectile.velocity *= 0.98f;
                            Projectile.velocity = Projectile.velocity.MoveTowards(unitVectorTowardsPlayer * maxRetractSpeed, retractAcceleration);
                            player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());
                        }
                        break;
                    }
                case AIState.UnusedState: // Projectile.ai[0] == 3; This case is actually unused, but maybe a Terraria update will add it back in, or maybe it is useless, so I left it here.
                                          // Projectile.ai[0] == 3; 此情况实际上未使用，但可能会在Terraria更新中重新添加，或者它是无用的，因此我将其保留在这里。
                    {
                        if (!player.controlUseItem)
                        {
                            CurrentAIState = AIState.ForcedRetracting; // Move to super retracting mode if the player taps
                                                                       // 如果玩家轻敲则进入超级收缩模式
                            StateTimer = 0f;
                            Projectile.netUpdate = true;
                            break;
                        }
                        float currentChainLength = Projectile.Distance(mountedCenter);
                        Projectile.tileCollide = StateTimer == 1f;
                        bool flag3 = currentChainLength <= launchRange;
                        if (flag3 != Projectile.tileCollide)
                        {
                            Projectile.tileCollide = flag3;
                            StateTimer = Projectile.tileCollide ? 1 : 0;
                            Projectile.netUpdate = true;
                        }
                        if (currentChainLength > unusedChainLength)
                        {

                            if (currentChainLength >= launchRange)
                            {
                                Projectile.velocity *= 0.5f;
                                Projectile.velocity = Projectile.velocity.MoveTowards(Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero) * unusedMaxRetractSpeed, unusedMaxRetractSpeed);
                            }
                            Projectile.velocity *= 0.98f;
                            Projectile.velocity = Projectile.velocity.MoveTowards(Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero) * unusedMaxRetractSpeed, unusedRetractAcceleration);
                        }
                        else
                        {
                            if (Projectile.velocity.Length() < 6f)
                            {
                                Projectile.velocity.X *= 0.96f;
                                Projectile.velocity.Y += 0.2f;
                            }
                            if (player.velocity.X == 0f)
                            {
                                Projectile.velocity.X *= 0.96f;
                            }
                        }
                        player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());
                        break;
                    }
                case AIState.ForcedRetracting:
                    {
                        Projectile.tileCollide = false;
                        Vector2 unitVectorTowardsPlayer = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);
                        if (Projectile.Distance(mountedCenter) <= maxForcedRetractSpeed)
                        {
                            Projectile.Kill(); // Kill the projectile once it is close enough to the player
                                               // 当离玩家足够近时杀死它
                            return;
                        }
                        Projectile.velocity *= 0.98f;
                        Projectile.velocity = Projectile.velocity.MoveTowards(unitVectorTowardsPlayer * maxForcedRetractSpeed, forcedRetractAcceleration);
                        Vector2 target = Projectile.Center + Projectile.velocity;
                        Vector2 value = mountedCenter.DirectionFrom(target).SafeNormalize(Vector2.Zero);
                        if (Vector2.Dot(unitVectorTowardsPlayer, value) < 0f)
                        {
                            Projectile.Kill(); // Kill projectile if it will pass the player
                                               // 当它即将经过玩家时杀死该弹射物。
                            return;
                        }
                        player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());
                        break;
                    }
                case AIState.Ricochet:
                    if (StateTimer++ >= ricochetTimeLimit)
                    {
                        CurrentAIState = AIState.Dropping;
                        StateTimer = 0f;
                        Projectile.netUpdate = true;
                    }
                    else
                    {
                        Projectile.localNPCHitCooldown = movingHitCooldown;
                        Projectile.velocity.Y += 0.6f;
                        Projectile.velocity.X *= 0.95f;
                        player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());
                    }
                    break;
                case AIState.Dropping:
                    if (!player.controlUseItem || Projectile.Distance(mountedCenter) > maxDroppedRange)
                    {
                        CurrentAIState = AIState.ForcedRetracting;
                        StateTimer = 0f;
                        Projectile.netUpdate = true;
                    }
                    else
                    {
                        Projectile.velocity.Y += 0.8f;
                        Projectile.velocity.X *= 0.95f;
                        player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());
                    }
                    break;
            }

            // This is where Flower Pow launches projectiles. Decompile Terraria to view that code.
            // 这就是Flower Pow发射项目的地方。反编译Terraria以查看该代码。

            Projectile.direction = (Projectile.velocity.X > 0f).ToDirectionInt();
            Projectile.spriteDirection = Projectile.direction;
            Projectile.ownerHitCheck = shouldOwnerHitCheck; // This prevents attempting to damage enemies without line of sight to the player. The custom Colliding code for spinning makes this necessary.
                                                            // 这可以防止玩家在没有视线的情况下试图攻击敌人。旋转时自定义碰撞代码使其成为必要条件。

            // This rotation code is unique to this flail, since the sprite isn't rotationally symmetric and has tip.
            // 这个旋转代码是独特的，因为精灵不具有旋转对称性并且有尖端。
            bool freeRotation = CurrentAIState == AIState.Ricochet || CurrentAIState == AIState.Dropping;
            if (freeRotation)
            {
                if (Projectile.velocity.Length() > 1f)
                    Projectile.rotation = Projectile.velocity.ToRotation() + Projectile.velocity.X * 0.1f; // skid
                else
                    Projectile.rotation += Projectile.velocity.X * 0.1f; // roll
            }
            else
            {
                Vector2 vectorTowardsPlayer = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);
                Projectile.rotation = vectorTowardsPlayer.ToRotation() + MathHelper.PiOver2;
            }

            // If you have a ball shaped flail, you can use this simplified rotation code instead
            // 如果你有一个球形流星锤，你可以使用这个简化的旋转代码
            /*
			if (Projectile.velocity.Length() > 1f)
				Projectile.rotation = Projectile.velocity.ToRotation() + Projectile.velocity.X * 0.1f; // skid
			else
				Projectile.rotation += Projectile.velocity.X * 0.1f; // roll
			*/

            Projectile.timeLeft = 2; // Makes sure the flail doesn't die (good when the flail is resting on the ground)
                                     // 确保流星锤不会死亡（当流星锤静止在地面上时很好）

            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2); //Add a delay so the player can't button mash the flail
                                        //添加延迟以防止玩家连续按按钮

            player.itemRotation = Projectile.DirectionFrom(mountedCenter).ToRotation();
            if (Projectile.Center.X < mountedCenter.X)
            {
                player.itemRotation += (float)Math.PI;
            }
            player.itemRotation = MathHelper.WrapAngle(player.itemRotation);

            // Spawning dust. We spawn dust more often when in the LaunchingForward state
            // 产生尘土。我们在向前发射状态下更频繁地生成尘土
            int dustRate = 15;
            if (doFastThrowDust)
                dustRate = 1;

            if (Main.rand.NextBool(dustRate))
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Sparkle>(), 0f, 0f, 150, default, 1.3f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            int defaultLocalNPCHitCooldown = 10;
            int impactIntensity = 0;
            Vector2 velocity = Projectile.velocity;
            float bounceFactor = 0.2f;
            if (CurrentAIState == AIState.LaunchingForward || CurrentAIState == AIState.Ricochet)
            {
                bounceFactor = 0.4f;
            }

            if (CurrentAIState == AIState.Dropping)
            {
                bounceFactor = 0f;
            }

            if (oldVelocity.X != Projectile.velocity.X)
            {
                if (Math.Abs(oldVelocity.X) > 4f)
                {
                    impactIntensity = 1;
                }

                Projectile.velocity.X = (0f - oldVelocity.X) * bounceFactor;
                CollisionCounter += 1f;
            }

            if (oldVelocity.Y != Projectile.velocity.Y)
            {
                if (Math.Abs(oldVelocity.Y) > 4f)
                {
                    impactIntensity = 1;
                }

                Projectile.velocity.Y = (0f - oldVelocity.Y) * bounceFactor;
                CollisionCounter += 1f;
            }

            // If in the Launched state, spawn sparks
            // 如果处于已发射状态，则生成火花
            if (CurrentAIState == AIState.LaunchingForward)
            {
                CurrentAIState = AIState.Ricochet;
                Projectile.localNPCHitCooldown = defaultLocalNPCHitCooldown;
                Projectile.netUpdate = true;
                Point scanAreaStart = Projectile.TopLeft.ToTileCoordinates();
                Point scanAreaEnd = Projectile.BottomRight.ToTileCoordinates();
                impactIntensity = 2;
                Projectile.CreateImpactExplosion(2, Projectile.Center, ref scanAreaStart, ref scanAreaEnd, Projectile.width, out bool causedShockwaves);
                Projectile.CreateImpactExplosion2_FlailTileCollision(Projectile.Center, causedShockwaves, velocity);
                Projectile.position -= velocity;
            }

            // Here the tiles spawn dust indicating they've been hit
            // 在此处，瓷砖生成指示它们被打中的灰尘
            if (impactIntensity > 0)
            {
                Projectile.netUpdate = true;
                for (int i = 0; i < impactIntensity; i++)
                {
                    Collision.HitTiles(Projectile.position, velocity, Projectile.width, Projectile.height);
                }

                SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            }

            // Force retraction if stuck on tiles while retracting
            // 强制收缩如果卡在平铺物上而正在收缩
            if (CurrentAIState != AIState.UnusedState && CurrentAIState != AIState.Spinning && CurrentAIState != AIState.Ricochet && CurrentAIState != AIState.Dropping && CollisionCounter >= 10f)
            {
                CurrentAIState = AIState.ForcedRetracting;
                Projectile.netUpdate = true;
            }

            // tModLoader currently does not provide the wetVelocity parameter, this code should make the flail bounce back faster when colliding with tiles underwater.
            // tModLoader目前不提供wetVelocity参数，在水下与平铺物碰撞时应该使流星锤反弹得更快。
            //if (Projectile.wet)
            //	wetVelocity = Projectile.velocity;

            return false;
        }

        public override bool? CanDamage()
        {
            // Flails in spin mode won't damage enemies within the first 12 ticks. Visually this delays the first hit until the player swings the flail around for a full spin before damaging anything.
            // 旋转模式下的流星锤将在前12个tick内无法伤害敌人。从视觉上看，在玩家完全摆动流星锤之前延迟第一次命中任何东西。
            if (CurrentAIState == AIState.Spinning && SpinningStateTimer <= 12f)
            {
                return false;
            }
            return base.CanDamage();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // Flails do special collision logic that serves to hit anything within an ellipse centered on the player when the flail is spinning around the player. For Analysis, the projectile rotating around the player won't actually hit a bee if it is directly on the player usually, but this code ensures that the bee is hit. This code makes hitting enemies while spinning more consistant and not reliant of the actual position of the flail projectile.
            // 流星锤进行特殊碰撞逻辑，用于击中围绕着玩家旋转的流星锤周围椭圆形内的任何东西。对于分析，如果蜜蜂直接在玩家身上通常不会被击中，但是这段代码确保了蜜蜂被击中。此代码使得旋转时打击敌人更加一致，并且不依赖于流星锤弹道的实际位置。
            if (CurrentAIState == AIState.Spinning)
            {
                Vector2 mountedCenter = Main.player[Projectile.owner].MountedCenter;
                Vector2 shortestVectorFromPlayerToTarget = targetHitbox.ClosestPointInRect(mountedCenter) - mountedCenter;
                shortestVectorFromPlayerToTarget.Y /= 0.8f; // Makes the hit area an ellipse. Vertical hit distance is smaller due to this math.
                                                            // 使命中区域成为一个椭圆形。由于这个数学公式，垂直命中距离较小。

                float hitRadius = 55f; // The length of the semi-major radius of the ellipse (the long end)
                                       // 椭圆半长轴长度（长端）

                return shortestVectorFromPlayerToTarget.Length() <= hitRadius;
            }
            // Regular collision logic happens otherwise.
            // 否则发生常规碰撞逻辑。
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // Flails do a few custom things, you'll want to keep these to have the same feel as vanilla flails.
            // 流星锤有一些自定义功能，您需要保留它们以具有与香草流星锤相同的感觉。

            // Flails do 20% more damage while spinning
            // 旋转时流星锤造成20%额外伤害
            if (CurrentAIState == AIState.Spinning)
            {
                modifiers.SourceDamage *= 1.2f;
            }
            // Flails do 100% more damage while launched or retracting. This is the damage the item tooltip for flails aim to match, as this is the most common mode of attack. This is why the item has ItemID.Sets.ToolTipDamageMultiplier[Type] = 2f;
            // 发射或收缩时流星锤造成100%额外伤害。这是用来匹配物品工具提示所需达到的伤害值，因为这是最常见的攻击模式。这就是为什么该物品具有ItemID.Sets.ToolTipDamageMultiplier[Type] = 2f; 的原因。
            else if (CurrentAIState == AIState.LaunchingForward || CurrentAIState == AIState.Retracting)
            {
                modifiers.SourceDamage *= 2f;
            }

            // The hitDirection is always set to hit away from the player, even if the flail damages the npc while returning
            // 命中方向始终设置为远离玩家，即使在返回过程中也会损坏npc
            modifiers.HitDirectionOverride = (Main.player[Projectile.owner].Center.X < target.Center.X).ToDirectionInt();

            // Knockback is only 25% as powerful when in spin mode
            // 在旋转模式下反冲只有25%的威力
            if (CurrentAIState == AIState.Spinning)
            {
                modifiers.Knockback *= 0.25f;
            }
            // Knockback is only 50% as powerful when in drop down mode
            // 在下降模式下反冲只有50%的威力
            else if (CurrentAIState == AIState.Dropping)
            {
                modifiers.Knockback *= 0.5f;
            }
        }

        // PreDraw is used to draw a chain and trail before the projectile is drawn normally.
        // PreDraw用于在正常绘制弹丸之前绘制链和拖尾。
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 playerArmPosition = Main.GetPlayerArmPosition(Projectile);

            // This fixes a vanilla GetPlayerArmPosition bug causing the chain to draw incorrectly when stepping up slopes. The flail itself still draws incorrectly due to another similar bug. This should be removed once the vanilla bug is fixed.
            // 这修复了香草GetPlayerArmPosition错误，导致链在爬坡时绘制不正确。由于另一个类似的错误，流星锤本身仍然会绘制不正确。一旦香草bug被修复，应该将其删除。
            playerArmPosition.Y -= Main.player[Projectile.owner].gfxOffY;

            Asset<Texture2D> chainTexture = ModContent.Request<Texture2D>(ChainTexturePath);
            Asset<Texture2D> chainTextureExtra = ModContent.Request<Texture2D>(ChainTextureExtraPath); // This texture and related code is optional and used for a unique effect
                                                                                                       // 这个纹理和相关代码是可选的，用于产生独特的效果。

            Rectangle? chainSourceRectangle = null;
            // Drippler Crippler customizes sourceRectangle to cycle through sprite frames: sourceRectangle = asset.Frame(1, 6);

            // Drippler Crippler 自定义 sourceRectangle 来循环播放精灵帧：sourceRectangle = asset.Frame(1, 6);
            float chainHeightAdjustment = 0f; // Use this to adjust the chain overlap.
                                              // 使用它来调整链条重叠部分。

            Vector2 chainOrigin = chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Size() / 2f : chainTexture.Size() / 2f;
            Vector2 chainDrawPosition = Projectile.Center;
            Vector2 vectorFromProjectileToPlayerArms = playerArmPosition.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
            Vector2 unitVectorFromProjectileToPlayerArms = vectorFromProjectileToPlayerArms.SafeNormalize(Vector2.Zero);
            float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : chainTexture.Height()) + chainHeightAdjustment;
            if (chainSegmentLength == 0)
            {
                chainSegmentLength = 10; // When the chain texture is being loaded, the height is 0 which would cause infinite loops.
                                         // 当加载链条纹理时，高度为 0，会导致无限循环。
            }
            float chainRotation = unitVectorFromProjectileToPlayerArms.ToRotation() + MathHelper.PiOver2;
            int chainCount = 0;
            float chainLengthRemainingToDraw = vectorFromProjectileToPlayerArms.Length() + chainSegmentLength / 2f;

            // This while loop draws the chain texture from the projectile to the player, looping to draw the chain texture along the path
            // 这个 while 循环从投射物到玩家绘制链条纹理，并沿路径循环绘制
            while (chainLengthRemainingToDraw > 0f)
            {
                // This code gets the lighting at the current tile coordinates
                // 这段代码获取当前坐标处的光照情况
                Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

                // Flaming Mace and Drippler Crippler use code here to draw custom sprite frames with custom lighting.
                // Cycling through frames: sourceRectangle = asset.Frame(1, 6, 0, chainCount % 6);
                // This Analysis shows how Flaming Mace works. It checks chainCount and changes chainTexture and draw color at different values

                // Flaming Mace 和 Drippler Crippler 在此处使用代码来绘制自定义精灵帧和自定义光照。
                // 循环播放精灵帧：sourceRectangle = asset.Frame(1, 6, 0, chainCount % 6);
                // 此分析展示了 Flaming Mace 的工作原理。它检查 chainCount 并在不同值下更改 chainTexture 和 draw color。

                var chainTextureToDraw = chainTexture;
                if (chainCount >= 4)
                {
                    // Use normal chainTexture and lighting, no changes
                    // 使用普通的 chainTexture 和光照，没有变化
                }
                else if (chainCount >= 2)
                {
                    // Near to the ball, we draw a custom chain texture and slightly make it glow if unlit.
                    // 靠近球时，我们会绘制一个定制的链条纹理，并使其微微发亮（如果未被点亮）。
                    chainTextureToDraw = chainTextureExtra;
                    byte minValue = 140;
                    if (chainDrawColor.R < minValue)
                        chainDrawColor.R = minValue;

                    if (chainDrawColor.G < minValue)
                        chainDrawColor.G = minValue;

                    if (chainDrawColor.B < minValue)
                        chainDrawColor.B = minValue;
                }
                else
                {
                    // Close to the ball, we draw a custom chain texture and draw it at full brightness glow.
                    // 接近球时，我们会绘制一个定制的链条纹理，并以完全明亮发光方式进行绘制。
                    chainTextureToDraw = chainTextureExtra;
                    chainDrawColor = Color.White;
                }

                // Here, we draw the chain texture at the coordinates
                // 在这里，在指定坐标处绘制链条纹理
                Main.spriteBatch.Draw(chainTextureToDraw.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

                // chainDrawPosition is advanced along the vector back to the player by the chainSegmentLength
                // chainDrawPosition 沿着向量朝向玩家的方向前进，链条段长度为 chainSegmentLength。
                chainDrawPosition += unitVectorFromProjectileToPlayerArms * chainSegmentLength;
                chainCount++;
                chainLengthRemainingToDraw -= chainSegmentLength;
            }

            // Add a motion trail when moving forward, like most flails do (don't add trail if already hit a tile)
            // 当向前移动时添加运动轨迹，就像大多数连枷一样（如果已经击中了瓷砖，则不要添加轨迹）。
            if (CurrentAIState == AIState.LaunchingForward)
            {
                Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
                Vector2 drawOrigin = new Vector2(projectileTexture.Width * 0.5f, Projectile.height * 0.5f);
                SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                for (int k = 0; k < Projectile.oldPos.Length && k < StateTimer; k++)
                {
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                    Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    Main.spriteBatch.Draw(projectileTexture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale - k / (float)Projectile.oldPos.Length / 3, spriteEffects, 0f);
                }
            }
            return true;
        }
    }
}
