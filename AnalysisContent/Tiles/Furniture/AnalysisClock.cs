using AnalysisMod.AnalysisContent.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AnalysisMod.AnalysisContent.Tiles.Furniture
{
    public class AnalysisClock : ModTile
    {
        public override void SetStaticDefaults()
        {
            // Properties
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.Clock[Type] = true;

            DustType = ModContent.DustType<Sparkle>();
            AdjTiles = new int[] { TileID.GrandfatherClocks };

            // Placement
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 5;
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16, 16, 16 };
            TileObjectData.addTile(Type);

            // Etc
            AddMapEntry(new Color(200, 200, 200), Language.GetText("ItemName.GrandfatherClock"));
        }

        public override bool RightClick(int x, int y)
        {
            string text = "AM";
            // Get current weird time
            // 获取当前的时间
            double time = Main.time;
            if (!Main.dayTime)
            {
                // if it's night add this number
                // 如果是晚上，加上这个数字
                time += 54000.0;
            }

            // Divide by seconds in a day * 24
            // 除以一天中的秒数*24
            time = time / 86400.0 * 24.0;
            // Dunno why we're taking 19.5. Something about hour formatting
            // 不知道为什么要取19.5。大概是关于小时格式化的问题。
            time = time - 7.5 - 12.0;
            // Format in readable time
            // 格式化成可读时间
            if (time < 0.0)
            {
                time += 24.0;
            }

            if (time >= 12.0)
            {
                text = "PM";
            }

            int intTime = (int)time;
            // Get the decimal points of time.
            // 获取时间的小数点部分。
            double deltaTime = time - intTime;
            // multiply them by 60. Minutes, probably
            // 将它们乘以60。分钟，可能吧？
            deltaTime = (int)(deltaTime * 60.0);
            // This could easily be replaced by deltaTime.ToString()
            // 这可以很容易地被deltaTime.ToString()替换掉。
            string text2 = string.Concat(deltaTime);
            if (deltaTime < 10.0)
            {
                // if deltaTime is eg "1" (which would cause time to display as HH:M instead of HH:MM)
                // 如果deltaTime是例如“1”（这将导致时间显示为HH:M而不是HH:MM）
                text2 = "0" + text2;
            }

            if (intTime > 12)
            {
                // This is for AM/PM time rather than 24hour time
                // 这是针对AM/PM时间而不是24小时制的。
                intTime -= 12;
            }

            if (intTime == 0)
            {
                // 0AM = 12AM
                // 0AM = 12AM
                intTime = 12;
            }

            // Whack it all together to get a HH:MM format
            // 把所有东西都放在一起得到一个HH:MM格式
            Main.NewText($"Time: {intTime}:{text2} {text}", 255, 240, 20);
            return true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}