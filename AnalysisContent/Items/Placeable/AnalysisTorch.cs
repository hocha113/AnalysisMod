using AnalysisMod.AnalysisContent.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Placeable
{
    public class AnalysisTorch : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;

            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.ShimmerTorch;
            ItemID.Sets.SingleUseInGamepad[Type] = true;
            ItemID.Sets.Torches[Type] = true;
        }

        public override void SetDefaults()
        {
            // DefaultToTorch sets various properties common to torch placing items. Hover over DefaultToTorch in Visual Studio to see the specific properties set.
            // Of particular note to torches are Item.holdStyle, Item.flame, and Item.noWet. 

            // DefaultToTorch会设置火把物品的一些常见属性。在Visual Studio中悬停DefaultToTorch可以查看具体设置。
            // 特别注意火把的Item.holdStyle、Item.flame和Item.noWet属性。
            Item.DefaultToTorch(ModContent.TileType<Tiles.AnalysisTorch>(), 0, false);
            Item.value = 50;
        }

        public override void HoldItem(Player player)
        {
            // Note that due to biome select torch god's favour, the player may not actually have an AnalysisTorch in their inventory when this hook is called, so no modifications should be made to the item instance.
            // 请注意，由于生物群系选择了火把神的青睐，在调用此钩子时玩家可能并没有AnalysisTorch在他们的库存中，因此不应对物品实例进行修改。

            // Randomly spawn sparkles when the torch is held. Twice bigger chance to spawn them when swinging the torch.
            // 当手持火把时随机产生闪光。挥动火炬时产生它们的几率增加两倍。
            if (Main.rand.NextBool(player.itemAnimation > 0 ? 40 : 80))
            {
                Dust.NewDust(new Vector2(player.itemLocation.X + 16f * player.direction, player.itemLocation.Y - 14f * player.gravDir), 4, 4, ModContent.DustType<Sparkle>());
            }

            // Create a white (1.0, 1.0, 1.0) light at the torch's approximate position, when the item is held.
            // 当手持该物品时，在其大致位置创建一个白色（1.0, 1.0, 1.0）光源。
            Vector2 position = player.RotatedRelativePoint(new Vector2(player.itemLocation.X + 12f * player.direction + player.velocity.X, player.itemLocation.Y - 14f + player.velocity.Y), true);

            Lighting.AddLight(position, 1f, 1f, 1f);
        }

        public override void PostUpdate()
        {
            // Create a white (1.0, 1.0, 1.0) light when the item is in world, and isn't underwater.
            // 在世界上放置该物品且不处于水下时，创建一个白色（1.0, 1.0, 1.0）光源。
            if (!Item.wet)
            {
                Lighting.AddLight(Item.Center, 1f, 1f, 1f);
            }
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
