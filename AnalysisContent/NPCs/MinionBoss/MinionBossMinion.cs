using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.NPCs.MinionBoss
{
    // The minions spawned when the body spawns
    // Please read MinionBossBody.cs first for important comments, they won't be explained here again

    // 当主体生成时产生小兵
    // 请先阅读MinionBossBody.cs中的重要注释，这里不再解释
    public class MinionBossMinion : ModNPC
    {
        // This is a neat trick that uses the fact that NPCs have all NPC.ai[] values set to 0f on spawn (if not otherwise changed).
        // We set ParentIndex to a number in the body after spawning it. If we set ParentIndex to 3, NPC.ai[0] will be 4. If NPC.ai[0] is 0, ParentIndex will be -1.
        // Now combine both facts, and the conclusion is that if this NPC spawns by other means (not from the body), ParentIndex will be -1, allowing us to distinguish
        // between a proper spawn and an invalid/"cheated" spawn

        // 这是一个巧妙的技巧，利用了NPC在生成时所有NPC.ai[]值都设置为0f（如果没有被修改）的事实。
        // 我们在生成后将ParentIndex设置为身体中的数字。如果我们将ParentIndex设置为3，则NPC.ai[0]将为4。如果NPC.ai[0]为0，则ParentIndex将为-1。
        // 现在结合两个事实，得出结论：如果此NPC通过其他方式生成（而不是从身体中），则ParentIndex将为-1，使我们能够区分
        // 正确的刷怪和无效/“作弊”的刷怪。
        public int ParentIndex
        {
            get => (int)NPC.ai[0] - 1;
            set => NPC.ai[0] = value + 1;
        }

        public bool HasParent => ParentIndex > -1;

        public float PositionOffset
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }

        public const float RotationTimerMax = 360;
        public ref float RotationTimer => ref NPC.ai[2];

        // Helper method to determine the body type
        // 辅助方法以确定身体类型
        public static int BodyType()
        {
            return ModContent.NPCType<MinionBossBody>();
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 1;

            // By default enemies gain health and attack if hardmode is reached. this NPC should not be affected by that
            // 默认情况下，敌人会随着困难模式到来而获得健康和攻击力提升。但该NPC不应受此影响。
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            // Enemies can pick up coins, let's prevent it for this NPC
            // 敌人可以拾取硬币，请防止其对该NPC进行拾取
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            // Automatically group with other bosses
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            // Specify the debuffs it is immune to
            // 指定它免疫哪些debuffs
            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Poisoned,

                    BuffID.Confused // Most NPCs have this
                                    // 大多数NPC都有这个功能
				}
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);

            // Optional: If you don't want this NPC to show on the bestiary (if there is no reason to show a boss minion separately)
            // Make sure to remove SetBestiary code aswell

            // 可选项：如果您不想让此 NPC 显示在图鉴上（如果没有理由单独显示 boss minion）
            // 一定要删除 SetBestiary 代码

            // NPCID.Sets.NPCBestiaryDrawModifiers bestiaryData = new NPCID.Sets.NPCBestiaryDrawModifiers(0) {
            //	Hide = true // Hides this NPC from the bestiary
            // };
            // NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, bestiaryData);
        }

        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 30;
            NPC.damage = 7;
            NPC.defense = 0;
            NPC.lifeMax = 50;
            NPC.HitSound = SoundID.NPCHit9;
            NPC.DeathSound = SoundID.NPCDeath11;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0.8f;
            NPC.alpha = 255; // This makes it transparent upon spawning, we have to manually fade it in in AI()
                             // 这使其在生成时透明，我们必须在AI()中手动淡入它
            NPC.netAlways = true;

            NPC.aiStyle = -1;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // Makes it so whenever you beat the boss associated with it, it will also get unlocked immediately
            // 这样每当你打败与之关联的boss时，它也会立即解锁
            int associatedNPCType = BodyType();
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Plain black background
                                                                             // 纯黑色背景

				new FlavorTextBestiaryInfoElement("A minion protecting his boss from taking damage by sacrificing itself. If none are alive, the boss is exposed to damage.")
            });
        }

        public override Color? GetAlpha(Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                // This is required because we have NPC.alpha = 255, in the bestiary it would look transparent
                // 这是必需的，因为我们有NPC.alpha = 255，在图鉴中它看起来会变成透明的。
                return NPC.GetBestiaryEntryColor();
            }
            return Color.White * NPC.Opacity;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = ImmunityCooldownID.Bosses; // use the boss immunity cooldown counter, to prevent ignoring boss attacks by taking damage from other sources
                                                      // 使用 boss 免疫冷却计数器，以防止通过从其他来源受到伤害而忽略 boss 攻击
            return true;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                // If this NPC dies, spawn some visuals
                // 如果此 NPC 死亡，则产生一些视觉效果。

                int dustType = 59; // Some blue dust, read the dust guide on the wiki for how to find the perfect dust
                                   // 一些蓝色尘土，请参阅维基上的尘土指南以找到完美的尘土。

                for (int i = 0; i < 20; i++)
                {
                    Vector2 velocity = NPC.velocity + new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));
                    Dust dust = Dust.NewDustPerfect(NPC.Center, dustType, velocity, 26, Color.White, Main.rand.NextFloat(1.5f, 2.4f));

                    dust.noLight = true;
                    dust.noGravity = true;
                    dust.fadeIn = Main.rand.NextFloat(0.3f, 0.8f);
                }
            }
        }

        public override void AI()
        {
            if (Despawn())
            {
                return;
            }

            FadeIn();

            MoveInFormation();
        }

        private bool Despawn()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient &&
                (!HasParent || !Main.npc[ParentIndex].active || Main.npc[ParentIndex].type != BodyType()))
            {
                // * Not spawned by the boss body (didn't assign a position and parent) or
                // * Parent isn't active or
                // * Parent isn't the body
                // => invalid, kill itself without dropping any items

                //* 没有由boss body生成（没有分配位置和父级）或
                //* 父级不活跃或
                //* 父级不是身体
                //=>无效，杀死自己而不掉落任何物品
                NPC.active = false;
                NPC.life = 0;
                NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                return true;
            }
            return false;
        }

        private void FadeIn()
        {
            // Fade in (we have NPC.alpha = 255 in SetDefaults which means it spawns transparent)
            // 淡入（我们在SetDefaults中具有NPC.alpha = 255，这意味着它生成时是透明的）
            if (NPC.alpha > 0)
            {
                NPC.alpha -= 10;
                if (NPC.alpha < 0)
                {
                    NPC.alpha = 0;
                }
            }
        }

        private void MoveInFormation()
        {
            NPC parentNPC = Main.npc[ParentIndex];

            // This basically turns the NPCs PositionIndex into a number between 0f and TwoPi to determine where around
            // the main body it is positioned at

            // 基本上将NPC PositionIndex转换为0f至TwoPi之间的数字，
            // 以确定其围绕主体所处位置。
            float rad = (float)PositionOffset * MathHelper.TwoPi;

            // Add some slight uniform rotation to make the eyes move, giving a chance to touch the player and thus helping melee players
            // 添加一些轻微均匀旋转使眼睛移动，并给近战玩家提供接触机会。
            RotationTimer += 0.5f;
            if (RotationTimer > RotationTimerMax)
            {
                RotationTimer = 0;
            }

            // Since RotationTimer is in degrees (0..360) we can convert it to radians (0..TwoPi) easily
            // 由于RotationTimer是度数（0..360），因此可以轻松地将其转换为弧度（0..TwoPi）
            float continuousRotation = MathHelper.ToRadians(RotationTimer);
            rad += continuousRotation;
            if (rad > MathHelper.TwoPi)
            {
                rad -= MathHelper.TwoPi;
            }
            else if (rad < 0)
            {
                rad += MathHelper.TwoPi;
            }

            float distanceFromBody = parentNPC.width + NPC.width;

            // offset is now a vector that will determine the position of the NPC based on its index
            // 偏移现在是一个向量，它将根据其索引确定NPC的位置。
            Vector2 offset = Vector2.One.RotatedBy(rad) * distanceFromBody;

            Vector2 destination = parentNPC.Center + offset;
            Vector2 toDestination = destination - NPC.Center;
            Vector2 toDestinationNormalized = toDestination.SafeNormalize(Vector2.Zero);

            float speed = 8f;
            float inertia = 20;

            Vector2 moveTo = toDestinationNormalized * speed;
            NPC.velocity = (NPC.velocity * (inertia - 1) + moveTo) / inertia;
        }
    }
}
