using AnalysisMod.AnalysisContent.Tiles;
using System.IO;
using Terraria;
using Terraria.GameContent.Tile_Entities;
using Terraria.ID;
using Terraria.ModLoader.Default;

namespace AnalysisMod.AnalysisContent.TileEntities
{
    /// <summary>
    /// This TileEntity is used in direct tandem with <seealso cref="AnalysisPylonTileAdvanced"/> in order to grant more flexibility than
    /// vanilla's normal pylon TileEntity (AKA <seealso cref="TETeleportationPylon"/>) using the <seealso cref="TEModdedPylon"></seealso> class
    /// that is built into tML itself.<br/>
    /// 这个TileEntity与<seealso cref="AnalysisPylonTileAdvanced"/>直接配合使用，以比普通的传送门石柱 TileEntity（也称为 <seealso cref="TETeleportationPylon"/>) 更具灵活性，
    /// 使用内置于tML中的 <seealso cref="TEModdedPylon"></seealso> 类。
    /// <para>
    /// The main Analysis shown here is having a Pylon that is only active at completely random intervals.<br/>
    /// 这里展示了主要分析：一个仅在完全随机时间间隔处于激活状态的石柱。
    /// </para>
    /// </summary>
    public class AdvancedPylonTileEntity : TEModdedPylon
    {
        // This is the main crux of this TileEntity; its pylon functionality will only work when this boolean is true.
        // 这是这个TileEntity的核心；只有当布尔值为true时，它才能发挥其石柱功能。
        public bool isActive;

        public override void OnNetPlace()
        {
            // This hook is only ever called on the server; its purpose is to give more freedom in terms of syncing FROM the server to clients, which we take advantage of
            // by making sure to sync whenever this hook is called:
            // 此钩子仅在服务器上调用；其目的是提供更多自由度，以便从服务器到客户端进行同步。我们利用此功能：
            NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);
        }

        public override void NetSend(BinaryWriter writer)
        {
            // We want to make sure that our data is synced properly across clients and server.
            // NetSend is called whenever a TileEntitySharing message is sent, so the game will handle this automatically for us,
            // granted that we send a message when we need to.

            // 我们希望确保我们的数据在客户端和服务器之间正确同步。
            // 每当发送 TileEntitySharing 消息时都会调用 NetSend，因此游戏将自动处理此操作，
            // 只要我们需要发送消息即可。

            writer.Write(isActive);
        }

        public override void NetReceive(BinaryReader reader)
        {
            isActive = reader.ReadBoolean();
        }

        public override void Update()
        {
            // Update is only ever called on the Server or in SinglePlayer, so our randomness will be in that frame of reference
            // Every tick, there will be a 1/180 chance that the active state of this pylon will swap (ON to OFF or vice versa)

            // Update 仅在服务器或单人游戏中调用，因此我们将随机性放在该参考框架中
            // 每一帧都有1/180 的几率使得该石柱处于激活状态交替（ON 到 OFF 或反之）
            if (!Main.rand.NextBool(180))
            {
                return;
            }

            // Granted that the check passes, we change the active state, and if this is on the server, we sync it with the server:
            // 如果检查通过，则改变激活状态，并且如果在服务器上，则将其与服务器同步：
            isActive = !isActive;
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);
            }
        }
    }
}
