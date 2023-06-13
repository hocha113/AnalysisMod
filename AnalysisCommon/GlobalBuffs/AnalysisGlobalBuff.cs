//AnalysisMod.AnalysisContent.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisCommon.GlobalBuffs
{
    // Showcases how to work with all buffs
    // 展示如何使用所有增益效果
    public class AnalysisGlobalBuff : GlobalBuff
    {
        public static LocalizedText RemainingTimeText { get; private set; }

        public override void SetStaticDefaults()
        {
            RemainingTimeText = Language.GetOrRegister(Mod.GetLocalizationKey($"{nameof(AnalysisGlobalBuff)}.RemainingTime"));
        }

        public override void Update(int type, Player player, ref int buffIndex)
        {
            // If the player gets the Chilled debuff while he already has more than 5 other buffs/debuffs, limit the max duration to 3 seconds
            // 如果玩家在已经拥有5个以上的增益/减益效果时获得了冰冻debuff，则将最大持续时间限制为3秒
            if (type == BuffID.Chilled && buffIndex >= 5)
            {
                int limit = 3 * 60;
                if (player.buffTime[buffIndex] > limit)
                {
                    player.buffTime[buffIndex] = limit;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, int type, int buffIndex, ref BuffDrawParams drawParams)
        {
            // Make the campfire buff have a different color and shake slightly
            // 让篝火增益拥有不同的颜色并微微晃动
            if (type == BuffID.Campfire)
            {
                drawParams.DrawColor = Main.DiscoColor * Main.buffAlpha[buffIndex];

                Vector2 shake = new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-2, 3));

                drawParams.Position += shake;
                drawParams.TextPosition += shake;
            }

            // If the buff is not drawn in the hook/mount/pet equip page, and the buff is one of the three specified:
            // 如果在挂钩/装备宠物页面中没有绘制出该buff，并且该buff是指定的三个之一：
            if (Main.EquipPage != 2 && (type == BuffID.Regeneration || type == BuffID.Ironskin || type == BuffID.Swiftness))
            {
                // Make text go up and down 6 pixels on each buff, offset by 4 ticks for each
                // 每个buff上下偏移6像素，每4个tick偏移一次
                int interval = 60;
                float time = ((int)Main.GameUpdateCount + 4 * buffIndex) % interval / (float)interval;

                int offset = (int)(6 * time);

                ref Vector2 textPos = ref drawParams.TextPosition; // You can use ref locals to keep modifying the same variable
                                                                   // 您可以使用 ref 局部变量来保持对同一变量的修改。
                textPos.Y += offset;
            }

            // Return true to let the game draw the buff icon.
            // 返回 true 以让游戏绘制增益图标。
            return true;
        }

        private static string randomBuffTextCache;
        private static int randomBuffTypeCache;

        public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare)
        {
            // This code adds a more extensible remaining time tooltip for suitable buffs
            // 这段代码为适当的增益效果添加了一个更具可扩展性的剩余时间工具提示
            Player player = Main.LocalPlayer;

            int buffIndex = player.FindBuffIndex(type);
            if (buffIndex < 0 || buffIndex >= player.buffTime.Length)
            {
                return;
            }

            if (Main.TryGetBuffTime(buffIndex, out int buffTimeValue) && buffTimeValue > 2)
            {
                string text = Lang.LocalizedDuration(new System.TimeSpan(0, 0, buffTimeValue / 60), abbreviated: false, showAllAvailableUnits: true);
                tip += "\n" + RemainingTimeText.Format(text);
            }

            // This code showcases adjusting buffName. Try it out by activating a Slice of Cake block
            // 这段代码展示了如何调整buffName。通过激活一个蛋糕块来尝试一下
            if (player.HasBuff(BuffID.SugarRush) && buffName.Length > 2)
            {
                if (Main.GameUpdateCount % 10 == 0 || randomBuffTypeCache != type)
                {
                    if (randomBuffTypeCache != type)
                    {
                        randomBuffTextCache = buffName;
                        randomBuffTypeCache = type;
                    }
                    char[] characters = randomBuffTextCache.ToCharArray();
                    int n = characters.Length;
                    int swaps = Main.rand.Next(1, randomBuffTextCache.Length / 2);
                    for (int swap = 0; swap < swaps; swap++)
                    {

                        int a = Main.rand.Next(n - 1);
                        int b = Main.rand.Next(a + 1, n);
                        Utils.Swap(ref characters[a], ref characters[b]);
                    }
                    randomBuffTextCache = new string(characters);
                }
                buffName = randomBuffTextCache;
            }
        }

        public override bool RightClick(int type, int buffIndex)
        {
            // This code makes it so while the player is standing still, he cannot remove the "AnalysisDefenseBuff" by right clicking the icon
            // 这段代码使得当玩家静止不动时，无法通过右键点击图标来移除“AnalysisDefenseBuff”效果
            if (type == BuffID.Ironskin && Main.LocalPlayer.velocity == Vector2.Zero)
            {
                Main.NewText("Cannot cancel this buff while stationary!");
                return false;
            }

            return base.RightClick(type, buffIndex);
        }
    }
}
