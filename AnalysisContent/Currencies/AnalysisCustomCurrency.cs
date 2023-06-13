using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI;
using Terraria.Localization;

namespace AnalysisMod.AnalysisContent.Currencies
{
    public class AnalysisCustomCurrency : CustomCurrencySingleCoin
    {
        public AnalysisCustomCurrency(int coinItemID, long currencyCap, string CurrencyTextKey) : base(coinItemID, currencyCap)
        {
            this.CurrencyTextKey = CurrencyTextKey;
            CurrencyTextColor = Color.BlueViolet;
        }
    }
}