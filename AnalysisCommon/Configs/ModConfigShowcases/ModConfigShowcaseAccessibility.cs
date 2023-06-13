﻿using Newtonsoft.Json;
using Terraria;
using Terraria.ModLoader.Config;
using AnalysisMod.AnalysisCommon.Configs.CustomDataTypes;

// This file contains fake ModConfig class that showcase using
// access modifiers (to control which fields should be visible and have their value saved to file)
// and properties (to implement simple "presets" system).
// 这个文件包含了一个虚假的 ModConfig 类，展示了如何使用访问修饰符（控制哪些字段应该可见并将其值保存到文件中）和属性（实现简单的“预设”系统）。

// Because this config was designed to show off various UI capabilities,
// this config have no effect on the mod and provides purely teaching Analysis.
// 因为这个配置文件旨在展示各种UI功能，
// 这个配置对模组没有任何影响，仅提供纯粹的教学分析。
namespace AnalysisMod.AnalysisCommon.Configs.ModConfigShowcases
{
	[BackgroundColor(164, 153, 190)]
	public class ModConfigShowcaseAccessibility : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;

        // Private and Internal fields and properties will not be shown.
        // Note that private and internal values will not be replaced by the deserialization, so initializer and ctor work.
        // You should avoid private and internal values in
        // 不会显示私有和内部字段和属性。
        // 请注意，私有和内部值不会被反序列化替换，因此初始化程序和构造函数仍然有效。
        // 您应该避免在中使用私有和内部值
#pragma warning disable CS0414
        private float Private = 144;
#pragma warning restore CS0414
		internal float Internal;

        // Public fields are most common. Use public for most items.
        // 公共字段是最常见的。对于大多数项目，请使用 public。
        public float Public;

        // Will not show. Avoid static. Due to how ModConfig works, static fields will not work correctly. Use a static field named Instance in the manner used in AnalysisConfigServer for accessing ModConfig fields in the rest of your mod.
        // 不会显示。避免使用静态字段。由于 ModConfig 的工作方式，静态字段将无法正确地工作。请像 AnalysisConfigServer 中使用的那样，在其余模组中访问 ModConfig 字段时使用名为 Instance 的静态字段。
        public static float Static;

        // Get only properties will show up, but will be grayed out to show that they can't be changed.
        // 只有属性会显示出来，但是会变灰以表示它们无法更改。
        public float Getter => Main.rand?.NextFloat(1f) ?? 0; // This is just an Analysis, please don't do this.
                                                              // 这只是一种分析，请不要这样做。

        // AutoProperies work the same as fields.
        // 自动属性与字段的工作方式相同。
        public float AutoProperty { get; set; }

        // Properties work as well. The backing field will be ignored when writing the json out.
        // 属性同样有效。在写出 JSON 时，后备字段将被忽略。
        private float propertyBackingField;
		public float Property {
			get { return propertyBackingField; }
			set { propertyBackingField = value + 0.2f; } // + 0.2f is just to mess with the user.
														 // +0.2f只是为了恶搞用户。
        }

        // Using JsonIgnore on a public field means the field won't show up in the json or UI. Not really useful.
        // 在公共字段上使用JsonIgnore意味着该字段不会显示在json或UI中。这并不是很有用。
        [JsonIgnore]
		public float Ignore;

        // Using ShowDespiteJsonIgnore overrides JsonIgnore for the UI. Use this to display info to the user if needed. The value won't be saved since it is derived from other fields.
        // Useful for things like displaying sums or calculated relationships.
        // 使用 ShowDespiteJsonIgnore 覆盖 JsonIgnore 以供 UI 使用。如果需要向用户显示信息，则可以使用此选项。该值不会被保存，因为它是从其他字段派生的。
        // 对于显示总和或计算关系等内容非常有用。
        [JsonIgnore]
		[ShowDespiteJsonIgnore]
		public float IgnoreWithLabelGetter => AutoProperty + Public;

        // Reference type getters kind of work with the UI. You can experiment with this if you want.
        // 引用类型的getter在UI上有点作用。如果你想的话，可以尝试一下。
        [JsonIgnore]
		public Pair pair2 => pair;
		public Pair pair;

        // Set only properties will crash tModLoader.
        // 仅设置属性会导致 tModLoader 崩溃。
        // public float Setter { set { Public = value; } }

        // The following shows how you can use properties to implement a preset system
        // 下面展示了如何使用属性来实现预设系统.
        public bool PresetA {
			get => Data1 == 23 && Data2 == 63;
			set {
				if (value) {
					Data1 = 23;
					Data2 = 63;
				}
			}
		}

		public bool PresetB {
			get => Data1 == 93 && Data2 == 13;
			set {
				if (value) {
					Data1 = 93;
					Data2 = 13;
				}
			}
		}

		[Slider]
		public int Data1 { get; set; }
		[Slider]
		public int Data2 { get; set; }

		public ModConfigShowcaseAccessibility() {
			Internal = 0.2f;
		}

        // ShouldSerialize{FieldNameHere}. ShouldSerialize can be useful, but this Analysis is simply replicating the behavior of JSONIgnore and is just an Analysis for Analysiss sake. https://www.newtonsoft.com/json/help/html/ConditionalProperties.htm
        // ShouldSerialize{FieldNameHere}。ShouldSerialize可能很有用，但这个分析只是复制JSONIgnore的行为，只是为了分析而进行的。https://www.newtonsoft.com/json/help/html/ConditionalProperties.htm
        public bool ShouldSerializeGetter() {
            // We can have some logic in here to determine if the value is worth saving, but this is just a trivial Analysis
            // 我们可以在这里加入一些逻辑来确定值是否值得保存，但这只是一个微不足道的分析案例
            return false;
		}
	}
}
