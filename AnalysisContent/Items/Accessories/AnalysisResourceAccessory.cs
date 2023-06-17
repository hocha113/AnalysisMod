using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AnalysisMod.AnalysisCommon.Players;
using Terraria.Localization;
using System;
using Microsoft.Xna.Framework;
using AnalysisMod.AnalysisContent.Dusts;

namespace AnalysisMod.AnalysisContent.Items.Accessories
{
    public class AnalysisResourceAccessory : ModItem
    {
        public static readonly int ResourceBoost = 100;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ResourceBoost);

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 32;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(gold: 5);
            Item.accessory = true;
            Item.rare = ItemRarityID.Red;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var modPlayer = player.GetModPlayer<AnalysisResourcePlayer>();
            modPlayer.AnalysisResourceMax2 += ResourceBoost; // add 100 to the AnalysisResourceMax2, which is our max for Analysis resource.
                                                             // 将AnalysisResourceMax2增加100，这是我们的分析资源上限。

            modPlayer.AnalysisResourceRegenRate *= 6f; // multiply our resource regeneration speed by 6.
                                                       // 将我们的资源再生速度乘以6。

            //【佩戴时生成的粒子】
            Random random = new Random();
            double rdVeto = random.Next(0, 360) * (Math.PI / 180);
            Vector2 DustVcr = Vector2.One.RotatedBy(rdVeto);
            Dust.NewDust(new Vector2(player.Center.X, player.Center.Y - 64 ), 3, 3, ModContent.DustType<Sparkle>(), DustVcr.X * 2.5f, DustVcr.Y * 2.5f, 155, Color.Gold, 1f);
        }

            //【我们让它在背包里也能发挥作用】
        public override void UpdateInventory(Player player)
        {
            var modPlayer = player.GetModPlayer<AnalysisResourcePlayer>();
            modPlayer.AnalysisResourceMax2 += ResourceBoost;
            modPlayer.AnalysisResourceRegenRate *= 3f;

            //【放置于背包中时生成的粒子】
            Random random = new Random();
            double rdVeto = random.Next(0,360)*(Math.PI/180);
            Vector2 DustVcr= Vector2.One.RotatedBy(rdVeto);
            Dust.NewDust(new Vector2( player.Center.X,player.Center.Y -64 ) , 3, 3, ModContent.DustType<Sparkle>(), DustVcr.X*2, DustVcr.Y*2, 155, Color.Red, 0.5f);
        }
    }
}
