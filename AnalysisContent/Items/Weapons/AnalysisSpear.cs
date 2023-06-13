using AnalysisMod.AnalysisContent.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Weapons
{
    public class AnalysisSpear : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SkipsInitialUseSound[Item.type] = true; // This skips use animation-tied sound playback, so that we're able to make it be tied to use time instead in the UseItem() hook.
                                                                // 这样可以跳过使用动画绑定的声音播放，使其能够绑定到使用时间而不是UseItem()钩子。

            ItemID.Sets.Spears[Item.type] = true; // This allows the game to recognize our new item as a spear.
                                                  // 这允许游戏将我们的新物品识别为长矛。
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.rare = ItemRarityID.Pink; // Assign this item a rarity level of Pink
            Item.value = Item.sellPrice(silver: 10); // The number and type of coins item can be sold for to an NPC

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot; // How you use the item (swinging, holding out, etc.)
            Item.useAnimation = 12; // The length of the item's use animation in ticks (60 ticks == 1 second.)
            Item.useTime = 18; // The length of the item's use time in ticks (60 ticks == 1 second.)
            Item.UseSound = SoundID.Item71; // The sound that this item plays when used.
            Item.autoReuse = true; // Allows the player to hold click to automatically use the item again. Most spears don't autoReuse, but it's possible when used in conjunction with CanUseItem()

            // Weapon Properties
            Item.damage = 25;
            Item.knockBack = 6.5f;
            Item.noUseGraphic = true; // When true, the item's sprite will not be visible while the item is in use. This is true because the spear projectile is what's shown so we do not want to show the spear sprite as well.
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true; // Allows the item's animation to do damage. This is important because the spear is actually a projectile instead of an item. This prevents the melee hitbox of this item.

            // Projectile Properties
            Item.shootSpeed = 3.7f; // The speed of the projectile measured in pixels per frame.
            Item.shoot = ModContent.ProjectileType<AnalysisSpearProjectile>(); // The projectile that is fired from this weapon
        }

        public override bool CanUseItem(Player player)
        {
            // Ensures no more than one spear can be thrown out, use this when using autoReuse
            // 确保最多只能扔出一支长矛，在使用autoReuse时使用此选项
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override bool? UseItem(Player player)
        {
            // Because we're skipping sound playback on use animation start, we have to play it ourselves whenever the item is actually used.
            // 因为我们在使用动画开始时跳过了声音播放，所以每当实际使用该物品时都必须自己播放它。
            if (!Main.dedServ && Item.UseSound.HasValue)
            {
                SoundEngine.PlaySound(Item.UseSound.Value, player.Center);
            }

            return null;
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
