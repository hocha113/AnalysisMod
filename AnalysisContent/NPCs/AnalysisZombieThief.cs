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
    //分析僵尸窃贼与普通的僵尸基本相同，但它会偷取分析物品并保留它们，直到被杀死，并在拥有足够数量的情况下随世界一起保存。
    public class AnalysisZombieThief : ModNPC
    {
        public int StolenItems = 0;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.Zombie];

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                // Influences how the NPC looks in the Bestiary
                //影响NPC在图鉴中的外观
                Velocity = 1f // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
                              // 将NPC绘制在图鉴中，就好像它向x方向行走了+1个瓷砖
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
                             // 战斗者AI，重要的是选择与我们想模仿的NPCID匹配的aiStyle

            AIType = NPCID.Zombie; // Use vanilla zombie's type when executing AI code. (This also means it will try to despawn during daytime)
                                   // 执行AI代码时使用香草僵尸类型。（这也意味着它会在白天试图消失）

            AnimationType = NPCID.Zombie; // Use vanilla zombie's type when executing animation code. Important to also match Main.npcFrameCount[NPC.type] in SetStaticDefaults.
                                          // 执行动画代码时使用香草僵尸类型。还要注意SetStaticDefaults中匹配Main.npcFrameCount[NPC.type]。

            Banner = Item.NPCtoBanner(NPCID.Zombie); // Makes this NPC get affected by the normal zombie banner.
                                                     // 使此NPC受到正常僵尸旗帜影响。

            BannerItem = Item.BannerToItem(Banner); // Makes kills of this NPC go towards dropping the banner it's associated with.
                                                    // 使此NPC击败后掉落其关联旗帜计数器增加。

            SpawnModBiomes = new int[] { ModContent.GetInstance<AnalysisSurfaceBiome>().Type }; // Associates this NPC with the AnalysisSurfaceBiome in Bestiary
                                                                                                // 将此NPC与Bestiary中AnalysisSurfaceBiome相关联
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            // 我们可以使用AddRange而不是多次调用Add来一次添加多个项目
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
                // 设置列出于最佳记录表格内该 NPC 的生成条件.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,

				// Sets the description of this NPC that is listed in the bestiary.
                // 设置列出于最佳记录表格内该 NPC 的描述.
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
                // 只有当 NPC 接触物品并且玩家没有抓取时才捡起物品
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
            // 当 NPC 死亡时放下所有被偷的物品
            while (StolenItems > 0)
            {
                // Loop until all items are dropped, to avoid dropping more than maxStack items
                // 循环直到放下所有物品，以避免掉落超过maxStack个物品
                int droppedAmount = Math.Min(ModContent.GetInstance<AnalysisItem>().Item.maxStack, StolenItems);
                StolenItems -= droppedAmount;
                Item.NewItem(NPC.GetSource_Death(), NPC.Center, ModContent.ItemType<AnalysisItem>(), droppedAmount, true);
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.InModBiome(ModContent.GetInstance<AnalysisSurfaceBiome>()) // Can only spawn in the AnalysisSurfaceBiome
                                                                                            // 只能在AnalysisSurfaceBiome中生成
                && !NPC.AnyNPCs(Type))
            {
                // Can only spawn if there are no other AnalysisZombieThiefs
                // 仅当没有其他AnalysisZombieThiefs时才会生成。
                return SpawnCondition.OverworldNightMonster.Chance * 0.1f; // Spawn with 1/10th the chance of a regular zombie.
                                                                           // 与普通僵尸相比，只有1/10的几率生成。
            }

            return 0f;
        }

        public override bool NeedSaving()
        {
            return StolenItems >= 10; // Only save if the NPC has more than 10 stolen items, to avoid keeping the NPC in memory if it only has few
                                      // 仅在NPC拥有超过10个被盗物品时保存，以避免如果它只有少量而将其保留在内存中。
        }

        public override void SaveData(TagCompound tag)
        {
            if (StolenItems > 0)
            {
                // Note that at this point it may have less than 10 stolen items, if another mod or part of our decides to save the NPC
                // 请注意，在此时可能具有少于10个被盗项目，如果另一个模组或我们的某部分决定保存NPC，则如此。
                tag["StolenItems"] = StolenItems;
            }
        }

        public override void LoadData(TagCompound tag)
        {
            StolenItems = tag.GetInt("StolenItems");
        }
    }
}