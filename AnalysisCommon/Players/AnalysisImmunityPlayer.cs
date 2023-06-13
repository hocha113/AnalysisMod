using Terraria;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisCommon.Players
{
    public class AnalysisImmunityPlayer : ModPlayer
    {
        public bool HasAnalysisImmunityAcc;

        // Always reset the accessory field to its default value here.
        public override void ResetEffects()
        {
            HasAnalysisImmunityAcc = false;
        }

        // Vanilla applies immunity time before this method and after PreHurt and Hurt
        // Therefore, we should apply our immunity time increment here
        public override void PostHurt(Player.HurtInfo info)
        {
            // Here we increase the player's immunity time by 1 second when Analysis Immunity Accessory is equipped
            if (!HasAnalysisImmunityAcc)
            {
                return;
            }

            // Different cooldownCounter values mean different damage types taken and different cooldown slots
            // See ImmunityCooldownID for a list.
            // Don't apply extra immunity time to pvp damage (like vanilla)
            if (!info.PvP)
            {
                Player.AddImmuneTime(info.CooldownCounter, 60);
            }
        }
    }
}
