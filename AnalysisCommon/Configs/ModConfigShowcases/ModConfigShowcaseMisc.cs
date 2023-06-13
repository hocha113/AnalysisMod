using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Terraria.ModLoader.Config;
using AnalysisMod.AnalysisCommon.Configs.CustomDataTypes;
using AnalysisMod.AnalysisCommon.Configs.CustomUI;

// This file contains fake ModConfig class that showcase various attributes
// that can be used to customize behavior config fields.

// Because this config was designed to show off various UI capabilities,
// this config have no effect on the mod and provides purely teaching Analysis.
namespace AnalysisMod.AnalysisCommon.Configs.ModConfigShowcases
{
    /// <summary>
    /// This config is just a showcase of various attributes and their effects in the UI window.
    /// 这个配置只是展示了各种属性及其在UI窗口中的效果。
    /// </summary>
    public class ModConfigShowcaseMisc : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;

		[CustomModConfigItem(typeof(GradientElement))]
		public Gradient gradient = new Gradient();

        /*
		// Here are some more Analysiss, showing a complex JsonDefaultListValue and a initializer overriding the defaults of the constructor.
		//这里有更多的分析，展示了一个复杂的JsonDefaultListValue和一个初始化器覆盖构造函数的默认值。
		[CustomModConfigItem(typeof(GradientElement))]
		public Gradient gradient2 = new Gradient() {
			start = Color.AliceBlue,
			end = Color.DeepSkyBlue
		};

		[JsonDefaultListValue("{\"start\": \"238, 248, 255, 255\", \"end\": \"0, 191, 255, 255\"}")]
		public List<Gradient> gradients = new List<Gradient>();
		*/

        // In this case, CustomModConfigItem is annotating the Enum instead of the Field. Either is acceptable and can be used for different situations.
        // 在这种情况下，CustomModConfigItem注释的是枚举而不是字段。两者都可以接受，并可用于不同的情况。
        public Corner corner;

        // You can put multiple attributes in the same [] if you like.
        // ColorHueSliderAttribute displays Hue Saturation Lightness. Passing in false means only Hue is shown.
        // 如果您愿意，可以将多个属性放在同一个[]中。
        // ColorHueSliderAttribute 显示色相、饱和度和亮度。传入 false 表示仅显示色相。
        [DefaultValue(typeof(Color), "255, 0, 0, 255"), ColorHSLSlider(false), ColorNoAlpha]
		public Color hsl;

        // In this Analysis we inherit from a tmodloader config UIElement to slightly customize the colors.
        // 在这个分析中，我们从tmodloader配置UIElement继承，稍微自定义了一些颜色。
        [CustomModConfigItem(typeof(CustomFloatElement))]
		public float tint;

		public Dictionary<string, Pair> StringPairDictionary = new Dictionary<string, Pair>();
		public Dictionary<ItemDefinition, float> JsonItemFloatDictionary = new Dictionary<ItemDefinition, float>();

		public HashSet<ItemDefinition> itemSet = new HashSet<ItemDefinition>();

		public List<Pair> ListOfPair2 = new List<Pair>();
		public Pair pairAnalysis2 = new Pair();

        // In this Analysis, the list defaults to collapse.
        // 在这个分析案例中，列表默认为折叠状态。
        [Expand(false)]
		public List<string> collapsedList = new List<string>() { "1", "2", "3", "4", "5" };

        // This Analysis collapses the list elements as well as the list itself.
        // 此分析案例将列表元素以及列表本身都折叠起来。
        [Expand(false, false)]
		public List<Pair> collapsedListOfCollapsedObjects = new List<Pair>() { new Pair() { enabled = true, boost = 3 }, new Pair { enabled = true, boost = 6 } };

		[Expand(false)]
		public SimpleData simpleDataAnalysis; // you can also initialize in the constructor, see initialization in public ModConfigShowcaseMisc() below.

        // This annotation allows the UI to null out this class. You need to make sure to initialize fields without the NullAllowed annotation in constructor or initializer or you might have issues. Of course, if you allow nulls, you'll need to make sure the rest of your mod will handle them correctly. Try to avoid null unless you have a good reason to use them, as null objects will only complicate the rest of your code.
        // 此注释允许UI将此类置空。您需要确保在构造函数或初始化程序中对没有NullAllowed注释的字段进行初始化，否则可能会出现问题。当然，如果允许null值，则需要确保您的模块的其余部分能够正确处理它们。除非有很好的理由使用它们，否则请尽量避免使用null值，因为null对象只会使代码变得更加复杂。
        [NullAllowed]
		[JsonDefaultValue("{\"boost\": 777}")] // With NullAllowed, you can specify a default value like this.
											   // 使用 NullAllowed，您可以像这样指定默认值。
        public SimpleData simpleDataAnalysis2;

		public ComplexData complexData = new ComplexData();

		[JsonExtensionData]
		private IDictionary<string, JToken> _additionalData = new Dictionary<string, JToken>();

        // See _additionalData usage in OnDeserializedMethod to see how this ListOfInts can be populated from old versions of this mod.
        // 请查看OnDeserializedMethod中的_additionalData用法，了解如何从旧版本的此mod中填充此ListOfInts。
        public List<int> ListOfInts = new List<int>();

		public ModConfigShowcaseMisc() {
			simpleDataAnalysis = new SimpleData();
			simpleDataAnalysis.boost = 32;
			simpleDataAnalysis.percent = 0.7f;
		}

		[OnDeserialized]
		internal void OnDeserializedMethod(StreamingContext context) {
            // If you change ModConfig fields between versions, your users might notice their configuration is lost when they update their mod.
            // We can use [JsonExtensionData] to capture un-de-serialized data and manually restore them to new fields.
            // Imagine in a previous version of this mod, we had a field "OldListOfInts" and we want to preserve that data in "ListOfInts".
            // To test this, insert the following into AnalysisMod_ModConfigShowcase.json: "OldListOfInts": [ 99, 999],

            // 如果您在版本之间更改ModConfig字段，则用户在更新其mod时可能会注意到其配置丢失。
            // 我们可以使用[JsonExtensionData]来捕获未反序列化的数据，并手动将它们恢复到新字段中。
            // 想象一下，在此mod的先前版本中，我们有一个名为“OldListOfInts”的字段，我们希望将该数据保留在“ListOfInts”中。
            // 要测试这个，请将以下内容插入AnalysisMod_ModConfigShowcase.json："OldListOfInts":[99,999], 
            if (_additionalData.TryGetValue("OldListOfInts", out var token)) {
				var OldListOfInts = token.ToObject<List<int>>();
				ListOfInts.AddRange(OldListOfInts);
			}
			_additionalData.Clear(); // make sure to clear this or it'll crash.
									 // 一定要清除这个，否则会崩溃
        }
	}
}
