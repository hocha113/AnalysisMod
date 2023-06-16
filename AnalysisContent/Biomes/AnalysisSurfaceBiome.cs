using AnalysisMod.AnalysisCommon.Systems;
using AnalysisMod.AnalysisContent.Items.Placeable;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Biomes
{
    // Shows setting up two basic biomes. For a more complicated Analysis, please request.
    //展示设置两个基本生物群系。如需更复杂的分析，请提出请求。
    public class AnalysisSurfaceBiome : ModBiome
    {
        // Select all the scenery
        //选择所有景观
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("AnalysisMod/AnalysisWaterStyle"); // Sets a water style for when inside this biome
                                                                                                                      // 设置在该生物群系内部的水样式

        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.Find<ModSurfaceBackgroundStyle>("AnalysisMod/AnalysisSurfaceBackgroundStyle");
        public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Crimson;

        // Select Music
        //选择音乐
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/MysteriousMystery");

        public override int BiomeTorchItemType => ModContent.ItemType<AnalysisTorch>();
        public override int BiomeCampfireItemType => ModContent.ItemType<AnalysisCampfire>();

        // Populate the Bestiary Filter
        //填充图鉴过滤器
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => base.BackgroundPath;
        public override Color? BackgroundColor => base.BackgroundColor;
        public override string MapBackground => BackgroundPath; // Re-uses Bestiary Background for Map Background
                                                                // 重用图鉴背景作为地图背景

        // Calculate when the biome is active.
        //计算生物群系活动时间。
        public override bool IsBiomeActive(Player player)
        {
            // First, we will use the AnalysisBlockCount from our added ModSystem for our first custom condition
            //首先，我们将使用添加的ModSystem中的AnalysisBlockCount作为第一个自定义条件。
            bool b1 = ModContent.GetInstance<AnalysisBiomeTileCount>().AnalysisBlockCount >= 40;

            // Second, we will limit this biome to the inner horizontal third of the map as our second custom condition
            //其次，我们将把这个生物群系限制在地图内部水平三分之一以内作为第二个自定义条件。
            bool b2 = Math.Abs(player.position.ToTileCoordinates().X - Main.maxTilesX / 2) < Main.maxTilesX / 6;

            // Finally, we will limit the height at which this biome can be active to above ground (ie sky and surface). Most (if not all) surface biomes will use this condition.
            //最后，我们将限制该生物群系可以活动的高度在地面以上（即天空和表面）。大多数（如果不是全部）表面生物群系都会使用此条件。
            bool b3 = player.ZoneSkyHeight || player.ZoneOverworldHeight;
            return b1 && b2 && b3;
        }
    }
}
