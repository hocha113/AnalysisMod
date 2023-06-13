using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Weapons
{
    public class AnalysisMinigun : ModItem
    {
        public override void SetDefaults()
        {
            // Modders can use Item.DefaultToRangedWeapon to quickly set many common properties,
            // such as: useTime, useAnimation, useStyle, autoReuse, DamageType, shoot, shootSpeed, useAmmo, and noMelee.
            // See AnalysisGun.SetDefaults to see comments explaining those properties

            // ģ�������߿���ʹ��Item.DefaultToRangedWeapon����������ೣ�����ԣ�
            // ���磺useTime��useAnimation��useStyle��autoReuse��DamageType��shoot��shootSpeed��useAmmo��noMelee��
            // �����AnalysisGun.SetDefaults�Բ鿴������Щ���Ե�ע��
            Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Bullet, 5, 16f, true);

            // Item.SetWeaponValues can quickly set damage, knockBack, and crit
            // Item.SetWeaponValues���Կ��������˺������˺ͱ�����
            Item.SetWeaponValues(11, 1f);

            Item.width = 54; // Hitbox width of the item.
            Item.height = 22; // Hitbox height of the item.
            Item.rare = ItemRarityID.Green; // The color that the item's name will be in-game.
            Item.UseSound = SoundID.Item11; // The sound that this item plays when used.
        }

        // Please see Content/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AnalysisItem>()
                .AddTile<Tiles.Furniture.AnalysisWorkbench>()
                .Register();
        }

        // The following method gives this gun a 38% chance to not consume ammo
        // ���·���ʹ��ǹ��38���ļ��ʲ����ĵ�ҩ
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return Main.rand.NextFloat() >= 0.38f;
        }

        // The following method allows this gun to shoot when having no ammo, as long as the player has at least 10 Analysis items in their inventory.
        // The gun will then shoot as if the default ammo for it, in this case the musket ball, is being used.

        // ���·���������û�е�ҩʱ�����ǹ֧��ֻҪ��ҿ����������10��������Ʒ��
        // Ȼ�󣬸�ǹ����Ĭ�ϵ�ҩһ�����䣬�ڱ�����Ϊ�����
        public override bool NeedsAmmo(Player player)
        {
            return player.CountItem(ModContent.ItemType<AnalysisItem>(), 10) < 10;
        }

        // The following method makes the gun slightly inaccurate
        // ���·���ʹǹ��΢��׼ȷ
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(10));
        }

        // This method lets you adjust position of the gun in the player's hands. Play with these values until it looks good with your graphics.
        // �˷��������������������������λ�á���������ͼ�ν��е�����ֱ��������á�
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-6f, -2f);
        }
    }
}
