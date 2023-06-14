using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace AnalysisMod.AnalysisContent.Tiles
{
    public class AnalysisOre : ModTile
    {
        public override void SetStaticDefaults()
        {
            TileID.Sets.Ore[Type] = true;
            Main.tileSpelunker[Type] = true; // The tile will be affected by spelunker highlighting
                                             // �����ש���ܵ���Ѩ̽���߸�����Ӱ�졣

            Main.tileOreFinderPriority[Type] = 410; // Metal Detector value, see https://terraria.wiki.gg/wiki/Metal_Detector
                                                    // ����̽������ֵ����μ� https://terraria.wiki.gg/wiki/Metal_Detector

            Main.tileShine2[Type] = true; // Modifies the draw color slightly.
                                          // ��΢�޸Ļ�����ɫ��

            Main.tileShine[Type] = 975; // How often tiny dust appear off this tile. Larger is less frequently
                                        // �˴�ש�ϳ���΢С������Ƶ�ʡ�����Խ�󣬳��ִ���Խ�١�

            Main.tileMergeDirt[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(152, 171, 198), name);

            DustType = 84;
            HitSound = SoundID.Tink;
            // MineResist = 4f;
            // MinPick = 200;
        }
    }

    public class AnalysisOreSystem : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            // Because world generation is like layering several images ontop of each other, we need to do some steps between the original world generation steps.
            // �����������ɾ����ڶ���ͼƬ֮����зֲ㣬���������Ҫ��ԭʼ�������ɲ���֮��ִ��һЩ������

            // Most vanilla ores are generated in a step called "Shinies", so for maximum compatibility, we will also do this.
            // First, we find out which step "Shinies" is.

            // �����������Ϸ�еĿ��ﶼ���ڡ����⡱�׶����ɵģ�Ϊ�����̶ȵ���߼����ԣ�����Ҳ����ȡ��ͬ������
            // �����ҳ������⡱�����Ľ׶Ρ�
            int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shinies"));

            if (ShiniesIndex != -1)
            {
                // Next, we insert our pass directly after the original "Shinies" pass.
                // AnalysisOrePass is a class seen bellow

                // ����������ԭʼ�����⡱�׶κ�ֱ�Ӳ��������Լ��Ĺ��̡�
                // AnalysisOrePass �����濴����һ����
                tasks.Insert(ShiniesIndex + 1, new AnalysisOrePass("Analysis Mod Ores", 237.4298f));
            }
        }
    }

    public class AnalysisOrePass : GenPass
    {
        public AnalysisOrePass(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            // progress.Message is the message shown to the user while the following code is running.
            // Try to make your message clear. You can be a little bit clever, but make sure it is descriptive enough for troubleshooting purposes.

            // progress.Message ���������´���ʱ���û���ʾ����Ϣ��
            // ����ʹ������Ϣ�����׶���������΢����һ�㣬����ȷ�����Թ����ų����㹻�����ԡ�
            progress.Message = "Analysis Mod Ores";

            // Ores are quite simple, we simply use a for loop and the WorldGen.TileRunner to place splotches of the specified Tile in the world.
            // "6E-05" is "scientific notation". It simply means 0.00006 but in some ways is easier to read.

            // ����ܼ򵥣�����ֻ��ʹ�� for ѭ���� WorldGen.TileRunner �������з���ָ�� Tile �İ߿鼴�ɡ�
            // ��6E-05���ǿ�ѧ������������ʾ 0.00006������ĳЩ����������Ķ���
            for (int k = 0; k < (int)(Main.maxTilesX * Main.maxTilesY * 6E-05); k++)
            {
                // The inside of this for loop corresponds to one single splotch of our Ore.
                // First, we randomly choose any coordinate in the world by choosing a random x and y value.

                // �� for ѭ�����ڲ���Ӧ�����ǿ����һ���߿顣
                // ���ȣ�����ͨ��ѡ����� x �� y ֵ�����ѡ�������е��κ����ꡣ
                int x = WorldGen.genRand.Next(0, Main.maxTilesX);

                // WorldGen.worldSurfaceLow is actually the highest surface tile. In practice you might want to use WorldGen.rockLayer or other WorldGen values.
                // WorldGen.worldSurfaceLow ʵ��������߱����ש��ʵ�ʲ����У�������ϣ��ʹ�� WorldGen.rockLayer ������ WorldGen ֵ��

                int y = WorldGen.genRand.Next((int)GenVars.worldSurfaceLow, Main.maxTilesY);

                // Then, we call WorldGen.TileRunner with random "strength" and random "steps", as well as the Tile we wish to place.
                // Feel free to experiment with strength and step to see the shape they generate.

                // Ȼ�����ǵ��þ��������ǿ�ȡ�����������衱�� WorldGen.TileRunner����ָ��Ҫ���õ� Tile��
                // ���Գ��Բ�ͬ�� strength �� step ���鿴�������ɵ���״��
                WorldGen.TileRunner(x, y, WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(2, 6), ModContent.TileType<AnalysisOre>());

                // Alternately, we could check the tile already present in the coordinate we are interested.
                // Wrapping WorldGen.TileRunner in the following condition would make the ore only generate in Snow.
                // Tile tile = Framing.GetTileSafely(x, y);
                // if (tile.HasTile && tile.TileType == TileID.SnowBlock) {
                // 	WorldGen.TileRunner(.....);

                // ���ߣ����ǿ��Լ�����Ȥ���괦�Ѿ����ڵ� Tile��
                // �� WorldGen.TileRunner ��װ�����������н�ʹ�ÿ������ѩ�������ɡ�
                // Tile tile = Framing.GetTileSafely(x, y);
                // if (tile.HasTile && tile.TileType == TileID.SnowBlock) {
                // WorldGen.TileRunner(.....);
                // }
            }
        }
    }
}
