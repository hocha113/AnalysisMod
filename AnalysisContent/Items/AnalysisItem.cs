using AnalysisMod.AnalysisContent.Items.Placeable;
using AnalysisMod.AnalysisContent.Items.Placeable.Furniture;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items
{
    public class AnalysisItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            // The text shown below some item names is called a tooltip. Tooltips are defined in the localization files. See en-US.hjson.
            //显示在某些物品名称下方的文本称为工具提示。 工具提示在本地化文件中定义。 请参见en-US.hjson。

            // How many items are needed in order to research duplication of this item in Journey mode. See https://terraria.wiki.gg/wiki/Journey_Mode#Research for a list of commonly used research amounts depending on item type. This defaults to 1, which is what most items will use, so you can omit this for most ModItems.
            // 在旅程模式下需要多少个物品才能研究此物品的复制。 有关根据项目类型使用常用研究量的列表，请参见https://terraria.wiki.gg/wiki/Journey_Mode#Research。 默认值为1，这是大多数项目将使用的内容，因此您可以省略大多数ModItems。
            Item.ResearchUnlockCount = 100;

            // This item is a custom currency (registered in AnalysisMod), so you might want to make it give "coin luck" to the player when thrown into shimmer. See https://terraria.wiki.gg/wiki/Luck#Coins
            // However, since this item is also used in other shimmer related Analysiss, it's commented out to avoid the item disappearing
            //ItemID.Sets.CoinLuckValue[Type] = Item.value;

            // 此项是自定义货币（已注册AnalysisMod），因此您可能希望在将其投入闪耀时使其给玩家“硬币运气”。 请参阅https://terraria.wiki.gg/wiki/Luck#Coins
            //但是，由于该项还用于其他与闪耀相关的分析中，因此对其进行了注释以避免该项消失
        }

        public override void SetDefaults()
        {
            Item.width = 20; // The item texture's width
                             // 物品纹理宽度

            Item.height = 20; // The item texture's height
                              // 物品纹理高度

            Item.maxStack = Item.CommonMaxStack; // The item's max stack value
                                                 // 物品最大堆叠值

            Item.value = Item.buyPrice(silver: 1); // The value of the item in copper coins. Item.buyPrice & Item.sellPrice are helper methods that returns costs in copper coins based on platinum/gold/silver/copper arguments provided to it.
                                                   // 铜板价值。 Item.buyPrice＆Item.sellPrice是辅助方法，根据提供给它的白金/黄金/白银/铜参数返回以铜币计算的成本。
        }

        // Please see AnalysisContent/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe(999)
                .AddIngredient(ItemID.DirtBlock, 10)
                .AddTile(TileID.WorkBenches)
                .Register();
        }

        // Researching the Analysis item will give you immediate access to the torch, block, wall and workbench!
        // 研究Analysis项目将立即让你获得火把、块、墙和工作台！
        //public override void OnResearched(bool fullyResearched)
        //{
        //    if (fullyResearched)
        //    {
        //        CreativeUI.ResearchItem(ModContent.ItemType<AnalysisTorch>());
        //        CreativeUI.ResearchItem(ModContent.ItemType<AnalysisBlock>());
        //        CreativeUI.ResearchItem(ModContent.ItemType<AnalysisWall>());
        //        CreativeUI.ResearchItem(ModContent.ItemType<AnalysisWorkbench>());
        //    }
        //}
    }
}
