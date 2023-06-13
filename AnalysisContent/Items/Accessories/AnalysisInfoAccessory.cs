using AnalysisMod.AnalysisCommon.Players;
using AnalysisMod.AnalysisContent.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Accessories
{
    /// <summary>
    /// ModItem that is coupled with <seealso cref="AnalysisInfoDisplay"/> and <seealso cref="AnalysisInfoDisplayPlayer"/> to show
    /// off how to add a new info accessory (such as a Radar, Lifeform Analyzer, etc.)<br/>
    /// ModItem与<seealso cref="AnalysisInfoDisplay"/>和<seealso cref="AnalysisInfoDisplayPlayer"/>配对使用，以展示如何添加新的信息附件（例如雷达、生物分析仪等）。
    /// </summary>
    public class AnalysisInfoAccessory : ModItem
    {
        public override void SetStaticDefaults()
        {
            // We want the information benefits of this accessory to work while in the void bag in order to keep
            // it in line with the vanilla accessories; this is what this set is used for.
            // If you DON'T want your info accessory to work in the void bag, then don't add this line.

            // 我们希望这个附件的信息效益在虚空袋中也能起作用，以使其符合原版附件；这就是此设置的用途。
            // 如果您不希望您的信息附件在虚空袋中工作，则不要添加此行。         
            ItemID.Sets.WorksInVoidBag[Type] = true;
        }

        public override void SetDefaults()
        {
            // We don't need to add anything particularly unique for the stats of this item; so let's just clone the Radar.
            // 对于该物品的统计数据，我们不需要添加任何特别独特之处；因此让我们只克隆雷达即可。
            Item.CloneDefaults(ItemID.Radar);
        }

        // This is the main hook that allows for our info display to actually work with this accessory. 
        // 这是主要钩子，它允许我们的信息显示实际上可以与此附件一起使用。
        public override void UpdateInfoAccessory(Player player)
        {
            player.GetModPlayer<AnalysisInfoDisplayPlayer>().showMinionCount = true;
        }

        // Please see AnalysisContent/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        // 有关制作配方详细说明，请参见AnalysisContent/AnalysisRecipes.cs。
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AnalysisItem>()
                .AddTile<AnalysisWorkbench>()
                .Register();
        }
    }
}
