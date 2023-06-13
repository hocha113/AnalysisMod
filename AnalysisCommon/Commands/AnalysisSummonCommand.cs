using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.DataStructures;

namespace AnalysisMod.AnalysisCommon.Commands
{
    public class AnalysisSummonCommand : ModCommand
    {
        // CommandType.World means that command can be used in Chat in SP and MP, but executes on the Server in MP
        //CommandType.World表示该命令可在SP和MP中用于聊天，但在MP中在服务器上执行。
        public override CommandType Type
            => CommandType.World;

        // The desired text to trigger this command
        //触发此命令的所需文本
        public override string Command
            => "Analsis_summon";

        // A short usage explanation for this command
        //该命令的简要使用说明
        public override string Usage
            => "/Analsis_summon type [[~]x] [[~]y] [number]" +
            "\n type - NPCID of NPC 生成的实体的ID索引." +
            "\n x and y - position of spawn 实体生成位置." +
            "\n ~ - to use position relative to player 实体生成在相对于玩家的什么位置." +
            "\n number - number of NPC's to spawn 生成的实体数量.";

        // A short description of this command
        //这个命令的简短描述
        public override string Description
            => "Spawn a NPC by NPCID" +
            "\n 通过ID索引生成实体";

        //Action这个命令触发后将执行的代码
        //caller==调用者
        //input==输入的字符串数组 ' [[~]x] [[~]y] [number] '
        //args==用来储存该字符串数组的数组的名称
        public override void Action(CommandCaller caller, string input, string[] args)
        {
            // Checking input Arguments
            //检查输入参数
            if (args.Length == 0)
            {
                throw new UsageException("At least one argument was expected\n至少需要一个参数.");
            }
            if (!int.TryParse(args[0], out int type))
            {
                throw new UsageException(args[0] + " is not a correct integer value\n不是正确的整数值.");
            }

            // Default values for spawn
            // Position - Player.Bottom, number of NPC - 1 
            //生成NPC的默认值
            //位置-玩家底部，NPC数量-1
            int xSpawnPosition = (int)caller.Player.Bottom.X;
            int ySpawnPosition = (int)caller.Player.Bottom.Y;
            int numToSpawn = 1;
            bool relativeX = false;
            bool relativeY = false;

            // If command has X position argument
            //如果命令有X位置参数
            if (args.Length > 1)
            {
                // X relative check
                // X轴相对校验
                //因为args是二维数组，所以这里使用了二维的索引，检测第二个元素的第一个字符
                if (args[1][0] == '~')
                {
                    relativeX = true;
                    args[1] = args[1].Substring(1);
                }
                // Parsing X position
                //解析X位置
                if (!int.TryParse(args[1], out xSpawnPosition))
                {
                    throw new UsageException(args[1] + " is not a correct X position (must be valid integer value) + " +
                        "\n不是正确的X位置（必须是有效的整数值）.");
                }
            }

            // If command has Y position argument
            //如果命令有 Y 位置参数
            if (args.Length > 2)
            {
                // Y relative check
                //Y轴相对位置检查
                if (args[2][0] == '~')
                {
                    relativeY = true;
                    args[2] = args[2].Substring(1);
                }
                // Parsing Y position
                //解析Y位置
                if (!int.TryParse(args[2], out ySpawnPosition))
                {
                    throw new UsageException(args[2] + " is not a correct Y position (must be valid integer value) + " +
                        "\nY位置不正确（必须是有效的整数值）.");
                }
            }

            // Adjusting the positions if they are relative
            //如果它们是相对位置，则调整位置
            if (relativeX)
            {
                xSpawnPosition += (int)caller.Player.Bottom.X;
            }
            if (relativeY)
            {
                ySpawnPosition += (int)caller.Player.Bottom.Y;
            }

            // If command has number argument
            //如果命令有数字参数
            if (args.Length > 3)
            {
                if (!int.TryParse(args[3], out numToSpawn))
                {
                    throw new UsageException(args[3] + " is not a correct number (must be valid integer value) + " +
                        "\n不是一个正确的数字（必须是有效的整数值）.");
                }
            }

            for (int k = 0; k < numToSpawn; k++)
            {
                // Spawning numToSpawn NPCs with a given postions and type
                // NPC.NewNPC return 200 (Main.maxNPCs) if there are not enough NPC slots to spawn
                // 生成numToSpawn个指定位置和类型的NPC
                // 如果没有足够的NPC插槽来生成，则NPC.NewNPC返回200（Main.maxNPCs）
                int slot = NPC.NewNPC(new EntitySource_DebugCommand($"{nameof(AnalysisMod)}_{nameof(AnalysisSummonCommand)}"), xSpawnPosition, ySpawnPosition, type);

                // Sync of NPCs on the server in MP
                // 在多人游戏中同步服务器上的NPC 
                if (Main.netMode == NetmodeID.Server && slot < Main.maxNPCs)
                {
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, slot);
                }
            }
        }
    }
}
