using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace AnalysisMod.AnalysisCommon.ItemDropRules.DropConditions
{
    // Drop condition where items drop only on Journey mode.
    public class AnalysisJourneyModeDropCondition : IItemDropRuleCondition
    {
        private static LocalizedText Description;

        public AnalysisJourneyModeDropCondition()
        {
            Description ??= Language.GetOrRegister("Mods.AnalysisMod.DropConditions.JourneyMode");
        }

        public bool CanDrop(DropAttemptInfo info)
        {
            return Main.GameModeInfo.IsJourneyMode;
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
