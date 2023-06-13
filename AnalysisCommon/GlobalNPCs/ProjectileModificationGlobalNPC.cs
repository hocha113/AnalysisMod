using Terraria.ModLoader;

namespace AnalysisMod.AnalysisCommon.GlobalNPCs
{
    // This is a class for functionality related to AnalysisProjectileModifications.
    // 这是一个与AnalysisProjectileModifications相关的功能类。
    public class ProjectileModificationGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public int timesHitByModifiedProjectiles;
    }
}
