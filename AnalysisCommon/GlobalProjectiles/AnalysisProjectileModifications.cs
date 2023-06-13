using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using AnalysisMod.AnalysisCommon.GlobalNPCs;

namespace AnalysisMod.AnalysisCommon.GlobalProjectiles
{
    // Here is a class dedicated to showcasing projectile modifications
    // 这是一个专门展示弹道修改的课程
    public class AnalysisProjectileModifications : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool applyBuffOnHit;
        public bool sayTimesHitOnThirdHit;
        // These are set when the user specifies that they want a trail.
        // 当用户指定他们想要拖尾时，这些被设置。
        private Color trailColor;
        private bool trailActive;

        // Here, a method is provided for setting the above fields.
        // 在这里，提供了一种设置上述字段的方法。
        public void SetTrail(Color color)
        {
            trailColor = color;
            trailActive = true;
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (sayTimesHitOnThirdHit)
            {
                ProjectileModificationGlobalNPC globalNPC = target.GetGlobalNPC<ProjectileModificationGlobalNPC>();
                if (globalNPC.timesHitByModifiedProjectiles % 3 == 0)
                {
                    Main.NewText($"This NPC has been hit with a modified projectile {globalNPC.timesHitByModifiedProjectiles} times.");
                }
                target.GetGlobalNPC<ProjectileModificationGlobalNPC>().timesHitByModifiedProjectiles += 1;
            }

            if (applyBuffOnHit)
            {
                target.AddBuff(BuffID.OnFire, 50);
            }
        }

        public override void PostAI(Projectile projectile)
        {
            if (trailActive)
            {
                Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.TintableDustLighted, default, default, default, trailColor);
            }
        }
    }
}
