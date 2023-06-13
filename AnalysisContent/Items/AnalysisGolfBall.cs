using AnalysisMod.AnalysisContent.Projectiles;
using Terraria;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items
{
    public class AnalysisGolfBall : ModItem
    {
        public override void SetDefaults()
        {
            // DefaultToGolfBall sets various properties common to golf balls. Hover over DefaultToGolfBall in Visual Studio to see the specific properties set.
            // ModContent.ProjectileType<AnalysisGolfBallProjectile>() is the projectile that is placed on the golf tee.

            // DefaultToGolfBall设置了高尔夫球常见的各种属性。在Visual Studio中将鼠标悬停在DefaultToGolfBall上，以查看设置的具体属性。
            // ModContent.ProjectileType<AnalysisGolfBallProjectile>()是放置在高尔夫球发射台上的弹丸。
            Item.DefaultToGolfBall(ModContent.ProjectileType<AnalysisGolfBallProjectile>());
        }
    }
}
