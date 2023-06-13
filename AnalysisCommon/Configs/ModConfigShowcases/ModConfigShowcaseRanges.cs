using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Terraria;
using Terraria.ModLoader.Config;


// This file contains fake ModConfig class that showcase creating config section
// by using fields with defined ranges.

// Because this config was designed to show off various UI capabilities,
// this config have no effect on the mod and provides purely teaching Analysis.
namespace AnalysisMod.AnalysisCommon.Configs.ModConfigShowcases
{
	[BackgroundColor(99, 180, 209)]
	public class ModConfigShowcaseRanges : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;

        // With no annotations on a float, a range from 0 to 1 with ticks of 0.01 is the default.
        //没有浮点数的注释，默认情况下，范围从0到1，刻度为0.01。
        public float NormalFloat;

        // We can specify range, increments, and even whether or not to draw guide ticks with annotations.
        // 我们可以指定范围、增量，甚至是否使用注释绘制辅助刻度。
        [Range(2f, 3f)]
		[Increment(.25f)]
		[DrawTicks]
		[DefaultValue(2f)]
		public float IncrementalRangedFloat;

		[Range(0f, 5f)]
		[Increment(.11f)]
		public float IncrementByPoint11;

		[Range(2f, 5f)]
		[DefaultValue(2f)]
		public float RangedFloat;

        // With no annotations on an int, a range from 0 to 100 is the default. Ints will be displayed as a text input unless a Slider attribute is present.
        // 如果一个整数没有注释，那么默认范围是从0到100。如果存在Slider属性，则整数将显示为文本输入。
        public int NormalInt;

		[Increment(5)]
		[Range(60, 250)]
		[DefaultValue(100)]
		[Slider] // The Slider attribute makes this field be presented with a slider rather than a text input. The default ticks is 1.
				 // Slider属性使该字段以滑块形式呈现，而不是文本输入。默认刻度为1。
        public int RangedInteger;

        // We can annotate a List<int> and the range, ticks, increment, and slider attributes will be used by all elements of the List.
        // We can use DefaultListValue to set the default value for items added to the list. Using DefaultValue here will crash the game.

        // 我们可以为List<int>添加注释，范围、刻度、增量和滑块属性将应用于列表的所有元素。
        // 我们可以使用DefaultListValue来设置添加到列表中的项的默认值。在此处使用DefaultValue会导致游戏崩溃。
        [Range(10, 20)]
		[Increment(2)]
		[DrawTicks]
		[DefaultListValue(16)]
		[Slider]
		public List<int> ListOfInts = new List<int>();

		[Range(-20f, 20f)]
		[Increment(5f)]
		[DrawTicks]
		public Vector2 RangedWithIncrementVector2;

        // A method annotated with OnDeserialized will run after deserialization. You can use it for enforcing things like ranges, since Range and Increment are UI suggestions.
        // 带有OnDeserialized注释的方法将在反序列化后运行。您可以使用它来强制执行诸如范围之类的内容，因为Range和Increment是UI建议。
        [OnDeserialized]
		internal void OnDeserializedMethod(StreamingContext context) {
            // RangeAttribute is just a suggestion to the UI. If we want to enforce constraints, we need to validate the data here. Users can edit config files manually with values outside the RangeAttribute, so we fix here if necessary.
            // Both enforcing ranges and not enforcing ranges have uses in mods. Make sure you fix config values if values outside the range will mess up your mod.

            // RangeAttribute只是对UI的建议。如果我们想强制执行约束，就需要在此验证数据。用户可以手动编辑配置文件，并使用超出RangeAttribute范围的值，因此必要时我们在这里进行修复。
            // 强制和不强制范围都有mod中的用途。确保您修复配置值，如果超出范围的值会破坏您的mod。
            RangedFloat = Utils.Clamp(RangedFloat, 2f, 5f);
		}
	}
}
