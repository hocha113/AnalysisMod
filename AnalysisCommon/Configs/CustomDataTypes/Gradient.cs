using Microsoft.Xna.Framework;
using System.ComponentModel;
using Terraria.ModLoader.Config;

// This file defines custom data type that represents Gradient data type that can be used in ModConfig classes.
// 该文件定义了自定义数据类型，表示可以在ModConfig类中使用的梯度数据类型。
namespace AnalysisMod.AnalysisCommon.Configs.CustomDataTypes
{
	public class Gradient
	{
		[DefaultValue(typeof(Color), "0, 0, 255, 255")]
		public Color start = Color.Blue; // For sub-objects, you'll want to make sure to set defaults in constructor or field initializer.
										 // 对于子对象，您需要确保在构造函数或字段初始化器中设置默认值。
        [DefaultValue(typeof(Color), "255, 0, 0, 255")]
		public Color end = Color.Red;

		public override bool Equals(object obj) {
			if (obj is Gradient other)
				return start == other.start && end == other.end;
			return base.Equals(obj);
		}

		public override int GetHashCode() {
			return new { start, end }.GetHashCode();
		}
	}
}
