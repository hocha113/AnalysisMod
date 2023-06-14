using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Accessories
{
    //Showcases a beard vanity item that uses a greyscale sprite which gets its' color from the players' hair
    //Requires ArmorIDs.Beard.Sets.UseHairColor and Item.color to be used properly
    //For a beard with a fixed color, remove the above mentioned code

    //չʾһ������װ���ʹ�ûҶȾ���ͼ��������ҵ�ͷ���л�ȡ��ɫ
    //��Ҫ����ArmorIDs.Beard.Sets.UseHairColor��Item.color��������ʹ��
    //���Ҫʹ�ù̶���ɫ�ĺ��룬��ɾ����������
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
