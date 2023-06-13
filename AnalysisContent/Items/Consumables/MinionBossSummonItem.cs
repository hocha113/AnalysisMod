using AnalysisMod.AnalysisContent.NPCs.MinionBoss;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Consumables
{
    // This is the item used to summon a boss, in this case the modded Minion Boss from Analysis Mod. For vanilla boss summons, see comments in SetStaticDefaults
    // 这是用于召唤Boss的物品，本例中来自Analysis Mod的改造后随从Boss。有关原版Boss召唤，请参见SetStaticDefaults中的注释。
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