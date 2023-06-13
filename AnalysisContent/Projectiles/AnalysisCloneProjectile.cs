using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AnalysisMod.AnalysisContent.Items.Weapons;
using Terraria.Audio;

namespace AnalysisMod.AnalysisContent.Projectiles
{
    /// <summary>
    /// This the class that clones the vanilla Meowmere projectile using CloneDefaults().
    /// Make sure to check out <see cref="AnalysisCloneWeapon" />, which fires this projectile; it itself is a cloned version of the Meowmere.<br/>
    /// 这是使用 CloneDefaults() 克隆原版 Meowmere 弹幕的类。
    /// 请确保查看 <see cref="AnalysisCloneWeapon" />，它会发射这个弹幕；它本身就是 Meowmere 的克隆版本。
    /// </summary>
    public class AnalysisCloneProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            // This method right here is the backbone of what we're doing here; by using this method, we copy all of
            // the Meowmere Projectile's SetDefault stats (such as projectile.friendly and projectile.penetrate) on to our projectile,
            // so we don't have to go into the source and copy the stats ourselves. It saves a lot of time and looks much cleaner;
            // if you're going to copy the stats of a projectile, use CloneDefaults().

            // 下面这个方法是我们所做的一切工作的支柱。通过使用此方法，我们将所有
            // Meowmere Projectile 的 SetDefault 统计数据（例如 projectile.friendly 和 projectile.penetrate）复制到我们的弹幕上，
            // 因此我们不必进入源代码并自己复制统计数据。这节省了很多时间，并且看起来更加简洁；
            // 如果要复制弹幕的统计数据，请使用 CloneDefaults()。

            Projectile.CloneDefaults(ProjectileID.Meowmere);

            // To further the Cloning process, we can also copy the ai of any given projectile using AIType, since we want
            // the projectile to essentially behave the same way as the vanilla projectile.

            // 为了进一步进行克隆过程，我们还可以使用 AIType 复制任何给定弹幕的 ai，因为我们希望
            // 弹幕基本上与原始弹幕行为相同。
            AIType = ProjectileID.Meowmere;

            // After CloneDefaults has been called, we can now modify the stats to our wishes, or keep them as they are.
            // For the sake of Analysis, lets make our projectile penetrate enemies a few more times than the vanilla projectile.
            // This can be done by modifying projectile.penetrate

            // 在调用 CloneDefaults 后，现在可以根据需要修改统计数据或保持其不变。
            // 出于分析目的，让我们使得该弹幕比原版能够穿透敌人更多次。
            // 这可以通过修改 projectile.penetrate 来实现
            Projectile.penetrate += 3;
        }

        // While there are several different ways to change how our projectile could behave differently, lets make it so
        // when our projectile finally dies, it will explode into 4 regular Meowmere projectiles.

        // 虽然有几种不同方式可以改变如何使得该弹幕表现出不同行为，
        // 让我们使得当该弹幕最终死亡时，它会爆炸成 4 个普通的 Meowmere 弹幕。
        public override void Kill(int timeLeft)
        {
            Vector2 launchVelocity = new Vector2(-4, 0); // Create a velocity moving the left.
                                                         // 创建一个向左移动的速度。

            for (int i = 0; i < 4; i++)
            {
                // Every iteration, rotate the newly spawned projectile by the equivalent 1/4th of a circle (MathHelper.PiOver4)
                // (Remember that all rotation in Terraria is based on Radians, NOT Degrees!)

                // 每次迭代，将新生成的弹幕旋转相当于1/4圆（MathHelper.PiOver4）
                // （请记住，在 Terraria 中所有旋转都是基于弧度而不是角度！）
                launchVelocity = launchVelocity.RotatedBy(MathHelper.PiOver4);

                // Spawn a new projectile with the newly rotated velocity, belonging to the original projectile owner. The new projectile will inherit the spawning source of this projectile.
                // 使用新旋转的速度生成一个新的弹幕，属于原始弹幕所有者。 新生成的弹幕将继承此弹幕产生源。
                Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, launchVelocity, ProjectileID.Meowmere, Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
            }
        }

        // Now, using CloneDefaults() and aiType doesn't copy EVERY aspect of the projectile. In Vanilla, several other methods
        // are used to generate different effects that aren't included in AI. For the case of the Meowmete projectile, since the
        // richochet sound is not included in the AI,
        // we must add it ourselves:

        // 现在，使用 CloneDefaults() 和 aiType 并不能复制每个方面。
        // 在 Vanilla 中，还使用了几种其他方法来生成未包含在 AI 中的不同效果。
        // 对于 Meowmete 弹幕而言，由于 richochet 声音未包含在 AI 中，
        // 我们必须自己添加它：
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Since there are two Richochet sounds for the Meowmere, we can randomly choose between them like this:
            // 由于 Meowmere 有两个 Richochet 声音，我们可以像这样随机选择其中之一：

            SoundEngine.PlaySound(Main.rand.NextBool() ? SoundID.Item57 : SoundID.Item58, Projectile.position);

            // Essentially, using ? and : is a glorified and shortened method of creating a simple if statement in
            // a single line. If Main.rand.NextBool() reurns true, it plays SoundID.Item57. If it returns false, then it
            // will play SoundID.Item58. The condition goes before the ? and the two possibilities follow, separated by a :

            // 实际上，“？”和“：”是创建单行简单 if 语句的一种创建且缩短方法。
            // 如果 Main.rand.NextBool() 返回 true，则播放 SoundID.Item57。如果返回 false，则
            // 它将播放 SoundID.Item58。条件位于 ? 的前面，并且后面跟着两个可能性，由 : 分隔。

            // This line calls the base (empty) implementation of this hook method to return its default value, which in its case is always 'true'.
            // Hover on the method below in VS to see its summary.

            // 此行调用此钩子方法的基本（空）实现以返回其默认值，在其情况下始终为“true”。
            // 在 VS 中悬停在下面的方法上以查看其摘要。
            return base.OnTileCollide(oldVelocity);
        }
    }
}
