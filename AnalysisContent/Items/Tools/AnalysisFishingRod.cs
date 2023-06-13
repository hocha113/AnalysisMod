using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Tools
{
    public class AnalysisFishingRod : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.CanFishInLava[Item.type] = true; // Allows the pole to fish in lava
                                                         // 允许钓鱼竿在熔岩中垂钓
        }

        public override void SetDefaults()
        {
            // These are copied through the CloneDefaults method:
            // Item.width = 24;
            // Item.height = 28;
            // Item.useStyle = ItemUseStyleID.Swing;
            // Item.useAnimation = 8;
            // Item.useTime = 8;
            // Item.UseSound = SoundID.Item1;
            Item.CloneDefaults(ItemID.WoodFishingPole);

            Item.fishingPole = 30; // Sets the poles fishing power
                                   // 设置钓竿的垂钓能力

            Item.shootSpeed = 12f; // Sets the speed in which the bobbers are launched. Wooden Fishing Pole is 9f and Golden Fishing Rod is 17f.
                                   // 设置浮标发射的速度。木质渔杆为9f，金色渔杆为17f。

            Item.shoot = ModContent.ProjectileType<Projectiles.AnalysisBobber>(); // The Bobber projectile.
                                                                                  // 浮标弹药。
        }

        // Grants the High Test Fishing Line bool if holding the item.
        // NOTE: Only triggers through the hotbar, not if you hold the item by hand outside of the inventory.

        // 如果手持该物品，则授予高级试验型钓线布尔值。
        // 注意：仅通过热键触发，如果您手持物品并不在库存之外，则无法触发。
        public override void HoldItem(Player player)
        {
            player.accFishingLine = true;
        }

        // Overrides the default shooting method to fire multiple bobbers.
        // NOTE: This will allow the fishing rod to summon multiple Duke Fishrons with multiple Truffle Worms in the inventory.

        // 覆盖默认射击方法以发射多个浮标。
        // 注意：这将允许钓鱼竿使用库存中的多个松露蠕虫召唤多个公爵·费雄。
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int bobberAmount = Main.rand.Next(3, 6); // 3 to 5 bobbers
                                                     // 3至5个浮标

            float spreadAmount = 75f; // how much the different bobbers are spread out.
                                      // 不同浮标分散程度如何。

            for (int index = 0; index < bobberAmount; ++index)
            {
                Vector2 bobberSpeed = velocity + new Vector2(Main.rand.NextFloat(-spreadAmount, spreadAmount) * 0.05f, Main.rand.NextFloat(-spreadAmount, spreadAmount) * 0.05f);

                // Generate new bobbers
                // 生成新的浮标
                Projectile.NewProjectile(source, position, bobberSpeed, type, 0, 0f, player.whoAmI);
            }
            return false;
        }

        // Please see AnalysisContent/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AnalysisItem>(10)
                .AddTile<Tiles.Furniture.AnalysisWorkbench>()
                .Register();
        }
    }
}