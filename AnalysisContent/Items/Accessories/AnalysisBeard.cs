using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Accessories
{
    //Showcases a beard vanity item that uses a greyscale sprite which gets its' color from the players' hair
    //Requires ArmorIDs.Beard.Sets.UseHairColor and Item.color to be used properly
    //For a beard with a fixed color, remove the above mentioned code

    //展示一个胡须装饰物，使用灰度精灵图，并从玩家的头发中获取颜色
    //需要启用ArmorIDs.Beard.Sets.UseHairColor和Item.color才能正常使用
    //如果要使用固定颜色的胡须，请删除上述代码
    [AutoloadEquip(EquipType.Beard)]
    public class AnalysisBeard : ModItem
    {
        public override void SetStaticDefaults()
        {
            ArmorIDs.Beard.Sets.UseHairColor[Item.beardSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 14;
            Item.maxStack = 1;
            Item.color = Main.LocalPlayer.hairColor;
            Item.value = Item.sellPrice(0, 1);
            Item.accessory = true;
            Item.vanity = true;
        }
    }
}
