using AnalysisMod.AnalysisCommon.Systems;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Biomes
{
    public class AnalysisUndergroundBiome : ModBiome
    {
        // Select all the scenery
        //选择所有的风景
        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.Find<ModUndergroundBackgroundStyle>("AnalysisMod/AnalysisUndergroundBackgroundStyle");

        // Select Music
        //选择音乐
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/MysteriousMystery");

        // Sets how the Scene Effect associated with this biome will be displayed with respect to vanilla Scene Effects. For more information see SceneEffectPriority & its values.
        //设置与此生物群系相关的场景效果如何显示，相对于基础场景效果。
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeLow; // We have set the SceneEffectPriority to be BiomeLow for purpose of Analysis, however default behavour is BiomeLow.
                                                                                      // 我们将SceneEffectPriority设置为BiomeLow以进行分析，但默认行为是BiomeLow。

        // Populate the Bestiary Filter
        //填充图鉴过滤器
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => base.BackgroundPath;
        public override Color? BackgroundColor => base.BackgroundColor;

        // Calculate when the biome is active.
        //计算生物群系活动时间。
        public override bool IsBiomeActive(Player player)
        {
            // Limit the biome height to be underground in either rock layer or dirt layer
            //将生物群系高度限制在岩层或泥土层下方地下。
            return (player.ZoneRockLayerHeight || player.ZoneDirtLayerHeight) &&
                // Check how many tiles of our biome are present, such that biome should be active
                //检查我们的生物群系有多少个瓷砖存在，使得该生物群系处于活动状态
                ModContent.GetInstance<AnalysisBiomeTileCount>().AnalysisBlockCount >= 40 &&
                // Limit our biome to be in only the horizontal center third of the world.
                //将我们的生物群系限制在世界水平中心三分之一内。
                Math.Abs(player.position.ToTileCoordinates().X - Main.maxTilesX / 2) < Main.maxTilesX / 6;
        }

        // In the event that both our biome AND one or more modded SceneEffect layers are active with the same SceneEffect Priority, this can decide which one.
        // It's uncommon that need to assign a weight - you'd have to specifically believe that you don't need higher SceneEffectPriority, but do need to be the active SceneEffect within the priority you designated
        // In this case, we don't need it, so this inclusion is purely to demonstrate this is available.
        // See the GetWeight documentation for more information.

        //如果我们的生物群系和一个或多个模组化场景效果层具有相同的SceneEffect Priority，则可以决定哪一个。
        //很少需要指定权重-您必须明确认为不需要更高的SceneEffectPriority，但确实需要成为您指定优先级内活动的Active SceneEffect
        //在这种情况下，我们不需要它，因此这个包含仅用于演示可用性。
        //有关详细信息，请参见GetWeight文档。
        /*
		public override float GetWeight(Player player) {
			int distanceToCenter = Math.Abs(player.position.ToTileCoordinates().X - Main.maxTilesX / 2);
			// We declare that our biome should have be more likely than not to be active if in center 1/6 of the world, and decreases in need to be active as player gets further away to the 1/3 mark.
            //声明我们的生态系统应该更可能处于活跃状态，在世界中心1/6时，并随着玩家远离1/3标记而减少。
			if (distanceToCenter <= Main.maxTilesX / 12) {
				return 1f;
			}
			else {
				return 1f - (distanceToCenter - Main.maxTilesX / 12) / (Main.maxTilesX / 12);
			}
		}
		*/
    }
}
