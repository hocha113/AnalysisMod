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
    // Party Zombie�����NPC��һ��������¡��Ҫ�˽���ν�һ����Ӧ���NPC��Ϊ����μ� https://github.com/tModLoader/tModLoader/wiki/Advanced-Vanilla-Code-Adaption#Analysis-npc-npc-clone-with-modified-projectile-hoplite
    public class PartyZombie : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.Zombie];

            NPCID.Sets.ShimmerTransformToNPC[NPC.type] = NPCID.Skeleton;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {   // Influences how the NPC looks in the Bestiary
                // Ӱ��NPC��ͼ���е����
                Velocity = 1f // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
                              // ��NPC������ͼ���У��ͺ�������x�����ƶ�+1����ש
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
                             // ս����AI��ѡ����������Ҫģ�µ�NPCID��ƥ���aiStyle�ǳ���Ҫ��

            AIType = NPCID.Zombie; // Use vanilla zombie's type when executing AI code. (This also means it will try to despawn during daytime)
                                   // ִ��AI����ʱʹ����ݽ�ʬ���͡�����Ҳ��ζ�������ڰ��쳢����ʧ��

            AnimationType = NPCID.Zombie; // Use vanilla zombie's type when executing animation code. Important to also match Main.npcFrameCount[NPC.type] in SetStaticDefaults.
                                          // ִ�ж�������ʱʹ����ݽ�ʬ���͡�ͬ����Ҫ���ǣ���SetStaticDefaults�л���Ҫƥ��Main.npcFrameCount[NPC.type]��

            Banner = Item.NPCtoBanner(NPCID.Zombie); // Makes this NPC get affected by the normal zombie banner.
                                                     // ʹ��NPC�ܵ���ͨ��ʬ����Ӱ�졣

            BannerItem = Item.BannerToItem(Banner); // Makes kills of this NPC go towards dropping the banner it's associated with.
                                                    // ʹ��NPC��ɱ������������������Ʒ������

            SpawnModBiomes = new int[1] { ModContent.GetInstance<AnalysisSurfaceBiome>().Type }; // Associates this NPC with the AnalysisSurfaceBiome in Bestiary
                                                                                                 // ����NPC��Bestiary�е�AnalysisSurfaceBiome�����
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Since Party Zombie is essentially just another variation of Zombie, we'd like to mimic the Zombie drops.
            // To do this, we can either (1) copy the drops from the Zombie directly or (2) just recreate the drops in our code.
            // (1) Copying the drops directly means that if Terraria updates and changes the Zombie drops, your ModNPC will also inherit the changes automatically.
            // (2) Recreating the drops can give you more control if desired but requires consulting the wiki, bestiary, or source code and then writing drop code.

            // ����Party Zombieʵ����ֻ��Zombie����һ�ֱ��壬�������ϣ��ģ��Zombie���䡣
            // Ϊ��������һ�㣬���ǿ���ֱ�Ӹ���Zombie���䣨����1���������ڴ��������´������䣨����2����
            // ��1��ֱ�Ӹ��Ƶ�����ζ�����Terraria���²�����Zombie���䣬����ModNPCҲ���Զ��̳и��ġ�
            // ��2�����´���������Ը�������Ŀ���Ȩ������Ҫ����wiki��bestiary��Դ���룬Ȼ���дdrop code��

            // (1) This Analysis shows copying the drops directly. For consistency and mod compatibility, we suggest using the smallest positive NPCID when dealing with npcs with many variants and shared drop pools.
            // (1)�˷�����ʾֱ�Ӹ����½���Ϊ�˱���һ���Ժ�ģ������ԣ��ڴ��������������͹����½��ص�npcsʱ����ʹ����С����NPCID��
            var zombieDropRules = Main.ItemDropsDB.GetRulesForNPCID(NPCID.Zombie, false); // false is important here
                                                                                          // false���������Ҫ
            foreach (var zombieDropRule in zombieDropRules)
            {
                // In this foreach loop, we simple add each drop to the PartyZombie drop pool. 
                // �ڴ�foreachѭ���У�����ֻ�轫ÿ��Ͷ�������ӵ�PartyZombieͶ�ų��м��ɡ�
                npcLoot.Add(zombieDropRule);
            }

            // (2) This Analysis shows recreating the drops. This code is commented out because we are using the previous method instead.
            // (2) �˷�����ʾ���´����½���������������ʹ����ǰ�ķ�������˸ô����ѱ�ע�͡�

            // npcLoot.Add(ItemDropRule.Common(ItemID.Shackle, 50)); // Drop shackles with a 1 out of 50 chance.
                                                                     // ��1/50�ļ��ʵ���������
            // npcLoot.Add(ItemDropRule.Common(ItemID.ZombieArm, 250)); // Drop zombie arm with a 1 out of 250 chance.
                                                                        // ��1/250�ļ��ʵ��佩ʬ�ֱۡ�

            // Finally, we can add additional drops. Many Zombie variants have their own unique drops: https://terraria.fandom.com/wiki/Zombie
            // ������ǿ�����������Ͷ����Ʒ�����ཀྵʬ���ֶ����Լ����ص�Ͷ����Ʒ��https://terraria.fandom.com/wiki/Zombie

            npcLoot.Add(ItemDropRule.Common(ItemID.Confetti, 100)); // 1% chance to drop Confetti
                                                                    // ��1% �ļ��ʶ������ֽм
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.OverworldNightMonster.Chance * 0.2f; // Spawn with 1/5th the chance of a regular zombie.
                                                                       // ����ͨ��ʬ������ɸ��ʼ����ķ�֮һ��
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            // ���ǿ���ʹ��AddRange�����Ƕ�ε���Add��һ�����Ӷ����
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
                // ���ô�NPC��������������������ͼ�����г���
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,

				// Sets the description of this NPC that is listed in the bestiary.
                // ���ô�NPC��ͼ�����г���������
				new FlavorTextBestiaryInfoElement("This type of zombie for some reason really likes to spread confetti around. Otherwise, it behaves just like a normal zombie."),

				// By default the last added IBestiaryBackgroundImagePathAndColorProvider will be used to show the background image.
				// The AnalysisSurfaceBiome ModBiomeBestiaryInfoElement is automatically populated into bestiaryEntry.Info prior to this method being called
				// so we use this line to tell the game to prioritize a specific InfoElement for sourcing the background image.

                // Ĭ������£���ʹ��������ӵ�IBestiaryBackgroundImagePathAndColorProvider��ʾ����ͼ��
                // AnalysisSurfaceBiome ModBiomeBestiaryInfoElement���Զ���䵽bestiaryEntry.Info�У��ڵ��ô˷���֮ǰ
                // �������ʹ�����д��������Ϸ���ȿ����ض���InfoElement�Ի�ȡ����ͼƬ��Դ��
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<AnalysisSurfaceBiome>().ModBiomeBestiaryInfoElement),
            });
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            // Spawn confetti when this zombie is hit.
            // �����������ʬʱ�������ֽм��

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

            // ����������NPCͨ����hitbox���������������Ͷ���������ң������ʹ���鷢����
            // ������;��Ӧ��buffs/debuffs��

            int buffType = ModContent.BuffType<AnimatedBuff>();
            // Alternatively, you can use a vanilla buff: int buffType = BuffID.Slow;
            // ���ߣ�������ʹ����ͨ��buff��int buffType = BuffID.Slow;

            int timeToAdd = 5 * 60; //This makes it 5 seconds, one second is 60 ticks
                                    //��ʹ�����5�룬ÿ��Ϊ60��ticks
            target.AddBuff(buffType, timeToAdd);
        }

        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            if (modifiers.DamageType.CountsAsClass(DamageClass.Magic))
            {
                // This Analysis shows how PartyZombie reduces magic damage by 75%. We use FinalDamage here rather than SourceDamage since we are affecting how the npc reacts to the damage.
                // Conceptually, the source dealing the damage isn't interpreted as stronger, but rather this NPC has a resistance to this damage source.

                // �������չʾ��PartyZombie��ν�ħ���˺�����75%������������ʹ��FinalDamage������SourceDamage����Ϊ��������Ӱ��npc���˺��ķ�Ӧ��
                // �Ӹ����Ͻ�������˺�����Դ��û�б�����Ϊ��ǿ�󣬶��Ǹ�NPC�Դ��˺���Դ���п��ԡ�
                modifiers.FinalDamage *= 0.25f;
            }
        }
    }
}