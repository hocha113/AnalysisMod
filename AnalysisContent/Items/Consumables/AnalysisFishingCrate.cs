using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Consumables
{
    // Basic code for a fishing crate
    // The catch code is in a separate ModPlayer class (AnalysisFishingPlayer)
    // The placed tile is in a separate ModTile class

    // 钓鱼箱的基本代码
    // 捕获代码在单独的ModPlayer类（AnalysisFishingPlayer）中
    // 放置的图块在单独的ModTile类中
    public class AnalysisFishingCrate : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Disclaimer for both of these sets (as per their docs): They are only checked for vanilla item IDs, but for cross-mod purposes it would be helpful to set them for modded crates too
            // 免责声明：根据它们的文档，这两个集合仅针对原版物品ID进行检查，但为了跨模组目的，设置modded crates也会很有帮助。
            ItemID.Sets.IsFishingCrate[Type] = true;
            //ItemID.Sets.IsFishingCrateHardmode[Type] = true; // This is a crate that mimics a pre-hardmode biome crate, so this is commented out

            Item.ResearchUnlockCount = 10;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.AnalysisFishingCrate>());
            Item.width = 12; //The hitbox dimensions are intentionally smaller so that it looks nicer when fished up on a bobber
                             //碰撞箱尺寸故意较小，以便在浮标上钓起时看起来更好看。

            Item.height = 12;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(0, 2);
        }

        // TODO AnalysisMod: apply this to all items where necessary (which are not automatically detected)
        // TODO AnalysisMod: 在必要时将其应用于所有物品（不能自动检测到的物品）
        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.Crates;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            // Drop a special weapon/accessory etc. specific to this crate's theme (i.e. Sky Crate dropping Fledgling Wings or Starfury)
            // 掉落一个特定于此箱子主题（例如Sky Crate掉落Fledgling Wings或Starfury） 的武器/配件等
            int[] themedDrops = new int[] {
                ModContent.ItemType<Accessories.AnalysisBeard>(),
                ModContent.ItemType<Accessories.AnalysisStatBonusAccessory>()
            };
            itemLoot.Add(ItemDropRule.OneFromOptionsNotScalingWithLuck(1, themedDrops));

            // Drop coins
            // 掉落硬币
            itemLoot.Add(ItemDropRule.Common(ItemID.GoldCoin, 4, 5, 13));

            // Drop pre-hm ores, with the addition of one from AnalysisMod
            // 掉落pre-hm矿石，并添加AnalysisMod中的一种
            IItemDropRule[] oreTypes = new IItemDropRule[] {
                ItemDropRule.Common(ItemID.CopperOre, 1, 30, 50),
                ItemDropRule.Common(ItemID.TinOre, 1, 30, 50),
                ItemDropRule.Common(ItemID.IronOre, 1, 30, 50),
                ItemDropRule.Common(ItemID.LeadOre, 1, 30, 50),
                ItemDropRule.Common(ItemID.SilverOre, 1, 30, 50),
                ItemDropRule.Common(ItemID.TungstenOre, 1, 30, 50),
                ItemDropRule.Common(ItemID.GoldOre, 1, 30, 50),
                ItemDropRule.Common(ItemID.PlatinumOre, 1, 30, 50),
                ItemDropRule.Common(ModContent.ItemType<Placeable.AnalysisOre>(), 1, 30, 50),
            };
            itemLoot.Add(new OneFromRulesRule(7, oreTypes));

            // Drop pre-hm bars (except copper/tin), with the addition of one from AnalysisMod
            // 掉落pre-hm金属条（除铜/锡外），并添加AnalysisMod中的一种
            IItemDropRule[] oreBars = new IItemDropRule[] {
                ItemDropRule.Common(ItemID.IronBar, 1, 10, 21),
                ItemDropRule.Common(ItemID.LeadBar, 1, 10, 21),
                ItemDropRule.Common(ItemID.SilverBar, 1, 10, 21),
                ItemDropRule.Common(ItemID.TungstenBar, 1, 10, 21),
                ItemDropRule.Common(ItemID.GoldBar, 1, 10, 21),
                ItemDropRule.Common(ItemID.PlatinumBar, 1, 10, 21),
                ItemDropRule.Common(ModContent.ItemType<Placeable.AnalysisBar>(), 1, 10, 21),
            };
            itemLoot.Add(new OneFromRulesRule(4, oreBars));

            // Drop an "exploration utility" potion, with the addition of one from AnalysisMod
            // 掉落“勘探实用”药水，并添加AnalysisMod中的一种
            IItemDropRule[] explorationPotions = new IItemDropRule[] {
                ItemDropRule.Common(ItemID.ObsidianSkinPotion, 1, 2, 5),
                ItemDropRule.Common(ItemID.SpelunkerPotion, 1, 2, 5),
                ItemDropRule.Common(ItemID.HunterPotion, 1, 2, 5),
                ItemDropRule.Common(ItemID.GravitationPotion, 1, 2, 5),
                ItemDropRule.Common(ItemID.MiningPotion, 1, 2, 5),
                ItemDropRule.Common(ItemID.HeartreachPotion, 1, 2, 5),
                ItemDropRule.Common(ModContent.ItemType<AnalysisContent.Items.Consumables.AnalysisBuffPotion>(), 1, 2, 5),
            };
            itemLoot.Add(new OneFromRulesRule(4, explorationPotions));

            // Drop (pre-hm) resource potion
            // 掉落(pre-hm)资源药水
            IItemDropRule[] resourcePotions = new IItemDropRule[] {
                ItemDropRule.Common(ItemID.HealingPotion, 1, 5, 18),
                ItemDropRule.Common(ItemID.ManaPotion, 1, 5, 18),
            };
            itemLoot.Add(new OneFromRulesRule(2, resourcePotions));

            // Drop (high-end) bait
            // 附加高级诱饵
            IItemDropRule[] highendBait = new IItemDropRule[] {
                ItemDropRule.Common(ItemID.JourneymanBait, 1, 2, 7),
                ItemDropRule.Common(ItemID.MasterBait, 1, 2, 7),
            };
            itemLoot.Add(new OneFromRulesRule(2, highendBait));
        }
    }
}
