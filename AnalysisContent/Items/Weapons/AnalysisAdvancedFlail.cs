using AnalysisMod.AnalysisContent.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Weapons
{
    // Analysis Advanced Flail is a complete adaption of Ball O' Hurt. The Projectile has the complete code needed to customize all aspects of the flail. See AnalysisFlail for a simpler Analysis that is less customizable. 
    // 高级分析链球是对“痛苦之球”的完全改编。该抛射物具有自定义链枷的所有方面所需的完整代码。如果需要更简单、不可定制化的分析，请参见AnalysisFlail。
    public class AnalysisAdvancedFlail : ModItem
    {
        public override void SetStaticDefaults()
        {
            // This line will make the damage shown in the tooltip twice the actual Item.damage. This multiplier is used to adjust for the dynamic damage capabilities of the projectile.
            // When thrown directly at enemies, the flail projectile will deal double Item.damage, matching the tooltip, but deals normal damage in other modes.

            // 此行将使工具提示中显示的伤害值加倍，以调整抛射物动态伤害能力造成的影响。
            // 当直接投向敌人时，链球抛射物会造成双倍Item.damage（与工具提示相匹配），但在其他模式下只会造成普通伤害。
            ItemID.Sets.ToolTipDamageMultiplier[Type] = 2f;
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Shoot; // How you use the item (swinging, holding out, etc.)
                                                  // 使用道具时采用挥舞或持续等方式。

            Item.useAnimation = 45; // The item's use time in ticks (60 ticks == 1 second.)
                                    // 道具使用时间以tick为单位（60 tick == 1秒）。

            Item.useTime = 45; // The item's use time in ticks (60 ticks == 1 second.)
                               // 物品使用时间，以tick为单位（60 tick等于1秒）。

            Item.knockBack = 5.5f; // The knockback of your flail, this is dynamically adjusted in the projectile code.
                                   // 你的连枷击退效果会在弹道代码中动态调整。

            Item.width = 32; // Hitbox width of the item.
                             // 道具打击框宽度

            Item.height = 32; // Hitbox height of the item.
                              // 道具打击框高度

            Item.damage = 15; // The damage of your flail, this is dynamically adjusted in the projectile code.
                              // 你的链枷伤害，在抛射物代码中进行动态调整。

            Item.noUseGraphic = true; // This makes sure the item does not get shown when the player swings his hand
                                      // 确保玩家挥手时不显示该项

            Item.shoot = ModContent.ProjectileType<AnalysisAdvancedFlailProjectile>(); // The flail projectile
                                                                                       // 链球抛射物

            Item.shootSpeed = 12f; // The speed of the projectile measured in pixels per frame.
                                   // 射弹的速度，以每帧像素计算。

            Item.UseSound = SoundID.Item1; // The sound that this item makes when used
                                           // 使用该项时发出的声音

            Item.rare = ItemRarityID.Green; // The color of the name of your item
                                            // 您项目名称颜色

            Item.value = Item.sellPrice(gold: 1, silver: 50); // Sells for 1 gold 50 silver
                                                              // 售价1金50银

            Item.DamageType = DamageClass.MeleeNoSpeed; // Deals melee damage
                                                        // 近战攻击
            Item.channel = true;
            Item.noMelee = true; // This makes sure the item does not deal damage from the swinging animation
                                 // 这确保物品不会从挥动动画中造成伤害。
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