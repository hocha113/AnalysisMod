using AnalysisMod.AnalysisCommon.GlobalNPCs;
using Terraria;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Buffs
{
    public class AnalysisJavelinDebuff : ModBuff
    {
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<DamageOverTimeGlobalNPC>().AnalysisJavelinDebuff = true;
        }
    }
}
