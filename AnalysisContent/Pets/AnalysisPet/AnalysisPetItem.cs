using AnalysisMod.AnalysisContent.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Pets.AnalysisPet
{
    public class AnalysisPetItem : ModItem
    {
        // Names and descriptions of all AnalysisPetX classes are defined using .hjson files in the Localization folder
        // 所有AnalysisPetX类的名称和描述都在Localization文件夹中使用.hjson文件定义。
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.ZephyrFish); // Copy the Defaults of the Zephyr Fish Item.
                                                   // 复制Zephyr Fish物品的默认值。

            Item.shoot = ModContent.ProjectileType<AnalysisPetProjectile>(); // "Shoot" your pet projectile.
                                                                             // "射出"你的宠物投射物。
            Item.buffType = ModContent.BuffType<AnalysisPetBuff>(); // Apply buff upon usage of the Item.
                                                                    // 在使用该物品时应用增益效果。
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 3600);
            }
        }

        // Please see Content/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AnalysisItem>()
                .AddTile<Tiles.Furniture.AnalysisWorkbench>()
                .Register();
        }
    }
}
