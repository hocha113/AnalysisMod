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

    // Analysis Advanced Flail��Ball O' Hurt�������������Ӧ�������Ѿ�������һЩ��д��ʹ���������⡣���˴����뷴�����Terraria������бȽϣ��Է��������Ӧ�������롣һЩע�ͺͶ���Ĵ���Ƭ��չʾ�����������������������ԡ�
    // Analysis Advanced Flailչʾ�˴����߼�AI����ײ���⡣
    // �йؼ򵥵���̫�ɶ��ƻ������������������μ�AnalysisFlail��
    public class AnalysisAdvancedFlailProjectile : ModProjectile
    {
        private const string ChainTexturePath = "AnalysisMod/AnalysisContent/Projectiles/AnalysisAdvancedFlailProjectileChain"; // The folder path to the flail chain sprite
                                                                                                                                // ���������ļ���·��

        private const string ChainTextureExtraPath = "AnalysisMod/AnalysisContent/Projectiles/AnalysisAdvancedFlailProjectileChainExtra";  // This texture and related code is optional and used for a unique effect
                                                                                                                                           // ���������ش����ǿ�ѡ����ڲ�������Ч��

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
        // ��Щ���԰�װ����ai��localAI���飬ʹ�ô�����������׶���
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
            // ��Щ�з�����β����
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true; // This ensures that the projectile is synced when other players join the world.
                                            // ȷ����������Ҽ�������ʱ�õ����ﱻͬ����

            Projectile.width = 24; // The width of your projectile
                                   // ��ĵ�������

            Projectile.height = 24; // The height of your projectile
                                    // ��ĵ�����߶�

            Projectile.friendly = true; // Deals damage to enemies
                                        // �Ե�������˺�

            Projectile.penetrate = -1; // Infinite pierce
                                       // ���޴�͸��

            Projectile.DamageType = DamageClass.Melee; // Deals melee damage
                                                       // ��ս�˺�����

            Projectile.usesLocalNPCImmunity = true; // Used for hit cooldown changes in the ai hook
                                                    // ����ai�����д����ȴʱ��仯

            Projectile.localNPCHitCooldown = 10; // This facilitates custom hit cooldown logic
                                                 // ���������Զ�������ȴ�߼�

            // Vanilla flails all use aiStyle 15, but the code isn't customizable so an adaption of that aiStyle is used in the AI method
            // ���л�������������ʹ��aiStyle 15, ���ô��벻�ɶ��ƻ��������AI������ʹ����һ���ı��aiStyle
        }

        // This AI code was adapted from vanilla code: Terraria.Projectile.AI_015_Flails() 

        // ��AI�����Ǵӻ��������иı������Terraria.Projectile.AI_015_Flails()
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            // Kill the projectile if the player dies or gets crowd controlled
            // �����������򱻿��ƣ���ɱ��������
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
                                       // ���������ջ�֮ǰ���Է��е�ʱ�䳤�ȣ��ٶȺ�shootTimer����������ķ�Χ��

            float launchSpeed = 14f; // How fast the projectile can move
                                     // ����������ƶ����ٶ�

            float maxLaunchLength = 800f; // How far the projectile's chain can stretch before being forced to retract when in launched state
                                          // ����״̬�£������ܹ���չ��Զ�Żᱻ������

            float retractAcceleration = 3f; // How quickly the projectile will accelerate back towards the player while retracting
                                            // ������ջ�ʱ����������ٵĿ����̶�

            float maxRetractSpeed = 10f; // The max speed the projectile will have while retracting
                                         // ���ջ�ʱ������������ٶ�

            float forcedRetractAcceleration = 6f; // How quickly the projectile will accelerate back towards the player while being forced to retract
                                                  // ����������ʱ����������ٷ�����ҵĿ����̶�

            float maxForcedRetractSpeed = 15f; // The max speed the projectile will have while being forced to retract
                                               // �ڱ�������ʱ������������ٶ�
            float unusedRetractAcceleration = 1f;
            float unusedMaxRetractSpeed = 14f;
            int unusedChainLength = 60;
            int defaultHitCooldown = 10; // How often your flail hits when resting on the ground, or retracting
                                         // ��ֹ�������ջ�״̬��������򹥻�Ƶ��

            int spinHitCooldown = 20; // How often your flail hits when spinning
                                      // ��ת״̬��������򹥻�Ƶ��

            int movingHitCooldown = 10; // How often your flail hits when moving
                                        // �ƶ�״̬��������򹥻�Ƶ��

            int ricochetTimeLimit = launchTimeLimit + 5;

            // Scaling these speeds and accelerations by the players melee speed makes the weapon more responsive if the player boosts it or general weapon speed
            // ͨ����ҽ�ս������������Щ�ٶȺͼ��ٶ�ʹ�ø�����������Ӧ�ԡ�
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
                                                 // �������ͷ����л�����ǰģʽ��
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
                        // ��һ�д�����һ����λ�������������Χ������ת��10f��������Ŀ�Ӿ������������ת�Ŀ����̶ȡ�
                        Vector2 offsetFromPlayer = new Vector2(player.direction).RotatedBy((float)Math.PI * 10f * (SpinningStateTimer / 60f) * player.direction);

                        offsetFromPlayer.Y *= 0.8f;
                        if (offsetFromPlayer.Y * player.gravDir > 0f)
                        {
                            offsetFromPlayer.Y *= 0.5f;
                        }
                        Projectile.Center = mountedCenter + offsetFromPlayer * 30f + new Vector2(0, player.gfxOffY);
                        Projectile.velocity = Vector2.Zero;
                        Projectile.localNPCHitCooldown = spinHitCooldown; // set the hit speed to the spinning hit speed
                                                                          // ������ٶ���Ϊ��ת����ٶ�
                        break;
                    }
                case AIState.LaunchingForward:
                    {
                        doFastThrowDust = true;
                        bool shouldSwitchToRetracting = StateTimer++ >= launchTimeLimit;
                        shouldSwitchToRetracting |= Projectile.Distance(mountedCenter) >= maxLaunchLength;
                        if (player.controlUseItem) // If the player clicks, transition to the Dropping state
                                                   // �����ҵ������ת��������״̬
                        {
                            CurrentAIState = AIState.Dropping;
                            StateTimer = 0f;
                            Projectile.netUpdate = true;
                            Projectile.velocity *= 0.2f;
                            // This is where Drippler Crippler spawns its projectile
                            // ����Drippler Crippler�����䵯�����λ�á�
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
                            // ��Ҳ��Drippler Crippler�����䵯�����λ�ã���μ�����Ĵ��롣
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
                                               // �����������ʱ��ɱ���õ����
                            return;
                        }
                        if (player.controlUseItem) // If the player clicks, transition to the Dropping state
                                                   // �����ҵ������ת��������״̬
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
                                          // Projectile.ai[0] == 3; �����ʵ����δʹ�ã������ܻ���Terraria������������ӣ������������õģ�����ҽ��䱣�������
                    {
                        if (!player.controlUseItem)
                        {
                            CurrentAIState = AIState.ForcedRetracting; // Move to super retracting mode if the player taps
                                                                       // ��������������볬������ģʽ
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
                                               // ��������㹻��ʱɱ����
                            return;
                        }
                        Projectile.velocity *= 0.98f;
                        Projectile.velocity = Projectile.velocity.MoveTowards(unitVectorTowardsPlayer * maxForcedRetractSpeed, forcedRetractAcceleration);
                        Vector2 target = Projectile.Center + Projectile.velocity;
                        Vector2 value = mountedCenter.DirectionFrom(target).SafeNormalize(Vector2.Zero);
                        if (Vector2.Dot(unitVectorTowardsPlayer, value) < 0f)
                        {
                            Projectile.Kill(); // Kill projectile if it will pass the player
                                               // ���������������ʱɱ���õ����
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
            // �����Flower Pow������Ŀ�ĵط���������Terraria�Բ鿴�ô��롣

            Projectile.direction = (Projectile.velocity.X > 0f).ToDirectionInt();
            Projectile.spriteDirection = Projectile.direction;
            Projectile.ownerHitCheck = shouldOwnerHitCheck; // This prevents attempting to damage enemies without line of sight to the player. The custom Colliding code for spinning makes this necessary.
                                                            // ����Է�ֹ�����û�����ߵ��������ͼ�������ˡ���תʱ�Զ�����ײ����ʹ���Ϊ��Ҫ������

            // This rotation code is unique to this flail, since the sprite isn't rotationally symmetric and has tip.
            // �����ת�����Ƕ��صģ���Ϊ���鲻������ת�Գ��Բ����м�ˡ�
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
            // �������һ���������Ǵ��������ʹ������򻯵���ת����
            /*
			if (Projectile.velocity.Length() > 1f)
				Projectile.rotation = Projectile.velocity.ToRotation() + Projectile.velocity.X * 0.1f; // skid
			else
				Projectile.rotation += Projectile.velocity.X * 0.1f; // roll
			*/

            Projectile.timeLeft = 2; // Makes sure the flail doesn't die (good when the flail is resting on the ground)
                                     // ȷ�����Ǵ����������������Ǵ���ֹ�ڵ�����ʱ�ܺã�

            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2); //Add a delay so the player can't button mash the flail
                                        //����ӳ��Է�ֹ�����������ť

            player.itemRotation = Projectile.DirectionFrom(mountedCenter).ToRotation();
            if (Projectile.Center.X < mountedCenter.X)
            {
                player.itemRotation += (float)Math.PI;
            }
            player.itemRotation = MathHelper.WrapAngle(player.itemRotation);

            // Spawning dust. We spawn dust more often when in the LaunchingForward state
            // ������������������ǰ����״̬�¸�Ƶ�������ɳ���
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
            // ��������ѷ���״̬�������ɻ�
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
            // �ڴ˴�����ש����ָʾ���Ǳ����еĻҳ�
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
            // ǿ�������������ƽ�����϶���������
            if (CurrentAIState != AIState.UnusedState && CurrentAIState != AIState.Spinning && CurrentAIState != AIState.Ricochet && CurrentAIState != AIState.Dropping && CollisionCounter >= 10f)
            {
                CurrentAIState = AIState.ForcedRetracting;
                Projectile.netUpdate = true;
            }

            // tModLoader currently does not provide the wetVelocity parameter, this code should make the flail bounce back faster when colliding with tiles underwater.
            // tModLoaderĿǰ���ṩwetVelocity��������ˮ����ƽ������ײʱӦ��ʹ���Ǵ������ø��졣
            //if (Projectile.wet)
            //	wetVelocity = Projectile.velocity;

            return false;
        }

        public override bool? CanDamage()
        {
            // Flails in spin mode won't damage enemies within the first 12 ticks. Visually this delays the first hit until the player swings the flail around for a full spin before damaging anything.
            // ��תģʽ�µ����Ǵ�����ǰ12��tick���޷��˺����ˡ����Ӿ��Ͽ����������ȫ�ڶ����Ǵ�֮ǰ�ӳٵ�һ�������κζ�����
            if (CurrentAIState == AIState.Spinning && SpinningStateTimer <= 12f)
            {
                return false;
            }
            return base.CanDamage();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // Flails do special collision logic that serves to hit anything within an ellipse centered on the player when the flail is spinning around the player. For Analysis, the projectile rotating around the player won't actually hit a bee if it is directly on the player usually, but this code ensures that the bee is hit. This code makes hitting enemies while spinning more consistant and not reliant of the actual position of the flail projectile.
            // ���Ǵ�����������ײ�߼������ڻ���Χ���������ת�����Ǵ���Χ��Բ���ڵ��κζ��������ڷ���������۷�ֱ�����������ͨ�����ᱻ���У�������δ���ȷ�����۷䱻���С��˴���ʹ����תʱ������˸���һ�£����Ҳ����������Ǵ�������ʵ��λ�á�
            if (CurrentAIState == AIState.Spinning)
            {
                Vector2 mountedCenter = Main.player[Projectile.owner].MountedCenter;
                Vector2 shortestVectorFromPlayerToTarget = targetHitbox.ClosestPointInRect(mountedCenter) - mountedCenter;
                shortestVectorFromPlayerToTarget.Y /= 0.8f; // Makes the hit area an ellipse. Vertical hit distance is smaller due to this math.
                                                            // ʹ���������Ϊһ����Բ�Ρ����������ѧ��ʽ����ֱ���о����С��

                float hitRadius = 55f; // The length of the semi-major radius of the ellipse (the long end)
                                       // ��Բ�볤�᳤�ȣ����ˣ�

                return shortestVectorFromPlayerToTarget.Length() <= hitRadius;
            }
            // Regular collision logic happens otherwise.
            // ������������ײ�߼���
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // Flails do a few custom things, you'll want to keep these to have the same feel as vanilla flails.
            // ���Ǵ���һЩ�Զ��幦�ܣ�����Ҫ���������Ծ�����������Ǵ���ͬ�ĸо���

            // Flails do 20% more damage while spinning
            // ��תʱ���Ǵ����20%�����˺�
            if (CurrentAIState == AIState.Spinning)
            {
                modifiers.SourceDamage *= 1.2f;
            }
            // Flails do 100% more damage while launched or retracting. This is the damage the item tooltip for flails aim to match, as this is the most common mode of attack. This is why the item has ItemID.Sets.ToolTipDamageMultiplier[Type] = 2f;
            // ���������ʱ���Ǵ����100%�����˺�����������ƥ����Ʒ������ʾ����ﵽ���˺�ֵ����Ϊ��������Ĺ���ģʽ�������Ϊʲô����Ʒ����ItemID.Sets.ToolTipDamageMultiplier[Type] = 2f; ��ԭ��
            else if (CurrentAIState == AIState.LaunchingForward || CurrentAIState == AIState.Retracting)
            {
                modifiers.SourceDamage *= 2f;
            }

            // The hitDirection is always set to hit away from the player, even if the flail damages the npc while returning
            // ���з���ʼ������ΪԶ����ң���ʹ�ڷ��ع�����Ҳ����npc
            modifiers.HitDirectionOverride = (Main.player[Projectile.owner].Center.X < target.Center.X).ToDirectionInt();

            // Knockback is only 25% as powerful when in spin mode
            // ����תģʽ�·���ֻ��25%������
            if (CurrentAIState == AIState.Spinning)
            {
                modifiers.Knockback *= 0.25f;
            }
            // Knockback is only 50% as powerful when in drop down mode
            // ���½�ģʽ�·���ֻ��50%������
            else if (CurrentAIState == AIState.Dropping)
            {
                modifiers.Knockback *= 0.5f;
            }
        }

        // PreDraw is used to draw a chain and trail before the projectile is drawn normally.
        // PreDraw�������������Ƶ���֮ǰ����������β��
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 playerArmPosition = Main.GetPlayerArmPosition(Projectile);

            // This fixes a vanilla GetPlayerArmPosition bug causing the chain to draw incorrectly when stepping up slopes. The flail itself still draws incorrectly due to another similar bug. This should be removed once the vanilla bug is fixed.
            // ���޸������GetPlayerArmPosition���󣬵�����������ʱ���Ʋ���ȷ��������һ�����ƵĴ������Ǵ�������Ȼ����Ʋ���ȷ��һ�����bug���޸���Ӧ�ý���ɾ����
            playerArmPosition.Y -= Main.player[Projectile.owner].gfxOffY;

            Asset<Texture2D> chainTexture = ModContent.Request<Texture2D>(ChainTexturePath);
            Asset<Texture2D> chainTextureExtra = ModContent.Request<Texture2D>(ChainTextureExtraPath); // This texture and related code is optional and used for a unique effect
                                                                                                       // ����������ش����ǿ�ѡ�ģ����ڲ������ص�Ч����

            Rectangle? chainSourceRectangle = null;
            // Drippler Crippler customizes sourceRectangle to cycle through sprite frames: sourceRectangle = asset.Frame(1, 6);

            // Drippler Crippler �Զ��� sourceRectangle ��ѭ�����ž���֡��sourceRectangle = asset.Frame(1, 6);
            float chainHeightAdjustment = 0f; // Use this to adjust the chain overlap.
                                              // ʹ���������������ص����֡�

            Vector2 chainOrigin = chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Size() / 2f : chainTexture.Size() / 2f;
            Vector2 chainDrawPosition = Projectile.Center;
            Vector2 vectorFromProjectileToPlayerArms = playerArmPosition.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
            Vector2 unitVectorFromProjectileToPlayerArms = vectorFromProjectileToPlayerArms.SafeNormalize(Vector2.Zero);
            float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : chainTexture.Height()) + chainHeightAdjustment;
            if (chainSegmentLength == 0)
            {
                chainSegmentLength = 10; // When the chain texture is being loaded, the height is 0 which would cause infinite loops.
                                         // ��������������ʱ���߶�Ϊ 0���ᵼ������ѭ����
            }
            float chainRotation = unitVectorFromProjectileToPlayerArms.ToRotation() + MathHelper.PiOver2;
            int chainCount = 0;
            float chainLengthRemainingToDraw = vectorFromProjectileToPlayerArms.Length() + chainSegmentLength / 2f;

            // This while loop draws the chain texture from the projectile to the player, looping to draw the chain texture along the path
            // ��� while ѭ����Ͷ���ﵽ��һ���������������·��ѭ������
            while (chainLengthRemainingToDraw > 0f)
            {
                // This code gets the lighting at the current tile coordinates
                // ��δ����ȡ��ǰ���괦�Ĺ������
                Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

                // Flaming Mace and Drippler Crippler use code here to draw custom sprite frames with custom lighting.
                // Cycling through frames: sourceRectangle = asset.Frame(1, 6, 0, chainCount % 6);
                // This Analysis shows how Flaming Mace works. It checks chainCount and changes chainTexture and draw color at different values

                // Flaming Mace �� Drippler Crippler �ڴ˴�ʹ�ô����������Զ��徫��֡���Զ�����ա�
                // ѭ�����ž���֡��sourceRectangle = asset.Frame(1, 6, 0, chainCount % 6);
                // �˷���չʾ�� Flaming Mace �Ĺ���ԭ������� chainCount ���ڲ�ֵͬ�¸��� chainTexture �� draw color��

                var chainTextureToDraw = chainTexture;
                if (chainCount >= 4)
                {
                    // Use normal chainTexture and lighting, no changes
                    // ʹ����ͨ�� chainTexture �͹��գ�û�б仯
                }
                else if (chainCount >= 2)
                {
                    // Near to the ball, we draw a custom chain texture and slightly make it glow if unlit.
                    // ������ʱ�����ǻ����һ�����Ƶ�����������ʹ��΢΢���������δ����������
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
                    // �ӽ���ʱ�����ǻ����һ�����Ƶ���������������ȫ�������ⷽʽ���л��ơ�
                    chainTextureToDraw = chainTextureExtra;
                    chainDrawColor = Color.White;
                }

                // Here, we draw the chain texture at the coordinates
                // �������ָ�����괦������������
                Main.spriteBatch.Draw(chainTextureToDraw.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

                // chainDrawPosition is advanced along the vector back to the player by the chainSegmentLength
                // chainDrawPosition ��������������ҵķ���ǰ���������γ���Ϊ chainSegmentLength��
                chainDrawPosition += unitVectorFromProjectileToPlayerArms * chainSegmentLength;
                chainCount++;
                chainLengthRemainingToDraw -= chainSegmentLength;
            }

            // Add a motion trail when moving forward, like most flails do (don't add trail if already hit a tile)
            // ����ǰ�ƶ�ʱ����˶��켣��������������һ��������Ѿ������˴�ש����Ҫ��ӹ켣����
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
