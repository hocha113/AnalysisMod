using AnalysisMod.AnalysisContent.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Placeable
{
    // AnalysisWaterTorch is very similar to AnalysisTorch, except it can be used and placed underwater, similar to Coral Torch.
    // The comments in this file will focus on the differences.
    // Both place the same tile, but a different tile style. The AnalysisWaterTorch tile style has custom code seen in the AnalysisTorch ModTile.

    // AnalysisWaterTorch与AnalysisTorch非常相似，只是它可以在水下使用和放置，类似于珊瑚火把。
    // 本文件中的注释将重点介绍区别。
    // 它们都放置相同的图块，但不同的图块样式。AnalysisWaterTorch图块样式具有在AnalysisTorch ModTile中看到的自定义代码。
    public class AnalysisWaterTorch : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;

            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.ShimmerTorch;
            ItemID.Sets.SingleUseInGamepad[Type] = true;
            ItemID.Sets.Torches[Type] = true;
            ItemID.Sets.WaterTorches[Type] = true; // The TileObjectData.newSubTile code in the AnalysisTorch ModTile is required as well to make a water torch.
                                                   // 在AnalysisTorch ModTile中的TileObjectData.newSubTile代码也需要用来制作水火把。
        }

        public override void SetDefaults()
        {
            // Instead of placing style 0, style 1 is placed. The allowWaterPlacement parameter is true, which will set Item.noWet to false, allowing the item to be held underwater.
            // 不是放置样式0，而是放置样式1。allowWaterPlacement参数为true，则会将Item.noWet设置为false，允许物品在水下持有。
            Item.DefaultToTorch(ModContent.TileType<Tiles.AnalysisTorch>(), 1, true);
            Item.value = 50;
        }

        public override void HoldItem(Player player)
        {
            if (Main.rand.NextBool(player.itemAnimation > 0 ? 40 : 80))
            {
                Dust.NewDust(new Vector2(player.itemLocation.X + 16f * player.direction, player.itemLocation.Y - 14f * player.gravDir), 4, 4, ModContent.DustType<Sparkle>());
            }

            // Create a greenish (0.5, 1.5, 0.5) light at the torch's approximate position, when the item is held.
            // 当持有物品时，在火把大致位置创建一个绿色（0.5、1.5、0.5）光线。
            Vector2 position = player.RotatedRelativePoint(new Vector2(player.itemLocation.X + 12f * player.direction + player.velocity.X, player.itemLocation.Y - 14f + player.velocity.Y), true);

            Lighting.AddLight(position, 0.5f, 1.5f, 0.5f);
        }

        public override void PostUpdate()
        {
            // Create a greenish (0.5, 1.5, 0.5) light when the item is in world, even if underwater.
            // 即使在水下，在世界上存在物品时也会创建一个绿色（0.5、1.5、0.5）光线。
            Lighting.AddLight(Item.Center, 0.5f, 1.5f, 0.5f);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AnalysisTorch>()
                .AddIngredient(ItemID.Gel)
                .Register();
        }
    }
}
