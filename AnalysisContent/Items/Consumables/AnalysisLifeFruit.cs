using AnalysisMod.AnalysisCommon.Players;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Consumables
{
    // This file showcases how to create an item that increases the player's maximum health on use.
    // Within your ModPlayer, you need to save/load a count of usages. You also need to sync the data to other players.
    // The overlay used to display the custom life fruit can be found in Common/UI/ResourceDisplay/VanillaLifeOverlay.cs

    // 这个文件展示了如何创建一个使用后增加玩家最大生命值的物品。
    // 在你的 ModPlayer 中，你需要保存/加载使用次数计数。你还需要将数据同步到其他玩家。
    // 用于显示自定义生命果实的覆盖层可以在 AnalysisCommon/UI/ResourceDisplay/VanillaLifeOverlay.cs 中找到。
    internal class AnalysisLifeFruit : ModItem
    {
        public static readonly int MaxAnalysisLifeFruits = 10;
        public static readonly int LifePerFruit = 10;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(LifePerFruit, MaxAnalysisLifeFruits);

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 10;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.LifeFruit);
        }

        public override bool CanUseItem(Player player)
        {
            // This check prevents this item from being used before vanilla health upgrades are maxed out.
            // 这个检查防止这个物品在基础健康升级达到最大之前被使用。
            return player.ConsumedLifeCrystals == Player.LifeCrystalMax && player.ConsumedLifeFruit == Player.LifeFruitMax;
        }

        public override bool? UseItem(Player player)
        {
            // Moving the AnalysisLifeFruits check from CanUseItem to here allows this Analysis fruit to still "be used" like Life Fruit can be
            // when at the max allowed, but it will just play the animation and not affect the player's max life

            // 将 AnalysisLifeFruits 检查从 CanUseItem 移动到此处允许像 Life Fruit 一样“使用”Analysis fruit，
            // 当已经达到最大时，但它只会播放动画而不影响玩家的最大生命
            if (player.GetModPlayer<AnalysisStatIncreasePlayer>().AnalysisLifeFruits >= MaxAnalysisLifeFruits)
            {
                // Returning null will make the item not be consumed
                // 返回 null 将使该物品不被消耗
                return null;
            }

            // This method handles permanently increasing the player's max health and displaying the green heal text
            // 此方法处理永久性地增加玩家的最大生命和显示绿色治愈文本
            player.UseHealthMaxIncreasingItem(LifePerFruit);

            // This field tracks how many of the Analysis fruit have been consumed
            // 此字段跟踪已经消耗了多少 Analysis fruit
            player.GetModPlayer<AnalysisStatIncreasePlayer>().AnalysisLifeFruits++;

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
