using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Dusts
{
    public class AnalysisBubble : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.frame = new Rectangle(0, 0, 30, 30);
            // If our texture had 3 different dust on top of each other (a 30x90 pixel image), we might do this:
            // 如果我们的纹理上有三种不同的灰尘叠在一起（一个30x90像素的图像），我们可以这样做：

            // dust.frame = new Rectangle(0, Main.rand.Next(3) * 30, 30, 30);
        }

        public override bool Update(Dust dust)
        {
            // Move the dust based on its velocity and reduce its size to then remove it, as the 'return false;' at the end will prevent vanilla logic.
            // 根据其速度移动灰尘并减小其大小，然后将其删除，因为最后的'return false;'将阻止香草逻辑。
            dust.position += dust.velocity;
            dust.scale -= 0.01f;

            if (dust.scale < 0.75f)
                dust.active = false;

            return false;
        }
    }
}
