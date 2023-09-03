using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.NPCs
{
    public enum WormSegmentType
    {
        /// <summary>
        /// ���ӵ�ͷ���ֽڡ������κθ����ĳ��ӣ�ֻ��һ����ͷ������Ϊ�ǻ�ġ�
        /// </summary>
        Head,
        /// <summary>
        /// ����ֽڡ�����ǰ��ķֽڡ�
        /// </summary>
        Body,
        /// <summary>
        /// β���ֽڡ��������������ͬ��AI�������κθ����ĳ��ӣ�ֻ����һ����β�͡���
        /// </summary>
        Tail
    }

    /// <summary>
    /// �Ƿ��������˵Ļ��ࡣ
    /// </summary>
    public abstract class Worm : ModNPC
    {
        /*  
         *  ai[]�÷���
         *
		 *  ai[0] = ��follower���Σ�������˶���Ķ���
         *  ai[1] = ���ڸ����Ƭ�εġ�following��Ƭ��
         *  
		 *  localAI [0] = ��ͬ�����ĵ���ײ���ʱʹ��
		 *  localAI [1] = ����Ƿ������Init����
		 */

        /// <summary>
        /// ���NPC����Ϊ���������͵�Ƭ��
        /// </summary>
        public abstract WormSegmentType SegmentType { get; }

        /// <summary>
        /// NPC����ٶ�
        /// </summary>
        public float MoveSpeed { get; set; }

        /// <summary>
        /// NPC�����ٶȵ�����
        /// </summary>
        public float Acceleration { get; set; }

        /// <summary>
        /// ������ NPC ��ͷ���ֽ�ʵ����
        /// </summary>
        public NPC HeadSegment => Main.npc[NPC.realLife];

        /// <summary>
        /// ��Ƭ�����ڸ����Ƭ�Σ�ai[1]�� �� NPC ʵ���� ����ͷ���ֽڣ�������ʼ�շ��� <see langword="null"/>��
        /// </summary>
        public NPC FollowingNPC => SegmentType == WormSegmentType.Head ? null : Main.npc[(int)NPC.ai[1]];

        /// <summary>
        /// ���ڸ����Ƭ�ϣ�ai[0]) �� NPC ʵ�� �� ����β���ֽڣ�������ʼ�շ��� <see langword="null"/>��
        /// </summary>
        public NPC FollowerNPC => SegmentType == WormSegmentType.Tail ? null : Main.npc[(int)NPC.ai[0]];

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return SegmentType == WormSegmentType.Head ? null : false;
        }

        private bool startDespawning;

        public sealed override bool PreAI()
        {
            if (NPC.localAI[1] == 0)
            {
                NPC.localAI[1] = 1f;
                Init();
            }

            if (SegmentType == WormSegmentType.Head)
            {
                HeadAI();

                if (!NPC.HasValidTarget)
                {
                    NPC.TargetClosest(true);

                    // ��� NPC �� boss ����û��Ŀ�꣬��Ѹ�ٵ����������
                    if (!NPC.HasValidTarget && NPC.boss)
                    {
                        NPC.velocity.Y += 8f;

                        MoveSpeed = 1000f;

                        if (!startDespawning)
                        {
                            startDespawning = true;

                            // ��� NPC �����㹻Զ������ 90 ticks (1.5 ��) ����ʧ
                            NPC.timeLeft = 90;
                        }
                    }
                }
            }
            else
                BodyTailAI();

            return true;
        }

        // �Թ��� API ���ɼ���������ָʾҪ���е� AI
        internal virtual void HeadAI() { }

        internal virtual void BodyTailAI() { }

        public abstract void Init();
    }

    /// <summary>
    /// ������ͷ���ֽ� NPC �Ļ��ࡣ
    /// </summary>
    public abstract class WormHead : Worm
    {
        public sealed override WormSegmentType SegmentType => WormSegmentType.Head;

        /// <summary>
        /// �����NPC��NPCID��ModContent.NPCType��<br/>
        /// ����<see cref="HasCustomBodySegments"/>����<see langword="false"/>ʱʹ�ô����ԡ�
        /// </summary>
        public abstract int BodyType { get; }

        /// <summary>
        /// β����NPC��NPCID��ModContent.NPCType��<br/>
        /// ����<see cref="HasCustomBodySegments"/>����<see langword="false"/>ʱʹ�ô����ԡ�
        /// </summary>
        public abstract int TailType { get; }

        /// <summary>
        /// ��������С�ֶ���������ͷ��β���ֶ�
        /// </summary>
        public int MinSegmentLength { get; set; }

        /// <summary>
        /// ���������ֶ���������ͷ��β���ֶ�
        /// </summary>
        public int MaxSegmentLength { get; set; }

        /// <summary>
        /// �Ƿ���Դ�ש��ײ�ԡ��ھ򡱴�����ש������ѩ��һ��������
        /// </summary>
        public bool CanFly { get; set; }

        /// <summary>
        /// �ھ����NPC�ڣ���<b>����</b>Ϊ��λ�������<see cref="CanFly"/>����<see langword="false"/>��������ƽ����ײ��⡣<br/>
        /// Ĭ��ֵΪ1000�����أ��൱��62.5��ƽ�̡�
        /// </summary>
        public virtual int MaxDistanceForUsingTileCollision => 1000;

        /// <summary> 
        /// �Ƿ�ʹ�ø�NPC
        /// </summary>
        public virtual bool HasCustomBodySegments => false;

        /// <summary>
        /// ������� <see langword="null"/> ����� NPC ����Ը�������λ�ö��������Ŀ�����Ŀ������
        /// </summary>
        public Vector2? ForcedTargetPosition { get; set; }

        /// <summary>
        /// ���Ǵ˷�����ʹ���Զ������ɴ��롣 <br/>
        /// ֻ���� <see cref="HasCustomBodySegments"/> ���� <see langword="true"/> ʱ�����д˷�����
        /// </summary>
        /// <param name="segmentCount">Ԥ��Ҫ���ɶ��ٸ�����Ƭ��</param>
        /// <returns>������ɵ� NPC �� whoAmI ֵ�����ǵ��� <see cref="NPC.NewNPC(IEntitySource, int, int, int, int, float, float, float, float, int)"/> �Ľ��</returns>
        public virtual int SpawnBodySegments(int segmentCount)
        {
            // Ĭ��ֻ���ش� NPC �� whoAmI����Ϊβ����ʹ�÷���ֵ��Ϊ�䡰���桱NPC����
            return NPC.whoAmI;
        }

        /// <summary>
        /// ���ɳ���������β���ֶΡ�
        /// </summary>
        /// <param name="source">����Դ</param><br/>
        /// <param name="type">Ҫ���ɵ�Ƭ�� NPC �� ID</param><br/>
        /// <param name="latestNPC">������������ɵ�Ƭ�� NPC������ͷ���� whoAmI ֵ</param><br/>
        /// <returns></returns>
        protected int SpawnSegment(IEntitySource source, int type, int latestNPC)
        {
            // ���ǲ���һ���µ� NPC���� latestNPC ����Ϊ���µ� NPC��ͬʱҲʹ��ͬһ����
            // ����������� NPC �ĸ����� �� NPC �ĸ�����������β�ͻ����岿�֣�
            // ����������� NPC ���ƶ���
            // ���������棬���ǻ��������³��ֵ�NPCʵ������ֵ����Ϊ������͹��ˡ�
            int oldLatest = latestNPC;
            latestNPC = NPC.NewNPC(source, (int)NPC.Center.X, (int)NPC.Center.Y, type, NPC.whoAmI, 0, latestNPC);

            Main.npc[oldLatest].ai[0] = latestNPC;

            NPC latest = Main.npc[latestNPC];
            //  npc.realLife realLife��NPC��whoAmI�����ɵ�NPC����֮����������ֵ
            latest.realLife = NPC.whoAmI;

            return latestNPC;
        }

        internal sealed override void HeadAI()
        {
            HeadAI_SpawnSegments();

            bool collision = HeadAI_CheckCollisionForDustSpawns();

            HeadAI_CheckTargetDistance(ref collision);

            HeadAI_Movement(collision);
        }

        private void HeadAI_SpawnSegments()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                // �������ǴӼ�鿪ʼAI���npc.ai [0] ������npcs˭ami����0��
                // �⼸�����Ǹող���һ�� npc ����������ζ�����ǵ�һ�θ��¡�
                // ��Ȼ���ǵ�һ�θ��£����ǿ��԰�ȫ�ؼ�����Ҫ�������ʣ�ಿ�֣�����+β�ͣ���

                bool hasFollower = NPC.ai[0] > 0;
                if (!hasFollower)
                {
                    // ���ԣ��������Ƿ�����NPC.realLifeֵ��
                    // npc.realLife ֵ��Ҫ����ȷ���ĸ� NPC �����ǻ��д� NPC ʱʧȥ������
                    // ���ǲ�ϣ������ÿһ���ֶ����Լ��� HP �أ���������һ���ܺõĽ��������
                    NPC.realLife = NPC.whoAmI;

                    // latestNPC ���� SpawnSegment() ��ʹ�ã��ҽ��������������
                    int latestNPC = NPC.whoAmI;

                    // ��������ȷ�����ĳ��ȡ�
                    int randomWormLength = Main.rand.Next(MinSegmentLength, MaxSegmentLength + 1);

                    int distance = randomWormLength - 2;

                    IEntitySource source = NPC.GetSource_FromAI();

                    if (HasCustomBodySegments)
                    {
                        // ���ô�����������εķ���
                        latestNPC = SpawnBodySegments(distance);
                    }
                    else
                    {
                        // ������һ����������Ƭ��
                        while (distance > 0)
                        {
                            latestNPC = SpawnSegment(source, BodyType, latestNPC);
                            distance--;
                        }
                    }

                    // ����β��Ƭ��
                    SpawnSegment(source, TailType, latestNPC);

                    NPC.netUpdate = true;

                    // ȷ������ϸ�ڶ��������ɡ� ������ܣ�����ȫɾ�����
                    int count = 0;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC n = Main.npc[i];

                        if (n.active && (n.type == Type || n.type == BodyType || n.type == TailType) && n.realLife == NPC.whoAmI)
                            count++;
                    }

                    if (count != randomWormLength)
                    {
                        // �޷��������е�Ƭ��...ɱ������
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            NPC n = Main.npc[i];

                            if (n.active && (n.type == Type || n.type == BodyType || n.type == TailType) && n.realLife == NPC.whoAmI)
                            {
                                n.active = false;
                                n.netUpdate = true;
                            }
                        }
                    }

                    // �������Ŀ����ȷ��׼ȷ��
                    NPC.TargetClosest(true);
                }
            }
        }

        private bool HeadAI_CheckCollisionForDustSpawns()
        {
            int minTilePosX = (int)(NPC.Left.X / 16) - 1;
            int maxTilePosX = (int)(NPC.Right.X / 16) + 2;
            int minTilePosY = (int)(NPC.Top.Y / 16) - 1;
            int maxTilePosY = (int)(NPC.Bottom.Y / 16) + 2;

            // ȷ����ש��Χ������߽���
            if (minTilePosX < 0)
                minTilePosX = 0;
            if (maxTilePosX > Main.maxTilesX)
                maxTilePosX = Main.maxTilesX;
            if (minTilePosY < 0)
                minTilePosY = 0;
            if (maxTilePosY > Main.maxTilesY)
                maxTilePosY = Main.maxTilesY;

            bool collision = false;

            // ���Ǽ�����ש��ײ�ĳ�ʼ��顣
            for (int i = minTilePosX; i < maxTilePosX; ++i)
            {
                for (int j = minTilePosY; j < maxTilePosY; ++j)
                {
                    Tile tile = Main.tile[i, j];

                    // ����ô�ש��ʵ�Ļ���Ϊƽ̨���������Ч��ײ��
                    if (tile.HasUnactuatedTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType] && tile.TileFrameY == 0) || tile.LiquidAmount > 64)
                    {
                        Vector2 tileWorld = new Point16(i, j).ToWorldCoordinates(0, 0);

                        if (NPC.Right.X > tileWorld.X && NPC.Left.X < tileWorld.X + 16 && NPC.Bottom.Y > tileWorld.Y && NPC.Top.Y < tileWorld.Y + 16)
                        {
                            // Collision found
                            // ������ײ
                            collision = true;

                            if (Main.rand.NextBool(100))
                                WorldGen.KillTile(i, j, fail: true, effectOnly: true, noItem: false);
                        }
                    }
                }
            }

            return collision;
        }

        private void HeadAI_CheckTargetDistance(ref bool collision)
        {
            // If there is no collision with tiles, we check if the distance between this NPC and its target is too large, so that we can still trigger "collision".
            // ���û����ؿ鷢����ײ�����ǽ�����NPC����Ŀ��֮��ľ����Ƿ�����Ա���Ȼ���Դ�������ײ����
            if (!collision)
            {
                Rectangle hitbox = NPC.Hitbox;

                int maxDistance = MaxDistanceForUsingTileCollision;

                bool tooFar = true;

                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Rectangle areaCheck;

                    Player player = Main.player[i];

                    if (ForcedTargetPosition is Vector2 target)
                        areaCheck = new Rectangle((int)target.X - maxDistance, (int)target.Y - maxDistance, maxDistance * 2, maxDistance * 2);
                    else if (player.active && !player.dead && !player.ghost)
                        areaCheck = new Rectangle((int)player.position.X - maxDistance, (int)player.position.Y - maxDistance, maxDistance * 2, maxDistance * 2);
                    else
                        continue;  // Not a valid player
                                   // ������Ч���

                    if (hitbox.Intersects(areaCheck))
                    {
                        tooFar = false;
                        break;
                    }
                }

                if (tooFar)
                    collision = true;
            }
        }

        private void HeadAI_Movement(bool collision)
        {
            // MoveSpeedȷ����NPC�����ƶ�������ٶȡ�
            // ���ߵ�ֵ=������ٶȡ�
            float speed = MoveSpeed;

            // ���ٶȾ�����������һ�������NPC���ٵ��ٶȡ�
            float acceleration = Acceleration;

            float targetXPos, targetYPos;

            Player playerTarget = Main.player[NPC.target];

            Vector2 forcedTarget = ForcedTargetPosition ?? playerTarget.Center;

            // ʹ��ValueTuple�������������ɷ�����ֵ
            (targetXPos, targetYPos) = (forcedTarget.X, forcedTarget.Y);

            // ���Ƹ�ֵ����Ϊ�Ժ�ᱻ���ǵ�
            Vector2 npcCenter = NPC.Center;

            float targetRoundedPosX = (int)(targetXPos / 16f) * 16;
            float targetRoundedPosY = (int)(targetYPos / 16f) * 16;
            npcCenter.X = (int)(npcCenter.X / 16f) * 16;
            npcCenter.Y = (int)(npcCenter.Y / 16f) * 16;
            float dirX = targetRoundedPosX - npcCenter.X;
            float dirY = targetRoundedPosY - npcCenter.Y;

            float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);

            // �������û���κ����͵ĳ�ͻ������ϣ��NPC���µ��䲢��X����١�
            if (!collision && !CanFly)
                HeadAI_Movement_HandleFallingFromNoCollision(dirX, speed, acceleration);
            else
            {
                // ���������벥��һЩ��Ƶ��soundDelay��������Ŀ���ƶ���
                HeadAI_Movement_PlayDigSounds(length);

                HeadAI_Movement_HandleMovement(dirX, dirY, length, speed, acceleration);
            }

            HeadAI_Movement_SetRotation(collision);
        }

        private void HeadAI_Movement_HandleFallingFromNoCollision(float dirX, float speed, float acceleration)
        {
            // ����Ѱ����Ŀ��
            NPC.TargetClosest(true);

            // �����㶨Ϊ0.11����/�̶�
            NPC.velocity.Y += 0.11f;

            // ȷ��NPC�����½���̫��
            if (NPC.velocity.Y > speed)
                NPC.velocity.Y = speed;

            // ������Ϊģ���������˶�
            if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 0.4f)
            {
                // �ٶ��㹻�죬������̫��
                if (NPC.velocity.X < 0.0f)
                    NPC.velocity.X -= acceleration * 1.1f;
                else
                    NPC.velocity.X += acceleration * 1.1f;
            }
            else if (NPC.velocity.Y == speed)
            {
                // NPC�Ѵﵽ�ն��ٶ�
                if (NPC.velocity.X < dirX)
                    NPC.velocity.X += acceleration;
                else if (NPC.velocity.X > dirX)
                    NPC.velocity.X -= acceleration;
            }
            else if (NPC.velocity.Y > 4)
            {
                if (NPC.velocity.X < 0)
                    NPC.velocity.X += acceleration * 0.9f;
                else
                    NPC.velocity.X -= acceleration * 0.9f;
            }
        }

        private void HeadAI_Movement_PlayDigSounds(float length)
        {
            if (NPC.soundDelay == 0)
            {
                // ����Ŀ��λ��ʱ�����������ٶȸ���
                float num1 = length / 40f;

                if (num1 < 10)
                    num1 = 10f;

                if (num1 > 20)
                    num1 = 20f;

                NPC.soundDelay = (int)num1;

                SoundEngine.PlaySound(SoundID.WormDig, NPC.position);
            }
        }

        private void HeadAI_Movement_HandleMovement(float dirX, float dirY, float length, float speed, float acceleration)
        {
            float absDirX = Math.Abs(dirX);
            float absDirY = Math.Abs(dirY);
            float newSpeed = speed / length;
            dirX *= newSpeed;
            dirY *= newSpeed;

            if (NPC.velocity.X > 0 && dirX > 0 || NPC.velocity.X < 0 && dirX < 0 || NPC.velocity.Y > 0 && dirY > 0 || NPC.velocity.Y < 0 && dirY < 0)
            {
                // NPC ������Ŀ��λ���ƶ�
                if (NPC.velocity.X < dirX)
                    NPC.velocity.X += acceleration;
                else if (NPC.velocity.X > dirX)
                    NPC.velocity.X -= acceleration;

                if (NPC.velocity.Y < dirY)
                    NPC.velocity.Y += acceleration;
                else if (NPC.velocity.Y > dirY)
                    NPC.velocity.Y -= acceleration;

                // ��ͼ�� Y-�ٶȺ�С������ NPC �����ƶ���Ŀ�������Ҳ��֮��Ȼ
                if (Math.Abs(dirY) < speed * 0.2 && (NPC.velocity.X > 0 && dirX < 0 || NPC.velocity.X < 0 && dirX > 0))
                {
                    if (NPC.velocity.Y > 0)
                        NPC.velocity.Y += acceleration * 2f;
                    else
                        NPC.velocity.Y -= acceleration * 2f;
                }

                // ��ͼ�� X-�ٶȺ�С������ NPC �����ƶ���Ŀ�������·����Ϸ�
                if (Math.Abs(dirX) < speed * 0.2 && (NPC.velocity.Y > 0 && dirY < 0 || NPC.velocity.Y < 0 && dirY > 0))
                {
                    if (NPC.velocity.X > 0)
                        NPC.velocity.X = NPC.velocity.X + acceleration * 2f;
                    else
                        NPC.velocity.X = NPC.velocity.X - acceleration * 2f;
                }
            }
            else if (absDirX > absDirY)
            {
                // X ������� Y ���롣ǿ������ X ���˶���ǿ����
                if (NPC.velocity.X < dirX)
                    NPC.velocity.X += acceleration * 1.1f;
                else if (NPC.velocity.X > dirX)
                    NPC.velocity.X -= acceleration * 1.1f;

                if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 0.5)
                {
                    if (NPC.velocity.Y > 0)
                        NPC.velocity.Y += acceleration;
                    else
                        NPC.velocity.Y -= acceleration;
                }
            }
            else
            {
                // Ϊ�� NPC ������ȷ����ת��ǿ������ X ���˶���ǿ����
                if (NPC.velocity.Y < dirY)
                    NPC.velocity.Y += acceleration * 1.1f;
                else if (NPC.velocity.Y > dirY)
                    NPC.velocity.Y -= acceleration * 1.1f;

                if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 0.5)
                {
                    if (NPC.velocity.X > 0)
                        NPC.velocity.X += acceleration;
                    else
                        NPC.velocity.X -= acceleration;
                }
            }
        }

        private void HeadAI_Movement_SetRotation(bool collision)
        {
            // // Ϊ�� NPC ������ȷ����ת��
            // ����� NPC �ľ���ͼ���ϡ���������Ҫ�޸Ĵ�������ȷ�������� NPC ����
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;

            if (collision)
            {
                if (NPC.localAI[0] != 1)
                    NPC.netUpdate = true;

                NPC.localAI[0] = 1f;
            }
            else
            {
                if (NPC.localAI[0] != 0)
                    NPC.netUpdate = true;

                NPC.localAI[0] = 0f;
            }

            // ��� NPC ���ٶȸı���Ų�����û�б���ҡ��ջ��С�����ǿ�ƽ��о����¡�
            if ((NPC.velocity.X > 0 && NPC.oldVelocity.X < 0 || NPC.velocity.X < 0 && NPC.oldVelocity.X > 0 || NPC.velocity.Y > 0 && NPC.oldVelocity.Y < 0 || NPC.velocity.Y < 0 && NPC.oldVelocity.Y > 0) && !NPC.justHit)
                NPC.netUpdate = true;
        }
    }

    public abstract class WormBody : Worm
    {
        public sealed override WormSegmentType SegmentType => WormSegmentType.Body;

        internal override void BodyTailAI()
        {
            CommonAI_BodyTail(this);
        }

        internal static void CommonAI_BodyTail(Worm worm)
        {
            if (!worm.NPC.HasValidTarget)
                worm.NPC.TargetClosest(true);

            if (Main.player[worm.NPC.target].dead && worm.NPC.timeLeft > 30000)
                worm.NPC.timeLeft = 10;

            NPC following = worm.NPC.ai[1] >= Main.maxNPCs ? null : worm.FollowingNPC;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                // �������/β�����ǵ������ɵģ�����ܳ�������һЩ����
                // ����������Ƭ��NPC������Ч����ɱ����Ƭ�Ρ�
                if (following is null || !following.active || following.friendly || following.townNPC || following.lifeMax <= 5)
                {
                    worm.NPC.life = 0;
                    worm.NPC.HitEffect(0, 10);
                    worm.NPC.active = false;
                }
            }

            if (following is not null)
            {
                // �����NPCǰ���������Ƭ��
                // ʹ�õ�ǰNPC.Center����ָ���NPC������NPC���ķ���
                float dirX = following.Center.X - worm.NPC.Center.X;
                float dirY = following.Center.Y - worm.NPC.Center.Y;

                // Ȼ������ʹ�� Atan2 �������ȷ�����Ǹ�����NPC����ת��
                // ����� NPC �ľ���ͼ���ϡ���������Ҫ�޸Ĵ�������ȷ�������� NPC ����
                worm.NPC.rotation = (float)Math.Atan2(dirY, dirX) + MathHelper.PiOver2;

                // ���ǻ���ȡ�˷���ʸ���ĳ��ȡ�
                float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);

                // ���Ǽ���һ���µġ���ȷ�ľ��롣
                float dist = (length - worm.NPC.width) / length;
                float posX = dirX * dist;
                float posY = dirY * dist;

                // ���ô� NPC ���ٶȣ���Ϊ���ǲ�ϣ�����Լ��ƶ�
                worm.NPC.velocity = Vector2.Zero;

                // ������ NPC ��λ������Ϊ�丸�� NPC ��λ�á�
                worm.NPC.position.X += posX;
                worm.NPC.position.Y += posY;
            }
        }
    }

    // ��Ϊ�����β���ι�����ͬ�� AI
    public abstract class WormTail : Worm
    {
        public sealed override WormSegmentType SegmentType => WormSegmentType.Tail;

        internal override void BodyTailAI()
        {
            WormBody.CommonAI_BodyTail(this);
        }
    }
}
