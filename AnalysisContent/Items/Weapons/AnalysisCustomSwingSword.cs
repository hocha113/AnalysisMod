using AnalysisMod.AnalysisContent.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Weapons
{
    // AnalysisCustomSwingSword is an Analysis of a sword with a custom swing using a held projectile
    // This is great if you want to make melee weapons with complex swing behaviour

    // AnalysisCustomSwingSword 是一种带有自定义挥舞和持有弹药的剑分析
    // 如果您想制作具有复杂摆动行为的近战武器，这非常棒
    public class AnalysisCustomSwingSword : ModItem
    {
        public int attackType = 0; // keeps track of which attack it is
                                   // 跟踪攻击类型

        public int comboExpireTimer = 0; // we want the attack pattern to reset if the weapon is not used for certain period of time
                                         // 我们希望在武器一段时间内未使用时重置攻击模式

        public override void SetDefaults()
        {
            // Common Properties
            // 常见属性
            Item.width = 46;
            Item.height = 48;
            Item.value = Item.sellPrice(gold: 2, silver: 50);
            Item.rare = ItemRarityID.Green;

            // Use Properties
            // 使用属性

            // Note that useTime and useAnimation for this item don't actually affect the behavior because the held projectile handles that. 
            // Each attack takes a different amount of time to execute
            // Conforming to the item useTime and useAnimation makes it much harder to design
            // It does, however, affect the item tooltip, so don't leave it out.

            // 请注意，此物品的 useTime 和 useAnimation 实际上不会影响其行为，因为持有弹药处理该问题。
            // 每次攻击需要执行不同数量的时间
            // 符合项目 useTime 和 useAnimation 使设计变得更加困难
            // 然而它确实影响了项目工具提示，所以不要忽略它。
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;

            // Weapon Properties
            // 武器属性
            Item.knockBack = 7;  // The knockback of your sword, this is dynamically adjusted in the projectile code.
                                 // 你的剑反冲力，在弹道代码中动态调整。

            Item.autoReuse = true; // This determines whether the weapon has autoswing
                                   // 这决定了武器是否可以自动挥舞。

            Item.damage = 62; // The damage of your sword, this is dynamically adjusted in the projectile code.
                              // 你的剑伤害，在弹道代码中动态调整。

            Item.DamageType = DamageClass.Melee; // Deals melee damage
                                                 // 造成近战伤害。

            Item.noMelee = true;  // This makes sure the item does not deal damage from the swinging animation
                                  // 这确保物品不会从摆动动画中造成伤害。

            Item.noUseGraphic = true; // This makes sure the item does not get shown when the player swings his hand
                                      // 这确保当玩家挥手时不显示物品。

            // Projectile Properties
            // 弹药属性
            Item.shoot = ModContent.ProjectileType<AnalysisCustomSwingProjectile>(); // The sword as a projectile
                                                                                     // 将剑作为一个弹药体现出来.
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Using the shoot function, we override the swing projectile to set ai[0] (which attack it is)
            // 使用 shoot 函数，我们覆盖 swing projectile 来设置 ai[0]（表示攻击类型）
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, Main.myPlayer, attackType);
            attackType = (attackType + 1) % 2; // Increment attackType to make sure next swing is different
                                               // 增加 attackType 以确保下一次挥动不同

            comboExpireTimer = 0; // Every time the weapon is used, we reset this so the combo does not expire
                                  // 每次使用武器时，我们都会重置它，以便连击不会过期。

            return false; // return false to prevent original projectile from being shot
                          // 返回 false 以防止发射原始弹药
        }

        public override void UpdateInventory(Player player)
        {
            if (comboExpireTimer++ >= 120) // after 120 ticks (== 2 seconds) in inventory, reset the attack pattern
                                           // 在库存中经过120个ticks（==2秒）后，重置攻击模式
                attackType = 0;
        }

        public override bool MeleePrefix()
        {
            return true; // return true to allow weapon to have melee prefixes (e.g. Legendary)
                         // 返回 true 允许武器具有近战前缀（例如传奇）
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AnalysisItem>()
                .AddTile<Tiles.Furniture.AnalysisWorkbench>()
                .Register();
        }
    }
}