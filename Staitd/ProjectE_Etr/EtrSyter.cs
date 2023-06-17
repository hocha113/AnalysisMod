using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AnalysisMod.Staitd.ProjectE_Etr
{
    public class EtrSyter : ModPlayer
    {
        public int ETR = 0;

        //【保存属于本地玩家的ETR属性】
        public override void SaveData(TagCompound tag)
        {
            if (ETR != 0)
            {
                tag["ETR"] = ETR;
            }
        }

        //【加载属于本地玩家的ETR属性】
        public override void LoadData(TagCompound tag)
        {
                ETR = tag.GetInt("ETR");
        }
    }
}
