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

    // ����ļ��������ٻ������������д���
    // - ModItem - �����ٻ���ӵ�����
    // - ModBuff - ���ͼ������ʧ��ӵ�״̬��
    // - ModProjectile - ��ӱ���

    // �����齫������Щ�����ͬһ���ļ��С�Ϊ����ʾ���㣬���Ƕ���ѹ����һ���Ա������õظ�����
    // Ҫ���õ�������ʹ����Эͬ����������д��� AI�����Ķ�ָ�ϣ�https://github.com/tModLoader/tModLoader/wiki/Basic-Minion-Guide
    // �ⲻ������߼���� AI ��ָ�ϡ�

    public class AnalysisSimpleMinionBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true; // This buff won't save when you exit the world
                                          // ��״̬�����˳�����󲻻ᱣ��

            Main.buffNoTimeDisplay[Type] = true; // The time remaining won't display on this buff
                                                 // ʣ��ʱ�䲻����ʾ�ڴ�״̬����
        }

        public override void Update(Player player, ref int buffIndex)
        {
            // If the minions exist reset the buff time, otherwise remove the buff from the player
            // ������ٻ�����ڣ������������ʱ�䣻�������Ƴ�������ϡ�
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
                                                                      // ���������ʹ�ÿ�����ʱ��׼������Ļ����λ�á�
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;

            ItemID.Sets.StaffMinionSlotsRequired[Type] = 1f; // The default value is 1, but other values are supported. See the docs for more guidance.
                                                             // Ĭ��ֵΪ 1����֧������ֵ��������ĵ���ȡ����ָ����Ϣ��
        }

        public override void SetDefaults()
        {
            Item.damage = 30;
            Item.knockBack = 3f;
            Item.mana = 10; // mana cost
                            // ��������
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Swing; // how the player's arm moves when using the item
                                                  // ��ʹ�ø���Ʒʱ������ֱ�Ӧ����ƶ���

            Item.value = Item.sellPrice(gold: 30);
            Item.rare = ItemRarityID.Cyan;
            Item.UseSound = SoundID.Item44; // What sound should play when using the item
                                            // ʹ�ø���ƷʱӦ��������������

            // These below are needed for a minion weapon:
            // ������Щ�������ٻ�����������ģ�

            Item.noMelee = true; // this item doesn't do any melee damage
                                 // ����Ʒû�н�ս�˺�

            Item.DamageType = DamageClass.Summon; // Makes the damage register as summon. If your item does not have any damage type, it becomes true damage (which means that damage scalars will not affect it). Be sure to have a damage type
                                                  // ʹ�˺�ע��Ϊ�ٻ������������Ʒû���κ��˺����ͣ����Ϊ��ʵ�˺�������ζ���˺���������Ӱ���䣩����ȷ����һ���˺����͡�

            Item.buffType = ModContent.BuffType<AnalysisSimpleMinionBuff>();
            // No buffTime because otherwise the item tooltip would say something like "1 minute duration"
            // û�� buffTime���������Ʒ�Ĺ�����ʾ����ʾ��1 ���ӳ���ʱ�䡱��
            Item.shoot = ModContent.ProjectileType<AnalysisSimpleMinion>(); // This item creates the minion projectile
                                                                            // ����Ʒ���������Ͷ���
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            // Here you can change where the minion is spawned. Most vanilla minions spawn at the cursor position
            // ����������Ը���������ɵ�λ�á������������Ϸ�е���Ӷ��ڹ��λ�����ɡ�
            position = Main.MouseWorld;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
            // ���Ǳ���ģ��Ա�Ӧ�ÿ�����������Ӵ���ȷ��ʧ��״̬��
            player.AddBuff(Item.buffType, 2);

            // Minions have to be spawned manually, then have originalDamage assigned to the damage of the summon item
            // ��ӱ����ֶ����ɣ�Ȼ�� originalDamage ������ٻ����ߵ��˺�ֵ
            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
            projectile.originalDamage = Item.damage;

            // Since we spawned the projectile manually already, we do not need the game to spawn it for ourselves anymore, so return false
            // ���������Ѿ��ֶ�������Ͷ�����˲���Ҫ������Ϸ�Զ������ˣ��뷵�� false��
            return false;
        }

        // Please see Content/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        // �й������䷽��ϸ˵������μ� Content/AnalysisRecipes.cs �ļ���
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

    // �����չʾ��һЩǿ��Ҫ����ʹ���������С�
    // ���򵥵ع�����������˾���С�� 43 �������Ŀ�겢��ɽӴ��˺�
    // �������Ҽ����ĳ�� NPC���򴩹�ǽ�ڷ���� NPC
    // ���δ���й������򿿽����Ư���������������ƶ�
    public class AnalysisSimpleMinion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // Sets the amount of frames this minion has on its spritesheet
            // ���ø�������侫����ϵ�֡��
            Main.projFrames[Projectile.type] = 4;
            // This is necessary for right-click targeting
            // ������Ҽ�Ŀ���Ǳ�Ҫ��
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

            Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion
                                                  // ��ʾ�����������һ����������

            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
                                                                          // ����Ϊ��ʹ�������ܹ����ٻ�ʱ��ȷ���ɣ�����������ӱ��ٻ�ʱ�滻��

            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
                                                                            // ��а��ͽ�Դ���������п��ԣ���Ϊ�������и��ٵ����п��ԡ�
        }

        public sealed override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 28;
            Projectile.tileCollide = false; // Makes the minion go through tiles freely
                                            // ʹ�ø���ӿ������ɴ�����ש

            // These below are needed for a minion weapon
            // ����������������һ���������
            Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
                                        // �������Ƿ�����˽Ӵ�����˺����Ժ����ϸ���ܣ�

            Projectile.minion = true; // Declares this as a minion (has many effects)
                                      // ��������Ϊһ����ӣ����ж���Ч����

            Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
                                                        // �����˺����ͣ���Ҫ����������˺���

            Projectile.minionSlots = 1f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
                                         // �����ռ����ҿ����ܹ������еĲ��������Ժ����ϸ���ܣ�

            Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
                                       // ��Ҫȷ��������˻�ؿ���ײʱ������ʧ������ù�����ʧ��
        }

        // Here you can decide if your minion breaks things like grass or pots
        // �����������Ծ�������С���Ƿ���Ʋݻ����֮��Ķ���
        public override bool? CanCutTiles()
        {
            return false;
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        // �������С����ɽӴ��˺��������ִ�д˲�����Movement������AI()�����Ϣ���ࣩ��
        public override bool MinionContactDamage()
        {
            return true;
        }

        // The AI of this minion is split into multiple methods to avoid bloat. This method just passes values between calls actual parts of the AI.
        // ��С��AI�ֽ�Ϊ��������Ա���ӷ�ס��˷���ֻ���ڵ���ʵ��AI����֮�䴫��ֵ��
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
        // ���ǡ����顱��ȷ����Ҵ��ʱ���Ҳ�����ڲ����ʱ��ʧ
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
                                   // �����ƶ�48�����꣨�����������������ש��

            // If your minion doesn't aimlessly move around when it's idle, you need to "put" it into the line of other summoned minions
            // The index is projectile.minionPos

            // �������С�����ڿ���״̬����������Ŀ�ĵ��ƶ�������Ҫ������������ٻ�С����������
            // ����Ϊprojectile.minionPos
            float minionPositionOffsetX = (10 + Projectile.minionPos * 40) * -owner.direction;
            idlePosition.X += minionPositionOffsetX; // Go behind the player
                                                     // ��ʾ����Һ���

            // All of this code below this line is adapted from Spazmamini code (ID 388, aiStyle 66)
            // �������д��붼�ı���Spazmamini���루ID 388��aiStyle 66��

            // Teleport to player if distance is too big
            // �������̫Զ���봫�͵�������
            vectorToIdlePosition = idlePosition - Projectile.Center;
            distanceToIdlePosition = vectorToIdlePosition.Length();

            if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 2000f)
            {
                // Whenever you deal with non-regular events that change the behavior or position drastically, make sure to only run the code on the owner of the projectile,
                // and then set netUpdate to true

                // ÿ������ǳ����¼�����Ϊ��λ�÷��������仯ʱ����ȷ���������������������иô��룬
                // Ȼ��netUpdate����Ϊtrue��
                Projectile.position = idlePosition;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }

            // If your minion is flying, you want to do this independently of any conditions
            // �������С�����ڷ��У���ϣ���������κ�����ִ�д˲�����
            float overlapVelocity = 0.04f;

            // Fix overlap with other minions
            // �������������ص�����
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
            // ��ʼ��������
            distanceFromTarget = 700f;
            targetCenter = Projectile.position;
            foundTarget = false;

            // This code is required if your minion weapon has the targeting feature
            // �����������������ж�λ���ܣ�����Ҫ�˴���
            if (owner.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[owner.MinionAttackTargetNPC];
                float between = Vector2.Distance(npc.Center, Projectile.Center);

                // Reasonable distance away so it doesn't target across multiple screens
                // ����ľ��룬�Ա��������ڶ����Ļ�Ͻ��ж�λ
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
                // ������ζ���Ҫ��δ��룬���ڲ���Ŀ��
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

                        // ����ض������Ϊ�Ķ����飬����һ����Խ��ש��ͻ�ֹͣ����
                        // ����ȡ���������˶������п����ĸ��ֲ��������Բ�ͬ����ֱ������������
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

            // friendly ����Ϊ true ��ʹ����ܹ���ɽӴ��˺�
            // friendly ����Ϊ false ��ʹ���ڿ���ʱ�����������֮��Ķ�����
            // ����������ȡ�����Ƿ���Ŀ�꣬�������ֻ��һ����ֵ������
            // �����������������������ɽӴ��˺�������Ҫ�˸�ֵ������
            Projectile.friendly = foundTarget;
        }

        private void Movement(bool foundTarget, float distanceFromTarget, Vector2 targetCenter, float distanceToIdlePosition, Vector2 vectorToIdlePosition)
        {
            // Default movement parameters (here for attacking)
            // Ĭ���ƶ����������ڹ�����
            float speed = 8f;
            float inertia = 20f;

            if (foundTarget)
            {
                // Minion has a target: attack (here, fly towards the enemy)
                // С����һ��Ŀ�꣺���������������˷�ȥ��
                if (distanceFromTarget > 40f)
                {
                    // The immediate range around the target (so it doesn't latch onto it when close)
                    // Ŀ����Χ��ʱ��Χ����˵�����ʱ�����ḽ����Ŀ���ϣ�
                    Vector2 direction = targetCenter - Projectile.Center;
                    direction.Normalize();
                    direction *= speed;

                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
                }
            }
            else
            {
                // Minion doesn't have a target: return to player and idle
                // ���û��Ŀ�꣺������Ҳ����ڿ���״̬
                if (distanceToIdlePosition > 600f)
                {
                    // Speed up the minion if it's away from the player
                    // ���Զ����ң����������ٶ�
                    speed = 12f;
                    inertia = 60f;
                }
                else
                {
                    // Slow down the minion if closer to the player
                    // ���������ң����������ٶ�
                    speed = 4f;
                    inertia = 80f;
                }

                if (distanceToIdlePosition > 20f)
                {
                    // The immediate range around the player (when it passively floats about)
                    // �����Χ��ֱ�ӷ�Χ������������Ư��ʱ��

                    // This is a simple movement formula using the two parameters and its desired direction to create a "homing" movement
                    // ����һ���򵥵��ƶ���ʽ��ʹ�����������������跽����������׷�١��˶�
                    vectorToIdlePosition.Normalize();
                    vectorToIdlePosition *= speed;
                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
                }
                else if (Projectile.velocity == Vector2.Zero)
                {
                    // If there is a case where it's not moving at all, give it a little "poke"
                    // �����һ����������������������һ��С���ơ�
                    Projectile.velocity.X = -0.15f;
                    Projectile.velocity.Y = -0.05f;
                }
            }
        }

        private void Visuals()
        {
            // So it will lean slightly towards the direction it's moving
            // ���������΢��б�����������ƶ��ķ���
            Projectile.rotation = Projectile.velocity.X * 0.05f;

            // This is a simple "loop through all frames from top to bottom" animation
            // ����һ���򵥵ġ����ϵ���ѭ����������֡���Ķ�����
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
            // һЩ�Ӿ�Ч��
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }
    }
}
