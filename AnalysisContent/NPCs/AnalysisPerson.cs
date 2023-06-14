using AnalysisMod.AnalysisContent.Biomes;
using AnalysisMod.AnalysisContent.Dusts;
using AnalysisMod.AnalysisContent.Items.Accessories;
using AnalysisMod.AnalysisContent.Items.Armor;
using AnalysisMod.AnalysisContent.Tiles;
using AnalysisMod.AnalysisContent.Tiles.Furniture;
using AnalysisMod.AnalysisContent.Walls;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.GameContent.Personalities;
using System.Collections.Generic;
using Terraria.ModLoader.IO;
using AnalysisMod.AnalysisCommon.Configs;
using AnalysisMod.AnalysisCommon;
using AnalysisMod.AnalysisContent.Projectiles;
using AnalysisMod.AnalysisContent.Items;

namespace AnalysisMod.AnalysisContent.NPCs
{
    // [AutoloadHead] and NPC.townNPC are extremely important and absolutely both necessary for any Town NPC to work at all.
    // [AutoloadHead]和NPC.townNPC对于任何城镇NPC的工作都非常重要，绝对必不可少。
    [AutoloadHead]
    public class AnalysisPerson : ModNPC
    {
        public const string ShopName = "Shop";
        public int NumberOfTimesTalkedTo = 0;

        private static int ShimmerHeadIndex;
        private static Profiles.StackedNPCProfile NPCProfile;

        public override void Load()
        {
            // Adds our Shimmer Head to the NPCHeadLoader.
            //将我们的Shimmer Head添加到NPCHeadLoader中。
            ShimmerHeadIndex = Mod.AddNPCHeadTexture(Type, Texture + "_Shimmer_Head");
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 25; // The total amount of frames the NPC has
                                           // 该NPC拥有的总帧数

            NPCID.Sets.ExtraFramesCount[Type] = 9; // Generally for Town NPCs, but this is how the NPC does extra things such as sitting in a chair and talking to other NPCs. This is the remaining frames after the walking frames.
                                                   // 通常用于城镇NPC，但这是NPC执行额外操作（如坐在椅子上并与其他NPC交谈）的方式。这是行走帧之后剩余的帧数。

            NPCID.Sets.AttackFrameCount[Type] = 4; // The amount of frames in the attacking animation.
                                                   // 攻击动画中的帧数。

            NPCID.Sets.DangerDetectRange[Type] = 700; // The amount of pixels away from the center of the NPC that it tries to attack enemies.
                                                      // 距离该 NPC 中心多少像素时它会尝试攻击敌人。

            NPCID.Sets.AttackType[Type] = 0; // The type of attack the Town NPC performs. 0 = throwing, 1 = shooting, 2 = magic, 3 = melee
                                             // 城镇 NPC 执行的攻击类型。0 = 投掷、1 = 射击、2 = 魔法、3 = 近战

            NPCID.Sets.AttackTime[Type] = 90; // The amount of time it takes for the NPC's attack animation to be over once it starts.
                                              // 一旦开始，需要多长时间才能完成 NPC 的攻击动画。

            NPCID.Sets.AttackAverageChance[Type] = 30; // The denominator for the chance for a Town NPC to attack. Lower numbers make the Town NPC appear more aggressive.
                                                       // 用于 Town NPC 攻击几率分母。较低数字使 Town NPC 看起来更具侵略性。

            NPCID.Sets.HatOffsetY[Type] = 4; // For when a party is active, the party hat spawns at a Y offset.
                                             // 当派对活跃时，聚会帽出现在 Y 偏移处。

            NPCID.Sets.ShimmerTownTransform[NPC.type] = true; // This set says that the Town NPC has a Shimmered form. Otherwise, the Town NPC will become transparent when touching Shimmer like other enemies.
                                                              // 此设置表示 Town NPC 具有闪光形式。否则，Town NPC 会像其他敌人一样在接触 Shimmer 时变得透明。

            NPCID.Sets.ShimmerTownTransform[Type] = true; // Allows for this NPC to have a different texture after touching the Shimmer liquid.
                                                          // 允许此 NPC 在接触 Shimmer 液体后具有不同的纹理。

            // Influences how the NPC looks in the Bestiary
            // 影响 Bestiary 中显示该 NPC 的外观
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Velocity = 1f, // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
                               // 在 x 方向上以其行走 +1 格绘制最佳记录中的 NPC

                Direction = 1 // -1 is left and 1 is right. NPCs are drawn facing the left by default but AnalysisPerson will be drawn facing the right
                              // Rotation = MathHelper.ToRadians(180) // You can also change the rotation of an NPC. Rotation is measured in radians
                              // If you want to see an Analysis of manually modifying these when the NPC is drawn, see PreDraw

                              // -1 表示向左，1 表示向右。NPC 默认面朝左侧绘制，但 AnalysisPerson 将面朝右侧绘制。
                              // Rotation = MathHelper.ToRadians(180) // 您还可以更改 NPC 的旋转。旋转以弧度为单位测量
                              // 如果您想在绘制 NPC 时手动修改这些内容的分析，请参见 PreDraw
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

            // Set Analysis Person's biome and neighbor preferences with the NPCHappiness hook. You can add happiness text and remarks with localization (See an Analysis in AnalysisMod/Localization/en-US.lang).
            // NOTE: The following code uses chaining - a style that works due to the fact that the SetXAffection methods return the same NPCHappiness instance they're called on.

            // 使用 NPCHappiness 钩子设置 Analysis Person 的生物群系和邻居偏好。您可以使用本地化添加幸福文本和备注（请参阅 AnalysisMod/Localization/en-US.lang 中的分析）。
            // 注意：以下代码使用链接-一种由于 SetXAffection 方法返回它们调用的相同 NPCHappiness 实例而起作用的样式。
            //【我不得不承认，翻译这些东西是个大挑战，比如该如何比较？去他的】

            NPC.Happiness
                .SetBiomeAffection<ForestBiome>(AffectionLevel.Like) // Analysis Person prefers the forest.
                                                                     // 分析人喜欢森林。

                .SetBiomeAffection<SnowBiome>(AffectionLevel.Dislike) // Analysis Person dislikes the snow.
                                                                      // 分析人不喜欢雪。

                .SetBiomeAffection<AnalysisSurfaceBiome>(AffectionLevel.Love) // Analysis Person likes the Analysis Surface Biome
                                                                              // 分析人喜欢表层生物群系。

                .SetNPCAffection(NPCID.Dryad, AffectionLevel.Love) // Loves living near the dryad.
                                                                   // 喜欢住在树精附近。

                .SetNPCAffection(NPCID.Guide, AffectionLevel.Like) // Likes living near the guide.
                                                                   // 喜欢住在向导附近。

                .SetNPCAffection(NPCID.Merchant, AffectionLevel.Dislike) // Dislikes living near the merchant.
                                                                         // 不喜欢住在商人附近。

                .SetNPCAffection(NPCID.Demolitionist, AffectionLevel.Hate) // Hates living near the demolitionist.
                                                                           // 讨厌住在爆破专家附近。

            ; // < Mind the semicolon!
              // 注意分号！

            // This creates a "profile" for AnalysisPerson, which allows for different textures during a party and/or while the NPC is shimmered.
            // 这将为AnalysisPerson创建一个“配置文件”，允许在聚会期间和/或NPC闪烁时使用不同的纹理
            NPCProfile = new Profiles.StackedNPCProfile(
                new Profiles.DefaultNPCProfile(Texture, NPCHeadLoader.GetHeadSlot(HeadTexture), Texture + "_Party"),
                new Profiles.DefaultNPCProfile(Texture + "_Shimmer", ShimmerHeadIndex, Texture + "_Shimmer_Party")
            );
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true; // Sets NPC to be a Town NPC
                                // 设置NPC为城镇NPC

            NPC.friendly = true; // NPC Will not attack player
                                 // NPC不会攻击玩家

            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = 7;
            NPC.damage = 10;
            NPC.defense = 15;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;

            AnimationType = NPCID.Guide;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            // 我们可以使用AddRange而不是多次调用Add来一次添加多个项目
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.

                // 在图鉴中列出此城镇NPC首选的生物群系。
                // 对于城镇NPC，通常将其设置为与NPC幸福度相关的最受青睐的生物群系。
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,

				// Sets your NPC's flavor text in the bestiary.
                // 在图鉴中设置您的NPC口味文本。
				new FlavorTextBestiaryInfoElement("Hailing from a mysterious greyscale cube world, the Analysis Person is here to help you understand everything about tModLoader."),

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)

                // 如果你真想要，你可以添加多个元素
                // 您还可以使用本地化键（请参见Localization / en-US.lang）
				new FlavorTextBestiaryInfoElement("Mods.AnalysisMod.Bestiary.AnalysisPerson")
            });
        }

        // The PreDraw hook is useful for drawing things before our sprite is drawn or running code before the sprite is drawn
        // Returning false will allow you to manually draw your NPC

        // PreDraw钩子对于绘制我们的精灵图之前绘制东西或运行代码非常有用
        // 返回false将允许您手动绘制您的 NPC
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            // This code slowly rotates the NPC in the bestiary
            // (simply checking NPC.IsABestiaryIconDummy and incrementing NPC.Rotation won't work here as it gets overridden by drawModifiers.Rotation each tick)

            // 此代码缓慢旋转图鉴中的 NPC
            //(仅检查NPC.IsABestiaryIconDummy并递增NPC.Rotation在这里不起作用，因为它每个tick都被drawModifiers.Rotation覆盖)
            if (NPCID.Sets.NPCBestiaryDrawOffset.TryGetValue(Type, out NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers))
            {
                drawModifiers.Rotation += 0.001f;

                // Replace the existing NPCBestiaryDrawModifiers with our new one with an adjusted rotation
                // 用调整后的旋转替换现有的 NPCBestiaryDrawModifiers
                NPCID.Sets.NPCBestiaryDrawOffset.Remove(Type);
                NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
            }

            return true;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            int num = NPC.life > 0 ? 1 : 5;

            for (int k = 0; k < num; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<Sparkle>());
            }

            // Create gore when the NPC is killed.
            // 在NPC死亡时创建尸块碎片。
            if (Main.netMode != NetmodeID.Server && NPC.life <= 0)
            {
                // Retrieve the gore types. This NPC has shimmer and party variants for head, arm, and leg gore. (12 total gores)
                // 检索碎片类型。此NPC具有头部、手臂和腿部碎片的闪烁和聚会变体。（共12个碎片）
                string variant = "";
                if (NPC.IsShimmerVariant) variant += "_Shimmer";
                if (NPC.altTexture == 1) variant += "_Party";
                int hatGore = NPC.GetPartyHatGore();
                int headGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Head").Type;
                int armGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Arm").Type;
                int legGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Leg").Type;

                // Spawn the gores. The positions of the arms and legs are lowered for a more natural look.
                // 生成碎片。将手臂和腿部的位置降低以获得更自然的外观。
                if (hatGore > 0)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, hatGore);
                }
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, headGore, 1f);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 20), NPC.velocity, armGore);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 20), NPC.velocity, armGore);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 34), NPC.velocity, legGore);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 34), NPC.velocity, legGore);
            }
        }

        public override bool CanTownNPCSpawn(int numTownNPCs)
        {   // Requirements for the town NPC to spawn.
            // 城镇NPC生成要求。
            for (int k = 0; k < Main.maxPlayers; k++)
            {
                Player player = Main.player[k];
                if (!player.active)
                {
                    continue;
                }

                // Player has to have either an AnalysisItem or an AnalysisBlock in order for the NPC to spawn
                // 玩家必须拥有AnalysisItem或AnalysisBlock之一才能使NPC生成
                if (player.inventory.Any(item => item.type == ModContent.ItemType<AnalysisItem>() || item.type == ModContent.ItemType<Items.Placeable.AnalysisBlock>()))
                {
                    return true;
                }
            }

            return false;
        }

        // Analysis Person needs a house built out of AnalysisMod tiles. You can delete this whole method in your townNPC for the regular house conditions.
        // Analysis Person需要建造由AnalysisMod瓷砖构成的房屋。您可以在townNPC中删除整个方法以获取常规房屋条件。
        public override bool CheckConditions(int left, int right, int top, int bottom)
        {
            int score = 0;
            for (int x = left; x <= right; x++)
            {
                for (int y = top; y <= bottom; y++)
                {
                    int type = Main.tile[x, y].TileType;
                    if (type == ModContent.TileType<AnalysisBlock>() || type == ModContent.TileType<AnalysisChair>() || type == ModContent.TileType<AnalysisWorkbench>() || type == ModContent.TileType<AnalysisBed>() || type == ModContent.TileType<AnalysisDoorOpen>() || type == ModContent.TileType<AnalysisDoorClosed>())
                    {
                        score++;
                    }

                    if (Main.tile[x, y].WallType == ModContent.WallType<AnalysisWall>())
                    {
                        score++;
                    }
                }
            }

            return score >= (right - left) * (bottom - top) / 2;
        }

        public override ITownNPCProfile TownNPCProfile()
        {
            return NPCProfile;
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string>() {
                "Someone",
                "Somebody",
                "Blocky",
                "Colorless"
            };
        }

        public override void FindFrame(int frameHeight)
        {
            /*npc.frame.Width = 40;
			if (((int)Main.time / 10) % 2 == 0)
			{
				npc.frame.X = 40;
			}
			else
			{
				npc.frame.X = 0;
			}*/
        }

        public override string GetChat()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();

            int partyGirl = NPC.FindFirstNPC(NPCID.PartyGirl);
            if (partyGirl >= 0 && Main.rand.NextBool(4))
            {
                chat.Add(Language.GetTextValue("Mods.AnalysisMod.Dialogue.AnalysisPerson.PartyGirlDialogue", Main.npc[partyGirl].GivenName));
            }
            // These are things that the NPC has a chance of telling you when you talk to it.
            // 这些是与你交谈时该 NPC 可能告诉你的事情。
            chat.Add(Language.GetTextValue("Mods.AnalysisMod.Dialogue.AnalysisPerson.StandardDialogue1"));
            chat.Add(Language.GetTextValue("Mods.AnalysisMod.Dialogue.AnalysisPerson.StandardDialogue2"));
            chat.Add(Language.GetTextValue("Mods.AnalysisMod.Dialogue.AnalysisPerson.StandardDialogue3"));
            chat.Add(Language.GetTextValue("Mods.AnalysisMod.Dialogue.AnalysisPerson.CommonDialogue"), 5.0);
            chat.Add(Language.GetTextValue("Mods.AnalysisMod.Dialogue.AnalysisPerson.RareDialogue"), 0.1);

            NumberOfTimesTalkedTo++;
            if (NumberOfTimesTalkedTo >= 10)
            {
                // This counter is linked to a single instance of the NPC, so if AnalysisPerson is killed, the counter will reset.
                // 此计数器链接到单个 NPC 实例，因此如果分析人被杀死，则计数器将重置。
                chat.Add(Language.GetTextValue("Mods.AnalysisMod.Dialogue.AnalysisPerson.TalkALot"));
            }

            return chat; // chat is implicitly cast to a string.
                         // chat隐式转换为字符串。
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {   // What the chat buttons are when you open up the chat UI
            // 打开聊天界面时聊天按钮是什么
            button = Language.GetTextValue("LegacyInterface.28");
            button2 = "Awesomeify";
            if (Main.LocalPlayer.HasItem(ItemID.HiveBackpack))
            {
                button = "Upgrade " + Lang.GetItemNameValue(ItemID.HiveBackpack);
            }
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shop)
        {
            if (firstButton)
            {
                // We want 3 different functionalities for chat buttons, so we use HasItem to change button 1 between a shop and upgrade action.
                //我们希望聊天按钮有3种不同的功能，因此我们使用HasItem来在商店和升级操作之间更改按钮1。

                if (Main.LocalPlayer.HasItem(ItemID.HiveBackpack))
                {
                    SoundEngine.PlaySound(SoundID.Item37); // Reforge/Anvil sound
                                                           // 重铸/砧声音

                    Main.npcChatText = $"I upgraded your {Lang.GetItemNameValue(ItemID.HiveBackpack)} to a {Lang.GetItemNameValue(ModContent.ItemType<WaspNest>())}";

                    int hiveBackpackItemIndex = Main.LocalPlayer.FindItem(ItemID.HiveBackpack);
                    var entitySource = NPC.GetSource_GiftOrReward();

                    Main.LocalPlayer.inventory[hiveBackpackItemIndex].TurnToAir();
                    Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<WaspNest>());

                    return;
                }

                shop = ShopName; // Name of the shop tab we want to open.
                                 // 我们要打开的商店选项卡名称。
            }
        }

        // Not completely finished, but below is what the NPC will sell
        //尚未完全完成，但以下是NPC将出售的物品
        public override void AddShops()
        {
            var npcShop = new NPCShop(Type, ShopName)
                .Add<AnalysisItem>()
                //.Add<EquipMaterial>()
                //.Add<BossItem>()
                .Add(new Item(ModContent.ItemType<Items.Placeable.Furniture.AnalysisWorkbench>()) { shopCustomPrice = Item.buyPrice(copper: 15) }) // This Analysis sets a custom price, AnalysisNPCShop.cs has more info on custom prices and currency.
                                                                                                                                                   // 此分析设置自定义价格，AnalysisNPCShop.cs中有关于自定义价格和货币的更多信息。
                .Add<Items.Placeable.Furniture.AnalysisChair>()
                .Add<Items.Placeable.Furniture.AnalysisDoor>()
                .Add<Items.Placeable.Furniture.AnalysisBed>()
                .Add<Items.Placeable.Furniture.AnalysisChest>()
                .Add<AnalysisContent.Items.Tools.AnalysisPickaxe>()
                .Add<AnalysisContent.Items.Tools.AnalysisHamaxe>()
                .Add<Items.Consumables.AnalysisHealingPotion>(new Condition("Mods.AnalysisMod.Conditions.PlayerHasLifeforceBuff", () => Main.LocalPlayer.HasBuff(BuffID.Lifeforce)))
                .Add<Items.Weapons.AnalysisSword>(Condition.MoonPhasesQuarter0)
                //.Add<AnalysisGun>(Condition.MoonPhasesQuarter1)
                .Add<Items.Ammo.AnalysisBullet>(Condition.MoonPhasesQuarter1)
                //.Add<AnalysisStaff>(Condition.MoonPhasesQuarter2)
                .Add<AnalysisOnBuyItem>()
                .Add<Items.Weapons.AnalysisYoyo>(Condition.IsNpcShimmered); // Let's sell an yoyo if this NPC is shimmered!
                                                                            // 如果这个NPC被闪烁了，让我们卖一个悠悠球！

            if (ModContent.GetInstance<AnalysisModConfig>().AnalysisWingsToggle)
            {
                npcShop.Add<AnalysisWings>(AnalysisConditions.InAnalysisBiome);
            }

            if (ModContent.TryFind("SummonersAssociation/BloodTalisman", out ModItem bloodTalisman))
            {
                npcShop.Add(bloodTalisman.Type);
            }
            npcShop.Register(); // Name of this shop tab
                                // 这个商店选项卡的名称
        }

        public override void ModifyActiveShop(string shopName, Item[] items)
        {
            foreach (Item item in items)
            {
                // Skip 'air' items and null items.
                // 跳过“空气”物品和空项目。
                if (item == null || item.type == ItemID.None)
                {
                    continue;
                }

                // If NPC is shimmered then reduce all prices by 50%.
                // 如果NPC被闪烁，则所有价格减少50％。
                if (NPC.IsShimmerVariant)
                {
                    int value = item.shopCustomPrice ?? item.value;
                    item.shopCustomPrice = value / 2;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AnalysisCostume>()));
        }

        // Make this Town NPC teleport to the King and/or Queen statue when triggered. Return toKingStatue for only King Statues. Return !toKingStatue for only Queen Statues. Return true for both.
        // 当触发时使该城镇NPC传送到国王和/或女王雕像。仅返回toKingStatue以获取国王雕像。仅返回！toKingStatue以获取女王雕像。对于两者都返回true。
        public override bool CanGoToStatue(bool toKingStatue) => true;

        // Make something happen when the npc teleports to a statue. Since this method only runs server side, any visual effects like dusts or gores have to be synced across all clients manually.
        // 当npc传送到雕像时发生某些事情。由于此方法仅在服务器端运行，因此必须手动在所有客户端上同步任何视觉效果（如灰尘或血液）等内容。
        public override void OnGoToStatue(bool toKingStatue)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)AnalysisMod.MessageType.AnalysisTeleportToStatue);
                packet.Write((byte)NPC.whoAmI);
                packet.Send();
            }
            else
            {
                StatueTeleport();
            }
        }

        // Create a square of pixels around the NPC on teleport.
        // 创建一个围绕npc传送点的像素正方形区域。
        public void StatueTeleport()
        {
            for (int i = 0; i < 30; i++)
            {
                Vector2 position = Main.rand.NextVector2Square(-20, 21);
                if (Math.Abs(position.X) > Math.Abs(position.Y))
                {
                    position.X = Math.Sign(position.X) * 20;
                }
                else
                {
                    position.Y = Math.Sign(position.Y) * 20;
                }

                Dust.NewDustPerfect(NPC.Center + position, ModContent.DustType<Sparkle>(), Vector2.Zero).noGravity = true;
            }
        }

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 20;
            knockback = 4f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 30;
            randExtraCooldown = 30;
        }

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ModContent.ProjectileType<AnalysisSparklingBall>();
            attackDelay = 1;
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 12f;
            randomOffset = 2f;
            // SparklingBall is not affected by gravity, so gravityCorrection is left alone.
            // SparklingBall不受重力影响，因此保持gravityCorrection不变。
        }

        public override void LoadData(TagCompound tag)
        {
            NumberOfTimesTalkedTo = tag.GetInt("numberOfTimesTalkedTo");
        }

        public override void SaveData(TagCompound tag)
        {
            tag["numberOfTimesTalkedTo"] = NumberOfTimesTalkedTo;
        }
    }
}