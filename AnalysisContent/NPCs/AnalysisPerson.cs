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
    // [AutoloadHead]��NPC.townNPC�����κγ���NPC�Ĺ������ǳ���Ҫ�����Աز����١�
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
            //�����ǵ�Shimmer Head��ӵ�NPCHeadLoader�С�
            ShimmerHeadIndex = Mod.AddNPCHeadTexture(Type, Texture + "_Shimmer_Head");
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 25; // The total amount of frames the NPC has
                                           // ��NPCӵ�е���֡��

            NPCID.Sets.ExtraFramesCount[Type] = 9; // Generally for Town NPCs, but this is how the NPC does extra things such as sitting in a chair and talking to other NPCs. This is the remaining frames after the walking frames.
                                                   // ͨ�����ڳ���NPC��������NPCִ�ж�������������������ϲ�������NPC��̸���ķ�ʽ����������֮֡��ʣ���֡����

            NPCID.Sets.AttackFrameCount[Type] = 4; // The amount of frames in the attacking animation.
                                                   // ���������е�֡����

            NPCID.Sets.DangerDetectRange[Type] = 700; // The amount of pixels away from the center of the NPC that it tries to attack enemies.
                                                      // ����� NPC ���Ķ�������ʱ���᳢�Թ������ˡ�

            NPCID.Sets.AttackType[Type] = 0; // The type of attack the Town NPC performs. 0 = throwing, 1 = shooting, 2 = magic, 3 = melee
                                             // ���� NPC ִ�еĹ������͡�0 = Ͷ����1 = �����2 = ħ����3 = ��ս

            NPCID.Sets.AttackTime[Type] = 90; // The amount of time it takes for the NPC's attack animation to be over once it starts.
                                              // һ����ʼ����Ҫ�೤ʱ�������� NPC �Ĺ���������

            NPCID.Sets.AttackAverageChance[Type] = 30; // The denominator for the chance for a Town NPC to attack. Lower numbers make the Town NPC appear more aggressive.
                                                       // ���� Town NPC �������ʷ�ĸ���ϵ�����ʹ Town NPC ���������������ԡ�

            NPCID.Sets.HatOffsetY[Type] = 4; // For when a party is active, the party hat spawns at a Y offset.
                                             // ���ɶԻ�Ծʱ���ۻ�ñ������ Y ƫ�ƴ���

            NPCID.Sets.ShimmerTownTransform[NPC.type] = true; // This set says that the Town NPC has a Shimmered form. Otherwise, the Town NPC will become transparent when touching Shimmer like other enemies.
                                                              // �����ñ�ʾ Town NPC ����������ʽ������Town NPC ������������һ���ڽӴ� Shimmer ʱ���͸����

            NPCID.Sets.ShimmerTownTransform[Type] = true; // Allows for this NPC to have a different texture after touching the Shimmer liquid.
                                                          // ����� NPC �ڽӴ� Shimmer Һ�����в�ͬ������

            // Influences how the NPC looks in the Bestiary
            // Ӱ�� Bestiary ����ʾ�� NPC �����
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Velocity = 1f, // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
                               // �� x �������������� +1 �������Ѽ�¼�е� NPC

                Direction = 1 // -1 is left and 1 is right. NPCs are drawn facing the left by default but AnalysisPerson will be drawn facing the right
                              // Rotation = MathHelper.ToRadians(180) // You can also change the rotation of an NPC. Rotation is measured in radians
                              // If you want to see an Analysis of manually modifying these when the NPC is drawn, see PreDraw

                              // -1 ��ʾ����1 ��ʾ���ҡ�NPC Ĭ���泯�����ƣ��� AnalysisPerson ���泯�Ҳ���ơ�
                              // Rotation = MathHelper.ToRadians(180) // �������Ը��� NPC ����ת����ת�Ի���Ϊ��λ����
                              // ��������ڻ��� NPC ʱ�ֶ��޸���Щ���ݵķ�������μ� PreDraw
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

            // Set Analysis Person's biome and neighbor preferences with the NPCHappiness hook. You can add happiness text and remarks with localization (See an Analysis in AnalysisMod/Localization/en-US.lang).
            // NOTE: The following code uses chaining - a style that works due to the fact that the SetXAffection methods return the same NPCHappiness instance they're called on.

            // ʹ�� NPCHappiness �������� Analysis Person ������Ⱥϵ���ھ�ƫ�á�������ʹ�ñ��ػ�����Ҹ��ı��ͱ�ע������� AnalysisMod/Localization/en-US.lang �еķ�������
            // ע�⣺���´���ʹ������-һ������ SetXAffection �����������ǵ��õ���ͬ NPCHappiness ʵ���������õ���ʽ��
            //���Ҳ��ò����ϣ�������Щ�����Ǹ�����ս���������αȽϣ�ȥ���ġ�

            NPC.Happiness
                .SetBiomeAffection<ForestBiome>(AffectionLevel.Like) // Analysis Person prefers the forest.
                                                                     // ������ϲ��ɭ�֡�

                .SetBiomeAffection<SnowBiome>(AffectionLevel.Dislike) // Analysis Person dislikes the snow.
                                                                      // �����˲�ϲ��ѩ��

                .SetBiomeAffection<AnalysisSurfaceBiome>(AffectionLevel.Love) // Analysis Person likes the Analysis Surface Biome
                                                                              // ������ϲ���������Ⱥϵ��

                .SetNPCAffection(NPCID.Dryad, AffectionLevel.Love) // Loves living near the dryad.
                                                                   // ϲ��ס������������

                .SetNPCAffection(NPCID.Guide, AffectionLevel.Like) // Likes living near the guide.
                                                                   // ϲ��ס���򵼸�����

                .SetNPCAffection(NPCID.Merchant, AffectionLevel.Dislike) // Dislikes living near the merchant.
                                                                         // ��ϲ��ס�����˸�����

                .SetNPCAffection(NPCID.Demolitionist, AffectionLevel.Hate) // Hates living near the demolitionist.
                                                                           // ����ס�ڱ���ר�Ҹ�����

            ; // < Mind the semicolon!
              // ע��ֺţ�

            // This creates a "profile" for AnalysisPerson, which allows for different textures during a party and/or while the NPC is shimmered.
            // �⽫ΪAnalysisPerson����һ���������ļ����������ھۻ��ڼ��/��NPC��˸ʱʹ�ò�ͬ������
            NPCProfile = new Profiles.StackedNPCProfile(
                new Profiles.DefaultNPCProfile(Texture, NPCHeadLoader.GetHeadSlot(HeadTexture), Texture + "_Party"),
                new Profiles.DefaultNPCProfile(Texture + "_Shimmer", ShimmerHeadIndex, Texture + "_Shimmer_Party")
            );
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true; // Sets NPC to be a Town NPC
                                // ����NPCΪ����NPC

            NPC.friendly = true; // NPC Will not attack player
                                 // NPC���ṥ�����

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
            // ���ǿ���ʹ��AddRange�����Ƕ�ε���Add��һ����Ӷ����Ŀ
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.

                // ��ͼ�����г��˳���NPC��ѡ������Ⱥϵ��
                // ���ڳ���NPC��ͨ����������Ϊ��NPC�Ҹ�����ص���������������Ⱥϵ��
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,

				// Sets your NPC's flavor text in the bestiary.
                // ��ͼ������������NPC��ζ�ı���
				new FlavorTextBestiaryInfoElement("Hailing from a mysterious greyscale cube world, the Analysis Person is here to help you understand everything about tModLoader."),

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)

                // ���������Ҫ���������Ӷ��Ԫ��
                // ��������ʹ�ñ��ػ�������μ�Localization / en-US.lang��
				new FlavorTextBestiaryInfoElement("Mods.AnalysisMod.Bestiary.AnalysisPerson")
            });
        }

        // The PreDraw hook is useful for drawing things before our sprite is drawn or running code before the sprite is drawn
        // Returning false will allow you to manually draw your NPC

        // PreDraw���Ӷ��ڻ������ǵľ���ͼ֮ǰ���ƶ��������д���ǳ�����
        // ����false���������ֶ��������� NPC
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            // This code slowly rotates the NPC in the bestiary
            // (simply checking NPC.IsABestiaryIconDummy and incrementing NPC.Rotation won't work here as it gets overridden by drawModifiers.Rotation each tick)

            // �˴��뻺����תͼ���е� NPC
            //(�����NPC.IsABestiaryIconDummy������NPC.Rotation�����ﲻ�����ã���Ϊ��ÿ��tick����drawModifiers.Rotation����)
            if (NPCID.Sets.NPCBestiaryDrawOffset.TryGetValue(Type, out NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers))
            {
                drawModifiers.Rotation += 0.001f;

                // Replace the existing NPCBestiaryDrawModifiers with our new one with an adjusted rotation
                // �õ��������ת�滻���е� NPCBestiaryDrawModifiers
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
            // ��NPC����ʱ����ʬ����Ƭ��
            if (Main.netMode != NetmodeID.Server && NPC.life <= 0)
            {
                // Retrieve the gore types. This NPC has shimmer and party variants for head, arm, and leg gore. (12 total gores)
                // ������Ƭ���͡���NPC����ͷ�����ֱۺ��Ȳ���Ƭ����˸�;ۻ���塣����12����Ƭ��
                string variant = "";
                if (NPC.IsShimmerVariant) variant += "_Shimmer";
                if (NPC.altTexture == 1) variant += "_Party";
                int hatGore = NPC.GetPartyHatGore();
                int headGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Head").Type;
                int armGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Arm").Type;
                int legGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Leg").Type;

                // Spawn the gores. The positions of the arms and legs are lowered for a more natural look.
                // ������Ƭ�����ֱۺ��Ȳ���λ�ý����Ի�ø���Ȼ����ۡ�
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
            // ����NPC����Ҫ��
            for (int k = 0; k < Main.maxPlayers; k++)
            {
                Player player = Main.player[k];
                if (!player.active)
                {
                    continue;
                }

                // Player has to have either an AnalysisItem or an AnalysisBlock in order for the NPC to spawn
                // ��ұ���ӵ��AnalysisItem��AnalysisBlock֮һ����ʹNPC����
                if (player.inventory.Any(item => item.type == ModContent.ItemType<AnalysisItem>() || item.type == ModContent.ItemType<Items.Placeable.AnalysisBlock>()))
                {
                    return true;
                }
            }

            return false;
        }

        // Analysis Person needs a house built out of AnalysisMod tiles. You can delete this whole method in your townNPC for the regular house conditions.
        // Analysis Person��Ҫ������AnalysisMod��ש���ɵķ��ݡ���������townNPC��ɾ�����������Ի�ȡ���淿��������
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
            // ��Щ�����㽻̸ʱ�� NPC ���ܸ���������顣
            chat.Add(Language.GetTextValue("Mods.AnalysisMod.Dialogue.AnalysisPerson.StandardDialogue1"));
            chat.Add(Language.GetTextValue("Mods.AnalysisMod.Dialogue.AnalysisPerson.StandardDialogue2"));
            chat.Add(Language.GetTextValue("Mods.AnalysisMod.Dialogue.AnalysisPerson.StandardDialogue3"));
            chat.Add(Language.GetTextValue("Mods.AnalysisMod.Dialogue.AnalysisPerson.CommonDialogue"), 5.0);
            chat.Add(Language.GetTextValue("Mods.AnalysisMod.Dialogue.AnalysisPerson.RareDialogue"), 0.1);

            NumberOfTimesTalkedTo++;
            if (NumberOfTimesTalkedTo >= 10)
            {
                // This counter is linked to a single instance of the NPC, so if AnalysisPerson is killed, the counter will reset.
                // �˼��������ӵ����� NPC ʵ���������������˱�ɱ����������������á�
                chat.Add(Language.GetTextValue("Mods.AnalysisMod.Dialogue.AnalysisPerson.TalkALot"));
            }

            return chat; // chat is implicitly cast to a string.
                         // chat��ʽת��Ϊ�ַ�����
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {   // What the chat buttons are when you open up the chat UI
            // ���������ʱ���찴ť��ʲô
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
                //����ϣ�����찴ť��3�ֲ�ͬ�Ĺ��ܣ��������ʹ��HasItem�����̵����������֮����İ�ť1��

                if (Main.LocalPlayer.HasItem(ItemID.HiveBackpack))
                {
                    SoundEngine.PlaySound(SoundID.Item37); // Reforge/Anvil sound
                                                           // ����/������

                    Main.npcChatText = $"I upgraded your {Lang.GetItemNameValue(ItemID.HiveBackpack)} to a {Lang.GetItemNameValue(ModContent.ItemType<WaspNest>())}";

                    int hiveBackpackItemIndex = Main.LocalPlayer.FindItem(ItemID.HiveBackpack);
                    var entitySource = NPC.GetSource_GiftOrReward();

                    Main.LocalPlayer.inventory[hiveBackpackItemIndex].TurnToAir();
                    Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<WaspNest>());

                    return;
                }

                shop = ShopName; // Name of the shop tab we want to open.
                                 // ����Ҫ�򿪵��̵�ѡ����ơ�
            }
        }

        // Not completely finished, but below is what the NPC will sell
        //��δ��ȫ��ɣ���������NPC�����۵���Ʒ
        public override void AddShops()
        {
            var npcShop = new NPCShop(Type, ShopName)
                .Add<AnalysisItem>()
                //.Add<EquipMaterial>()
                //.Add<BossItem>()
                .Add(new Item(ModContent.ItemType<Items.Placeable.Furniture.AnalysisWorkbench>()) { shopCustomPrice = Item.buyPrice(copper: 15) }) // This Analysis sets a custom price, AnalysisNPCShop.cs has more info on custom prices and currency.
                                                                                                                                                   // �˷��������Զ���۸�AnalysisNPCShop.cs���й����Զ���۸�ͻ��ҵĸ�����Ϣ��
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
                                                                            // ������NPC����˸�ˣ���������һ��������

            if (ModContent.GetInstance<AnalysisModConfig>().AnalysisWingsToggle)
            {
                npcShop.Add<AnalysisWings>(AnalysisConditions.InAnalysisBiome);
            }

            if (ModContent.TryFind("SummonersAssociation/BloodTalisman", out ModItem bloodTalisman))
            {
                npcShop.Add(bloodTalisman.Type);
            }
            npcShop.Register(); // Name of this shop tab
                                // ����̵�ѡ�������
        }

        public override void ModifyActiveShop(string shopName, Item[] items)
        {
            foreach (Item item in items)
            {
                // Skip 'air' items and null items.
                // ��������������Ʒ�Ϳ���Ŀ��
                if (item == null || item.type == ItemID.None)
                {
                    continue;
                }

                // If NPC is shimmered then reduce all prices by 50%.
                // ���NPC����˸�������м۸����50����
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
        // ������ʱʹ�ó���NPC���͵�������/��Ů�����񡣽�����toKingStatue�Ի�ȡ�������񡣽����أ�toKingStatue�Ի�ȡŮ�����񡣶������߶�����true��
        public override bool CanGoToStatue(bool toKingStatue) => true;

        // Make something happen when the npc teleports to a statue. Since this method only runs server side, any visual effects like dusts or gores have to be synced across all clients manually.
        // ��npc���͵�����ʱ����ĳЩ���顣���ڴ˷������ڷ����������У���˱����ֶ������пͻ�����ͬ���κ��Ӿ�Ч������ҳ���ѪҺ�������ݡ�
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
        // ����һ��Χ��npc���͵����������������
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
            // SparklingBall��������Ӱ�죬��˱���gravityCorrection���䡣
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