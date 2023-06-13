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

    // AnalysisWaterTorch��AnalysisTorch�ǳ����ƣ�ֻ����������ˮ��ʹ�úͷ��ã�������ɺ����ѡ�
    // ���ļ��е�ע�ͽ��ص��������
    // ���Ƕ�������ͬ��ͼ�飬����ͬ��ͼ����ʽ��AnalysisWaterTorchͼ����ʽ������AnalysisTorch ModTile�п������Զ�����롣
    public class AnalysisWaterTorch : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;

            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.ShimmerTorch;
            ItemID.Sets.SingleUseInGamepad[Type] = true;
            ItemID.Sets.Torches[Type] = true;
            ItemID.Sets.WaterTorches[Type] = true; // The TileObjectData.newSubTile code in the AnalysisTorch ModTile is required as well to make a water torch.
                                                   // ��AnalysisTorch ModTile�е�TileObjectData.newSubTile����Ҳ��Ҫ��������ˮ��ѡ�
        }

        public override void SetDefaults()
        {
            // Instead of placing style 0, style 1 is placed. The allowWaterPlacement parameter is true, which will set Item.noWet to false, allowing the item to be held underwater.
            // ���Ƿ�����ʽ0�����Ƿ�����ʽ1��allowWaterPlacement����Ϊtrue����ὫItem.noWet����Ϊfalse��������Ʒ��ˮ�³��С�
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
            // ��������Ʒʱ���ڻ�Ѵ���λ�ô���һ����ɫ��0.5��1.5��0.5�����ߡ�
            Vector2 position = player.RotatedRelativePoint(new Vector2(player.itemLocation.X + 12f * player.direction + player.velocity.X, player.itemLocation.Y - 14f + player.velocity.Y), true);

            Lighting.AddLight(position, 0.5f, 1.5f, 0.5f);
        }

        public override void PostUpdate()
        {
            // Create a greenish (0.5, 1.5, 0.5) light when the item is in world, even if underwater.
            // ��ʹ��ˮ�£��������ϴ�����ƷʱҲ�ᴴ��һ����ɫ��0.5��1.5��0.5�����ߡ�
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
