using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items
{
    public class AnalysisTooltipsItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(30, 4));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true; // Makes the item have an animation while in world (not held.). Use in combination with RegisterItemAnimation
                                                          // 使物品在世界中拥有动画效果（非手持状态）。与RegisterItemAnimation结合使用

            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.sellPrice(silver: 1);
            Item.rare = ItemRarityID.Blue;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            // 在此处添加一个工具提示行，稍后将被删除，展示如何从物品中移除工具提示
            var line = new TooltipLine(Mod, "Verbose:RemoveMe", "This tooltip won't show in-game");
            tooltips.Add(line);

            line = new TooltipLine(Mod, "Face", "I'm feeling just fine!")
            {
                OverrideColor = new Color(100, 100, 255)
            };
            tooltips.Add(line);

            // Here we give the item name a rainbow effect.
            // 在此处为物品名称添加彩虹效果。
            foreach (TooltipLine line2 in tooltips)
            {
                if (line2.Mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.OverrideColor = Main.DiscoColor;
                }
            }

            // Here we will remove all tooltips whose title end with ':RemoveMe'
            // One like that is added at the start of this method

            // 在这里我们将删除所有标题以“：RemoveMe”结尾的工具提示
            // 其中之一是在该方法的开头添加的
            tooltips.RemoveAll(l => l.Name.EndsWith(":RemoveMe"));

            // Another method of removal can be done if you know the index of the tooltip:
            // 如果您知道工具提示的索引，则可以执行另一种删除方法：

            // tooltips.RemoveAt(index);

            // You can also remove a specific line, if you have access to that object:
            // 如果您可以访问该对象，则还可以删除特定行：

            // tooltips.Remove(tooltipLine);
        }

        // Please see Content/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AnalysisItem>()
                .AddTile<Tiles.Furniture.AnalysisWorkbench>()
                .Register();
        }
    }
}