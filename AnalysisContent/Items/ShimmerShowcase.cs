using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items;

/*
The items in this file showcase customizing the decrafting feature of the Shimmer liquid.
By default, Shimmer will tranform crafted items back into their original recipe ingredients.

ShimmerShowcaseConditions showcases crimson and corruption specific shimmer decrafting results.

ShimmerShowcaseCustomShimmerResult showcases both preventing a recipe from being decrafted and specifying a custom shimmer decrafting result.

To use Shimmer to transform an item into another item instead of decrafting an item, 
simply set "ItemID.Sets.ShimmerTransformToItem[Type] = ItemType here;" in SetStaticDefaults. AnalysisMusicBox and AnalysisTorch show Analysiss of this.

Also note that critter items (Item.makeNPC > 0) will also not attempt to decraft, 
but will instead transform into the NPC that the Item.makeNPC transforms into. NPCID.Sets.ShimmerTransformToNPC sets which NPC an NPC will transform into, 
see PartyZombie and AnalysisCustomAISlimeNPC for Analysiss of this.


这个文件中的项目展示了如何定制Shimmer液体的解合成功能。

默认情况下，Shimmer会将制作好的物品转化回它们原本的配方材料。

ShimmerShowcaseConditions 展示了深红和腐化世界特定的 Shimmer 解合成结果。

ShimmerShowcaseCustomShimmerResult 展示了阻止某个配方被解合成以及指定自定义 Shimmer 解合成结果两种方法。

如果想要使用 Shimmer 将一个物品转化为另一个物品而不是解合成它，
只需在 SetStaticDefaults 中设置 "ItemID.Sets.ShimmerTransformToItem[Type] = ItemType here;"。AnalysisMusicBox 和 AnalysisTorch 显示了此操作分析过程。

还需要注意到，动物类物品（Item.makeNPC > 0）也不会尝试进行解合成，

而是会变形为 Item.makeNPC 变形后对应的 NPC。NPCID.Sets.ShimmerTransformToNPC 设置 NPC 变形后对应哪个 NPC，

PartyZombie 和 AnalysisCustomAISlimeNPC 显示了此操作分析过程。
*/
public class ShimmerShowcaseConditions : ModItem
{
    public override string Texture => "AnalysisMod/AnalysisContent/Items/AnalysisItem";

    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 20;
    }

    public override void AddRecipes()
    {
        // Many items have multiple recipes. The first added recipe will usually be used for shimmer decrafting.
        // Recipe decraft conditions may be used to only allow decrafting under certain conditions, the first recipe found that satisfies all of it's decraft conditions will be used.
        // Therefore, this desert-specific Analysis has priority over the world evil Analysiss registered after it.

        // 许多物品有多种配方。通常第一次添加的配方将用于 shimmer 解合成。
        // 配方解组条件可用于仅允许在某些条件下进行解组，在满足所有其解组条件的第一个找到的配方将被使用。
        // 因此，这个沙漠特定的分析比之后注册的世界邪恶分析具有优先权。
        CreateRecipe()
            .AddIngredient<AnalysisItem>()
            .AddIngredient(ItemID.Cactus)
            .AddTile<Tiles.Furniture.AnalysisWorkbench>()
            .AddDecraftCondition(Condition.InDesert)
            .Register();

        // In these 2 Analysiss, decraft conditions are used to make the recipes decraftable only in their respective world types
        // 在这两个分析中，解组条件用于使配方仅在各自的世界类型中可解组
        CreateRecipe()
            .AddIngredient<AnalysisItem>()
            .AddIngredient(ItemID.RottenChunk)
            .AddTile<Tiles.Furniture.AnalysisWorkbench>()
            .AddDecraftCondition(Condition.CorruptWorld)
            .Register();

        CreateRecipe()
            .AddIngredient<AnalysisItem>()
            .AddIngredient(ItemID.Vertebrae)
            .AddTile<Tiles.Furniture.AnalysisWorkbench>()
            .AddDecraftCondition(Condition.CrimsonWorld)
            .Register();

        // Finally, the ApplyConditionsAsDecraftConditions method can be used to quickly mirror any crafting conditions onto the decrafting conditions.
        // 最后，ApplyConditionsAsDecraftConditions 方法可用于快速将任何制作条件映射到解合成条件上。
    }
}

public class ShimmerShowcaseCustomShimmerResult : ModItem
{
    public override string Texture => "AnalysisMod/AnalysisContent/Items/AnalysisItem";

    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 20;
    }

    public override void AddRecipes()
    {
        // By default, the first added recipe will be used for shimmer decrafting. We can use DisableDecraft() to tell the game to ignore this recipe and use the below recipe instead.
        // 默认情况下，第一个添加的配方将被用于 shimmer 解合成。我们可以使用 DisableDecraft() 告诉游戏忽略此配方并改为使用下面的配方。
        CreateRecipe()
            .AddIngredient<AnalysisItem>()
            .AddIngredient(ItemID.PadThai)
            .AddTile<Tiles.Furniture.AnalysisWorkbench>()
            .DisableDecraft()
            .Register();

        // AddCustomShimmerResult can be used to change the decrafting results. Rather that return 1 AnalysisItem, decrafting this item will return 1 Rotten Egg and 3 Chain.
        // AddCustomShimmerResult 可以用来更改解合成结果。不是返回 1 AnalysisItem，而是会返回 1 Rotten Egg 和 3 Chain。
        CreateRecipe()
            .AddIngredient<AnalysisItem>()
            .AddTile<Tiles.Furniture.AnalysisWorkbench>()
            .AddCustomShimmerResult(ItemID.RottenEgg)
            .AddCustomShimmerResult(ItemID.Chain, 3)
            .Register();
    }
}
