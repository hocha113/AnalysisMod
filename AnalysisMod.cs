using Terraria.GameContent.UI;
using Terraria.ModLoader;

namespace AnalysisMod
{
    public partial class AnalysisMod : Mod
    {
        public const string AssetPath = $"{nameof(AnalysisMod)}/Assets/";

        public static int AnalysisCustomCurrencyId;

        public override void Load()
        {
            // Registers a new custom currency
            // 注册新的自定义货币
            AnalysisCustomCurrencyId = CustomCurrencyManager.RegisterCurrency(new AnalysisContent.Currencies.AnalysisCustomCurrency(ModContent.ItemType<AnalysisContent.Items.AnalysisItem>(), 999L, "Mods.AnalysisMod.Currencies.AnalysisCustomCurrency"));
        }

        public override void Unload()
        {
            // The Unload() methods can be used for unloading/disposing/clearing special objects, unsubscribing from events, or for undoing some of your mod's actions.
            // Be sure to always write unloading code when there is a chance of some of your mod's objects being kept present inside the vanilla assembly.
            // The most common reason for that to happen comes from using events, NOT counting On.* and IL.* code-injection namespaces.
            // If you subscribe to an event - be sure to eventually unsubscribe from it.

            // Unload() 方法可用于卸载/释放/清除特殊对象、取消订阅事件或撤销您的某些 mod 操作。
            // 请务必在可能存在一些 mod 对象仍保留在原始程序集中时编写卸载代码。
            // 最常见的原因是使用事件，不包括 On.* 和 IL.* 代码注入命名空间。
            // 如果您订阅了一个事件，请确保最终取消订阅。

            // NOTE: When writing unload code - be sure use 'defensive programming'. Or, in other words, you should always assume that everything in the mod you're unloading might've not even been initialized yet.
            // NOTE: There is rarely a need to null-out values of static fields, since TML aims to completely dispose mod assemblies in-between mod reloads.

            // 注意：编写卸载代码时，请务必使用“防御性编程”。换句话说，您应该始终假设要卸载的 mod 中的所有内容都可能尚未初始化。
            // 注意：几乎没有必要将静态字段值设置为 null，因为 TML 的目标是在 mod 重新加载之间完全处理掉 mod 程序集。
        }
    }
}