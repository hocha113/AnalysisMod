using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Weapons
{
    public class AnalysisJavelin : ModItem
    {
        public override void SetDefaults()
        {
            // Alter any of these values as you see fit, but you should probably keep useStyle on 1, as well as the noUseGraphic and noMelee bools
            // 根据需要更改这些值，但最好将useStyle保持为1，并且noUseGraphic和noMelee布尔值也是如此。

            // Common Properties
            // 常见属性
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(silver: 5);
            Item.maxStack = 999;

            // Use Properties
            // 使用属性
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 25;
            Item.useTime = 25;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.consumable = true;

            // Weapon Properties
            // 武器属性			
            Item.damage = 33;
            Item.knockBack = 5f;
            Item.noUseGraphic = true; // The item should not be visible when used
                                      // 使用该物品时不应可见

            Item.noMelee = true; // The projectile will do the damage and not the item
                                 // 弹射物会造成伤害而非物品本身

            Item.DamageType = DamageClass.Ranged;

            // Projectile Properties
            // 弹射物属性
            Item.shootSpeed = 12f;
            Item.shoot = ModContent.ProjectileType<Projectiles.AnalysisJavelinProjectile>(); // The projectile that will be thrown
                                                                                             // 将被投掷的弹射物
        }

        // Please see AnalysisContent/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe(20)
                .AddIngredient<AnalysisItem>()
                .AddTile<Tiles.Furniture.AnalysisWorkbench>()
                .Register();
        }
    }
}