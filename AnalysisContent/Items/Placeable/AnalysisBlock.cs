using AnalysisMod.AnalysisContent.Items.Placeable.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Placeable
{
    public class AnalysisBlock : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            ItemID.Sets.ExtractinatorMode[Item.type] = Item.type;

            // Some please convert this to lang files, I'm too lazy to do it
            // Sorry Itorius, I feel you

            // 有人能把这个转换成语言文件吗？我太懒了
            // 对不起 Itorius，我理解你

            // DisplayName.AddTranslation(GameCulture.German, "Beispielblock");
            // Tooltip.AddTranslation(GameCulture.German, "Dies ist ein modded Block");
            // DisplayName.AddTranslation(GameCulture.Italian, "Blocco di esempio");
            // Tooltip.AddTranslation(GameCulture.Italian, "Questo è un blocco moddato");
            // DisplayName.AddTranslation(GameCulture.French, "Bloc d'exemple");
            // Tooltip.AddTranslation(GameCulture.French, "C'est un bloc modgé");
            // DisplayName.AddTranslation(GameCulture.Spanish, "Bloque de ejemplo");
            // Tooltip.AddTranslation(GameCulture.Spanish, "Este es un bloque modded");
            // DisplayName.AddTranslation(GameCulture.Russian, "Блок примера");
            // Tooltip.AddTranslation(GameCulture.Russian, "Это модифицированный блок");
            // DisplayName.AddTranslation(GameCulture.Chinese, "例子块");
            // Tooltip.AddTranslation(GameCulture.Chinese, "这是一个修改块");
            // DisplayName.AddTranslation(GameCulture.Portuguese, "Bloco de exemplo");
            // Tooltip.AddTranslation(GameCulture.Portuguese, "Este é um bloco modded");
            // DisplayName.AddTranslation(GameCulture.Polish, "Przykładowy blok");
            // Tooltip.AddTranslation(GameCulture.Polish, "Jest to modded blok");
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.AnalysisBlock>());
            Item.width = 12;
            Item.height = 12;
        }

        // Please see AnalysisContent/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe(10)
                .AddIngredient<AnalysisItem>()
                .AddTile<Tiles.Furniture.AnalysisWorkbench>()
                .Register();

            CreateRecipe() // Add multiple recipes set to one Item.
                           // 将多个配方设置为一个物品。(从某种意义上来讲，你可以使用这种方法来代替合成组)

                .AddIngredient<AnalysisWall>(4)
                .AddTile<Tiles.Furniture.AnalysisWorkbench>()
                .Register();

            CreateRecipe()
                .AddIngredient<AnalysisPlatform>(2)
                .AddTile<Tiles.Furniture.AnalysisWorkbench>()
                .Register();
        }

        public override void ExtractinatorUse(int extractinatorBlockType, ref int resultType, ref int resultStack)
        { // Calls upon use of an extractinator. Below is the chance you will get AnalysisOre from the extractinator.
            // 使用extractinator时调用。以下是从extractinator获得AnalysisOre的几率。
            if (Main.rand.NextBool(3))
            {
                resultType = ModContent.ItemType<AnalysisOre>();  // Get this from the extractinator with a 1 in 3 chance.
                                                                  // 有1/3的几率从extractinator中获取AnalysisOre。
                if (Main.rand.NextBool(5))
                {
                    resultStack += Main.rand.Next(2); // Add a chance to get more than one of AnalysisOre from the extractinator.
                                                      // 增加从extractinator中获得多个AnalysisOre的机会。
                }
            }
        }
    }
}
