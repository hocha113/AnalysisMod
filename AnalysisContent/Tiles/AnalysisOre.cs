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
                                             // 这个瓷砖会受到洞穴探险者高亮的影响。

            Main.tileOreFinderPriority[Type] = 410; // Metal Detector value, see https://terraria.wiki.gg/wiki/Metal_Detector
                                                    // 金属探测器数值，请参见 https://terraria.wiki.gg/wiki/Metal_Detector

            Main.tileShine2[Type] = true; // Modifies the draw color slightly.
                                          // 略微修改绘制颜色。

            Main.tileShine[Type] = 975; // How often tiny dust appear off this tile. Larger is less frequently
                                        // 此瓷砖上出现微小尘埃的频率。数字越大，出现次数越少。

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
            // 由于世界生成就像在多张图片之间进行分层，因此我们需要在原始世界生成步骤之间执行一些操作。

            // Most vanilla ores are generated in a step called "Shinies", so for maximum compatibility, we will also do this.
            // First, we find out which step "Shinies" is.

            // 大多数基础游戏中的矿物都是在“闪光”阶段生成的，为了最大程度地提高兼容性，我们也将采取相同方法。
            // 首先找出“闪光”所处的阶段。
            int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shinies"));

            if (ShiniesIndex != -1)
            {
                // Next, we insert our pass directly after the original "Shinies" pass.
                // AnalysisOrePass is a class seen bellow

                // 接下来，在原始“闪光”阶段后直接插入我们自己的过程。
                // AnalysisOrePass 是下面看到的一个类
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

            // progress.Message 是运行以下代码时向用户显示的消息。
            // 尽量使您的消息清晰易懂。可以稍微聪明一点，但请确保它对故障排除有足够描述性。
            progress.Message = "Analysis Mod Ores";

            // Ores are quite simple, we simply use a for loop and the WorldGen.TileRunner to place splotches of the specified Tile in the world.
            // "6E-05" is "scientific notation". It simply means 0.00006 but in some ways is easier to read.

            // 矿物很简单，我们只需使用 for 循环和 WorldGen.TileRunner 在世界中放置指定 Tile 的斑块即可。
            // “6E-05”是科学计数法。它表示 0.00006，但在某些方面更易于阅读。
            for (int k = 0; k < (int)(Main.maxTilesX * Main.maxTilesY * 6E-05); k++)
            {
                // The inside of this for loop corresponds to one single splotch of our Ore.
                // First, we randomly choose any coordinate in the world by choosing a random x and y value.

                // 此 for 循环的内部对应于我们矿物的一个斑块。
                // 首先，我们通过选择随机 x 和 y 值来随机选择世界中的任何坐标。
                int x = WorldGen.genRand.Next(0, Main.maxTilesX);

                // WorldGen.worldSurfaceLow is actually the highest surface tile. In practice you might want to use WorldGen.rockLayer or other WorldGen values.
                // WorldGen.worldSurfaceLow 实际上是最高表面瓷砖。实际操作中，您可能希望使用 WorldGen.rockLayer 或其他 WorldGen 值。

                int y = WorldGen.genRand.Next((int)GenVars.worldSurfaceLow, Main.maxTilesY);

                // Then, we call WorldGen.TileRunner with random "strength" and random "steps", as well as the Tile we wish to place.
                // Feel free to experiment with strength and step to see the shape they generate.

                // 然后，我们调用具有随机“强度”和随机“步骤”的 WorldGen.TileRunner，并指定要放置的 Tile。
                // 可以尝试不同的 strength 和 step 来查看它们生成的形状。
                WorldGen.TileRunner(x, y, WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(2, 6), ModContent.TileType<AnalysisOre>());

                // Alternately, we could check the tile already present in the coordinate we are interested.
                // Wrapping WorldGen.TileRunner in the following condition would make the ore only generate in Snow.
                // Tile tile = Framing.GetTileSafely(x, y);
                // if (tile.HasTile && tile.TileType == TileID.SnowBlock) {
                // 	WorldGen.TileRunner(.....);

                // 或者，我们可以检查感兴趣坐标处已经存在的 Tile。
                // 将 WorldGen.TileRunner 包装在以下条件中将使该矿物仅在雪地中生成。
                // Tile tile = Framing.GetTileSafely(x, y);
                // if (tile.HasTile && tile.TileType == TileID.SnowBlock) {
                // WorldGen.TileRunner(.....);
                // }
            }
        }
    }
}
