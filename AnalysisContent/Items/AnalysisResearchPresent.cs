using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items
{
    public class AnalysisResearchPresent : ModItem
    {
        public override void SetStaticDefaults()
        {
            // 必须研究与游戏中的物品数量相同次数。
            // 如果完全研究，并添加了新的模组，则会变为未研究状态并需要更多时间进行研究
            // 研究量永远不会降低或超过最大限制9999。
            Item.ResearchUnlockCount = Utils.Clamp(ItemLoader.ItemCount, 1, 9999);

            // 使用MonoMod钩子允许我们的掉落物通过掉落物系统运行。
            On_CreativeUI.SacrificeItem_refItem_refInt32_bool += OnSacrificeItem;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.GoodieBag);
        }

        // 即使您已经拥有无限个，这也可以使礼物被研究。
        // 这不是研究系统的标准用法，但允许重新运行“完成研究”的效果
        private CreativeUI.ItemSacrificeResult OnSacrificeItem(On_CreativeUI.orig_SacrificeItem_refItem_refInt32_bool orig,
                ref Item item, out int amountWeSacrificed, bool returnRemainderToPlayer)
        {

            // 如果被牺牲的物品与我们具有相同类型（即AnalysisResearchPresent）且已完全研究
            if (item.type == Type && CreativeUI.GetSacrificesRemaining(Type) == 0)
            {

                // 重新解锁所有配件，以防万一修改了模组
                OnResearched(true);

                // 我们总是在进行礼物研究时失去一个礼物，即使您已经拥有无限个。为向用户显示发生了某些事情
                item.stack -= 1;

                // 这段代码是从SacrificeItem的结尾复制而来
                if (item.stack > 0 && returnRemainderToPlayer)
                {
                    item.position.X = Main.player[Main.myPlayer].Center.X - item.width / 2;
                    item.position.Y = Main.player[Main.myPlayer].Center.Y - item.height / 2;
                    item = Main.LocalPlayer.GetItem(Main.myPlayer, item, GetItemSettings.InventoryUIToInventorySettings);
                }

                // 这是祭品计数器增加的数量。实际上我们没有改变总祭品数量，所以这里是0.
                amountWeSacrificed = 0;

                // 返回SacrifiedAndDone, 以便动画和效果发生
                return CreativeUI.ItemSacrificeResult.SacrificedAndDone;
            }

            // 否则，请调用原始方法来运行默认行为
            return orig(ref item, out amountWeSacrificed, returnRemainderToPlayer);
        }

        public override void OnResearched(bool fullyResearched)
        {
            if (fullyResearched)
            {
                LearnAllAccessories();
            }
            else
            {

                // 尝试学习每个被牺牲者随机配件
                int count = 0;
                for (int j = Item.stack; j > 0; j--)
                {
                    if (LearnRandomAccessory())
                    {
                        count++;
                    }
                }
                if (count == 0)
                {
                    Main.NewText("No new accessory...");
                }
                else
                {
                    Main.NewText("Learned " + count + " new accessor" + (count == 1 ? "y" : "ies") + " !");
                }
            }
        }

        // 尝试1000个随机物品ID，如果我们随机选择了一个配件，则尝试学习它
        private bool LearnRandomAccessory()
        {
            for (int i = 0; i < 1000; i++)
            {
                int type = Main.rand.Next(1, ItemLoader.ItemCount);
                if (ContentSamples.ItemsByType[type].accessory)
                {
                    if (CreativeUI.ResearchItem(type) == CreativeUI.ItemSacrificeResult.SacrificedAndDone)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void LearnAllAccessories()
        {
            for (int i = 1; i < ItemLoader.ItemCount; i++)
            {
                if (ContentSamples.ItemsByType[i].accessory)
                {
                    CreativeUI.ResearchItem(i);
                }
            }

            Main.NewText("You got all accessories!");
        }
    }
}
