using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisCommon.GlobalNPCs
{
    public class GuideGlobalNPC : GlobalNPC
    {
        public override bool AppliesToEntity(NPC npc, bool lateInstatiation)
        {
            return npc.type == NPCID.Guide;
        }

        public override void AI(NPC npc)
        {
            // Make the guide giant and green.
            // 把指南变成巨大的绿色。
            npc.scale = 1.5f;
            npc.color = Color.ForestGreen;
        }
    }
}
