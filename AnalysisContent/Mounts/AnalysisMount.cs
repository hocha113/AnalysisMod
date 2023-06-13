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
    // ���������һ�������ӵ���������Ϊ�����ڶ������������������3������
    public class AnalysisMount : ModMount
    {
        // Since only a single instance of ModMountData ever exists, we can use player.mount._mountSpecificData to store additional data related to a specific mount.
        // Using something like this for gameplay effects would require ModPlayer syncing, but this Analysis is purely visual.

        // ����ModMountDataֻ����һ��ʵ�������ǿ���ʹ��player.mount._mountSpecificData���洢���ض�������صĸ������ݡ�
        // ������ϷЧ��֮��Ķ�����ҪModPlayerͬ��������������������Ӿ��ϵġ�
        protected class CarSpecificData
        {
            internal static float[] offsets = new float[] { 0, 14, -14 };

            internal int count; // Tracks how many balloons are still left.
                                // ����ʣ������������

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
            // �ƶ�
            MountData.jumpHeight = 5; // How high the mount can jump.
                                      // �����ܹ�����ߡ�

            MountData.acceleration = 0.19f; // The rate at which the mount speeds up.
                                            // ������ٶȡ�

            MountData.jumpSpeed = 4f; // The rate at which the player and mount ascend towards (negative y velocity) the jump height when the jump button is presssed.
                                      // ��������Ծ��ťʱ����Һ������������𣨸�y�ٶȣ�������Ծ�߶ȵ����ʡ�

            MountData.blockExtraJumps = false; // Determines whether or not you can use a double jump (like cloud in a bottle) while in the mount.
                                               // ȷ���Ƿ�����ڼ�ʻ��ʹ��˫����������ƿ����

            MountData.constantJump = true; // Allows you to hold the jump button down.
                                           // �����㰴ס��Ծ��ť�����֡�

            MountData.heightBoost = 20; // Height between the mount and the ground
                                        // ����͵���֮��ĸ߶�

            MountData.fallDamage = 0.5f; // Fall damage multiplier.
                                         // �����˺�����.

            MountData.runSpeed = 11f; // The speed of the mount
                                      // �����ƶ��ٶ�

            MountData.dashSpeed = 8f; // The speed the mount moves when in the state of dashing.
                                      // ���ڳ��״̬ʱ�������ƶ����ٶ�.

            MountData.flightTimeMax = 0; // The amount of time in frames a mount can be in the state of flying.
                                         // ���ﴦ�ڷ���״̬�¿ɳ���ʱ�䣨��֡���㣩.

            // Misc
            // ����
            MountData.fatigueMax = 0;
            MountData.buff = ModContent.BuffType<Buffs.AnalysisMountBuff>(); // The ID number of the buff assigned to the mount.
                                                                             // ָ�������������仺����ID����.

            // Effects
            // Ч��
            MountData.spawnDust = ModContent.DustType<Dusts.Sparkle>(); // The ID of the dust spawned when mounted or dismounted.
                                                                        // ��װ��ж��ʱ�����ҳ���ID��

            // Frame data and player offsets
            // ֡���ݺ����ƫ����
            MountData.totalFrames = 4; // Amount of animation frames for the mount
                                       // ���ﶯ��֡��

            MountData.playerYOffsets = Enumerable.Repeat(20, MountData.totalFrames).ToArray(); // Fills an array with values for less repeating code
                                                                                               // ������������Լ����ظ������ֵ
            MountData.xOffset = 13;
            MountData.yOffset = -12;
            MountData.playerHeadOffset = 22;
            MountData.bodyFrame = 3;

            // Standing
            // վ��״̬
            MountData.standingFrameCount = 4;
            MountData.standingFrameDelay = 12;
            MountData.standingFrameStart = 0;

            // Running
            // �ܲ�״̬
            MountData.runningFrameCount = 4;
            MountData.runningFrameDelay = 12;
            MountData.runningFrameStart = 0;

            // Flying
            // ����״̬
            MountData.flyingFrameCount = 0;
            MountData.flyingFrameDelay = 0;
            MountData.flyingFrameStart = 0;

            // In-air
            // �ڿ���ʱ��״̬
            MountData.inAirFrameCount = 1;
            MountData.inAirFrameDelay = 12;
            MountData.inAirFrameStart = 0;

            // Idle
            // ����״̬
            MountData.idleFrameCount = 4;
            MountData.idleFrameDelay = 12;
            MountData.idleFrameStart = 0;
            MountData.idleFrameLoop = true;

            // Swim
            // ��Ӿ
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
            // ��δ���ģ������ķ��衣
            var balloons = (CarSpecificData)player.mount._mountSpecificData;
            float ballonMovementScale = 0.05f;

            for (int i = 0; i < balloons.count; i++)
            {
                ref float rotation = ref balloons.rotations[i]; // This is a reference variable. It's set to point directly to the 'i' index in the rotations array, so it works like an alias here.
                                                                // ����һ�����ñ�������������Ϊֱ��ָ����ת�����еġ�i������������������𵽱��������á�

                if (Math.Abs(rotation) > MathHelper.PiOver2)
                    ballonMovementScale *= -1;

                rotation += -player.velocity.X * ballonMovementScale * Main.rand.NextFloat();
                rotation = rotation.AngleLerp(0, 0.05f);
            }

            // This code spawns some dust if we are moving fast enough.
            // ��������ƶ����㹻�죬��δ��������һЩ������
            if (Math.Abs(player.velocity.X) > 4f)
            {
                Rectangle rect = player.getRect();

                Dust.NewDust(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, ModContent.DustType<Dusts.Sparkle>());
            }
        }

        public override void SetMount(Player player, ref bool skipDust)
        {
            // When this mount is mounted, we initialize _mountSpecificData with a new CarSpecificData object which will track some extra visuals for the mount.
            // ����װ������ʱ������ʹ���µ�CarSpecificData�����ʼ��_mountSpecificData���ö��󽫸��������һЩ�����Ӿ�Ч����
            player.mount._mountSpecificData = new CarSpecificData();

            // This code bypasses the normal mount spawning dust and replaces it with our own visual.
            // �˴����ƹ���������������ҳ����滻Ϊ�����Լ����Ӿ�Ч����
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
            // ���������ṩ��ÿ��������������Draw������������Ǽ��drawType�Ա����ظ����ơ�
            if (drawType == 0)
            {
                // We draw some extra balloons before _Back texture
                // ������_Back����֮ǰ����һЩ��������
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
            // ͨ������true����Ȼ�ᷢ�������ͼ��
            return true;
        }
    }
}