using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Placeable
{
    internal class AnalysisLamp : ModItem
    {
        // This Analysis uses LocalizedText.Empty to prevent any translation key from being generated. This can be used for items that definitely won't have a tooltip, keeping the localization file cleaner.
        // 这个分析使用LocalizedText.Empty来防止生成任何翻译键。这可以用于绝对不会有工具提示的项目，使本地化文件更加清洁。
        public override LocalizedText Tooltip => LocalizedText.Empty;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.AnalysisLamp>());
            Item.width = 10;
            Item.height = 24;
            Item.value = 500;
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
