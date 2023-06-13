using System;
using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisCommon.GlobalItems
{
    public class BossBagLoot : GlobalItem
    {
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            // In addition to this code, we also do similar code in Common/GlobalNPCs/AnalysisNPCLoot.cs to edit the boss loot for non-expert drops. Remember to do both if your edits should affect non-expert drops as well.
            // 除了这段代码之外，我们还在Common/GlobalNPCs/AnalysisNPCLoot.cs中编写类似的代码来编辑非专家难度下的Boss战利品。如果您的修改也应该影响到非专家难度下的掉落，请记得同时进行。
            if (item.type == ItemID.QueenBeeBossBag)
            {
                foreach (var rule in itemLoot.Get())
                {
                    if (rule is OneFromOptionsNotScaledWithLuckDropRule oneFromOptionsDrop && oneFromOptionsDrop.dropIds.Contains(ItemID.BeeGun))
                    {
                        var original = oneFromOptionsDrop.dropIds.ToList();
                        original.Add(ItemID.Wood);
                        oneFromOptionsDrop.dropIds = original.ToArray();
                    }
                }
            }
        }
    }
}
