using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Placeable
{
    public class AnalysisOre : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            ItemID.Sets.SortingPriorityMaterials[Item.type] = 58;

            // This ore can spawn in slime bodies like other pre-boss ores. (copper, tin, iron, etch)
            // It will drop in amount from 3 to 13.

            // 这种矿石可以像其他BOSS前矿石一样在史莱姆身上生成。(铜、锡、铁等)
            // 它的掉落数量为3到13个。
            ItemID.Sets.OreDropsFromSlime[Type] = (3, 13);
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.AnalysisOre>());
            Item.width = 12;
            Item.height = 12;
            Item.value = 3000;
        }
    }
}