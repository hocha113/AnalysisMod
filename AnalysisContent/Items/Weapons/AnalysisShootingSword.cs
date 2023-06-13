using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Weapons
{
    /// <summary>
    ///     Star Wrath/Starfury style weapon. Spawn projectiles from sky that aim towards mouse.
    ///     See Source code for Star Wrath projectile to see how it passes through tiles.
    ///     For a detailed sword guide see <see cref="AnalysisTwoSword" /><br/>
    ///     
    ///     星怒/星陨风格武器。从天空中生成瞄准鼠标的抛射物。
    ///     查看“Star Wrath”抛射物的源代码，了解它如何穿过方块。
    ///     有关详细剑术指南，请参见<see cref="AnalysisTwoSword" />
    /// </summary>
    public class AnalysisShootingSword : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 42;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.autoReuse = true;

            Item.DamageType = DamageClass.Melee;
            Item.damage = 50;
            Item.knockBack = 6;
            Item.crit = 6;

            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item1;

            Item.shoot = ProjectileID.StarWrath; // ID of the projectiles the sword will shoot
                                                 // 剑将发射的抛射物ID

            Item.shootSpeed = 8f; // Speed of the projectiles the sword will shoot
                                  // 剑将发射的抛射物速度

            // If you want melee speed to only affect the swing speed of the weapon and not the shoot speed (not recommended)
            // 如果您希望近战速度仅影响武器挥动

            // Item.attackSpeedOnlyAffectsWeaponAnimation = true;
        }
        // This method gets called when firing your weapon/sword.
        // 当你开火/挥剑时，此方法被调用。
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 target = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY);
            float ceilingLimit = target.Y;
            if (ceilingLimit > player.Center.Y - 200f)
            {
                ceilingLimit = player.Center.Y - 200f;
            }
            // Loop these functions 3 times.
            // 重复执行这些函数3次。
            for (int i = 0; i < 3; i++)
            {
                position = player.Center - new Vector2(Main.rand.NextFloat(401) * player.direction, 600f);
                position.Y -= 100 * i;
                Vector2 heading = target - position;

                if (heading.Y < 0f)
                {
                    heading.Y *= -1f;
                }

                if (heading.Y < 20f)
                {
                    heading.Y = 20f;
                }

                heading.Normalize();
                heading *= velocity.Length();
                heading.Y += Main.rand.Next(-40, 41) * 0.02f;
                Projectile.NewProjectile(source, position, heading, type, damage * 2, knockback, player.whoAmI, 0f, ceilingLimit);
            }

            return false;
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
