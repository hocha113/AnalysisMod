using AnalysisMod.AnalysisCommon.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Buffs
{
    /// <summary>
    /// This buff is modeled after the "Holy Protection" buff given to the player by the Hallowed armor set bonus. <br/>
    /// Use <see cref="Items.Weapons.HitModifiersShowcase"/> in mode 6 to apply this buff.<br/>
    /// 这个增益效果是根据神圣护甲套装的套装效果“神圣保护”所设计的。<br/>
    /// 使用模式 6 中的 <see cref="Items.Weapons.HitModifiersShowcase"/> 来应用此增益效果。
    /// </summary>
    internal class AnalysisDodgeBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<AnalysisDamageModificationPlayer>().AnalysisDodge = true;
        }
    }
}
