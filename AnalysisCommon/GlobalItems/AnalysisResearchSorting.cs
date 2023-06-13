using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisCommon.GlobalItems
{
    // This file shows Analysiss of creating and setting your own sorting group in Journey mode's Duplication menu, as well as changing the sorting groups of existing items.
    // Creating your own research sorting group can be useful if your mod has a specific custom item type, or the vanilla sorting method doesn't assign the right group to your item.
    // While you can do this in ModItem, there are benefits to adding all modded items to sorting groups in bulk using GlobalItem, as shown here.
    // 本文件展示了在旅程模式的复制菜单中创建和设置自己的排序组，以及更改现有物品的排序组。
    // 如果您的mod具有特定的自定义物品类型或者原始游戏中默认分类方法没有为您的物品分配正确的组别，则创建自己研究用途的分类组可能会很有用。
    // 虽然您可以在ModItem中完成此操作，但使用GlobalItem批量添加所有modded items到排序组也是一种不错选择。
    public class AnalysisResearchSorting : GlobalItem
    {
        // Here we add both every item in this mod to a single custom sorting group, as well as add an existing item, the copper shortsword, to a vanilla sorting group.
        // These can be interchanged, modded items can go in vanilla sorting groups and vice versa.
        // 在这里，我们将此模组中的每个物品都添加到一个自定义排序组中，并将现有物品——铜短剑——添加到原版排序组中。
        // 这些可以互换，modded 物品可以放在原版排序组中，反之亦然。
        public override void ModifyResearchSorting(Item item, ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            if (item.ModItem?.Mod == Mod)
            {
                itemGroup = (ContentSamples.CreativeHelper.ItemGroup)1337; // This number is where the item sort is in relation to any other sorts added by vanilla or mods; 1337 set here is in between the Critters and Keys sorts. To know where your custom group relates to the vanilla sorting numbers, refer to the vanilla ItemGroup class, which you can easily get to by pressing f12 if using Visual Studio.
                                                                           // 这个数字表示该物品分类与由原版或模组添加的其他分类之间的关系；设置为1337，位于 Critters 和 Keys 分类之间。要了解自定义分组与原始排序数字的关系，请参考原始 ItemGroup 类，如果使用 Visual Studio，则可以轻松访问它，按 f12 即可。
            }

            if (item.type == ItemID.CopperShortsword)
            {
                itemGroup = ContentSamples.CreativeHelper.ItemGroup.EventItem; // Changed the copper shortsword's default sorting to be with the event items instead of melee weapons.
                                                                               // Vanilla already has many default research sorting groups that you can add your item into. It is usually done automatically with a few exceptions. For an Analysis of an exception, refer to the AnalysisFishingCrate file.
                                                                               // 将铜短剑的默认排序更改为与事件物品而非近战武器一起。
                                                                               // Vanilla已经有许多默认的研究分类组，您可以将自己的物品添加到其中。通常情况下会自动完成，但也有少数例外。关于异常情况的分析，请参阅AnalysisFishingCrate文件。
            }
        }
    }
}
