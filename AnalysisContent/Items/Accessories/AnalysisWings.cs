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
        // Ҫ�鿴������ѡ�����ӷ�ʽ����μ�AnalysisModConfig.cs
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

            // ��Щ���ʹ����̫���ܳ����ͬ��ֵ
            // ����ʱ�䣺180���̶�= 3����
            // �����ٶȣ�9
            // ���ٱ�������2.5
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
                                       // �½������ٶ�

            ascentWhenRising = 0.15f; // Rising speed
                                      // �����ٶ�

            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 3f;
            constantAscend = 0.135f;
        }

        // Please see Content/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        // �й��䷽��������ϸ˵������μ�Content / AnalysisRecipes.cs��
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AnalysisItem>()
                .AddTile<Tiles.Furniture.AnalysisWorkbench>()
                .SortBefore(Main.recipe.First(recipe => recipe.createItem.wingSlot != -1)) // Places this recipe before any wing so every wing stays together in the crafting menu.
                                                                                           // �����䷽�������κγ��֮ǰ���Ա�ÿ����򶼱����������˵��С�
                .Register();
        }
    }
}
