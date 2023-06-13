using Terraria;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Dusts
{
    public class Sparkle : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.velocity *= 0.4f; // Multiply the dust's start velocity by 0.4, slowing it down
                                   // 将尘埃的初始速度乘以0.4，减缓其速度。

            dust.noGravity = true; // Makes the dust have no gravity.
                                   // 使尘埃没有重力。

            dust.noLight = true; // Makes the dust emit no light.
                                 // 使尘埃不发出光线。

            dust.scale *= 1.5f; // Multiplies the dust's initial scale by 1.5.
                                // 将尘埃的初始比例乘以1.5。
        }

        public override bool Update(Dust dust)
        { // Calls every frame the dust is active
            // 在每个帧中调用活动状态下的灰尘
            dust.position += dust.velocity;
            dust.rotation += dust.velocity.X * 0.15f;
            dust.scale *= 0.99f;

            float light = 0.35f * dust.scale;

            Lighting.AddLight(dust.position, light, light, light);

            if (dust.scale < 0.5f)
            {
                dust.active = false;
            }

            return false; // Return false to prevent vanilla behavior.
                          // 返回false以防止原始行为。
        }
    }
}
