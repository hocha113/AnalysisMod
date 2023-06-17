using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.BossBars
{
    // Shows basic boss bar code using a custom colored texture. It only does visual things, so for a more practical boss bar, see the other Analysis (MinionBossBossBar)
    // To use this, in an NPCs SetDefaults, write:

    // 使用自定义颜色纹理显示基本的BOSS血条代码。它只做视觉效果，如果需要更实用的BOSS血条，请参见其他分析（MinionBossBossBar）
    // 要使用此功能，在NPCs SetDefaults中编写：
    //  NPC.BossBar = ModContent.GetInstance<AnalysisBossBar>();

    // Keep in mind that if the NPC has a boss head icon, it will automatically have the common boss health bar from vanilla. A ModBossBar is not mandatory for a boss.
    // 请注意，如果NPC有一个boss头像，它将自动拥有来自原版的普通boss生命条。ModBossBar对于boss并非强制要求。

    // You can make it so your NPC never shows a boss bar, such as Dungeon Guardian or Lunatic Cultist Clone:
    // 您可以使您的NPC永远不显示BOSS血条，例如Dungeon Guardian或Lunatic Cultist Clone：
    // NPC.BossBar = Main.BigBossProgressBar.NeverValid;
    public class AnalysisBossBar : ModBossBar
    {
        public override Asset<Texture2D> GetIconTexture(ref Rectangle? iconFrame)
        {
            return TextureAssets.NpcHead[36]; // Corgi head icon
                                              // Corgi头像
        }

        public override string Texture => base.Texture;

        public override bool PreDraw(SpriteBatch spriteBatch, NPC npc, ref BossBarDrawParams drawParams)
        {
            // Make the bar shake the less health the NPC has
            // 让血量越少时条形图震动
            float lifePercent = drawParams.Life / drawParams.LifeMax;
            float shakeIntensity = Utils.Clamp(1f - lifePercent - 0.2f, 0f, 1f);
            drawParams.BarCenter.Y -= 20f;
            drawParams.BarCenter += Main.rand.NextVector2Circular(0.5f, 0.5f) * shakeIntensity * 15f;

            drawParams.IconColor = Main.DiscoColor;

            return true;
        }
    }
}
