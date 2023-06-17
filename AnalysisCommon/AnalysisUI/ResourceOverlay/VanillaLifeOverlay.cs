using AnalysisMod.AnalysisCommon.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisCommon.AnalysisUI.ResourceOverlay
{
    public class VanillaLifeOverlay : ModResourceOverlay
    {
        // This field is used to cache vanilla assets used in the CompareAssets helper method further down in this file
        // 这个字段用于缓存在文件中进一步使用CompareAssets帮助方法所需的原始资产
        private Dictionary<string, Asset<Texture2D>> vanillaAssetCache = new();

        // These fields are used to cache the result of ModContent.Request<Texture2D>()
        // 这些字段用于缓存ModContent.Request<Texture2D>()的结果
        private Asset<Texture2D> heartTexture, fancyPanelTexture, barsFillingTexture, barsPanelTexture;

        public override void PostDrawResource(ResourceOverlayDrawContext context)
        {
            Asset<Texture2D> asset = context.texture;

            string fancyFolder = "Images/UI/PlayerResourceSets/FancyClassic/";
            string barsFolder = "Images/UI/PlayerResourceSets/HorizontalBars/";

            bool drawingBarsPanels = CompareAssets(asset, barsFolder + "HP_Panel_Middle");

            int AnalysisFruits = Main.LocalPlayer.GetModPlayer<AnalysisStatIncreasePlayer>().AnalysisLifeFruits;

            // Life resources are drawn over in groups of two
            // 生命资源以两组为单位绘制
            if (context.resourceNumber >= 2 * AnalysisFruits)
                return;

            // NOTE: CompareAssets is defined below this method's body
            // 注意：CompareAssets定义在此方法体下面
            if (asset == TextureAssets.Heart || asset == TextureAssets.Heart2)
            {
                // Draw over the Classic hearts
                // 绘制经典心形图案上方的内容
                DrawClassicFancyOverlay(context);
            }
            else if (CompareAssets(asset, fancyFolder + "Heart_Fill") || CompareAssets(asset, fancyFolder + "Heart_Fill_B"))
            {
                // Draw over the Fancy hearts
                // 绘制华精致形图案(1.4新生命心样式)上方的内容
                DrawClassicFancyOverlay(context);
            }
            else if (CompareAssets(asset, barsFolder + "HP_Fill") || CompareAssets(asset, barsFolder + "HP_Fill_Honey"))
            {
                // Draw over the Bars life bars
                // 绘制水平条生命栏上方的内容
                DrawBarsOverlay(context);
            }
            else if (CompareAssets(asset, fancyFolder + "Heart_Left") || CompareAssets(asset, fancyFolder + "Heart_Middle") || CompareAssets(asset, fancyFolder + "Heart_Right") || CompareAssets(asset, fancyFolder + "Heart_Right_Fancy") || CompareAssets(asset, fancyFolder + "Heart_Single_Fancy"))
            {
                // Draw over the Fancy heart panels
                // 绘制精致心形面板(1.4新生命条样式)上方的内容
                DrawFancyPanelOverlay(context);
            }
            else if (drawingBarsPanels)
            {
                // Draw over the Bars middle life panels
                // 在水平条生命栏中间绘制内容
                DrawBarsPanelOverlay(context);
            }
        }

        private bool CompareAssets(Asset<Texture2D> existingAsset, string compareAssetPath)
        {
            // This is a helper method for checking if a certain vanilla asset was drawn
            //这是一个检查是否已绘制某个原始资产的辅助方法。
            if (!vanillaAssetCache.TryGetValue(compareAssetPath, out var asset))
                asset = vanillaAssetCache[compareAssetPath] = Main.Assets.Request<Texture2D>(compareAssetPath);

            return existingAsset == asset;
        }

        private void DrawClassicFancyOverlay(ResourceOverlayDrawContext context)
        {
            // Draw over the Classic / Fancy hearts
            // "context" contains information used to draw the resource
            // If you want to draw directly on top of the vanilla hearts, just replace the texture and have the context draw the new texture

            // 在经典/精致心形图案之上进行绘画
            //"context"包含用于绘画资源所需信息。
            //如果要直接在香草心脏顶部进行绘画，请替换纹理并让context将新纹理进行绘画。
            context.texture = heartTexture ??= ModContent.Request<Texture2D>("AnalysisMod/AnalysisCommon/AnalysisUI/ResourceOverlay/ClassicLifeOverlay");
            context.Draw();
        }

        // Drawing over the panel backgrounds is not required.
        // This Analysis just showcases changing the "inner" part of the heart panels to more closely resemble the Analysis life fruit.

        // 不需要覆盖面板背景。
        // 此分析仅展示如何更改“内部”部分，使其更类似于Analysis生命果实。
        private void DrawFancyPanelOverlay(ResourceOverlayDrawContext context)
        {
            // Draw over the Fancy heart panels
            // 绘制精致心形面板
            string fancyFolder = "Images/UI/PlayerResourceSets/FancyClassic/";

            // The original position refers to the entire panel slice.
            // However, since this overlay only modifies the "inner" portion of the slice (aka the part behind the heart),
            // the position should be modified to compensate for the sprite size difference

            // 在精致心形面板之上进行描画
            // 原始位置指整个面板切片。
            //但是，由于此叠加层仅修改了切片（即位于心脏后面的部分） 的“内部”部分，
            Vector2 positionOffset;

            if (context.resourceNumber == context.snapshot.AmountOfLifeHearts)
            {
                // Final panel to draw has a special "Fancy" variant.  Determine whether it has panels to the left of it
                // 最终面板有一个特殊的“精致”变体。 确定其左侧是否有面板
                if (CompareAssets(context.texture, fancyFolder + "Heart_Single_Fancy"))
                {
                    // First and only panel in this panel's row
                    // 这个面板是该行中的第一个，也是唯一的
                    positionOffset = new Vector2(8, 8);
                }
                else
                {
                    // Other panels existed in this panel's row
                    // Vanilla texture is "Heart_Right_Fancy"

                    // 在此面板的行中存在其他面板
                    // 香草材质为“Heart_Right_Fancy”
                    positionOffset = new Vector2(8, 8);
                }
            }
            else if (CompareAssets(context.texture, fancyFolder + "Heart_Left"))
            {
                // First panel in this row
                // 此行中的第一个面板
                positionOffset = new Vector2(4, 4);
            }
            else if (CompareAssets(context.texture, fancyFolder + "Heart_Middle"))
            {
                // Any panel that has a panel to its left AND right
                // 任何左右两侧都有面板的面板
                positionOffset = new Vector2(0, 4);
            }
            else
            {
                // Final panel in the first row
                // Vanilla texture is "Heart_Right"

                // 第一行最后一个面板
                // 香草材质为“Heart_Right”
                positionOffset = new Vector2(0, 4);
            }

            // "context" contains information used to draw the resource
            // If you want to draw directly on top of the vanilla hearts, just replace the texture and have the context draw the new texture

            // “context”包含用于绘制资源的信息
            // 如果要直接在香草心形图案上绘制，请替换纹理并让上下文绘制新纹理
            context.texture = fancyPanelTexture ??= ModContent.Request<Texture2D>("AnalysisMod/AnalysisCommon/AnalysisUI/ResourceOverlay/FancyLifeOverlay_Panel");

            // Due to the replacement texture and the vanilla texture having different dimensions, the source needs to also be modified
            // 由于替换纹理和香草纹理具有不同的尺寸，因此还需要修改源。
            context.source = context.texture.Frame();
            context.position += positionOffset;
            context.Draw();
        }

        private void DrawBarsOverlay(ResourceOverlayDrawContext context)
        {
            // Draw over the Bars life bars
            // "context" contains information used to draw the resource
            // If you want to draw directly on top of the vanilla bars, just replace the texture and have the context draw the new texture

            // 覆盖 Bars 生命条
            // “context”包含用于绘制资源的信息。
            // 如果要直接在香草栏上方进行绘制，请替换纹理并让上下文绘制新纹理。
            context.texture = barsFillingTexture ??= ModContent.Request<Texture2D>("AnalysisMod/AnalysisCommon/AnalysisUI/ResourceOverlay/BarsLifeOverlay_Fill");
            context.Draw();
        }

        // Drawing over the panel backgrounds is not required.
        // This Analysis just showcases changing the "inner" part of the bar panels to more closely resemble the Analysis life fruit.

        // 不需要覆盖面板背景。
        // 这个分析只展示了如何更改栏内部分以更接近 Analysis 生命果实。
        private void DrawBarsPanelOverlay(ResourceOverlayDrawContext context)
        {
            // Draw over the Bars middle life panels
            // "context" contains information used to draw the resource
            // If you want to draw directly on top of the vanilla bar panels, just replace the texture and have the context draw the new texture

            // 覆盖 Bars 中间生命栏
            // “context”包含用于绘制资源的信息。
            // 如果要直接在香草栏版块顶部进行绘画，请替换纹理并让上下文画出新图案。
            context.texture = barsPanelTexture ??= ModContent.Request<Texture2D>("AnalysisMod/AnalysisCommon/AnalysisUI/ResourceOverlay/BarsLifeOverlay_Panel");

            // Due to the replacement texture and the vanilla texture having different heights, the source needs to also be modified
            // 由于替换纹理和香草材质具有不同的高度，因此还需要修改源。
            context.source = context.texture.Frame();
            // The original position refers to the entire panel slice.
            // However, since this overlay only modifies the "inner" portion of the slice (aka the part behind the bar filling),
            // the position should be modified to compensate for the sprite size difference

            // 原始位置是指整个面板切片。
            // 但是，由于此覆盖层仅修改了切片的“内部”部分（即在栏填充后面的部分），
            // 因此应该修改位置以补偿精灵大小差异
            context.position.Y += 6;
            context.Draw();
        }
    }
}
