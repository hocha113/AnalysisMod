using AnalysisMod.AnalysisContent.Dusts;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AnalysisMod.AnalysisContent.Items.Weapons;
using AnalysisMod.AnalysisContent.Items;

namespace AnalysisMod.AnalysisContent.NPCs
{
    /// <summary>
    /// 这个NPC的主要重点是展示如何制作类似于香草骨头商人的东西；
    /// 这意味着该NPC将像其他城镇NPC一样行动，但不会有幸福按钮，不会出现在小地图上，
    /// 并且会像敌对NPC一样生成。如果您想要传统的城镇NPC，请参见<see cref="AnalysisPerson"/>。
    /// </summary>
    public class AnalysisBoneMerchant : ModNPC
    {
        private static Profiles.StackedNPCProfile NPCProfile;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 25; // The amount of frames the NPC has
                                           // 该NPC拥有的帧数

            NPCID.Sets.ExtraFramesCount[Type] = 9; // Generally for Town NPCs, but this is how the NPC does extra things such as sitting in a chair and talking to other NPCs.
                                                   // 通常用于城镇NPC，但这是指该NPC如何做额外的事情，例如坐在椅子上和与其他NPC交谈。

            NPCID.Sets.AttackFrameCount[Type] = 4;
            NPCID.Sets.DangerDetectRange[Type] = 700; // The amount of pixels away from the center of the npc that it tries to attack enemies.
                                                      // 尝试攻击敌人时距离npc中心多少像素。

            NPCID.Sets.PrettySafe[Type] = 300;
            NPCID.Sets.AttackType[Type] = 1; // Shoots a weapon.
                                             // 射击武器。

            NPCID.Sets.AttackTime[Type] = 60; // The amount of time it takes for the NPC's attack animation to be over once it starts.
                                              // 当开始攻击动画后需要多长时间才能结束。

            NPCID.Sets.AttackAverageChance[Type] = 30;
            NPCID.Sets.HatOffsetY[Type] = 4; // For when a party is active, the party hat spawns at a Y offset.
                                             // 当一个派对活跃时，在Y轴偏移处生成聚会帽子。

            NPCID.Sets.ShimmerTownTransform[NPC.type] = true; // This set says that the Town NPC has a Shimmered form. Otherwise, the Town NPC will become transparent when touching Shimmer like other enemies.
                                                              // 此设置表示城镇 NPC 具有闪烁形式。否则，城镇 NPC 将变为透明状态，并像其他敌人一样接触到 Shimmer 时消失。

            // 这个设置是此 NPC 最重要的部分。由于它为 true，则告诉游戏我们希望此 NPC 像一个城镇 NPC 一样行动而实际上并非如此。
            // 这意味着：该 NPC 将具有城镇 NPC 的 AI、攻击方式以及商店（或任何其他附加功能，如果您愿意）
            // 但是，该 NPC 不会在地图上显示其头像，在没有玩家附近或世界关闭时将消失，并像其他任何 NPC 一样生成。
            NPCID.Sets.ActsLikeTownNPC[Type] = true;

            // 这个设置防止了幸福按钮
            NPCID.Sets.NoTownNPCHappiness[Type] = true;

            // 再次强调，由于此 NPC 并不是实际上的城镇 NPC，我们需要告诉游戏即使它们生成时仍然希望此 NPC 具有自定义/随机名称。
            // 为了做到这一点，我们只需使此钩子返回 true 即可，这将使游戏在生成NPC时调用 TownNPCName 方法以确定NPC的名称。
            NPCID.Sets.SpawnsWithCustomName[Type] = true;

            // 香草骨头商人不能与门互动（特别是打开或关闭它们），但如果您希望您的NPC能够尽管如此与它们互动，
            // 就取消下面这一行的注释。

            //NPCID.Sets.AllowDoorInteraction[Type] = true;

            // 影响NPC在怪物图鉴中的外观
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Velocity = 1f, // 绘制npc，就好像他向x方向走了+1格

                Direction = 1 // 表示左边而1表示右边。默认情况下NPC面向左侧绘制，但AnalysisPerson将面向右侧绘制
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

            NPCProfile = new Profiles.StackedNPCProfile(
                new Profiles.DefaultNPCProfile(Texture, -1),
                new Profiles.DefaultNPCProfile(Texture + "_Shimmer", -1)
            );
        }

        public override void SetDefaults()
        {
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

        // 确保允许你的 NPC 聊天, 因为“像一个城镇 npc”并不自动允许聊天.
        public override bool CanChat()
        {
            return true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // 我们可以使用 AddRange 而不是多次调用 Add 来一次添加多个项
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {

                // 设置此城镇 NPC 在怪物图鉴中列出的首选生物群系。
                // 对于城镇NPC，通常将其设置为与NPC幸福度相关的最喜欢的生物群系。
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Underground,

                // 在怪物图鉴中设置您的 NPC 的独特介绍文本。
				new FlavorTextBestiaryInfoElement("Hailing from a mysterious greyscale cube world, the Analysis Bone Merchant will show you how to make a mysterious merchant underground with tModLoader."),

                // 如果你真的想要，你可以添加多个元素
                // 您还可以使用本地化键（请参见 Localization/en-US.lang）
				new FlavorTextBestiaryInfoElement("Mods.AnalysisMod.Bestiary.AnalysisBoneMerchant")
            });
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            // 当 NPC 受到伤害时产生尘埃。
            int num = NPC.life > 0 ? 1 : 5;

            for (int k = 0; k < num; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<Sparkle>());
            }

            // 当 NPC 被杀死时创建血腥效果。
            if (Main.netMode != NetmodeID.Server && NPC.life <= 0)
            {
                // 检索血腥类型。该NPC仅具有闪烁变体。（6种总共）
                string variant = "";
                if (NPC.IsShimmerVariant) variant += "_Shimmer";
                int headGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Head").Type;
                int armGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Arm").Type;
                int legGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Leg").Type;

                // 生成尸骨。手臂和腿部的位置下降，看起来更自然。
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, headGore, 1f);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 20), NPC.velocity, armGore);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 20), NPC.velocity, armGore);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 34), NPC.velocity, legGore);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 34), NPC.velocity, legGore);
            }
        }

        public override ITownNPCProfile TownNPCProfile()
        {
            return NPCProfile;
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string> {
                "Blocky Bones",
                "Someone's Ribcage",
                "Underground Blockster",
                "Darkness"
            };
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            // 如果任何玩家在地下并且背包里有分析物品，则分析骨头商人将有一定几率生成。
            if (spawnInfo.Player.ZoneDirtLayerHeight && spawnInfo.Player.inventory.Any(item => item.type == ModContent.ItemType<AnalysisItem>()))
            {
                return 0.34f;
            }

            // 否则，如果不满足上述条件，则不会生成分析骨头商人。
            return 0f;
        }

        public override string GetChat()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();

            // 这些是NPC与你交谈时可能告诉你的事情。
            chat.Add(Language.GetTextValue("Mods.AnalysisMod.Dialogue.AnalysisBoneMerchant.StandardDialogue1"));
            chat.Add(Language.GetTextValue("Mods.AnalysisMod.Dialogue.AnalysisBoneMerchant.StandardDialogue2"));
            chat.Add(Language.GetTextValue("Mods.AnalysisMod.Dialogue.AnalysisBoneMerchant.StandardDialogue3"));
            return chat; // 聊天被隐式转换为字符串。
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
          // 打开聊天界面后，你会看到哪些聊天按钮
            button = Language.GetTextValue("LegacyInterface.28"); //这是“商店”一词的关键
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shop)
        {
            if (firstButton)
            {
                shop = "Shop";
            }
        }

        public override void AddShops()
        {
            new NPCShop(Type)
                .Add<AnalysisItem>()
                .Register();
        }

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 20;
            knockback = 2f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 10;
            randExtraCooldown = 1;
        }

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ProjectileID.NanoBullet;
            attackDelay = 1;

            // 此代码逐渐延迟后续射击时间。
            if (NPC.localAI[3] > attackDelay)
            {
                attackDelay = 12;
            }
            if (NPC.localAI[3] > attackDelay)
            {
                attackDelay = 24;
            }
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 10f;
            randomOffset = 0.2f;
        }

        public override void TownNPCAttackShoot(ref bool inBetweenShots)
        {
            if (NPC.localAI[3] > 1)
            {
                inBetweenShots = true;
            }
        }

        public override void DrawTownAttackGun(ref Texture2D item, ref Rectangle itemFrame, ref float scale, ref int horizontalHoldoutOffset)
        {
            if (!NPC.IsShimmerVariant)
            {
                // 如果使用现有物品，请使用此方法
                int itemType = ModContent.ItemType<AnalysisCustomAmmoGun>();
                Main.GetItemDrawFrame(itemType, out item, out itemFrame);
                horizontalHoldoutOffset = (int)Main.DrawPlayerItemPos(1f, itemType).X - 12;
            }
            else
            {
                // 这个纹理实际上不是一个现有物品，但仍然可以使用。
                item = ModContent.Request<Texture2D>(Texture + "_Shimmer_Gun").Value;
                itemFrame = item.Frame();
                horizontalHoldoutOffset = -2;
            }
        }
    }
}
