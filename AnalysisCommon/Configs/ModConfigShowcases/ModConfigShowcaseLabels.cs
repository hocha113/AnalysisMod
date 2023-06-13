using System.Collections.Generic;
using Terraria.ModLoader.Config;
using AnalysisMod.AnalysisCommon.Configs.CustomDataTypes;
using Terraria.ID;

// This file contains fake ModConfig class that showcase making config fields more readable
// with use of labels, headers and tooltips.

// Because this config was designed to show off various UI capabilities,
// this config have no effect on the mod and provides purely teaching Analysis.
//【重复的意思就不翻译了】
namespace AnalysisMod.AnalysisCommon.Configs.ModConfigShowcases
{
	[BackgroundColor(154, 152, 181)]
	public class ModConfigShowcaseLabels : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;

        // By default, all ModConfig fields and properties will have an automatically assigned Label and Tooltip translation key. You'll find these translation keys in your translation files. All of the English translations for the configs in AnalysisMod are found in AnalysisMod/Localization/en-US_Mods.AnalysisMod.Configs.hjson

        // Use Tooltip to convey additional information about the config item.
        // This Analysis shows additional text when hovered.

        // 默认情况下，所有的 ModConfig 字段和属性都会有自动分配的标签和工具提示翻译键。您可以在翻译文件中找到这些翻译键。
		// AnalysisMod 中配置项的所有英文翻译都在 AnalysisMod/Localization/en-US_Mods.AnalysisMod.Configs.hjson 文件中。

        // 使用 Tooltip 来传达关于配置项的附加信息。
        // 当鼠标悬停时，此分析显示额外的文本。
        [SliderColor(255, 0, 127)]
		public float SomeFloat;

        // Modders can pass in custom localization keys. This can be useful for reusing translations.
        // 模组开发者可以传入自定义本地化键。这对于重复使用翻译非常有用。
        [LabelKey("$Mods.AnalysisMod.Configs.Common.LocalizedLabel")]
		[TooltipKey("$Mods.AnalysisMod.Configs.Common.LocalizedTooltip")]
		public int LocalizedLabel;

        // These 3 Analysiss showcase the power of interpolating values into the translations.
        // Note how all 3 are using the same label key, but are interpolating different values into the label translation, resulting in different text. The same is done for tooltips.
        // Use this approach to reduce unnecessary duplication of text.
        // Note: using nameof can help avoid typos and errors. That would look like: $"$Mods.AnalysisMod.Items.{nameof(AnalysisYoyo)}.DisplayName"
        // Note: These Analysiss use color and item chat tags. See here for help on using Tags: https://terraria.wiki.gg/wiki/Chat#Tags

        // 这三个分析展示了将值插入翻译中的强大功能。
        // 请注意，所有3个都使用相同的标签键，但是在标签翻译中插入不同的值，从而产生不同的文本。提示工具也是如此。
        // 使用这种方法可以减少不必要的文本重复。
        // 注意：使用 nameof 可以帮助避免拼写错误和错误。它看起来像这样：$"$Mods.AnalysisMod.Items.{nameof(AnalysisYoyo)}.DisplayName"
        // 注意：这些分析使用颜色和物品聊天标记。有关如何使用标记，请参见此处: https://terraria.wiki.gg/wiki/Chat#Tags
        const string InterpolatedLabel = "$Mods.AnalysisMod.Configs.Common.InterpolatedLabel";
		const string InterpolatedTooltip = "$Mods.AnalysisMod.Configs.Common.InterpolatedTooltip";

		[LabelKey(InterpolatedLabel), TooltipKey(InterpolatedTooltip)] // Attributes can also be combined into a single line
																	   // 属性也可以合并成一行
        [LabelArgs("AnalysisMod/AnalysisYoyo", 1, "=>", "$Mods.AnalysisMod.Items.AnalysisYoyo.DisplayName")]
		[TooltipArgs("$Mods.AnalysisMod.Items.AnalysisYoyo.DisplayName", "FF55AA", "22a2dd")]
		public bool InterpolatedTextA;

		[LabelKey(InterpolatedLabel), TooltipKey(InterpolatedTooltip)]
		[LabelArgs("AnalysisMod/AnalysisSword", 2, "=>", "$Items.AnalysisSword.DisplayName")] // due to scope simplification, "Mods.AnalysisMod." can be omitted. (https://github.com/tModLoader/tModLoader/wiki/Localization#scope-simplification)
																							  // 由于范围简化，"Mods.AnalysisMod."可以省略。(https://github.com/tModLoader/tModLoader/wiki/Localization#scope-simplification)
        [TooltipArgs("$Mods.AnalysisMod.Items.AnalysisSword.DisplayName", "77bd8e", "88AADD")]
		public bool InterpolatedTextB;

		[LabelKey(InterpolatedLabel), TooltipKey(InterpolatedTooltip)]
		[LabelArgs(ItemID.Meowmere, 3, "=>", $"$ItemName.{nameof(ItemID.Meowmere)}")]
		[TooltipArgs($"$ItemName.{nameof(ItemID.Meowmere)}", "c441c6", "deeb55")]
		public bool InterpolatedTextC;

        // This Analysis shows advanced capabilities of string formatting. Values can be formatted to appear as percentages, with language appropriate thousandths separators, and with specific padding or precision.
        // The c# documentaion has more information: https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings
        // 该分析展示了字符串格式化的高级功能。可以将值格式化为百分比，使用适当语言的千位分隔符，并具有特定填充或精度。
        // C#文档提供更多信息：https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings
        [LabelArgs(.15753f, 1234567890, 12, 1.77777f)]
		public bool StringFormatting;

        // The color of the config entry can be customized. R, G, B
        // 配置项的颜色可以自定义。R、G、B
        [BackgroundColor(255, 0, 255)]
        // The corresponding tooltip translation for this entry is empty, so the tooltip shown will be from the Pair class.
        // If the Pair class tooltip had entries for arguments, we could use [TooltipArgs] here to customize it.
        // 此条目的对应工具提示翻译为空，因此显示的工具提示将来自Pair类。
        // 如果Pair类工具提示中有参数条目，我们可以在此处使用[TooltipArgs]进行自定义。
        public Pair pairAnalysis = new Pair();

        // List elements also inherit BackgroundColor
        // 列表元素也继承背景颜色
        [BackgroundColor(255, 0, 0)]
		public List<Pair> ListOfPair = new List<Pair>();

        // We can also add section headers, separating fields for organization
        // Using [Header("HeaderIdentifier")], Mods.AnalysisMod.Configs.ModConfigShowcaseLabels.Headers.HeaderIdentifier will automatically appear in localization files. We have populated the English entry with the value "Headers Section".
        // 我们还可以添加节标题，以分隔字段进行组织
        // 使用 [Header("HeaderIdentifier")]，Mods.AnalysisMod.Configs.ModConfigShowcaseLabels.Headers.HeaderIdentifier 将自动出现在本地化文件中。我们已经将英文条目填充了值“Headers Section”。
        [Header("HeaderIdentifier_头标识符")]
		public int TypicalHeader;

        // We can also specify a specific translation key, if desired.
        // The "$" character before a name means it should interpret the value as a translation key and use the loaded translation with the same key.
        // 如果需要，我们还可以指定特定的翻译键。
        // 名称前面的"$"字符表示它应该将值解释为翻译键，并使用具有相同键的加载翻译。
        [Header("$Mods.AnalysisMod.Configs.Common.LocalizedHeader")]
		public int LocalizedHeader;

        // Chat tags such as colored text or item icons can help users find config sections quickly
        // 聊天标签，如彩色文本或物品图标，可以帮助用户快速找到配置部分
        [Header("ChatTagAnalysis_聊天标签分析案例")]
		public int CoolHeader;

        // The class declaration of SimpleData specifies [BackgroundColor(255, 7, 7)]. Field and data structure field annotations override class annotations.
        // SimpleData类的声明指定了[BackgroundColor(255, 7, 7)]。字段和数据结构字段注释会覆盖类注释。
        [BackgroundColor(85, 107, 47)]
		public SimpleData simpleDataAnalysis2 = new SimpleData();
	}
}
