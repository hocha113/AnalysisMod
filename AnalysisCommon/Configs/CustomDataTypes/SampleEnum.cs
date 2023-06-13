using Terraria.ModLoader.Config;

// This file defines an enum data type that can be used in ModConfig classes.
// 该文件定义了一个枚举数据类型，可用于 ModConfig 类中。
namespace AnalysisMod.AnalysisCommon.Configs.CustomDataTypes
{
	public enum SampleEnum
	{
		Weird,
		Odd,
        // Enum members can be individually labeled as well
        // 枚举成员也可以单独标记
        // [LabelKey("$Mods.AnalysisMod.Configs.SampleEnum.Strange.Label")]
        Strange,
		Peculiar
	}
}
