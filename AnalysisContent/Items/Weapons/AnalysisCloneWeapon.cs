using AnalysisMod.AnalysisContent.Projectiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Weapons
{
    /// <summary>
    /// This weapon and its corresponding projectile showcase the CloneDefaults() method, which allows for cloning of other items.
    /// For this Analysis, we shall copy the Meowmere and its projectiles with the CloneDefaults() method, while also changing them slightly.
    /// For a more detailed description of each item field used here, check out <see cref="AnalysisTwoSword" />.<br/>
    /// ������������Ӧ�ĵ�ҩչʾ��CloneDefaults()�����������¡������Ʒ��
    /// �ڱ������У����ǽ�ʹ��CloneDefaults()��������Meowmere�����ĵ�ҩ������΢�޸����ǡ�
    /// �йش˴�ʹ�õ�ÿ����Ŀ�ֶεĸ���ϸ��������鿴<see cref="AnalysisTwoSword" />��
    /// </summary>
    public class AnalysisCloneWeapon : ModItem
    {
        public override void SetDefaults()
        {
            // This method right here is the backbone of what we're doing here; by using this method, we copy all of
            // the meowmere's SetDefault stats (such as Item.melee and Item.shoot) on to our item, so we don't have to
            // go into the source and copy the stats ourselves. It saves a lot of time and looks much cleaner; if you're
            // going to copy the stats of an item, use CloneDefaults().

            // ����ķ���������������һ�й�����֧����ͨ��ʹ�ô˷��������ǽ�����Meowmere SetDefaultͳ����Ϣ������Item.melee��Item.shoot�����Ƶ����ǵ���Ʒ�ϣ�
            // ��˲��ؽ���Դ���벢�Լ�����ͳ����Ϣ�����ʡ�˺ܶ�ʱ�䣬���ҿ��������������������Ҫ����һ����Ŀ��ͳ����Ϣ����ʹ��CloneDefaults()��

            Item.CloneDefaults(ItemID.Meowmere);

            // After CloneDefaults has been called, we can now modify the stats to our wishes, or keep them as they are.
            // For the sake of Analysis, let's swap the vanilla Meowmere projectile shot from our item for our own projectile by changing Item.shoot:

            // ����CloneDefaults֮�����ڿ��Ը�����Ҫ�޸�ͳ�����ݻ򱣳�ԭ����
            // Ϊ�˽��з�����������ͨ���ı�Item.shoot���������Լ�����һ����ҩ�滻Vanilla Meowmere projectile shot��

            Item.shoot = ModContent.ProjectileType<AnalysisCloneProjectile>(); // Remember that we must use ProjectileType<>() since it is a modded projectile!
                                                                               // ���ס����������modded projectile, ���Ǳ���ʹ��ProjectileType <>()��

                                                                               // Check out AnalysisCloneProjectile to see how this projectile is different from the Vanilla Meowmere projectile.
                                                                               // �鿴AnalysisCloneProjectile�Բ鿴��projectile��Vanilla Meowmere projectile�кβ�ͬ��

            // While we're at it, let's make our weapon's stats a bit stronger than the Meowmere, which can be done
            // by using math on each given stat.

            // ˳��˵һ�£�������ʹ����������Meowmere����ǿ��
            // ��ÿ������״̬�϶����Խ�����ѧ���㡣

            Item.damage *= 2; // Makes this weapon's damage double the Meowmere's damage.
                              // ʹ���������������Meowmere���˺���

            Item.shootSpeed *= 1.25f; // Makes this weapon's projectiles shoot 25% faster than the Meowmere's projectiles.
                                      // ʹ����������ĵ�ҩ��Meowmere�ĵ�ҩ��25%��
        }

        // Please see AnalysisContent/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AnalysisItem>()
                .AddTile<Tiles.Furniture.AnalysisWorkbench>()
                .Register();
        }
    }
}
