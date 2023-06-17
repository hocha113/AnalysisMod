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

        /// <summary>
        /// 摆动攻击覆盖角度
        /// </summary>
        private const float SWINGRANGE = 1.67f * (float)Math.PI; // The angle a swing attack covers (300 deg)
                                                                 // 摆动攻击覆盖角度（300度）

        /// <summary>
        /// 在达到目标角度之前发生多少摇摆
        /// </summary>
        private const float FIRSTHALFSWING = 0.45f; // How much of the swing happens before it reaches the target angle (in relation to swingRange)
                                                    // 在达到目标角度之前发生多少摇摆（与swingRange相关）

        /// <summary>
        /// 旋转攻击覆盖角度
        /// </summary>
        private const float SPINRANGE = 3.5f * (float)Math.PI; // The angle a spin attack covers (630 degrees)
                                                               // 旋转攻击覆盖角度（630度）

        /// <summary>
        /// 玩家手臂后退多远时开始攻击准备
        /// </summary>
        private const float WINDUP = 0.15f; // How far back the player's hand goes when winding their attack (in relation to swingRange)
                                            // 玩家手臂后退多远时开始攻击准备（与swingRange相关）

        /// <summary>
        /// 剑应何时开始消失
        /// </summary>
        private const float UNWIND = 0.4f; // When should the sword start disappearing
                                           // 剑应何时开始消失

        /// <summary>
        /// 旋转比普通挥砍长多少时间
        /// </summary>
        private const float SPINTIME = 2.5f; // How much longer a spin is than a swing
                                             // 旋转比普通挥砍长多少时间

        /// <summary>
        /// 【攻击种类】
        /// </summary>
        private enum AttackType // Which attack is being performed
                                // 正在执行哪种攻击
        {
            // Swings are normal sword swings that can be slightly aimed
            // Swings goes through the full cycle of animations

            // Swings是正常剑挥砍，可以略微瞄准
            // Swings通过完整循环播放所有动画
            /// <summary>
            /// 【大摆臂式挥砍】
            /// </summary>
            Swing,

            // Spins are swings that go full circle
            // They are slower and deal more knockback

            // Spins是完整旋转循环的挥砍
            // 它们较慢，但会造成更多击退
            /// <summary>
            /// 【大风车式挥砍】
            /// </summary>
            Spin,
        }

        /// <summary>
        /// 【攻击阶段，这包括了摆臂砍和旋转砍的过程】
        /// </summary>
        private enum AttackStage // What stage of the attack is being executed, see functions found in AI for description
                                 // 正在执行攻击的阶段，请参见AI中找到的函数以获取描述
        {
            /// <summary>
            /// 【出刀】=0
            /// </summary>
            Prepare,
            /// <summary>
            /// 【挥砍】=1
            /// </summary>
            Execute,
            /// <summary>
            /// 【收刀】=2
            /// </summary>
            Unwind
        }

        // These properties wrap the usual ai and localAI arrays for cleaner and easier to understand code.
        // 这些属性包装了常规ai和localAI数组，使代码更清晰易懂。

        /// <summary>
        /// 攻击种类的AI数组封装
        /// </summary>
        private AttackType CurrentAttack
        {
            get => (AttackType)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }

        /// <summary>
        /// 攻击阶段的AI数组封装
        /// </summary>
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

        /// <summary>
        /// 【瞄准角度】
        /// </summary>
        private ref float InitialAngle => ref Projectile.ai[1]; // Angle aimed in (with constraints)
                                                                // 瞄准角度（带约束）

        /// <summary>
        /// 用于跟踪每个阶段进展的计时器
        /// </summary>
        private ref float Timer => ref Projectile.ai[2]; // Timer to keep track of progression of each stage
                                                         // 用于跟踪每个阶段进展的计时器

        /// <summary>
        /// 剑相对于初始角度的位置
        /// </summary>
        private ref float Progress => ref Projectile.localAI[1]; // Position of sword relative to initial angle
                                                                 // 剑相对于初始角度的位置

        /// <summary>
        /// 剑大小
        /// </summary>
        private ref float Size => ref Projectile.localAI[2]; // Size of sword
                                                             // 剑大小

        // We define timing functions for each stage, taking into account melee attack speed
        // Note that you can change this to suit the need of your projectile

        // 我们为每个阶段定义时间函数，考虑近战攻击速度
        // 请注意，您可以根据需要更改此设置以适应您的投射物需求。

        /// <summary>
        /// 【拔刀时间】
        /// </summary>
        private float PrepTime => 12f / Owner.GetTotalAttackSpeed(Projectile.DamageType);  // 【Owner.GetTotalAttackSpeed(Projectile.DamageType)是Terraria中的方法，用于获取玩家或NPC的总攻击速度。这个方法的参数是Projectile.DamageType，它指定了伤害类型】

        /// <summary>
        /// 【挥砍时间】
        /// </summary>
        private float ExecTime => 12f / Owner.GetTotalAttackSpeed(Projectile.DamageType);  // 【Projectile.DamageType是一个枚举类型，用于指定伤害的类型。它用于确定计算玩家或NPC总攻击速度时应考虑哪种类型的伤害】

        /// <summary>
        /// 【收刀时间】
        /// </summary>
        private float HideTime => 12f / Owner.GetTotalAttackSpeed(Projectile.DamageType);

        /// <summary>
        /// 【纹理路径】
        /// </summary>
        public override string Texture => "AnalysisMod/AnalysisContent/Items/Weapons/AnalysisCustomSwingSword"; // Use texture of item as projectile texture
                                                                                                                // 使用物品纹理作为投射物纹理

        /// <summary>
        /// 弹幕所有者
        /// </summary>
        private Player Owner => Main.player[Projectile.owner];//【获取弹幕所有者，也就是正在挥剑的您】

        public override void SetStaticDefaults()
        {
            //【HeldProjDoesNotUsePlayerGfxOffY是一个属性，它控制是否使用玩家的图像偏移值（gfxOffY）】
            //【图像偏移值是一个用于调整图像绘制位置的数值，它可以让图像在垂直方向上有所偏移】
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
                                             // 确保弹丸所有者能够看到目标（即不能穿过图块打中东西）。

            Projectile.DamageType = DamageClass.Melee; // Projectile is a melee projectile
                                                       // 该弹丸是近战武器
        }

        //【这个函数将在该弹幕实例在世界上被创建时调用】
        public override void OnSpawn(IEntitySource source)
        {
            // 【 (Main.MouseWorld.X , Main.MouseWorld.Y) 是玩家光标在世界中的位置坐标】
            // 【 (Owner.MountedCenter.X , Owner.MountedCenter.Y) 是玩家中心点在世界中的坐标】
            // 【 Projectile.spriteDirection 用于确定弹幕（Projectile）的精灵方向（Sprite Direction）。它表示弹幕的当前面朝方向，即弹幕精灵在水平方向上的朝向】

            Projectile.spriteDirection = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1; // 【1为向右，-1为向左，因为坐标系的X轴是向右递增的】
            float targetAngle = (Main.MouseWorld - Owner.MountedCenter).ToRotation(); // 【关于ToRotation()函数的解析见 AdditionalAnalysis/TmodLoaderMathHelper.cs 】

            /*
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
                    // 但是，我们限制可能的方向范围，以便它看起来不太荒谬
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
            */

            // 【您也可以选择注释掉限制瞄准角度的代码并仅仅启用该段赋值代码，这样武器仍旧正常使用，甚至手感会更好】
            InitialAngle = targetAngle - FIRSTHALFSWING * SWINGRANGE * Projectile.spriteDirection;

            /*
            //【如果您疑惑角度的初始角是从哪里开始，可以分别启用这些代码去游戏里观察】
            //【您会发现，初始轴就是极坐标的母轴，原点为人物真中心，也就是角色向右的那条水平线为0弧度轴】
            InitialAngle = 0;
            InitialAngle = Projectile.spriteDirection;
            */

        }

        //【网络同步-写入】
        public override void SendExtraAI(BinaryWriter writer)
        {
            // Projectile.spriteDirection for this projectile is derived from the mouse position of the owner in OnSpawn, as such it needs to be synced. spriteDirection is not one of the fields automatically synced over the network. All Projectile.ai slots are used already, so we will sync it manually. 
            // 这个投射物的 Projectile.spriteDirection 是从所有者鼠标位置派生出来的，因此需要进行同步。spriteDirection 不是自动在网络上同步的字段之一。所有 Projectile.ai 插槽都已经被使用了，所以我们将手动同步它。
            writer.Write((sbyte)Projectile.spriteDirection);
        }

        //【网络同步-读出】
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
                //【出刀】
                case AttackStage.Prepare:
                    PrepareStrike();
                    break;
                //【挥砍】
                case AttackStage.Execute:
                    ExecuteStrike();
                    break;
                //【收刀】
                default:
                    UnwindStrike();
                    break;
            }
            SetSwordPosition();
            Timer++;
        }

        //【绘制刀刃的精灵，因为刀会往左右两个方向挥舞，这涉及到精灵的翻转调整，同时刀刃的精灵的角度非水平，所以我们需要根据挥动方向来设置精灵的角度纠正】
        public override bool PreDraw(ref Color lightColor)
        {
            // Calculate origin of sword (hilt) based on orientation and offset sword rotation (as sword is angled in its sprite)
            // 根据方向计算剑（柄）的起点，并根据偏移量旋转剑（因为剑在其精灵图中呈角度）
            Vector2 origin;
            float rotationOffset;//【精灵偏移角度】

            //【SpriteEffects 是一个枚举类型，用于指定在绘制图像时应用的特定效果】
            //【SpriteEffects 枚举包含以下成员：】
            //【None：无特殊效果，图像按原样绘制。】
            //【FlipHorizontally：水平翻转图像，即左右翻转。】
            //【FlipVertically：垂直翻转图像，即上下翻转。】
            //【FlipBoth：同时水平和垂直翻转图像，即旋转180度。】
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

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;//【获取纹理】

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, default, lightColor * Projectile.Opacity, Projectile.rotation + rotationOffset, origin, Projectile.scale, effects, 0);

            // Since we are doing a custom draw, prevent it from normally drawing
            // 由于我们正在进行自定义绘制，因此要防止正常绘制
            return false;
        }

        // Find the start and end of the sword and use a line collider to check for collision with enemies
        // 找到剑的起点和终点，并使用线碰撞器检查与敌人是否发生碰撞
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Owner.MountedCenter;//【获取玩家位置，考虑到玩家坐在家具上或者坐骑上的情况】

            //【 oRotationVector2() 是 float 类型的扩展方法，用于将旋转角度转换为二维单位向量】
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
            float collisionPoint = 0f;

            bool BoolCollision= Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 15f * Projectile.scale, ref collisionPoint);

            //【我们也可以利用这个碰撞函数给这把真近战武器加一点技能，比如砍中敌人时生成随机射向四周的恶魔镰刀】
            if (BoolCollision)
            {
                Random random = new Random();
                float Oing = MathHelper.ToRadians((random.Next(0, 360)));
                Vector2 OingVector = Oing.ToRotationVector2();
                Vector2 SentginVector = Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(),Projectile.Center + SentginVector, OingVector,ProjectileID.DemonScythe,Projectile.damage,0f);
            }

            return BoolCollision;
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
        // 我们使得该投射物只能在挥刀和旋转阶段造成伤害
        public override bool? CanDamage()
        {
            if (CurrentStage == AttackStage.Prepare)
                return false;
            return base.CanDamage();
        }

        //【此函数设置击打效果，弹幕打中敌人后触发的代码放这里会更加合适】
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // Make knockback go away from player
            // 让击退远离玩家
            modifiers.HitDirectionOverride = target.position.X > Owner.MountedCenter.X ? 1 : -1;

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ProjectileID.DeathSickle, Projectile.damage, 0f);//【在受击敌人的位置处生成死神镰刀弹幕】

            // If the NPC is hit by the spin attack, increase knockback slightly
            // 如果 NPC 被旋转攻击命中，则'略微'增加击退力量
            if (CurrentAttack == AttackType.Spin)
                modifiers.Knockback += 10;
        }

        // Function to easily set projectile and arm position
        // 方便设置投射物旋转和手臂位置的函数
        public void SetSwordPosition()
        {
            Projectile.rotation = InitialAngle + Projectile.spriteDirection * Progress; // Set projectile rotation
                                                                                        // 设置投射物旋转

            // Set composite arm allows you to set the rotation of the arm and stretch of the front and back arms independently
            // 设置复合手臂允许您独立设置手臂的旋转以及前后手臂的伸展程度
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f)); // set arm position (90 degree offset since arm starts lowered)
                                                                                                                                      // 设置手臂位置（由于手臂开始时处于下降状态，因此需要进行90度偏移）

            Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)Math.PI / 2); // get position of hand
                                                                                                                                               // 获取手的位置

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
        /// <summary>
        /// 【促进拔剑动作的函数】
        /// </summary>
        private void PrepareStrike()
        {
            Progress = WINDUP * SWINGRANGE * (1f - Timer / PrepTime); // Calculates rotation from initial angle
                                                                      // 从初始角度计算旋转

            Size = MathHelper.SmoothStep(0, 1, Timer / PrepTime); // Make sword slowly increase in size as we prepare to strike until it reaches max
                                                                  // 使剑慢慢增大，直到达到最大值，准备攻击

            if (Timer >= PrepTime)// 【我们不希望 (Timer / PrepTime) >1 ,这会使武器挥舞起来很荒谬，所以在这里我们切换下一阶段】
            {
                SoundEngine.PlaySound(SoundID.Item1); // Play sword sound here since playing it on spawn is too early
                                                      // 在这里播放剑声音，因为在生成时播放太早了

                CurrentStage = AttackStage.Execute; // If attack is over prep time, we go to next stage
                                                    // 如果攻击时间超过准备时间，则进入下一阶段
            }
        }

        // Function facilitating the first half of the swing
        // 促进挥舞动作前半部分的函数
        /// <summary>
        /// 【促进挥舞动作前半部分的函数】
        /// </summary>
        private void ExecuteStrike()
        {
            if (CurrentAttack== AttackType.Swing)
            {
                Progress = MathHelper.SmoothStep(0, SWINGRANGE, (1f - UNWIND) * Timer / ExecTime);

                if (Timer >= ExecTime)
                {
                    CurrentStage = AttackStage.Unwind;
                }
            }

            //【这里便是旋转大风车式攻击的实现】
            else
            {
                Progress = MathHelper.SmoothStep(0, SPINRANGE, (1f - UNWIND / 2) * Timer / (ExecTime * SPINTIME));

                if (Timer == (int)(ExecTime * SPINTIME * 3 / 4))
                {
                    SoundEngine.PlaySound(SoundID.Item1); // Play sword sound again
                                                          // 再次播放剑声音

                    Projectile.ResetLocalNPCHitImmunity(); // Reset the local npc hit immunity for second half of spin
                                                           // 重置第二个半圈中本地 NPC 的受击无敌帧
                }

                if (Timer >= ExecTime * SPINTIME)
                {
                    CurrentStage = AttackStage.Unwind;

                }
            }
        }

        // Function facilitating the latter half of the swing where the sword disappears
        // 促进挥舞动作后半部分并隐藏剑的函数
        /// <summary>
        /// 【促进挥舞动作后半部分并隐藏剑的函数】
        /// </summary>
        private void UnwindStrike()
        {
            if (CurrentAttack == AttackType.Swing)
            {
                Progress = MathHelper.SmoothStep(0, SWINGRANGE, 1f - UNWIND + UNWIND * Timer / HideTime);
                Size = 1f - MathHelper.SmoothStep(0, 1, Timer / HideTime); // Make sword slowly decrease in size as we end the swing to make a smooth hiding animation
                                                                           // 使剑缓慢缩小，以进行平滑隐藏动画

                if (Timer >= HideTime)
                {
                    Projectile.Kill();
                }
            }
            else
            {
                Progress = MathHelper.SmoothStep(0, SPINRANGE, 1f - UNWIND / 2 + UNWIND / 2 * Timer / (HideTime * SPINTIME / 2));
                Size = 1f - MathHelper.SmoothStep(0, 1, Timer / (HideTime * SPINTIME / 2));

                if (Timer >= HideTime * SPINTIME / 2)
                {
                    Projectile.Kill();
                }
            }
        }
    }
}