using AnalysisMod.AnalysisContent.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Tools
{
    public class AnalysisPickaxe : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = Item.buyPrice(gold: 1); // Buy this item for one gold - change gold to any coin and change the value to any number <= 100
                                                 // ��һö��ҹ������Ʒ - ����Ҹ���Ϊ�κ�Ӳ�ң�������ֵ����Ϊ<= 100���κ�����
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            Item.pick = 220; // How strong the pickaxe is, see https://terraria.wiki.gg/wiki/Pickaxe_power for a list of common values
                             // �йظ�ͷ��ǿ�ȣ���μ�https://terraria.wiki.gg/wiki/Pickaxe_power�Ի�ȡ����ֵ�б�
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(10))
            {
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, ModContent.DustType<AnalysisCustomDrawDust>());
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
