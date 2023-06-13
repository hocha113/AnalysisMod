using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Pets.AnalysisPet
{
    public class AnalysisPetProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            Main.projPet[Projectile.type] = true;

            // This code is needed to customize the vanity pet display in the player select screen. Quick explanation:
            // * It uses fluent API syntax, just like Recipe
            // * You start with ProjectileID.Sets.SimpleLoop, specifying the start and end frames aswell as the speed, and optionally if it should animate from the end after reaching the end, effectively "bouncing"
            // * To stop the animation if the player is not highlighted/is standing, as done by most grounded pets, add a .WhenNotSelected(0, 0) (you can customize it just like SimpleLoop)
            // * To set offset and direction, use .WithOffset(x, y) and .WithSpriteDirection(-1)
            // * To further customize the behavior and animation of the pet (as its AI does not run), you have access to a few vanilla presets in DelegateMethods.CharacterPreview to use via .WithCode(). You can also make your own, showcased in MinionBossPetProjectile

            // 这段代码用于自定义玩家选择界面中的宠物显示。简单解释如下：
            // * 它使用流畅API语法，就像Recipe一样
            // * 您可以从ProjectileID.Sets.SimpleLoop开始，指定起始和结束帧以及速度，并可选地指定是否在到达末尾后从末尾动画反弹。
            // * 要停止动画，如果玩家未被突出显示/站立，则添加.WhenNotSelected(0, 0)（您可以像SimpleLoop一样自定义它）
            // * 要设置偏移量和方向，请使用.WithOffset(x, y) 和 .WithSpriteDirection(-1)
            // * 要进一步自定义宠物的行为和动画（因为其AI不运行），您可以通过.DelegateMethods.CharacterPreview访问几个香草预设来使用.WithCode()。您还可以制作自己的预设，在MinionBossPetProjectile中展示
            ProjectileID.Sets.CharacterPreviewAnimations[Projectile.type] = ProjectileID.Sets.SimpleLoop(0, Main.projFrames[Projectile.type], 6)
                .WithOffset(-10, -20f)
                .WithSpriteDirection(-1)
                .WithCode(DelegateMethods.CharacterPreview.Float);
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.ZephyrFish); // Copy the stats of the Zephyr Fish
                                                               // 复制Zephyr Fish的统计数据

            AIType = ProjectileID.ZephyrFish; // Mimic as the Zephyr Fish during AI.
                                              // 在AI期间模仿Zephyr Fish。
        }

        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];

            player.zephyrfish = false; // Relic from AIType

            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            // Keep the projectile from disappearing as long as the player isn't dead and has the pet buff.
            // 只要玩家没有死亡并且拥有宠物Buff，就保持投射物不会消失。
            if (!player.dead && player.HasBuff(ModContent.BuffType<AnalysisPetBuff>()))
            {
                Projectile.timeLeft = 2;
            }
        }
    }
}
