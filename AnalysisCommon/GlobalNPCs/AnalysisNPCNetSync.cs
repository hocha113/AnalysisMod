using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AnalysisMod.AnalysisCommon.GlobalNPCs
{
    // Here is a class dedicated to showcasing Send/ReceiveExtraAI()
    // 这里是一个专门展示Send/ReceiveExtraAI()的类
    public class AnalysisNPCNetSync : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        private bool differentBehavior;

        // This reduces how many NPCs actually have this GlobalNPC
        // 这样可以减少实际拥有这个GlobalNPC的NPC数量
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return entity.type == NPCID.Sharkron2;
        }

        // Although this runs on both client and server, only the session that spawned the NPC knows its source
        // As such, the check demonstrated below will always be false client-side and the code will never run!

        // 尽管它在客户端和服务器上都运行，但只有生成NPC的会话知道其来源
        // 因此，下面演示的检查在客户端始终为false，代码永远不会运行！
        public override void OnSpawn(NPC npc, IEntitySource source)
        {

            // When spawned by a Cthulunado during a Blood Moon
            // 当由血月期间Cthulunado生成时
            if (source is EntitySource_Parent parent
                && parent.Entity is Projectile projectile
                && projectile.type == ProjectileID.Cthulunado
                && Main.bloodMoon)
            {
                differentBehavior = true;
            }
        }

        // Because this GlobalNPC only applies to Sharkrons, this data is not attached to all NPC sync packets
        // 由于这个GlobalNPC仅适用于Sharkron，因此这些数据未附加到所有NPC同步包中
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            bitWriter.WriteBit(differentBehavior);
        }

        // Make sure you always read exactly as much data as you sent!
        // 确保您始终读取与发送的数据完全相同！
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            differentBehavior = bitReader.ReadBit();
        }

        public override void AI(NPC npc)
        {
            if (differentBehavior)
            {
                npc.scale *= 1.0025f;
                if (npc.scale > 3f)
                {
                    npc.scale = 3f;
                }
            }
        }
    }
}