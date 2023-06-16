using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace AnalysisMod.AnalysisCommon.AnalysisUI.AnalysisCoinsUI
{
    // This DragableUIPanel class inherits from UIPanel
    // Inheriting is a great tool for UI design. By inheriting, we get the background drawing for free from UIPanel
    // We've added some code to allow the panel to be dragged around
    // We've also added some code to ensure that the panel will bounce back into bounds if it is dragged outside or the screen resizes
    // UIPanel does not prevent the player from using items when the mouse is clicked, so we've added that as well

    // 这个DragableUIPanel类继承自UIPanel
    // 继承是UI设计的一个很好的工具。通过继承，我们可以从UIPanel中免费获得背景绘制。
    // 我们添加了一些代码来允许面板被拖动
    // 我们还添加了一些代码，以确保如果将其拖到屏幕外或调整屏幕大小，则面板会反弹回边界。
    // UIPanel不会阻止玩家在单击鼠标时使用物品，因此我们也添加了这个功能。
    public class AnalysisDragableUIPanel : UIPanel
    {
        // Stores the offset from the top left of the UIPanel while dragging
        // 存储在拖动时相对于UIPanel左上角的偏移量
        private Vector2 offset;

        // A flag that checks if the panel is currently being dragged
        // 用于检查面板是否正在被拖动的标志
        private bool dragging;

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            // When you override UIElement methods, don't forget call the base method
            // This helps to keep the basic behavior of the UIElement

            // 当你重写UIElement方法时，请不要忘记调用基本方法
            // 这有助于保持基本行为的UIElement
            base.LeftMouseDown(evt);

            // When the mouse button is down, then we start dragging
            // 当鼠标按钮按下时，开始拖动
            DragStart(evt);
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            base.LeftMouseUp(evt);
            // When the mouse button is up, then we stop dragging
            // 当鼠标按钮按下时，开始拖动
            DragEnd(evt);
        }

        private void DragStart(UIMouseEvent evt)
        {
            // The offset variable helps to remember the position of the panel relative to the mouse position
            // So no matter where you start dragging the panel, it will move smoothly

            // 偏移变量有助于记住与鼠标位置相关联的面板位置，
            // 因此无论您从哪里开始拖放该面板，它都会平稳地移动。
            offset = new Vector2(evt.MousePosition.X - Left.Pixels, evt.MousePosition.Y - Top.Pixels);
            dragging = true;
        }

        private void DragEnd(UIMouseEvent evt)
        {
            Vector2 endMousePosition = evt.MousePosition;
            dragging = false;

            Left.Set(endMousePosition.X - offset.X, 0f);
            Top.Set(endMousePosition.Y - offset.Y, 0f);

            Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Checking ContainsPoint and then setting mouseInterface to true is very common
            // This causes clicks on this UIElement to not cause the player to use current items

            // 检查ContainsPoint然后将mouseInterface设置为true非常常见
            // 这使得对此UI元素进行点击不会导致玩家使用当前物品
            if (ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            if (dragging)
            {
                Left.Set(Main.mouseX - offset.X, 0f); // Main.MouseScreen.X and Main.mouseX are the same
                                                      // Main.MouseScreen.X和Main.mouseX是相同的。
                Top.Set(Main.mouseY - offset.Y, 0f);
                Recalculate();
            }

            // Here we check if the DragableUIPanel is outside the Parent UIElement rectangle
            // (In our Analysis, the parent would be AnalysisCoinsUI, a UIState. This means that we are checking that the DragableUIPanel is outside the whole screen)
            // By doing this and some simple math, we can snap the panel back on screen if the user resizes his window or otherwise changes resolution

            // 在这里，我们检查DragableUIPanel是否在父UIElement矩形之外
            // （在我们的分析中，父级将是AnalysisCoinsUI，一个UIState。这意味着我们正在检查DragableUIPanel是否超出了整个屏幕）
            // 通过执行此操作以及一些简单的数学运算，如果用户调整窗口大小或以其他方式更改分辨率，则可以将面板捕捉回屏幕上。
            var parentSpace = Parent.GetDimensions().ToRectangle();
            if (!GetDimensions().ToRectangle().Intersects(parentSpace))
            {
                Left.Pixels = Utils.Clamp(Left.Pixels, 0, parentSpace.Right - Width.Pixels);
                Top.Pixels = Utils.Clamp(Top.Pixels, 0, parentSpace.Bottom - Height.Pixels);

                // Recalculate forces the UI system to do the positioning math again.
                // Recalculate强制使UI系统重新进行定位计算。
                Recalculate();
            }
        }
    }
}
