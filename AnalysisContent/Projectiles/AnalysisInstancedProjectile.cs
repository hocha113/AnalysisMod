using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Projectiles
{
    public class AnalysisInstancedProjectile : ModProjectile
    {
        private Color trailColor;

        public override void SetDefaults()
        {
            Projectile.width = 16; //The width of projectile hitbox
            Projectile.height = 16; //The height of projectile hitbox

            Projectile.aiStyle = 1; //The ai style of the projectile, please reference the source code of Terraria
                                    //弹幕的AI风格，请参考Terraria的源代码

            Projectile.friendly = true; //Can the projectile deal damage to enemies?
                                        //弹幕能对敌人造成伤害吗？

            Projectile.hostile = false; //Can the projectile deal damage to the player?
                                        //弹幕能对玩家造成伤害吗？

            Projectile.DamageType = DamageClass.Ranged; //Is the projectile shoot by a ranged weapon?
                                                        //弹幕是由远程武器发射的吗？

            Projectile.ignoreWater = true; //Does the projectile's speed be influenced by water?
                                           //水会影响弹幕速度吗？

            Projectile.tileCollide = true; //Can the projectile collide with tiles?
                                           //弹幕能与方块碰撞吗？

            AIType = ProjectileID.Bullet; //Act exactly like default Bullet
                                          //完全像默认子弹一样行动
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Dust.NewDustPerfect(Projectile.Center, DustID.TintableDust, newColor: trailColor);

            return false;
        }

        public override void OnSpawn(IEntitySource data)
        {
            trailColor = Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.7f); // Assign a random color on spawn
                                                                         // 生成时随机分配颜色
        }
    }
}