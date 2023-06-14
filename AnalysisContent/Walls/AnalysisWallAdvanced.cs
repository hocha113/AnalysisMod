using AnalysisMod.AnalysisContent.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace AnalysisMod.AnalysisContent.Walls
{
    // This is a more advanced ModWall showing off animation, dynamic dust, light emitting, and simple custom framing logic
    // 这是一个更高级的 ModWall，展示了动画、动态灰尘、发光和简单的自定义框架逻辑
    public class AnalysisWallAdvanced : ModWall
	{
		public override void SetStaticDefaults() {
			Main.wallHouse[Type] = true;

			DustType = DustID.Stone;

			AddMapEntry(new Color(68, 68, 68));
		}

		public override bool CreateDust(int i, int j, ref int type) {
			type = DustType;
			if (Main.tile[i, j].WallFrameNumber == 0)
				type = DustID.GemEmerald;
			return true;
		}

		public override void NumDust(int i, int j, bool fail, ref int num) {
			num = fail ? 3 : 10;
		}

		public override void AnimateWall(ref byte frame, ref byte frameCounter) {
            // Loop through 2 frames of animation, changing every 5 game frames
            // 循环播放 2 帧动画，每 5 游戏帧更换一次
            if (++frameCounter >= 5) {
				frameCounter = 0;
				frame = (byte)(++frame % 2);
			}
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
			if (!Main.dayTime) {
				r = 0.1f;
				g = 0.5f;
				b = 0f;
			}
		}

		public override bool WallFrame(int i, int j, bool randomizeFrame, ref int style, ref int frameNumber) {
			if (randomizeFrame) {
                // Here we make the chance of WallFrameNumber 0 very rare, just for visual variety:
                // 在这里我们让 WallFrameNumber 0 的概率非常罕见，只是为了视觉上的多样性：
                // https://i.imgur.com/9Irak3p.png
                if (frameNumber == 0 && WorldGen.genRand.NextBool(3, 4)) {
					frameNumber = WorldGen.genRand.Next(1, 3);
				}
			}
			return base.WallFrame(i, j, randomizeFrame, ref style, ref frameNumber);
		}
	}
}