using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisCommon.Commands
{
    public class AnalysisItemCommand : ModCommand
    {
        // CommandType.Chat means that command can be used in Chat in SP and MP
        // CommandType.Chat 表示该命令可在SP和MP的聊天中使用
        public override CommandType Type
            => CommandType.Chat;

        // The desired text to trigger this command
        // 触发此命令的所需文本
        public override string Command
            => "Analysis_give";

        // A short usage explanation for this command
        // 这个命令的简短使用说明
        public override string Usage
            => "/Analysis_give <type|name> [stack]" +
            "\n type — ItemID of item 物品的索引." +
            "\n name — name of Item in current localization 当前本地化的物品名称." +
            "\n Replace spaces in item name with underscores 需要将商品名称中的空格替换为下划线.";

        // A short description of this command
        // 这个命令的简短描述
        public override string Description
            => "Spawn an item by name or by typeId\n通过名称或类型ID生成物品";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            // Checking input Arguments
            // 检查输入参数
            if (args.Length == 0)
                throw new UsageException("At least one argument was expected.\n至少需要一个参数");

            // If we can't parse the int, it means we have a name (or a wrong use of the command)
            // In that case type be equal to 0
            // 如果我们无法解析整数，那么意味着我们有一个名称（或命令的错误使用）
            // 在这种情况下，类型将等于0 
            if (!int.TryParse(args[0], out int type))
            {
                // Replacing the underscore in an element name with spaces
                // 将元素名称中的下划线替换为空格
                string name = args[0].Replace("_", " ");

                // We go through all the subjects to find the required typeId
                // Only if the name of the item matches the desired one in the current localization (no case sensitive) 
                // 我们遍历所有的主题来找到所需的typeId
                // 仅当项目名称与当前本地化中所需的名称匹配（不区分大小写）时才进行
                for (int k = 1; k < ItemLoader.ItemCount; k++)
                {
                    if (name.ToLower() == Lang.GetItemNameValue(k).ToLower())
                    {
                        type = k;
                        break;
                    }
                }
            }

            if (type <= 0 || type >= ItemLoader.ItemCount)
                throw new UsageException(string.Format("Unknown item — Must be valid name or 0 < type < {0}", ItemLoader.ItemCount));

            // If the command has at least two arguments, we try to get the stack value
            // Default stack is 1
            // 如果命令至少有两个参数，我们尝试获取堆栈值
            // 默认堆栈为1 
            int stack = 1;
            if (args.Length >= 2)
            {
                if (!int.TryParse(args[1], out stack))
                    throw new UsageException("Stack value must be integer, but met: " + args[1]);
            }

            // Spawn the item where the calling player is
            // 在调用玩家所在位置生成物品
            caller.Player.QuickSpawnItem(new EntitySource_DebugCommand($"{nameof(AnalysisMod)}_{nameof(AnalysisItemCommand)}"), type, stack);
        }
    }
}