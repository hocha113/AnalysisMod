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

            // DefaultToTorch�����û����Ʒ��һЩ�������ԡ���Visual Studio����ͣDefaultToTorch���Բ鿴�������á�
            // �ر�ע���ѵ�Item.holdStyle��Item.flame��Item.noWet���ԡ�
            Item.DefaultToTorch(ModContent.TileType<Tiles.AnalysisTorch>(), 0, false);
            Item.value = 50;
        }

        public override void HoldItem(Player player)
        {
            // Note that due to biome select torch god's favour, the player may not actually have an AnalysisTorch in their inventory when this hook is called, so no modifications should be made to the item instance.
            // ��ע�⣬��������Ⱥϵѡ���˻������������ڵ��ô˹���ʱ��ҿ��ܲ�û��AnalysisTorch�����ǵĿ���У���˲�Ӧ����Ʒʵ�������޸ġ�

            // Randomly spawn sparkles when the torch is held. Twice bigger chance to spawn them when swinging the torch.
            // ���ֳֻ��ʱ����������⡣�Ӷ����ʱ�������ǵļ�������������
            if (Main.rand.NextBool(player.itemAnimation > 0 ? 40 : 80))
            {
                Dust.NewDust(new Vector2(player.itemLocation.X + 16f * player.direction, player.itemLocation.Y - 14f * player.gravDir), 4, 4, ModContent.DustType<Sparkle>());
            }

            // Create a white (1.0, 1.0, 1.0) light at the torch's approximate position, when the item is held.
            // ���ֳָ���Ʒʱ���������λ�ô���һ����ɫ��1.0, 1.0, 1.0����Դ��
            Vector2 position = player.RotatedRelativePoint(new Vector2(player.itemLocation.X + 12f * player.direction + player.velocity.X, player.itemLocation.Y - 14f + player.velocity.Y), true);

            Lighting.AddLight(position, 1f, 1f, 1f);
        }

        public override void PostUpdate()
        {
            // Create a white (1.0, 1.0, 1.0) light when the item is in world, and isn't underwater.
            // �������Ϸ��ø���Ʒ�Ҳ�����ˮ��ʱ������һ����ɫ��1.0, 1.0, 1.0����Դ��
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
