using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader.Config.UI;


// 此文件定义了基于Float数据类型的自定义ConfigElement，
// 其中实现了可用于ModConfig类的自定义滑块着色方法。
namespace AnalysisMod.AnalysisCommon.Configs.CustomUI
{

	class CustomFloatElement : FloatElement
	{
		public CustomFloatElement() {
			ColorMethod = new Utils.ColorLerpMethod((percent) => Color.Lerp(Color.BlueViolet, Color.Aquamarine, percent));
		}
	}
}
