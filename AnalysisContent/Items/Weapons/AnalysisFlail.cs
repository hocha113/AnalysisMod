using AnalysisMod.AnalysisContent.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Weapons
{
    // AnalysisFlail and AnalysisFlailProjectile show the minimum amount of code needed for a flail using the existing vanilla code and behavior.
    // AnalysisAdvancedFlail and AnalysisAdvancedFlailProjectile need to be consulted if more advanced customization is desired, or if you want to learn more advanced modding techniques.
    // AnalysisFlail is a copy of the Sunfury flail weapon.

    // AnalysisFlail和AnalysisFlailProjectile展示了使用现有的基础代码和行为所需的最少量代码。如果需要更高级的自定义，或者想要学习更高级的modding技术，
    // 则需要参考AnalysisAdvancedFlail和AnalysisAdvancedFlailProjectile。
    // AnalysisFlail是Sunfury连枷武器的副本。
    internal class AnalysisFlail : ModItem
    {
        public override void SetStaticDefaults()
        {
            // This line will make the damage shown in the tooltip twice the actual Item.damage. This multiplier is used to adjust for the dynamic damage capabilities of the projectile.
            // When thrown directly at enemies, the flail projectile will deal double Item.damage, matching the tooltip, but deals normal damage in other modes.

            // 这一行将使工具提示中显示的伤害值加倍。这个乘数用于调整弹丸动态伤害能力。
            // 直接投向敌人时，连枷弹射物会造成双倍Item.damage，与工具提示相匹配，但在其他模式下则造成普通伤害。
            ItemID.Sets.ToolTipDamageMultiplier[Type] = 2f;
        }

        public override void SetDefaults()
        {
            // These default values aside from Item.shoot match the Sunfury values, feel free to tweak them.
            // 除了Item.shoot之外，默认值与Sunfury值相同，请随意进行调整。

            Item.useStyle = ItemUseStyleID.Shoot; // How you use the item (swinging, holding out, etc.)
                                                  // 如何使用该物品（挥舞、握住等）

            Item.useAnimation = 45; // The item's use time in ticks (60 ticks == 1 second.)
                                    // 该物品使用时间以tick为单位（60 tick == 1秒）。

            Item.useTime = 45; // The item's use time in ticks (60 ticks == 1 second.)
                               // 该物品使用时间以tick为单位（60 tick == 1秒）。

            Item.knockBack = 6.75f; // The knockback of your flail, this is dynamically adjusted in the projectile code.
                                    // 你连枷击退敌人时产生的后坐力，在弹丸代码中动态调整。

            Item.width = 30; // Hitbox width of the item.
            Item.height = 10; // Hitbox height of the item.
            Item.damage = 32; // The damage of your flail, this is dynamically adjusted in the projectile code.
                              // 你连枷造成的伤害，在弹丸代码中动态调整。

            Item.crit = 7; // Critical damage chance %
                           // 暴击几率 单位：％

            Item.scale = 1.1f;
            Item.noUseGraphic = true; // This makes sure the item does not get shown when the player swings his hand
                                      // 这确保当玩家挥手时不会显示该物品

            Item.shoot = ModContent.ProjectileType<AnalysisFlailProjectile>(); // The flail projectile
                                                                               // 连枷发射的弹幕

            Item.shootSpeed = 12f; // The speed of the projectile measured in pixels per frame.
                                   // 以像素每帧测量的弹幕速度。

            Item.UseSound = SoundID.Item1; // The sound that this item makes when used
                                           // 使用时发出的声音

            Item.rare = ItemRarityID.Orange; // The color of the name of your item
                                             // 您的项目名称的颜色

            Item.value = Item.sellPrice(gold: 2, silver: 50); // Sells for 2 gold 50 silver
                                                              // 以2金50银的价格出售

            Item.DamageType = DamageClass.MeleeNoSpeed; // Deals melee damage
                                                        // 造成近战伤害
            Item.channel = true;
            Item.noMelee = true; // This makes sure the item does not deal damage from the swinging animation
                                 // 这确保该物品不会从挥舞动画中造成伤害。
        }

        public override Color? GetAlpha(Color lightColor)
        {
            // Aside from SetDefaults, when making a copy of a vanilla weapon you may have to hunt down other bits of code. This code makes the item draw in full brightness when dropped.
            // 除了SetDefaults之外，当制作一个基础武器的副本时，您可能需要寻找其他代码。此代码使项目在掉落时以完全亮度绘制。
            return Color.White;
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
