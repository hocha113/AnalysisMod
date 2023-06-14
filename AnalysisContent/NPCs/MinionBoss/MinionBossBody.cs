using AnalysisMod.AnalysisCommon.Systems;
using AnalysisMod.AnalysisContent.BossBars;
using AnalysisMod.AnalysisContent.Items;
using AnalysisMod.AnalysisContent.Items.Armor.Vanity;
using AnalysisMod.AnalysisContent.Items.Consumables;
using AnalysisMod.AnalysisContent.Pets.MinionBossPet;
using AnalysisMod.AnalysisContent.Projectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.NPCs.MinionBoss
{
    // The main part of the boss, usually refered to as "body"
    // Boss的主要部分，通常指“身体”
    [AutoloadBossHead] // This attribute looks for a texture called "ClassName_Head_Boss" and automatically registers it as the NPC boss head icon
                       // 该属性寻找名为“ClassName_Head_Boss”的纹理，并自动将其注册为NPC boss头像

    public class MinionBossBody : ModNPC
    {
        // This boss has a second phase and we want to give it a second boss head icon, this variable keeps track of the registered texture from Load().
        // It is applied in the BossHeadSlot hook when the boss is in its second stage

        // 这个Boss有第二阶段，我们想给它一个第二个boss头像图标，这个变量跟踪从Load()中注册的纹理。
        // 当Boss处于第二阶段时，在BossHeadSlot钩子中应用它。
        public static int secondStageHeadSlot = -1;

        // This code here is called a property: It acts like a variable, but can modify other things. In this case it uses the NPC.ai[] array that has four entries.
        // We use properties because it makes code more readable ("if (SecondStage)" vs "if (NPC.ai[0] == 1f)").
        // We use NPC.ai[] because in combination with NPC.netUpdate we can make it multiplayer compatible. Otherwise (making our own fields) we would have to write extra code to make it work (not covered here)

        // 我们使用属性是因为它使代码更易读（“if (SecondStage)” vs “if (NPC.ai[0] == 1f)”）。
        // 我们使用NPC.ai[]是因为与NPC.netUpdate结合使用可以使其多人游戏兼容。
        // 否则（制作自己的字段），我们需要编写额外的代码来使其工作（此处未涉及）
        public bool SecondStage
        {
            get => NPC.ai[0] == 1f;
            set => NPC.ai[0] = value ? 1f : 0f;
        }
        // If your boss has more than two stages, and since this is a boolean and can only be two things (true, false), concider using an integer or enum
        // 如果你的Boss有超过两个阶段，并且由于布尔值只能是两件事（真、假），请考虑使用整数或枚举

        // More advanced usage of a property, used to wrap around to floats to act as a Vector2
        // 属性更高级别用法，用于包装到浮点型以充当Vector2
        public Vector2 FirstStageDestination
        {
            get => new Vector2(NPC.ai[1], NPC.ai[2]);
            set
            {
                NPC.ai[1] = value.X;
                NPC.ai[2] = value.Y;
            }
        }

        public int MinionMaxHealthTotal
        {
            get => (int)NPC.ai[3];
            set => NPC.ai[3] = value;
        }

        public int MinionHealthTotal { get; set; }

        // Auto-implemented property, acts exactly like a variable by using a hidden backing field
        // 自动实现属性与隐藏后备字段完全相同
        public Vector2 LastFirstStageDestination { get; set; } = Vector2.Zero;

        // This property uses NPC.localAI[] instead which doesn't get synced, but because SpawnedMinions is only used on spawn as a flag, this will get set by all parties to true.
        // Knowing what side (client, server, all) is in charge of a variable is important as NPC.ai[] only has four entries, so choose wisely which things you need synced and not synced

        // 此属性改用不会同步化的 NPC.localAI[] ，但由于 SpawnedMinions 仅在生成时用作标志，因此所有方都将其设置为 true。
        // 知道哪一方（客户端、服务器、全部）负责变量很重要，因为NPC.ai[]只有四个条目，所以明智地选择需要同步和不需要同步的内容
        public bool SpawnedMinions
        {
            get => NPC.localAI[0] == 1f;
            set => NPC.localAI[0] = value ? 1f : 0f;
        }

        private const int FirstStageTimerMax = 90;
        // This is a reference property. It lets us write FirstStageTimer as if it's NPC.localAI[1], essentially giving it our own name
        // 这是一个引用属性。它让我们像使用 NPC.localAI[1] 一样编写 FirstStageTimer ，从本质上给它自己的名称
        public ref float FirstStageTimer => ref NPC.localAI[1];

        // We could also repurpose FirstStageTimer since it's unused in the second stage, or write "=> ref FirstStageTimer", but then we have to reset the timer when the state switch happens
        // 我们也可以重新定义 FirstStageTimer ，因为在第二阶段中未使用它，或者编写“=> ref FirstStageTimer”，但这样我们必须在状态切换发生时重置计时器。
        public ref float SecondStageTimer_SpawnEyes => ref NPC.localAI[3];

        // Do NOT try to use NPC.ai[4]/NPC.localAI[4] or higher indexes, it only accepts 0, 1, 2 and 3!
        // If you choose to go the route of "wrapping properties" for NPC.ai[], make sure they don't overlap (two properties using the same variable in different ways), and that you don't accidently use NPC.ai[] directly

        // 不要尝试使用 NPC.ai[4]/NPC.localAI[4] 或更高索引，它只接受0、1、2和3！
        // 如果你选择走“包装属性”的路线来处理 NPC.ai[] ，请确保它们不会重叠（两个属性以不同方式使用相同的变量），并且不要意外地直接使用 NPC.ai[]

        // Helper method to determine the minion type
        // 辅助方法确定随从类型
        public static int MinionType()
        {
            return ModContent.NPCType<MinionBossMinion>();
        }

        // Helper method to determine the amount of minions summoned
        // 辅助方法确定召唤出的随从数量
        public static int MinionCount()
        {
            int count = 15;

            if (Main.expertMode)
            {
                count += 5; // Increase by 5 if expert or master mode
                            // 如果是专家模式或大师模式，则增加5点。
            }

            if (Main.getGoodWorld)
            {
                count += 5; // Increase by 5 if using the "For The Worthy" seed
                            // 如果使用“For The Worthy”种子，则增加5点。
            }

            return count;
        }

        public override void Load()
        {
            // We want to give it a second boss head icon, so we register one
            // 我们想给他第二个boss头像图标，所以注册一个
            string texture = BossHeadTexture + "_SecondStage"; // Our texture is called "ClassName_Head_Boss_SecondStage"
                                                               // 我们的纹理名为“ClassName_Head_Boss_SecondStage”

            secondStageHeadSlot = Mod.AddBossHeadTexture(texture, -1); // -1 because we already have one registered via the [AutoloadBossHead] attribute, it would overwrite it otherwise
                                                                       // -1是因为我们已经通过[AutoloadBossHead]属性注册了一个，否则它会覆盖它
        }

        public override void BossHeadSlot(ref int index)
        {
            int slot = secondStageHeadSlot;
            if (SecondStage && slot != -1)
            {
                // If the boss is in its second stage, display the other head icon instead
                // 如果Boss处于第二阶段，则显示另一个头像图标
                index = slot;
            }
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 6;

            // Add this in for bosses that have a summon item, requires corresponding code in the item (See MinionBossSummonItem.cs)
            // 对于具有召唤物品的boss，请添加此项，需要与项目中的相应代码配合使用（请参见MinionBossSummonItem.cs）
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            // Automatically group with other bosses
            // 自动分组其他boss
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            // Specify the debuffs it is immune to
            // 指定其免疫的debuffs
            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Poisoned,

                    BuffID.Confused // Most NPCs have this
                                    // 大多数NPC都有这个。
				}
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);

            // Influences how the NPC looks in the Bestiary
            // 影响NPC在Bestiary中的外观
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = "AnalysisMod/Assets/Textures/Bestiary/MinionBoss_Preview",
                PortraitScale = 0.6f, // Portrait refers to the full picture when clicking on the icon in the bestiary
                                      // 肖像指的是在图鉴中点击图标时显示的完整图片

                PortraitPositionYOverride = 0f,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void SetDefaults()
        {
            NPC.width = 110;
            NPC.height = 110;
            NPC.damage = 12;
            NPC.defense = 10;
            NPC.lifeMax = 2000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(gold: 5);
            NPC.SpawnWithHigherTime(30);
            NPC.boss = true;
            NPC.npcSlots = 10f; // Take up open spawn slots, preventing random NPCs from spawning during the fight
                                // 占用开放生成槽，防止战斗期间随机NPC生成

            // Don't set immunities like this as of 1.4:
            // NPC.buffImmune[BuffID.Confused] = true;
            // immunities are handled via dictionaries through NPCID.Sets.DebuffImmunitySets

            // 不要像1.4版本那样设置免疫：
            // NPC.buffImmune[BuffID.Confused] = true;
            // 免疫通过NPCID.Sets.DebuffImmunitySets字典处理

            // Custom AI, 0 is "bound town NPC" AI which slows the NPC down and changes sprite orientation towards the target
            // 自定义AI，0为“绑定城镇NPC” AI，可以减缓NPC速度并将精灵图方向朝向目标
            NPC.aiStyle = -1;

            // Custom boss bar
            // 自定义Boss血条
            NPC.BossBar = ModContent.GetInstance<MinionBossBossBar>();

            // The following code assigns a music track to the boss in a simple way.
            // 以下代码以简单方式为Boss分配音乐曲目。
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Ropocalypse2");
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // Sets the description of this NPC that is listed in the bestiary
            // 设置此NPC在图鉴中列出的描述信息
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Plain black background
                                                                             // 纯黑色背景

				new FlavorTextBestiaryInfoElement("Analysis Minion Boss that spawns minions on spawn, summoned with a spawn item. Showcases boss minion handling, multiplayer conciderations, and custom boss bar.")
            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Do NOT misuse the ModifyNPCLoot and OnKill hooks: the former is only used for registering drops, the latter for everything else
            // 不要滥用ModifyNPCLoot和OnKill钩子：前者仅用于注册掉落物品，后者则用于其他所有内容。

            // Add the treasure bag using ItemDropRule.BossBag (automatically checks for expert mode)
            // 使用ItemDropRule.BossBag添加宝藏袋（自动检查专家模式）
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<MinionBossBag>()));

            // Trophies are spawned with 1/10 chance
            // 奖杯有1/10的几率产生。
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Placeable.Furniture.MinionBossTrophy>(), 10));

            // ItemDropRule.MasterModeCommonDrop for the relic
            // ItemDropRule.MasterModeCommonDrop获取遗物掉落物品，
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeable.Furniture.MinionBossRelic>()));

            // ItemDropRule.MasterModeDropOnAllPlayers for the pet
            // ItemDropRule.MasterModeDropOnAllPlayers获取宠物掉落物品，
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<MinionBossPetItem>(), 4));

            // All our drops here are based on "not expert", meaning we use .OnSuccess() to add them into the rule, which then gets added
            // 我们这里所有的掉落都基于“非专家”，这意味着我们使用.OnSuccess()将其添加到规则中，然后再添加到其中。
            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());

            // Notice we use notExpertRule.OnSuccess instead of npcLoot.Add so it only applies in normal mode
            // 请注意，在普通模式下只使用notExpertRule.OnSuccess而不是npcLoot.Add，以便它仅适用于普通模式

            // Boss masks are spawned with 1/7 chance
            // Boss面具有1/7的几率产生
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<MinionBossMask>(), 7));

            // This part is not required for a boss and is just showcasing some advanced stuff you can do with drop rules to control how items spawn
            // We make 12-15 AnalysisItems spawn randomly in all directions, like the lunar pillar fragments. Hereby we need the DropOneByOne rule,
            // which requires these parameters to be defined

            // 这部分对于Boss来说不是必需的，只是展示一些高级掉落规则可以做到的东西来控制物品生成方式。
            // 我们让12-15个AnalysisItems在所有方向上随机生成，就像月柱碎片一样。因此我们需要DropOneByOne规则，
            // 需要定义这些参数。
            int itemType = ModContent.ItemType<AnalysisItem>();
            var parameters = new DropOneByOne.Parameters()
            {
                ChanceNumerator = 1,
                ChanceDenominator = 1,
                MinimumStackPerChunkBase = 1,
                MaximumStackPerChunkBase = 1,
                MinimumItemDropsCount = 12,
                MaximumItemDropsCount = 15,
            };

            notExpertRule.OnSuccess(new DropOneByOne(itemType, parameters));

            // Finally add the leading rule
            // 最后添加主导规则
            npcLoot.Add(notExpertRule);
        }

        public override void OnKill()
        {
            // This sets downedMinionBoss to true, and if it was false before, it initiates a lantern night
            // 这将downedMinionBoss设置为true，并且如果之前为false，则启动灯笼之夜。
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedMinionBoss, -1);

            // Since this hook is only ran in singleplayer and serverside, we would have to sync it manually.
            // 由于此钩子仅在单人游戏和服务器端运行，因此我们必须手动同步它。

            // Thankfully, vanilla sends the MessageID.WorldData packet if a BOSS was killed automatically, shortly after this hook is ran
            // 幸运的是，在自动杀死BOSS后不久，香草会发送MessageID.WorldData数据包，

            // If your NPC is not a boss and you need to sync the world (which includes ModSystem, check DownedBossSystem), use this code:
            // 如果您的NPC不是boss并且需要同步世界（其中包括ModSystem，请检查DownedBossSystem），请使用以下代码：
            /*
			if (Main.netMode == NetmodeID.Server) {
				NetMessage.SendData(MessageID.WorldData);
			}
			*/
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            // Here you'd want to change the potion type that drops when the boss is defeated. Because this boss is early pre-hardmode, we keep it unchanged
            // (Lesser Healing Potion). If you wanted to change it, simply write "potionType = ItemID.HealingPotion;" or any other potion type

            // 在这里你想改变打败boss时掉落药水类型。因为这个boss处于早期预硬模式阶段，所以我们保持不变
            //(Lesser Healing Potion)。如果你想改变它，只需写“potionType = ItemID.HealingPotion;”或任何其他药水类型即可
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = ImmunityCooldownID.Bosses; // use the boss immunity cooldown counter, to prevent ignoring boss attacks by taking damage from other sources
                                                      // 利用Boss免疫冷却计数器，防止通过从其他来源受到伤害来忽略Boss攻击
            return true;
        }

        public override void FindFrame(int frameHeight)
        {
            // This NPC animates with a simple "go from start frame to final frame, and loop back to start frame" rule
            // In this case: First stage: 0-1-2-0-1-2, Second stage: 3-4-5-3-4-5, 5 being "total frame count - 1"

            // 这个NPC使用一个简单的“从起始帧到最终帧，然后循环回起始帧”的规则进行动画
            // 在这种情况下：第一阶段：0-1-2-0-1-2，第二阶段：3-4-5-3-4-5，其中5为“总帧数 - 1”
            int startFrame = 0;
            int finalFrame = 2;

            if (SecondStage)
            {
                startFrame = 3;
                finalFrame = Main.npcFrameCount[NPC.type] - 1;

                if (NPC.frame.Y < startFrame * frameHeight)
                {
                    // If we were animating the first stage frames and then switch to second stage, immediately change to the start frame of the second stage
                    // 如果我们正在播放第一阶段的框架并切换到第二阶段，则立即更改为第二阶段的开始框架
                    NPC.frame.Y = startFrame * frameHeight;
                }
            }

            int frameSpeed = 5;
            NPC.frameCounter += 0.5f;
            NPC.frameCounter += NPC.velocity.Length() / 10f; // Make the counter go faster with more movement speed
                                                             // 随着移动速度增加，使计数器变得更快
            if (NPC.frameCounter > frameSpeed)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;

                if (NPC.frame.Y > finalFrame * frameHeight)
                {
                    NPC.frame.Y = startFrame * frameHeight;
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            // If the NPC dies, spawn gore and play a sound
            // 如果NPC死亡，生成血腥效果并播放声音
            if (Main.netMode == NetmodeID.Server)
            {
                // We don't want Mod.Find<ModGore> to run on servers as it will crash because gores are not loaded on servers
                // 我们不希望在服务器上运行Mod.Find <ModGore>，因为它会崩溃，因为gores未加载到服务器上
                return;
            }

            if (NPC.life <= 0)
            {
                // These gores work by simply existing as a texture inside any folder which path contains "Gores/"
                // 这些gore的工作方式仅仅是存在于任何路径包含“ Gores /” 的文件夹中的纹理。
                int backGoreType = Mod.Find<ModGore>("MinionBossBody_Back").Type;
                int frontGoreType = Mod.Find<ModGore>("MinionBossBody_Front").Type;

                var entitySource = NPC.GetSource_Death();

                for (int i = 0; i < 2; i++)
                {
                    Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), backGoreType);
                    Gore.NewGore(entitySource, NPC.position, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), frontGoreType);
                }

                SoundEngine.PlaySound(SoundID.Roar, NPC.Center);

                // This adds a screen shake (screenshake) similar to Deerclops
                // 这添加了类似于巨鹿的屏幕抖动（screenshake）
                PunchCameraModifier modifier = new PunchCameraModifier(NPC.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 20f, 6f, 20, 1000f, FullName);
                Main.instance.CameraModifiers.Add(modifier);
            }
        }

        public override void AI()
        {
            // This should almost always be the first code in AI() as it is responsible for finding the proper player target
            // 几乎总是应该是AI（）中第一段代码，因为它负责查找正确的玩家目标
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }

            Player player = Main.player[NPC.target];

            if (player.dead)
            {
                // If the targeted player is dead, flee
                // 如果所选玩家已经死亡，则逃跑
                NPC.velocity.Y -= 0.04f;
                // This method makes it so when the boss is in "despawn range" (outside of the screen), it despawns in 10 ticks
                // 此方法使得当boss处于“消失范围”（屏幕外）时，在10个ticks内消失。
                NPC.EncourageDespawn(10);
                return;
            }

            SpawnMinions();

            CheckSecondStage();

            // Be invulnerable during the first stage
            // 在第一阶段无敌
            NPC.dontTakeDamage = !SecondStage;

            if (SecondStage)
            {
                DoSecondStage(player);
            }
            else
            {
                DoFirstStage(player);
            }
        }

        private void SpawnMinions()
        {
            if (SpawnedMinions)
            {
                // No point executing the code in this method again
                // 没有必要再次执行此方法中的代码
                return;
            }

            SpawnedMinions = true;

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                // Because we want to spawn minions, and minions are NPCs, we have to do this on the server (or singleplayer, "!= NetmodeID.MultiplayerClient" covers both)
                // This means we also have to sync it after we spawned and set up the minion

                // 因为我们想要生成小兵，并且小兵是NPCs，所以我们必须在服务器端进行操作（或单人游戏，“！= NetmodeID.MultiplayerClient”覆盖两者）
                //这意味着我们还需要在生成和设置小兵之后同步它。
                return;
            }

            int count = MinionCount();
            var entitySource = NPC.GetSource_FromAI();

            MinionMaxHealthTotal = 0;
            for (int i = 0; i < count; i++)
            {
                NPC minionNPC = NPC.NewNPCDirect(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<MinionBossMinion>(), NPC.whoAmI);
                if (minionNPC.whoAmI == Main.maxNPCs)
                    continue; // spawn failed due to spawn cap
                              // 由于出生限制而导致生成失败

                // Now that the minion is spawned, we need to prepare it with data that is necessary for it to work
                // This is not required usually if you simply spawn NPCs, but because the minion is tied to the body, we need to pass this information to it

                // 现在小兵被召唤出来了，我们需要准备数据以便其正常工作
                //如果您只是简单地产生NPC，则通常不需要此操作，但是因为小兵与身体绑定，所以我们需要将此信息传递给它。
                MinionBossMinion minion = (MinionBossMinion)minionNPC.ModNPC;
                minion.ParentIndex = NPC.whoAmI; // Let the minion know who the "parent" is
                                                 // 让小兵知道“父母”是谁

                minion.PositionOffset = i / (float)count; // Give it a separate position offset
                                                          // 给它一个单独的位置偏移量

                MinionMaxHealthTotal += minionNPC.lifeMax; // add the total minion life for boss bar shield texxt
                                                           // 添加总小兵生命值以用于boss bar shield texxt

                // Finally, syncing, only sync on server and if the NPC actually exists (Main.maxNPCs is the index of a dummy NPC, there is no point syncing it)
                // 最后，同步，在服务器上和NPC实际存在时才进行同步（Main.maxNPCs是虚拟NPC的索引，没有同步点）
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.SyncNPC, number: minionNPC.whoAmI);
                }
            }

            // sync MinionMaxHealthTotal
            // 同步MinionMaxHealthTotal
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
            }
        }

        private void CheckSecondStage()
        {
            if (SecondStage)
            {
                // No point checking if the NPC is already in its second stage
                // 没有必要检查NPC是否已经处于第二阶段
                return;
            }

            MinionHealthTotal = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC otherNPC = Main.npc[i];
                if (otherNPC.active && otherNPC.type == MinionType() && otherNPC.ModNPC is MinionBossMinion minion)
                {
                    if (minion.ParentIndex == NPC.whoAmI)
                    {
                        MinionHealthTotal += otherNPC.life;
                    }
                }
            }

            if (MinionHealthTotal <= 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                // If we have no shields (aka "no minions alive"), we initiate the second stage, and notify other players that this NPC has reached its second stage
                // by setting NPC.netUpdate to true in this tick. It will send important data like position, velocity and the NPC.ai[] array to all connected clients

                // 如果我们没有护盾（也就是“没有活着的小兵”），则启动第二阶段，并在这个tick中将 NPC.netUpdate设置为true 通知其他玩家该NPC已达到其第二阶段
                //通过设置 NPC.ai []数组来发送重要数据如位置、速度和。连接到所有客户端。

                // Because SecondStage is a property using NPC.ai[], it will get synced this way
                // 因为SecondStage 是使用 NPC.ai[]属性, 所以会被同步.
                SecondStage = true;
                NPC.netUpdate = true;
            }
        }

        private void DoFirstStage(Player player)
        {
            // Each time the timer is 0, pick a random position a fixed distance away from the player but towards the opposite side
            // The NPC moves directly towards it with fixed speed, while displaying its trajectory as a telegraph

            // 每次计时器归零时, 都从固定距离内随机选择一个位置并朝相反方向移动。
            // NPC直接向其移动，并显示其轨迹作为预判线

            FirstStageTimer++;
            if (FirstStageTimer > FirstStageTimerMax)
            {
                FirstStageTimer = 0;
            }

            float distance = 200; // Distance in pixels behind the player
                                  // 玩家后面的像素距离

            if (FirstStageTimer == 0)
            {
                Vector2 fromPlayer = NPC.Center - player.Center;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    // Important multiplayer concideration: drastic change in behavior (that is also decided by randomness) like this requires
                    // to be executed on the server (or singleplayer) to keep the boss in sync

                    // 重要的多人游戏考虑：这种行为上的 drastical 变化（也由随机性决定）
                    // 需要在服务器（或单人游戏）上执行，以保持 boss 的同步。

                    float angle = fromPlayer.ToRotation();
                    float twelfth = MathHelper.Pi / 6;

                    angle += MathHelper.Pi + Main.rand.NextFloat(-twelfth, twelfth);
                    if (angle > MathHelper.TwoPi)
                    {
                        angle -= MathHelper.TwoPi;
                    }
                    else if (angle < 0)
                    {
                        angle += MathHelper.TwoPi;
                    }

                    Vector2 relativeDestination = angle.ToRotationVector2() * distance;

                    FirstStageDestination = player.Center + relativeDestination;
                    NPC.netUpdate = true;
                }
            }

            // Move along the vector
            // 沿着向量移动
            Vector2 toDestination = FirstStageDestination - NPC.Center;
            Vector2 toDestinationNormalized = toDestination.SafeNormalize(Vector2.UnitY);
            float speed = Math.Min(distance, toDestination.Length());
            NPC.velocity = toDestinationNormalized * speed / 30;

            if (FirstStageDestination != LastFirstStageDestination)
            {
                // If destination changed
                // 如果目标改变了
                NPC.TargetClosest(); // Pick the closest player target again
                                     // 再次选择最近的玩家目标

                // "Why is this not in the same code that sets FirstStageDestination?" Because in multiplayer it's ran by the server.
                // The client has to know when the destination changes a different way. Keeping track of the previous ticks' destination is one way

                // “为什么不与设置 FirstStageDestination 的代码相同？”因为在多人游戏中，它是由服务器运行的。
                // 客户端必须知道当目标以不同方式更改时。跟踪前一个刻度的目标是一种方法。
                if (Main.netMode != NetmodeID.Server)
                {
                    // For visuals regarding NPC position, netOffset has to be concidered to make visuals align properly
                    // 对于有关 NPC 位置的视觉效果，必须考虑 netOffset 以使视觉效果正确对齐。
                    NPC.position += NPC.netOffset;

                    // Draw a line between the NPC and its destination, represented as dusts every 20 pixels
                    Dust.QuickDustLine(NPC.Center + toDestinationNormalized * NPC.width, FirstStageDestination, toDestination.Length() / 20f, Color.Yellow);

                    NPC.position -= NPC.netOffset;
                }
            }
            LastFirstStageDestination = FirstStageDestination;

            // No damage during first phase
            // 在第一阶段没有伤害
            NPC.damage = 0;

            // Fade in based on remaining total minion life
            // 基于剩余总小兵生命淡入淡出
            float remainingShields = MinionHealthTotal / (float)MinionMaxHealthTotal;
            NPC.alpha = (int)(remainingShields * 255);

            NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;
        }

        private void DoSecondStage(Player player)
        {
            Vector2 toPlayer = player.Center - NPC.Center;

            float offsetX = 200f;

            Vector2 abovePlayer = player.Top + new Vector2(NPC.direction * offsetX, -NPC.height);

            Vector2 toAbovePlayer = abovePlayer - NPC.Center;
            Vector2 toAbovePlayerNormalized = toAbovePlayer.SafeNormalize(Vector2.UnitY);

            // The NPC tries to go towards the offsetX position, but most likely it will never get there exactly, or close to if the player is moving
            // This checks if the npc is "70% there", and then changes direction

            // NPC 尝试朝 offsetX 位置走去，但很可能永远无法到达那里，如果玩家正在移动，则接近。
            // 这检查 npc 是否“70% 到达”，然后改变方向。
            float changeDirOffset = offsetX * 0.7f;

            if (NPC.direction == -1 && NPC.Center.X - changeDirOffset < abovePlayer.X ||
                NPC.direction == 1 && NPC.Center.X + changeDirOffset > abovePlayer.X)
            {
                NPC.direction *= -1;
            }

            float speed = 8f;
            float inertia = 40f;

            // If the boss is somehow below the player, move faster to catch up
            // 如果老板某些情况下低于玩家，请加快速度以追赶他们。
            if (NPC.Top.Y > player.Bottom.Y)
            {
                speed = 12f;
            }

            Vector2 moveTo = toAbovePlayerNormalized * speed;
            NPC.velocity = (NPC.velocity * (inertia - 1) + moveTo) / inertia;

            DoSecondStage_SpawnEyes(player);

            NPC.damage = NPC.defDamage;

            NPC.alpha = 0;

            NPC.rotation = toPlayer.ToRotation() - MathHelper.PiOver2;
        }

        private void DoSecondStage_SpawnEyes(Player player)
        {
            // At 100% health, spawn every 90 ticks
            // Drops down until 33% health to spawn every 30 ticks

            // 在100％健康状况下每90个tick生成一次
            // 下降到33％健康状况以每30个tick生成一次
            float timerMax = Utils.Clamp((float)NPC.life / NPC.lifeMax, 0.33f, 1f) * 90;

            SecondStageTimer_SpawnEyes++;
            if (SecondStageTimer_SpawnEyes > timerMax)
            {
                SecondStageTimer_SpawnEyes = 0;
            }

            if (NPC.HasValidTarget && SecondStageTimer_SpawnEyes == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                // Spawn projectile randomly below player, based on horizontal velocity to make kiting harder, starting velocity 1f upwards
                // (The projectiles accelerate from their initial velocity)

                // 根据水平速度在玩家下方随机生成弹丸，使躲避更加困难，起始速度为1f向上
                // （这些弹丸从其初始速度加速）

                float kitingOffsetX = Utils.Clamp(player.velocity.X * 16, -100, 100);
                Vector2 position = player.Bottom + new Vector2(kitingOffsetX + Main.rand.Next(-100, 100), Main.rand.Next(50, 100));

                int type = ModContent.ProjectileType<AnalysisMinionBossEye>();
                int damage = NPC.damage / 2;
                var entitySource = NPC.GetSource_FromAI();

                Projectile.NewProjectile(entitySource, position, -Vector2.UnitY, type, damage, 0f, Main.myPlayer);
            }
        }
    }
}
