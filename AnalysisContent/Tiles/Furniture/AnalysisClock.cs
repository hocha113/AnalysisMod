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
            // ��ȡ��ǰ��ʱ��
            double time = Main.time;
            if (!Main.dayTime)
            {
                // if it's night add this number
                // ��������ϣ������������
                time += 54000.0;
            }

            // Divide by seconds in a day * 24
            // ����һ���е�����*24
            time = time / 86400.0 * 24.0;
            // Dunno why we're taking 19.5. Something about hour formatting
            // ��֪��ΪʲôҪȡ19.5������ǹ���Сʱ��ʽ�������⡣
            time = time - 7.5 - 12.0;
            // Format in readable time
            // ��ʽ���ɿɶ�ʱ��
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
            // ��ȡʱ���С���㲿�֡�
            double deltaTime = time - intTime;
            // multiply them by 60. Minutes, probably
            // �����ǳ���60�����ӣ����ܰɣ�
            deltaTime = (int)(deltaTime * 60.0);
            // This could easily be replaced by deltaTime.ToString()
            // ����Ժ����׵ر�deltaTime.ToString()�滻����
            string text2 = string.Concat(deltaTime);
            if (deltaTime < 10.0)
            {
                // if deltaTime is eg "1" (which would cause time to display as HH:M instead of HH:MM)
                // ���deltaTime�����硰1�����⽫����ʱ����ʾΪHH:M������HH:MM��
                text2 = "0" + text2;
            }

            if (intTime > 12)
            {
                // This is for AM/PM time rather than 24hour time
                // �������AM/PMʱ�������24Сʱ�Ƶġ�
                intTime -= 12;
            }

            if (intTime == 0)
            {
                // 0AM = 12AM
                // 0AM = 12AM
                intTime = 12;
            }

            // Whack it all together to get a HH:MM format
            // �����ж���������һ��õ�һ��HH:MM��ʽ
            Main.NewText($"Time: {intTime}:{text2} {text}", 255, 240, 20);
            return true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}