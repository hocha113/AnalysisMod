using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ModLoader.Config;
using AnalysisMod.AnalysisCommon.Configs.CustomDataTypes;

// This file contains fake ModConfig class that showcase defining default values for config fields.

// Because this config was designed to show off various UI capabilities,
// this config have no effect on the mod and provides purely teaching Analysis.

// 这个文件包含了一个虚假的 ModConfig 类，用于展示如何定义配置字段的默认值。

// 由于这个配置是为了展示各种 UI 功能而设计的，
// 它对模组没有任何影响，仅提供纯粹的教学分析。
namespace AnalysisMod.AnalysisCommon.Configs.ModConfigShowcases
{
	[BackgroundColor(164, 153, 190)]
	public class ModConfigShowcaseDefaultValues : ModConfig
	{
        // There are 2 approaches to default values. One is applicable only to value types (int, bool, float, string, structs, etc) and the other to reference types (classes).
        // For value types, annotate the field with the DefaultValue attribute. Some structs, like Color and Vector2, accept a string that will be converted to a default value.
        // For reference types (classes), simply assign the value in the field initializer or constructor as you would typically do.
        // 默认值有两种方法。一种适用于值类型（int、bool、float、string、structs等），另一种适用于引用类型（classes）。
        // 对于值类型，请使用DefaultValue属性注释字段。某些结构体，如Color和Vector2，接受一个字符串，该字符串将转换为默认值。
        // 对于引用类型（classes），只需像通常那样在字段初始化程序或构造函数中分配该值即可。

        public override ConfigScope Mode => ConfigScope.ClientSide;

        // Using DefaultValue, we can specify a default value.
        //使用DefaultValue，我们可以指定一个默认值。
		[DefaultValue(99)]
		public int SimpleDefaultInt;

		[DefaultValue(typeof(Color), "73, 94, 171, 255")] // needs 4 comma separated bytes. The Color struct has [TypeConverter(typeof(ColorConverter))] annotating it supplying a way to convert a text constant to a runtime default value.
														  // 需要4个逗号分隔的字节。Color结构体带有[TypeConverter(typeof(ColorConverter))]注释，提供了一种将文本常量转换为运行时默认值的方法。
        public Color SomeColor;

		[DefaultValue(typeof(Vector2), "0.23, 0.77")]
		public Vector2 SomeVector2;

		[DefaultValue(SampleEnum.Strange)]
		[DrawTicks]
		public SampleEnum EnumAnalysis2;

        // Using StringEnumConverter, Enums are read and written as strings rather than the numerical value of the Enum. This makes the config file more readable, but prone to errors if a player manually modifies the config file.
        // 使用StringEnumConverter，枚举将被读取和写入为字符串，而不是枚举的数值。这使得配置文件更易于阅读，但如果玩家手动修改配置文件，则容易出现错误。
        [JsonConverter(typeof(StringEnumConverter))]
		public SampleEnum EnumAnalysis1 { get; set; }

        // OptionStrings makes a string appear as a choice rather than an input field. Remember that users can manually edit json files, so be aware that a value other than the Options in OptionStrings might populate the field.
        // TODO: Not working. Won't restore defaults
        // OptionStrings可以将字符串显示为选择而不是输入字段。请记住，用户可以手动编辑json文件，因此要注意除OptionStrings中的选项外，可能会填充该字段的其他值。
        // TODO：无法正常工作。无法恢复默认设置
        [OptionStrings(new string[] { "Win", "Lose", "Give Up" })]
		[DefaultValue(new string[] { "Give Up", "Give Up" })]
		public string[] ArrayOfString;

		[DrawTicks]
		[OptionStrings(new string[] { "Pikachu", "Charmander", "Bulbasaur", "Squirtle" })]
		[DefaultValue("Bulbasaur")]
		public string FavoritePokemon;

        // DefaultListValue provides the default value to be added when the user clicks add in the UI.
        // DefaultListValue 提供了在用户单击 UI 中的添加按钮时要添加的默认值。
        [DefaultListValue(123)]
		public List<int> ListOfInts = new List<int>();

		[DefaultListValue(typeof(Vector2), "0.1, 0.2")]
		public List<Vector2> ListOfVector2 = new List<Vector2>();

        // JsonDefaultListValue provides the default value for reference types/classes, expressed as JSON. If you are unsure of the JSON, you can copy from a saved config file itself.
        // JsonDefaultListValue 提供了引用类型/类的默认值，以 JSON 形式表示。如果您不确定 JSON 的格式，可以从已保存的配置文件中复制。
        [JsonDefaultListValue("{\"name\": \"GoldBar\"}")]
		public List<ItemDefinition> ListOfItemDefinition = new List<ItemDefinition>();

        // For Dictionaries, additional attributes (DefaultDictionaryKeyValue or JsonDefaultDictionaryKeyValue) are used to specify a default value for the Key of the Dictionary entry. The Value uses the DefaultListValue or JsonDefaultListValue as List and HashSet do.
        // 对于字典，使用附加属性（DefaultDictionaryKeyValue或JsonDefaultDictionaryKeyValue）来指定字典条目的键的默认值。 值使用DefaultListValue或JsonDefaultListValue作为列表和HashSet。
        [DefaultDictionaryKeyValue(0.3f)]
		[DefaultListValue(10)]
		public Dictionary<float, int> DictionaryDefaults = new Dictionary<float, int>();

		[JsonDefaultDictionaryKeyValue("{\"name\": \"GoldBar\"}")]
		[JsonDefaultListValue("{\"name\": \"SilverBar\"}")]
		public Dictionary<ItemDefinition, ItemDefinition> DictionaryDefaults2 = new Dictionary<ItemDefinition, ItemDefinition>();
	}
}
