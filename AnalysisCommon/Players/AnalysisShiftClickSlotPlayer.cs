using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace AnalysisMod.AnalysisCommon.Players
{
    // If we hover the cursor over a gel, the cursor style will change.
    // If we shift-click it, it changes its color and rarity.
    // See GelGlobalItem.cs as well, we add a tooltip line for gel to indicate what will happen

    // 当鼠标悬停在凝胶上时，光标样式会改变。
    // 如果按住Shift键单击它，则会更改其颜色和稀有度。
    // 请参阅GelGlobalItem.cs，我们为凝胶添加了一个工具提示行以指示将发生什么
    public class AnalysisShiftClickSlotPlayer : ModPlayer
    {
        public override bool ShiftClickSlot(Item[] inventory, int context, int slot)
        {
            // Apply our changes if this item is in inventory and is gel
            // 如果此物品在库存中且是凝胶，则应用我们的更改
            if (context == ItemSlot.Context.InventoryItem && inventory[slot].type == ItemID.Gel)
            {
                inventory[slot].color = Main.DiscoColor; // Change the color of the item into a "random" color
                                                         // 将物品的颜色更改为“随机”颜色

                inventory[slot].rare = Main.rand.Next(ItemRarityID.Count); // Random rarity
                                                                           // 随机稀有度

                SoundEngine.PlaySound(SoundID.Item4); // Play mana crystal using sound
                                                      // 播放法力水晶音效

                // Block vanilla code so the item will not be picked up when it is clicked.
                // 阻止基础代码拾取该物品。
                return true;
            }
            return base.ShiftClickSlot(inventory, context, slot);
        }

        // Here we override the cursor style
        // 在这里覆盖光标样式。
        public override bool HoverSlot(Item[] inventory, int context, int slot)
        {
            // Apply our changes if this item is in inventory and is gel
            // 如果此物品在库存中且是凝胶，则应用我们的更改。
            if (context == ItemSlot.Context.InventoryItem && inventory[slot].type == ItemID.Gel)
            {
                // If player is holding shift, use FavoriteStar texture to indicate that a special action will be performed
                // 如果玩家按住Shift键，则使用FavoriteStar纹理表示将执行特殊操作。
                if (ItemSlot.ShiftInUse)
                {
                    Main.cursorOverride = CursorOverrideID.FavoriteStar;
                    return true; // return true to prevent other things from overriding cursor
                                 // 返回true以防止其他内容覆盖光标。
                }
            }
            return base.HoverSlot(inventory, context, slot);
        }
    }
}
