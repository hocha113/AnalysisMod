using AnalysisMod.AnalysisContent.Biomes;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisCommon.GlobalNPCs
{
    public class AnalysisNPCHappiness : GlobalNPC
    {
        public override void SetStaticDefaults()
        {
            int AnalysisPersonType = ModContent.NPCType<AnalysisContent.NPCs.AnalysisPerson>(); // Get AnalysisPerson's type
                                                                                                // 获取AnalysisPerson的类型
            var guideHappiness = NPCHappiness.Get(NPCID.Guide); // Get the key into The Guide's happiness
                                                                // 让The Guide快乐地拿到钥匙

            guideHappiness.SetNPCAffection(AnalysisPersonType, AffectionLevel.Love); // Make the Guide love AnalysisPerson!
                                                                                     // 让Guide爱上AnalysisPerson！

            guideHappiness.SetBiomeAffection<AnalysisSurfaceBiome>(AffectionLevel.Love);  // Make the Guide love AnalysisSurfaceBiome!
                                                                                          // 让Guide爱上AnalysisSurfaceBiome！
        }
    }
}
