using AnalysisMod.AnalysisCommon.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.ResourceSets;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisCommon.AnalysisUI.ResourceOverlay
{
    public class VanillaManaOverlay : ModResourceOverlay
    {
        // This field is used to cache vanilla assets used in the CompareAssets helper method further down in this file
        // 此字段用于缓存在此文件中的CompareAssets帮助方法中使用的原始资产
        private Dictionary<string, Asset<Texture2D>> vanillaAssetCache = new();

        // These fields are used to cache the result of ModContent.Request<Texture2D>()
        // 这些字段用于缓存ModContent.Request<Texture2D>()的结果
        private Asset<Texture2D> starTexture, fancyPanelTexture, barsFillingTexture, barsPanelTexture;

        // Unlike VanillaLifeOverlay, every star is drawn over by this hook.
        // 与VanillaLifeOverlay不同，每个魔力星都会被这个钩子覆盖。
        public override void PostDrawResource(ResourceOverlayDrawContext context)
        {
            Asset<Texture2D> asset = context.texture;

            string fancyFolder = "Images/UI/PlayerResourceSets/FancyClassic/";
            string barsFolder = "Images/UI/PlayerResourceSets/HorizontalBars/";

            if (Main.LocalPlayer.GetModPlayer<AnalysisStatIncreasePlayer>().AnalysisManaCrystals <= 0)
                return;

            // NOTE: CompareAssets is defined below this method's body
            // 注意：CompareAssets在该方法体下面定义
            if (asset == TextureAssets.Mana)
            {
                // Draw over the Classic stars
                // 覆盖经典魔力星
                DrawClassicFancyOverlay(context);
            }
            else if (CompareAssets(asset, fancyFolder + "Star_Fill"))
            {
                // Draw over the Fancy stars
                // 覆盖花式魔力星
                DrawClassicFancyOverlay(context);
            }
            else if (CompareAssets(asset, barsFolder + "MP_Fill"))
            {
                // Draw over the Bars mana bars
                // 覆盖魔力条
                DrawBarsOverlay(context);
            }
            else if (CompareAssets(asset, fancyFolder + "Star_A") || CompareAssets(asset, fancyFolder + "Star_B") || CompareAssets(asset, fancyFolder + "Star_C") || CompareAssets(asset, fancyFolder + "Star_Single"))
            {
                // Draw over the Fancy star panels
                // 覆盖花式魔力星面板
                DrawFancyPanelOverlay(context);
            }
            else if (CompareAssets(asset, barsFolder + "MP_Panel_Middle"))
            {
                // Draw over the Bars middle mana panels
                // 在魔法面板的中间绘制
                DrawBarsPanelOverlay(context);
            }
        }

        private bool CompareAssets(Asset<Texture2D> existingAsset, string compareAssetPath)
        {
            // This is a helper method for checking if a certain vanilla asset was drawn
            // 这是一个检查是否绘制了某个原始资产的辅助方法
            if (!vanillaAssetCache.TryGetValue(compareAssetPath, out var asset))
                asset = vanillaAssetCache[compareAssetPath] = Main.Assets.Request<Texture2D>(compareAssetPath);

            return existingAsset == asset;
        }

        private void DrawClassicFancyOverlay(ResourceOverlayDrawContext context)
        {
            // Draw over the Classic / Mana stars
            // "context" contains information used to draw the resource
            // If you want to draw directly on top of the vanilla stars, just replace the texture and have the context draw the new texture

            // 绘制经典/Mana stars
            //"context"包含用于绘制资源的信息。
            //如果要直接在香草明星上方绘制，请替换纹理并使上下文绘制新纹理。
            context.texture = starTexture ??= ModContent.Request<Texture2D>("AnalysisMod/AnalysisCommon/AnalysisUI/ResourceOverlay/ClassicManaOverlay");
            context.Draw();
        }

        // Drawing over the panel backgrounds is not required.
        // This Analysis just showcases changing the "inner" part of the star panels to more closely resemble the Analysis life fruit.

        // 不需要覆盖面板背景。
        // 此分析仅展示将“内部”部分更改为更接近Analysis生命果实的明星面板。
        private void DrawFancyPanelOverlay(ResourceOverlayDrawContext context)
        {
            // Draw over the Fancy star panels
            // 覆盖花式魔力星面板
            string fancyFolder = "Images/UI/PlayerResourceSets/FancyClassic/";

            // The original position refers to the entire panel slice.
            // However, since this overlay only modifies the "inner" portion of the slice (aka the part behind the star),
            // the position should be modified to compensate for the sprite size difference

            // 原始位置指整个窗格切片。
            // 但是，由于此叠加层仅修改切片的“内部”部分（即魔力星后面），
            // 因此应修改位置以补偿精灵大小差异
            Vector2 positionOffset;

            if (context.resourceNumber == context.snapshot.AmountOfManaStars)
            {
                // Final panel in the column.  Determine whether it has panels above it
                // 列中的最后一个面板。确定它上面是否有面板
                if (CompareAssets(context.texture, fancyFolder + "Star_Single"))
                {
                    // First and only panel
                    // 第一个也是唯一的面板
                    positionOffset = new Vector2(4, 4);
                }
                else
                {
                    // Other panels existed above this panel
                    // Vanilla texture is "Star_C"

                    // 此面板上方和下方都存在其他面板
                    // 香草纹理为“Star_C”
                    positionOffset = new Vector2(4, 0);
                }
            }
            else if (CompareAssets(context.texture, fancyFolder + "Star_A"))
            {
                // First panel in the column
                // 列中的第一个窗格
                positionOffset = new Vector2(4, 4);
            }
            else
            {
                // Any panel that has a panel above AND below it
                // Vanilla texture is "Star_B"

                // 任何具有上方和下方窗格的窗格
                //香草纹理为“Star_B”
                positionOffset = new Vector2(4, 0);
            }

            // "context" contains information used to draw the resource
            // If you want to draw directly on top of the vanilla stars, just replace the texture and have the context draw the new texture

            // “context”包含用于绘制资源的信息。
            // 如果要直接在vanilla bars之上绘制，请替换纹理并使上下文绘制新纹理。
            context.texture = fancyPanelTexture ??= ModContent.Request<Texture2D>("AnalysisMod/AnalysisCommon/AnalysisUI/ResourceOverlay/FancyManaOverlay_Panel");

            // Due to the replacement texture and the vanilla texture having different dimensions, the source needs to also be modified
            // 由于替换纹理和原版纹理具有不同的尺寸，源代码也需要进行修改。
            context.source = context.texture.Frame();
            context.position += positionOffset;
            context.Draw();
        }

        private void DrawBarsOverlay(ResourceOverlayDrawContext context)
        {
            // Draw over the Bars mana bars
            // "context" contains information used to draw the resource
            // If you want to draw directly on top of the vanilla bars, just replace the texture and have the context draw the new texture

            // 在魔力条上绘制法力条
            // “context”包含用于绘制资源的信息
            // 如果要直接在原始条上方绘制，请替换纹理并让上下文绘制新纹理
            context.texture = barsFillingTexture ??= ModContent.Request<Texture2D>("AnalysisMod/AnalysisCommon/AnalysisUI/ResourceOverlay/BarsManaOverlay_Fill");
            context.Draw();
        }

        // Drawing over the panel backgrounds is not required.
        // This Analysis just showcases changing the "inner" part of the bar panels to more closely resemble the Analysis life fruit.

        // 不需要在面板背景上进行绘图。
        // 此分析仅展示将“内部”栏面更改为更类似于分析生命果实的方式。
        private void DrawBarsPanelOverlay(ResourceOverlayDrawContext context)
        {
            // Draw over the Bars middle life panels
            // "context" contains information used to draw the resource
            // If you want to draw directly on top of the vanilla bar panels, just replace the texture and have the context draw the new texture

            // 在中间生命栏面上画画
            // “context”包含用于绘制资源的信息
            // 如果要直接在原始栏面顶部进行绘图，请替换纹理并让上下文绘制新纹理
            context.texture = barsPanelTexture ??= ModContent.Request<Texture2D>("AnalysisMod/AnalysisCommon/AnalysisUI/ResourceOverlay/BarsManaOverlay_Panel");

            // Due to the replacement texture and the vanilla texture having different heights, the source needs to also be modified
            // 由于替换纹理和原版纹理具有不同的高度，源代码也需要进行修改
            context.source = context.texture.Frame();

            // The original position refers to the entire panel slice.
            // However, since this overlay only modifies the "inner" portion of the slice (aka the part behind the bar filling),
            // the position should be modified to compensate for the sprite size difference

            // 原始位置是指整个面板切片。
            // 然而，由于此覆盖层仅修改了切片“内部”的一部分（即填充栏后面的部分），
            // 因此应该调整位置以补偿精灵大小差异。
            context.position.Y += 6;
            context.Draw();
        }
    }
}
