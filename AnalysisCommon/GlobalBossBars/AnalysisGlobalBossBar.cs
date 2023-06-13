using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.GameContent;
using Terraria.ModLoader;
using ReLogic.Graphics;
using Terraria.DataStructures;

namespace AnalysisMod.AnalysisCommon.GlobalBossBars
{
    // Shows things you can do around drawing boss bars
    // 展示如何在绘制首领血条周围进行操作
    public class AnalysisGlobalBossBar : GlobalBossBar
    {
        public override bool PreDraw(SpriteBatch spriteBatch, NPC npc, ref BossBarDrawParams drawParams)
        {
            if (npc.type == NPCID.EyeofCthulhu)
            {
                drawParams.IconColor = Main.DiscoColor;
            }

            return true;
        }

        public override void PostDraw(SpriteBatch spriteBatch, NPC npc, BossBarDrawParams drawParams)
        {
            if (npc.type == NPCID.EyeofCthulhu)
            {
                string text = "GlobalBossBar Showcase";
                var font = FontAssets.MouseText.Value;
                Vector2 size = font.MeasureString(text);
                // Draw centered on the boss bar, offset upwards, otherwise it will overlap with the health text
                // 在首领血条中央绘制，向上偏移，否则会与生命值文本重叠
                spriteBatch.DrawString(font, text, drawParams.BarCenter - size / 2 + new Vector2(0, -30), Color.White);
            }
        }
    }
}
