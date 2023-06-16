using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using AnalysisMod.AnalysisCommon.Players;
using AnalysisMod.AnalysisContent.Items.Weapons;
using Terraria.GameContent;
using System.Collections.Generic;

namespace AnalysisMod.AnalysisCommon.AnalysisUI.AnalysisResourceUI
{
    // This custom UI will show whenever the player is holding the AnalysisCustomResourceWeapon item and will display the player's custom resource amounts that are tracked in AnalysisResourcePlayer
    // 当玩家持有AnalysisCustomResourceWeapon物品时，此自定义UI将显示，并显示在AnalysisResourcePlayer中跟踪的玩家自定义资源数量。
    internal class AnalysisResourceBar : UIState
    {
        // For this bar we'll be using a frame texture and then a gradient inside bar, as it's one of the more simpler approaches while still looking decent.
        // Once this is all set up make sure to go and do the required stuff for most UI's in the ModSystem class.

        // 对于这个条形图，我们将使用一个框架纹理和一个渐变内部条形图，因为它是更简单的方法之一，同时看起来还不错。
        // 设置完所有内容后，请确保在ModSystem类中执行大多数UI所需的操作。
        private UIText text;
        private UIElement area;
        private UIImage barFrame;
        private Color gradientA;
        private Color gradientB;

        public override void OnInitialize()
        {
            // Create a UIElement for all the elements to sit on top of, this simplifies the numbers as nested elements can be positioned relative to the top left corner of this element. 
            // UIElement is invisible and has no padding.

            // 为所有元素创建一个UIElement以放置其上方，这样可以简化数字，因为嵌套元素可以相对于该元素的左上角定位。
            // UIElement是不可见且没有填充的。
            area = new UIElement();
            area.Left.Set(-area.Width.Pixels - 600, 1f); // Place the resource bar to the left of the hearts.
                                                         // 将资源栏放置在心形图标左侧。

            area.Top.Set(30, 0f); // Placing it just a bit below the top of the screen.
                                  // 稍微向屏幕顶部下移一点位置。

            area.Width.Set(182, 0f); // We will be placing the following 2 UIElements within this 182x60 area.
                                     // 我们将把以下2个UIElements放置在此182x60区域内.

            area.Height.Set(60, 0f);

            barFrame = new UIImage(ModContent.Request<Texture2D>("AnalysisMod/AnalysisCommon/AnalysisUI/AnalysisResourceUI/AnalysisResourceFrame")); // Frame of our resource bar
                                                                                                                                                     // 资源栏框架
            barFrame.Left.Set(22, 0f);
            barFrame.Top.Set(0, 0f);
            barFrame.Width.Set(138, 0f);
            barFrame.Height.Set(34, 0f);

            text = new UIText("0/0", 0.8f); // text to show stat
                                            // 用于显示状态文本

            text.Width.Set(138, 0f);
            text.Height.Set(34, 0f);
            text.Top.Set(40, 0f);
            text.Left.Set(0, 0f);

            gradientA = new Color(123, 25, 138); // A dark purple
                                                 // 深紫色

            gradientB = new Color(187, 91, 201); // A light purple
                                                 // 浅紫色

            area.Append(text);
            area.Append(barFrame);
            Append(area);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // This prevents drawing unless we are using an AnalysisCustomResourceWeapon
            // 这样可以防止绘制除非正在使用AnalysisCustomResourceWeapon
            if (Main.LocalPlayer.HeldItem.ModItem is not AnalysisCustomResourceWeapon)
                return;

            base.Draw(spriteBatch);
        }

        // Here we draw our UI
        // 在这里绘制我们的UI
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            var modPlayer = Main.LocalPlayer.GetModPlayer<AnalysisResourcePlayer>();
            // Calculate quotient
            float quotient = (float)modPlayer.AnalysisResourceCurrent / modPlayer.AnalysisResourceMax2; // Creating a quotient that represents the difference of your currentResource vs your maximumResource, resulting in a float of 0-1f.
                                                                                                        // 创建商表示当前资源与最大资源之差，并得到0-1f范围内的浮点数。

            quotient = Utils.Clamp(quotient, 0f, 1f); // Clamping it to 0-1f so it doesn't go over that.
                                                      // 将其限制在0-1f范围内以避免超出该范围。

            // Here we get the screen dimensions of the barFrame element, then tweak the resulting rectangle to arrive at a rectangle within the barFrame texture that we will draw the gradient. These values were measured in a drawing program.
            // 在这里获取barFrame元素的屏幕尺寸，然后调整结果矩形以到达barFrame纹理内的矩形，我们将在其中绘制渐变。这些值是在绘图程序中测量的。
            Rectangle hitbox = barFrame.GetInnerDimensions().ToRectangle();
            hitbox.X += 12;
            hitbox.Width -= 24;
            hitbox.Y += 8;
            hitbox.Height -= 16;

            // Now, using this hitbox, we draw a gradient by drawing vertical lines while slowly interpolating between the 2 colors.
            // 现在，使用此命中框，在缓慢插值两种颜色之间时通过绘制垂直线来绘制渐变。
            int left = hitbox.Left;
            int right = hitbox.Right;
            int steps = (int)((right - left) * quotient);
            for (int i = 0; i < steps; i += 1)
            {
                // float percent = (float)i / steps; // Alternate Gradient Approach
                                                     // 备选渐变方法
                float percent = (float)i / (right - left);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(left + i, hitbox.Y, 1, hitbox.Height), Color.Lerp(gradientA, gradientB, percent));
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (Main.LocalPlayer.HeldItem.ModItem is not AnalysisCustomResourceWeapon)
                return;

            var modPlayer = Main.LocalPlayer.GetModPlayer<AnalysisResourcePlayer>();

            // Setting the text per tick to update and show our resource values.
            // 将每个时刻的文本设置为更新和显示我们的资源值。
            text.SetText($"Analysis Resource: {modPlayer.AnalysisResourceCurrent} / {modPlayer.AnalysisResourceMax2}");
            base.Update(gameTime);
        }
    }

    class AnalysisResourseUISystem : ModSystem
    {
        private UserInterface AnalysisResourceBarUserInterface;

        internal AnalysisResourceBar AnalysisResourceBar;

        public override void Load()
        {
            // All code below runs only if we're not loading on a server
            // 以下所有代码仅在我们不在服务器上加载时运行
            if (!Main.dedServ)
            {
                AnalysisResourceBar = new();
                AnalysisResourceBarUserInterface = new();
                AnalysisResourceBarUserInterface.SetState(AnalysisResourceBar);
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            AnalysisResourceBarUserInterface?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
            if (resourceBarIndex != -1)
            {
                layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
                    "AnalysisMod: Analysis Resource Bar",
                    delegate
                    {
                        AnalysisResourceBarUserInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }
}
