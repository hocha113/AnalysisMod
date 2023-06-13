using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Weapons
{
    public class AnalysisMagicWeapon : ModItem
    {
        public override void SetDefaults()
        {
            // DefaultToStaff handles setting various Item values that magic staff weapons use.
            // Hover over DefaultToStaff in Visual Studio to read the documentation!
            // Shoot a black bolt, also known as the projectile shot from the onyx blaster.

            // DefaultToStaff ����ħ��Ȩ������ʹ�õĸ�����Ʒֵ��
            // �� Visual Studio ����ͣ�� DefaultToStaff �����Ķ��ĵ���
            // �����ɫ���磬Ҳ��Ϊ����觱���ǹ����������
            Item.DefaultToStaff(ProjectileID.BlackBolt, 7, 20, 11);
            Item.width = 34;
            Item.height = 40;
            Item.UseSound = SoundID.Item71;

            // A special method that sets the damage, knockback, and bonus critical strike chance.
            // This weapon has a crit of 32% which is added to the players default crit chance of 4%

            // һ������ķ����������˺������˺Ͷ��Ⱪ�����ʡ�
            // ����������32%�ı����ʣ��������Ĭ�ϱ�������4%��
            Item.SetWeaponValues(25, 6, 32);

            Item.SetShopValues(ItemRarityColor.LightRed4, 10000);
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
