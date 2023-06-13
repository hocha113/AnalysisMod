using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ModLoader.Config;
using AnalysisMod.AnalysisCommon.Configs.CustomDataTypes;

// This file contains fake ModConfig class that showcase defining subpages
// that can be used to separate config section into sub-configs for easier management.

// Because this config was designed to show off various UI capabilities,
// this config have no effect on the mod and provides purely teaching Analysis.
namespace AnalysisMod.AnalysisCommon.Configs.ModConfigShowcases
{
	[BackgroundColor(148, 72, 188)]
	public class ModConfigShowcaseSubpages : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;

		[Header("SeparatePageAnalysiss")]
        // Using SeparatePage, an object will be presented to the user as a button. That button will lead to a separate page where the usual UI will be presented. Useful for organization.
        // 使用SeparatePage，一个对象将被呈现给用户作为按钮。该按钮将导向单独的页面，在那里通常的UI将被呈现。对于组织非常有用。
        [SeparatePage]
		public Gradient gradient = new Gradient();

        // This Analysis has multiple levels of subpages, check it out. In this Analysis, the SubConfigAnalysis class itself is annotated with [SeparatePage]
        // 这个分析有多层子页面，请查看。在这个分析中，SubConfigAnalysis类本身带有[SeparatePage]注释。
        public SubConfigAnalysis subConfigAnalysis = new SubConfigAnalysis();

		[SeparatePage]
		public Dictionary<ItemDefinition, SubConfigAnalysis> DictionaryofSubConfigAnalysis = new Dictionary<ItemDefinition, SubConfigAnalysis>();

        // These 2 Analysiss show how [SeparatePage] works on annotating both a field for a class and annotating a List of a class
        // 这两个分析展示了如何使用 [SeparatePage] 注释一个类的字段以及注释一个类的列表
        [SeparatePage]
		public List<Pair> SeparateListOfPairs = new List<Pair>();

		[SeparatePage]
		public Pair pair = new Pair();

        // C# allows inner classes (used here), which might be useful for organization if you want.
        // C#允许内部类（在此处使用），如果需要组织，这可能会很有用。
        [SeparatePage]
		public class SubConfigAnalysis
		{
			[DefaultValue(99)]
			public int boost = 99;
			public float percent;
			public bool enabled;

			[SeparatePage]
			[BackgroundColor(50, 200, 100)]
			public SubSubConfigAnalysis SubA = new SubSubConfigAnalysis();

			[SeparatePage]
			public SubSubConfigAnalysis SubB = new SubSubConfigAnalysis();

			public override string ToString() {
				return $"{boost} {percent} {enabled} {SubA.whoa}/{SubB.whoa}";
			}

			public override bool Equals(object obj) {
				if (obj is SubConfigAnalysis other)
					return boost == other.boost && percent == other.percent && enabled == other.enabled && SubA.Equals(other.SubA) && SubB.Equals(other.SubB);
				return base.Equals(obj);
			}

			public override int GetHashCode() {
				return new { boost, percent, enabled, SubA, SubB }.GetHashCode();
			}
		}

		public class SubSubConfigAnalysis
		{
			public int whoa;
			public override bool Equals(object obj) {
				if (obj is SubSubConfigAnalysis other)
					return whoa == other.whoa;
				return base.Equals(obj);
			}

			public override int GetHashCode() => whoa.GetHashCode();
		}
	}
}
