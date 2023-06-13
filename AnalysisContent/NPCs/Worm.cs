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
        /// 虫子的头部分节。对于任何给定的虫子，只有一个“头”被认为是活动的。
        /// </summary>
        Head,
        /// <summary>
        /// The body segment.  Follows the segment in front of it<br/>
        /// 身体分节。跟随前面的分节。
        /// </summary>
        Body,
        /// <summary>
        /// The tail segment.  Has the same AI as the body segments.  Only one "tail" is considered to be active for any given worm<br/>
        /// 尾部分节。具有与身体段相同的AI。对于任何给定的虫子，只考虑一个“尾巴”。
        /// </summary>
        Tail
    }

    /// <summary>
    /// The base class for non-separating Worm enemies.<br/>
    /// 非分离蠕虫敌人的基类。
    /// </summary>
    public abstract class Worm : ModNPC
    {
        /*  ai[] usage:
         *  ai[]用法：
		 *  
		 *  ai[0] = "follower" segment, the segment that's following this segment
		 *  ai[1] = "following" segment, the segment that this segment is following
		 *  
		 *  ai[0] = “follower”段，即跟随此段落的段落
         *  ai[1] = 正在跟随此片段的“following”片段
		 *  
		 *  localAI[0] = used when syncing changes to collision detection
		 *  localAI[1] = checking if Init() was called
		 *  
		 *  localAI [0] = 在同步更改到碰撞检测时使用
		 *  localAI [1] = 检查是否调用了Init（）
		 */

        /// <summary>
        /// Which type of segment this NPC is considered to be<br/>
        /// 这个NPC被认为是哪种类型的片段
        /// </summary>
        public abstract WormSegmentType SegmentType { get; }

        /// <summary>
        /// The maximum velocity for the NPC<br/>
        /// NPC最大速度
        /// </summary>
        public float MoveSpeed { get; set; }

        /// <summary>
        /// The rate at which the NPC gains velocity<br/>
        /// NPC增加速度的速率
        /// </summary>
        public float Acceleration { get; set; }

        /// <summary>
        /// The NPC instance of the head segment for this worm.<br/>
        /// 这个蠕虫 NPC 的头部分节实例。
        /// </summary>
        public NPC HeadSegment => Main.npc[NPC.realLife];

        /// <summary>
        /// The NPC instance of the segment that this segment is following (ai[1]).  For head segments, this property always returns <see langword="null"/>.<br/>
        /// 该片段正在跟随此片段（ai[1]） 的 NPC 实例。 对于头部分节，此属性始终返回 <see langword="null"/>。
        /// </summary>
        public NPC FollowingNPC => SegmentType == WormSegmentType.Head ? null : Main.npc[(int)NPC.ai[1]];

        /// <summary>
        /// The NPC instance of the segment that is following this segment (ai[0]).  For tail segment, this property always returns <see langword="null"/>.<br/>
        /// 正在跟随该片断（ai[0]) 的 NPC 实例 。 对于尾部分节，此属性始终返回 <see langword="null"/>。
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
                    // 如果 NPC 是 boss 并且没有目标，则迅速掉入地下世界
                    if (!NPC.HasValidTarget && NPC.boss)
                    {
                        NPC.velocity.Y += 8f;

                        MoveSpeed = 1000f;

                        if (!startDespawning)
                        {
                            startDespawning = true;

                            // Despawn after 90 ticks (1.5 seconds) if the NPC gets far enough away
                            // 如果 NPC 距离足够远，则在 90 ticks (1.5 秒) 后消失
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
        // 对公共 API 不可见，但用于指示要运行的 AI
        internal virtual void HeadAI() { }

        internal virtual void BodyTailAI() { }

        public abstract void Init();
    }

    /// <summary>
    /// The base class for head segment NPCs of Worm enemies<br/>
    /// 蠕虫敌人头部分节 NPC 的基类。
    /// </summary>
    public abstract class WormHead : Worm
    {
        public sealed override WormSegmentType SegmentType => WormSegmentType.Head;

        /// <summary>
        /// The NPCID or ModContent.NPCType for the body segment NPCs.<br/>
        /// 身体段NPC的NPCID或ModContent.NPCType。<br/>
        /// This property is only used if <see cref="HasCustomBodySegments"/> returns <see langword="false"/>.
        /// 仅当<see cref="HasCustomBodySegments"/>返回<see langword="false"/>时使用此属性。
        /// </summary>
        public abstract int BodyType { get; }

        /// <summary>
        /// The NPCID or ModContent.NPCType for the tail segment NPC.<br/>
        /// 尾部段NPC的NPCID或ModContent.NPCType。<br/>
        /// This property is only used if <see cref="HasCustomBodySegments"/> returns <see langword="false"/>.
        /// 仅当<see cref="HasCustomBodySegments"/>返回<see langword="false"/>时使用此属性。
        /// </summary>
        public abstract int TailType { get; }

        /// <summary>
        /// The minimum amount of segments expected, including the head and tail segments
        /// 期望的最小分段数，包括头和尾部分段
        /// </summary>
        public int MinSegmentLength { get; set; }

        /// <summary>
        /// The maximum amount of segments expected, including the head and tail segments
        /// 期望的最大分段数，包括头和尾部分段
        /// </summary>
        public int MaxSegmentLength { get; set; }

        /// <summary>
        /// Whether the NPC ignores tile collision when attempting to "dig" through tiles, like how Wyverns work.
        /// 是否忽略瓷砖碰撞以“挖掘”穿过瓷砖，就像雪怪一样工作。
        /// </summary>
        public bool CanFly { get; set; }

        /// <summary>
        /// The maximum distance in <b>pixels</b> within which the NPC will use tile collision, if <see cref="CanFly"/> returns <see langword="false"/>.<br/>
        /// 在距离该NPC内（以<b>像素</b>为单位），如果<see cref="CanFly"/>返回<see langword="false"/>则将其用于平铺碰撞检测。<br/>
        /// Defaults to 1000 pixels, which is equivalent to 62.5 tiles.
        /// 默认值为1000个像素，相当于62.5个平铺。
        /// </summary>
        public virtual int MaxDistanceForUsingTileCollision => 1000;

        /// <summary>
        /// Whether the NPC uses 
        /// 是否使用该NPC
        /// </summary>
        public virtual bool HasCustomBodySegments => false;

        /// <summary>
        /// If not <see langword="null"/>, this NPC will target the given world position instead of its player target<br/>
        /// 如果不是 <see langword="null"/> ，则该 NPC 将针对给定世界位置而非其玩家目标进行目标设置
        /// </summary>
        public Vector2? ForcedTargetPosition { get; set; }

        /// <summary>
        /// Override this method to use custom body-spawning code.<br/>
        /// 覆盖此方法以使用自定义生成代码。 <br/>
        /// This method only runs if <see cref="HasCustomBodySegments"/> returns <see langword="true"/>.<br/>
        /// 只有在 <see cref="HasCustomBodySegments"/> 返回 <see langword="true"/> 时才运行此方法。
        /// </summary>
        /// <param name="segmentCount">How many body segements are expected to be spawned</param>
        /// <param name="segmentCount">预计要生成多少个身体片段</param>
        /// <returns>最近生成的 NPC 的 whoAmI 值，这是调用 <see cref="NPC.NewNPC(IEntitySource, int, int, int, int, float, float, float, float, int)"/> 的结果</returns>
        public virtual int SpawnBodySegments(int segmentCount)
        {
            // Defaults to just returning this NPC's whoAmI, since the tail segment uses the return value as its "following" NPC index
            // 默认只返回此 NPC 的 whoAmI，因为尾部段使用返回值作为其“跟随”NPC索引
            return NPC.whoAmI;
        }

        /// <summary>
        /// Spawns a body or tail segment of the worm.<br/>
        /// 生成虫体的身体或尾部分段。
        /// </summary>
        /// <param name="source">生成源</param><br/>
        /// <param name="type">要生成的片段 NPC 的 ID</param><br/>
        /// <param name="latestNPC">虫子中最近生成的片段 NPC（包括头）的 whoAmI 值</param><br/>
        /// <returns></returns>
        protected int SpawnSegment(IEntitySource source, int type, int latestNPC)
        {
            // We spawn a new NPC, setting latestNPC to the newer NPC, whilst also using that same variable
            // to set the parent of this new NPC. The parent of the new NPC (may it be a tail or body part)
            // will determine the movement of this new NPC.
            // Under there, we also set the realLife value of the new NPC, because of what is explained above.

            // 我们产生一个新的 NPC，将 latestNPC 设置为较新的 NPC，同时也使用同一变量
            // 来设置这个新 NPC 的父级。 新 NPC 的父级（可能是尾巴或身体部分）
            // 将决定这个新 NPC 的移动。
            // 在那里下面，我们还设置了新出现的NPC实际生命值，因为上面解释过了。
            int oldLatest = latestNPC;
            latestNPC = NPC.NewNPC(source, (int)NPC.Center.X, (int)NPC.Center.Y, type, NPC.whoAmI, 0, latestNPC);

            Main.npc[oldLatest].ai[0] = latestNPC;

            NPC latest = Main.npc[latestNPC];
            // NPC.realLife is the whoAmI of the NPC that the spawned NPC will share its health with
            //  npc.realLife realLife是NPC的whoAmI，生成的NPC将与之分享其生命值
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

                // 所以我们从检查开始AI如果npc.ai [0] （以下npcs谁ami）是0。
                // 这几乎总是刚刚产生一个 npc ，所以这意味着这是第一次更新。
                // 既然这是第一次更新，我们可以安全地假设需要产生蠕虫剩余部分（身体+尾巴）。

                bool hasFollower = NPC.ai[0] > 0;
                if (!hasFollower)
                {
                    // So, here we assign the NPC.realLife value.
                    // The NPC.realLife value is mainly used to determine which NPC loses life when we hit this NPC.
                    // We don't want every single piece of the worm to have its own HP pool, so this is a neat way to fix that.

                    // 所以，这里我们分配了NPC.realLife值。
                    // npc.realLife 值主要用于确定哪个 NPC 在我们击中此 NPC 时失去生命。
                    // 我们不希望蠕虫的每一部分都有自己的 HP 池，所以这是一个很好的解决方法。

                    NPC.realLife = NPC.whoAmI;
                    // latestNPC is going to be used in SpawnSegment() and I'll explain it there.
                    // latestNPC 将在 SpawnSegment() 中使用，我将在那里解释它。
                    int latestNPC = NPC.whoAmI;

                    // Here we determine the length of the worm.
                    // 这里我们确定蠕虫的长度。
                    int randomWormLength = Main.rand.Next(MinSegmentLength, MaxSegmentLength + 1);

                    int distance = randomWormLength - 2;

                    IEntitySource source = NPC.GetSource_FromAI();

                    if (HasCustomBodySegments)
                    {
                        // Call the method that'll handle spawning the body segments
                        // 调用处理生成身体段的方法
                        latestNPC = SpawnBodySegments(distance);
                    }
                    else
                    {
                        // Spawn the body segments like usual
                        // 像往常一样生成身体片段
                        while (distance > 0)
                        {
                            latestNPC = SpawnSegment(source, BodyType, latestNPC);
                            distance--;
                        }
                    }

                    // Spawn the tail segment
                    // 产生尾部片段
                    SpawnSegment(source, TailType, latestNPC);

                    NPC.netUpdate = true;

                    // Ensure that all of the segments could spawn.  If they could not, despawn the worm entirely
                    // 确保所有细节都可以生成。 如果不能，则完全删除蠕虫
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
                        // 无法生成所有的片段...杀死虫子
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
                    // 设置玩家目标以确保准确性
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
            // 确保瓷砖范围在世界边界内
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
            // 这是检查与瓷砖碰撞的初始检查。
            for (int i = minTilePosX; i < maxTilePosX; ++i)
            {
                for (int j = minTilePosY; j < maxTilePosY; ++j)
                {
                    Tile tile = Main.tile[i, j];

                    // If the tile is solid or is considered a platform, then there's valid collision
                    // 如果该瓷砖是实心或被视为平台，则存在有效碰撞。
                    if (tile.HasUnactuatedTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType] && tile.TileFrameY == 0) || tile.LiquidAmount > 64)
                    {
                        Vector2 tileWorld = new Point16(i, j).ToWorldCoordinates(0, 0);

                        if (NPC.Right.X > tileWorld.X && NPC.Left.X < tileWorld.X + 16 && NPC.Bottom.Y > tileWorld.Y && NPC.Top.Y < tileWorld.Y + 16)
                        {
                            // Collision found
                            // 发现碰撞
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
            // 如果没有与地块发生碰撞，我们将检查此NPC与其目标之间的距离是否过大，以便仍然可以触发“碰撞”。
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
                                   // 不是有效玩家

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

            // MoveSpeed确定此NPC可以移动的最大速度。
            // 更高的值=更快的速度。
            float speed = MoveSpeed;
            // acceleration is exactly what it sounds like. The speed at which this NPC accelerates.
            // 加速度就像它听起来一样。这个NPC加速的速度。
            float acceleration = Acceleration;

            float targetXPos, targetYPos;

            Player playerTarget = Main.player[NPC.target];

            Vector2 forcedTarget = ForcedTargetPosition ?? playerTarget.Center;
            // Using a ValueTuple like this allows for easy assignment of multiple values
            // 使用ValueTuple这样做允许轻松分配多个值
            (targetXPos, targetYPos) = (forcedTarget.X, forcedTarget.Y);

            // Copy the value, since it will be clobbered later
            // 复制该值，因为稍后会被覆盖掉
            Vector2 npcCenter = NPC.Center;

            float targetRoundedPosX = (int)(targetXPos / 16f) * 16;
            float targetRoundedPosY = (int)(targetYPos / 16f) * 16;
            npcCenter.X = (int)(npcCenter.X / 16f) * 16;
            npcCenter.Y = (int)(npcCenter.Y / 16f) * 16;
            float dirX = targetRoundedPosX - npcCenter.X;
            float dirY = targetRoundedPosY - npcCenter.Y;

            float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);

            // If we do not have any type of collision, we want the NPC to fall down and de-accelerate along the X axis.
            // 如果我们没有任何类型的冲突，我们希望NPC向下掉落并沿X轴减速。
            if (!collision && !CanFly)
                HeadAI_Movement_HandleFallingFromNoCollision(dirX, speed, acceleration);
            else
            {
                // Else we want to play some audio (soundDelay) and move towards our target.
                // 否则，我们想播放一些音频（soundDelay）并朝着目标移动。
                HeadAI_Movement_PlayDigSounds(length);

                HeadAI_Movement_HandleMovement(dirX, dirY, length, speed, acceleration);
            }

            HeadAI_Movement_SetRotation(collision);
        }

        private void HeadAI_Movement_HandleFallingFromNoCollision(float dirX, float speed, float acceleration)
        {
            // Keep searching for a new target
            // 继续寻找新目标
            NPC.TargetClosest(true);

            // Constant gravity of 0.11 pixels/tick
            // 重力恒定为0.11像素/刻度
            NPC.velocity.Y += 0.11f;

            // Ensure that the NPC does not fall too quickly
            // 确保NPC不会下降得太快
            if (NPC.velocity.Y > speed)
                NPC.velocity.Y = speed;

            // The following behaviour mimicks vanilla worm movement
            // 以下行为模仿香草蠕虫运动
            if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 0.4f)
            {
                // Velocity is sufficiently fast, but not too fast
                // 速度足够快，但不会太快
                if (NPC.velocity.X < 0.0f)
                    NPC.velocity.X -= acceleration * 1.1f;
                else
                    NPC.velocity.X += acceleration * 1.1f;
            }
            else if (NPC.velocity.Y == speed)
            {
                // NPC has reached terminal velocity
                // NPC已达到终端速度
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
                // 靠近目标位置时，播放声音速度更快
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
                // NPC 正在向目标位置移动
                if (NPC.velocity.X < dirX)
                    NPC.velocity.X += acceleration;
                else if (NPC.velocity.X > dirX)
                    NPC.velocity.X -= acceleration;

                if (NPC.velocity.Y < dirY)
                    NPC.velocity.Y += acceleration;
                else if (NPC.velocity.Y > dirY)
                    NPC.velocity.Y -= acceleration;

                // The intended Y-velocity is small AND the NPC is moving to the left and the target is to the right of the NPC or vice versa
                // 意图的 Y-速度很小，并且 NPC 向左移动而目标在其右侧或反之亦然
                if (Math.Abs(dirY) < speed * 0.2 && (NPC.velocity.X > 0 && dirX < 0 || NPC.velocity.X < 0 && dirX > 0))
                {
                    if (NPC.velocity.Y > 0)
                        NPC.velocity.Y += acceleration * 2f;
                    else
                        NPC.velocity.Y -= acceleration * 2f;
                }

                // The intended X-velocity is small AND the NPC is moving up/down and the target is below/above the NPC
                // 意图的 X-速度很小，并且 NPC 上下移动而目标在其下方或上方
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
                // X 距离大于 Y 距离。强制沿着 X 轴运动更强劲。
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
                // 为此 NPC 设置正确的旋转。强制沿着 X 轴运动更强劲。
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
            // // 为此 NPC 设置正确的旋转。
            // 假设该 NPC 的精灵朝上。您可能需要修改此行以正确考虑您的 NPC 方向。
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
            // 如果 NPC 的速度改变符号并且它没有被玩家“刚击中”，则强制进行净更新。
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

                // 如果身体/尾部段是单独生成的，则可能出现其中一些条件
                // 如果所跟随的片段NPC不再有效，则杀死该片段。
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

                // 在这个NPC前面跟随后面的片段
                // 使用当前NPC.Center计算指向该NPC“父级NPC”的方向。
                float dirX = following.Center.X - worm.NPC.Center.X;
                float dirY = following.Center.Y - worm.NPC.Center.Y;
                // We then use Atan2 to get a correct rotation towards that parent NPC.
                // Assumes the sprite for the NPC points upward.  You might have to modify this line to properly account for your NPC's orientation

                // 然后我们使用 Atan2 来获得正确朝向那个父级NPC的旋转。
                // 假设该 NPC 的精灵朝上。您可能需要修改此行以正确考虑您的 NPC 方向。
                worm.NPC.rotation = (float)Math.Atan2(dirY, dirX) + MathHelper.PiOver2;
                // We also get the length of the direction vector.
                // 我们还获取了方向矢量的长度。
                float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);
                // We calculate a new, correct distance.
                // 我们计算一个新的、正确的距离。
                float dist = (length - worm.NPC.width) / length;
                float posX = dirX * dist;
                float posY = dirY * dist;

                // Reset the velocity of this NPC, because we don't want it to move on its own
                // 重置此 NPC 的速度，因为我们不希望它自己移动
                worm.NPC.velocity = Vector2.Zero;
                // And set this NPCs position accordingly to that of this NPCs parent NPC.
                // 并将此 NPC 的位置设置为其父级 NPC 的位置。
                worm.NPC.position.X += posX;
                worm.NPC.position.Y += posY;
            }
        }
    }

    // Since the body and tail segments share the same AI
    // 因为身体和尾部段共享相同的 AI
    public abstract class WormTail : Worm
    {
        public sealed override WormSegmentType SegmentType => WormSegmentType.Tail;

        internal override void BodyTailAI()
        {
            WormBody.CommonAI_BodyTail(this);
        }
    }
}
