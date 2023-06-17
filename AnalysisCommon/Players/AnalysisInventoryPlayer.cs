using AnalysisMod.AnalysisContent.Items.Placeable.Furniture;
using AnalysisMod.AnalysisContent.Items.Placeable;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AnalysisMod.AnalysisContent.Items;

namespace AnalysisMod.AnalysisCommon.Players
{
    public class AnalysisInventoryPlayer : ModPlayer
    {
        // AddStartingItems是一种方法，可用于向玩家的起始库存中添加物品。
        // 当玩家以mediumcore死亡时，也会调用它
        // 返回一个枚举器，其中包含要添加到库存中的物品。
        // 此方法将AnalysisItem和256个金矿石添加到玩家的库存中。
        // 如果您知道什么是“yield return”，也可以在此处使用它（如果您更喜欢）。
        public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
        {
            if (mediumCoreDeath)
            {
                return new[] {
                    new Item(ItemID.HealingPotion)
                };
            }

            return new[] {
                new Item(ModContent.ItemType<AnalysisItem>()),
                new Item(ItemID.GoldOre, 256),
                new Item(ModContent.ItemType<AnalysisBlock>(), 256),
                new Item(ModContent.ItemType<AnalysisWall>(), 256),
                new Item(ModContent.ItemType<AnalysisOre>(), 256),
                new Item(ModContent.ItemType<AnalysisChair>(), 99),
                new Item(ModContent.ItemType<AnalysisTable>(), 99),
                new Item(ModContent.ItemType<AnalysisChest>(), 99),
                new Item(ModContent.ItemType<AnalysisPlatform>(), 256),
                new Item(ItemID.Zenith, 1)
            };
        }
        // ModifyStartingItems是AddStartingItems的更详细版本，允许您删除其他模组或原版游戏添加的物品。你理论上也可以使用它来添加物品，
        // 但建议只在AddStartingItems中这样做。
        // 在这个分析中，我们阻止Terraria在旅程模式下向玩家的库存中添加铁斧头。（如果你想阻止另一个mod添加一个项目，则其条目为该mod的内部名称，
        // 如itemsByMod [“SomeMod”] Terraria 的条目总是命名为 “Terraria”
        public override void ModifyStartingInventory(IReadOnlyDictionary<string, List<Item>> itemsByMod, bool mediumCoreDeath)
        {
            itemsByMod["Terraria"].RemoveAll(item => item.type == ItemID.IronAxe);
        }
    }
}
