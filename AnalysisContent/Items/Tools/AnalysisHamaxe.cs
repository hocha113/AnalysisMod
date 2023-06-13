using AnalysisMod.AnalysisContent.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Tools
{
    public class AnalysisHamaxe : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 25;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true; // Automatically re-swing/re-use this item after its swinging animation is over.
                                   // 在武器挥动动画结束后自动重新挥舞/使用此物品。

            Item.axe = 30; // How much axe power the weapon has, note that the axe power displayed in-game is this value multiplied by 5
                           // 注意，游戏中显示的斧头威力是该值乘以5。

            Item.hammer = 100; // How much hammer power the weapon has
                               // 武器具有多少锤子威力。
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(10))
            { // This creates a 1/10 chance that a dust will spawn every frame that this item is in its 'Swinging' animation.
              // Creates a dust at the hitbox rectangle, following the rules of our 'if' conditional.

              // 这将创建一个1/10的几率，在此物品处于“摆动”动画时每帧生成一次尘埃。
              // 在命中框矩形处创建一个灰尘，遵循我们的“if”条件规则。
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, ModContent.DustType<Sparkle>());
            }
        }

        // Please see AnalysisContent/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AnalysisItem>()
                .AddTile<Tiles.Furniture.AnalysisWorkbench>()
                .Register();
        }
    }
}
