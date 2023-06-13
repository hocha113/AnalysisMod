using AnalysisMod.AnalysisContent.Items;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisCommon.GlobalNPCs
{
    class AnalysisNPCShop : GlobalNPC
    {
        public override void ModifyShop(NPCShop shop)
        {
            if (shop.NpcType == NPCID.Dryad)
            {
                // Adding an item to a vanilla NPC is easy:
                // This item sells for the normal price.

                // 给普通NPC添加物品很容易：
                // 此物品以正常价格出售。
                shop.Add<AnalysisMountItem>();

                // We can use shopCustomPrice and shopSpecialCurrency to support custom prices and currency. Usually a shop sells an item for item.value.
                // Editing item.value in SetupShop is an incorrect approach.

                // 我们可以使用shopCustomPrice和shopSpecialCurrency来支持自定义价格和货币。通常商店会按照item.value的价值出售商品。
                // 在SetupShop中编辑item.value是一种错误的方法。

                // This shop entry sells for 2 Defenders Medals.
                // 这个商店条目以2个卫士奖章的价格出售。
                shop.Add(new Item(ModContent.ItemType<AnalysisMountItem>())
                {
                    shopCustomPrice = 2,
                    shopSpecialCurrency = CustomCurrencyID.DefenderMedals // omit this line if shopCustomPrice should be in regular coins.
                                                                          // 如果shopCustomPrice应该在普通硬币中，则省略此行。
                });

                // This shop entry sells for 3 of a custom currency added in our mod.
                // 这个商店条目以我们mod中新增的3种自定义货币之一的价格出售。
                shop.Add(new Item(ModContent.ItemType<AnalysisMountItem>())
                {
                    shopCustomPrice = 2,
                    shopSpecialCurrency = AnalysisMod.AnalysisCustomCurrencyId
                });
            }
            else if (shop.NpcType == NPCID.Wizard)
            {
                // shopContents.Add(ModContent.ItemType<Infinity>(), ChestLoot.Condition.InExpertMode);
            }
            else if (shop.NpcType == NPCID.Stylist)
            {
                shop.Add<AnalysisHairDye>();
            }

            // Analysis of adding new items with complex conditions in the Merchant shop.
            // Style 1 check for application

            // 分析如何在Merchant商店中添加具有复杂条件的新物品。
            // 样式1检查申请
            if (shop.FullName != NPCShopDatabase.GetShopName(NPCID.Merchant, "Shop"))
                return;

            // Style 2 check for application
            // 样式2检查申请
            if (shop.NpcType != NPCID.Merchant || shop.Name != "Shop")
                return;

            // Style 3 check for application (works just if NPC has one shop)
            // 样式3检查申请（仅适用于NPC只有一个商店）
            if (shop.NpcType != NPCID.Merchant)
                return;

            // Adding AnalysisTorch to Merchant, with condition being sold only during daytime. Have it appear just after Torch
            // 将AnalysisTorch添加到Merchant，条件为仅在白天销售。让它紧随Torch之后显示
            shop.InsertAfter(ItemID.Torch, ModContent.ItemType<AnalysisContent.Items.Placeable.AnalysisTorch>(), Condition.TimeDay);

            // Hiding Copper Pickaxe and Copper Axe. They will never appear in Merchant shop anymore
            // However, this approach may fail if item doesn't exists in shop.

            // 隐藏铜镐和铜斧。它们将不再出现在Merchant商店里
            // 但是，如果商品不存在于商店，则此方法可能失败。
            shop.GetEntry(ItemID.CopperAxe).Disable();

            // Safer approach for disabling item
            // 禁用商品更安全可靠
            if (shop.TryGetEntry(ItemID.CopperPickaxe, out NPCShop.Entry entry))
            {
                entry.Disable();
            }

            // Adding new Condition to Blue Flare. Now it will appear just if player carries a Flare Gun in their inventory AND is in Snow biome
            // 添加新条件到蓝色信号弹上。现在只有当玩家携带闪光枪并处于雪地生态时才会显示它
            shop.GetEntry(ItemID.BlueFlare).AddCondition(Condition.InSnow);

            // Let's add an item that appears just during Windy day and when NPC is happy enough (can sell pylons)
            // If condition is fulfilled, add an item to the shop.

            // 让我们添加一个物品，它只会在有风的日子里出现，并且NPC足够高兴（可以销售传送门）
            // 如果条件满足，则将物品添加到商店中。
            shop.Add<AnalysisItem>(Condition.HappyWindyDay, Condition.HappyEnough);

            // Custom condition, opposite of conditions for AnalysisItem above.
            // 自定义条件，与上面AnalysisItem的条件相反。
            var redPotCondition = new Condition("Mods.AnalysisMod.Conditions.NotSellingAnalysisItem", () => !Condition.HappyWindyDay.IsMet() || !Condition.HappyEnough.IsMet());
            // Otherwise, if condition is not fulfilled, then let's check if its For The Worthy world and then sell Red Potion.
            // 否则，如果不符合条件，则检查是否为“值得”的世界，然后出售红色药水。
            shop.Add(ItemID.RedPotion, redPotCondition, Condition.ForTheWorthyWorld);
        }
    }
}
