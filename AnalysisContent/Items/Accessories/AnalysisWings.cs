using System.Linq;
using AnalysisMod.AnalysisCommon.Configs;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Accessories
{
    [AutoloadEquip(EquipType.Wings)]
    public class AnalysisWings : ModItem
    {
        // To see how this config option was added, see AnalysisModConfig.cs
        // 要查看此配置选项的添加方式，请参见AnalysisModConfig.cs
        public override bool IsLoadingEnabled(Mod mod)
        {
            return ModContent.GetInstance<AnalysisModConfig>().AnalysisWingsToggle;
        }

        public override void SetStaticDefaults()
        {
            // These wings use the same values as the solar wings
            // Fly time: 180 ticks = 3 seconds
            // Fly speed: 9
            // Acceleration multiplier: 2.5

            // 这些翅膀使用与太阳能翅膀相同的值
            // 飞行时间：180个刻度= 3秒钟
            // 飞行速度：9
            // 加速倍增器：2.5
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(180, 9f, 2.5f);
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 20;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
            ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.85f; // Falling glide speed
                                       // 下降滑翔速度

            ascentWhenRising = 0.15f; // Rising speed
                                      // 上升速度

            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 3f;
            constantAscend = 0.135f;
        }

        // Please see Content/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        // 有关配方创建的详细说明，请参见Content / AnalysisRecipes.cs。
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AnalysisItem>()
                .AddTile<Tiles.Furniture.AnalysisWorkbench>()
                .SortBefore(Main.recipe.First(recipe => recipe.createItem.wingSlot != -1)) // Places this recipe before any wing so every wing stays together in the crafting menu.
                                                                                           // 将此配方放置在任何翅膀之前，以便每个翅膀都保持在制作菜单中。
                .Register();
        }
    }
}
