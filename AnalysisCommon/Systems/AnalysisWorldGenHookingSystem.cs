using MonoMod.Cil;
using System;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace AnalysisMod.AnalysisCommon.Systems
{
    // 这个ModSystem将演示如何编辑和重定向世界生成过程
    // 由于世界生成过程是匿名方法（它们没有名称），因此无法使用标准方式进行编辑（使用IL_xx或On_xx）
    public class AnalysisWorldGenHookingSystem : ModSystem
    {
        // 所有注册都应在加载时进行
        // 生成通行证挂钩需要手动卸载，因此不需要Unload方法
        public override void Load()
        {
            // 编辑金字塔通行证的IL(【决定你的世界是否生成金字塔-译者注】)
            WorldGen.ModifyPass((PassLegacy)WorldGen.VanillaGenPasses["Pyramids"], Modify_Pyramids);

            // 重定向闪亮的通行证（生成矿物）
            WorldGen.DetourPass((PassLegacy)WorldGen.VanillaGenPasses["Shinies"], Detour_Shinies);
        }

        void Modify_Pyramids(ILContext il)
        {
            try
            {
                var c = new ILCursor(il);
                c.EmitDelegate(() => ModContent.GetInstance<AnalysisMod>().Logger.Debug("(In ILHook) Generating Pyramids"));
            }
            catch (Exception)
            {
                MonoModHooks.DumpIL(ModContent.GetInstance<AnalysisMod>(), il);
            }
        }

        // 重定向应该相同（除了下面提到的一件事），这只是一个分析，以便您可以检查是否实际起作用
        // 需要注意的一件事是出于技术原因，self参数是对象类型
        // 您永远不需要将其强制转换为WorldGen类型，因为它不包含任何实例字段或方法
        void Detour_Shinies(WorldGen.orig_GenPassDetour orig, object self, GenerationProgress progress, GameConfiguration configuration)
        {
            ModContent.GetInstance<AnalysisMod>().Logger.Debug("(On Hook) Before Shinies");
            orig(self, progress, configuration);
            ModContent.GetInstance<AnalysisMod>().Logger.Debug("(On Hook) After Shinies");
        }
    }
}
