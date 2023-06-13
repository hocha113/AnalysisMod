using Terraria.ModLoader.Config;

// This file defines custom simple data type with two fields - boolean value and integer value.
// 这个文件定义了一个自定义的简单数据类型，包含两个字段 - 布尔值和整数值。
namespace AnalysisMod.AnalysisCommon.Configs.CustomDataTypes
{
    //指定用于ModConfig UI中的属性、字段或类的背景颜色
    [BackgroundColor(0, 255, 255)]
	public class Pair
	{
		public bool enabled;
		public int boost;

        // If you override ToString, it will show up appended to the Label in the ModConfig UI.
        // 如果你重写 ToString 方法，它将会显示在 ModConfig UI 的标签后面。
        public override string ToString() {
			return $"Boost: {(enabled ? "" + boost : "disabled")}";
		}

        // Implementing Equals and GetHashCode are critical for any classes you use.
        // 实现Equals和GetHashCode对于你使用的任何类都是至关重要的。
        public override bool Equals(object obj) {
			if (obj is Pair other)
				return enabled == other.enabled && boost == other.boost;
			return base.Equals(obj);
		}

		public override int GetHashCode() {
			return new { boost, enabled }.GetHashCode();
		}
	}
}
