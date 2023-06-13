using AnalysisMod.AnalysisContent.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Projectiles
{
    // This Analysis show how to implement simple homing projectile
    // Can be tested with AnalysisCustomAmmoGun

    // 这个分析展示了如何实现简单的追踪弹
    // 可以使用 AnalysisCustomAmmoGun 进行测试
    public class AnalysisHomingProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
                                                                            // 将邪教徒设为对此弹有高额的伤害减免，因为它对所有追踪弹都有抵抗力。
        }

        // Setting the default parameters of the projectile
        // You can check most of Fields and Properties here https://github.com/tModLoader/tModLoader/wiki/Projectile-Class-Documentation

        // 设置弹幕的默认参数
        // 您可以在这里查看大多数字段和属性 https://github.com/tModLoader/tModLoader/wiki/Projectile-Class-Documentation
        public override void SetDefaults()
        {
            Projectile.width = 8; // The width of projectile hitbox
            Projectile.height = 8; // The height of projectile hitbox

            Projectile.aiStyle = 0; // The ai style of the projectile (0 means custom AI). For more please reference the source code of Terraria
                                    // 弹幕的 AI 风格（0 表示自定义 AI）。更多信息请参考 Terraria 的源代码。

            Projectile.DamageType = DamageClass.Ranged; // What type of damage does this projectile affect?
                                                        // 这种类型的伤害会影响到这个弹幕吗？

            Projectile.friendly = true; // Can the projectile deal damage to enemies?
                                        // 这个弹幕能够对敌人造成伤害吗？

            Projectile.hostile = false; // Can the projectile deal damage to the player?
                                        // 这个弹幕能够对玩家造成伤害吗？

            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
                                           // 水会影响该弹丸速度吗？

            Projectile.light = 1f; // How much light emit around the projectile
                                   // 该项目周围发出多少光？

            Projectile.tileCollide = false; // Can the projectile collide with tiles?
                                            // 该项目是否可以与瓷砖碰撞？

            Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
                                       // 该项目存在时间（60 = 1 秒，所以 600 是 10 秒）
        }

        // Custom AI
        // 自定义 AI
        public override void AI()
        {
            float maxDetectRadius = 400f; // The maximum radius at which a projectile can detect a target
                                          // 一个子弹可以检测目标的最大半径

            float projSpeed = 5f; // The speed at which the projectile moves towards the target
                                  // 子弹向目标移动时的速度

            // Trying to find NPC closest to the projectile
            // 尝试找到距离子弾最近的 NPC
            NPC closestNPC = FindClosestNPC(maxDetectRadius);
            if (closestNPC == null)
                return;

            // If found, change the velocity of the projectile and turn it in the direction of the target
            // Use the SafeNormalize extension method to avoid NaNs returned by Vector2.Normalize when the vector is zero

            // 如果找到，则改变子彈速度并将其转向目标方向。
            // 使用 SafeNormalize 扩展方法避免 Vector2.Normalize 返回 NaN（当矢量为零时）
            Projectile.velocity = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * projSpeed;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        // Finding the closest NPC to attack within maxDetectDistance range
        // If not found then returns null

        // 在 maxDetectDistance 范围内查找最近的 NPC 进行攻击
        // 如果未找到，则返回 null
        public NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            // 在距离检查中使用平方值将使我们跳过平方根计算，从而大大提高此方法的速度。
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs(max always 200)
            // 循环遍历所有 NPC（最多 200 个）
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC target = Main.npc[k];
                // Check if NPC able to be targeted. It means that NPC is
                // 1. active (alive)
                // 2. chaseable (e.g. not a cultist archer)
                // 3. max life bigger than 5 (e.g. not a critter)
                // 4. can take damage (e.g. moonlord core after all it's parts are downed)
                // 5. hostile (!friendly)
                // 6. not immortal (e.g. not a target dummy)

                // 检查是否可以瞄准 NPC。这意味着该 NPC 是
                // 1. 活动状态（存活）
                // 2. 可追踪 (例如不是邪教徒弓箭手)
                // 3. 最大生命值大于5（例如不是小动物）
                // 4. 可以受到伤害 (例如月球领主核心在其所有部分都被打败后)
                // 5. 敌对 (!friendly)
                // 6. 不是无敌的 (例如不是目标模型)
                if (target.CanBeChasedBy())
                {
                    // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                    // DistanceSquared 函数返回两点之间的平方距离，跳过了相对昂贵的平方根计算
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    // Check if it is within the radius
                    // 检查是否在半径范围内 
                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }
    }
}
