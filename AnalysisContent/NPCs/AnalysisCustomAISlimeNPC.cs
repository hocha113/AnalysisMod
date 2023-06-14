using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace AnalysisMod.AnalysisContent.NPCs
{
    // This ModNPC serves as an Analysis of a completely custom AI.
    // 这个ModNPC作为完全自定义AI的分析。
    public class AnalysisCustomAISlimeNPC : ModNPC
    {
        // Here we define an enum we will use with the State slot. Using an ai slot as a means to store "state" can simplify things greatly. Think flowchart.
        // 在这里我们定义了一个枚举，将与State槽一起使用。使用ai槽作为存储“状态”的手段可以大大简化事情。想象流程图。
        private enum ActionState
        {
            Asleep,
            Notice,
            Jump,
            Hover,
            Fall
        }

        // Our texture is 36x36 with 2 pixels of padding vertically, so 38 is the vertical spacing.
        // These are for our benefit and the numbers could easily be used directly in the code below, but this is how we keep code organized.

        // 我们的纹理是36x36，垂直有2像素的填充，因此38是垂直间距。
        // 这些都是为了我们方便而设定的数字，并且可以直接在下面的代码中使用，但这就是如何保持代码组织良好。
        private enum Frame
        {
            Asleep,
            Notice,
            Falling,
            Flutter1,
            Flutter2,
            Flutter3
        }

        // These are reference properties. One, for Analysis, lets us write AI_State as if it's NPC.ai[0], essentially giving the index zero our own name.
        // Here they help to keep our AI code clear of clutter. Without them, every instance of "AI_State" in the AI code below would be "npc.ai[0]", which is quite hard to read.
        // This is all to just make beautiful, manageable, and clean code.

        // 这些都是参考属性。其中之一Analysis让我们编写AI_State就像它是NPC.ai[0]一样，本质上给索引零取一个名字。
        // 在这里它们有助于使我们的AI代码清晰无杂物。如果没有它们，在下面的AI代码中每个“ AI_State”实例都将成为“ npc.ai [0]”，这很难阅读。
        // 所有这些只是为了创造美观、易管理和干净整洁的代码。
        public ref float AI_State => ref NPC.ai[0];
        public ref float AI_Timer => ref NPC.ai[1];
        public ref float AI_FlutterTime => ref NPC.ai[2];

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6; // make sure to set this for your modnpcs.
                                              // 确保对您的modnpcs进行设置。

            NPCID.Sets.ShimmerTransformToNPC[NPC.type] = NPCID.ShimmerSlime;

            // Specify the debuffs it is immune to
            // 指定其免疫哪些debuff
            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
                    BuffID.Poisoned // This NPC will be immune to the Poisoned debuff.
                                    // 该NPC将免疫中毒debuff。
				}
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 36; // The width of the npc's hitbox (in pixels)
                            // npc碰撞箱宽度（以像素为单位）
            NPC.height = 36; // The height of the npc's hitbox (in pixels)
                             // npc碰撞箱高度（以像素为单位）
            NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
                              // 该npc具有完全独特的AI，因此我们将其设置为-1。默认的aiStyle 0会面向玩家，这可能与自定义AI代码冲突。

            NPC.damage = 7; // The amount of damage that this npc deals
                            // 该npc造成的伤害量

            NPC.defense = 2; // The amount of defense that this npc has
                             // 该npc具有的防御值

            NPC.lifeMax = 25; // The amount of health that this npc has
                              // 该npc拥有的生命值

            NPC.HitSound = SoundID.NPCHit1; // The sound the NPC will make when being hit.
                                            // NPC被击中时发出声音。

            NPC.DeathSound = SoundID.NPCDeath1; // The sound the NPC will make when it dies.
                                                // NPC死亡时发出声音。

            NPC.value = 25f; // How many copper coins the NPC will drop when killed.
                             // 当杀死时，NPC会掉落多少铜币。
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            // we would like this npc to spawn in the overworld.
            // 我们希望这个npc在外界生成。
            return SpawnCondition.OverworldDaySlime.Chance * 0.1f;
        }

        // Our AI here makes our NPC sit waiting for a player to enter range, jumps to attack, flutter mid-fall to stay afloat a little longer, then falls to the ground. Note that animation should happen in FindFrame
        // 我们的AI使我们的NPC坐等玩家进入范围内，跳起来攻击，飘动以延长下降时间，然后落到地上。请注意，在FindFrame中应进行动画处理
        public override void AI()
        {
            // The npc starts in the asleep state, waiting for a player to enter range
            // npc从睡眠状态开始，等待玩家进入范围内
            switch (AI_State)
            {
                case (float)ActionState.Asleep:
                    FallAsleep();
                    break;
                case (float)ActionState.Notice:
                    Notice();
                    break;
                case (float)ActionState.Jump:
                    Jump();
                    break;
                case (float)ActionState.Hover:
                    Hover();
                    break;
                case (float)ActionState.Fall:
                    if (NPC.velocity.Y == 0)
                    {
                        NPC.velocity.X = 0;
                        AI_State = (float)ActionState.Asleep;
                        AI_Timer = 0;
                    }

                    break;
            }
        }

        // Here in FindFrame, we want to set the animation frame our npc will use depending on what it is doing.
        // We set npc.frame.Y to x * frameHeight where x is the xth frame in our spritesheet, counting from 0. For convenience, we have defined a enum above.

        // 在FindFrame中，在根据它正在做什么设置动画帧之前要设置我们的npc将使用哪个动画帧。
        // 我们将npc.frame.Y设置为x * frameHeight，其中x是从0开始计算我们精灵图表格中第x个框架。为方便起见，在上面定义了一个枚举类型。
        public override void FindFrame(int frameHeight)
        {
            // This makes the sprite flip horizontally in conjunction with the npc.direction.
            // 这使得精灵图水平翻转与npc.direction相结合。
            NPC.spriteDirection = NPC.direction;

            // For the most part, our animation matches up with our states.
            // 在大多数情况下，我们的动画与状态匹配。
            switch (AI_State)
            {
                case (float)ActionState.Asleep:
                    // npc.frame.Y is the goto way of changing animation frames. npc.frame starts from the top left corner in pixel coordinates, so keep that in mind.
                    // npc.frame.Y是更改动画帧goto方式。 npc.frame从左上角像素坐标开始，请记住这一点。
                    NPC.frame.Y = (int)Frame.Asleep * frameHeight;
                    break;
                case (float)ActionState.Notice:
                    // Going from Notice to Asleep makes our npc look like it's crouching to jump.
                    // 从注意到睡眠使我们的npc看起来像是蹲下跳。
                    if (AI_Timer < 10)
                    {
                        NPC.frame.Y = (int)Frame.Notice * frameHeight;
                    }
                    else
                    {
                        NPC.frame.Y = (int)Frame.Asleep * frameHeight;
                    }

                    break;
                case (float)ActionState.Jump:
                    NPC.frame.Y = (int)Frame.Falling * frameHeight;
                    break;
                case (float)ActionState.Hover:
                    // Here we have 3 frames that we want to cycle through.
                    // 在这里，我们有3个要循环遍历的框架。
                    NPC.frameCounter++;

                    if (NPC.frameCounter < 10)
                    {
                        NPC.frame.Y = (int)Frame.Flutter1 * frameHeight;
                    }
                    else if (NPC.frameCounter < 20)
                    {
                        NPC.frame.Y = (int)Frame.Flutter2 * frameHeight;
                    }
                    else if (NPC.frameCounter < 30)
                    {
                        NPC.frame.Y = (int)Frame.Flutter3 * frameHeight;
                    }
                    else
                    {
                        NPC.frameCounter = 0;
                    }

                    break;
                case (float)ActionState.Fall:
                    NPC.frame.Y = (int)Frame.Falling * frameHeight;
                    break;
            }
        }

        // Here, because we use custom AI (aiStyle not set to a suitable vanilla value), we should manually decide when Flutter Slime can fall through platforms
        // 在这里，因为我们使用了自定义AI（aiStyle未设置为适当的基础值），我们需要手动决定Flutter Slime何时可以穿过平台掉落
        public override bool? CanFallThroughPlatforms()
        {
            if (AI_State == (float)ActionState.Fall && NPC.HasValidTarget && Main.player[NPC.target].Top.Y > NPC.Bottom.Y)
            {
                // If Flutter Slime is currently falling, we want it to keep falling through platforms as long as it's above the player
                // 如果Flutter Slime当前正在下落，只要它在玩家上方，我们希望它继续穿过平台下落
                return true;
            }

            return false;
            // You could also return null here to apply vanilla behavior (which is the same as false for custom AI)
            // 您也可以在此处返回null以应用基础行为（与自定义AI的false相同）
        }

        private void FallAsleep()
        {
            // TargetClosest sets npc.target to the player.whoAmI of the closest player.
            // The faceTarget parameter means that npc.direction will automatically be 1 or -1 if the targeted player is to the right or left.
            // This is also automatically flipped if npc.confused.

            // TargetClosest将npc.target设置为最近玩家的player.whoAmI。
            // faceTarget参数意味着如果目标玩家在右侧或左侧，则npc.direction将自动变为1或-1。
            // 如果npc.confused，则也会自动翻转。
            NPC.TargetClosest(true);

            // Now we check the make sure the target is still valid and within our specified notice range (500)
            // 现在我们检查目标是否仍然有效并且在指定的通知范围内（500）
            if (NPC.HasValidTarget && Main.player[NPC.target].Distance(NPC.Center) < 500f)
            {
                // Since we have a target in range, we change to the Notice state. (and zero out the Timer for good measure)
                // 由于我们有一个距离内的目标，所以切换到Notice状态。（并清零计时器以确保准确性）
                AI_State = (float)ActionState.Notice;
                AI_Timer = 0;
            }
        }

        private void Notice()
        {
            // If the targeted player is in attack range (250).
            // 如果被攻击者处于攻击范围内（250）。
            if (Main.player[NPC.target].Distance(NPC.Center) < 250f)
            {
                // Here we use our Timer to wait .33 seconds before actually jumping. In FindFrame you'll notice AI_Timer also being used to animate the pre-jump crouch
                // 在此处，我们使用计时器等待0.33秒才实际跳跃。您会注意到，在FindFrame中还使用了AI_Timer来播放预跳跃蹲姿动画
                AI_Timer++;

                if (AI_Timer >= 20)
                {
                    AI_State = (float)ActionState.Jump;
                    AI_Timer = 0;
                }
            }
            else
            {
                NPC.TargetClosest(true);

                if (!NPC.HasValidTarget || Main.player[NPC.target].Distance(NPC.Center) > 500f)
                {
                    // Out targeted player seems to have left our range, so we'll go back to sleep.
                    // 我们发现目标玩家已经超出了范围，所以回去休息吧。
                    AI_State = (float)ActionState.Asleep;
                    AI_Timer = 0;
                }
            }
        }

        private void Jump()
        {
            AI_Timer++;

            if (AI_Timer == 1)
            {
                // We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
                // 第一次进入Jump帧时，我们应用初始速度。请记住-Y是向上的。
                NPC.velocity = new Vector2(NPC.direction * 2, -10f);
            }
            else if (AI_Timer > 40)
            {
                // after .66 seconds, we go to the hover state. //TODO, gravity?
                // 0.66秒后，我们进入悬停状态。 //TODO, gravity?
                AI_State = (float)ActionState.Hover;
                AI_Timer = 0;
            }
        }

        private void Hover()
        {
            AI_Timer++;

            // Here we make a decision on how long this flutter will last. We check netmode != 1 to prevent Multiplayer Clients from running this code. (similarly, spawning projectiles should also be wrapped like this)
            // netMode == 0 is SP, netMode == 1 is MP Client, netMode == 2 is MP Server.
            // Typically in MP, Client and Server maintain the same state by running deterministic code individually. When we want to do something random, we must do that on the server and then inform MP Clients.

            // 在这里，我们决定这个flutter会持续多久。检查netmode！=1以防止多人游戏客户端运行此代码。（同样地，生成projectiles也应该像这样包装）
            // netMode == 0 是单机模式（SP），netMode == 1 是多人游戏客户端（MP Client），netMode == 2 是多人游戏服务器（MP Server）。
            // 在大型联机中，客户端和服务器通过分别运行确定性代码来保持相同的状态。当我们想要做一些随机的事情时，必须在服务器上执行然后通知MP Clients。
            if (AI_Timer == 1 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                // For reference: without proper syncing: https://gfycat.com/BackAnxiousFerret and with proper syncing: https://gfycat.com/TatteredKindlyDalmatian
                // 参考：没有适当同步：https://gfycat.com/BackAnxiousFerret 和有适当同步：https://gfycat.com/TatteredKindlyDalmatian
                AI_FlutterTime = Main.rand.NextBool() ? 100 : 50;

                // Informing MP Clients is done automatically by syncing the npc.ai array over the network whenever npc.netUpdate is set.
                // Don't set netUpdate unless you do something non-deterministic ("random")

                // 自动通过在设置npc.netUpdate时将npc.ai数组与网络进行同步来通知MP Clients。
                // 不要设置netUpdate除非您执行了某些不确定性操作（“随机”）
                NPC.netUpdate = true;
            }

            // Here we add a tiny bit of upward velocity to our npc.
            // 在这里为npc添加了一点向上的速度。
            NPC.velocity += new Vector2(0, -.35f);

            // ... and some additional X velocity when traveling slow.
            // ...并且在缓慢移动时增加额外X速度。
            if (Math.Abs(NPC.velocity.X) < 2)
            {
                NPC.velocity += new Vector2(NPC.direction * .05f, 0);
            }

            // after fluttering for 100 ticks (1.66 seconds), our Flutter Slime is tired, so he decides to go into the Fall state.
            // 持续飞舞100个tick（1.66秒）后，Flutter Slime感到疲倦，并决定进入Fall状态。
            if (AI_Timer > AI_FlutterTime)
            {
                AI_State = (float)ActionState.Fall;
                AI_Timer = 0;
            }
        }

        public override bool ModifyCollisionData(Rectangle victimHitbox, ref int immunityCooldownSlot, ref MultipliableFloat damageMultiplier, ref Rectangle npcHitbox)
        {
            // We can use ModifyCollisionData to customize collision damage.
            // Here we double damage when this npc is in the falling state and the victim is almost directly below the npc

            // 我们可以使用ModifyCollisionData来自定义碰撞伤害。
            // 在此处，当该npc处于下落状态且受害者几乎直接在npc下方时，我们将伤害加倍
            if (AI_State == (float)ActionState.Fall)
            {
                // We can modify npcHitbox directly to implement a dynamic hitbox, but in this Analysis we make a new hitbox to apply bonus damage
                // This math creates a hitbox focused on the bottom center of the original 36x36 hitbox:

                // 我们可以直接修改npcHitbox以实现动态hitbox，但是在本分析中，我们创建了一个新的hitbox以应用奖励伤害
                // 这个数学公式创建了一个聚焦于原始36x36 hitbox底部中心的hitbox：
                // --> ☐☐☐
                //     ☐☒☐
                Rectangle extraDamageHitbox = new Rectangle(npcHitbox.X + 12, npcHitbox.Y + 18, npcHitbox.Width - 24, npcHitbox.Height - 18);
                if (victimHitbox.Intersects(extraDamageHitbox))
                {
                    damageMultiplier *= 2f;
                    Main.NewText("You got stomped");
                }
            }
            return true;
        }
    }
}
