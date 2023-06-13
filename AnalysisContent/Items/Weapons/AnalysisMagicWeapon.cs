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

            // DefaultToStaff 处理魔法权杖武器使用的各种物品值。
            // 在 Visual Studio 中悬停在 DefaultToStaff 上以阅读文档！
            // 发射黑色闪电，也称为从玛瑙爆破枪发射的抛射物。
            Item.DefaultToStaff(ProjectileID.BlackBolt, 7, 20, 11);
            Item.width = 34;
            Item.height = 40;
            Item.UseSound = SoundID.Item71;

            // A special method that sets the damage, knockback, and bonus critical strike chance.
            // This weapon has a crit of 32% which is added to the players default crit chance of 4%

            // 一个特殊的方法，设置伤害、击退和额外暴击几率。
            // 此武器具有32%的暴击率，加上玩家默认暴击几率4%。
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
