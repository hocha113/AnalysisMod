using AnalysisMod.AnalysisContent.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Ammo
{
    public class AnalysisBullet : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
        {
            Item.damage = 12; // The damage for projectiles isn't actually 12, it actually is the damage combined with the projectile and the item together.
                              // ��ҩ���˺�ֵ������12��ʵ�����ǵ�ҩ����Ʒ������ɵ��˺�ֵ֮�͡�

            Item.DamageType = DamageClass.Ranged;
            Item.width = 8;
            Item.height = 8;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true; // This marks the item as consumable, making it automatically be consumed when it's used as ammunition, or something else, if possible.
                                    // ��Ὣ����Ʒ���Ϊ����Ʒ��ʹ����������ҩ������������ʱ�Զ������ġ�

            Item.knockBack = 1.5f;
            Item.value = 10;
            Item.rare = ItemRarityID.Green;
            Item.shoot = ModContent.ProjectileType<Projectiles.AnalysisBullet>(); // The projectile that weapons fire when using this item as ammunition.
                                                                                  // ����ʹ�ô���Ʒ��Ϊ��ҩ����������塣

            Item.shootSpeed = 16f; // The speed of the projectile.
                                   // �������ٶȡ�

            Item.ammo = AmmoID.Bullet; // The ammo class this ammo belongs to.
                                       // �˵�ҩ���������͡�
        }

        // Please see AnalysisContent/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        // �й��䷽��������ϸ˵������μ�AnalysisContent/AnalysisRecipes.cs��
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AnalysisItem>()
                .AddTile<AnalysisWorkbench>()
                .Register();
        }
    }
}
