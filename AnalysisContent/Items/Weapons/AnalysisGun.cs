using AnalysisMod.AnalysisContent.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Weapons
{
    public class AnalysisGun : ModItem
    {
        public override void SetDefaults()
        {
            // Modders can use Item.DefaultToRangedWeapon to quickly set many common properties, such as: useTime, useAnimation, useStyle, autoReuse, DamageType, shoot, shootSpeed, useAmmo, and noMelee. These are all shown individually here for teaching purposes.
            // Modders可以使用Item.DefaultToRangedWeapon快速设置许多常见属性，例如：useTime、useAnimation、useStyle、autoReuse、DamageType、shoot、shootSpeed、useAmmo和noMelee。这些都在此处单独显示以进行教学。

            // Common Properties
            // 常用属性
            Item.width = 62; // Hitbox width of the item.
            Item.height = 32; // Hitbox height of the item.
            Item.scale = 0.75f;
            Item.rare = ItemRarityID.Green; // The color that the item's name will be in-game.
                                            // 物品名称在游戏中的颜色。

            // Use Properties
            // 使用属性
            Item.useTime = 8; // The item's use time in ticks (60 ticks == 1 second.)
                              // 物品使用时间（以tick为单位，60 tick == 1秒）。

            Item.useAnimation = 8; // The length of the item's use animation in ticks (60 ticks == 1 second.)
                                   // 物品使用动画长度（以tick为单位，60 tick == 1秒）。

            Item.useStyle = ItemUseStyleID.Shoot; // How you use the item (swinging, holding out, etc.)
                                                  // 如何使用该物品（挥舞，伸出等）。
            Item.autoReuse = true; // Whether or not you can hold click to automatically use it again.
                                   // 是否可以按住鼠标自动再次使用它。

            // The sound that this item plays when used.
            // 物品被使用时播放的声音。
            Item.UseSound = new SoundStyle($"{nameof(AnalysisMod)}/Assets/Sounds/Items/Guns/AnalysisGun")
            {
                Volume = 0.9f,
                PitchVariance = 0.2f,
                MaxInstances = 3,
            };

            // Weapon Properties
            // 武器属性
            Item.DamageType = DamageClass.Ranged; // Sets the damage type to ranged.
                                                  // 将伤害类型设置为远程武器。

            Item.damage = 20; // Sets the item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
                              // 设置该物品的伤害值。请注意，由该武器射出的弹丸将其和所用弹药的伤害相加后一起计算。

            Item.knockBack = 5f; // Sets the item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
                                 // 设置该物品的击退值。请注意，由该武器射出的弹丸将其和所用弹药的击退相加后一起计算。

            Item.noMelee = true; // So the item's animation doesn't do damage.
                                 // 因此，该项动画不会造成伤害。

            // Gun Properties
            // 枪械属性
            Item.shoot = ProjectileID.PurificationPowder; // For some reason, all the guns in the vanilla source have this.
                                                          // 由于某种原因，在vanilla源代码中所有枪械都有这个功能。

            Item.shootSpeed = 16f; // The speed of the projectile (measured in pixels per frame.)
                                   // 抛射体速度（以像素/帧为单位）。

            Item.useAmmo = AmmoID.Bullet; // The "ammo Id" of the ammo item that this weapon uses. Ammo IDs are magic numbers that usually correspond to the item id of one item that most commonly represent the ammo type.
                                          // 此武器所需子弹类型对应项目ID的“弹药ID”。弹药ID是通常对应于最常用表示弹药类型的一个项目ID的魔法数字。
        }

        // Please see AnalysisContent/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AnalysisItem>()
                .AddTile<Tiles.Furniture.AnalysisWorkbench>()
                .Register();
        }

        // This method lets you adjust position of the gun in the player's hands. Play with these values until it looks good with your graphics.
        // 此方法允许您调整玩家手中枪械的位置。根据您的图形进行调整，直到看起来很好。
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(0f, -1f);
        }

        //TODO: Move this to a more specifically named Analysis. Say, a paint gun?
        //TODO：将此移动到更具体命名的Analysis中。比如说，喷漆枪？
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            // Every projectile shot from this gun has a 1/3 chance of being an AnalysisInstancedProjectile
            // 从该枪械射出的每个抛射物都有1/3几率成为AnalysisInstancedProjectile
            if (Main.rand.NextBool(3))
            {
                type = ModContent.ProjectileType<AnalysisInstancedProjectile>();
            }
        }

        /*让子弹顺时针旋转45度
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float rotation = MathHelper.ToRadians(45);
            Vector2 perturbedSpeed = velocity.RotatedBy(rotation);
            Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, knockback, player.whoAmI);
            return false; // return false to stop vanilla from calling Projectile.NewProjectile.
                          // 返回false以阻止vanilla调用Projectile.NewProjectile。
        }

        /*
		* Feel free to uncomment any of the Analysiss below to see what they do
		* 随意取消注释下面任何一个分析以查看它们所做的事情。
		*/

        // What if I wanted it to work like Uzi, replacing regular bullets with High Velocity Bullets?
        // 如果我想要它像Uzi一样工作，用高速子弹替换普通子弹呢？
        // Uzi/Molten Fury style: Replace normal Bullets with High Velocity
        // Uzi/Molten Fury风格：使用高速子弹替换普通子弹

        /*public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			if (type == ProjectileID.Bullet) { // or ProjectileID.WoodenArrowFriendly
				type = ProjectileID.BulletHighVelocity; // or ProjectileID.FireArrow;
			}
		}*/

        // What if I wanted multiple projectiles in a even spread? (Vampire Knives)
        // 如果我想要多个投影在均匀分布中？（吸血鬼小刀）
        // Even Arc style: Multiple Projectile, Even Spread
        // 均匀圆形风格：多重投影，在均匀分布中

        /*public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			float numberProjectiles = 3 + Main.rand.Next(3); // 3, 4, or 5 shots
			float rotation = MathHelper.ToRadians(45);

			position += Vector2.Normalize(velocity) * 45f;

			for (int i = 0; i < numberProjectiles; i++) {
				Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .2f; // Watch out for dividing by 0 if there is only 1 projectile.
				Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, knockback, player.whoAmI);
			}

			return false; // return false to stop vanilla from calling Projectile.NewProjectile.
                          // 返回false以阻止vanilla调用Projectile.NewProjectile。
		}*/

        // How can I make the shots appear out of the muzzle exactly?
        // Also, when I do this, how do I prevent shooting through tiles?
        // 如何让子弹从枪口准确出现？此外，我该如何防止穿过瓷砖？

        /*public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			Vector2 muzzleOffset = Vector2.Normalize(velocity) * 25f;

			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0)) {
				position += muzzleOffset;
			}
		}*/

        // How can I get a "Clockwork Assault Rifle" effect?
        // 如何实现“发条突击步枪”的效果？
        // 3 round burst, only consume 1 ammo for burst. Delay between bursts, use reuseDelay
        // 3连发，每次只消耗1个弹药。在连发之间加入延迟，使用reuseDelay。

        // Make the following changes to SetDefaults():
        /*
			item.useAnimation = 12;
			item.useTime = 4; // one third of useAnimation
			item.reuseDelay = 14;
			item.consumeAmmoOnLastShotOnly = true;
		*/

        // How can I shoot 2 different projectiles at the same time?
        // 如何同时射出2种不同的抛射物？
        /*public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			// Here we manually spawn the 2nd projectile, manually specifying the projectile type that we wish to shoot.
			Projectile.NewProjectile(source, position, velocity, ProjectileID.GrenadeI, damage, knockback, player.whoAmI);

			// By returning true, the vanilla behavior will take place, which will shoot the 1st projectile, the one determined by the ammo.
			return true;
		}*/

        // How can I choose between several projectiles randomly?
        // 如何随机选择多个抛射物之间进行切换？
        /*public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			// Here we randomly set type to either the original (as defined by the ammo), a vanilla projectile, or a mod projectile.
			type = Main.rand.Next(new int[] { type, ProjectileID.GoldenBullet, ModContent.ProjectileType<Projectiles.AnalysisBullet>() });
		}*/
    }
}
