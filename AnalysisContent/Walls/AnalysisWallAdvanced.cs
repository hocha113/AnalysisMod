using AnalysisMod.AnalysisContent.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace AnalysisMod.AnalysisContent.Walls
{
    // This is a more advanced ModWall showing off animation, dynamic dust, light emitting, and simple custom framing logic
    // ����һ�����߼��� ModWall��չʾ�˶�������̬�ҳ�������ͼ򵥵��Զ������߼�
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
            // ѭ������ 2 ֡������ÿ 5 ��Ϸ֡����һ��
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
                // ������������ WallFrameNumber 0 �ĸ��ʷǳ�������ֻ��Ϊ���Ӿ��ϵĶ����ԣ�
                // https://i.imgur.com/9Irak3p.png
                if (frameNumber == 0 && WorldGen.genRand.NextBool(3, 4)) {
					frameNumber = WorldGen.genRand.Next(1, 3);
				}
			}
			return base.WallFrame(i, j, randomizeFrame, ref style, ref frameNumber);
		}
	}
}