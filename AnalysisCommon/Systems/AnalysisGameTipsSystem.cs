using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisCommon.Systems
{
    /// <summary>
    /// 这个小的 ModSystem 展示了 <seealso cref="ModSystem.ModifyGameTipVisibility"/> 钩子，它允许你修改在加载屏幕期间显示的提示/提示信息。
    /// </summary>
    public class AnalysisGameTipsSystem : ModSystem
    {

        public override void ModifyGameTipVisibility(IReadOnlyList<GameTipData> gameTips)
        {
            // 如果您想添加自己的提示，则必须将它们放入本地化文件中。
            // 请查看 Localization/en-US.hjson 文件中 GameTips 键的功能。

            // 如果我们想修改 Vanilla 提示怎么办？tModLoader 中内置了一个 GameTipID，应该可以更轻松地禁用某些提示。
            // 为了分析，让我们关闭血月和日食提示！
            gameTips[GameTipID.BloodMoonZombieDoorOpening].Hide();
            gameTips[GameTipID.SolarEclipseCreepyMonsters].Hide();

            // 现在，假设您想修改其他 mod 的提示？你也可以这样做！确保使用正确的 mod 和键名。
            GameTipData disabledTip = gameTips.FirstOrDefault(tip => tip.FullName == "AnalysisMod/DisabledAnalysisTip");

            // 可选地，如果您希望对提示名称和模组名称进行更具体的说明，则还可以使用 Mod 和 Name 属性：

            // 如果您以前没有见过空值传播，简单介绍一下：问号检查值是否为空，
            // 如果是，则什么都不会发生，并且不会抛出错误；但如果它不是 null，则像往常一样调用方法！
            disabledTip?.Hide();
        }
    }
}
