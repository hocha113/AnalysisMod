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
    /// ���NPC����Ҫ�ص���չʾ���������������ݹ�ͷ���˵Ķ�����
    /// ����ζ�Ÿ�NPC������������NPCһ���ж������������Ҹ���ť�����������С��ͼ�ϣ�
    /// ���һ���ж�NPCһ�����ɡ��������Ҫ��ͳ�ĳ���NPC����μ�<see cref="AnalysisPerson"/>��
    /// </summary>
    public class AnalysisBoneMerchant : ModNPC
    {
        private static Profiles.StackedNPCProfile NPCProfile;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 25; // The amount of frames the NPC has
                                           // ��NPCӵ�е�֡��

            NPCID.Sets.ExtraFramesCount[Type] = 9; // Generally for Town NPCs, but this is how the NPC does extra things such as sitting in a chair and talking to other NPCs.
                                                   // ͨ�����ڳ���NPC��������ָ��NPC�������������飬�������������Ϻ�������NPC��̸��

            NPCID.Sets.AttackFrameCount[Type] = 4;
            NPCID.Sets.DangerDetectRange[Type] = 700; // The amount of pixels away from the center of the npc that it tries to attack enemies.
                                                      // ���Թ�������ʱ����npc���Ķ������ء�

            NPCID.Sets.PrettySafe[Type] = 300;
            NPCID.Sets.AttackType[Type] = 1; // Shoots a weapon.
                                             // ���������

            NPCID.Sets.AttackTime[Type] = 60; // The amount of time it takes for the NPC's attack animation to be over once it starts.
                                              // ����ʼ������������Ҫ�೤ʱ����ܽ�����

            NPCID.Sets.AttackAverageChance[Type] = 30;
            NPCID.Sets.HatOffsetY[Type] = 4; // For when a party is active, the party hat spawns at a Y offset.
                                             // ��һ���ɶԻ�Ծʱ����Y��ƫ�ƴ����ɾۻ�ñ�ӡ�

            NPCID.Sets.ShimmerTownTransform[NPC.type] = true; // This set says that the Town NPC has a Shimmered form. Otherwise, the Town NPC will become transparent when touching Shimmer like other enemies.
                                                              // �����ñ�ʾ���� NPC ������˸��ʽ�����򣬳��� NPC ����Ϊ͸��״̬��������������һ���Ӵ��� Shimmer ʱ��ʧ��

            //This sets entry is the most important part of this NPC. Since it is true, it tells the game that we want this NPC to act like a town NPC without ACTUALLY being one.
            //What that means is: the NPC will have the AI of a town NPC, will attack like a town NPC, and have a shop (or any other additional functionality if you wish) like a town NPC.
            //However, the NPC will not have their head displayed on the map, will de-spawn when no players are nearby or the world is closed, and will spawn like any other NPC.

            // ��������Ǵ� NPC ����Ҫ�Ĳ��֡�������Ϊ true���������Ϸ����ϣ���� NPC ��һ������ NPC һ���ж���ʵ���ϲ�����ˡ�
            // ����ζ�ţ��� NPC �����г��� NPC �� AI��������ʽ�Լ��̵꣨���κ��������ӹ��ܣ������Ը�⣩��
            // ���ǣ��� NPC �����ڵ�ͼ����ʾ��ͷ����û����Ҹ���������ر�ʱ����ʧ�����������κ� NPC һ�����ɡ�
            NPCID.Sets.ActsLikeTownNPC[Type] = true;

            // This prevents the happiness button
            // ������÷�ֹ���Ҹ���ť
            NPCID.Sets.NoTownNPCHappiness[Type] = true;

            //To reiterate, since this NPC isn't technically a town NPC, we need to tell the game that we still want this NPC to have a custom/randomized name when they spawn.
            //In order to do this, we simply make this hook return true, which will make the game call the TownNPCName method when spawning the NPC to determine the NPC's name.

            // �ٴ�ǿ�������ڴ� NPC �����Ǽ����ϵĳ��� NPC��������Ҫ������Ϸ��ʹ��������ʱ��Ȼϣ���� NPC �����Զ���/������ơ�
            // Ϊ��������һ�㣬����ֻ��ʹ�˹��ӷ��� true ���ɣ��⽫ʹ��Ϸ������NPCʱ���� TownNPCName ������ȷ��NPC�����ơ�
            NPCID.Sets.SpawnsWithCustomName[Type] = true;

            //The vanilla Bone Merchant cannot interact with doors (open or close them, specifically), but if you want your NPC to be able to interact with them despite this,
            //uncomment this line below.

            // ��ݹ�ͷ���˲������Ż������ر��Ǵ򿪻�ر����ǣ����������ϣ������NPC�ܹ�������������ǻ�����
            // ��ȡ��������һ�е�ע�͡�

            //NPCID.Sets.AllowDoorInteraction[Type] = true;

            // Influences how the NPC looks in the Bestiary
            // Ӱ��NPC�ڹ���ͼ���е����
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Velocity = 1f, // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
                               // �����iary�л���npc�ͺ�������x��������+1��

                Direction = 1 // -1 is left and 1 is right. NPCs are drawn facing the left by default but AnalysisPerson will be drawn facing the right-1
                              // ��ʾ��߶�1��ʾ�ұߡ�Ĭ�������NPC���������ƣ���AnalysisPerson�������Ҳ����
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

        //Make sure to allow your NPC to chat, since being "like a town NPC" doesn't automatically allow for chatting.
        // ȷ��������� NPC ����, ��Ϊ����һ������ npc�������Զ���������.
        public override bool CanChat()
        {
            return true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            // ���ǿ���ʹ�� AddRange �����Ƕ�ε��� Add ��һ����Ӷ����
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.

                // ���ô˳��� NPC �ڹ���ͼ�����г�����ѡ����Ⱥϵ��
                // ���ڳ���NPC��ͨ����������Ϊ��NPC�Ҹ�����ص���ϲ��������Ⱥϵ��
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Underground,

				// Sets your NPC's flavor text in the bestiary.
                // �ڹ���ͼ������������ NPC �Ŀ�ζ�ı���
				new FlavorTextBestiaryInfoElement("Hailing from a mysterious greyscale cube world, the Analysis Bone Merchant will show you how to make a mysterious merchant underground with tModLoader."),

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)

                // ����������Ҫ���������Ӷ��Ԫ��
                // ��������ʹ�ñ��ػ�������μ� Localization/en-US.lang��
				new FlavorTextBestiaryInfoElement("Mods.AnalysisMod.Bestiary.AnalysisBoneMerchant")
            });
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            // Causes dust to spawn when the NPC takes damage.
            // �� NPC �ܵ��˺�ʱ����������
            int num = NPC.life > 0 ? 1 : 5;

            for (int k = 0; k < num; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<Sparkle>());
            }

            // Create gore when the NPC is killed.
            // �� NPC ��ɱ��ʱ����Ѫ��Ч����
            if (Main.netMode != NetmodeID.Server && NPC.life <= 0)
            {
                // Retrieve the gore types. This NPC only has shimmer variants. (6 total gores)
                // ����Ѫ�����͡���NPC��������˸���塣��6���ܹ���
                string variant = "";
                if (NPC.IsShimmerVariant) variant += "_Shimmer";
                int headGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Head").Type;
                int armGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Arm").Type;
                int legGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Leg").Type;

                // Spawn the gores. The positions of the arms and legs are lowered for a more natural look.
                // ����ʬ�ǡ��ֱۺ��Ȳ���λ���½�������������Ȼ��
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
            // ����κ�����ڵ��²��ұ������з�����Ʒ���������ͷ���˽���һ���������ɡ�
            if (spawnInfo.Player.ZoneDirtLayerHeight && spawnInfo.Player.inventory.Any(item => item.type == ModContent.ItemType<AnalysisItem>()))
            {
                return 0.34f;
            }

            //Else, the Analysis bone merchant will not spawn if the above conditions are not met.
            // ������������������������򲻻����ɷ�����ͷ���ˡ�
            return 0f;
        }

        public override string GetChat()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();

            // These are things that the NPC has a chance of telling you when you talk to it.
            // ��Щ��NPC���㽻̸ʱ���ܸ���������顣
            chat.Add(Language.GetTextValue("Mods.AnalysisMod.Dialogue.AnalysisBoneMerchant.StandardDialogue1"));
            chat.Add(Language.GetTextValue("Mods.AnalysisMod.Dialogue.AnalysisBoneMerchant.StandardDialogue2"));
            chat.Add(Language.GetTextValue("Mods.AnalysisMod.Dialogue.AnalysisBoneMerchant.StandardDialogue3"));
            return chat; // chat is implicitly cast to a string.
                         // ���챻��ʽת��Ϊ�ַ�����
        }

        public override void SetChatButtons(ref string button, ref string button2)
        { // What the chat buttons are when you open up the chat UI
          // ������������ῴ����Щ���찴ť
            button = Language.GetTextValue("LegacyInterface.28"); //This is the key to the word "Shop"
                                                                  //���ǡ��̵ꡱһ�ʵĹؼ���
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
            // �˴������ӳٺ������ʱ�䡣
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
                // ���ʹ��������Ʒ����ʹ�ô˷���
                int itemType = ModContent.ItemType<AnalysisCustomAmmoGun>();
                Main.GetItemDrawFrame(itemType, out item, out itemFrame);
                horizontalHoldoutOffset = (int)Main.DrawPlayerItemPos(1f, itemType).X - 12;
            }
            else
            {
                // This texture isn't actually an existing item, but can still be used.
                // �������ʵ���ϲ���һ��������Ʒ������Ȼ����ʹ�á�
                item = ModContent.Request<Texture2D>(Texture + "_Shimmer_Gun").Value;
                itemFrame = item.Frame();
                horizontalHoldoutOffset = -2;
            }
        }
    }
}
