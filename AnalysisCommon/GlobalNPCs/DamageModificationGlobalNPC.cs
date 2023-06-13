using AnalysisMod.AnalysisContent.Buffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisCommon.GlobalNPCs
{
    internal class DamageModificationGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public bool analysisDefenseDebuff;

        public override void ResetEffects(NPC npc)
        {
            analysisDefenseDebuff = false;
        }

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (analysisDefenseDebuff)
            {
                // For best results, defense debuffs should be multiplicative
                // 为了获得最佳效果，防御减益应该是乘法的
                modifiers.Defense *= AnalysisDefenseDebuff.DefenseReductionPercent;
            }
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            // This simple color effect indicates that the buff is active
            // 这种简单的颜色效果表示增益已激活
            if (analysisDefenseDebuff)
            {
                drawColor.G = 0;
            }
        }
    }
}
