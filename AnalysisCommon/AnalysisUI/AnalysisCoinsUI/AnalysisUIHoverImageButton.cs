using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace AnalysisMod.AnalysisCommon.AnalysisUI.AnalysisCoinsUI
{
    // 这个 AnalysisUIHoverImageButton 类继承自 UIImageButton。
    // 继承是 UI 设计中的一个很好的工具。
    // 通过继承，我们可以从 UIImageButton 中免费获得图像绘制、鼠标悬停声音和淡入淡出效果。
    // 我们添加了一些代码，使按钮在悬停时显示文本提示。
    internal class AnalysisUIHoverImageButton : UIImageButton
    {
        // Tooltip text that will be shown on hover
        // 悬停时将显示的提示文本
        internal string hoverText;

        public AnalysisUIHoverImageButton(Asset<Texture2D> texture, string hoverText) : base(texture)
        {
            this.hoverText = hoverText;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            // 当你重写 UIElement 方法时，请不要忘记调用基类方法
            // 这有助于保持 UIElement 的基本行为
            base.DrawSelf(spriteBatch);

            // 当鼠标悬停在当前 UIElement 上时，IsMouseHovering 变为 true
            if (IsMouseHovering)
                Main.hoverItemName = hoverText;
        }
    }
}
