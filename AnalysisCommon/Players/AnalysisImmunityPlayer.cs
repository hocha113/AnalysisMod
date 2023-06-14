using Terraria;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisCommon.Players
{
    public class AnalysisImmunityPlayer : ModPlayer
    {
        public bool HasAnalysisImmunityAcc;

        // Always reset the accessory field to its default value here.
        // 在这里始终将配件字段重置为其默认值。
        public override void ResetEffects()
        {
            HasAnalysisImmunityAcc = false;
        }

        // Vanilla applies immunity time before this method and after PreHurt and Hurt
        // Therefore, we should apply our immunity time increment here

        // Vanilla在PreHurt和Hurt方法之前和之后应用免疫时间
        // 因此，我们应该在这里应用我们的免疫时间增量
        public override void PostHurt(Player.HurtInfo info)
        {
            // Here we increase the player's immunity time by 1 second when Analysis Immunity Accessory is equipped
            // 当装备分析免疫配件时，在此处增加玩家的免疫时间1秒钟
            if (!HasAnalysisImmunityAcc)
            {
                return;
            }

            // Different cooldownCounter values mean different damage types taken and different cooldown slots
            // See ImmunityCooldownID for a list.
            // Don't apply extra immunity time to pvp damage (like vanilla)

            // 不同的cooldownCounter值意味着所受到的不同伤害类型和不同冷却槽位
            // 请参见ImmunityCooldownID列表。
            // 不要对pvp伤害（如香草）施加额外的免疫时间
            if (!info.PvP)
            {
                Player.AddImmuneTime(info.CooldownCounter, 60);
            }
        }
    }
}
