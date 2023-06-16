using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace AnalysisMod.AnalysisCommon.AnalysisUI.AnalysisCoinsUI
{
    [Autoload(Side = ModSide.Client)] // This attribute makes this class only load on a particular side. Naturally this makes sense here since UI should only be a thing clientside. Be wary though that accessing this class serverside will error
                                      // 这个属性使得这个类只在特定的一侧加载。自然而然，这在此处是有意义的，因为 UI 应该只存在于客户端。但要注意，在服务器端访问此类将会出错。

    public class AnalysisCoinsUISystem : ModSystem
    {
        private UserInterface AnalysisCoinUserInterface;
        internal AnalysisCoinsUIState AnalysisCoinsUI;

        // These two methods will set the state of our custom UI, causing it to show or hide
        // 这两种方法将设置我们自定义 UI 的状态，从而显示或隐藏它
        public void ShowMyUI()
        {
            AnalysisCoinUserInterface?.SetState(AnalysisCoinsUI);
        }

        public void HideMyUI()
        {
            AnalysisCoinUserInterface?.SetState(null);
        }

        public override void Load()
        {
            // Create custom interface which can swap between different UIStates
            // 创建可以在不同 UI 状态之间切换的自定义接口
            AnalysisCoinUserInterface = new UserInterface();

            // Creating custom UIState
            // 创建自定义 UIState
            AnalysisCoinsUI = new AnalysisCoinsUIState();

            // Activate calls Initialize() on the UIState if not initialized, then calls OnActivate and then calls Activate on every child element
            // Activate 在未初始化时调用 UIState 的 Initialize() 方法，然后调用 OnActivate，并对每个子元素调用 Activate
            AnalysisCoinsUI.Activate();
        }

        public override void UpdateUI(GameTime gameTime)
        {
            // Here we call .Update on our custom UI and propagate it to its state and underlying elements
            // 在此处，我们调用 .Update 来更新我们的自定义 UI，并传播到其状态和底层元素上
            if (AnalysisCoinUserInterface?.CurrentState != null)
                AnalysisCoinUserInterface?.Update(gameTime);
        }

        // Adding a custom layer to the vanilla layer list that will call .Draw on your interface if it has a state
        // Setting the InterfaceScaleType to UI for appropriate UI scaling

        // 将一个自定义图层添加到原始图层列表中，如果具有状态，则会在其中调用 .Draw
        // 适当地将 InterfaceScaleType 设置为“UI”以进行适当的界面缩放
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "AnalysisMod: Coins Per Minute",
                    delegate
                    {
                        if (AnalysisCoinUserInterface?.CurrentState != null)
                            AnalysisCoinUserInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }
}
