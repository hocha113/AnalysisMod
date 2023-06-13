using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Mounts
{
    // This mount is a car with wheels which behaves simillarly to the unicorn mount. The car has 3 baloons attached to the back.
    // 这个坐骑是一辆带轮子的汽车，行为类似于独角兽坐骑。汽车后面有3个气球。
    public class AnalysisMount : ModMount
    {
        // Since only a single instance of ModMountData ever exists, we can use player.mount._mountSpecificData to store additional data related to a specific mount.
        // Using something like this for gameplay effects would require ModPlayer syncing, but this Analysis is purely visual.

        // 由于ModMountData只存在一个实例，我们可以使用player.mount._mountSpecificData来存储与特定坐骑相关的附加数据。
        // 对于游戏效果之类的东西需要ModPlayer同步，但这个分析纯粹是视觉上的。
        protected class CarSpecificData
        {
            internal static float[] offsets = new float[] { 0, 14, -14 };

            internal int count; // Tracks how many balloons are still left.
                                // 跟踪剩余气球数量。

            internal float[] rotations;

            public CarSpecificData()
            {
                count = 3;
                rotations = new float[count];
            }
        }

        public override void SetStaticDefaults()
        {
            // Movement
            // 移动
            MountData.jumpHeight = 5; // How high the mount can jump.
                                      // 坐骑能够跳多高。

            MountData.acceleration = 0.19f; // The rate at which the mount speeds up.
                                            // 坐骑加速度。

            MountData.jumpSpeed = 4f; // The rate at which the player and mount ascend towards (negative y velocity) the jump height when the jump button is presssed.
                                      // 当按下跳跃按钮时，玩家和坐骑向上升起（负y速度）到达跳跃高度的速率。

            MountData.blockExtraJumps = false; // Determines whether or not you can use a double jump (like cloud in a bottle) while in the mount.
                                               // 确定是否可以在驾驶中使用双重跳（如云瓶）。

            MountData.constantJump = true; // Allows you to hold the jump button down.
                                           // 允许你按住跳跃按钮不放手。

            MountData.heightBoost = 20; // Height between the mount and the ground
                                        // 坐骑和地面之间的高度

            MountData.fallDamage = 0.5f; // Fall damage multiplier.
                                         // 掉落伤害乘数.

            MountData.runSpeed = 11f; // The speed of the mount
                                      // 坐骑移动速度

            MountData.dashSpeed = 8f; // The speed the mount moves when in the state of dashing.
                                      // 处于冲刺状态时，坐骑移动的速度.

            MountData.flightTimeMax = 0; // The amount of time in frames a mount can be in the state of flying.
                                         // 坐骑处于飞行状态下可持续时间（以帧计算）.

            // Misc
            // 杂项
            MountData.fatigueMax = 0;
            MountData.buff = ModContent.BuffType<Buffs.AnalysisMountBuff>(); // The ID number of the buff assigned to the mount.
                                                                             // 指定给该座机分配缓冲区ID号码.

            // Effects
            // 效果
            MountData.spawnDust = ModContent.DustType<Dusts.Sparkle>(); // The ID of the dust spawned when mounted or dismounted.
                                                                        // 安装或卸载时产生灰尘的ID。

            // Frame data and player offsets
            // 帧数据和玩家偏移量
            MountData.totalFrames = 4; // Amount of animation frames for the mount
                                       // 坐骑动画帧数

            MountData.playerYOffsets = Enumerable.Repeat(20, MountData.totalFrames).ToArray(); // Fills an array with values for less repeating code
                                                                                               // 用于填充数组以减少重复代码的值
            MountData.xOffset = 13;
            MountData.yOffset = -12;
            MountData.playerHeadOffset = 22;
            MountData.bodyFrame = 3;

            // Standing
            // 站立状态
            MountData.standingFrameCount = 4;
            MountData.standingFrameDelay = 12;
            MountData.standingFrameStart = 0;

            // Running
            // 跑步状态
            MountData.runningFrameCount = 4;
            MountData.runningFrameDelay = 12;
            MountData.runningFrameStart = 0;

            // Flying
            // 飞行状态
            MountData.flyingFrameCount = 0;
            MountData.flyingFrameDelay = 0;
            MountData.flyingFrameStart = 0;

            // In-air
            // 在空中时的状态
            MountData.inAirFrameCount = 1;
            MountData.inAirFrameDelay = 12;
            MountData.inAirFrameStart = 0;

            // Idle
            // 空闲状态
            MountData.idleFrameCount = 4;
            MountData.idleFrameDelay = 12;
            MountData.idleFrameStart = 0;
            MountData.idleFrameLoop = true;

            // Swim
            // 游泳
            MountData.swimFrameCount = MountData.inAirFrameCount;
            MountData.swimFrameDelay = MountData.inAirFrameDelay;
            MountData.swimFrameStart = MountData.inAirFrameStart;

            if (!Main.dedServ)
            {
                MountData.textureWidth = MountData.backTexture.Width() + 20;
                MountData.textureHeight = MountData.backTexture.Height();
            }
        }

        public override void UpdateEffects(Player player)
        {
            // This code simulates some wind resistance for the balloons.
            // 这段代码模拟气球的风阻。
            var balloons = (CarSpecificData)player.mount._mountSpecificData;
            float ballonMovementScale = 0.05f;

            for (int i = 0; i < balloons.count; i++)
            {
                ref float rotation = ref balloons.rotations[i]; // This is a reference variable. It's set to point directly to the 'i' index in the rotations array, so it works like an alias here.
                                                                // 这是一个引用变量。它被设置为直接指向旋转数组中的“i”索引，因此在这里起到别名的作用。

                if (Math.Abs(rotation) > MathHelper.PiOver2)
                    ballonMovementScale *= -1;

                rotation += -player.velocity.X * ballonMovementScale * Main.rand.NextFloat();
                rotation = rotation.AngleLerp(0, 0.05f);
            }

            // This code spawns some dust if we are moving fast enough.
            // 如果我们移动得足够快，这段代码会生成一些尘土。
            if (Math.Abs(player.velocity.X) > 4f)
            {
                Rectangle rect = player.getRect();

                Dust.NewDust(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, ModContent.DustType<Dusts.Sparkle>());
            }
        }

        public override void SetMount(Player player, ref bool skipDust)
        {
            // When this mount is mounted, we initialize _mountSpecificData with a new CarSpecificData object which will track some extra visuals for the mount.
            // 当安装此坐骑时，我们使用新的CarSpecificData对象初始化_mountSpecificData，该对象将跟踪坐骑的一些额外视觉效果。
            player.mount._mountSpecificData = new CarSpecificData();

            // This code bypasses the normal mount spawning dust and replaces it with our own visual.
            // 此代码绕过正常的坐骑产生灰尘并替换为我们自己的视觉效果。
            if (!Main.dedServ)
            {
                for (int i = 0; i < 16; i++)
                {
                    Dust.NewDustPerfect(player.Center + new Vector2(80, 0).RotatedBy(i * Math.PI * 2 / 16f), MountData.spawnDust);
                }

                skipDust = true;
            }
        }

        public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
        {
            // Draw is called for each mount texture we provide, so we check drawType to avoid duplicate draws.
            // 对于我们提供的每个坐骑纹理都调用Draw方法，因此我们检查drawType以避免重复绘制。
            if (drawType == 0)
            {
                // We draw some extra balloons before _Back texture
                // 我们在_Back纹理之前画了一些额外气球
                var balloons = (CarSpecificData)drawPlayer.mount._mountSpecificData;
                int timer = DateTime.Now.Millisecond % 800 / 200;
                Texture2D balloonTexture = Mod.Assets.Request<Texture2D>("AnalysisContent/Items/Armor/SimpleAccessory_Balloon").Value;

                for (int i = 0; i < balloons.count; i++)
                {
                    var position = drawPosition + new Vector2((-36 + CarSpecificData.offsets[i]) * drawPlayer.direction, 14);
                    var srcRect = new Rectangle(28, balloonTexture.Height / 4 * ((timer + i) % 4), 28, 42);
                    float drawRotation = rotation + balloons.rotations[i];
                    var origin = new Vector2(14 + drawPlayer.direction * 7, 42);

                    playerDrawData.Add(new DrawData(balloonTexture, position, srcRect, drawColor, drawRotation, origin, drawScale, spriteEffects ^ SpriteEffects.FlipHorizontally, 0));
                }
            }

            // by returning true, the regular drawing will still happen.
            // 通过返回true，仍然会发生常规绘图。
            return true;
        }
    }
}