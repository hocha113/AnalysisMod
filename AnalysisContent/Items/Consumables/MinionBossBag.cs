using AnalysisMod.AnalysisContent.Items.Armor.Vanity;
using AnalysisMod.AnalysisContent.NPCs.MinionBoss;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Consumables
{
    // Basic code for a boss treasure bag
    // Boss宝藏袋的基本代码
    public class MinionBossBag : ModItem
    {
        public override void SetStaticDefaults()
        {
            // This set is one that every boss bag should have.
            // It will create a glowing effect around the item when dropped in the world.
            // It will also let our boss bag drop dev armor..

            // 这个设置是每个Boss宝藏袋都应该有的。
            // 它会在物品掉落到世界上时创建一个发光效果。
            // 它还可以让我们的Boss宝藏袋掉落开发者盔甲。
            ItemID.Sets.BossBag[Type] = true;
            ItemID.Sets.PreHardmodeLikeBossBag[Type] = true; // ..But this set ensures that dev armor will only be dropped on special world seeds, since that's the behavior of pre-hardmode boss bags
                                                             // ...但这套装备确保了只有在特殊的世界种子中才会掉落开发者盔甲，因为这是前硬模式Boss宝藏袋的行为。

            Item.ResearchUnlockCount = 3;
        }

        public override void SetDefaults()
        {
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.Purple;
            Item.expert = true; // This makes sure that "Expert" displays in the tooltip and the item name color changes
                                // 这确保了“专家”显示在工具提示中，并更改了物品名称颜色。
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            // We have to replicate the expert drops from MinionBossBody here
            // 我们必须在此处复制MinionBossBody中的专家掉落

            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<MinionBossMask>(), 7));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<AnalysisItem>(), 1, 12, 16));
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<MinionBossBody>()));
        }
    }
}
