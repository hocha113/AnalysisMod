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
    /// The main focus of this NPC is to show how to make something similar to the vanilla bone merchant;
    /// which means that the NPC will act like any other town NPC but won't have a happiness button, won't appear on the minimap,
    /// and will spawn like an enemy NPC. If you want a traditional town NPC instead, see <see cref="AnalysisPerson"/>.<br/>
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

            //This sets entry is the most important part of this NPC. Since it is true, it tells the game that we want this NPC to act like a town NPC without ACTUALLY being one.
            //What that means is: the NPC will have the AI of a town NPC, will attack like a town NPC, and have a shop (or any other additional functionality if you wish) like a town NPC.
            //However, the NPC will not have their head displayed on the map, will de-spawn when no players are nearby or the world is closed, and will spawn like any other NPC.

            // 这个设置是此 NPC 最重要的部分。由于它为 true，则告诉游戏我们希望此 NPC 像一个城镇 NPC 一样行动而实际上并非如此。
            // 这意味着：该 NPC 将具有城镇 NPC 的 AI、攻击方式以及商店（或任何其他附加功能，如果您愿意）。
            // 但是，该 NPC 不会在地图上显示其头像，在没有玩家附近或世界关闭时将消失，并像其他任何 NPC 一样生成。
            NPCID.Sets.ActsLikeTownNPC[Type] = true;

            // This prevents the happiness button
            // 这个设置防止了幸福按钮
            NPCID.Sets.NoTownNPCHappiness[Type] = true;

            //To reiterate, since this NPC isn't technically a town NPC, we need to tell the game that we still want this NPC to have a custom/randomized name when they spawn.
            //In order to do this, we simply make this hook return true, which will make the game call the TownNPCName method when spawning the NPC to determine the NPC's name.

            // 再次强调，由于此 NPC 并不是技术上的城镇 NPC，我们需要告诉游戏即使它们生成时仍然希望此 NPC 具有自定义/随机名称。
            // 为了做到这一点，我们只需使此钩子返回 true 即可，这将使游戏在生成NPC时调用 TownNPCName 方法以确定NPC的名称。
            NPCID.Sets.SpawnsWithCustomName[Type] = true;

            //The vanilla Bone Merchant cannot interact with doors (open or close them, specifically), but if you want your NPC to be able to interact with them despite this,
            //uncomment this line below.

            // 香草骨头商人不能与门互动（特别是打开或关闭它们），但如果您希望您的NPC能够尽管如此与它们互动，
            // 就取消下面这一行的注释。

            //NPCID.Sets.AllowDoorInteraction[Type] = true;

            // Influences how the NPC looks in the Bestiary
            // 影响NPC在怪物图鉴中的外观
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Velocity = 1f, // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
                               // 在最佳iary中绘制npc就好像他向x方向走了+1格

                Direction = 1 // -1 is left and 1 is right. NPCs are drawn facing the left by default but AnalysisPerson will be drawn facing the right-1
                              // 表示左边而1表示右边。默认情况下NPC面向左侧绘制，但AnalysisPerson将面向右侧绘制
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

        //Make sure to allow your NPC to chat, since being "like a town NPC" doesn't automatically allow for chatting.
        // 确保允许你的 NPC 聊天, 因为“像一个城镇 npc”并不自动允许聊天.
        public override bool CanChat()
        {
            return true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            // 我们可以使用 AddRange 而不是多次调用 Add 来一次添加多个项
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.

                // 设置此城镇 NPC 在怪物图鉴中列出的首选生物群系。
                // 对于城镇NPC，通常将其设置为与NPC幸福度相关的最喜欢的生物群系。
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Underground,

				// Sets your NPC's flavor text in the bestiary.
                // 在怪物图鉴中设置您的 NPC 的口味文本。
				new FlavorTextBestiaryInfoElement("Hailing from a mysterious greyscale cube world, the Analysis Bone Merchant will show you how to make a mysterious merchant underground with tModLoader."),

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)

                // 如果你真的想要，你可以添加多个元素
                // 您还可以使用本地化键（请参见 Localization/en-US.lang）
				new FlavorTextBestiaryInfoElement("Mods.AnalysisMod.Bestiary.AnalysisBoneMerchant")
            });
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            // Causes dust to spawn when the NPC takes damage.
            // 当 NPC 受到伤害时产生尘埃。
            int num = NPC.life > 0 ? 1 : 5;

            for (int k = 0; k < num; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<Sparkle>());
            }

            // Create gore when the NPC is killed.
            // 当 NPC 被杀死时创建血腥效果。
            if (Main.netMode != NetmodeID.Server && NPC.life <= 0)
            {
                // Retrieve the gore types. This NPC only has shimmer variants. (6 total gores)
                // 检索血腥类型。该NPC仅具有闪烁变体。（6种总共）
                string variant = "";
                if (NPC.IsShimmerVariant) variant += "_Shimmer";
                int headGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Head").Type;
                int armGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Arm").Type;
                int legGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Leg").Type;

                // Spawn the gores. The positions of the arms and legs are lowered for a more natural look.
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
            //If any player is underground and has an Analysis item in their inventory, the Analysis bone merchant will have a slight chance to spawn.
            // 如果任何玩家在地下并且背包里有分析物品，则分析骨头商人将有一定几率生成。
            if (spawnInfo.Player.ZoneDirtLayerHeight && spawnInfo.Player.inventory.Any(item => item.type == ModContent.ItemType<AnalysisItem>()))
            {
                return 0.34f;
            }

            //Else, the Analysis bone merchant will not spawn if the above conditions are not met.
            // 否则，如果不满足上述条件，则不会生成分析骨头商人。
            return 0f;
        }

        public override string GetChat()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();

            // These are things that the NPC has a chance of telling you when you talk to it.
            // 这些是NPC与你交谈时可能告诉你的事情。
            chat.Add(Language.GetTextValue("Mods.AnalysisMod.Dialogue.AnalysisBoneMerchant.StandardDialogue1"));
            chat.Add(Language.GetTextValue("Mods.AnalysisMod.Dialogue.AnalysisBoneMerchant.StandardDialogue2"));
            chat.Add(Language.GetTextValue("Mods.AnalysisMod.Dialogue.AnalysisBoneMerchant.StandardDialogue3"));
            return chat; // chat is implicitly cast to a string.
                         // 聊天被隐式转换为字符串。
        }

        public override void SetChatButtons(ref string button, ref string button2)
        { // What the chat buttons are when you open up the chat UI
          // 打开聊天界面后，你会看到哪些聊天按钮
            button = Language.GetTextValue("LegacyInterface.28"); //This is the key to the word "Shop"
                                                                  //这是“商店”一词的关键。
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

            // This code progressively delays subsequent shots.
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
                // If using an existing item, use this approach
                // 如果使用现有物品，请使用此方法
                int itemType = ModContent.ItemType<AnalysisCustomAmmoGun>();
                Main.GetItemDrawFrame(itemType, out item, out itemFrame);
                horizontalHoldoutOffset = (int)Main.DrawPlayerItemPos(1f, itemType).X - 12;
            }
            else
            {
                // This texture isn't actually an existing item, but can still be used.
                // 这个纹理实际上不是一个现有物品，但仍然可以使用。
                item = ModContent.Request<Texture2D>(Texture + "_Shimmer_Gun").Value;
                itemFrame = item.Frame();
                horizontalHoldoutOffset = -2;
            }
        }
    }
}
