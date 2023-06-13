using AnalysisMod.AnalysisContent.Items.Weapons;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisCommon.GlobalItems
{
    // This is another part of the AnalysisShiftClickSlotPlayer.cs that adds a tooltip line to the gel
    // 这是 AnalysisShiftClickSlotPlayer.cs 的另一部分，它向凝胶添加了一个工具提示行
    public class GelGlobalItem : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ModContent.ItemType<AnalysisSword>();

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            // Here we add a tooltip to the gel to let the player know what will happen
            // 在这里我们添加一个工具提示到凝胶上，让玩家知道会发生什么
            tooltips.Add(new(Mod, "SpecialShiftClick", "Shift-click on this item from your inventory to get a random color and rarity!"));
        }
    }
}
