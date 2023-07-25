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
            // Modders����ʹ��Item.DefaultToRangedWeapon����������ೣ�����ԣ����磺useTime��useAnimation��useStyle��autoReuse��DamageType��shoot��shootSpeed��useAmmo��noMelee����Щ���ڴ˴�������ʾ�Խ��н�ѧ��

            // Common Properties
            // ��������
            Item.width = 62; // Hitbox width of the item.
            Item.height = 32; // Hitbox height of the item.
            Item.scale = 0.75f;
            Item.rare = ItemRarityID.Green; // The color that the item's name will be in-game.
                                            // ��Ʒ��������Ϸ�е���ɫ��

            // Use Properties
            // ʹ������
            Item.useTime = 8; // The item's use time in ticks (60 ticks == 1 second.)
                              // ��Ʒʹ��ʱ�䣨��tickΪ��λ��60 tick == 1�룩��

            Item.useAnimation = 8; // The length of the item's use animation in ticks (60 ticks == 1 second.)
                                   // ��Ʒʹ�ö������ȣ���tickΪ��λ��60 tick == 1�룩��

            Item.useStyle = ItemUseStyleID.Shoot; // How you use the item (swinging, holding out, etc.)
                                                  // ���ʹ�ø���Ʒ�����裬����ȣ���
            Item.autoReuse = true; // Whether or not you can hold click to automatically use it again.
                                   // �Ƿ���԰�ס����Զ��ٴ�ʹ������

            // The sound that this item plays when used.
            // ��Ʒ��ʹ��ʱ���ŵ�������
            Item.UseSound = new SoundStyle($"{nameof(AnalysisMod)}/Assets/Sounds/Items/Guns/AnalysisGun")
            {
                Volume = 0.9f,
                PitchVariance = 0.2f,
                MaxInstances = 3,
            };

            // Weapon Properties
            // ��������
            Item.DamageType = DamageClass.Ranged; // Sets the damage type to ranged.
                                                  // ���˺���������ΪԶ��������

            Item.damage = 20; // Sets the item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
                              // ���ø���Ʒ���˺�ֵ����ע�⣬�ɸ���������ĵ��轫������õ�ҩ���˺���Ӻ�һ����㡣

            Item.knockBack = 5f; // Sets the item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
                                 // ���ø���Ʒ�Ļ���ֵ����ע�⣬�ɸ���������ĵ��轫������õ�ҩ�Ļ�����Ӻ�һ����㡣

            Item.noMelee = true; // So the item's animation doesn't do damage.
                                 // ��ˣ��������������˺���

            // Gun Properties
            // ǹе����
            Item.shoot = ProjectileID.PurificationPowder; // For some reason, all the guns in the vanilla source have this.
                                                          // ����ĳ��ԭ����vanillaԴ����������ǹе����������ܡ�

            Item.shootSpeed = 16f; // The speed of the projectile (measured in pixels per frame.)
                                   // �������ٶȣ�������/֡Ϊ��λ����

            Item.useAmmo = AmmoID.Bullet; // The "ammo Id" of the ammo item that this weapon uses. Ammo IDs are magic numbers that usually correspond to the item id of one item that most commonly represent the ammo type.
                                          // �����������ӵ����Ͷ�Ӧ��ĿID�ġ���ҩID������ҩID��ͨ����Ӧ����ñ�ʾ��ҩ���͵�һ����ĿID��ħ�����֡�
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
        // �˷��������������������ǹе��λ�á���������ͼ�ν��е�����ֱ���������ܺá�
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(0f, -1f);
        }

        //TODO: Move this to a more specifically named Analysis. Say, a paint gun?
        //TODO�������ƶ���������������Analysis�С�����˵������ǹ��
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            // Every projectile shot from this gun has a 1/3 chance of being an AnalysisInstancedProjectile
            // �Ӹ�ǹе�����ÿ�������ﶼ��1/3���ʳ�ΪAnalysisInstancedProjectile
            if (Main.rand.NextBool(3))
            {
                type = ModContent.ProjectileType<AnalysisInstancedProjectile>();
            }
        }

        /*���ӵ�˳ʱ����ת45��
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float rotation = MathHelper.ToRadians(45);
            Vector2 perturbedSpeed = velocity.RotatedBy(rotation);
            Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, knockback, player.whoAmI);
            return false; // return false to stop vanilla from calling Projectile.NewProjectile.
                          // ����false����ֹvanilla����Projectile.NewProjectile��
        }

        /*
		* Feel free to uncomment any of the Analysiss below to see what they do
		* ����ȡ��ע�������κ�һ�������Բ鿴�������������顣
		*/

        // What if I wanted it to work like Uzi, replacing regular bullets with High Velocity Bullets?
        // �������Ҫ����Uziһ���������ø����ӵ��滻��ͨ�ӵ��أ�
        // Uzi/Molten Fury style: Replace normal Bullets with High Velocity
        // Uzi/Molten Fury���ʹ�ø����ӵ��滻��ͨ�ӵ�

        /*public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			if (type == ProjectileID.Bullet) { // or ProjectileID.WoodenArrowFriendly
				type = ProjectileID.BulletHighVelocity; // or ProjectileID.FireArrow;
			}
		}*/

        // What if I wanted multiple projectiles in a even spread? (Vampire Knives)
        // �������Ҫ���ͶӰ�ھ��ȷֲ��У�����Ѫ��С����
        // Even Arc style: Multiple Projectile, Even Spread
        // ����Բ�η�񣺶���ͶӰ���ھ��ȷֲ���

        /*public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			float numberProjectiles = 3 + Main.rand.Next(3); // 3, 4, or 5 shots
			float rotation = MathHelper.ToRadians(45);

			position += Vector2.Normalize(velocity) * 45f;

			for (int i = 0; i < numberProjectiles; i++) {
				Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .2f; // Watch out for dividing by 0 if there is only 1 projectile.
				Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, knockback, player.whoAmI);
			}

			return false; // return false to stop vanilla from calling Projectile.NewProjectile.
                          // ����false����ֹvanilla����Projectile.NewProjectile��
		}*/

        // How can I make the shots appear out of the muzzle exactly?
        // Also, when I do this, how do I prevent shooting through tiles?
        // ������ӵ���ǹ��׼ȷ���֣����⣬�Ҹ���η�ֹ������ש��

        /*public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			Vector2 muzzleOffset = Vector2.Normalize(velocity) * 25f;

			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0)) {
				position += muzzleOffset;
			}
		}*/

        // How can I get a "Clockwork Assault Rifle" effect?
        // ���ʵ�֡�����ͻ����ǹ����Ч����
        // 3 round burst, only consume 1 ammo for burst. Delay between bursts, use reuseDelay
        // 3������ÿ��ֻ����1����ҩ��������֮������ӳ٣�ʹ��reuseDelay��

        // Make the following changes to SetDefaults():
        /*
			item.useAnimation = 12;
			item.useTime = 4; // one third of useAnimation
			item.reuseDelay = 14;
			item.consumeAmmoOnLastShotOnly = true;
		*/

        // How can I shoot 2 different projectiles at the same time?
        // ���ͬʱ���2�ֲ�ͬ�������
        /*public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			// Here we manually spawn the 2nd projectile, manually specifying the projectile type that we wish to shoot.
			Projectile.NewProjectile(source, position, velocity, ProjectileID.GrenadeI, damage, knockback, player.whoAmI);

			// By returning true, the vanilla behavior will take place, which will shoot the 1st projectile, the one determined by the ammo.
			return true;
		}*/

        // How can I choose between several projectiles randomly?
        // ������ѡ����������֮������л���
        /*public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			// Here we randomly set type to either the original (as defined by the ammo), a vanilla projectile, or a mod projectile.
			type = Main.rand.Next(new int[] { type, ProjectileID.GoldenBullet, ModContent.ProjectileType<Projectiles.AnalysisBullet>() });
		}*/
    }
}
