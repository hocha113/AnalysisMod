using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Buffs
{
    public class AnalysisWhipDebuff : ModBuff
    {
        public static readonly int TagDamage = 5;

        public override void SetStaticDefaults()
        {
            // This allows the debuff to be inflicted on NPCs that would otherwise be immune to all debuffs.
            // Other mods may check it for different purposes.

            //这允许对那些本来免疫所有debuff的NPC造成debuff。
            //其他模组可能会出于不同目的检查它。
            BuffID.Sets.IsAnNPCWhipDebuff[Type] = true;
        }
    }

    public class AnalysisWhipAdvancedDebuff : ModBuff
    {
        public static readonly int TagDamagePercent = 30;
        public static readonly float TagDamageMultiplier = TagDamagePercent / 100f;

        public override void SetStaticDefaults()
        {
            BuffID.Sets.IsAnNPCWhipDebuff[Type] = true;
        }
    }

    public class AnalysisWhipDebuffNPC : GlobalNPC
    {
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            // Only player attacks should benefit from this buff, hence the NPC and trap checks.
            //只有玩家攻击应该从此增益中受益，因此需要进行NPC和陷阱检查。
            if (projectile.npcProj || projectile.trap || !projectile.IsMinionOrSentryRelated)
                return;


            // SummonTagDamageMultiplier scales down tag damage for some specific minion and sentry projectiles for balance purposes.
            //SummonTagDamageMultiplier为某些特定仆从和哨兵弹幕缩小标签伤害以实现平衡。
            var projTagMultiplier = ProjectileID.Sets.SummonTagDamageMultiplier[projectile.type];

            if (npc.HasBuff<AnalysisWhipDebuff>())
            {
                // Apply a flat bonus to every hit
                //对每次打击施加固定奖励
                modifiers.FlatBonusDamage += AnalysisWhipDebuff.TagDamage * projTagMultiplier;
            }

            // if you have a lot of buffs in your mod, it might be faster to loop over the NPC.buffType and buffTime arrays once, and track the buffs you find, rather than calling HasBuff many times
            //如果你在mod中有很多增益效果，最好遍历一次NPC.buffType和buffTime数组并跟踪找到的增益效果，而不是多次调用HasBuff方法
            if (npc.HasBuff<AnalysisWhipAdvancedDebuff>())
            {
                // Apply the scaling bonus to the next hit, and then remove the buff, like the vanilla firecracker
                //将缩放奖励应用于下一次打击，然后移除增益效果，就像香草火鞭炮
                modifiers.ScalingBonusDamage += AnalysisWhipAdvancedDebuff.TagDamageMultiplier * projTagMultiplier;
                npc.RequestBuffRemoval(ModContent.BuffType<AnalysisWhipAdvancedDebuff>());
            }
        }
    }
}
