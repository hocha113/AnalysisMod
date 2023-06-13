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
                                                   // 物品的价值。在这种情况下，为10银币。Item.buyPrice和Item.sellPrice是辅助方法，根据提供的白金/黄金/白银/铜参数以铜币形式返回成本。

            Item.DefaultToThrownWeapon(ModContent.ProjectileType<Projectiles.AnalysisPaperAirplaneProjectile>(), 17, 5f); // A special method that sets a variety of item parameters that make the item act like a throwing weapon.
                                                                                                                          // 一个特殊的方法设置了各种项目参数，使该项目像投掷武器一样运作。

            // The above Item.DefaultToThrownWeapon() //does the following. Uncomment these if you don't want to use the above method or want to change something about it.
            //                                        // 执行以下操作。如果您不想使用上述方法或想更改某些内容，请取消对其进行注释。
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
                                         // 一个特殊的方法设置伤害、击退和额外暴击几率。

            // The above Item.SetWeaponValues() // does the following. Uncomment these if you don't want to use the above method.
            //                                  // 执行以下操作。如果您不想使用上述方法，请取消对其进行注释。
            // Item.damage = 4;
            // Item.knockBack = 2;
            // Item.crit = 0; // Even though this says 0, this is more like "bonus critical strike chance". All weapons have a base critical strike chance of 4.
            //                //尽管此处显示为0，但更像是“额外暴击几率”。所有武器都有4个基础暴击几率。
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
