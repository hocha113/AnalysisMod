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

            // ��������Ǵ� NPC ����Ҫ�Ĳ��֡�������Ϊ true���������Ϸ����ϣ���� NPC ��һ������ NPC һ���ж���ʵ���ϲ�����ˡ�
            // ����ζ�ţ��� NPC �����г��� NPC �� AI��������ʽ�Լ��̵꣨���κ��������ӹ��ܣ������Ը�⣩
            // ���ǣ��� NPC �����ڵ�ͼ����ʾ��ͷ����û����Ҹ���������ر�ʱ����ʧ�����������κ� NPC һ�����ɡ�
            NPCID.Sets.ActsLikeTownNPC[Type] = true;

            // ������÷�ֹ���Ҹ���ť
            NPCID.Sets.NoTownNPCHappiness[Type] = true;

            // �ٴ�ǿ�������ڴ� NPC ������ʵ���ϵĳ��� NPC��������Ҫ������Ϸ��ʹ��������ʱ��Ȼϣ���� NPC �����Զ���/������ơ�
            // Ϊ��������һ�㣬����ֻ��ʹ�˹��ӷ��� true ���ɣ��⽫ʹ��Ϸ������NPCʱ���� TownNPCName ������ȷ��NPC�����ơ�
            NPCID.Sets.SpawnsWithCustomName[Type] = true;

            // ��ݹ�ͷ���˲������Ż������ر��Ǵ򿪻�ر����ǣ����������ϣ������NPC�ܹ�������������ǻ�����
            // ��ȡ��������һ�е�ע�͡�

            //NPCID.Sets.AllowDoorInteraction[Type] = true;

            // Ӱ��NPC�ڹ���ͼ���е����
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Velocity = 1f, // ����npc���ͺ�������x��������+1��

                Direction = 1 // ��ʾ��߶�1��ʾ�ұߡ�Ĭ�������NPC���������ƣ���AnalysisPerson�������Ҳ����
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

        // ȷ��������� NPC ����, ��Ϊ����һ������ npc�������Զ���������.
        public override bool CanChat()
        {
            return true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // ���ǿ���ʹ�� AddRange �����Ƕ�ε��� Add ��һ����Ӷ����
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {

                // ���ô˳��� NPC �ڹ���ͼ�����г�����ѡ����Ⱥϵ��
                // ���ڳ���NPC��ͨ����������Ϊ��NPC�Ҹ�����ص���ϲ��������Ⱥϵ��
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Underground,

                // �ڹ���ͼ������������ NPC �Ķ��ؽ����ı���
				new FlavorTextBestiaryInfoElement("Hailing from a mysterious greyscale cube world, the Analysis Bone Merchant will show you how to make a mysterious merchant underground with tModLoader."),

                // ����������Ҫ���������Ӷ��Ԫ��
                // ��������ʹ�ñ��ػ�������μ� Localization/en-US.lang��
				new FlavorTextBestiaryInfoElement("Mods.AnalysisMod.Bestiary.AnalysisBoneMerchant")
            });
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            // �� NPC �ܵ��˺�ʱ����������
            int num = NPC.life > 0 ? 1 : 5;

            for (int k = 0; k < num; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<Sparkle>());
            }

            // �� NPC ��ɱ��ʱ����Ѫ��Ч����
            if (Main.netMode != NetmodeID.Server && NPC.life <= 0)
            {
                // ����Ѫ�����͡���NPC��������˸���塣��6���ܹ���
                string variant = "";
                if (NPC.IsShimmerVariant) variant += "_Shimmer";
                int headGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Head").Type;
                int armGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Arm").Type;
                int legGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Leg").Type;

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
            // ����κ�����ڵ��²��ұ������з�����Ʒ���������ͷ���˽���һ���������ɡ�
            if (spawnInfo.Player.ZoneDirtLayerHeight && spawnInfo.Player.inventory.Any(item => item.type == ModContent.ItemType<AnalysisItem>()))
            {
                return 0.34f;
            }

            // ������������������������򲻻����ɷ�����ͷ���ˡ�
            return 0f;
        }

        public override string GetChat()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();

            // ��Щ��NPC���㽻̸ʱ���ܸ���������顣
            chat.Add(Language.GetTextValue("Mods.AnalysisMod.Dialogue.AnalysisBoneMerchant.StandardDialogue1"));
            chat.Add(Language.GetTextValue("Mods.AnalysisMod.Dialogue.AnalysisBoneMerchant.StandardDialogue2"));
            chat.Add(Language.GetTextValue("Mods.AnalysisMod.Dialogue.AnalysisBoneMerchant.StandardDialogue3"));
            return chat; // ���챻��ʽת��Ϊ�ַ�����
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
          // ������������ῴ����Щ���찴ť
            button = Language.GetTextValue("LegacyInterface.28"); //���ǡ��̵ꡱһ�ʵĹؼ�
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
                // ���ʹ��������Ʒ����ʹ�ô˷���
                int itemType = ModContent.ItemType<AnalysisCustomAmmoGun>();
                Main.GetItemDrawFrame(itemType, out item, out itemFrame);
                horizontalHoldoutOffset = (int)Main.DrawPlayerItemPos(1f, itemType).X - 12;
            }
            else
            {
                // �������ʵ���ϲ���һ��������Ʒ������Ȼ����ʹ�á�
                item = ModContent.Request<Texture2D>(Texture + "_Shimmer_Gun").Value;
                itemFrame = item.Frame();
                horizontalHoldoutOffset = -2;
            }
        }
    }
}
