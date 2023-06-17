using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.DataStructures;
using AnalysisMod.Staitd.ProjectE_Etr;

namespace AnalysisMod.AnalysisCommon.Commands
{
    public class SetETR : ModCommand
    {
        public override CommandType Type
            => CommandType.Chat;

        public override string Command
            => "Analysis_ETR";

        public override string Usage
            => "/Analysis_ETR ETRINTGET" +
            "\n ETRINTGET — 你需要给予自己的ETR值.";

        public override string Description
            => "修改玩家的ETR数值";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            EtrSyter modPlayer = Main.LocalPlayer.GetModPlayer<EtrSyter>();

            int GetEtr=0;

            if (args.Length == 0)
            {
                throw new UsageException($"你的ETR为{modPlayer.ETR}");
            }
            if (!int.TryParse(args[0], out int NewGetEtr))
            {
                throw new UsageException(args[0] + " is not a correct integer value.");
            }

            GetEtr = NewGetEtr;

            modPlayer.ETR += GetEtr;
        }

    }
}
