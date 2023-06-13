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
    /// ����ʹ�� CloneDefaults() ��¡ԭ�� Meowmere ��Ļ���ࡣ
    /// ��ȷ���鿴 <see cref="AnalysisCloneWeapon" />�����ᷢ�������Ļ����������� Meowmere �Ŀ�¡�汾��
    /// </summary>
    public class AnalysisCloneProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            // This method right here is the backbone of what we're doing here; by using this method, we copy all of
            // the Meowmere Projectile's SetDefault stats (such as projectile.friendly and projectile.penetrate) on to our projectile,
            // so we don't have to go into the source and copy the stats ourselves. It saves a lot of time and looks much cleaner;
            // if you're going to copy the stats of a projectile, use CloneDefaults().

            // �����������������������һ�й�����֧����ͨ��ʹ�ô˷��������ǽ�����
            // Meowmere Projectile �� SetDefault ͳ�����ݣ����� projectile.friendly �� projectile.penetrate�����Ƶ����ǵĵ�Ļ�ϣ�
            // ������ǲ��ؽ���Դ���벢�Լ�����ͳ�����ݡ����ʡ�˺ܶ�ʱ�䣬���ҿ��������Ӽ�ࣻ
            // ���Ҫ���Ƶ�Ļ��ͳ�����ݣ���ʹ�� CloneDefaults()��

            Projectile.CloneDefaults(ProjectileID.Meowmere);

            // To further the Cloning process, we can also copy the ai of any given projectile using AIType, since we want
            // the projectile to essentially behave the same way as the vanilla projectile.

            // Ϊ�˽�һ�����п�¡���̣����ǻ�����ʹ�� AIType �����κθ�����Ļ�� ai����Ϊ����ϣ��
            // ��Ļ��������ԭʼ��Ļ��Ϊ��ͬ��
            AIType = ProjectileID.Meowmere;

            // After CloneDefaults has been called, we can now modify the stats to our wishes, or keep them as they are.
            // For the sake of Analysis, lets make our projectile penetrate enemies a few more times than the vanilla projectile.
            // This can be done by modifying projectile.penetrate

            // �ڵ��� CloneDefaults �����ڿ��Ը�����Ҫ�޸�ͳ�����ݻ򱣳��䲻�䡣
            // ���ڷ���Ŀ�ģ�������ʹ�øõ�Ļ��ԭ���ܹ���͸���˸���Ρ�
            // �����ͨ���޸� projectile.penetrate ��ʵ��
            Projectile.penetrate += 3;
        }

        // While there are several different ways to change how our projectile could behave differently, lets make it so
        // when our projectile finally dies, it will explode into 4 regular Meowmere projectiles.

        // ��Ȼ�м��ֲ�ͬ��ʽ���Ըı����ʹ�øõ�Ļ���ֳ���ͬ��Ϊ��
        // ������ʹ�õ��õ�Ļ��������ʱ�����ᱬը�� 4 ����ͨ�� Meowmere ��Ļ��
        public override void Kill(int timeLeft)
        {
            Vector2 launchVelocity = new Vector2(-4, 0); // Create a velocity moving the left.
                                                         // ����һ�������ƶ����ٶȡ�

            for (int i = 0; i < 4; i++)
            {
                // Every iteration, rotate the newly spawned projectile by the equivalent 1/4th of a circle (MathHelper.PiOver4)
                // (Remember that all rotation in Terraria is based on Radians, NOT Degrees!)

                // ÿ�ε������������ɵĵ�Ļ��ת�൱��1/4Բ��MathHelper.PiOver4��
                // �����ס���� Terraria ��������ת���ǻ��ڻ��ȶ����ǽǶȣ���
                launchVelocity = launchVelocity.RotatedBy(MathHelper.PiOver4);

                // Spawn a new projectile with the newly rotated velocity, belonging to the original projectile owner. The new projectile will inherit the spawning source of this projectile.
                // ʹ������ת���ٶ�����һ���µĵ�Ļ������ԭʼ��Ļ�����ߡ� �����ɵĵ�Ļ���̳д˵�Ļ����Դ��
                Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, launchVelocity, ProjectileID.Meowmere, Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
            }
        }

        // Now, using CloneDefaults() and aiType doesn't copy EVERY aspect of the projectile. In Vanilla, several other methods
        // are used to generate different effects that aren't included in AI. For the case of the Meowmete projectile, since the
        // richochet sound is not included in the AI,
        // we must add it ourselves:

        // ���ڣ�ʹ�� CloneDefaults() �� aiType �����ܸ���ÿ�����档
        // �� Vanilla �У���ʹ���˼�����������������δ������ AI �еĲ�ͬЧ����
        // ���� Meowmete ��Ļ���ԣ����� richochet ����δ������ AI �У�
        // ���Ǳ����Լ��������
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Since there are two Richochet sounds for the Meowmere, we can randomly choose between them like this:
            // ���� Meowmere ������ Richochet ���������ǿ������������ѡ������֮һ��

            SoundEngine.PlaySound(Main.rand.NextBool() ? SoundID.Item57 : SoundID.Item58, Projectile.position);

            // Essentially, using ? and : is a glorified and shortened method of creating a simple if statement in
            // a single line. If Main.rand.NextBool() reurns true, it plays SoundID.Item57. If it returns false, then it
            // will play SoundID.Item58. The condition goes before the ? and the two possibilities follow, separated by a :

            // ʵ���ϣ��������͡������Ǵ������м� if ����һ�ִ��������̷�����
            // ��� Main.rand.NextBool() ���� true���򲥷� SoundID.Item57��������� false����
            // �������� SoundID.Item58������λ�� ? ��ǰ�棬���Һ���������������ԣ��� : �ָ���

            // This line calls the base (empty) implementation of this hook method to return its default value, which in its case is always 'true'.
            // Hover on the method below in VS to see its summary.

            // ���е��ô˹��ӷ����Ļ������գ�ʵ���Է�����Ĭ��ֵ�����������ʼ��Ϊ��true����
            // �� VS ����ͣ������ķ������Բ鿴��ժҪ��
            return base.OnTileCollide(oldVelocity);
        }
    }
}
