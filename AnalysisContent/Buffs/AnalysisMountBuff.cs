using Terraria;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Buffs
{
    public class AnalysisMountBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true; // The time remaining won't display on this buff
                                                 // �������Ч��������ʾʣ��ʱ��

            Main.buffNoSave[Type] = true; // This buff won't save when you exit the world
                                          // �˳�����󣬴�����Ч�������ᱣ��
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(ModContent.MountType<Mounts.AnalysisMount>(), player);
            player.buffTime[buffIndex] = 10; // reset buff time
                                             // ��������ʱ��
        }
    }
}
