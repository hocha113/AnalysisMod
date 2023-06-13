using AnalysisMod.AnalysisCommon.Players;
using Terraria;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Accessories
{
    public class AnalysisImmunityAccessory : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 32;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 1);
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // Set the HasAnalysisImmunityAcc bool to true to ensure we have this accessory
            // And apply the changes in ModPlayer.PostHurt correctly

            // 将 HasAnalysisImmunityAcc 布尔值设置为 true，以确保我们拥有此配件
            // 并在 ModPlayer.PostHurt 中正确应用更改
            player.GetModPlayer<AnalysisImmunityPlayer>().HasAnalysisImmunityAcc = true;
        }
    }
}
