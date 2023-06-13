using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items
{
    /// <summary>
    /// This item lets you test the existing ItemHoldStyleID values for Item.holdStyle. Note that the sword texture might not fit each of the holdStyle animations.<br/>
    /// 此项功能可用于测试Item.holdStyle的现有ItemHoldStyleID值。请注意，剑的纹理可能不适合每个holdStyle动画。
    /// </summary>
    public class HoldStyleShowcase : ModItem
    {
        public override string Texture => "AnalysisMod/AnalysisContent/Items/Weapons/AnalysisSword";

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;

            // In Visual Studio, you can click on "ItemHoldStyleID" and then press F12 to see the list of possible values. You can also type "ItemHoldStyleID." to view the list of possible values.
            // 在Visual Studio中，您可以单击“ItemHoldStyleID”，然后按F12键查看可能的值列表。您也可以输入“ItemHoldStyleID.”来查看可能的值列表。
            Item.holdStyle = ItemHoldStyleID.None;
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write((byte)Item.holdStyle);
        }

        public override void NetReceive(BinaryReader reader)
        {
            Item.holdStyle = reader.ReadByte();
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.holdStyle++;
                if (Item.holdStyle > ItemHoldStyleID.HoldRadio)
                {
                    Item.holdStyle = ItemHoldStyleID.None;
                }
                Main.NewText($"Switching to ItemHoldStyleID #{Item.holdStyle}");
                // This line will trigger NetSend to be called at the end of this game update, allowing the changes to holdStyle to be in sync. 
                // 此行将在游戏更新结束时触发NetSend调用，以使holdStyle更改保持同步。
                Item.NetStateChanged();
            }
            else
            {
                Main.NewText($"This is ItemHoldStyleID #{Item.holdStyle}");
            }
            return true;
        }
    }
}
