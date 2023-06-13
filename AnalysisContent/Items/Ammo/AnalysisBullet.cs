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
                              // 弹药的伤害值并不是12，实际上是弹药和物品本身造成的伤害值之和。

            Item.DamageType = DamageClass.Ranged;
            Item.width = 8;
            Item.height = 8;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true; // This marks the item as consumable, making it automatically be consumed when it's used as ammunition, or something else, if possible.
                                    // 这会将该物品标记为消耗品，使其在用作弹药或其他可能性时自动被消耗。

            Item.knockBack = 1.5f;
            Item.value = 10;
            Item.rare = ItemRarityID.Green;
            Item.shoot = ModContent.ProjectileType<Projectiles.AnalysisBullet>(); // The projectile that weapons fire when using this item as ammunition.
                                                                                  // 武器使用此物品作为弹药发射的抛射体。

            Item.shootSpeed = 16f; // The speed of the projectile.
                                   // 抛射体速度。

            Item.ammo = AmmoID.Bullet; // The ammo class this ammo belongs to.
                                       // 此弹药所属的类型。
        }

        // Please see AnalysisContent/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        // 有关配方创建的详细说明，请参见AnalysisContent/AnalysisRecipes.cs。
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AnalysisItem>()
                .AddTile<AnalysisWorkbench>()
                .Register();
        }
    }
}
