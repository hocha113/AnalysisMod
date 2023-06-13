using AnalysisMod.AnalysisCommon.GlobalProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Weapons
{
    public class AnalysisModifiedProjectilesItem : ModItem
    {
        public override string Texture => "AnalysisMod/AnalysisContent/Items/Weapons/AnalysisShootingSword";

        public override void SetDefaults()
        {
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.damage = 20;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 32;
            Item.height = 32;
            Item.shoot = ProjectileID.PurificationPowder;
            // This Ammo is nonspecific. I want to modify what it shoots, however.
            // 这种弹药是非特定的。我想修改它射出的东西。
            Item.useAmmo = AmmoID.Bullet;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // NewProjectile returns the index of the projectile it creates in the NewProjectile array.
            // Here we are using it to gain access to the projectile object.

            // NewProjectile 返回它在 NewProjectile 数组中创建的弹丸对象的索引。
            // 在这里，我们使用它来访问弹丸对象。
            int projectileID = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            Projectile projectile = Main.projectile[projectileID];

            AnalysisProjectileModifications globalProjectile = projectile.GetGlobalProjectile<AnalysisProjectileModifications>();
            // For more context, see AnalysisProjectileModifications.cs
            // 更多上下文，请参见 AnalysisProjectileModifications.cs
            globalProjectile.SetTrail(Color.Green);
            globalProjectile.sayTimesHitOnThirdHit = true;
            globalProjectile.applyBuffOnHit = true;

            // We do not want vanilla to spawn a duplicate projectile.
            // 我们不希望原版生成重复的弹丸。
            return false;
        }
    }
}
