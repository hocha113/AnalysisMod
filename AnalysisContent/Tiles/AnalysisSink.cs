using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AnalysisMod.AnalysisContent.Tiles
{
    public class AnalysisSink : ModTile
    {
        public override void SetStaticDefaults()
        {
            // Hielo! As you may have noticed, this is a sink --- and as such, it ought to be a water source, right?
            // Well, let's do it one better, shall we?

            // 嗨，你可能已经注意到了，这是一个水槽——因此，它应该是一个水源，对吧？
            // 那么，我们能做得更好一些吗？
            TileID.Sets.CountsAsWaterSource[Type] = true;
            TileID.Sets.CountsAsHoneySource[Type] = true;
            TileID.Sets.CountsAsLavaSource[Type] = true;
            // By using these three sets, we've registered our sink as counting as a water, lava, and honey source for crafting purposes! The future is now.
            // Each one works individually and independently of the other two, so feel free to make your sink a source for whatever you'd like it to be!

            // 通过使用这三个设置，我们将我们的水槽注册为可用于制作目的的水、岩浆和蜂蜜来源！现在就可以实现未来。
            // 每个设置都独立地工作，并且与其他两个无关。所以请随意将您的水槽变成任何您想要的来源！

            // ...modded liquids sold separately.
            // ...附加模组液体需另行购买。

            Main.tileSolid[Type] = false;
            Main.tileLavaDeath[Type] = false;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(100, 100, 100), Language.GetText("MapObject.Sink"));

            DustType = 84;
            AdjTiles = new int[] { Type };
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}