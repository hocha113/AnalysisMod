using AnalysisMod.AnalysisCommon.GlobalNPCs;
using AnalysisMod.AnalysisCommon.Players;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Buffs
{
    /// <summary>
    /// This debuff reduces enemy armor by 25%. Use <see cref="Content.Items.Weapons.HitModifiersShowcase"/> to apply.<br/>
    /// By using a buff we can apply to both players and NPCs, and also rely on vanilla to sync the AddBuff calls so we don't need to write our own netcode<br/>
    /// 这个减益效果会使敌人的护甲值降低25%。使用<see cref="Content.Items.Weapons.HitModifiersShowcase"/>来应用。<br/>
    /// 通过使用一个可以对玩家和NPC都有效的增益效果，并且依赖于原版同步AddBuff调用，因此我们不需要编写自己的网络代码。
    /// </summary>
    public class AnalysisDefenseDebuff : ModBuff
    {
        public const int DefenseReductionPercent = 25;
        public static float DefenseMultiplier = 1 - DefenseReductionPercent / 100f;

        public override void SetStaticDefaults()
        {
            Main.pvpBuff[Type] = true; // This buff can be applied by other players in Pvp, so we need this to be true.
                                       // 在PvP中其他玩家也可以施加这个增益效果，所以我们需要确保它是正确的。
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<DamageModificationGlobalNPC>().analysisDefenseDebuff = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<AnalysisDamageModificationPlayer>().AnalysisDefenseDebuff = true;
            player.statDefense *= DefenseMultiplier;
        }
    }
}
