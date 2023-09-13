using AnalysisMod.AnalysisContent.NPCs.MinionBoss;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Consumables
{
    // This is the item used to summon a boss, in this case the modded Minion Boss from Analysis Mod. For vanilla boss summons, see comments in SetStaticDefaults
    // 这是用于召唤Boss的物品，本例中来自Analysis Mod的改造后随从Boss。有关原版Boss召唤，请参见SetStaticDefaults中的注释。

    // 如果你想实现更多的召唤判定限制可以参照一下NPC定义代码片段：

    /*
    //
    // 摘要:
    //     Denotes whether or not Plantera has been defeated at least once in the current
    //     world.
    表示当前世界是否至少击败过普朗特拉(世纪之花)
    public static bool downedPlantBoss;
    //
    // 摘要:
    //     Denotes whether or not at least one Clown has been killed in the current world.
    //     Only used to make the Clothier sell the Clown set once at least one has been
    //     killed.
    表示当前世界是否至少有一个小丑被击败。
    只用于在至少有一个小丑被击败后，让服装商出售小丑套装。
    public static bool downedClown;
    //
    // 摘要:
    //     Denotes whether or not at least one Pirate Invasion has been defeated in the
    //     current world.
    表示当前世界是否至少打败了一次海盗入侵。
    public static bool downedPirates;
    //
    // 摘要:
    //     Denotes whether or not at least one Goblin Army has been defeated in the current
    //     world.
    表示当前世界是否已经击败了至少一支哥布林军队。
    public static bool downedGoblins;
    //
    // 摘要:
    //     Denotes whether or not Golem has been defeated at least once in the current world.
    表示在当前世界中是否至少击败过石巨人。
    public static bool downedGolemBoss;
    //
    // 摘要:
    //     Denotes whether or not King Slime has been defeated at least once in the current
    //     world.
    表示当前世界是否至少击败过史莱姆王一次。
    public static bool downedSlimeKing;
    //
    // 摘要:
    //     Denotes whether or not at least one Queen Bee has been defeated in the current
    //     world.
    表示当前世界是否至少击败过一只蜂后。
    public static bool downedQueenBee;
    //
    // 摘要:
    //     Denotes whether or not Skeletron has been defeated at least once in the current
    //     world.
    表示当前世界是否至少击败过骷髅王一次。
    public static bool downedBoss3;
    //
    // 摘要:
    //     Denotes whether or not the Frost Legion has been defeated at least once in the
    //     current world.
    表示当前世界是否至少击败过霜军团一次。
    public static bool downedFrost;
    //
    // 摘要:
    //     Denotes whether or not at least one Martian Madness event has been cleared in
    //     the current world.
    表示当前世界中是否已经清除了至少一个火星狂潮事件。
    public static bool downedMartians;
    //
    // 摘要:
    //     Denotes whether or not at least one Pumpking has been defeated in the current
    //     world.
    表示当前世界中是否已经打败了至少一个南瓜头怪物。
    public static bool downedHalloweenKing;
    //
    // 摘要:
    //     Denotes whether or not at least one Mourning Wood has been defeated in the current
    //     world.
    表示在当前的游戏世界里，你是否已经打倒了哀悼之木（Mourning Wood）中的任意一个BOSS
    public static bool downedHalloweenTree;
    //
    // 摘要:
    //     Denotes whether or not the Eater of Worlds OR the Brain of Cthulhu have been
    //     defeated at least once in the current world.
    //     This does NOT track the two of them separately; you will need to establish your
    //     own fields in a Terraria.ModLoader.ModSystem for that.
    指示Eater of Worlds或Brain of Cthulhu在目前的游戏中有没有被打敗過。这不会分别追踪它们两个；您需要为此建立自己的领域Terraria.ModLoader.ModSystem.
    public static bool downedBoss2;
    //
    // 摘要:
    //     Denotes whether or not at least one Ice Queen has been defeated in the current
    //     world.
    指示在目前的游戏中，您是否已经成功地打倒了冰雪女王（Ice Queen）其中之一
    public static bool downedChristmasIceQueen;
    //
    // 摘要:
    //     Denotes whether or not at least one Everscream has been defeated in the current
    //     world.
    指示在目前的游戏中，您是不是已经成功地打倒了常青树人(Everscream) 中其中之一
    public static bool downedChristmasTree;
    //
    // 摘要:
    //     Denotes whether or not at least one Santa-NK1 has been defeated in the current
    //     world.
    指示在目前的游戏中，您是不是已经成功地打倒了圣诞机器人(Santa-NK1) 中其中之一
    public static bool downedChristmasSantank;
    //
    // 摘要:
    //     Denotes whether or not the Lunatic Cultist has been defeated at least once in
    //     the current world.
    表示当前世界中是否至少击败过疯狂邪教徒。
    public static bool downedAncientCultist;
    //
    // 摘要:
    //     Denotes whether or not the Moon Lord has been defeated at least once in the current
    //     world.
    表示当前世界中是否至少击败过月球领主
    public static bool downedMoonlord;
    //
    // 摘要:
    //     Denotes whether or not the Solar Pillar has been defeated at least once in the
    //     current world.
    表示当前世界中是否至少击败过太阳柱。
    public static bool downedTowerSolar;
    //
    // 摘要:
    //     Denotes whether or not the Vortex Pillar has been defeated at least once in the
    //     current world.
    表示漩涡柱在当前世界中是否至少被击败一次。
    public static bool downedTowerVortex;
    //
    // 摘要:
    //     Denotes whether or not the Nebula Pillar has been defeated at least once in the
    //     current world.
    表示当前世界中是否至少击败过星云柱。表示当前世界中是否至少击败过星云柱。
    public static bool downedTowerNebula;
    //
    // 摘要:
    //     Denotes whether or not the Stardust Pillar has been defeated at least once in
    //     the current world.
    表示当前世界中是否至少击败过星尘柱。
    public static bool downedTowerStardust;
    //
    // 摘要:
    //     Denotes whether or not the Empress of Light has been defeated at least once in
    //     the current world.
    表示当前世界中是否至少击败过光之女皇
    public static bool downedEmpressOfLight;
    //
    // 摘要:
    //     Denotes whether or not Queen Slime has been defeated at least once in the current
    //     world.
    表示当前世界中是否至少击败过史莱姆女王。
    public static bool downedQueenSlime;
    //
    // 摘要:
    //     Denotes whether or not Duke Fishron has been defeated at least once in the current
    //     world.
    表示当前世界中是否至少击败过猪鲨公爵。
    public static bool downedFishron;
    */

    public class MinionBossSummonItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 3;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12; // This helps sort inventory know that this is a boss summoning Item.
                                                              // 这有助于将其分类为Boss召唤物品。

            // If this would be for a vanilla boss that has no summon item, you would have to include this line here:
            // 如果这是用于没有召唤物品的原版boss，则必须在此处包括以下行：

            // NPCID.Sets.MPAllowedEnemies[NPCID.Plantera] = true;

            // Otherwise the UseItem code to spawn it will not work in multiplayer
            // 否则，在多人游戏中，UseItem代码生成它将无法正常工作
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 20;
            Item.value = 100;
            Item.rare = ItemRarityID.Blue;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
        }

        public override bool CanUseItem(Player player)
        {
            // If you decide to use the below UseItem code, you have to include !NPC.AnyNPCs(id), as this is also the check the server does when receiving MessageID.SpawnBoss.
            // If you want more constraints for the summon item, combine them as boolean expressions:

            // 如果您决定使用下面的UseItem代码，则必须包括!NPC.AnyNPCs(id)，因为这也是服务器在接收MessageID.SpawnBoss时进行检查的内容。
            // 如果要对召唤物品添加更多约束，请将它们组合为布尔表达式：

            //    return !Main.dayTime && !NPC.AnyNPCs(ModContent.NPCType<MinionBossBody>()); would mean "not daytime and no MinionBossBody currently alive"//将意味着“不是白天且当前没有MinionBossBody存活”
            return !NPC.AnyNPCs(ModContent.NPCType<MinionBossBody>());
        }

        public override bool? UseItem(Player player)
        {
            bool playerDefeatedSkeletron = NPC.downedBoss2; // 判定玩家是否击败了骷髅王
            //bool playerInDungeon = Main.LocalPlayer.ZoneDungeon; // 判定玩家是否在地牢环境

            if (player.whoAmI == Main.myPlayer)
            {
                // If the player using the item is the client
                // (explicitely excluded serverside here)

                // 如果使用该项目的玩家是客户端
                //（此处明确排除了服务器端）
                SoundEngine.PlaySound(SoundID.Roar, player.position);

                int type = ModContent.NPCType<MinionBossBody>();

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    // If the player is not in multiplayer, spawn directly
                    // 如果玩家不在多人游戏中，则直接生成
                    NPC.SpawnOnPlayer(player.whoAmI, type);
                }
                else
                {
                    // If the player is in multiplayer, request a spawn
                    // This will only work if NPCID.Sets.MPAllowedEnemies[type] is true, which we set in MinionBossBody

                    // 如果玩家正在进行多人游戏，则请求生成
                    // 仅当NPCID.Sets.MPAllowedEnemies[type]为true时才能正常工作，我们已经设置了MinionBossBody
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
                }
            }

            return true;
        }

        // Please see Content/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AnalysisItem>()
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}