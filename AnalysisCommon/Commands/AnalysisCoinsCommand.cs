using AnalysisMod.AnalysisCommon.Systems;
using AnalysisMod.AnalysisCommon.AnalysisUI.AnalysisCoinsUI;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisCommon.Commands
{
    public class AnalysisCoinsCommand : ModCommand
    {
        // CommandType.Chat means that command can be used in Chat in SP and MP
        // CommandType.Chat 表示该命令可在SP和MP的聊天中使用
        public override CommandType Type
            => CommandType.Chat;

        // The desired text to trigger this command
        // 触发此命令的所需文本
        public override string Command
            => "Analysis_coins";

        // A short description of this command
        // 此命令的简短描述
        public override string Description
            => "Show the coin rate UI";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            ModContent.GetInstance<AnalysisCoinsUISystem>().ShowMyUI();
        }
    }
}