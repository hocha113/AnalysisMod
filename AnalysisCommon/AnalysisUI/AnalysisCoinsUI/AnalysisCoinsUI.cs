using AnalysisMod.AnalysisCommon.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace AnalysisMod.AnalysisCommon.AnalysisUI.AnalysisCoinsUI
{
    // AnalysisUIs visibility is toggled by typing "/Analysis_coins" in chat (See AnalysisCoinCommand.cs)
    // AnalysisCoinsUI is a simple UI Analysis showing how to use UIPanel, UIImageButton, and even a custom UIElement
    // For more info about UI you can check https://github.com/tModLoader/tModLoader/wiki/Basic-UI-Element and https://github.com/tModLoader/tModLoader/wiki/Advanced-guide-to-custom-UI 

    // 通过在聊天中输入“/Anslysis_coins”来切换AnalysisUI的可见性（请参阅AnalysisCoinCommand.cs）
    // AnalysisCoinsUI是一个简单的UI分析，展示了如何使用UIPanel、UIImageButton甚至自定义的UIElement
    // 关于UI更多信息，请查看https://github.com/tModLoader/tModLoader/wiki/Basic-UI-Element和https://github.com/tModLoader/tModLoader/wiki/Advanced-guide-to-custom-UI
    internal class AnalysisCoinsUIState : UIState
    {
        public AnalysisDragableUIPanel CoinCounterPanel;
        public UIMoneyDisplay MoneyDisplay;

        // In OnInitialize, we place various UIElements onto our UIState (this class).
        // UIState classes have width and height equal to the full screen, because of this, usually we first define a UIElement that will act as the container for our UI.
        // We then place various other UIElement onto that container UIElement positioned relative to the container UIElement.

        // 在OnInitialize中，我们将各种 UIElements 放置到我们的 UIState（这个类）上。
        // UIState 类具有与全屏相同的宽度和高度，因此通常我们首先定义一个作为容器用于放置其他 UIElement 的 UIElement。
        // 然后将各种其他 UIElement 放置到该容器 UIElement 上，并相对于该容器定位。
        public override void OnInitialize()
        {
            // Here we define our container UIElement. In DragableUIPanel.cs, you can see that DragableUIPanel is a UIPanel with a couple added features.
            // 这里我们定义了 container UIElement。在 DragableUIPanel.cs 中，您可以看到 DragableUIPanel 是带有一些附加功能的 UIPanel。
            CoinCounterPanel = new AnalysisDragableUIPanel();
            CoinCounterPanel.SetPadding(0);

            // We need to place this UIElement in relation to its Parent. Later we will be calling `base.Append(coinCounterPanel);`. 
            // This means that this class, AnalysisCoinsUI, will be our Parent. Since AnalysisCoinsUI is a UIState, the Left and Top are relative to the top left of the screen.
            // SetRectangle method help us to set the position and size of UIElement

            // 我们需要根据其父级放置此 UIElement。稍后我们将调用 `base.Append(coinCounterPanel);`。
            // 这意味着这个类 AnalysisCoinsUI 将是我们的 Parent。由于 AnalysisCoinsUI 是一个 UIState，所以 Left 和 Top 相对于屏幕左上角。
            // SetRectangle 方法帮助我们设置了位置和大小。
            SetRectangle(CoinCounterPanel, left: 400f, top: 100f, width: 170f, height: 70f);
            CoinCounterPanel.BackgroundColor = new Color(73, 94, 171);

            // Next, we create another UIElement that we will place. Since we will be calling `coinCounterPanel.Append(playButton);`, Left and Top are relative to the top left of the coinCounterPanel UIElement. 
            // By properly nesting UIElements, we can position things relatively to each other easily.

            // 接下来，创建另一个要放置的元素。由于我们将调用 `coinCounterPanel.Append(playButton);`，所以 Left 和 Top 相对于 coinCounterPanel UIElement 的左上角。
            // 通过正确嵌套 UIElements，我们可以轻松地相对定位。
            Asset<Texture2D> buttonPlayTexture = ModContent.Request<Texture2D>("Terraria/Images/UI/ButtonPlay");
            AnalysisUIHoverImageButton playButton = new AnalysisUIHoverImageButton(buttonPlayTexture, "Reset Coins Per Minute Counter");
            SetRectangle(playButton, left: 110f, top: 10f, width: 22f, height: 22f);

            // UIHoverImageButton doesn't do anything when Clicked. Here we assign a method that we'd like to be called when the button is clicked.
            // 当单击按钮时，UIHoverImageButton 不执行任何操作。在这里分配了一个方法，在单击按钮时会调用该方法。
            playButton.OnLeftClick += new MouseEvent(PlayButtonClicked);
            CoinCounterPanel.Append(playButton);

            Asset<Texture2D> buttonDeleteTexture = ModContent.Request<Texture2D>("Terraria/Images/UI/ButtonDelete");
            AnalysisUIHoverImageButton closeButton = new AnalysisUIHoverImageButton(buttonDeleteTexture, Language.GetTextValue("LegacyInterface.52")); // Localized text for "Close"
                                                                                                                                                       // “关闭”的本地化文本
            SetRectangle(closeButton, left: 140f, top: 10f, width: 22f, height: 22f);
            closeButton.OnLeftClick += new MouseEvent(CloseButtonClicked);
            CoinCounterPanel.Append(closeButton);

            // UIMoneyDisplay is a fairly complicated custom UIElement. UIMoneyDisplay handles drawing some text and coin textures.
            // Organization is key to managing UI design. Making a contained UIElement like UIMoneyDisplay will make many things easier.

            // UIMoneyDisplay 是一个相当复杂的自定义 UIElement。UIMoneyDisplay 处理绘制一些文本和硬币纹理。
            // 组织是管理 UI 设计的关键。创建像 UIMoneyDisplay 这样的包含元素将使许多事情变得更加容易。
            MoneyDisplay = new UIMoneyDisplay();
            SetRectangle(MoneyDisplay, 15f, 20f, 100f, 40f);
            CoinCounterPanel.Append(MoneyDisplay);

            Append(CoinCounterPanel);

            // As a recap, AnalysisCoinsUI is a UIState, meaning it covers the whole screen. We attach coinCounterPanel to AnalysisCoinsUI some distance from the top left corner.
            // We then place playButton, closeButton, and moneyDiplay onto coinCounterPanel so we can easily place these UIElements relative to coinCounterPanel.
            // Since coinCounterPanel will move, this proper organization will move playButton, closeButton, and moneyDiplay properly when coinCounterPanel moves.

            // 总之，AnalysisCoinsUI 是一个 UIState，意味着它覆盖整个屏幕。我们将 coinCounterPanel 放置到 AnalysisCoinsUI 上，并与屏幕左上角保持一定距离。
            // 然后将 playButton、closeButton 和 moneyDiplay 放置到 coinCounterPanel 上，以便可以轻松地相对于 coinCounterPanel 定位这些 UIElements。
            // 由于 coinCounterPanel 将移动，因此适当组织将在移动 coinCounterPanel 时正确移动 playButton、closeButton 和 moneyDiplay。
        }

        private void SetRectangle(UIElement uiElement, float left, float top, float width, float height)
        {
            uiElement.Left.Set(left, 0f);
            uiElement.Top.Set(top, 0f);
            uiElement.Width.Set(width, 0f);
            uiElement.Height.Set(height, 0f);
        }

        private void PlayButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuOpen);
            MoneyDisplay.ResetCoins();
        }

        private void CloseButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuClose);
            ModContent.GetInstance<AnalysisCoinsUISystem>().HideMyUI();
        }

        public void UpdateValue(int pickedUp)
        {
            MoneyDisplay.AddCoinsPerMinute(pickedUp);
        }
    }

    public class UIMoneyDisplay : UIElement
    {
        // How many coins have been collected in copper
        // 已收集的铜币数量
        public long collectedCoins;

        // Time from start(or reset) to calculate how many coins collected per minute
        // 从开始（或重置）计算每分钟收集的硬币数所用的时间
        private DateTime? startTime;

        // Saving coin textures to an array to make them easier to access
        // 将硬币纹理保存到数组中，以便更轻松地访问它们
        private readonly Texture2D[] coinsTextures = new Texture2D[4];

        public UIMoneyDisplay()
        {
            startTime = null;

            for (int j = 0; j < 4; j++)
            {
                // Textures may not be loaded without it
                // 如果没有加载纹理，则可能无法显示它们
                Main.instance.LoadItem(74 - j);
                coinsTextures[j] = TextureAssets.Item[74 - j].Value;
            }
        }
        public void AddCoinsPerMinute(int coins)
        {
            collectedCoins += coins;

            // We begin to remember the time only after at least one coin has been collected
            // 只有在至少收集了一枚硬币后才开始记时
            if (startTime == null)
                startTime = DateTime.Now;
        }

        public int GetCoinsPerMinute()
        {
            if (collectedCoins == 0)
                return 0;

            // If the time has passed less than minutes, the current number of coins will be displayed
            // 如果经过的时间不足一分钟，则将显示当前硬币数量
            return (int)(collectedCoins / Math.Max(1, (DateTime.Now - startTime.Value).TotalMinutes));
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle innerDimensions = GetInnerDimensions();

            // Getting top left position of this UIElement
            // 获取此UI元素左上角位置
            float shopx = innerDimensions.X;
            float shopy = innerDimensions.Y;

            // Drawing first line of coins (current collected coins)
            // CoinsSplit converts the number of copper coins into an array of all types of coins

            // 绘制第一行硬币（当前已收集的）
            // CoinsSplit将铜币数量转换为所有类型硬币的数组
            DrawCoins(spriteBatch, shopx, shopy, Utils.CoinsSplit(collectedCoins));

            // Drawing second line of coins (coins per minute) and text "CPM"
            // 绘制第二行硬币（每分钟收集量）和文本“CPM”
            DrawCoins(spriteBatch, shopx, shopy, Utils.CoinsSplit(GetCoinsPerMinute()), 0, 25);
            Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.ItemStack.Value, "CPM", shopx + 24 * 4, shopy + 25f, Color.White, Color.Black, new Vector2(0.3f), 0.75f);
        }

        private void DrawCoins(SpriteBatch spriteBatch, float shopx, float shopy, int[] coinsArray, int xOffset = 0, int yOffset = 0)
        {
            for (int j = 0; j < 4; j++)
            {
                spriteBatch.Draw(coinsTextures[j], new Vector2(shopx + 11f + 24 * j + xOffset, shopy + yOffset), null, Color.White, 0f, coinsTextures[j].Size() / 2f, 1f, SpriteEffects.None, 0f);
                Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.ItemStack.Value, coinsArray[3 - j].ToString(), shopx + 24 * j + xOffset, shopy + yOffset, Color.White, Color.Black, new Vector2(0.3f), 0.75f);
            }
        }

        public void ResetCoins()
        {
            collectedCoins = 0;
            startTime = DateTime.Now;
        }
    }

    public class MoneyCounterGlobalItem : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstantiation)
        {
            return item.type >= ItemID.CopperCoin && item.type <= ItemID.PlatinumCoin;
        }

        public override bool OnPickup(Item item, Player player)
        {
            // If we have picked up coins of any type, then we will update the values in AnalysisCoinsUI
            // 如果我们拾取了任何类型的硬币，则会更新AnalysisCoinsUI中的值
            ModContent.GetInstance<AnalysisCoinsUISystem>().AnalysisCoinsUI.UpdateValue(item.stack * (item.value / 5));
            return base.OnPickup(item, player);
        }
    }
}
