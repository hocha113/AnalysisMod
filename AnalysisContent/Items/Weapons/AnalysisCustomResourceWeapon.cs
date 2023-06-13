using System.Collections.Generic;
using AnalysisMod.AnalysisCommon.Players;
using AnalysisMod.AnalysisContent.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Weapons
{
    // Holding this item will cause the AnalysisResourceBar UI to show, displaying the player's custom resource amounts tracked in AnalysisResourcePlayer.
    // 拿起这个物品会导致 AnalysisResourceBar UI 显示，显示玩家在 AnalysisResourcePlayer 中跟踪的自定义资源数量。
    public class AnalysisCustomResourceWeapon : ModItem
    {
        private int AnalysisResourceCost; // Add our custom resource cost
                                          // 添加我们的自定义资源成本

        public override void SetDefaults()
        {
            Item.damage = 130;
            Item.DamageType = DamageClass.Magic;
            Item.width = 38;
            Item.height = 38;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6;
            Item.value = Item.buyPrice(gold: 15);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item71;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.VortexBeaterRocket;
            Item.shootSpeed = 7;
            Item.crit = 32;
            AnalysisResourceCost = 5; // Set our custom resource cost to 5
                                      // 将我们的自定义资源成本设置为 5
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "Analysis Resource Cost", $"Uses {AnalysisResourceCost} Analysis resource"));
        }

        // Make sure you can't use the item if you don't have enough resource
        // 确保如果你没有足够的资源，就不能使用该物品
        public override bool CanUseItem(Player player)
        {
            var AnalysisResourcePlayer = player.GetModPlayer<AnalysisResourcePlayer>();

            if (AnalysisResourcePlayer.AnalysisResourceCurrent >= AnalysisResourceCost)
            {
                AnalysisResourcePlayer.AnalysisResourceCurrent -= AnalysisResourceCost;
                return true;
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AnalysisItem>(10)
                .AddTile<AnalysisWorkbench>()
                .Register();
        }
    }
}
