using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;
using System.IO;

namespace AnalysisMod.AnalysisCommon.GlobalProjectiles
{
    // Here is a class dedicated to showcasing Send/ReceiveExtraAI()
    // 这是一个专门展示Send/ReceiveExtraAI()的类
    public class AnalysisProjectileNetSync : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        private bool differentBehaviour;
        private float distance;

        // This reduces how many projectiles actually have this GlobalProjectile
        // 这减少了实际拥有此GlobalProjectile的弹幕数量
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            return entity.type == ProjectileID.SharknadoBolt;
        }

        // Although this runs on both client and server, only the session that spawned the projectile knows its source
        // As such, the check demonstrated below will always be false client-side and the code will never run!

        // 尽管这在客户端和服务器上都运行，但只有生成弹幕的会话才知道其来源
        // 因此，下面演示的检查在客户端始终为false，并且代码永远不会运行！
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {

            // When spawned by Duke Fishron during a Blood Moon
            // 当由Duke Fishron在血月期间生成时
            if (source is EntitySource_Parent parent
                && parent.Entity is NPC npc
                && npc.type == NPCID.DukeFishron
                && Main.bloodMoon)
            {

                differentBehaviour = true;
                distance = projectile.Distance(Main.player[npc.target].Center);
            }
        }

        // Because this GlobalProjectile only applies to typhoons, this data is not attached to all projectile sync packets
        // 由于该GlobalProjectile仅适用于台风，因此该数据未附加到所有弹幕同步包中
        public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            bitWriter.WriteBit(differentBehaviour);

            // This check further avoids sending distance when it wouldn't be necessary
            // 此检查进一步避免了在不必要时发送距离
            if (differentBehaviour)
            {
                binaryWriter.Write(distance);
            }
        }

        // Make sure you always read exactly as much data as you sent!
        // 确保您始终读取与发送的完全相同的数据！
        public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
        {
            differentBehaviour = bitReader.ReadBit();

            if (differentBehaviour)
            {
                distance = binaryReader.ReadSingle();
            }
        }

        public override void AI(Projectile projectile)
        {
            if (differentBehaviour)
            {
                int p = Player.FindClosest(projectile.position, projectile.width, projectile.height);
                float currentDistance = p == -1 ? 0 : projectile.Distance(Main.player[p].Center);
                int dustType = DustID.GemSapphire;

                // Ends behaviour when in very close range
                // 在非常近距离内结束行为
                if (currentDistance < distance / 4)
                {
                    differentBehaviour = false;
                    projectile.netUpdate = true;
                }
                // Move at normal speed but can speed back up
                // 以正常速度移动但可以加速回来
                else if (currentDistance < distance / 2)
                {
                    projectile.extraUpdates = 0;
                }
                // Becomes faster when out of range
                // 越出范围后变得更快
                else
                {
                    projectile.extraUpdates = 1;
                    dustType = DustID.GemRuby;
                }

                // Visually indicates this typhoon has special behaviour and which mode it is in
                // 视觉上指示这个台风具有特殊行为并显示它所处模式aaaaaaaaaaaa
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, Scale: 5f);
                Main.dust[d].noGravity = true;
            }
        }
    }
}
