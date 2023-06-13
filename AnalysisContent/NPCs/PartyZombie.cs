using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader.Utilities;
using Terraria.DataStructures;
using AnalysisMod.AnalysisContent.Biomes;
using AnalysisMod.AnalysisContent.Buffs;

namespace AnalysisMod.AnalysisContent.NPCs
{
    // Party Zombie is a pretty basic clone of a vanilla NPC. To learn how to further adapt vanilla NPC behaviors, see https://github.com/tModLoader/tModLoader/wiki/Advanced-Vanilla-Code-Adaption#Analysis-npc-npc-clone-with-modified-projectile-hoplite
    // Party Zombie是香草NPC的一个基本克隆。要了解如何进一步适应香草NPC行为，请参见 https://github.com/tModLoader/tModLoader/wiki/Advanced-Vanilla-Code-Adaption#Analysis-npc-npc-clone-with-modified-projectile-hoplite
    public class PartyZombie : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.Zombie];

            NPCID.Sets.ShimmerTransformToNPC[NPC.type] = NPCID.Skeleton;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {   // Influences how the NPC looks in the Bestiary
                // 影响NPC在图鉴中的外观
                Velocity = 1f // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
                              // 将NPC绘制在图鉴中，就好像它向x方向移动+1个瓷砖
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 18;
            NPC.height = 40;
            NPC.damage = 14;
            NPC.defense = 6;
            NPC.lifeMax = 200;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.value = 60f;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = 3; // Fighter AI, important to choose the aiStyle that matches the NPCID that we want to mimic
                             // 战斗者AI，选择与我们想要模仿的NPCID相匹配的aiStyle非常重要。

            AIType = NPCID.Zombie; // Use vanilla zombie's type when executing AI code. (This also means it will try to despawn during daytime)
                                   // 执行AI代码时使用香草僵尸类型。（这也意味着它会在白天尝试消失）

            AnimationType = NPCID.Zombie; // Use vanilla zombie's type when executing animation code. Important to also match Main.npcFrameCount[NPC.type] in SetStaticDefaults.
                                          // 执行动画代码时使用香草僵尸类型。同样重要的是，在SetStaticDefaults中还需要匹配Main.npcFrameCount[NPC.type]。

            Banner = Item.NPCtoBanner(NPCID.Zombie); // Makes this NPC get affected by the normal zombie banner.
                                                     // 使此NPC受到普通僵尸旗帜影响。

            BannerItem = Item.BannerToItem(Banner); // Makes kills of this NPC go towards dropping the banner it's associated with.
                                                    // 使此NPC击杀计入其关联横幅掉落物品数量。

            SpawnModBiomes = new int[1] { ModContent.GetInstance<AnalysisSurfaceBiome>().Type }; // Associates this NPC with the AnalysisSurfaceBiome in Bestiary
                                                                                                 // 将此NPC与Bestiary中的AnalysisSurfaceBiome相关联
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Since Party Zombie is essentially just another variation of Zombie, we'd like to mimic the Zombie drops.
            // To do this, we can either (1) copy the drops from the Zombie directly or (2) just recreate the drops in our code.
            // (1) Copying the drops directly means that if Terraria updates and changes the Zombie drops, your ModNPC will also inherit the changes automatically.
            // (2) Recreating the drops can give you more control if desired but requires consulting the wiki, bestiary, or source code and then writing drop code.

            // 由于Party Zombie实质上只是Zombie的另一种变体，因此我们希望模仿Zombie掉落。
            // 为了做到这一点，我们可以直接复制Zombie掉落（方法1），或者在代码中重新创建掉落（方法2）。
            // （1）直接复制掉落意味着如果Terraria更新并更改Zombie掉落，您的ModNPC也将自动继承更改。
            // （2）重新创建掉落可以给您更多的控制权，但需要查阅wiki、bestiary或源代码，然后编写drop code。

            // (1) This Analysis shows copying the drops directly. For consistency and mod compatibility, we suggest using the smallest positive NPCID when dealing with npcs with many variants and shared drop pools.
            // (1)此分析显示直接复制下降。为了保持一致性和模组兼容性，在处理具有许多变体和共享下降池的npcs时建议使用最小正数NPCID。
            var zombieDropRules = Main.ItemDropsDB.GetRulesForNPCID(NPCID.Zombie, false); // false is important here
                                                                                          // false在这里很重要
            foreach (var zombieDropRule in zombieDropRules)
            {
                // In this foreach loop, we simple add each drop to the PartyZombie drop pool. 
                // 在此foreach循环中，我们只需将每个投放物添加到PartyZombie投放池中即可。
                npcLoot.Add(zombieDropRule);
            }

            // (2) This Analysis shows recreating the drops. This code is commented out because we are using the previous method instead.
            // (2) 此分析显示重新创建下降。由于我们正在使用先前的方法，因此该代码已被注释。

            // npcLoot.Add(ItemDropRule.Common(ItemID.Shackle, 50)); // Drop shackles with a 1 out of 50 chance.
                                                                     // 有1/50的几率掉落铁链。
            // npcLoot.Add(ItemDropRule.Common(ItemID.ZombieArm, 250)); // Drop zombie arm with a 1 out of 250 chance.
                                                                        // 有1/250的几率掉落僵尸手臂。

            // Finally, we can add additional drops. Many Zombie variants have their own unique drops: https://terraria.fandom.com/wiki/Zombie
            // 最后，我们可以添加其他投放物品。许多僵尸变种都有自己独特的投放物品：https://terraria.fandom.com/wiki/Zombie

            npcLoot.Add(ItemDropRule.Common(ItemID.Confetti, 100)); // 1% chance to drop Confetti
                                                                    // 以1% 的几率丢出五彩纸屑
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.OverworldNightMonster.Chance * 0.2f; // Spawn with 1/5th the chance of a regular zombie.
                                                                       // 与普通僵尸相比生成概率减少四分之一。
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            // 我们可以使用AddRange而不是多次调用Add来一次添加多个项
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
                // 设置此NPC的生成条件，该条件在图鉴中列出。
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,

				// Sets the description of this NPC that is listed in the bestiary.
                // 设置此NPC在图鉴中列出的描述。
				new FlavorTextBestiaryInfoElement("This type of zombie for some reason really likes to spread confetti around. Otherwise, it behaves just like a normal zombie."),

				// By default the last added IBestiaryBackgroundImagePathAndColorProvider will be used to show the background image.
				// The AnalysisSurfaceBiome ModBiomeBestiaryInfoElement is automatically populated into bestiaryEntry.Info prior to this method being called
				// so we use this line to tell the game to prioritize a specific InfoElement for sourcing the background image.

                // 默认情况下，将使用最后添加的IBestiaryBackgroundImagePathAndColorProvider显示背景图像。
                // AnalysisSurfaceBiome ModBiomeBestiaryInfoElement会自动填充到bestiaryEntry.Info中，在调用此方法之前
                // 因此我们使用这行代码告诉游戏优先考虑特定的InfoElement以获取背景图片来源。
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<AnalysisSurfaceBiome>().ModBiomeBestiaryInfoElement),
            });
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            // Spawn confetti when this zombie is hit.
            // 当击打这个僵尸时产生五彩纸屑。

            for (int i = 0; i < 10; i++)
            {
                int dustType = Main.rand.Next(139, 143);
                var dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, dustType);

                dust.velocity.X += Main.rand.NextFloat(-0.05f, 0.05f);
                dust.velocity.Y += Main.rand.NextFloat(-0.05f, 0.05f);

                dust.scale *= 1f + Main.rand.NextFloat(-0.03f, 0.03f);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            // Here we can make things happen if this NPC hits a player via its hitbox (not projectiles it shoots, this is handled in the projectile code usually)
            // Common use is applying buffs/debuffs:

            // 在这里，如果该NPC通过其hitbox（而不是它发射的投放物）击中玩家，则可以使事情发生。
            // 常见用途是应用buffs/debuffs：

            int buffType = ModContent.BuffType<AnimatedBuff>();
            // Alternatively, you can use a vanilla buff: int buffType = BuffID.Slow;
            // 或者，您可以使用普通的buff：int buffType = BuffID.Slow;

            int timeToAdd = 5 * 60; //This makes it 5 seconds, one second is 60 ticks
                                    //这使其持续5秒，每秒为60个ticks
            target.AddBuff(buffType, timeToAdd);
        }

        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            if (modifiers.DamageType.CountsAsClass(DamageClass.Magic))
            {
                // This Analysis shows how PartyZombie reduces magic damage by 75%. We use FinalDamage here rather than SourceDamage since we are affecting how the npc reacts to the damage.
                // Conceptually, the source dealing the damage isn't interpreted as stronger, but rather this NPC has a resistance to this damage source.

                // 这个分析展示了PartyZombie如何将魔法伤害减少75%。我们在这里使用FinalDamage而不是SourceDamage，因为我们正在影响npc对伤害的反应。
                // 从概念上讲，造成伤害的来源并没有被解释为更强大，而是该NPC对此伤害来源具有抗性。
                modifiers.FinalDamage *= 0.25f;
            }
        }
    }
}
