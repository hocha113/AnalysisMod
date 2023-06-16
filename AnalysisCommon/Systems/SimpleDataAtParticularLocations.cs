using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

/// <summary>
/// This Analysis illustrates a solution for storing Small-Sparse-Simple data at locations. The definitions of those are as follows:
/// Small/Large - < 10 locations are actively using the data per frame is small, > 10 is large. Use UNRELEASEDSYSTEM1 to do large-X-simple data.
/// Sparse/Filled - Sparse is that not all locations will have data, typically less than 60% in the world will have data. Use UNRELEASEDSYSTEM1 to do large-X-simple data.
/// Simple/Complex - Sorta arbitrary. Simple data will not contain methods, nor complicated functionality, and typically is just basic data types. Use TileEntities if working with complex data.
/// 这个分析展示了一种在位置上存储小型稀疏简单数据的解决方案。它们的定义如下：
/// - 小/大 - 每帧使用该数据的位置少于10个为小，多于10个为大。对于大X简单数据，请使用UNRELEASEDSYSTEM1。
/// - 稀疏/填充 - 稀疏表示不是所有位置都有数据，通常世界中不到60%的位置会有数据。对于大X简单数据，请使用UNRELEASEDSYSTEM1。
/// - 简单/复杂 - 有点难以区分。简单的数据不包含方法或复杂功能，并且通常只是基本类型的数据。如果处理复杂数据，请使用TileEntities。
/// </summary>

///			Some other common use cases not Analysisd:
/// Getting data for a particular tile type your mod added, that was placed in world:
///		Trigger fetch of data using adjTiles[type]. If data is ordered, use appropriate version of PosData.Lookup. If data is not ordered, you will likely need to find via enumeration.
///		If it is unordered additions, you may elect to build myMap yourself OR attempt to insert the data so it remains ordered. The latter will lead to better post-event performance.
///	Clustering data to achieve sparsity:
///		If your application has multiple repeat static data in a row, you should elect to use Clustered mode in the builder to compress it. Note that you should NOT use PosData.LookupExact in this case.
///		    还有其他一些常见用例未被分析：
/// - 获取你添加到游戏中特定方块类型的信息：触发adjTiles[type]来获取信息。如果信息已排序，则使用适当版本的PosData.Lookup；如果没有排序，则可能需要通过枚举查找。
///     - 聚类以实现稀疏性：如果应用程序中存在多个重复静态连续数据，则应选择在构建器中使用聚类模式进行压缩。请注意，在这种情况下，您不应该使用PosData.LookupExact。


// Future TODO: Improve documentation.
// 未来计划：改进文档。
namespace AnalysisMod.AnalysisCommon.Systems
{
    // Saving and loading requires TagCompounds, a guide exists on the wiki:
    // 保存和加载需要TagCompounds，在wiki上有指南：
    // https://github.com/tModLoader/tModLoader/wiki/Saving-and-loading-using-TagCompound
    public class SimpleDataAtParticularLocations : ModSystem
    {
        // Create our map. Uses generics for whatever type you want of the data to store.
        // 创建我们自己想要存储的数据类型的泛型地图。
        public PosData<byte>[] myMap;

        // Next, we ensure we initialize the map on world load to an empty map.
        // 接下来，我们确保在世界加载时将地图初始化为空地图。
        public override void ClearWorld()
        {
            myMap = new PosData<byte>[0];
        }

        // We save our data sets using TagCompounds.
        // NOTE: The tag instance provided here is always empty by default.

        // 我们使用TagCompounds保存我们的数据集。
        // 注意：此处提供的标签实例默认始终为空。
        public override void SaveWorldData(TagCompound tag)
        {
            if (myMap.Length != 0)
            {
                tag["myMap"] = myMap.Select(info => new TagCompound
                {
                    ["pos"] = info.pos,
                    ["data"] = info.value
                }).ToList();
            }
        }

        // We load our data sets using the provided TagCompound. Should mirror SaveWorldData()
        // 我们使用提供的TagCompound加载我们的数据集。应该与SaveWorldData()相对应。
        public override void LoadWorldData(TagCompound tag)
        {

            List<PosData<byte>> list = new List<PosData<byte>>();
            foreach (var entry in tag.GetList<TagCompound>("myMap"))
            {
                list.Add(new PosData<byte>(
                    entry.GetInt("pos"),
                    entry.Get<byte>("data")
                ));
            }
            myMap = list.ToArray();
        }

        // We define what we want to generate as additional location data, for this Analysis, in PostWorldGen.
        // We will create a simple column of byte data going down the horizontal center of the world that we will later use in PreUpdateWorld.

        // 在PostWorldGen中定义我们要生成为附加位置数据。
        // 对于这个分析，我们将创建一个简单的字节列，在世界水平中心向下延伸，并在PreUpdateWorld中使用它。
        public override void PostWorldGen()
        {
            var builder = new PosData<byte>.OrderedSparseLookupBuilder(compressEqualValues: false);

            int xCenter = Main.maxTilesX / 2;

            for (int y = 0; y < Main.maxTilesY; y++)
            {
                builder.Add(
                    xCenter, y, // The locations

                    (byte)(y % 255) // The data we want to store at the location
                                    // 需要存储在位置上的数据
                );
            }

            myMap = builder.Build();
        }

        // We call our custom method after testing that our map isn't empty - this ensures safe-loading on previous generated worlds!
        // 在测试地图不为空的情况下调用我们的自定义方法 - 这确保了在之前生成的世界上进行安全加载！
        public override void PreUpdateWorld()
        {
            if (myMap.Length == 0)
            {
                return;
            }
            foreach (var player in Main.player)
            {
                if (player.active)
                {
                    UpdateFromNearestInMap(player);
                }
            }
        }

        // We use the column at world center to paint nearby tiles based on the player's proximity to the nearest entry in the map.
        // In this case, the nearest entry should correspond to the player's depth.

        // 我们使用位于世界中心的列来根据玩家与地图中最近条目之间的距离绘制附近瓷砖。
        // 在这种情况下，最近条目应该对应于玩家所处深度。
        public void UpdateFromNearestInMap(Player player)
        {
            // Get player position in tile coordinates
            // 获取玩家在瓷砖坐标系中的位置
            Point z = player.position.ToTileCoordinates();

            // Search for an entry within 32 tiles of our player
            // 搜索距离我们玩家32个方块内是否有条目
            if (PosData.NearbySearchOrderedPosMap(myMap, z, 32, out var entry))
            {
                // If found, we grab the data from the corresponding output index
                // 如果找到，则获取相应输出索引处的数据
                var data = entry.value;

                // We then proceed to paint a 5x2 area around the player position with our locational custom values.
                // 然后，我们将围绕着玩家位置以我们定位自定义值为基础绘制一个5x2区域。
                for (int i = -2; i < 3; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        Tile tile = Main.tile[z.X + i, z.Y + j];
                        if (tile.HasTile)
                        {
                            tile.TileColor = data;
                        }
                    }
                }
            }
        }
    }
}