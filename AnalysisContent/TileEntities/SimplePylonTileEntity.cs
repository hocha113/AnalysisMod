using Terraria.ModLoader.Default;

namespace AnalysisMod.AnalysisContent.TileEntities
{
    /// <summary>
    /// This is an empty child class that acts exactly like the default implementation of the abstract <seealso cref="TEModdedPylon"/>
    /// class, which itself acts nearly identical to vanilla pylon TEs. This inheritance only exists so that modded pylon entities
    /// will properly have their "Mod" property set, for I/O purposes. Has the sealed modifier since this TE acts identical to its parent.<br/>
    /// 这是一个空的子类，其行为与抽象类 <seealso cref="TEModdedPylon"/> 的默认实现完全相同，
    /// 该抽象类本身的行为几乎与原版电塔 TE 相同。这种继承只存在于 modded pylon 实体中，
    /// 以便它们的 "Mod" 属性能够正确设置，用于输入输出目的。由于此 TE 的行为与其父级相同，因此具有密封修饰符。
    /// </summary>
    public sealed class SimplePylonTileEntity : TEModdedPylon { }
}
