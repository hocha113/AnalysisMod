using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.ID;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace AnalysisMod.AnalysisContent.Projectiles
{
    // AnalysisCustomSwingSword is an Analysis of a sword with a custom swing using a held projectile
    // This is great if you want to make melee weapons with complex swing behaviour
    // Note that this projectile only covers 2 relatively simple swings, everything else is up to you
    // Aside from the custom animation, the custom collision code in Colliding is very important to this weapon

    // AnalysisCustomSwingSword是一种使用持有的投射物进行自定义挥动的剑分析
    // 如果您想制作具有复杂挥动行为的近战武器，这非常棒
    // 请注意，此投射物仅涵盖2个相对简单的摆动，其他所有内容都由您决定
    // 除了自定义动画外，在Colliding中的自定义碰撞代码对于该武器非常重要
    public class AnalysisCustomSwingProjectile : ModProjectile
    {
        // We define some constants that determine the swing range of the sword
        // Not that we use multipliers here since that simplifies the amount of tweaks for these interactions
        // You could change the values or even replace them entirely, but they are tweaked with looks in mind

        // 我们定义了一些常量来确定剑的摆动范围
        // 注意我们在这里使用乘数，因为这样可以简化这些交互所需调整的数量
        // 您可以更改值甚至完全替换它们，但它们是根据外观进行微调的
        private const float SWINGRANGE = 1.67f * (float)Math.PI; // The angle a swing attack covers (300 deg)
                                                                 // 摆动攻击覆盖角度（300度）

        private const float FIRSTHALFSWING = 0.45f; // How much of the swing happens before it reaches the target angle (in relation to swingRange)
                                                    // 在达到目标角度之前发生多少摇摆（与swingRange相关）

        private const float SPINRANGE = 3.5f * (float)Math.PI; // The angle a spin attack covers (630 degrees)
                                                               // 旋转攻击覆盖角度（630度）

        private const float WINDUP = 0.15f; // How far back the player's hand goes when winding their attack (in relation to swingRange)
                                            // 玩家手臂后退多远时开始攻击准备（与swingRange相关）

        private const float UNWIND = 0.4f; // When should the sword start disappearing
                                           // 剑应何时开始消失

        private const float SPINTIME = 2.5f; // How much longer a spin is than a swing
                                             // 旋转比普通挥砍长多少时间

        private enum AttackType // Which attack is being performed
                                // 正在执行哪种攻击
        {
            // Swings are normal sword swings that can be slightly aimed
            // Swings goes through the full cycle of animations

            // Swings是正常剑挥砍，可以略微瞄准
            // Swings通过完整循环播放所有动画
            Swing,

            // Spins are swings that go full circle
            // They are slower and deal more knockback

            // Spins是完整循环的挥砍
            // 它们较慢，但会造成更多击退
            Spin,
        }

        private enum AttackStage // What stage of the attack is being executed, see functions found in AI for description
                                 // 正在执行攻击的阶段，请参见AI中找到的函数以获取描述
        {
            Prepare,
            Execute,
            Unwind
        }

        // These properties wrap the usual ai and localAI arrays for cleaner and easier to understand code.
        // 这些属性包装了常规ai和localAI数组，使代码更清晰易懂。
        private AttackType CurrentAttack
        {
            get => (AttackType)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }

        private AttackStage CurrentStage
        {
            get => (AttackStage)Projectile.localAI[0];
            set
            {
                Projectile.localAI[0] = (float)value;
                Timer = 0; // reset the timer when the projectile switches states
                           // 在投射物切换状态时重置计时器
            }
        }

        // Variables to keep track of during runtime
        // 在运行时跟踪的变量
        private ref float InitialAngle => ref Projectile.ai[1]; // Angle aimed in (with constraints)
                                                                // 瞄准角度（带约束）

        private ref float Timer => ref Projectile.ai[2]; // Timer to keep track of progression of each stage
                                                         // 用于跟踪每个阶段进展的计时器

        private ref float Progress => ref Projectile.localAI[1]; // Position of sword relative to initial angle
                                                                 // 剑相对于初始角度的位置

        private ref float Size => ref Projectile.localAI[2]; // Size of sword
                                                             // 剑大小

        // We define timing functions for each stage, taking into account melee attack speed
        // Note that you can change this to suit the need of your projectile

        // 我们为每个阶段定义时间函数，考虑近战攻击速度
        // 请注意，您可以根据需要更改此设置以适应您的投射物需求。
        private float prepTime => 12f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
        private float execTime => 12f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
        private float hideTime => 12f / Owner.GetTotalAttackSpeed(Projectile.DamageType);

        public override string Texture => "AnalysisMod/AnalysisContent/Items/Weapons/AnalysisCustomSwingSword"; // Use texture of item as projectile texture
                                                                                                                // 使用物品纹理作为投射物纹理
        private Player Owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 46; // Hitbox width of projectile
            Projectile.height = 48; // Hitbox height of projectile
            Projectile.friendly = true; // Projectile hits enemies
            Projectile.timeLeft = 10000; // Time it takes for projectile to expire
                                         // 投射物过期所需时间

            Projectile.penetrate = -1; // Projectile pierces infinitely
                                       // 无限穿透弹丸

            Projectile.tileCollide = false; // Projectile does not collide with tiles
                                            // 弹丸不与图块碰撞

            Projectile.usesLocalNPCImmunity = true; // Uses local immunity frames
                                                    // 使用本地免疫帧

            Projectile.localNPCHitCooldown = -1; // We set this to -1 to make sure the projectile doesn't hit twice
                                                 // 我们将其设置为-1以确保弹丸不会命中两次

            Projectile.ownerHitCheck = true; // Make sure the owner of the projectile has line of sight to the target (aka can't hit things through tile).
                                             // 确保弹丸所有者能够看到目标（即不能通过图块打中东西）。

            Projectile.DamageType = DamageClass.Melee; // Projectile is a melee projectile
                                                       // 该弹丸是近战武器
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.spriteDirection = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
            float targetAngle = (Main.MouseWorld - Owner.MountedCenter).ToRotation();

            if (CurrentAttack == AttackType.Spin)
            {
                InitialAngle = (float)(-Math.PI / 2 - Math.PI * 1 / 3 * Projectile.spriteDirection); // For the spin, starting angle is designated based on direction of hit
                                                                                                     // 对于旋转，起始角度基于命中方向指定
            }
            else
            {
                if (Projectile.spriteDirection == 1)
                {
                    // However, we limit the rangle of possible directions so it does not look too ridiculous
                    // 但是，我们限制可能方向范围，以便它看起来不太荒谬
                    targetAngle = MathHelper.Clamp(targetAngle, (float)-Math.PI * 1 / 3, (float)Math.PI * 1 / 6);
                }
                else
                {
                    if (targetAngle < 0)
                    {
                        targetAngle += 2 * (float)Math.PI; // This makes the range continuous for easier operations
                                                           // 这使得范围连续，易于操作
                    }

                    targetAngle = MathHelper.Clamp(targetAngle, (float)Math.PI * 5 / 6, (float)Math.PI * 4 / 3);
                }

                InitialAngle = targetAngle - FIRSTHALFSWING * SWINGRANGE * Projectile.spriteDirection; // Otherwise, we calculate the angle
                                                                                                       // 否则，我们计算角度
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            // Projectile.spriteDirection for this projectile is derived from the mouse position of the owner in OnSpawn, as such it needs to be synced. spriteDirection is not one of the fields automatically synced over the network. All Projectile.ai slots are used already, so we will sync it manually. 
            // 这个投射物的 Projectile.spriteDirection 是从所有者鼠标位置派生出来的，因此需要进行同步。spriteDirection 不是自动在网络上同步的字段之一。所有 Projectile.ai 插槽都已经被使用了，所以我们将手动同步它。
            writer.Write((sbyte)Projectile.spriteDirection);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.spriteDirection = reader.ReadSByte();
        }

        public override void AI()
        {
            // Extend use animation until projectile is killed
            // 延长使用动画直到投射物死亡
            Owner.itemAnimation = 2;
            Owner.itemTime = 2;

            // Kill the projectile if the player dies or gets crowd controlled
            // 如果玩家死亡或被控制，则杀死投射物
            if (!Owner.active || Owner.dead || Owner.noItems || Owner.CCed)
            {
                Projectile.Kill();
                return;
            }

            // AI depends on stage and attack
            // Note that these stages are to facilitate the scaling effect at the beginning and end
            // If this is not desireable for you, feel free to simplify

            // AI 取决于阶段和攻击
            // 请注意，这些阶段是为了促进开始和结束时的缩放效果
            // 如果您不希望这样，请随意简化
            switch (CurrentStage)
            {
                case AttackStage.Prepare:
                    PrepareStrike();
                    break;
                case AttackStage.Execute:
                    ExecuteStrike();
                    break;
                default:
                    UnwindStrike();
                    break;
            }

            SetSwordPosition();
            Timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Calculate origin of sword (hilt) based on orientation and offset sword rotation (as sword is angled in its sprite)
            // 根据方向计算剑（柄）的起点，并根据偏移量旋转剑（因为剑在其精灵中呈角度）
            Vector2 origin;
            float rotationOffset;
            SpriteEffects effects;

            if (Projectile.spriteDirection > 0)
            {
                origin = new Vector2(0, Projectile.height);
                rotationOffset = MathHelper.ToRadians(45f);
                effects = SpriteEffects.None;
            }
            else
            {
                origin = new Vector2(Projectile.width, Projectile.height);
                rotationOffset = MathHelper.ToRadians(135f);
                effects = SpriteEffects.FlipHorizontally;
            }

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, default, lightColor * Projectile.Opacity, Projectile.rotation + rotationOffset, origin, Projectile.scale, effects, 0);

            // Since we are doing a custom draw, prevent it from normally drawing
            // 由于我们正在进行自定义绘制，因此要防止正常绘制
            return false;
        }

        // Find the start and end of the sword and use a line collider to check for collision with enemies
        // 找到剑的起点和终点，并使用线碰撞器检查与敌人是否发生碰撞
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Owner.MountedCenter;
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 15f * Projectile.scale, ref collisionPoint);
        }

        // Do a similar collision check for tiles
        // 对瓷砖执行类似的碰撞检查
        public override void CutTiles()
        {
            Vector2 start = Owner.MountedCenter;
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
            Utils.PlotTileLine(start, end, 15 * Projectile.scale, DelegateMethods.CutTiles);
        }

        // We make it so that the projectile can only do damage in its release and unwind phases
        // 我们使得该投射物只能在释放和解开阶段造成伤害
        public override bool? CanDamage()
        {
            if (CurrentStage == AttackStage.Prepare)
                return false;
            return base.CanDamage();
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // Make knockback go away from player
            // 让击退远离玩家
            modifiers.HitDirectionOverride = target.position.X > Owner.MountedCenter.X ? 1 : -1;

            // If the NPC is hit by the spin attack, increase knockback slightly
            // 如果 NPC 被旋转攻击命中，则略微增加击退力量
            if (CurrentAttack == AttackType.Spin)
                modifiers.Knockback += 1;
        }

        // Function to easily set projectile and arm position
        // 方便设置投射物和手臂位置的函数
        public void SetSwordPosition()
        {
            Projectile.rotation = InitialAngle + Projectile.spriteDirection * Progress; // Set projectile rotation
                                                                                        // 设置投射物旋转

            // Set composite arm allows you to set the rotation of the arm and stretch of the front and back arms independently
            // 设置复合手臂允许您独立设置手臂的旋转以及前后手臂的伸展程度
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f)); // set arm position (90 degree offset since arm starts lowered)
            Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)Math.PI / 2); // get position of hand

            armPosition.Y += Owner.gfxOffY;
            Projectile.Center = armPosition; // Set projectile to arm position
                                             // 将投射物设为手臂位置

            Projectile.scale = Size * 1.2f * Owner.GetAdjustedItemScale(Owner.HeldItem); // Slightly scale up the projectile and also take into account melee size modifiers
                                                                                         // 稍微放大投射物，并考虑近战大小修饰符

            Owner.heldProj = Projectile.whoAmI; // set held projectile to this projectile
                                                // 将持有的投射物设为此投射物
        }

        // Function facilitating the taking out of the sword
        // 促进拔剑动作的函数
        private void PrepareStrike()
        {
            Progress = WINDUP * SWINGRANGE * (1f - Timer / prepTime); // Calculates rotation from initial angle
                                                                      // 从初始角度计算旋转

            Size = MathHelper.SmoothStep(0, 1, Timer / prepTime); // Make sword slowly increase in size as we prepare to strike until it reaches max
                                                                  // 使剑慢慢增大，直到达到最大值，准备攻击

            if (Timer >= prepTime)
            {
                SoundEngine.PlaySound(SoundID.Item1); // Play sword sound here since playing it on spawn is too early
                                                      // 在这里播放剑声音，因为在生成时播放太早了

                CurrentStage = AttackStage.Execute; // If attack is over prep time, we go to next stage
                                                    // 如果攻击时间超过准备时间，则进入下一阶段
            }
        }

        // Function facilitating the first half of the swing
        // 促进挥舞动作前半部分的函数
        private void ExecuteStrike()
        {
            if (CurrentAttack == AttackType.Swing)
            {
                Progress = MathHelper.SmoothStep(0, SWINGRANGE, (1f - UNWIND) * Timer / execTime);

                if (Timer >= execTime)
                {
                    CurrentStage = AttackStage.Unwind;
                }
            }
            else
            {
                Progress = MathHelper.SmoothStep(0, SPINRANGE, (1f - UNWIND / 2) * Timer / (execTime * SPINTIME));

                if (Timer == (int)(execTime * SPINTIME * 3 / 4))
                {
                    SoundEngine.PlaySound(SoundID.Item1); // Play sword sound again
                                                          // 再次播放剑声音

                    Projectile.ResetLocalNPCHitImmunity(); // Reset the local npc hit immunity for second half of spin
                                                           // 重置第二个半圈中本地 NPC 的受击免疫性
                }

                if (Timer >= execTime * SPINTIME)
                {
                    CurrentStage = AttackStage.Unwind;
                }
            }
        }

        // Function facilitating the latter half of the swing where the sword disappears
        // 促进挥舞动作后半部分并隐藏剑的函数
        private void UnwindStrike()
        {
            if (CurrentAttack == AttackType.Swing)
            {
                Progress = MathHelper.SmoothStep(0, SWINGRANGE, 1f - UNWIND + UNWIND * Timer / hideTime);
                Size = 1f - MathHelper.SmoothStep(0, 1, Timer / hideTime); // Make sword slowly decrease in size as we end the swing to make a smooth hiding animation
                                                                           // 使剑缓慢缩小，以进行平滑隐藏动画

                if (Timer >= hideTime)
                {
                    Projectile.Kill();
                }
            }
            else
            {
                Progress = MathHelper.SmoothStep(0, SPINRANGE, 1f - UNWIND / 2 + UNWIND / 2 * Timer / (hideTime * SPINTIME / 2));
                Size = 1f - MathHelper.SmoothStep(0, 1, Timer / (hideTime * SPINTIME / 2));

                if (Timer >= hideTime * SPINTIME / 2)
                {
                    Projectile.Kill();
                }
            }
        }
    }
}