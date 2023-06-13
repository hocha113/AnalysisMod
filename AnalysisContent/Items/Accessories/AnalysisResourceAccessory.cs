using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AnalysisMod.AnalysisCommon.Players;
using Terraria.Localization;

namespace AnalysisMod.AnalysisContent.Items.Accessories
{
    public class AnalysisResourceAccessory : ModItem
    {
        public static readonly int ResourceBoost = 100;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ResourceBoost);

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 32;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(gold: 5);
            Item.accessory = true;
            Item.rare = ItemRarityID.Red;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var modPlayer = player.GetModPlayer<AnalysisResourcePlayer>();
            modPlayer.AnalysisResourceMax2 += ResourceBoost; // add 100 to the AnalysisResourceMax2, which is our max for Analysis resource.
                                                             // 将AnalysisResourceMax2增加100，这是我们的分析资源上限。
            modPlayer.AnalysisResourceRegenRate *= 6f; // multiply our resource regeneration speed by 6.
                                                       // 将我们的资源再生速度乘以6。
        }
    }
}
