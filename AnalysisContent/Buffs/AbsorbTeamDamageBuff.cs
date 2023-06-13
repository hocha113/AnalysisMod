using AnalysisMod.AnalysisContent.Items.Accessories;
using AnalysisMod.AnalysisCommon.Players;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Buffs
{
    public class AbsorbTeamDamageBuff : ModBuff
    {
        public override LocalizedText Description => base.Description.WithFormatArgs(AbsorbTeamDamageAccessory.DamageAbsorptionPercent);

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<AnalysisDamageModificationPlayer>().defendedByAbsorbTeamDamageEffect = true;
        }
    }
}
