using System;
using AnalysisMod.AnalysisContent.Items;
using AnalysisMod.AnalysisContent.Tiles.Furniture;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Projectiles.Minions
{
    // This file contains all the code necessary for a minion
    // - ModItem - the weapon which you use to summon the minion with
    // - ModBuff - the icon you can click on to despawn the minion
    // - ModProjectile - the minion itself

    // It is not recommended to put all these classes in the same file. For demonstrations sake they are all compacted together so you get a better overwiew.
    // To get a better understanding of how everything works together, and how to code minion AI, read the guide: https://github.com/tModLoader/tModLoader/wiki/Basic-Minion-Guide
    // This is NOT an in-depth guide to advanced minion AI

    // 这个文件包含了召唤随从所需的所有代码
    // - ModItem - 用于召唤随从的武器
    // - ModBuff - 点击图标以消失随从的状态栏
    // - ModProjectile - 随从本身

    // 不建议将所有这些类放在同一个文件中。为了演示方便，它们都被压缩到一起，以便您更好地概览。
    // 要更好地理解如何使它们协同工作，并编写随从 AI，请阅读指南：https://github.com/tModLoader/tModLoader/wiki/Basic-Minion-Guide
    // 这不是深入高级随从 AI 的指南。

    public class AnalysisSimpleMinionBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true; // This buff won't save when you exit the world
                                          // 此状态栏在退出世界后不会保存

            Main.buffNoTimeDisplay[Type] = true; // The time remaining won't display on this buff
                                                 // 剩余时间不会显示在此状态栏上
        }

        public override void Update(Player player, ref int buffIndex)
        {
            // If the minions exist reset the buff time, otherwise remove the buff from the player
            // 如果有召唤物存在，则重置其持续时间；否则将其移除玩家身上。
            if (player.ownedProjectileCounts[ModContent.ProjectileType<AnalysisSimpleMinion>()] > 0)
            {
                player.buffTime[buffIndex] = 18000;
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }

    public class AnalysisSimpleMinionItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true; // This lets the player target anywhere on the whole screen while using a controller
                                                                      // 这允许玩家使用控制器时瞄准整个屏幕任意位置。
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;

            ItemID.Sets.StaffMinionSlotsRequired[Type] = 1f; // The default value is 1, but other values are supported. See the docs for more guidance.
                                                             // 默认值为 1，但支持其他值。请参阅文档获取更多指导信息。
        }

        public override void SetDefaults()
        {
            Item.damage = 30;
            Item.knockBack = 3f;
            Item.mana = 10; // mana cost
                            // 法力消耗
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Swing; // how the player's arm moves when using the item
                                                  // 当使用该物品时，玩家手臂应如何移动？

            Item.value = Item.sellPrice(gold: 30);
            Item.rare = ItemRarityID.Cyan;
            Item.UseSound = SoundID.Item44; // What sound should play when using the item
                                            // 使用该物品时应播放哪种声音？

            // These below are needed for a minion weapon:
            // 下面这些内容是召唤武器所必需的：

            Item.noMelee = true; // this item doesn't do any melee damage
                                 // 此物品没有近战伤害

            Item.DamageType = DamageClass.Summon; // Makes the damage register as summon. If your item does not have any damage type, it becomes true damage (which means that damage scalars will not affect it). Be sure to have a damage type
                                                  // 使伤害注册为召唤。如果您的物品没有任何伤害类型，则成为真实伤害（这意味着伤害标量不会影响其）。请确保有一个伤害类型。

            Item.buffType = ModContent.BuffType<AnalysisSimpleMinionBuff>();
            // No buffTime because otherwise the item tooltip would say something like "1 minute duration"
            // 没有 buffTime，否则该物品的工具提示将显示“1 分钟持续时间”。
            Item.shoot = ModContent.ProjectileType<AnalysisSimpleMinion>(); // This item creates the minion projectile
                                                                            // 此物品创建了随从投射物。
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            // Here you can change where the minion is spawned. Most vanilla minions spawn at the cursor position
            // 在这里，您可以更改随从生成的位置。大多数基础游戏中的随从都在光标位置生成。
            position = Main.MouseWorld;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
            // 这是必需的，以便应用可让您保持随从存活并正确消失的状态栏
            player.AddBuff(Item.buffType, 2);

            // Minions have to be spawned manually, then have originalDamage assigned to the damage of the summon item
            // 随从必须手动生成，然后将 originalDamage 分配给召唤道具的伤害值
            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
            projectile.originalDamage = Item.damage;

            // Since we spawned the projectile manually already, we do not need the game to spawn it for ourselves anymore, so return false
            // 由于我们已经手动生成了投射物，因此不需要再由游戏自动生成了，请返回 false。
            return false;
        }

        // Please see Content/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        // 有关制作配方详细说明，请参见 Content/AnalysisRecipes.cs 文件。
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<AnalysisItem>())
                .AddTile(ModContent.TileType<AnalysisWorkbench>())
                .Register();
        }
    }

    // This minion shows a few mandatory things that make it behave properly.
    // Its attack pattern is simple: If an enemy is in range of 43 tiles, it will fly to it and deal contact damage
    // If the player targets a certain NPC with right-click, it will fly through tiles to it
    // If it isn't attacking, it will float near the player with minimal movement

    // 此随从展示了一些强制要求以使其正常运行。
    // 它简单地攻击：如果敌人距离小于 43 格，则飞向目标并造成接触伤害
    // 如果玩家右键点击某个 NPC，则穿过墙壁飞向该 NPC
    // 如果未进行攻击，则靠近玩家漂浮，并做出最少移动
    public class AnalysisSimpleMinion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // Sets the amount of frames this minion has on its spritesheet
            // 设置该随从在其精灵表上的帧数
            Main.projFrames[Projectile.type] = 4;
            // This is necessary for right-click targeting
            // 这对于右键目标是必要的
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

            Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion
                                                  // 表示这个抛射物是一个宠物或随从

            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
                                                                          // 这是为了使你的随从能够在召唤时正确生成，并在其他随从被召唤时替换。

            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
                                                                            // 让邪教徒对此抛射物具有抗性，因为它对所有跟踪弹具有抗性。
        }

        public sealed override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 28;
            Projectile.tileCollide = false; // Makes the minion go through tiles freely
                                            // 使得该随从可以自由穿过瓷砖

            // These below are needed for a minion weapon
            // 以下内容用于描述一个随从武器
            Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
                                        // 仅控制是否与敌人接触造成伤害（稍后会详细介绍）

            Projectile.minion = true; // Declares this as a minion (has many effects)
                                      // 声明此项为一种随从（具有多种效果）

            Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
                                                        // 声明伤害类型（需要它本身造成伤害）

            Projectile.minionSlots = 1f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
                                         // 该随从占据玩家可用总共数量中的槽数量（稍后会详细介绍）

            Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
                                       // 需要确保当与敌人或地块碰撞时不会消失，以免该怪物消失。
        }

        // Here you can decide if your minion breaks things like grass or pots
        // 在这里您可以决定您的小兵是否打破草或罐子之类的东西
        public override bool? CanCutTiles()
        {
            return false;
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        // 如果您的小兵造成接触伤害，则必须执行此操作（Movement区域中AI()相关信息更多）。
        public override bool MinionContactDamage()
        {
            return true;
        }

        // The AI of this minion is split into multiple methods to avoid bloat. This method just passes values between calls actual parts of the AI.
        // 此小兵AI分解为多个方法以避免臃肿。此方法只是在调用实际AI部分之间传递值。
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            if (!CheckActive(owner))
            {
                return;
            }

            GeneralBehavior(owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
            SearchForTargets(owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
            Movement(foundTarget, distanceFromTarget, targetCenter, distanceToIdlePosition, vectorToIdlePosition);
            Visuals();
        }

        // This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
        // 这是“活动检查”，确保玩家存活时随从也存活，并在不存活时消失
        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<AnalysisSimpleMinionBuff>());

                return false;
            }

            if (owner.HasBuff(ModContent.BuffType<AnalysisSimpleMinionBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            return true;
        }

        private void GeneralBehavior(Player owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition)
        {
            Vector2 idlePosition = owner.Center;
            idlePosition.Y -= 48f; // Go up 48 coordinates (three tiles from the center of the player)
                                   // 向上移动48个坐标（距离玩家中心三个瓷砖）

            // If your minion doesn't aimlessly move around when it's idle, you need to "put" it into the line of other summoned minions
            // The index is projectile.minionPos

            // 如果您的小兵处于空闲状态而不会漫无目的地移动，则需要将其放入其他召唤小兵的行列中
            // 索引为projectile.minionPos
            float minionPositionOffsetX = (10 + Projectile.minionPos * 40) * -owner.direction;
            idlePosition.X += minionPositionOffsetX; // Go behind the player
                                                     // 显示在玩家后面

            // All of this code below this line is adapted from Spazmamini code (ID 388, aiStyle 66)
            // 以下所有代码都改编自Spazmamini代码（ID 388，aiStyle 66）

            // Teleport to player if distance is too big
            // 如果距离太远，请传送到玩家身边
            vectorToIdlePosition = idlePosition - Projectile.Center;
            distanceToIdlePosition = vectorToIdlePosition.Length();

            if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 2000f)
            {
                // Whenever you deal with non-regular events that change the behavior or position drastically, make sure to only run the code on the owner of the projectile,
                // and then set netUpdate to true

                // 每当处理非常规事件且行为或位置发生显著变化时，请确保仅对抛射物所有者运行该代码，
                // 然后将netUpdate设置为true。
                Projectile.position = idlePosition;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }

            // If your minion is flying, you want to do this independently of any conditions
            // 如果您的小兵正在飞行，则希望独立于任何条件执行此操作。
            float overlapVelocity = 0.04f;

            // Fix overlap with other minions
            // 解决与其他随从重叠问题
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile other = Main.projectile[i];

                if (i != Projectile.whoAmI && other.active && other.owner == Projectile.owner && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
                {
                    if (Projectile.position.X < other.position.X)
                    {
                        Projectile.velocity.X -= overlapVelocity;
                    }
                    else
                    {
                        Projectile.velocity.X += overlapVelocity;
                    }

                    if (Projectile.position.Y < other.position.Y)
                    {
                        Projectile.velocity.Y -= overlapVelocity;
                    }
                    else
                    {
                        Projectile.velocity.Y += overlapVelocity;
                    }
                }
            }
        }

        private void SearchForTargets(Player owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter)
        {
            // Starting search distance
            // 开始搜索距离
            distanceFromTarget = 700f;
            targetCenter = Projectile.position;
            foundTarget = false;

            // This code is required if your minion weapon has the targeting feature
            // 如果您的随从武器具有定位功能，则需要此代码
            if (owner.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[owner.MinionAttackTargetNPC];
                float between = Vector2.Distance(npc.Center, Projectile.Center);

                // Reasonable distance away so it doesn't target across multiple screens
                // 合理的距离，以便它不会在多个屏幕上进行定位
                if (between < 2000f)
                {
                    distanceFromTarget = between;
                    targetCenter = npc.Center;
                    foundTarget = true;
                }
            }

            if (!foundTarget)
            {
                // This code is required either way, used for finding a target
                // 无论如何都需要这段代码，用于查找目标
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];

                    if (npc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(npc.Center, Projectile.Center);
                        bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
                        // Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
                        // The number depends on various parameters seen in the movement code below. Test different ones out until it works alright

                        // 针对特定随从行为的额外检查，否则一旦飞越瓷砖后就会停止攻击
                        // 数字取决于下面运动代码中看到的各种参数。尝试不同数字直到正常工作。
                        bool closeThroughWall = between < 100f;

                        if ((closest && inRange || !foundTarget) && (lineOfSight || closeThroughWall))
                        {
                            distanceFromTarget = between;
                            targetCenter = npc.Center;
                            foundTarget = true;
                        }
                    }
                }
            }

            // friendly needs to be set to true so the minion can deal contact damage
            // friendly needs to be set to false so it doesn't damage things like target dummies while idling
            // Both things depend on if it has a target or not, so it's just one assignment here
            // You don't need this assignment if your minion is shooting things instead of dealing contact damage

            // friendly 设置为 true 以使随从能够造成接触伤害
            // friendly 设置为 false 以使其在空闲时不会损坏像靶子之类的东西。
            // 这两件事情取决于是否有目标，因此这里只是一个赋值操作。
            // 如果您的随从正在射击而非造成接触伤害，则不需要此赋值操作。
            Projectile.friendly = foundTarget;
        }

        private void Movement(bool foundTarget, float distanceFromTarget, Vector2 targetCenter, float distanceToIdlePosition, Vector2 vectorToIdlePosition)
        {
            // Default movement parameters (here for attacking)
            // 默认移动参数（用于攻击）
            float speed = 8f;
            float inertia = 20f;

            if (foundTarget)
            {
                // Minion has a target: attack (here, fly towards the enemy)
                // 小兵有一个目标：攻击（在这里，向敌人飞去）
                if (distanceFromTarget > 40f)
                {
                    // The immediate range around the target (so it doesn't latch onto it when close)
                    // 目标周围即时范围（因此当靠近时它不会附着在目标上）
                    Vector2 direction = targetCenter - Projectile.Center;
                    direction.Normalize();
                    direction *= speed;

                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
                }
            }
            else
            {
                // Minion doesn't have a target: return to player and idle
                // 随从没有目标：返回玩家并处于空闲状态
                if (distanceToIdlePosition > 600f)
                {
                    // Speed up the minion if it's away from the player
                    // 如果远离玩家，则加速随从速度
                    speed = 12f;
                    inertia = 60f;
                }
                else
                {
                    // Slow down the minion if closer to the player
                    // 如果靠近玩家，则减慢随从速度
                    speed = 4f;
                    inertia = 80f;
                }

                if (distanceToIdlePosition > 20f)
                {
                    // The immediate range around the player (when it passively floats about)
                    // 玩家周围的直接范围（当它被动地漂浮时）

                    // This is a simple movement formula using the two parameters and its desired direction to create a "homing" movement
                    // 这是一个简单的移动公式，使用两个参数和其所需方向来创建“追踪”运动
                    vectorToIdlePosition.Normalize();
                    vectorToIdlePosition *= speed;
                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
                }
                else if (Projectile.velocity == Vector2.Zero)
                {
                    // If there is a case where it's not moving at all, give it a little "poke"
                    // 如果有一种情况它根本不动，则给它一个小“推”
                    Projectile.velocity.X = -0.15f;
                    Projectile.velocity.Y = -0.05f;
                }
            }
        }

        private void Visuals()
        {
            // So it will lean slightly towards the direction it's moving
            // 因此它会稍微倾斜朝着它正在移动的方向
            Projectile.rotation = Projectile.velocity.X * 0.05f;

            // This is a simple "loop through all frames from top to bottom" animation
            // 这是一个简单的“从上到下循环遍历所有帧”的动画。
            int frameSpeed = 5;

            Projectile.frameCounter++;

            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }

            // Some visuals here
            // 一些视觉效果
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }
    }
}
