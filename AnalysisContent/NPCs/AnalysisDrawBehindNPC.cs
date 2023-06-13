using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.NPCs
{
    // This NPC is simply an exhibition of the DrawBehind method.
    // The npc cycles between all the available "layers" that a ModNPC can be drawn at.
    // Spawn this NPC with something like Cheat Sheet or Hero's Mod to view the effect.

    // 这个NPC只是DrawBehind方法的展示。
    // NPC会在所有可用的ModNPC绘制层之间循环。
    // 使用Cheat Sheet或Hero's Mod之类的工具生成此NPC以查看效果。
    public class AnalysisDrawBehindNPC : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // Total count animation frames
            // 总帧数
            Main.npcFrameCount[NPC.type] = 6;
        }

        public override void SetDefaults()
        {
            //【这些字段的翻译请见之前的分析案例】
            NPC.width = 30; // The width of the npc hitbox
            NPC.height = 40; // The height of the npc hitbox
            NPC.aiStyle = -1; // Using custom AI
            NPC.damage = 0; // The amount of damage this NPC will deal on collision
            NPC.defense = 2; // How resistant to damage this NPC is
            NPC.lifeMax = 100; // The maximum life of this NPC
            NPC.HitSound = SoundID.NPCHit2; // The sound that plays when this npc is hit
            NPC.DeathSound = SoundID.NPCDeath2; // The sound that plays when this npc dies
            NPC.noGravity = true; // If true, the npc will not be affected by gravity
            NPC.noTileCollide = true; // If true, the npc does not collide with tiles
            NPC.knockBackResist = 0f; // How much of the knockback it receives will actually apply. 1f: full knockback; 0f: no knockback
        }

        // The current drawing layer will change every 40 ticks
        // 当前绘制层将每40个tick更改一次
        private int CurrentLayer => (int)(NPC.ai[0] / 40);

        // This changes the frame from the this NPC's texture that is drawn, depending on the current layer
        // 根据当前图层更改要绘制的这个NPC纹理中的帧
        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Y = CurrentLayer * frameHeight;
        }

        public override void AI()
        {
            NPC.ai[0] = (NPC.ai[0] + 1) % 240;

            // These are the defaults for normal drawing(case 3)
            // 这些是正常绘画（case 3）的默认值
            NPC.hide = false;
            NPC.behindTiles = false;

            switch (CurrentLayer)
            {
                case 0:
                case 1:
                case 4:
                case 5:
                    NPC.hide = true;
                    break;
                case 2:
                    NPC.behindTiles = true;
                    break;
                case 3:
                    break;
            }
        }

        // This method allows you to specify that this npc should be drawn behind certain elements
        // 此方法允许您指定此npc应该在某些元素后面绘制
        public override void DrawBehind(int index)
        {
            // The 6 available positions are as follows:
            // 可用位置如下：
            switch (CurrentLayer)
            {
                case 0: // Behind tiles and walls
                        // 背景瓷砖和墙壁后面

                    Main.instance.DrawCacheNPCsMoonMoon.Add(index);
                    break;
                case 1: // Behind non solid tiles, but in front of walls
                        // 非实心瓷砖后面，但在墙壁前面

                    Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index);
                    break;
                case 2: // Behind tiles, but in front of non solid tiles
                        // 瓷砖后面，但在非实心瓷砖前面

                case 3: // Normal (in front of tiles)
                        // 正常(在瓷砖前)

                    break;
                case 4: // In front of all normal NPC
                        // 所有普通NPC前方

                    Main.instance.DrawCacheNPCProjectiles.Add(index);
                    break;
                case 5: // In front of Players
                        // 玩家前方
                    Main.instance.DrawCacheNPCsOverPlayers.Add(index);
                    break;
            }
        }
    }
}
