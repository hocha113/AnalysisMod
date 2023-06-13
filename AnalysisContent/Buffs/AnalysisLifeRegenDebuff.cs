using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Buffs
{
    // This class serves as an Analysis of a debuff that causes constant loss of life
    // See AnalysisLifeRegenDebuffPlayer.UpdateBadLifeRegen at the end of the file for more information

    // 这个类用于分析一个导致持续生命损失的debuff
    // 有关更多信息，请参见文件末尾的AnalysisLifeRegenDebuffPlayer.UpdateBadLifeRegen
    public class AnalysisLifeRegenDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;  // Is it a debuff?
                                       // 它是一个debuff吗？

            Main.pvpBuff[Type] = true; // Players can give other players buffs, which are listed as pvpBuff
                                       // 玩家可以给其他玩家提供增益效果，这些被列为pvpBuff

            Main.buffNoSave[Type] = true; // Causes this buff not to persist when exiting and rejoining the world
                                          // 导致该增益效果在退出并重新加入世界时不会持续存在

            BuffID.Sets.LongerExpertDebuff[Type] = true; // If this buff is a debuff, setting this to true will make this buff last twice as long on players in expert mode
                                                         // 如果此增益效果是debuff，则将其设置为true将使该增益效果在专家模式下对玩家持续时间延长一倍
        }

        // Allows you to make this buff give certain effects to the given player
        // 允许您使此增益效果对给定玩家产生某些负面影响
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<AnalysisLifeRegenDebuffPlayer>().lifeRegenDebuff = true;
        }
    }

    public class AnalysisLifeRegenDebuffPlayer : ModPlayer
    {
        // Flag checking when life regen debuff should be activated
        // 标志检查何时应激活生命恢复debuff
        public bool lifeRegenDebuff;

        public override void ResetEffects()
        {
            lifeRegenDebuff = false;
        }

        // Allows you to give the player a negative life regeneration based on its state (for Analysis, the "On Fire!" debuff makes the player take damage-over-time)
        // This is typically done by setting player.lifeRegen to 0 if it is positive, setting player.lifeRegenTime to 0, and subtracting a number from player.lifeRegen
        // The player will take damage at a rate of half the number you subtract per second

        // 允许您根据其状态（例如，“着火！” debuff）使玩家具有负面生命再生能力
        // 通常通过将player.lifeRegen设置为0（如果它是正数），将player.lifeRegenTime设置为0，并从player.lifeRegen中减去一个数字来完成
        // 玩家每秒受到减去的数字的一半伤害。
        public override void UpdateBadLifeRegen()
        {
            if (lifeRegenDebuff)
            {
                // These lines zero out any positive lifeRegen. This is expected for all bad life regeneration effects
                // 这些行清除任何正面lifeRegen。所有不良生命再生成效应都需要这样做。
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;
                // Player.lifeRegenTime uses to increase the speed at which the player reaches its maximum natural life regeneration
                // So we set it to 0, and while this debuff is active, it never reaches it

                // Player.lifeRegenTime用于加快玩家达到最大自然生命恢复速度的速度
                // 所以我们将其设置为0，并且在此debuff处于活动状态时，它永远不会达到最大值
                Player.lifeRegenTime = 0;
                // lifeRegen is measured in 1/2 life per second. Therefore, this effect causes 8 life lost per second
                // lifeRegen以每秒1/2生命计量。因此，该效果导致每秒损失8点生命。
                Player.lifeRegen -= 16;
            }
        }
    }
}
