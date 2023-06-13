using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items
{
    /// <summary>
    /// This item lets you test the existing ItemUseStyleID values for Item.useStyle. Note that the sword texture might not fit each of the useStyle animations.<br/>
    /// 此项功能可用于测试Item.useStyle的现有ItemUseStyleID值。请注意，剑的纹理可能不适合每个useStyle动画。
    /// </summary>
    public class UseStyleShowcase : ModItem
    {
        public override string Texture => "AnalysisMod/AnalysisContent/Items/Weapons/AnalysisSword";

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;

            // In Visual Studio, you can click on "ItemUseStyleID" and then press F12 to see the list of possible values. You can also type "ItemUseStyleID." to view the list of possible values.
            // 在Visual Studio中，您可以单击“ItemUseStyleID”，然后按F12查看可能值列表。您还可以键入“ItemUseStyleID.”以查看可能值列表。
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1;
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write((byte)Item.useStyle);
        }

        public override void NetReceive(BinaryReader reader)
        {
            Item.useStyle = reader.ReadByte();
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.useStyle++;
                if (Item.useStyle > ItemUseStyleID.RaiseLamp)
                {
                    Item.useStyle = ItemUseStyleID.Swing;
                }
                Main.NewText($"Switching to ItemUseStyleID #{Item.useStyle}");
                // This line will trigger NetSend to be called at the end of this game update, allowing the changes to useStyle to be in sync. 
                // 此行将触发NetSend在此游戏更新结束时被调用，从而使useStyle更改保持同步。
                Item.NetStateChanged();
            }
            else
            {
                Main.NewText($"This is ItemUseStyleID #{Item.useStyle}");
            }
            return true;
        }
    }
}
