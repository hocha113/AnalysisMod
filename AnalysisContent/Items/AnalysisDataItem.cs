﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items
{
    public class AnalysisDataItem : ModItem
    {
        public override string Texture => "AnalysisMod/AnalysisContent/Items/AnalysisItem";

        public int timer;

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tooltip = new TooltipLine(Mod, "AnalysisMod: HotPatato", $"You have {timer / 60f:N1} seconds left!") { OverrideColor = Color.Red };
            tooltips.Add(tooltip);
        }

        public override void UpdateInventory(Player player)
        {
            if (--timer <= 0)
            {
                player.statLife += 100;
                if (player.statLife > player.statLifeMax2) player.statLife = player.statLifeMax2;
                player.HealEffect(100);
                Item.TurnToAir();
            }
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient<AnalysisItem>(100);
            ((AnalysisDataItem)recipe.createItem.ModItem).timer = 300;
            recipe.Register();
        }
    }
}