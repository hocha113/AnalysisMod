using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisCommon.GlobalItems
{
    // These 2 files show off making a new extractinator type. This Analysis converts any torch placing item into any other torch placing item. ModSystem.PostSetupContent is used instead of GlobalItem.SetStaticDefaults to determine which items are torch items because it needs to run after all mods setup their Content.
    // 这两个文件展示了如何制作一个新的提取器类型。该分析将任何放置火把物品转换为其他放置火把物品。使用ModSystem.PostSetupContent而不是GlobalItem.SetStaticDefaults来确定哪些物品是火把物品，因为它需要在所有模组设置其内容之后运行。
    public class TorchExtractinatorGlobalItem : GlobalItem
    {
        public override void ExtractinatorUse(int extractType, int extractinatorBlockType, ref int resultType, ref int resultStack)
        {
            // If the extractinator type isn't torch, we won't change anything
            // 如果提取器类型不是火把，我们将不会改变任何东西
            if (extractType != ItemID.Torch)
                return;

            // If it is, we set stack to 1 and return a random torch. If the user is using the Chlorophyte Extractinator, we return Ultrabright Torch 10% of the time.
            // 如果是，则将堆栈设置为1并返回一个随机火把。如果用户正在使用叶绿萃取器，我们有10%的几率返回超亮火把。
            resultStack = 1;
            if (extractinatorBlockType == TileID.ChlorophyteExtractinator && Main.rand.NextBool(10))
            {
                resultType = ItemID.UltrabrightTorch;
                return;
            }

            resultType = Main.rand.Next(TorchExtractinatorModSystem.TorchItems);
        }
    }

    public class TorchExtractinatorModSystem : ModSystem
    {
        internal static List<int> TorchItems;

        public override void PostSetupContent()
        {
            // Here we iterate through all items and find items that place tiles that are indicated as being torch tiles. We set these items to the extractinator mode of ItemID.Torch to indicate that they all share the torch extractinator result pool.
            // 在这里，我们遍历所有物品，并找到放置被指定为火把砖块的物品。我们将这些物品设置为ItemID.Torch的extractinator模式，以表示它们都共享火把提取器结果池。
            ItemID.Sets.ExtractinatorMode[ItemID.Torch] = ItemID.Torch;
            TorchItems = new List<int>();

            for (int i = 0; i < ItemLoader.ItemCount; i++)
            {
                int createTile = ContentSamples.ItemsByType[i].createTile;
                if (createTile != -1 && TileID.Sets.Torch[createTile] && ItemID.Sets.ExtractinatorMode[i] == -1)
                {
                    ItemID.Sets.ExtractinatorMode[i] = ItemID.Torch;
                    TorchItems.Add(i);
                }
            }
        }
    }
}
