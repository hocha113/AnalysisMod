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
            //��ʾ��ĳЩ��Ʒ�����·����ı���Ϊ������ʾ�� ������ʾ�ڱ��ػ��ļ��ж��塣 ��μ�en-US.hjson��

            // How many items are needed in order to research duplication of this item in Journey mode. See https://terraria.wiki.gg/wiki/Journey_Mode#Research for a list of commonly used research amounts depending on item type. This defaults to 1, which is what most items will use, so you can omit this for most ModItems.
            // ���ó�ģʽ����Ҫ���ٸ���Ʒ�����о�����Ʒ�ĸ��ơ� �йظ�����Ŀ����ʹ�ó����о������б���μ�https://terraria.wiki.gg/wiki/Journey_Mode#Research�� Ĭ��ֵΪ1�����Ǵ������Ŀ��ʹ�õ����ݣ����������ʡ�Դ����ModItems��
            Item.ResearchUnlockCount = 100;

            // This item is a custom currency (registered in AnalysisMod), so you might want to make it give "coin luck" to the player when thrown into shimmer. See https://terraria.wiki.gg/wiki/Luck#Coins
            // However, since this item is also used in other shimmer related Analysiss, it's commented out to avoid the item disappearing
            //ItemID.Sets.CoinLuckValue[Type] = Item.value;

            // �������Զ�����ң���ע��AnalysisMod�������������ϣ���ڽ���Ͷ����ҫʱʹ�����ҡ�Ӳ���������� �����https://terraria.wiki.gg/wiki/Luck#Coins
            //���ǣ����ڸ��������������ҫ��صķ����У���˶��������ע���Ա��������ʧ
        }

        public override void SetDefaults()
        {
            Item.width = 20; // The item texture's width
                             // ��Ʒ������

            Item.height = 20; // The item texture's height
                              // ��Ʒ����߶�

            Item.maxStack = Item.CommonMaxStack; // The item's max stack value
                                                 // ��Ʒ���ѵ�ֵ

            Item.value = Item.buyPrice(silver: 1); // The value of the item in copper coins. Item.buyPrice & Item.sellPrice are helper methods that returns costs in copper coins based on platinum/gold/silver/copper arguments provided to it.
                                                   // ͭ���ֵ�� Item.buyPrice��Item.sellPrice�Ǹ��������������ṩ�����İ׽�/�ƽ�/����/ͭ����������ͭ�Ҽ���ĳɱ���
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
        // �о�Analysis��Ŀ�����������û�ѡ��顢ǽ�͹���̨��
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
