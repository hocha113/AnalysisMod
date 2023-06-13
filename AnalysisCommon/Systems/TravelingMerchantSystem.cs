using AnalysisMod.AnalysisContent.NPCs;
using System;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AnalysisMod.AnalysisCommon.Systems;

public class TravelingMerchantSystem : ModSystem
{
    public override void PreUpdateWorld()
    {
        AnalysisTravelingMerchant.UpdateTravelingMerchant();
    }

    public override void SaveWorldData(TagCompound tag)
    {
        if (AnalysisTravelingMerchant.spawnTime != double.MaxValue)
        {
            tag["AnalysisTravelingMerchantSpawnTime"] = AnalysisTravelingMerchant.spawnTime;
        }
    }

    public override void LoadWorldData(TagCompound tag)
    {
        if (!tag.TryGet("AnalysisTravelingMerchantSpawnTime", out AnalysisTravelingMerchant.spawnTime))
        {
            AnalysisTravelingMerchant.spawnTime = double.MaxValue;
        }
    }
}