using AnalysisMod.AnalysisContent.Biomes;
using AnalysisMod.AnalysisContent.Items;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.Utilities;

namespace AnalysisMod.AnalysisContent.NPCs
{
    //The AnalysisZombieThief is essentially the same as a regular Zombie, but it steals AnalysisItems and keep them until it is killed, being saved with the world if it has enough of them.
    //������ʬ��������ͨ�Ľ�ʬ������ͬ��������͵ȡ������Ʒ���������ǣ�ֱ����ɱ��������ӵ���㹻�����������������һ�𱣴档
    public class AnalysisZombieThief : ModNPC
    {
        public int StolenItems = 0;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.Zombie];

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                // Influences how the NPC looks in the Bestiary
                //Ӱ��NPC��ͼ���е����
                Velocity = 1f // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
                              // ��NPC������ͼ���У��ͺ�������x����������+1����ש
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
                             // ս����AI����Ҫ����ѡ����������ģ�µ�NPCIDƥ���aiStyle

            AIType = NPCID.Zombie; // Use vanilla zombie's type when executing AI code. (This also means it will try to despawn during daytime)
                                   // ִ��AI����ʱʹ����ݽ�ʬ���͡�����Ҳ��ζ�������ڰ�����ͼ��ʧ��

            AnimationType = NPCID.Zombie; // Use vanilla zombie's type when executing animation code. Important to also match Main.npcFrameCount[NPC.type] in SetStaticDefaults.
                                          // ִ�ж�������ʱʹ����ݽ�ʬ���͡���Ҫע��SetStaticDefaults��ƥ��Main.npcFrameCount[NPC.type]��

            Banner = Item.NPCtoBanner(NPCID.Zombie); // Makes this NPC get affected by the normal zombie banner.
                                                     // ʹ��NPC�ܵ�������ʬ����Ӱ�졣

            BannerItem = Item.BannerToItem(Banner); // Makes kills of this NPC go towards dropping the banner it's associated with.
                                                    // ʹ��NPC���ܺ������������ļ��������ӡ�

            SpawnModBiomes = new int[] { ModContent.GetInstance<AnalysisSurfaceBiome>().Type }; // Associates this NPC with the AnalysisSurfaceBiome in Bestiary
                                                                                                // ����NPC��Bestiary��AnalysisSurfaceBiome�����
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            // ���ǿ���ʹ��AddRange�����Ƕ�ε���Add��һ����Ӷ����Ŀ
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
                // �����г�����Ѽ�¼����ڸ� NPC ����������.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,

				// Sets the description of this NPC that is listed in the bestiary.
                // �����г�����Ѽ�¼����ڸ� NPC ������.
				new FlavorTextBestiaryInfoElement("This type of zombie really like Analysis Items. They steal them as soon as they find some."),
            });
        }

        public override void AI()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }

            Rectangle hitbox = NPC.Hitbox;
            foreach (Item item in Main.item)
            {
                // Pickup the items only if the NPC touches them and they aren't already being grabbed by a player
                // ֻ�е� NPC �Ӵ���Ʒ�������û��ץȡʱ�ż�����Ʒ
                if (item.active && !item.beingGrabbed && item.type == ModContent.ItemType<AnalysisItem>() &&
                    hitbox.Intersects(item.Hitbox))
                {
                    item.active = false;
                    StolenItems += item.stack;

                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, item.whoAmI);
                }
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(StolenItems);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            StolenItems = reader.ReadInt32();
        }

        public override void OnKill()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }

            // Drop all the stolen items when the NPC dies
            // �� NPC ����ʱ�������б�͵����Ʒ
            while (StolenItems > 0)
            {
                // Loop until all items are dropped, to avoid dropping more than maxStack items
                // ѭ��ֱ������������Ʒ���Ա�����䳬��maxStack����Ʒ
                int droppedAmount = Math.Min(ModContent.GetInstance<AnalysisItem>().Item.maxStack, StolenItems);
                StolenItems -= droppedAmount;
                Item.NewItem(NPC.GetSource_Death(), NPC.Center, ModContent.ItemType<AnalysisItem>(), droppedAmount, true);
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.InModBiome(ModContent.GetInstance<AnalysisSurfaceBiome>()) // Can only spawn in the AnalysisSurfaceBiome
                                                                                            // ֻ����AnalysisSurfaceBiome������
                && !NPC.AnyNPCs(Type))
            {
                // Can only spawn if there are no other AnalysisZombieThiefs
                // ����û������AnalysisZombieThiefsʱ�Ż����ɡ�
                return SpawnCondition.OverworldNightMonster.Chance * 0.1f; // Spawn with 1/10th the chance of a regular zombie.
                                                                           // ����ͨ��ʬ��ȣ�ֻ��1/10�ļ������ɡ�
            }

            return 0f;
        }

        public override bool NeedSaving()
        {
            return StolenItems >= 10; // Only save if the NPC has more than 10 stolen items, to avoid keeping the NPC in memory if it only has few
                                      // ����NPCӵ�г���10��������Ʒʱ���棬�Ա��������ֻ�����������䱣�����ڴ��С�
        }

        public override void SaveData(TagCompound tag)
        {
            if (StolenItems > 0)
            {
                // Note that at this point it may have less than 10 stolen items, if another mod or part of our decides to save the NPC
                // ��ע�⣬�ڴ�ʱ���ܾ�������10��������Ŀ�������һ��ģ������ǵ�ĳ���־�������NPC������ˡ�
                tag["StolenItems"] = StolenItems;
            }
        }

        public override void LoadData(TagCompound tag)
        {
            StolenItems = tag.GetInt("StolenItems");
        }
    }
}