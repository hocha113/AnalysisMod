using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.NPCs
{
    // These three class showcase usage of the WormHead, WormBody and WormTail classes from Worm.cs
    // 这三个类展示了Worm.cs中WormHead、WormBody和WormTail类的用法
    internal class AnalysisWormHead : WormHead
    {
        public override int BodyType => ModContent.NPCType<AnalysisWormBody>();

        public override int TailType => ModContent.NPCType<AnalysisWormTail>();

        public override void SetStaticDefaults()
        {
            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {   // Influences how the NPC looks in the Bestiary
                // 影响NPC在图鉴中的外观
                CustomTexturePath = "AnalysisMod/AnalysisContent/NPCs/AnalysisWorm_Bestiary", // If the NPC is multiple parts like a worm, a custom texture for the Bestiary is encouraged.
                                                                                              // 如果NPC是由多个部分组成，比如蠕虫，建议为其设置自定义纹理。
                Position = new Vector2(40f, 24f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 12f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        }

        public override void SetDefaults()
        {
            // Head is 10 defense, body 20, tail 30.
            // 头部防御值为10，身体为20，尾巴为30。
            NPC.CloneDefaults(NPCID.DiggerHead);
            NPC.aiStyle = -1;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            // 我们可以使用AddRange方法一次性添加多个项而不是多次调用Add方法来添加单个项
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
                // 设置此NPC在图鉴中出现的条件。
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Underground,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,

				// Sets the description of this NPC that is listed in the bestiary.
                // 设置此NPC在图鉴中列出的描述信息。
				new FlavorTextBestiaryInfoElement("Looks like a Digger fell into some aqua-colored paint. Oh well.")
            });
        }

        public override void Init()
        {
            // Set the segment variance
            // If you want the segment length to be constant, set these two properties to the same value

            // 设置段落变化
            // 如果您想要段落长度保持恒定，请将这两个属性设置为相同的值
            MinSegmentLength = 6;
            MaxSegmentLength = 12;

            CommonWormInit(this);
        }

        // This method is invoked from AnalysisWormHead, AnalysisWormBody and AnalysisWormTail
        // 此方法从AnalysisWormHead、AnalysisWormBody和AnalysisWormTail中调用
        internal static void CommonWormInit(Worm worm)
        {
            // These two properties handle the movement of the worm
            // 这两个属性处理蠕虫移动
            worm.MoveSpeed = 5.5f;
            worm.Acceleration = 0.045f;
        }

        private int attackCounter;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(attackCounter);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            attackCounter = reader.ReadInt32();
        }

        public override void AI()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (attackCounter > 0)
                {
                    attackCounter--; // tick down the attack counter.
                                     // 减少攻击计数器。
                }

                Player target = Main.player[NPC.target];
                // If the attack counter is 0, this NPC is less than 12.5 tiles away from its target, and has a path to the target unobstructed by blocks, summon a projectile.
                // 如果攻击计数器为0，并且该NPC距离目标小于12.5格，并且有通向目标无障碍方块路径，则召唤一个投射物。
                if (attackCounter <= 0 && Vector2.Distance(NPC.Center, target.Center) < 200 && Collision.CanHit(NPC.Center, 1, 1, target.Center, 1, 1))
                {
                    Vector2 direction = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX);
                    direction = direction.RotatedByRandom(MathHelper.ToRadians(10));

                    int projectile = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, direction * 1, ProjectileID.ShadowBeamHostile, 5, 0, Main.myPlayer);
                    Main.projectile[projectile].timeLeft = 300;
                    attackCounter = 500;
                    NPC.netUpdate = true;
                }
            }
        }
    }

    internal class AnalysisWormBody : WormBody
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Hide = true // 隐藏此NPC使其不出现在图鉴中，适用于由多个部分组成的NPC，您只想要一个条目的情况。
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DiggerBody);
            NPC.aiStyle = -1;
        }

        public override void Init()
        {
            AnalysisWormHead.CommonWormInit(this);
        }
    }

    internal class AnalysisWormTail : WormTail
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
                            // 隐藏此NPC使其不出现在图鉴中，适用于由多个部分组成的NPC，您只想要一个条目的情况。
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DiggerTail);
            NPC.aiStyle = -1;
        }

        public override void Init()
        {
            AnalysisWormHead.CommonWormInit(this);
        }
    }
}
