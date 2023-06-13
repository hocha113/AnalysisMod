using Terraria;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Buffs
{
    public class AnalysisMountBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true; // The time remaining won't display on this buff
                                                 // 这个增益效果不会显示剩余时间

            Main.buffNoSave[Type] = true; // This buff won't save when you exit the world
                                          // 退出世界后，此增益效果将不会保存
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(ModContent.MountType<Mounts.AnalysisMount>(), player);
            player.buffTime[buffIndex] = 10; // reset buff time
                                             // 重置增益时间
        }
    }
}
