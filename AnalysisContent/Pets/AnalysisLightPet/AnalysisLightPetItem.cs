using AnalysisMod.AnalysisContent.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Pets.AnalysisLightPet
{
    public class AnalysisLightPetItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 0;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shoot = ModContent.ProjectileType<AnalysisLightPetProjectile>();
            Item.width = 16;
            Item.height = 30;
            Item.UseSound = SoundID.Item2;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.rare = ItemRarityID.Yellow;
            Item.noMelee = true;
            Item.value = Item.sellPrice(0, 5, 50);
            Item.buffType = ModContent.BuffType<AnalysisLightPetBuff>();
        }

        // Please see Content/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        //public override void AddRecipes()
        //{
        //    CreateRecipe()
        //        .AddIngredient<AnalysisItem>()
        //        .AddTile<Tiles.Furniture.AnalysisWorkbench>()
        //        .Register();
        //}

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 3600);
            }
        }
    }
}