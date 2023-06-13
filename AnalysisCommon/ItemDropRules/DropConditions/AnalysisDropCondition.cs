using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace AnalysisMod.AnalysisCommon.ItemDropRules.DropConditions
{
    // Very simple drop condition: drop during daytime
    public class AnalysisDropCondition : IItemDropRuleCondition
    {
        private static LocalizedText Description;

        public AnalysisDropCondition()
        {
            Description ??= Language.GetOrRegister("Mods.AnalysisMod.DropConditions.Analysis");
        }

        public bool CanDrop(DropAttemptInfo info)
        {
            return Main.dayTime;
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
