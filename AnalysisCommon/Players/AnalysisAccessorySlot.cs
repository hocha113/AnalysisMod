using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisCommon.Players
{
    public class AnalysisModAccessorySlot1 : ModAccessorySlot
    {
        // 如果类为空，则所有内容都将默认为基本香草字段值。
    }

    public class AnalysisCustomLocationAndTextureSlot : ModAccessorySlot
    {
        // 我们将把插槽放在地图中心，决定不遵循内部UI处理
        public override Vector2? CustomLocation => new Vector2(Main.screenWidth / 2, 3 * Main.screenHeight / 4);

        // 当有染料时，我们会绘制装饰插槽
        public override bool DrawVanitySlot => !DyeItem.IsAir;

        // 我们将使用自定义纹理
        // 背景纹理->通常情况下，您可以使用大多数现有的香草来获得不同的颜色
        public override string VanityBackgroundTexture => "Terraria/Images/Inventory_Back14"; // yellow
        public override string FunctionalBackgroundTexture => "Terraria/Images/Inventory_Back7"; // pale blue

        // 图标纹理。名义上的图像大小为32x32。小猪储钱罐是16x24，但它仍然可以正常工作，因为它被居中绘制。
        public override string VanityTexture => "Terraria/Images/Item_" + ItemID.PiggyBank;

        // 大多数时间我们会保持隐藏状态，以免过于突兀分析
        public override bool IsHidden()
        {
            return IsEmpty; // Only show when it contains an item, items can end up in functional slots via quick swap (right click accessory)
                            // 只有当包含物品时才显示，在快速交换（右键配件）中可能会出现功能性插槽中的物品
        }
    }

    public class AnalysisModWingSlot : ModAccessorySlot
    {
        public override bool CanAcceptItem(Item checkItem, AccessorySlotType context)
        {
            if (checkItem.wingSlot > 0) // if is Wing, then can go in slot
                                        // 如果是翅膀，则可以放入插槽
                return true;

            return false; // Otherwise nothing in slot
                          // 否则没有东西在插槽里面
        }

        // Designates our slot to be a priority for putting wings in to. NOTE: use ItemLoader.CanEquipAccessory if aiming for restricting other slots from having wings!
        // 指定我们的插槽优先考虑放置翅膀。注意：如果要限制其他插槽拥有翅膀，请使用ItemLoader.CanEquipAccessory！
        public override bool ModifyDefaultSwapSlot(Item item, int accSlotToSwapTo)
        {
            if (item.wingSlot > 0) // If is Wing, then we want to prioritize it to go in to our slot.
                                   // 如果是翅膀，则希望优先考虑放入我们的插槽。
                return true;

            return false;
        }

        public override bool IsEnabled()
        {
            if (Player.armor[0].headSlot >= 0) // if player is wearing a helmet, because flight safety
                                               // 如果玩家戴着头盔，因为飞行安全

                return true; // Then can use Slot

            return false; // Can't use slot
        }

        // Overrides the default behaviour where a disabled accessory slot will allow retrieve items if it contains items
        // 覆盖默认行为，禁用配件插槽将允许检索包含物品的项目
        public override bool IsVisibleWhenNotEnabled()
        {
            return false; // We set to false to just not display if not Enabled. NOTE: this does not affect behavour when mod is unloaded!
                          // 我们将其设置为false，以便在未启用时不显示。注意：这不会影响模组卸载后的行为！
        }

        // Icon textures. Nominal image size is 32x32. Will be centered on the slot.
        // 图标纹理。名义上的图像大小为32x32。将居中于插槽上。
        public override string FunctionalTexture => "Terraria/Images/Item_" + ItemID.CreativeWings;

        // Can be used to modify stuff while the Mouse is hovering over the slot.
        // 可用于在鼠标悬停在插槽上时修改一些内容。
        public override void OnMouseHover(AccessorySlotType context)
        {
            // We will modify the hover text while an item is not in the slot, so that it says "Wings".
            // 当物品不在插槽中时，我们将修改悬停文本，使其显示“翅膀”。
            switch (context)
            {
                case AccessorySlotType.FunctionalSlot:
                case AccessorySlotType.VanitySlot:
                    Main.hoverItemName = "Wings";
                    break;
                case AccessorySlotType.DyeSlot:
                    Main.hoverItemName = "Wings Dye";
                    break;
            }
        }
    }
}
