using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Dyes;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items
{
    public class AnalysisHairDye : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Avoid loading assets on dedicated servers. They don't use graphics cards.
            // 避免在专用服务器上加载资源。它们不使用图形卡。
            if (!Main.dedServ)
            {
                // The following code creates a hair color-returning delegate (anonymous method), and associates it with this item's type Id.
                // 以下代码创建一个返回头发颜色的委托（匿名方法），并将其与此项的类型 Id 关联。
                GameShaders.Hair.BindShader(
                    Item.type,
                    new LegacyHairShaderData().UseLegacyMethod((Player player, Color newColor, ref bool lighting) => Main.DiscoColor) // Returning Main.DiscoColor will make our hair an animated rainbow. You can return any Color here.
                                                                                                                                      // 返回 Main.DiscoColor 将使我们的头发成为动态彩虹。您可以在此处返回任何颜色。
                );
            }

            Item.ResearchUnlockCount = 3;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 26;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item3;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useTurn = true;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.consumable = true;
        }
    }
}