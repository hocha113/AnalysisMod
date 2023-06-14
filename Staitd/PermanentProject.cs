using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AnalysisMod.Staitd
{
    public class MyModPlayer : ModPlayer
    {
        public int ETR=0;

        public override void Initialize()
        {
            ETR = 0;
        }

        public TagCompound Save()
        {
            return new TagCompound
        {
            {"ETR", ETR}
        };
        }

        public void Load(TagCompound tag)
        {
            ETR = tag.GetInt("ETR");
        }

        public void OnEnterWorld(Player player)
        {
            LoadData(player);
        }

        public void LoadData(Player player)
        {
            Mod mod = ModLoader.GetMod("MyMod"); // 将"MyMod"替换为你的模组名称
            MyModPlayer modPlayer = player.GetModPlayer<MyModPlayer>();
            modPlayer.ETR = player.GetModPlayer<MyModPlayer>().ETR;
        }

        public void OnExitWorld(Player player)
        {
            SaveData(player);
        }

        public void SaveData(Player player)
        {
            Mod mod = ModLoader.GetMod("MyMod"); // 将"MyMod"替换为你的模组名称
            MyModPlayer modPlayer = player.GetModPlayer<MyModPlayer>();
            player.GetModPlayer<MyModPlayer>().ETR = modPlayer.ETR;
        }
    }
}
