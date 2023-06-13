using AnalysisMod.AnalysisContent.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Tools
{
    // AnalysisDrill closely mimics Titanium Drill, except where noted.
    // Of note, this Analysis showcases Item.tileBoost and teaches the basic concepts of a held projectile.

    // AnalysisDrill与Titanium Drill非常相似，除非另有说明。
    // 值得注意的是，此分析展示了Item.tileBoost并教授了持有弹丸的基本概念。
    public class AnalysisDrill : ModItem
    {
        public override void SetStaticDefaults()
        {
            // As mentioned in the documentation, IsDrill and IsChainsaw automatically reduce useTime and useAnimation to 60% of what is set in SetDefaults and decrease tileBoost by 1, but only for vanilla items.
            // We set it here despite it doing nothing because it is likely to be used by other mods to provide special effects to drill or chainsaw items globally.

            // 如文档所述，IsDrill和IsChainsaw会自动将useTime和useAnimation减少到SetDefaults中设置值的60％，并将tileBoost减少1个单位。但仅适用于原版物品。
            // 我们在这里进行设置尽管它没有任何作用，因为其他模组可能会使用它来全局提供钻头或链锯物品的特殊效果。
            ItemID.Sets.IsDrill[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 27;
            Item.DamageType = DamageClass.Melee;
            Item.width = 20;
            Item.height = 12;

            // IsDrill/IsChainsaw effects must be applied manually, so 60% or 0.6 times the time of the corresponding pickaxe. In this case, 60% of 7 is 4 and 60% of 25 is 15.
            // If you decide to copy values from vanilla drills or chainsaws, you should multiply each one by 0.6 to get the expected behavior.

            // IsDrill/IsChainsaw效果必须手动应用，因此需要对应镐子时间的60％或0.6倍。在这种情况下，7的60％为4，25的60％为15。
            // 如果您决定复制原版钻头或链锯上的数值，则应将每个数值乘以0.6以获得预期行为。
            Item.useTime = 4;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 0.5f;
            Item.value = Item.buyPrice(gold: 12, silver: 60);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item23;
            Item.shoot = ModContent.ProjectileType<AnalysisDrillProjectile>(); // Create the drill projectile
                                                                               // 创建钻头弹丸

            Item.shootSpeed = 32f; // Adjusts how far away from the player to hold the projectile
                                   // 调整离玩家多远才能持有弹丸

            Item.noMelee = true; // Turns off damage from the item itself, as we have a projectile
                                 // 关闭物品本身造成伤害功能（因为我们已经有一个弹丸）

            Item.noUseGraphic = true; // Stops the item from drawing in your hands, for the aforementioned reason
                                      // 停止从手中绘制该物品（出于上述原因）

            Item.channel = true; // Important as the projectile checks if the player channels
                                 // 重要提示：由于弹丸检查玩家是否通道，请务必执行此操作。

            // tileBoost changes the range of tiles that the item can reach.
            // To match Titanium Drill, we should set this to -1, but we'll set it to 10 blocks of extra range for the sake of an Analysis.

            // tileBoost更改物品可以到达的瓷砖范围。
            // 为了与Titanium Drill相匹配，我们应将其设置为-1，但出于分析目的，我们将其设置为额外10个方块的范围。
            Item.tileBoost = 10;

            Item.pick = 190; // How strong the drill is, see https://terraria.wiki.gg/wiki/Pickaxe_power for a list of common values
                             // 钻头强度，请参见https://terraria.wiki.gg/wiki/Pickaxe_power以获取常见值列表
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
