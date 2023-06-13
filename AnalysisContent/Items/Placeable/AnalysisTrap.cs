using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Placeable
{
    // This item shows off using 1 class to load multiple items. This is an alternate to typical inheritance.
    // Read the comments in this Analysis carefully, as there are many parts necessary to make this approach work.
    // The real strength of this approach is when you have many items that vary by small changes, like how these 2 trap items vary only by placeStyle.

    // 这个物品展示了使用一个类来加载多个物品。这是一种替代典型继承的方法。
    // 仔细阅读此分析中的注释，因为有许多必要的部分使得这种方法能够工作。
    // 这种方法真正的优势在于当你有许多只有小变化的物品时，比如这两个陷阱物品只是placeStyle不同。
    public class AnalysisTrap : ModItem
    {
        // This inner class is an ILoadable, the game will automatically call the Load method when loading this mod.
        // Using this class, we manually call AddContent with 2 instances of the AnalysisTrap class. This adds them to the game.

        // 这个内部类是ILoadable，游戏会自动调用Load方法来加载此mod。
        // 使用这个类，我们手动调用AddContent并添加了两个AnalysisTrap实例。这将它们添加到游戏中。
        public class AnalysisTrapLoader : ILoadable
        {
            public void Load(Mod mod)
            {
                mod.AddContent(new AnalysisTrap(0));
                mod.AddContent(new AnalysisTrap(1));
            }

            public void Unload()
            {
            }
        }

        // CloneNewInstances is needed so that fields in this class are Cloned onto new instances, such as when this item is crafted or hovered over.
        // By default, the game creates new instances rather than clone. By forcing Clone, we can preserve fields per Item added by the mod while sharing the same class.

        // CloneNewInstances需要确保该类中的字段被复制到新实例上，例如当制作或悬停在该项上时。
        // 默认情况下，游戏创建新实例而不是克隆。通过强制Clone操作，我们可以保留由mod添加每个Item所拥有的字段，并共享相同的class。
        protected override bool CloneNewInstances => true;
        private readonly int placeStyle;

        // The internal name of each ModItem must be unique. This code ensures that each of the 2 AnalysisTrap instances added have a unique name.
        // In the localization files, these internal names are used as keys for DisplayName and Tooltip, rather than the classname.

        // 每个ModItem都必须具有唯一名称。此代码确保添加的2个AnalysisTrap实例具有唯一名称。
        // 在本地化文件中，使用这些内部名称作为DisplayName和Tooltip键名字而不是classname。
        public override string Name => GetInternalNameFromStyle(placeStyle);

        // This helper method converts from the custom instanced data to the internal name. In this Analysis the placeStyle value is the only custom data.
        // This method is called by the Name property and 

        // 此辅助方法将从自定义实例数据转换为内部名称。在本次分析中placeStyle值是唯一定制数据。
        // 此方法由Name属性调用。
        public static string GetInternalNameFromStyle(int style)
        {
            // Here we define some strings that will be used as the ModItem.Name, the internal name of the ModItem.
            // Every ModItem must have a unique internal name, so this step is necessary.
            // We use these in the AnalysisMod.AnalysisContent.Tiles.AnalysisTrap.GetItemDrops rather than ModContent.ItemType<Items.Placeable.AnalysisTrap>() to retrieve the correct ItemID.

            // 在这里，我们定义了一些字符串，它们将用作ModItem.Name，即ModItem的内部名称。
            // 每个ModItem都必须具有唯一的内部名称，因此这一步是必要的。
            // 我们在AnalysisMod.AnalysisContent.Tiles.AnalysisTrap.GetItemDrops中使用它们而不是使用 ModContent.ItemType<Items.Placeable.AnalysisTrap>() 来检索正确的 ItemID。
            if (style == 0)
            {
                return "AnalysisTrapIchorBullet";
            }
            if (style == 1)
            {
                return "AnalysisTrapChlorophyteBullet";
            }
            throw new Exception("Invalid style");
        }

        // Content loaded multiple times must have a non-default constructor. This is where unique data is passed in to be used later. This also prevents the game from attempting to add this ModItem to the game automatically.
        // 多次加载内容必须具有非默认构造函数。这是传递唯一数据以供稍后使用的地方。这也可以防止游戏尝试自动将此ModItem添加到游戏中。
        public AnalysisTrap(int placeStyle)
        {
            this.placeStyle = placeStyle;
        }

        public override void SetDefaults()
        {
            // With all the setup above, placeStyle will be either 0 or 1 for the 2 AnalysisTrap instances we've loaded.
            // 通过以上所有设置，placeStyle将为我们已经加载过的2个AnalysisTrap实例之一为0或1.
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.AnalysisTrap>(), placeStyle);

            Item.width = 12;
            Item.height = 12;
            Item.value = 10000;
            Item.mech = true; // lets you see wires while holding.
                              // 让你在手持时看到电线。
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.DartTrap)
                .Register();
        }
    }
}
