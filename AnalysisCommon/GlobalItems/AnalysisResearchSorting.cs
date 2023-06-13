using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisCommon.GlobalItems
{
    // This file shows Analysiss of creating and setting your own sorting group in Journey mode's Duplication menu, as well as changing the sorting groups of existing items.
    // Creating your own research sorting group can be useful if your mod has a specific custom item type, or the vanilla sorting method doesn't assign the right group to your item.
    // While you can do this in ModItem, there are benefits to adding all modded items to sorting groups in bulk using GlobalItem, as shown here.
    // ���ļ�չʾ�����ó�ģʽ�ĸ��Ʋ˵��д����������Լ��������飬�Լ�����������Ʒ�������顣
    // �������mod�����ض����Զ�����Ʒ���ͻ���ԭʼ��Ϸ��Ĭ�Ϸ��෽��û��Ϊ������Ʒ������ȷ������򴴽��Լ��о���;�ķ�������ܻ�����á�
    // ��Ȼ��������ModItem����ɴ˲�������ʹ��GlobalItem�����������modded items��������Ҳ��һ�ֲ���ѡ��
    public class AnalysisResearchSorting : GlobalItem
    {
        // Here we add both every item in this mod to a single custom sorting group, as well as add an existing item, the copper shortsword, to a vanilla sorting group.
        // These can be interchanged, modded items can go in vanilla sorting groups and vice versa.
        // ��������ǽ���ģ���е�ÿ����Ʒ����ӵ�һ���Զ����������У�����������Ʒ����ͭ�̽�������ӵ�ԭ���������С�
        // ��Щ���Ի�����modded ��Ʒ���Է���ԭ���������У���֮��Ȼ��
        public override void ModifyResearchSorting(Item item, ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            if (item.ModItem?.Mod == Mod)
            {
                itemGroup = (ContentSamples.CreativeHelper.ItemGroup)1337; // This number is where the item sort is in relation to any other sorts added by vanilla or mods; 1337 set here is in between the Critters and Keys sorts. To know where your custom group relates to the vanilla sorting numbers, refer to the vanilla ItemGroup class, which you can easily get to by pressing f12 if using Visual Studio.
                                                                           // ������ֱ�ʾ����Ʒ��������ԭ���ģ����ӵ���������֮��Ĺ�ϵ������Ϊ1337��λ�� Critters �� Keys ����֮�䡣Ҫ�˽��Զ��������ԭʼ�������ֵĹ�ϵ����ο�ԭʼ ItemGroup �࣬���ʹ�� Visual Studio����������ɷ��������� f12 ���ɡ�
            }

            if (item.type == ItemID.CopperShortsword)
            {
                itemGroup = ContentSamples.CreativeHelper.ItemGroup.EventItem; // Changed the copper shortsword's default sorting to be with the event items instead of melee weapons.
                                                                               // Vanilla already has many default research sorting groups that you can add your item into. It is usually done automatically with a few exceptions. For an Analysis of an exception, refer to the AnalysisFishingCrate file.
                                                                               // ��ͭ�̽���Ĭ���������Ϊ���¼���Ʒ���ǽ�ս����һ��
                                                                               // Vanilla�Ѿ������Ĭ�ϵ��о������飬�����Խ��Լ�����Ʒ��ӵ����С�ͨ������»��Զ���ɣ���Ҳ���������⡣�����쳣����ķ����������AnalysisFishingCrate�ļ���
            }
        }
    }
}
