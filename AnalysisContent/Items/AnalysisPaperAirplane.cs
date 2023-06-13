using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items
{
    public class AnalysisPaperAirplane : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22; // The item texture's width
            Item.height = 16; // The item texture's height

            Item.value = Item.sellPrice(0, 0, 10); // The value of the item. In this case, 10 silver. Item.buyPrice & Item.sellPrice are helper methods that returns costs in copper coins based on platinum/gold/silver/copper arguments provided to it.
                                                   // ��Ʒ�ļ�ֵ������������£�Ϊ10���ҡ�Item.buyPrice��Item.sellPrice�Ǹ��������������ṩ�İ׽�/�ƽ�/����/ͭ������ͭ����ʽ���سɱ���

            Item.DefaultToThrownWeapon(ModContent.ProjectileType<Projectiles.AnalysisPaperAirplaneProjectile>(), 17, 5f); // A special method that sets a variety of item parameters that make the item act like a throwing weapon.
                                                                                                                          // һ������ķ��������˸�����Ŀ������ʹ����Ŀ��Ͷ������һ��������

            // The above Item.DefaultToThrownWeapon() //does the following. Uncomment these if you don't want to use the above method or want to change something about it.
            //                                        // ִ�����²��������������ʹ�����������������ĳЩ���ݣ���ȡ���������ע�͡�
            // Item.autoReuse = false;
            // Item.useStyle = ItemUseStyleID.Swing;
            // Item.useAnimation = 17;
            // Item.useTime = 17;
            // Item.shoot = ModContent.ProjectileType<Projectiles.AnalysisPaperAirplaneProjectile>();
            // Item.shootSpeed = 5f;
            // Item.noMelee = true;
            // Item.DamageType = DamageClass.Ranged;
            // Item.consumable = true;
            // Item.maxStack = Item.CommonMaxStack;

            Item.SetWeaponValues(4, 2f); // A special method that sets the damage, knockback, and bonus critical strike chance.
                                         // һ������ķ��������˺������˺Ͷ��Ⱪ�����ʡ�

            // The above Item.SetWeaponValues() // does the following. Uncomment these if you don't want to use the above method.
            //                                  // ִ�����²��������������ʹ��������������ȡ���������ע�͡�
            // Item.damage = 4;
            // Item.knockBack = 2;
            // Item.crit = 0; // Even though this says 0, this is more like "bonus critical strike chance". All weapons have a base critical strike chance of 4.
            //                //���ܴ˴���ʾΪ0���������ǡ����Ⱪ�����ʡ���������������4�������������ʡ�
        }

        // Please see Content/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe(10)
                .AddIngredient(ModContent.ItemType<AnalysisItem>())
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
