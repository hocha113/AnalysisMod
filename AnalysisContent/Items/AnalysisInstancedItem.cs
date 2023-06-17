using System.Collections.Generic;
using System.Linq;
using AnalysisMod.Staitd.ProjectE_Etr;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AnalysisMod.AnalysisContent.Items
{
    public class AnalysisInstancedItem : ModItem
    {
        public Color[] colors;

        string TextureString= "AnalysisMod/AnalysisContent/Items/AnalysisItem_1_0";

        public override void SetDefaults()
        {
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
        }

        public override ModItem Clone(Item item)
        {
            AnalysisInstancedItem clone = (AnalysisInstancedItem)base.Clone(item);//【执行类的克隆操作】

            clone.colors = (Color[])colors?.Clone(); // note the ? here is important, colors may be null if spawned from other mods which don't call OnCreate
                                                     // 请注意，这里的问号很重要，如果从其他未调用OnCreate方法的模组生成，则颜色可能为空。
            return clone;
        }

        public override void OnCreated(ItemCreationContext context)
        {
            GenerateNewColors();           
        }

        public override string Texture => TextureString;

        private void GenerateNewColors()
        {
            colors = new Color[5];
            for (int i = 0; i < 5; i++)
            {
                colors[i] = Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.7f);
            }           
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (colors == null) //colors may be null if spawned from other mods which don't call OnCreate
                                //如果从其他未调用OnCreate的模组生成，则颜色可能为空
                return;

            EtrSyter etrSyter = new EtrSyter();
            TooltipLine tooltipLine1 = new TooltipLine(Mod, "ETR: " + etrSyter.ETR, "") { };
            tooltips.Add(tooltipLine1);

            for (int i = 0; i < colors.Length; i++)
            {                
                TooltipLine tooltipLine = new TooltipLine(Mod, "EM" + i, "Analysis " + i) { OverrideColor = colors[i] };
                tooltips.Add(tooltipLine);                
            }
        }

        public override void UseAnimation(Player player)
        {
            if (colors == null)
            {
                GenerateNewColors();
            }
            else
            {
                // cycle through the colours
                // 循环遍历颜色
                colors = colors.Skip(1).Concat(colors.Take(1)).ToArray();
            }
        }

        // NOTE: The tag instance provided here is always empty by default.
        // Read https://github.com/tModLoader/tModLoader/wiki/Saving-and-loading-using-TagCompound to better understand Saving and Loading data.

        // 注意：此处提供的标签实例默认始终为空。
        // 请阅读 https://github.com/tModLoader/tModLoader/wiki/Saving-and-loading-using-TagCompound 以更好地理解数据保存和加载。
        public override void SaveData(TagCompound tag)
        {
            tag["Colors"] = colors;
        }

        public override void LoadData(TagCompound tag)
        {
            colors = tag.Get<Color[]>("Colors");
        }

        public override void AddRecipes() => CreateRecipe().AddIngredient<AnalysisItem>(10).Register();
    }
}
