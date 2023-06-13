using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Buffs
{
    // This class serves as an Analysis of a debuff that causes constant loss of life
    // See AnalysisLifeRegenDebuffPlayer.UpdateBadLifeRegen at the end of the file for more information

    // ��������ڷ���һ�����³���������ʧ��debuff
    // �йظ�����Ϣ����μ��ļ�ĩβ��AnalysisLifeRegenDebuffPlayer.UpdateBadLifeRegen
    public class AnalysisLifeRegenDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;  // Is it a debuff?
                                       // ����һ��debuff��

            Main.pvpBuff[Type] = true; // Players can give other players buffs, which are listed as pvpBuff
                                       // ��ҿ��Ը���������ṩ����Ч������Щ����ΪpvpBuff

            Main.buffNoSave[Type] = true; // Causes this buff not to persist when exiting and rejoining the world
                                          // ���¸�����Ч�����˳������¼�������ʱ�����������

            BuffID.Sets.LongerExpertDebuff[Type] = true; // If this buff is a debuff, setting this to true will make this buff last twice as long on players in expert mode
                                                         // ���������Ч����debuff����������Ϊtrue��ʹ������Ч����ר��ģʽ�¶���ҳ���ʱ���ӳ�һ��
        }

        // Allows you to make this buff give certain effects to the given player
        // ������ʹ������Ч���Ը�����Ҳ���ĳЩ����Ӱ��
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<AnalysisLifeRegenDebuffPlayer>().lifeRegenDebuff = true;
        }
    }

    public class AnalysisLifeRegenDebuffPlayer : ModPlayer
    {
        // Flag checking when life regen debuff should be activated
        // ��־����ʱӦ���������ָ�debuff
        public bool lifeRegenDebuff;

        public override void ResetEffects()
        {
            lifeRegenDebuff = false;
        }

        // Allows you to give the player a negative life regeneration based on its state (for Analysis, the "On Fire!" debuff makes the player take damage-over-time)
        // This is typically done by setting player.lifeRegen to 0 if it is positive, setting player.lifeRegenTime to 0, and subtracting a number from player.lifeRegen
        // The player will take damage at a rate of half the number you subtract per second

        // ������������״̬�����磬���Ż𣡡� debuff��ʹ��Ҿ��и���������������
        // ͨ��ͨ����player.lifeRegen����Ϊ0�������������������player.lifeRegenTime����Ϊ0������player.lifeRegen�м�ȥһ�����������
        // ���ÿ���ܵ���ȥ�����ֵ�һ���˺���
        public override void UpdateBadLifeRegen()
        {
            if (lifeRegenDebuff)
            {
                // These lines zero out any positive lifeRegen. This is expected for all bad life regeneration effects
                // ��Щ������κ�����lifeRegen�����в�������������ЧӦ����Ҫ��������
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;
                // Player.lifeRegenTime uses to increase the speed at which the player reaches its maximum natural life regeneration
                // So we set it to 0, and while this debuff is active, it never reaches it

                // Player.lifeRegenTime���ڼӿ���Ҵﵽ�����Ȼ�����ָ��ٶȵ��ٶ�
                // �������ǽ�������Ϊ0�������ڴ�debuff���ڻ״̬ʱ������Զ����ﵽ���ֵ
                Player.lifeRegenTime = 0;
                // lifeRegen is measured in 1/2 life per second. Therefore, this effect causes 8 life lost per second
                // lifeRegen��ÿ��1/2������������ˣ���Ч������ÿ����ʧ8��������
                Player.lifeRegen -= 16;
            }
        }
    }
}
