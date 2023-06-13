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
        /// The head segment for the worm.  Only one "head" is considered to be active for any given worm<br/>
        /// ���ӵ�ͷ���ֽڡ������κθ����ĳ��ӣ�ֻ��һ����ͷ������Ϊ�ǻ�ġ�
        /// </summary>
        Head,
        /// <summary>
        /// The body segment.  Follows the segment in front of it<br/>
        /// ����ֽڡ�����ǰ��ķֽڡ�
        /// </summary>
        Body,
        /// <summary>
        /// The tail segment.  Has the same AI as the body segments.  Only one "tail" is considered to be active for any given worm<br/>
        /// β���ֽڡ��������������ͬ��AI�������κθ����ĳ��ӣ�ֻ����һ����β�͡���
        /// </summary>
        Tail
    }

    /// <summary>
    /// The base class for non-separating Worm enemies.<br/>
    /// �Ƿ��������˵Ļ��ࡣ
    /// </summary>
    public abstract class Worm : ModNPC
    {
        /*  ai[] usage:
         *  ai[]�÷���
		 *  
		 *  ai[0] = "follower" segment, the segment that's following this segment
		 *  ai[1] = "following" segment, the segment that this segment is following
		 *  
		 *  ai[0] = ��follower���Σ�������˶���Ķ���
         *  ai[1] = ���ڸ����Ƭ�εġ�following��Ƭ��
		 *  
		 *  localAI[0] = used when syncing changes to collision detection
		 *  localAI[1] = checking if Init() was called
		 *  
		 *  localAI [0] = ��ͬ�����ĵ���ײ���ʱʹ��
		 *  localAI [1] = ����Ƿ������Init����
		 */

        /// <summary>
        /// Which type of segment this NPC is considered to be<br/>
        /// ���NPC����Ϊ���������͵�Ƭ��
        /// </summary>
        public abstract WormSegmentType SegmentType { get; }

        /// <summary>
        /// The maximum velocity for the NPC<br/>
        /// NPC����ٶ�
        /// </summary>
        public float MoveSpeed { get; set; }

        /// <summary>
        /// The rate at which the NPC gains velocity<br/>
        /// NPC�����ٶȵ�����
        /// </summary>
        public float Acceleration { get; set; }

        /// <summary>
        /// The NPC instance of the head segment for this worm.<br/>
        /// ������ NPC ��ͷ���ֽ�ʵ����
        /// </summary>
        public NPC HeadSegment => Main.npc[NPC.realLife];

        /// <summary>
        /// The NPC instance of the segment that this segment is following (ai[1]).  For head segments, this property always returns <see langword="null"/>.<br/>
        /// ��Ƭ�����ڸ����Ƭ�Σ�ai[1]�� �� NPC ʵ���� ����ͷ���ֽڣ�������ʼ�շ��� <see langword="null"/>��
        /// </summary>
        public NPC FollowingNPC => SegmentType == WormSegmentType.Head ? null : Main.npc[(int)NPC.ai[1]];

        /// <summary>
        /// The NPC instance of the segment that is following this segment (ai[0]).  For tail segment, this property always returns <see langword="null"/>.<br/>
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

                    // If the NPC is a boss and it has no target, force it to fall to the underworld quickly
                    // ��� NPC �� boss ����û��Ŀ�꣬��Ѹ�ٵ����������
                    if (!NPC.HasValidTarget && NPC.boss)
                    {
                        NPC.velocity.Y += 8f;

                        MoveSpeed = 1000f;

                        if (!startDespawning)
                        {
                            startDespawning = true;

                            // Despawn after 90 ticks (1.5 seconds) if the NPC gets far enough away
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

        // Not visible to public API, but is used to indicate what AI to run
        // �Թ��� API ���ɼ���������ָʾҪ���е� AI
        internal virtual void HeadAI() { }

        internal virtual void BodyTailAI() { }

        public abstract void Init();
    }

    /// <summary>
    /// The base class for head segment NPCs of Worm enemies<br/>
    /// ������ͷ���ֽ� NPC �Ļ��ࡣ
    /// </summary>
    public abstract class WormHead : Worm
    {
        public sealed override WormSegmentType SegmentType => WormSegmentType.Head;

        /// <summary>
        /// The NPCID or ModContent.NPCType for the body segment NPCs.<br/>
        /// �����NPC��NPCID��ModContent.NPCType��<br/>
        /// This property is only used if <see cref="HasCustomBodySegments"/> returns <see langword="false"/>.
        /// ����<see cref="HasCustomBodySegments"/>����<see langword="false"/>ʱʹ�ô����ԡ�
        /// </summary>
        public abstract int BodyType { get; }

        /// <summary>
        /// The NPCID or ModContent.NPCType for the tail segment NPC.<br/>
        /// β����NPC��NPCID��ModContent.NPCType��<br/>
        /// This property is only used if <see cref="HasCustomBodySegments"/> returns <see langword="false"/>.
        /// ����<see cref="HasCustomBodySegments"/>����<see langword="false"/>ʱʹ�ô����ԡ�
        /// </summary>
        public abstract int TailType { get; }

        /// <summary>
        /// The minimum amount of segments expected, including the head and tail segments
        /// ��������С�ֶ���������ͷ��β���ֶ�
        /// </summary>
        public int MinSegmentLength { get; set; }

        /// <summary>
        /// The maximum amount of segments expected, including the head and tail segments
        /// ���������ֶ���������ͷ��β���ֶ�
        /// </summary>
        public int MaxSegmentLength { get; set; }

        /// <summary>
        /// Whether the NPC ignores tile collision when attempting to "dig" through tiles, like how Wyverns work.
        /// �Ƿ���Դ�ש��ײ�ԡ��ھ򡱴�����ש������ѩ��һ��������
        /// </summary>
        public bool CanFly { get; set; }

        /// <summary>
        /// The maximum distance in <b>pixels</b> within which the NPC will use tile collision, if <see cref="CanFly"/> returns <see langword="false"/>.<br/>
        /// �ھ����NPC�ڣ���<b>����</b>Ϊ��λ�������<see cref="CanFly"/>����<see langword="false"/>��������ƽ����ײ��⡣<br/>
        /// Defaults to 1000 pixels, which is equivalent to 62.5 tiles.
        /// Ĭ��ֵΪ1000�����أ��൱��62.5��ƽ�̡�
        /// </summary>
        public virtual int MaxDistanceForUsingTileCollision => 1000;

        /// <summary>
        /// Whether the NPC uses 
        /// �Ƿ�ʹ�ø�NPC
        /// </summary>
        public virtual bool HasCustomBodySegments => false;

        /// <summary>
        /// If not <see langword="null"/>, this NPC will target the given world position instead of its player target<br/>
        /// ������� <see langword="null"/> ����� NPC ����Ը�������λ�ö��������Ŀ�����Ŀ������
        /// </summary>
        public Vector2? ForcedTargetPosition { get; set; }

        /// <summary>
        /// Override this method to use custom body-spawning code.<br/>
        /// ���Ǵ˷�����ʹ���Զ������ɴ��롣 <br/>
        /// This method only runs if <see cref="HasCustomBodySegments"/> returns <see langword="true"/>.<br/>
        /// ֻ���� <see cref="HasCustomBodySegments"/> ���� <see langword="true"/> ʱ�����д˷�����
        /// </summary>
        /// <param name="segmentCount">How many body segements are expected to be spawned</param>
        /// <param name="segmentCount">Ԥ��Ҫ���ɶ��ٸ�����Ƭ��</param>
        /// <returns>������ɵ� NPC �� whoAmI ֵ�����ǵ��� <see cref="NPC.NewNPC(IEntitySource, int, int, int, int, float, float, float, float, int)"/> �Ľ��</returns>
        public virtual int SpawnBodySegments(int segmentCount)
        {
            // Defaults to just returning this NPC's whoAmI, since the tail segment uses the return value as its "following" NPC index
            // Ĭ��ֻ���ش� NPC �� whoAmI����Ϊβ����ʹ�÷���ֵ��Ϊ�䡰���桱NPC����
            return NPC.whoAmI;
        }

        /// <summary>
        /// Spawns a body or tail segment of the worm.<br/>
        /// ���ɳ���������β���ֶΡ�
        /// </summary>
        /// <param name="source">����Դ</param><br/>
        /// <param name="type">Ҫ���ɵ�Ƭ�� NPC �� ID</param><br/>
        /// <param name="latestNPC">������������ɵ�Ƭ�� NPC������ͷ���� whoAmI ֵ</param><br/>
        /// <returns></returns>
        protected int SpawnSegment(IEntitySource source, int type, int latestNPC)
        {
            // We spawn a new NPC, setting latestNPC to the newer NPC, whilst also using that same variable
            // to set the parent of this new NPC. The parent of the new NPC (may it be a tail or body part)
            // will determine the movement of this new NPC.
            // Under there, we also set the realLife value of the new NPC, because of what is explained above.

            // ���ǲ���һ���µ� NPC���� latestNPC ����Ϊ���µ� NPC��ͬʱҲʹ��ͬһ����
            // ����������� NPC �ĸ����� �� NPC �ĸ�����������β�ͻ����岿�֣�
            // ����������� NPC ���ƶ���
            // ���������棬���ǻ��������³��ֵ�NPCʵ������ֵ����Ϊ������͹��ˡ�
            int oldLatest = latestNPC;
            latestNPC = NPC.NewNPC(source, (int)NPC.Center.X, (int)NPC.Center.Y, type, NPC.whoAmI, 0, latestNPC);

            Main.npc[oldLatest].ai[0] = latestNPC;

            NPC latest = Main.npc[latestNPC];
            // NPC.realLife is the whoAmI of the NPC that the spawned NPC will share its health with
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
                // So, we start the AI off by checking if NPC.ai[0] (the following NPC's whoAmI) is 0.
                // This is practically ALWAYS the case with a freshly spawned NPC, so this means this is the first update.
                // Since this is the first update, we can safely assume we need to spawn the rest of the worm (bodies + tail).

                // �������ǴӼ�鿪ʼAI���npc.ai [0] ������npcs˭ami����0��
                // �⼸�����Ǹող���һ�� npc ����������ζ�����ǵ�һ�θ��¡�
                // ��Ȼ���ǵ�һ�θ��£����ǿ��԰�ȫ�ؼ�����Ҫ�������ʣ�ಿ�֣�����+β�ͣ���

                bool hasFollower = NPC.ai[0] > 0;
                if (!hasFollower)
                {
                    // So, here we assign the NPC.realLife value.
                    // The NPC.realLife value is mainly used to determine which NPC loses life when we hit this NPC.
                    // We don't want every single piece of the worm to have its own HP pool, so this is a neat way to fix that.

                    // ���ԣ��������Ƿ�����NPC.realLifeֵ��
                    // npc.realLife ֵ��Ҫ����ȷ���ĸ� NPC �����ǻ��д� NPC ʱʧȥ������
                    // ���ǲ�ϣ������ÿһ���ֶ����Լ��� HP �أ���������һ���ܺõĽ��������

                    NPC.realLife = NPC.whoAmI;
                    // latestNPC is going to be used in SpawnSegment() and I'll explain it there.
                    // latestNPC ���� SpawnSegment() ��ʹ�ã��ҽ��������������
                    int latestNPC = NPC.whoAmI;

                    // Here we determine the length of the worm.
                    // ��������ȷ�����ĳ��ȡ�
                    int randomWormLength = Main.rand.Next(MinSegmentLength, MaxSegmentLength + 1);

                    int distance = randomWormLength - 2;

                    IEntitySource source = NPC.GetSource_FromAI();

                    if (HasCustomBodySegments)
                    {
                        // Call the method that'll handle spawning the body segments
                        // ���ô�����������εķ���
                        latestNPC = SpawnBodySegments(distance);
                    }
                    else
                    {
                        // Spawn the body segments like usual
                        // ������һ����������Ƭ��
                        while (distance > 0)
                        {
                            latestNPC = SpawnSegment(source, BodyType, latestNPC);
                            distance--;
                        }
                    }

                    // Spawn the tail segment
                    // ����β��Ƭ��
                    SpawnSegment(source, TailType, latestNPC);

                    NPC.netUpdate = true;

                    // Ensure that all of the segments could spawn.  If they could not, despawn the worm entirely
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
                        // Unable to spawn all of the segments... kill the worm
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

                    // Set the player target for good measure
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

            // Ensure that the tile range is within the world bounds
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

            // This is the initial check for collision with tiles.
            // ���Ǽ�����ש��ײ�ĳ�ʼ��顣
            for (int i = minTilePosX; i < maxTilePosX; ++i)
            {
                for (int j = minTilePosY; j < maxTilePosY; ++j)
                {
                    Tile tile = Main.tile[i, j];

                    // If the tile is solid or is considered a platform, then there's valid collision
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
            // MoveSpeed determines the max speed at which this NPC can move.
            // Higher value = faster speed.

            // MoveSpeedȷ����NPC�����ƶ�������ٶȡ�
            // ���ߵ�ֵ=������ٶȡ�
            float speed = MoveSpeed;
            // acceleration is exactly what it sounds like. The speed at which this NPC accelerates.
            // ���ٶȾ�����������һ�������NPC���ٵ��ٶȡ�
            float acceleration = Acceleration;

            float targetXPos, targetYPos;

            Player playerTarget = Main.player[NPC.target];

            Vector2 forcedTarget = ForcedTargetPosition ?? playerTarget.Center;
            // Using a ValueTuple like this allows for easy assignment of multiple values
            // ʹ��ValueTuple�������������ɷ�����ֵ
            (targetXPos, targetYPos) = (forcedTarget.X, forcedTarget.Y);

            // Copy the value, since it will be clobbered later
            // ���Ƹ�ֵ����Ϊ�Ժ�ᱻ���ǵ�
            Vector2 npcCenter = NPC.Center;

            float targetRoundedPosX = (int)(targetXPos / 16f) * 16;
            float targetRoundedPosY = (int)(targetYPos / 16f) * 16;
            npcCenter.X = (int)(npcCenter.X / 16f) * 16;
            npcCenter.Y = (int)(npcCenter.Y / 16f) * 16;
            float dirX = targetRoundedPosX - npcCenter.X;
            float dirY = targetRoundedPosY - npcCenter.Y;

            float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);

            // If we do not have any type of collision, we want the NPC to fall down and de-accelerate along the X axis.
            // �������û���κ����͵ĳ�ͻ������ϣ��NPC���µ��䲢��X����١�
            if (!collision && !CanFly)
                HeadAI_Movement_HandleFallingFromNoCollision(dirX, speed, acceleration);
            else
            {
                // Else we want to play some audio (soundDelay) and move towards our target.
                // ���������벥��һЩ��Ƶ��soundDelay��������Ŀ���ƶ���
                HeadAI_Movement_PlayDigSounds(length);

                HeadAI_Movement_HandleMovement(dirX, dirY, length, speed, acceleration);
            }

            HeadAI_Movement_SetRotation(collision);
        }

        private void HeadAI_Movement_HandleFallingFromNoCollision(float dirX, float speed, float acceleration)
        {
            // Keep searching for a new target
            // ����Ѱ����Ŀ��
            NPC.TargetClosest(true);

            // Constant gravity of 0.11 pixels/tick
            // �����㶨Ϊ0.11����/�̶�
            NPC.velocity.Y += 0.11f;

            // Ensure that the NPC does not fall too quickly
            // ȷ��NPC�����½���̫��
            if (NPC.velocity.Y > speed)
                NPC.velocity.Y = speed;

            // The following behaviour mimicks vanilla worm movement
            // ������Ϊģ���������˶�
            if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 0.4f)
            {
                // Velocity is sufficiently fast, but not too fast
                // �ٶ��㹻�죬������̫��
                if (NPC.velocity.X < 0.0f)
                    NPC.velocity.X -= acceleration * 1.1f;
                else
                    NPC.velocity.X += acceleration * 1.1f;
            }
            else if (NPC.velocity.Y == speed)
            {
                // NPC has reached terminal velocity
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
                // Play sounds quicker the closer the NPC is to the target location
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
                // The NPC is moving towards the target location
                // NPC ������Ŀ��λ���ƶ�
                if (NPC.velocity.X < dirX)
                    NPC.velocity.X += acceleration;
                else if (NPC.velocity.X > dirX)
                    NPC.velocity.X -= acceleration;

                if (NPC.velocity.Y < dirY)
                    NPC.velocity.Y += acceleration;
                else if (NPC.velocity.Y > dirY)
                    NPC.velocity.Y -= acceleration;

                // The intended Y-velocity is small AND the NPC is moving to the left and the target is to the right of the NPC or vice versa
                // ��ͼ�� Y-�ٶȺ�С������ NPC �����ƶ���Ŀ�������Ҳ��֮��Ȼ
                if (Math.Abs(dirY) < speed * 0.2 && (NPC.velocity.X > 0 && dirX < 0 || NPC.velocity.X < 0 && dirX > 0))
                {
                    if (NPC.velocity.Y > 0)
                        NPC.velocity.Y += acceleration * 2f;
                    else
                        NPC.velocity.Y -= acceleration * 2f;
                }

                // The intended X-velocity is small AND the NPC is moving up/down and the target is below/above the NPC
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
                // The X distance is larger than the Y distance.  Force movement along the X-axis to be stronger
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
                // The X distance is larger than the Y distance.Force movement along the X-axis to be stronger
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
            // Set the correct rotation for this NPC.
            // Assumes the sprite for the NPC points upward.  You might have to modify this line to properly account for your NPC's orientation
            // // Ϊ�� NPC ������ȷ����ת��
            // ����� NPC �ľ��鳯�ϡ���������Ҫ�޸Ĵ�������ȷ�������� NPC ����
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;

            // Some netupdate stuff (multiplayer compatibility).
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

            // Force a netupdate if the NPC's velocity changed sign and it was not "just hit" by a player
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
                // Some of these conditions are possble if the body/tail segment was spawned individually
                // Kill the segment if the segment NPC it's following is no longer valid

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
                // Follow behind the segment "in front" of this NPC
                // Use the current NPC.Center to calculate the direction towards the "parent NPC" of this NPC.

                // �����NPCǰ���������Ƭ��
                // ʹ�õ�ǰNPC.Center����ָ���NPC������NPC���ķ���
                float dirX = following.Center.X - worm.NPC.Center.X;
                float dirY = following.Center.Y - worm.NPC.Center.Y;
                // We then use Atan2 to get a correct rotation towards that parent NPC.
                // Assumes the sprite for the NPC points upward.  You might have to modify this line to properly account for your NPC's orientation

                // Ȼ������ʹ�� Atan2 �������ȷ�����Ǹ�����NPC����ת��
                // ����� NPC �ľ��鳯�ϡ���������Ҫ�޸Ĵ�������ȷ�������� NPC ����
                worm.NPC.rotation = (float)Math.Atan2(dirY, dirX) + MathHelper.PiOver2;
                // We also get the length of the direction vector.
                // ���ǻ���ȡ�˷���ʸ���ĳ��ȡ�
                float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);
                // We calculate a new, correct distance.
                // ���Ǽ���һ���µġ���ȷ�ľ��롣
                float dist = (length - worm.NPC.width) / length;
                float posX = dirX * dist;
                float posY = dirY * dist;

                // Reset the velocity of this NPC, because we don't want it to move on its own
                // ���ô� NPC ���ٶȣ���Ϊ���ǲ�ϣ�����Լ��ƶ�
                worm.NPC.velocity = Vector2.Zero;
                // And set this NPCs position accordingly to that of this NPCs parent NPC.
                // ������ NPC ��λ������Ϊ�丸�� NPC ��λ�á�
                worm.NPC.position.X += posX;
                worm.NPC.position.Y += posY;
            }
        }
    }

    // Since the body and tail segments share the same AI
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
