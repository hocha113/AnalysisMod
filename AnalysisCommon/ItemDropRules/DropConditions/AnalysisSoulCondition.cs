using AnalysisMod.AnalysisContent.Biomes;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace AnalysisMod.AnalysisCommon.ItemDropRules.DropConditions
{
    public class AnalysisSoulCondition : IItemDropRuleCondition
    {
        private static LocalizedText Description;

        public AnalysisSoulCondition()
        {
            Description ??= Language.GetOrRegister("Mods.AnalysisMod.DropConditions.AnalysisSoul");
        }

        public bool CanDrop(DropAttemptInfo info)
        {
            NPC npc = info.npc;
            return Main.hardMode
                && !NPCID.Sets.CannotDropSouls[npc.type]
                && !npc.boss
                && !npc.friendly
                && npc.lifeMax > 1
                && npc.value >= 1f
                && info.player.InModBiome<AnalysisUndergroundBiome>();
        }

        public bool CanShowItemDropInUI()
        {
            return true;
        }

        public string GetConditionDescription()
        {
            return Description.Value;
        }
    }
}
