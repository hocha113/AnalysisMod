using Terraria;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Dusts
{
    public class Sparkle : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.velocity *= 0.4f; // Multiply the dust's start velocity by 0.4, slowing it down
                                   // �������ĳ�ʼ�ٶȳ���0.4���������ٶȡ�

            dust.noGravity = true; // Makes the dust have no gravity.
                                   // ʹ����û��������

            dust.noLight = true; // Makes the dust emit no light.
                                 // ʹ�������������ߡ�

            dust.scale *= 1.5f; // Multiplies the dust's initial scale by 1.5.
                                // �������ĳ�ʼ��������1.5��
        }

        public override bool Update(Dust dust)
        { // Calls every frame the dust is active
            // ��ÿ��֡�е��û״̬�µĻҳ�
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
                          // ����false�Է�ֹԭʼ��Ϊ��
        }
    }
}
