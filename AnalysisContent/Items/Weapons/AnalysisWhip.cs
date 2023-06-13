using AnalysisMod.AnalysisContent.Buffs;
using AnalysisMod.AnalysisContent.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Weapons
{
    public class AnalysisWhip : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(AnalysisWhipDebuff.TagDamage);

        public override void SetDefaults()
        {
            // This method quickly sets the whip's properties.
            // Mouse over to see its parameters.

            // 这个方法快速设置鞭子的属性。
            // 鼠标悬停以查看其参数。
            Item.DefaultToWhip(ModContent.ProjectileType<AnalysisWhipProjectile>(), 20, 2, 4);
            Item.rare = ItemRarityID.Green;
            Item.channel = true;
        }

        // Please see AnalysisContent/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AnalysisItem>()
                .AddTile<Tiles.Furniture.AnalysisWorkbench>()
                .Register();
        }

        // Makes the whip receive melee prefixes
        // 使鞭子接收近战前缀
        public override bool MeleePrefix()
        {
            return true;
        }
    }
}
