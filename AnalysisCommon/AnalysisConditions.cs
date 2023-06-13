using AnalysisMod.AnalysisContent.Biomes;
using Terraria.Localization;
using Terraria;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisCommon
{
    public static class AnalysisConditions
    {
        public static Condition InAnalysisBiome = new Condition("Mods.AnalysisMod.Conditions.InAnalysisBiome", () => Main.LocalPlayer.InModBiome<AnalysisSurfaceBiome>() || Main.LocalPlayer.InModBiome<AnalysisUndergroundBiome>());
    }
}
