using Terraria.ModLoader;

namespace AnalysisMod.AnalysisCommon.Systems
{
    // 作为此模组注册的按键绑定的容器。
    // 有关使用方法，请参见 Common/Players/AnalysisKeybindPlayer。
    public class KeybindSystem : ModSystem
    {
        public static ModKeybind RandomBuffKeybind { get; private set; }

        public override void Load()
        {
            // Registers a new keybind
            // We localize keybinds by adding a Mods.{ModName}.Keybind.{KeybindName} entry to our localization files. The actual text displayed to english users is in en-US.hjson

            // 注册新的按键绑定
            // 我们通过向本地化文件添加 Mods.{ModName}.Keybind.{KeybindName} 条目来本地化按键绑定。对于英语用户显示的实际文本在 en-US.hjson 中
            RandomBuffKeybind = KeybindLoader.RegisterKeybind(Mod, "RandomBuff", "P");
        }

        // 请参阅 AnalysisMod.cs 的 Unload() 方法，以获取卸载过程的详细说明。
        public override void Unload()
        {
            // 如果您的 AssemblyLoadContext 正确卸载，则不需要此操作，但将静态字段置空可以帮助您找出是什么使其保持加载状态。
            RandomBuffKeybind = null;
        }
    }
}
