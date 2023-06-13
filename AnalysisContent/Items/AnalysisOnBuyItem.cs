using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items
{
    /// <summary>
    /// This item showcases one of the ways for you to do something when an item is bought from an NPC with a shop.<br/>
    /// 这个项目展示了当从NPC商店购买物品时，您可以采取的一种方法。
    /// </summary>
    public class AnalysisOnBuyItem : ModItem
    {
        public static LocalizedText DeathMessage { get; private set; }

        public override void SetStaticDefaults()
        {
            // See the localization files for more info! (Localization/en-US.hjson)
            // 请查看本地化文件以获取更多信息！（Localization/en-US.hjson）
            DeathMessage = this.GetLocalization(nameof(DeathMessage));
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(silver: 1, copper: 50);
            Item.maxStack = 9999;
        }

        // Note that alternatively, you can use the ModPlayer.PostBuyItem hook to achieve the same functionality!
        // 注意，您还可以使用ModPlayer.PostBuyItem钩子来实现相同的功能！
        public override void OnCreated(ItemCreationContext context)
        {
            if (context is not BuyItemCreationContext buyContext)
            {
                return;
            }

            // For fun, we'll give the buying player a 50% chance to die whenever they buy this item from an NPC.
            // 为了好玩起见，每当玩家从NPC购买此物品时，我们将给予他们50％的死亡几率。
            if (!Main.rand.NextBool())
            {
                return;
            }

            // This is only ever called on the local client, so the local player will do.
            // 这仅在本地客户端上调用，因此只有本地玩家会受到影响。
            Player player = Main.LocalPlayer;
            player.KillMe(PlayerDeathReason.ByCustomReason(DeathMessage.Format(player.name)), 9999, 0);
        }
    }
}
