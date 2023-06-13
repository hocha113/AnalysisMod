using AnalysisMod.AnalysisContent.Buffs;
using AnalysisMod.AnalysisContent.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisCommon.GlobalNPCs
{
    internal class DamageOverTimeGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public bool AnalysisJavelinDebuff;

        public override void ResetEffects(NPC npc)
        {
            AnalysisJavelinDebuff = false;
        }

        public override void SetDefaults(NPC npc)
        {
            // TODO: This doesn't work currently. tModLoader needs to make a fix to allow changing buffImmune
            // TODO: 目前这个功能无法使用。tModLoader需要进行修复，以允许更改buffImmune

            // We want our AnalysisJavelin buff to follow the same immunities as BoneJavelin
            // 我们希望我们的AnalysisJavelin buff遵循与BoneJavelin相同的免疫性
            npc.buffImmune[ModContent.BuffType<AnalysisJavelinDebuff>()] = npc.buffImmune[BuffID.BoneJavelin];
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (AnalysisJavelinDebuff)
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                // Count how many AnalysisJavelinProjectile are attached to this npc.
                // 计算此npc附加了多少个AnalysisJavelinProjectile。
                int AnalysisJavelinCount = 0;
                for (int i = 0; i < 1000; i++)
                {
                    Projectile p = Main.projectile[i];
                    if (p.active && p.type == ModContent.ProjectileType<AnalysisJavelinProjectile>() && p.ai[0] == 1f && p.ai[1] == npc.whoAmI)
                    {
                        AnalysisJavelinCount++;
                    }
                }
                // Remember, lifeRegen affects the actual life loss, damage is just the text.
                // 请记住，lifeRegen影响实际生命损失，而伤害只是文本。

                // The logic shown here matches how vanilla debuffs stack in terms of damage numbers shown and actual life loss.
                // 此处显示的逻辑与香草debuff堆栈中显示的伤害数字和实际生命损失匹配。
                npc.lifeRegen -= AnalysisJavelinCount * 2 * 3;
                if (damage < AnalysisJavelinCount * 3)
                {
                    damage = AnalysisJavelinCount * 3;
                }
            }
        }

    }
}
