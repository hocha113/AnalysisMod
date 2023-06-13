using AnalysisMod.AnalysisContent.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Tools
{
    public class AnalysisHamaxe : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 25;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true; // Automatically re-swing/re-use this item after its swinging animation is over.
                                   // �������Ӷ������������Զ����»���/ʹ�ô���Ʒ��

            Item.axe = 30; // How much axe power the weapon has, note that the axe power displayed in-game is this value multiplied by 5
                           // ע�⣬��Ϸ����ʾ�ĸ�ͷ�����Ǹ�ֵ����5��

            Item.hammer = 100; // How much hammer power the weapon has
                               // �������ж��ٴ���������
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(10))
            { // This creates a 1/10 chance that a dust will spawn every frame that this item is in its 'Swinging' animation.
              // Creates a dust at the hitbox rectangle, following the rules of our 'if' conditional.

              // �⽫����һ��1/10�ļ��ʣ��ڴ���Ʒ���ڡ��ڶ�������ʱÿ֡����һ�γ�����
              // �����п���δ�����һ���ҳ�����ѭ���ǵġ�if����������
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, ModContent.DustType<Sparkle>());
            }
        }

        // Please see AnalysisContent/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AnalysisItem>()
                .AddTile<Tiles.Furniture.AnalysisWorkbench>()
                .Register();
        }
    }
}
