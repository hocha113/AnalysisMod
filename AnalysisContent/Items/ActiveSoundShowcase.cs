using AnalysisMod.AnalysisContent.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items
{
    // This Analysis showcases how to loop and adjust sounds as they are playing. These are referred to as active sounds.
    // The weapon will shoot a projectile that will behave differently depending on how far away from the player the cursor is.
    // This allows the modder to experiment with each behavior independently to see how they work in game.

    // 这个分析展示了如何在播放声音时循环和调整它们。这些被称为活动声音。
    // 武器将发射一个抛射物，其行为取决于光标距离玩家的远近而有所不同。
    // 这使得修改者可以独立地实验每种行为，以查看它们在游戏中的表现。
    public class ActiveSoundShowcase : ModItem
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.TiedEighthNote}";

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shootSpeed = 4f;
            Item.shoot = ModContent.ProjectileType<Projectiles.ActiveSoundShowcaseProjectile>();
            Item.width = 22;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            Item.UseSound = SoundID.Item1;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.value = Item.buyPrice(0, 0, 20, 0);
            Item.rare = ItemRarityID.Blue;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int style = CalculateStyle(player);

            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, Main.myPlayer, ai0: style);
            return false;
        }

        public override void HoldItem(Player player)
        {
            int style = CalculateStyle(player);
            player.cursorItemIconText = $"  {(ActiveSoundShowcaseProjectile.ActiveSoundShowcaseStyle)style}";
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = Type;
        }

        private static int CalculateStyle(Player player)
        {
            Vector2 playerToMouse = Main.MouseScreen + Main.screenPosition - player.Center;
            float disatanceToScreenEdge = Math.Min(Main.screenHeight / 2, Main.screenWidth / 2) / Main.GameViewMatrix.Zoom.X;
            int style = Utils.Clamp((int)(playerToMouse.Length() * 6 / disatanceToScreenEdge), 0, 5);
            return style;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AnalysisItem>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
