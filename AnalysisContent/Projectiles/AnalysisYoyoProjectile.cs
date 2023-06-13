using AnalysisMod.AnalysisContent.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Projectiles
{
    public class AnalysisYoyoProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // The following sets are only applicable to yoyo that use aiStyle 99.
            //以下设置仅适用于使用aiStyle 99的溜溜球

            // YoyosLifeTimeMultiplier is how long in seconds the yoyo will stay out before automatically returning to the player. 
            // Vanilla values range from 3f (Wood) to 16f (Chik), and defaults to -1f. Leaving as -1 will make the time infinite.

            // YoyosLifeTimeMultiplier是溜溜球在自动返回玩家之前停留的时间（以秒为单位）。
            //香草值范围从3f（木头）到16f（Chik），默认为-1f。保持为-1将使时间无限。
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 3.5f;

            // YoyosMaximumRange is the maximum distance the yoyo sleep away from the player. 
            // Vanilla values range from 130f (Wood) to 400f (Terrarian), and defaults to 200f.

            // YoyosMaximumRange是溜溜球离玩家最远的距离。
            //香草值范围从130f（木材）到400f（Terrarian），默认为200f。
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 300f;

            // YoyosTopSpeed is top speed of the yoyo Projectile.
            // Vanilla values range from 9f (Wood) to 17.5f (Terrarian), and defaults to 10f.

            // YoyosTopSpeed是溜溜球弹丸的最高速度。
            //香草值范围从9f（木头）到17.5f（Terrarian），默认为10f。
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 13f;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16; // The width of the projectile's hitbox.
            Projectile.height = 16; // The height of the projectile's hitbox.

            Projectile.aiStyle = ProjAIStyleID.Yoyo; // The projectile's ai style. Yoyos use aiStyle 99 (ProjAIStyleID.Yoyo). A lot of yoyo code checks for this aiStyle to work properly.
                                                     // 弹丸的ai样式。 溜溜球使用aiStyle 99 (ProjAIStyleID.Yoyo)。 许多悠悠代码检查此aiStyle以正常工作。

            Projectile.friendly = true; // Player shot projectile. Does damage to enemies but not to friendly Town NPCs.
                                        // 玩家发射弹丸。 对敌人造成伤害，但不对友好城镇NPC造成伤害。

            Projectile.DamageType = DamageClass.MeleeNoSpeed; // Benefits from melee bonuses. MeleeNoSpeed means the item will not scale with attack speed.
                                                              // 受益于近战奖励。 MeleeNoSpeed表示该物品不会随攻击速度缩放。

            Projectile.penetrate = -1; // All vanilla yoyos have infinite penetration. The number of enemies the yoyo can hit before being pulled back in is based on YoyosLifeTimeMultiplier.
                                       // 所有香草味道都具有无限穿透力。 溜铁棒可以打中多少个敌人，然后被拉回取决于YoyosLifeTimeMultiplier。

            // Projectile.scale = 1f; // The scale of the projectile. Most yoyos are 1f, but a few are larger. The Kraken is the largest at 1.2f
                                      // 弹丸的比例。 大多数溜溜球都是1f，但有几个更大。 猎鲸鱼是最大的，为1.2f
        }

        // notes for aiStyle 99: 
        // localAI[0] is used for timing up to YoyosLifeTimeMultiplier
        // localAI[1] can be used freely by specific types
        // ai[0] and ai[1] usually point towards the x and y world coordinate hover point
        // ai[0] is -1f once YoyosLifeTimeMultiplier is reached, when the player is stoned/frozen, when the yoyo is too far away, or the player is no longer clicking the shoot button.
        // ai[0] being negative makes the yoyo move back towards the player
        // Any AI method can be used for dust, spawning projectiles, etc specific to your yoyo.

        // aiStyle 99的注释：
        // localAI [0]用于计时达到YoyosLifeTimeMultiplier
        // localAI [1]可以由特定类型自由使用
        // ai [0]和ai [1]通常指向x和y世界坐标悬停点
        //一旦达到YoyosLifeTimeMultiplier，当玩家被石化/冻结时，溜铁棒太远或玩家不再点击射击按钮时，ai[0]为-1f。
        // ai[0]为负使溜铁棒向玩家移动回来
        //任何AI方法都可以用于粉尘、生成弹丸等与您的悠悠具体相关的内容。

        public override void PostAI()
        {
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Sparkle>()); // Makes the projectile emit dust.
                                                                                                                        // 使弹丸发出灰尘。
            }
        }
    }
}
