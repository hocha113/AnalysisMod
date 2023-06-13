using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Ammo
{
    // This Analysis class demonstrates how to make your own weapon ammo.
    // Used by AnalysisCustomAmmoGun

    // 这个 Analysis 类演示了如何制作自己的武器弹药。
    // 由 AnalysisCustomAmmoGun 使用
    public class AnalysisCustomAmmo : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
        {
            Item.width = 14; // The width of item hitbox
                             // 物品碰撞箱的宽度

            Item.height = 14; // The height of item hitbox
                              // 物品碰撞箱的高度

            Item.damage = 8; // The damage for projectiles isn't actually 8, it actually is the damage combined with the projectile and the item together
                             // 抛射物的伤害实际上不是8，而是将抛射物和物品组合在一起后的总伤害。

            Item.DamageType = DamageClass.Ranged; // What type of damage does this ammo affect?
                                                  // 这种弹药会造成什么类型的伤害？

            Item.maxStack = Item.CommonMaxStack; // The maximum number of items that can be contained within a single stack
                                                 // 单个堆叠中可以包含的最大数量。

            Item.consumable = true; // This marks the item as consumable, making it automatically be consumed when it's used as ammunition, or something else, if possible
                                    // 将该项标记为可消耗，使其在用作弹药或其他用途时自动被消耗掉（如果可能）。

            Item.knockBack = 2f; // Sets the item's knockback. Ammunition's knockback added together with weapon and projectiles.
                                 // 设置该项对应武器击退值。弹药、武器和抛射物击退值相加。

            Item.value = Item.sellPrice(0, 0, 1, 0); // Item price in copper coins (can be converted with Item.sellPrice/Item.buyPrice)
                                                     // 以铜币计算的商品价格（可以使用 Item.sellPrice/Item.buyPrice 进行转换）

            Item.rare = ItemRarityID.Yellow; // The color that the item's name will be in-game.
                                             // 该项目名称在游戏中显示时所呈现颜色。

            Item.shoot = ModContent.ProjectileType<Projectiles.AnalysisHomingProjectile>(); // The projectile that weapons fire when using this item as ammunition.
                                                                                            // 当使用此项作为弹药时，武器发射出来的抛射体。

            Item.ammo = Item.type; // Important. The first item in an ammo class sets the AmmoID to its type
                                   // 重要提示：一个 ammo 类中第一件 item 将 AmmoID 设置为其类型
        }

        // Please see AnalysisContent/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        // Here we create recipe for 999/AnalysisCustomAmmo stack from 1/AnalysisItem

        // 请参阅 AnalysisContent/AnalysisRecipes.cs 获取有关配方创建详细说明。
        // 在此我们从1/AnalysisItem创建999/AnalysisCustomAmmo堆叠配方 
        public override void AddRecipes()
        {
            CreateRecipe(999)
                .AddIngredient<AnalysisItem>()
                .AddTile<Tiles.Furniture.AnalysisWorkbench>()
                .Register();
        }
    }
}
