using AnalysisMod.AnalysisCommon.Configs;
using AnalysisMod.AnalysisCommon.GlobalItems;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

//Related to GlobalItem: WeaponWithGrowingDamage
//与GlobalItem相关：WeaponWithGrowingDamage
namespace AnalysisMod.AnalysisCommon.GlobalProjectiles
{
    public class ProjectileWithGrowingDamage : GlobalProjectile
    {
        private WeaponWithGrowingDamage sourceGlobalItem;

        public override bool InstancePerEntity => true;

        public override bool IsLoadingEnabled(Mod mod)
        {
            // To experiment with this Analysis, you'll need to enable it in the config.
            //要尝试此分析，您需要在配置中启用它。
            return ModContent.GetInstance<AnalysisModConfig>().WeaponWithGrowingDamageToggle;
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            //Don't try to store the itemSource.Item.  Terraria can re-use an item instance with SetDefaults(),
            //meaning the instance you save could become air or another item.  It is much safer to store the GlobalItem instance.

            //不要尝试存储itemSource.Item。Terraria可以使用SetDefaults（）重复使用项目实例，
            //这意味着您保存的实例可能会变成空气或其他物品。更安全的方法是存储GlobalItem实例。
            if (source is IEntitySource_WithStatsFromItem itemSource)
            {
                itemSource.Item.TryGetGlobalItem(out sourceGlobalItem);
            }
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (sourceGlobalItem == null)
            {
                return;
            }

            int owner = projectile.owner;
            if (owner < 0 || owner >= Main.player.Length)
            {
                return;
            }

            Player player = Main.player[owner];
            sourceGlobalItem.OnHitNPCGeneral(player, target, hit, projectile: projectile);
        }
    }
}
