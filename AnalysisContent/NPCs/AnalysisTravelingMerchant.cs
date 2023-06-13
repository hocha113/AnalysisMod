using AnalysisMod.AnalysisContent.Dusts;
using AnalysisMod.AnalysisContent.Items;
using AnalysisMod.AnalysisContent.Items.Armor;
using AnalysisMod.AnalysisContent.Items.Placeable;
using AnalysisMod.AnalysisContent.Items.Placeable.Furniture;
using AnalysisMod.AnalysisContent.Items.Tools;
using AnalysisMod.AnalysisContent.Items.Weapons;
using AnalysisMod.AnalysisContent.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace AnalysisMod.AnalysisContent.NPCs
{
    [AutoloadHead]
    class AnalysisTravelingMerchant : ModNPC
    {
        // Time of day for traveller to leave (6PM)
        //旅行商人离开的时间（下午6点）
        public const double despawnTime = 48600.0;

        // the time of day the traveler will spawn (double.MaxValue for no spawn)
        // saved and loaded with the world in TravelingMerchantSystem

        //旅行者生成的时间（double.MaxValue表示不生成）
        //在TravelingMerchantSystem中与世界一起保存和加载
        public static double spawnTime = double.MaxValue;

        // A static instance of the declarative shop, defining all the items which can be brought. Used to create a new inventory when the NPC spawns
        //声明商店的静态实例，定义可以购买的所有物品。用于创建NPC生成时新库存。
        public static AnalysisTravelingMerchantShop Shop;

        // The list of items in the traveler's shop. Saved with the world and set when the traveler spawns
        //旅行者商店中物品列表。随着世界而保存，并在旅行者生成时设置。
        public List<Item> shopItems;

        private static int ShimmerHeadIndex;
        private static Profiles.StackedNPCProfile NPCProfile;

        public override bool PreAI()
        {
            if ((!Main.dayTime || Main.time >= despawnTime) && !IsNpcOnscreen(NPC.Center)) // If it's past the despawn time and the NPC isn't onscreen
                                                                                           // 如果超过消失时间并且NPC不在屏幕上
            {
                // Here we despawn the NPC and send a message stating that the NPC has despawned
                // LegacyMisc.35 is {0) has departed!

                //这里我们会让NPC消失并发送消息说明NPC已经消失了
                //LegacyMisc.35是{0}已经离开了！
                if (Main.netMode == NetmodeID.SinglePlayer) Main.NewText(Language.GetTextValue("LegacyMisc.35", NPC.FullName), 50, 125, 255);
                else ChatHelper.BroadcastChatMessage(NetworkText.FromKey("LegacyMisc.35", NPC.GetFullNetName()), new Color(50, 125, 255));
                NPC.active = false;
                NPC.netSkip = -1;
                NPC.life = 0;
                return false;
            }

            return true;
        }

        public override void AddShops()
        {
            Shop = new AnalysisTravelingMerchantShop(NPC.type);

            // Always bring an AnalysisItem
            //始终携带一个AnalysisItem
            Shop.Add<AnalysisItem>();

            // Bring 2 Tools
            //携带2个工具
            Shop.AddPool("Tools", slots: 2)
                .Add<AnalysisDrill>()
                .Add<AnalysisHamaxe>()
                .Add<AnalysisFishingRod>()
                .Add<AnalysisHookItem>()
                .Add<AnalysisBugNet>()
                .Add<AnalysisPickaxe>();

            // Bring 4 Weapons
            //携带4个武器
            Shop.AddPool("Weapons", slots: 4)
                .Add<AnalysisContent.Items.Weapons.AnalysisSword>()
                .Add<AnalysisShortsword>()
                .Add<AnalysisShootingSword>()
                .Add<AnalysisJavelin>()
                .Add<AnalysisSpear>()
                .Add<AnalysisMagicWeapon>()
                .Add<AnalysisGun>()
                .Add<AnalysisShotgun>()
                .Add<AnalysisMinigun>()
                .Add<AnalysisFlail>()
                .Add<AnalysisAdvancedFlail>(Condition.Hardmode) // Only bring advanced Analysiss in hardmode!
                                                                // 只有在困难模式下才能携带高级分析设备！

                .Add<AnalysisWhip>()
                .Add<AnalysisWhipAdvanced>(Condition.Hardmode)
                .Add<AnalysisYoyo>();

            // Bring 3 Furniture
            //携带3件家具
            Shop.AddPool("Furniture", slots: 3)
                .Add<AnalysisLamp>()
                .Add<AnalysisBed>()
                .Add<AnalysisChair>()
                .Add<AnalysisChest>()
                .Add<AnalysisClock>()
                .Add<AnalysisDoor>()
                .Add<AnalysisSink>()
                .Add<AnalysisTable>()
                .Add<AnalysisToilet>()
                .Add<AnalysisWorkbench>();

            Shop.Register();
        }

        public static void UpdateTravelingMerchant()
        {
            bool travelerIsThere = NPC.FindFirstNPC(ModContent.NPCType<AnalysisTravelingMerchant>()) != -1; // Find a Merchant if there's one spawned in the world
                                                                                                            // 查找是否有出现过商人。

            // Main.time is set to 0 each morning, and only for one update. Sundialling will never skip past time 0 so this is the place for 'on new day' code
            // 每天清晨Main.time被设置为0，仅进行一次更新。日晷永远不会跳过时间0，因此这是“新一天”代码的位置。
            if (Main.dayTime && Main.time == 0)
            {
                // insert code here to change the spawn chance based on other conditions (say, npcs which have arrived, or milestones the player has passed)
                // You can also add a day counter here to prevent the merchant from possibly spawning multiple days in a row.

                // 插入代码以根据其他条件更改生成几率（例如到达的npc或玩家通过的里程碑）。
                // 您还可以添加一个计数器来防止商人可能连续多天产生。

                // NPC won't spawn today if it stayed all night
                // 如果它整夜都待在那里，则今天不会刷新NPC.
                if (!travelerIsThere && Main.rand.NextBool(4))
                { // 4 = 25% Chance
                  // 4 = 25％的几率

                  // Here we can make it so the NPC doesnt spawn at the EXACT same time every time it does spawn
                  // 在这里，我们可以使NPC不会每次生成时都在完全相同的时间刷新
                    spawnTime = GetRandomSpawnTime(5400, 8100); // minTime = 6:00am, maxTime = 7:30am                                                         
                }
                else
                {
                    spawnTime = double.MaxValue; // no spawn today
                                                 // 今天不会生成。
                }
            }

            // Spawn the traveler if the spawn conditions are met (time of day, no events, no sundial)
            // 如果满足生成条件（白天、无事件、没有日晷），则生成旅行商人。
            if (!travelerIsThere && CanSpawnNow())
            {
                int newTraveler = NPC.NewNPC(Terraria.Entity.GetSource_TownSpawn(), Main.spawnTileX * 16, Main.spawnTileY * 16, ModContent.NPCType<AnalysisTravelingMerchant>(), 1); // Spawning at the world spawn
                NPC traveler = Main.npc[newTraveler];
                traveler.homeless = true;
                traveler.direction = Main.spawnTileX >= WorldGen.bestX ? -1 : 1;
                traveler.netUpdate = true;

                // Prevents the traveler from spawning again the same day
                // 防止旅行商人当天再次出现
                spawnTime = double.MaxValue;

                // Annouce that the traveler has spawned in!
                // 宣布旅行商人已经出现！
                if (Main.netMode == NetmodeID.SinglePlayer) Main.NewText(Language.GetTextValue("Announcement.HasArrived", traveler.FullName), 50, 125, 255);
                else ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Announcement.HasArrived", traveler.GetFullNetName()), new Color(50, 125, 255));
            }
        }

        private static bool CanSpawnNow()
        {
            // can't spawn if any events are running
            // 如果有任何事件正在运行，则无法产生。
            if (Main.eclipse || Main.invasionType > 0 && Main.invasionDelay == 0 && Main.invasionSize > 0)
                return false;

            // can't spawn if the sundial is active
            // 如果日晷处于活动状态，则无法产生。
            if (Main.IsFastForwardingTime())
                return false;

            // can spawn if daytime, and between the spawn and despawn times
            // 如果是白天，并且在生成和消失时间之间，则可以产生。
            return Main.dayTime && Main.time >= spawnTime && Main.time < despawnTime;
        }

        private static bool IsNpcOnscreen(Vector2 center)
        {
            int w = NPC.sWidth + NPC.safeRangeX * 2;
            int h = NPC.sHeight + NPC.safeRangeY * 2;
            Rectangle npcScreenRect = new Rectangle((int)center.X - w / 2, (int)center.Y - h / 2, w, h);
            foreach (Player player in Main.player)
            {
                // If any player is close enough to the traveling merchant, it will prevent the npc from despawning
                // 如果有任何玩家靠近旅行商人，它将防止npc消失
                if (player.active && player.getRect().Intersects(npcScreenRect)) return true;
            }
            return false;
        }

        public static double GetRandomSpawnTime(double minTime, double maxTime)
        {
            // A simple formula to get a random time between two chosen times
            // 一个简单的公式来获取两个选择时间之间的随机时间
            return (maxTime - minTime) * Main.rand.NextDouble() + minTime;
        }

        public override void Load()
        {
            // Adds our Shimmer Head to the NPCHeadLoader.
            // 将我们的Shimmer Head添加到NPCHeadLoader中。
            ShimmerHeadIndex = Mod.AddNPCHeadTexture(Type, Texture + "_Shimmer_Head");
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 25;
            NPCID.Sets.ExtraFramesCount[Type] = 9;
            NPCID.Sets.AttackFrameCount[Type] = 4;
            NPCID.Sets.DangerDetectRange[Type] = 60;
            NPCID.Sets.AttackType[Type] = 3; // Swings a weapon. This NPC attacks in roughly the same manner as Stylist
                                             // 挥动武器。这个NPC攻击方式与理发师大致相同

            NPCID.Sets.AttackTime[Type] = 12;
            NPCID.Sets.AttackAverageChance[Type] = 1;
            NPCID.Sets.HatOffsetY[Type] = 4;
            NPCID.Sets.ShimmerTownTransform[Type] = true;
            NPCID.Sets.NoTownNPCHappiness[Type] = true; // Prevents the happiness button
                                                        // 防止幸福按钮出现

            // Influences how the NPC looks in the Bestiary
            // 影响NPC在图鉴中的外观
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Velocity = 2f, // Draws the NPC in the bestiary as if its walking +2 tiles in the x direction
                               // 在最佳iary中以其走+2个瓷砖x方向绘制NPC

                Direction = -1 // -1 is left and 1 is right.
                               // -1是左边，1是右边。
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

            NPCProfile = new Profiles.StackedNPCProfile(
                new Profiles.DefaultNPCProfile(Texture, NPCHeadLoader.GetHeadSlot(HeadTexture), Texture + "_Party"),
                new Profiles.DefaultNPCProfile(Texture + "_Shimmer", ShimmerHeadIndex)
            );
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = 7;
            NPC.damage = 10;
            NPC.defense = 15;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            AnimationType = NPCID.Stylist;
            TownNPCStayingHomeless = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            shopItems = Shop.GenerateNewInventoryList();
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface
            });
        }

        public override void SaveData(TagCompound tag)
        {
            tag["itemIds"] = shopItems;
        }

        public override void LoadData(TagCompound tag)
        {
            shopItems = tag.Get<List<Item>>("shopItems");
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            int num = NPC.life > 0 ? 1 : 5;
            for (int k = 0; k < num; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<Sparkle>());
            }

            // Create gore when the NPC is killed.
            // 当NPC被杀死时创建血腥场面。
            if (Main.netMode != NetmodeID.Server && NPC.life <= 0)
            {
                // Retrieve the gore types. This NPC has shimmer variants for head, arm, and leg gore. It also has a custom hat gore. (7 gores)
                // This NPC will spawn either the assigned party hat or a custom hat gore when not shimmered. When shimmered the top hat is part of the head and no hat gore is spawned.

                //检索血腥类型。此NPC具有头部、手臂和腿部闪光变体。它还有一个自定义帽子gore。(7 gores)
                //当未闪耀时，此NPC将生成分配的聚会帽或自定义帽gore。当闪耀时，礼帽是头部的一部分，并且不会生成hat gore。
                int hatGore = NPC.GetPartyHatGore();
                // If not wearing a party hat, and not shimmered, retrieve the custom hat gore 
                //如果没有戴派对帽，并且没有发亮，请检索自定义hat gore
                if (hatGore == 0 && !NPC.IsShimmerVariant)
                {
                    hatGore = Mod.Find<ModGore>($"{Name}_Gore_Hat").Type;
                }
                string variant = "";
                if (NPC.IsShimmerVariant) variant += "_Shimmer";
                int headGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Head").Type;
                int armGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Arm").Type;
                int legGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Leg").Type;

                // Spawn the gores. The positions of the arms and legs are lowered for a more natural look.
                //产生gores。为了更加自然地看起来，手臂和腿部位置下降了。
                if (hatGore > 0)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, hatGore);
                }
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, headGore);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 20), NPC.velocity, armGore);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 20), NPC.velocity, armGore);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 34), NPC.velocity, legGore);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 34), NPC.velocity, legGore);
            }
        }

        public override bool UsesPartyHat()
        {
            // AnalysisTravelingMerchant likes to keep his hat on while shimmered.
            // AnalysisTravelingMerchant喜欢在发亮时保持他的帽子。
            if (NPC.IsShimmerVariant)
            {
                return false;
            }
            return true;
        }

        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            return false; // This should always be false, because we spawn in the Traveling Merchant manually
                          // 这应该始终为false，因为我们会手动生成Traveling Merchant
        }

        public override ITownNPCProfile TownNPCProfile()
        {
            return NPCProfile;
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string>() {
                "Someone Else",
                "Somebody Else",
                "Blockster",
                "Colorful"
            };
        }

        public override string GetChat()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();

            int partyGirl = NPC.FindFirstNPC(NPCID.PartyGirl);
            if (partyGirl >= 0)
            {
                chat.Add(Language.GetTextValue("Mods.AnalysisMod.Dialogue.AnalysisTravelingMerchant.PartyGirlDialogue", Main.npc[partyGirl].GivenName));
            }

            chat.Add(Language.GetTextValue("Mods.AnalysisMod.Dialogue.AnalysisTravelingMerchant.StandardDialogue1"));
            chat.Add(Language.GetTextValue("Mods.AnalysisMod.Dialogue.AnalysisTravelingMerchant.StandardDialogue2"));
            chat.Add(Language.GetTextValue("Mods.AnalysisMod.Dialogue.AnalysisTravelingMerchant.StandardDialogue3"));

            string hivePackDialogue = Language.GetTextValue("Mods.AnalysisMod.Dialogue.AnalysisTravelingMerchant.HiveBackpackDialogue");
            chat.Add(hivePackDialogue);

            string dialogueLine = chat; // chat is implicitly cast to a string.
                                        // chat被隐式转换为字符串。
            if (hivePackDialogue.Equals(dialogueLine))
            {
                // Main.npcChatCornerItem shows a single item in the corner, like the Angler Quest chat.
                // Main.npcChatCornerItem在角落里显示单个物品，就像渔夫任务聊天一样。
                Main.npcChatCornerItem = ItemID.HiveBackpack;
            }

            return dialogueLine;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shop)
        {
            if (firstButton)
            {
                shop = Shop.Name; // Opens the shop
                                  // 打开商店
            }
        }

        public override void AI()
        {
            NPC.homeless = true; // Make sure it stays homeless
                                 // 确保它保持无家可归
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AnalysisCostume>()));
        }

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 20;
            knockback = 4f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 15;
            randExtraCooldown = 8;
        }

        public override void TownNPCAttackSwing(ref int itemWidth, ref int itemHeight)
        {
            itemWidth = itemHeight = 40;
        }

        public override void DrawTownAttackSwing(ref Texture2D item, ref Rectangle itemFrame, ref int itemSize, ref float scale, ref Vector2 offset)
        {
            Main.GetItemDrawFrame(ModContent.ItemType<AnalysisContent.Items.Weapons.AnalysisSword>(), out item, out itemFrame);
            itemSize = 40;
            // This adjustment draws the swing the way town npcs usually do.
            // 此调整使挥动方式与城镇npc通常所做的方式相同。
            if (NPC.ai[1] > NPCID.Sets.AttackTime[NPC.type] * 0.66f)
            {
                offset.Y = 12f;
            }
        }
    }

    // You have the freedom to implement custom shops however you want
    // This Analysis uses a 'pool' concept where items will be randomly selected from a pool with equal weight
    // We copy a bunch of code from NPCShop and NPCShop.Entry, allowing this shop to be easily adjusted by other mods. 
    // This uses some fairly advanced C# to avoid being accessively long, so make sure you learn the language before trying to adapt it significantly

    //您可以自由地按照自己的想法实现定制商店
    //此分析使用“池”概念，其中物品将从具有相等权重的池中随机选择
    //我们从NPCShop和NPCShop.Entry复制了一堆代码，使得其他模组可以轻松调整这个商店。
    //这里使用了一些相当高级的C#语言来避免过长，因此在尝试进行重大改编之前，请确保学习该语言。
    public class AnalysisTravelingMerchantShop : AbstractNPCShop
    {
        public new record Entry(Item Item, List<Condition> Conditions) : AbstractNPCShop.Entry
        {
            IEnumerable<Condition> AbstractNPCShop.Entry.Conditions => Conditions;

            public bool Disabled { get; private set; }

            public Entry Disable()
            {
                Disabled = true;
                return this;
            }

            public bool ConditionsMet() => Conditions.All(c => c.IsMet());
        }

        public record Pool(string Name, int Slots, List<Entry> Entries)
        {
            public Pool Add(Item item, params Condition[] conditions)
            {
                Entries.Add(new Entry(item, conditions.ToList()));
                return this;
            }

            public Pool Add<T>(params Condition[] conditions) where T : ModItem => Add(ModContent.ItemType<T>(), conditions);
            public Pool Add(int item, params Condition[] conditions) => Add(ContentSamples.ItemsByType[item], conditions);

            // Picks a number of items (up to Slots) from the entries list, provided conditions are met.
            // 如果满足条件，则从条目列表中选择一个数量（最多Slots）。
            public IEnumerable<Item> PickItems()
            {
                // This is not a fast way to pick items without replacement, but it's certainly easy. Be careful not to do this many many times per frame, or on huge lists of items.
                // 这不是一种快速无替换选取项目的方法，但它肯定很容易。请注意不要每帧执行太多次或在大量项目列表上执行此操作。
                var list = Entries.Where(e => !e.Disabled && e.ConditionsMet()).ToList();
                for (int i = 0; i < Slots; i++)
                {
                    if (list.Count == 0)
                        break;

                    int k = Main.rand.Next(list.Count);
                    yield return list[k].Item;

                    // remove the entry from the list so it can't be selected again this pick
                    // 删除列表中的条目以便不能再次选择
                    list.RemoveAt(k);
                }
            }
        }

        public List<Pool> Pools { get; } = new();

        public AnalysisTravelingMerchantShop(int npcType) : base(npcType) { }

        public override IEnumerable<Entry> ActiveEntries => Pools.SelectMany(p => p.Entries).Where(e => !e.Disabled);

        public Pool AddPool(string name, int slots)
        {
            var pool = new Pool(name, slots, new List<Entry>());
            Pools.Add(pool);
            return pool;
        }

        // Some methods to add a pool with a single item
        // 添加单个项池的某些方法
        public void Add(Item item, params Condition[] conditions) => AddPool(item.ModItem?.FullName ?? $"Terraria/{item.type}", slots: 1).Add(item, conditions);
        public void Add<T>(params Condition[] conditions) where T : ModItem => Add(ModContent.ItemType<T>(), conditions);
        public void Add(int item, params Condition[] conditions) => Add(ContentSamples.ItemsByType[item], conditions);

        // Here is where we actually 'roll' the contents of the shop
        //这里是我们实际上“卷”的商店内容
        public List<Item> GenerateNewInventoryList()
        {
            var items = new List<Item>();
            foreach (var pool in Pools)
            {
                items.AddRange(pool.PickItems());
            }
            return items;
        }

        public override void FillShop(ICollection<Item> items, NPC npc)
        {
            // use the items which were selected when the NPC spawned.
            // 使用生成NPC时选定的物品。
            foreach (var item in ((AnalysisTravelingMerchant)npc.ModNPC).shopItems)
            {
                // make sure to add a clone of the item, in case any ModifyActiveShop hooks adjust the item when the shop is opened
                // 确保添加物品副本，在打开商店时任何ModifyActiveShop钩子调整物品时都能生效
                items.Add(item.Clone());
            }
        }

        public override void FillShop(Item[] items, NPC npc, out bool overflow)
        {
            overflow = false;
            int i = 0;
            // use the items which were selected when the NPC spawned.
            // 使用生成NPC时选定的物品。
            foreach (var item in ((AnalysisTravelingMerchant)npc.ModNPC).shopItems)
            {

                if (i == items.Length - 1)
                {
                    // leave the last slot empty for selling
                    // 留下最后一个插槽用于出售
                    overflow = true;
                    return;
                }

                // make sure to add a clone of the item, in case any ModifyActiveShop hooks adjust the item when the shop is opened
                // 确保添加物品副本，在打开商店时任何ModifyActiveShop钩子调整物品时都能生效
                items[i++] = item.Clone();
            }
        }
    }
}
