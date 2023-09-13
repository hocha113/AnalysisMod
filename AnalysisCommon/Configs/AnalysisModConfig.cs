using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace AnalysisMod.AnalysisCommon.Configs
{
	public class AnalysisModConfig : ModConfig
	{
        // ConfigScope.ClientSide 通常用于客户端，例如视觉或音频调整。
        // ConfigScope.ServerSide 应该用于基本上所有其他事情，包括禁用物品或更改 NPC 行为
        public override ConfigScope Mode => ConfigScope.ServerSide;

        // The things in brackets are known as "Attributes".
        //括号中的内容被称为“属性”。

        [Header("Items")]
        // Headers are like titles in a config. You only need to declare a header on the item it should appear over, not every item in the category. 
        // [Label("$Some.Key")] // A label is the text displayed next to the option. This should usually be a short description of what it does. By default all ModConfig fields and properties have an automatic label translation key, but modders can specify a specific translation key.
        // [Tooltip("$Some.Key")] // A tooltip is a description showed when you hover your mouse over the option. It can be used as a more in-depth explanation of the option. Like with Label, a specific key can be provided.

        // 头部就像配置文件中的标题。您只需要在应该出现在其上方的项目上声明一个头部，而不是类别中的每个项目。
        // [Label("$Some.Key")] // 标签是选项旁边显示的文本。这通常应该是它所做内容的简短描述。默认情况下，所有ModConfig字段和属性都具有自动标签翻译键，但modder可以指定特定的翻译键。
        // [Tooltip("$Some.Key")] // 工具提示是当您将鼠标悬停在选项上时显示的说明。它可以用作更详细地解释选项。与Label一样，可以提供特定密钥。

        [DefaultValue(true)] // This sets the configs default value.
                             // 这将设置配置的默认值。
        [ReloadRequired] // Marking it with [ReloadRequired] makes tModLoader force a mod reload if the option is changed. It should be used for things like item toggles, which only take effect during mod loading
                         // 将其标记为[ReloadRequired]会使tModLoader在更改选项时强制重新加载模组。它应该用于像物品切换这样的东西，在模组加载期间才能生效。
        public bool AnalysisWingsToggle; // To see the implementation of this option, see AnalysisWings.cs
                                         // 要查看此选项的实现，请参阅AnalysisWings.cs

        [ReloadRequired]
		public bool WeaponWithGrowingDamageToggle;
	}
}
