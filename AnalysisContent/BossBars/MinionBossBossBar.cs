using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.GameContent.UI.BigProgressBar;
using AnalysisMod.AnalysisContent.NPCs.MinionBoss;

namespace AnalysisMod.AnalysisContent.BossBars
{
    // Showcases a custom boss bar with basic logic for displaying the icon, life, and shields properly.
    // Has no custom texture, meaning it will use the default vanilla boss bar texture

    // 展示一个自定义的Boss血条，具有显示图标、生命值和护盾的基本逻辑。
    // 没有自定义纹理，因此将使用默认的香草Boss血条纹理。
    public class MinionBossBossBar : ModBossBar
    {
        private int bossHeadIndex = -1;

        public override Asset<Texture2D> GetIconTexture(ref Rectangle? iconFrame)
        {
            

            // Display the previously assigned head index
            // 显示先前分配的头部索引
            if (bossHeadIndex != -1)
            {
                return TextureAssets.NpcHeadBoss[bossHeadIndex];
            }
            return null;
        }

        public override bool? ModifyInfo(ref BigProgressBarInfo info, ref float life, ref float lifeMax, ref float shield, ref float shieldMax)
        {
            // Here the game wants to know if to draw the boss bar or not. Return false whenever the conditions don't apply.
            // If there is no possibility of returning false (or null) the bar will get drawn at times when it shouldn't, so write defensive code!

            // 游戏想知道是否绘制Boss血条。当条件不适用时返回false。
            // 如果没有可能返回false（或null），则在不应该绘制时会绘制该栏，因此编写防御性代码！

            NPC npc = Main.npc[info.npcIndexToAimAt];
            if (!npc.active)
                return false;

            // We assign bossHeadIndex here because we need to use it in GetIconTexture
            // 我们在这里分配bossHeadIndex是因为我们需要在GetIconTexture中使用它
            bossHeadIndex = npc.GetBossHeadTextureIndex();

            life = npc.life;
            lifeMax = npc.lifeMax;

            if (npc.ModNPC is MinionBossBody body)
            {
                // We did all the calculation work on RemainingShields inside the body NPC already so we just have to fetch the value again
                // 我们已经在NPC主体内完成了RemainingShields上所有计算工作，因此我们只需再次获取该值
                shield = body.MinionHealthTotal;
                shieldMax = body.MinionMaxHealthTotal;
            }

            return true;
        }
    }
}
