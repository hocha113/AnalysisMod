using AnalysisMod.AnalysisContent.Items.Ammo;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Weapons
{
    // This is an Analysis showing how to create a weapon that fires custom ammunition
    // The most important property is "Item.useAmmo". It tells you which item to use as ammo.
    // You can see the description of other parameters in the AnalysisGun class and at https://github.com/tModLoader/tModLoader/wiki/Item-Class-Documentation

    // 这是一份分析，展示如何创建一个可以发射自定义弹药的武器。
    // 最重要的属性是 "Item.useAmmo"。它告诉你使用哪个物品作为弹药。
    // 你可以在 AnalysisGun 类和 https://github.com/tModLoader/tModLoader/wiki/Item-Class-Documentation 上看到其他参数的描述。
    public class AnalysisCustomAmmoGun : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 42; // The width of item hitbox
                             // 物品 hitbox 的宽度

            Item.height = 30; // The height of item hitbox
                              // 物品 hitbox 的高度

            Item.autoReuse = true;  // Whether or not you can hold click to automatically use it again.
                                    // 是否能够长按来自动再次使用该武器。

            Item.damage = 12; // Sets the item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
                              // 设置该物品造成的伤害。请注意，由此武器射出的抛射体将会使用其本身和所用弹药的伤害之和。

            Item.DamageType = DamageClass.Ranged; // What type of damage does this item affect?
                                                  // 这个物品影响什么类型的伤害？

            Item.knockBack = 4f; // Sets the item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
                                 // 设置该物品造成击退效果。请注意，由此武器射出的抛射体将会使用其本身和所用弹药的击退效果之和。

            Item.noMelee = true; // So the item's animation doesn't do damage.
                                 // 因此，该物品动画不会造成伤害。

            Item.rare = ItemRarityID.Yellow; // The color that the item's name will be in-game.
                                             // 在游戏中显示该物品名称时所呈现颜色。

            Item.shootSpeed = 10f; // The speed of the projectile (measured in pixels per frame.)
                                   // 射弹速度（以像素每帧计算）。

            Item.useAnimation = 35; // The length of the item's use animation in ticks (60 ticks == 1 second.)
                                    // 持续时间（以 tick 计算）表示该道具被使用时需要多少时间才能完成。（60 ticks == 1 秒）

            Item.useTime = 35; // The item's use time in ticks (60 ticks == 1 second.)
                               // 持续时间（以 tick 计算）表示两次连续攻击之间需要等待多久。（60 ticks == 1 秒）

            Item.UseSound = SoundID.Item11; // The sound that this item plays when used.
                                            // 当使用时播放声音效果。

            Item.useStyle = ItemUseStyleID.Shoot; // How you use the item (swinging, holding out, shoot, etc.)
                                                  // 如何使用该物品（挥舞、持续展示、射击等）。

            Item.value = Item.buyPrice(gold: 1); // The value of the weapon in copper coins
                                                 // 以铜币计算的武器价值

            // Custom ammo and shooting homing projectiles
            // 自定义弹药和发射追踪抛射体
            Item.shoot = ModContent.ProjectileType<Projectiles.AnalysisHomingProjectile>();
            Item.useAmmo = ModContent.ItemType<AnalysisCustomAmmo>(); // Restrict the type of ammo the weapon can use, so that the weapon cannot use other ammos
                                                                      // 限制武器可以使用的弹药类型，使其无法使用其他类型的弹药。
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
