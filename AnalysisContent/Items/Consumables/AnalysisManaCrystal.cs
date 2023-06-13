using AnalysisMod.AnalysisCommon.Players;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Consumables
{
    // This file showcases how to create an item that increases the player's maximum mana on use.
    // Within your ModPlayer, you need to save/load a count of usages. You also need to sync the data to other players.
    // The overlay used to display the custom mana crystals can be found in Common/UI/ResourceDisplay/VanillaManaOverlay.cs

    // 这个文件展示了如何创建一个使用后增加玩家最大法力值的物品。
    // 在你的ModPlayer中，你需要保存/加载使用次数计数。你还需要将数据同步到其他玩家。
    // 用于显示自定义魔晶石的覆盖层可以在Common/UI/ResourceDisplay/VanillaManaOverlay.cs中找到。
    internal class AnalysisManaCrystal : ModItem
    {
        public static readonly int MaxAnalysisManaCrystals = 10;
        public static readonly int ManaPerCrystal = 10;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ManaPerCrystal, MaxAnalysisManaCrystals);

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 10;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.ManaCrystal);
        }

        public override bool CanUseItem(Player player)
        {
            // This check prevents this item from being used before vanilla mana upgrades are maxed out.
            // 这个检查防止这个物品在基础法力升级达到最高级之前被使用。
            return player.ConsumedManaCrystals == Player.ManaCrystalMax;
        }

        public override bool? UseItem(Player player)
        {
            // Moving the AnalysisManaCrystals check from CanUseItem to here allows this Analysis crystal to still "be used" like Mana Crystals can be
            // when at the max allowed, but it will just play the animation and not affect the player's max mana

            // 将AnalysisManaCrystals检查从CanUseItem移动到这里允许像Mana Crystals一样“使用”Analysis crystal，
            // 当已经达到最大限制时，但它只会播放动画而不影响玩家的最大法力
            if (player.GetModPlayer<AnalysisStatIncreasePlayer>().AnalysisManaCrystals >= MaxAnalysisManaCrystals)
            {
                // Returning null will make the item not be consumed
                // 返回null将使该物品不被消耗
                return null;
            }

            // This method handles permanently increasing the player's max mana and displaying the blue mana text
            // 此方法处理永久性地增加玩家的最大法力并显示蓝色魔法文本
            player.UseManaMaxIncreasingItem(ManaPerCrystal);

            // This field tracks how many of the Analysis crystals have been consumed
            // 此字段跟踪已经消耗了多少分析水晶
            player.GetModPlayer<AnalysisStatIncreasePlayer>().AnalysisManaCrystals++;

            return true;
        }

        // Please see Content/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AnalysisItem>()
                .AddTile<Tiles.Furniture.AnalysisWorkbench>()
                .Register();
        }
    }
}
