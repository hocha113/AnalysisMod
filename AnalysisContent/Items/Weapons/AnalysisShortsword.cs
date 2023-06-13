using AnalysisMod.AnalysisContent.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Weapons
{
    public class AnalysisShortsword : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 8;
            Item.knockBack = 4f;
            Item.useStyle = ItemUseStyleID.Rapier; // Makes the player do the proper arm motion
                                                   // �����������ȷ���ֱ۶���
            Item.useAnimation = 12;
            Item.useTime = 12;
            Item.width = 32;
            Item.height = 32;
            Item.UseSound = SoundID.Item1;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.autoReuse = false;
            Item.noUseGraphic = true; // The sword is actually a "projectile", so the item should not be visible when used
                                      // ��ʵ������һ�֡�Ͷ����������ʹ��ʱ��Ӧ�ÿɼ�����Ʒ

            Item.noMelee = true; // The projectile will do the damage and not the item
                                 // �˺���Ͷ������ɣ�����������Ʒ���

            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(0, 0, 0, 10);

            Item.shoot = ModContent.ProjectileType<AnalysisShortswordProjectile>(); // The projectile is what makes a shortsword work
                                                                                    // Ͷ�����Ƕ̽��������õĹؼ�

            Item.shootSpeed = 2.1f; // This value bleeds into the behavior of the projectile as velocity, keep that in mind when tweaking values
                                    // ���ֵ��Ӱ�쵽Ͷ������ٶ���Ϊ��������ֵʱ��ע��
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
