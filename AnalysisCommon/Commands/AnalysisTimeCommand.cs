using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisCommon.Commands
{
    public class AnalysisTimeCommand : ModCommand
    {
        // CommandType.World means that command can be used in Chat in SP and MP, but executes on the Server in MP
        // CommandType.World 表示该命令可在SP和MP的聊天中使用，但在MP上执行时会在服务器端执行
        public override CommandType Type
            => CommandType.World;

        // The desired text to trigger this command
        // 触发此命令的所需文本
        public override string Command
            => "addTime";

        // A short usage explanation for this command
        // 这个命令的简短使用说明
        public override string Usage
            => "/addTime numTicks" +
            "\n numTicks — positive or negative number in ticks (1 second = 60 ticks) + " +
            "\n numTicks表示以tick为单位的正数或负数（1秒=60 ticks）.";

        // A short description of this command
        // 这个命令的简短描述
        public override string Description
            => "Adds numTicks to fast forward or rewind world time + " +
            "\n 将numTicks添加到快进或倒回世界时间";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            // Time in ticks for complete full day+night cycle (86600)
            // 完整的白天和黑夜循环所需的滴答时间（86600）
            const double cycleLength = Main.dayLength + Main.nightLength;
            // Checking input Arguments
            // 检查输入参数
            if (args.Length == 0)
            {
                throw new UsageException("At least one argument was expected.");
            }
            if (!int.TryParse(args[0], out int extraTime))
            {
                throw new UsageException(args[0] + " is not a correct integer value.");
            }

            // Convert current time (0-54000 for day and 0-32400 for night) to cycle time (0-86600)
            // 将当前时间（白天为0-54000，夜晚为0-32400）转换为周期时间（0-86600）
            double fullTime = Main.time;
            if (!Main.dayTime)
            {
                fullTime += Main.dayLength;
            }

            // Add time from argument
            // 从参数中添加时间
            fullTime += extraTime;
            // Cap the time when the cycle time range is exceeded (fullTime < 0 || fullTime > 86600)
            // 当周期时间范围超出限制时，将时间戳设为最大值 (fullTime < 0 || fullTime > 86600)
            fullTime %= cycleLength;
            if (fullTime < 0)
            {
                fullTime += cycleLength;
            }

            // If fullTime (0-86600) < dayLength (54000) its a day, otherwise night
            // 如果 fullTime (0-86600) 小于 dayLength (54000)，则为白天，否则为晚上
            Main.dayTime = fullTime < Main.dayLength;
            // Convert cycle time to default day/night time
            // 将循环时间转换为默认的白天/黑夜时间
            if (!Main.dayTime)
            {
                fullTime -= Main.dayLength;
            }
            Main.time = fullTime;

            // Sync of world data on the server in MP
            // 在多人游戏中同步服务器上的世界数据
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.WorldData);
            }
        }
    }
}