using AnalysisMod.AnalysisContent.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Weapons
{
    // Analysis Advanced Flail is a complete adaption of Ball O' Hurt. The Projectile has the complete code needed to customize all aspects of the flail. See AnalysisFlail for a simpler Analysis that is less customizable. 
    // �߼����������Ƕԡ�ʹ��֮�򡱵���ȫ�ıࡣ������������Զ������ϵ����з���������������롣�����Ҫ���򵥡����ɶ��ƻ��ķ�������μ�AnalysisFlail��
    public class AnalysisAdvancedFlail : ModItem
    {
        public override void SetStaticDefaults()
        {
            // This line will make the damage shown in the tooltip twice the actual Item.damage. This multiplier is used to adjust for the dynamic damage capabilities of the projectile.
            // When thrown directly at enemies, the flail projectile will deal double Item.damage, matching the tooltip, but deals normal damage in other modes.

            // ���н�ʹ������ʾ����ʾ���˺�ֵ�ӱ����Ե��������ﶯ̬�˺�������ɵ�Ӱ�졣
            // ��ֱ��Ͷ�����ʱ����������������˫��Item.damage���빤����ʾ��ƥ�䣩����������ģʽ��ֻ�������ͨ�˺���
            ItemID.Sets.ToolTipDamageMultiplier[Type] = 2f;
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Shoot; // How you use the item (swinging, holding out, etc.)
                                                  // ʹ�õ���ʱ���û��������ȷ�ʽ��

            Item.useAnimation = 45; // The item's use time in ticks (60 ticks == 1 second.)
                                    // ����ʹ��ʱ����tickΪ��λ��60 tick == 1�룩��

            Item.useTime = 45; // The item's use time in ticks (60 ticks == 1 second.)
                               // ��Ʒʹ��ʱ�䣬��tickΪ��λ��60 tick����1�룩��

            Item.knockBack = 5.5f; // The knockback of your flail, this is dynamically adjusted in the projectile code.
                                   // ������ϻ���Ч�����ڵ��������ж�̬������

            Item.width = 32; // Hitbox width of the item.
                             // ���ߴ������

            Item.height = 32; // Hitbox height of the item.
                              // ���ߴ����߶�

            Item.damage = 15; // The damage of your flail, this is dynamically adjusted in the projectile code.
                              // ��������˺���������������н��ж�̬������

            Item.noUseGraphic = true; // This makes sure the item does not get shown when the player swings his hand
                                      // ȷ����һ���ʱ����ʾ����

            Item.shoot = ModContent.ProjectileType<AnalysisAdvancedFlailProjectile>(); // The flail projectile
                                                                                       // ����������

            Item.shootSpeed = 12f; // The speed of the projectile measured in pixels per frame.
                                   // �䵯���ٶȣ���ÿ֡���ؼ��㡣

            Item.UseSound = SoundID.Item1; // The sound that this item makes when used
                                           // ʹ�ø���ʱ����������

            Item.rare = ItemRarityID.Green; // The color of the name of your item
                                            // ����Ŀ������ɫ

            Item.value = Item.sellPrice(gold: 1, silver: 50); // Sells for 1 gold 50 silver
                                                              // �ۼ�1��50��

            Item.DamageType = DamageClass.MeleeNoSpeed; // Deals melee damage
                                                        // ��ս����
            Item.channel = true;
            Item.noMelee = true; // This makes sure the item does not deal damage from the swinging animation
                                 // ��ȷ����Ʒ����ӻӶ�����������˺���
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