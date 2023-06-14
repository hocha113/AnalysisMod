using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Weapons
{
    // This is an Analysis gun designed to best demonstrate the various tML hooks that can be used for ammo-related specifications.
    // 这是一款分析枪，旨在最好地展示可用于弹药相关规格的各种tML钩子。
    public class AnalysisSpecificAmmoGun : ModItem
    {
        public static readonly int FreeAmmoChance1 = 20;
        public static readonly int FreeAmmoChance2 = 63;
        public static readonly int FreeAmmoChance3 = 36;
        public static readonly int AmmoUseDamageBoost = 20;

        private bool consumptionDamageBoost = false;

        public override string Texture => "AnalysisMod/AnalysisContent/Items/Weapons/AnalysisGun"; //TODO: remove when sprite is made for this
                                                                                                   //TODO：制作精灵图后删除此部分

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(FreeAmmoChance1, FreeAmmoChance2, FreeAmmoChance3, AmmoUseDamageBoost);

        public override void SetDefaults()
        {
            // Modders can use Item.DefaultToRangedWeapon to quickly set many common properties, such as: useTime, useAnimation, useStyle, autoReuse, DamageType, shoot, shootSpeed, useAmmo, and noMelee. These are all shown individually here for teaching purposes.
            // 模组开发者可以使用Item.DefaultToRangedWeapon快速设置许多常见属性，例如：useTime、useAnimation、useStyle、autoReuse、DamageType、shoot、shootSpeed、useAmmo和noMelee。这些都在此单独显示以供教学目的。

            // Common Properties
            Item.width = 62; // Hitbox width of the item.
            Item.height = 32; // Hitbox height of the item.
            Item.scale = 0.75f;
            Item.rare = ItemRarityID.Green; // The color that the item's name will be in-game.

            // Use Properties
            Item.useTime = 5; // The item's use time in ticks (60 ticks == 1 second.)
            Item.useAnimation = 15; // The length of the item's use animation in ticks (60 ticks == 1 second.)
            Item.reuseDelay = 5; // The amount of time the item waits between use animations (60 ticks == 1 second.)
            Item.useStyle = ItemUseStyleID.Shoot; // How you use the item (swinging, holding out, etc.)
            Item.autoReuse = true; // Whether or not you can hold click to automatically use it again.
            Item.UseSound = SoundID.Item11;

            // Weapon Properties
            Item.DamageType = DamageClass.Ranged; // Sets the damage type to ranged.
            Item.damage = 20; // Sets the item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
            Item.knockBack = 5f; // Sets the item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
            Item.noMelee = true; // So the item's animation doesn't do damage.

            // Gun Properties
            Item.shoot = ProjectileID.PurificationPowder; // For some reason, all the guns in the vanilla source have this.
            Item.shootSpeed = 16f; // The speed of the projectile (measured in pixels per frame.)
            Item.useAmmo = AmmoID.Bullet; // The "ammo Id" of the ammo item that this weapon uses. Ammo IDs are magic numbers that usually correspond to the item id of one item that most commonly represent the ammo type.
        }

        // Please see AnalysisContent/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AnalysisItem>()
                .AddTile<Tiles.Furniture.AnalysisWorkbench>()
                .Register();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(2f, -2f);
        }

        public override void UpdateInventory(Player player)
        {
            consumptionDamageBoost = false;
        }

        public override bool? CanChooseAmmo(Item ammo, Player player)
        {
            // CanChooseAmmo allows ammo to be chosen or denied independently of the useAmmo field's restrictions.
            // (Its sister hook, CanBeChosenAsAmmo, is called on the ammo, and has the same function.)
            // This returns null by default, which simply picks the ammo based on whether or not ammo.ammo == weapon.useAmmo.
            // Returning true will forcibly allow an ammo to be used; returning false will forcibly deny it.
            // For this Analysis, we'll forcefully deny Cursed Bullets from being used as ammunition, but otherwise make no changes to the ammo pool.

            // CanChooseAmmo允许独立选择或拒绝弹药，而不受useAmmo字段限制。
            // （它的姐妹钩子CanBeChosenAsAmmo被称为弹药，并具有相同的功能。）
            // 默认情况下返回null，仅基于ammo.ammo == weapon.useAmmo来选择弹药。
            // 返回true将强制允许使用某种类型的弹药；返回false将禁止使用该类型的弹药。
            // 对于本分析，我们将强制拒绝使用诅咒子弹作为武器装填物，但对其他装填池不做任何更改。
            if (ammo.type == ItemID.CursedBullet)
                return false;

            // Oh, and a word of advice: always default to returning null, as per the above.
            // Defaulting to returning true or false may have unintended consequences on what you can or can't use as ammo.

            // 还有一个建议：始终默认返回null，请参考上文内容。
            // 默认返回true或false可能会对您可以或不能用作装填物品产生意想不到的影响。
            return null;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            // CanConsumeAmmo allows ammo to be conserved or consumed depending on various conditions.
            // (Its sister hook, CanBeConsumedAsAmmo, is called on the ammo, and has the same function.)
            // This returns true by default; returning false for any reason will prevent ammo consumption.
            // Note that returning true does NOT allow you to force ammo consumption; this currently requires use of IL editing or detours.

            // CanConsumeAmmo允许根据各种条件保留或消耗弹药。
            // （它的附带钩子CanBeConsumedAsAmmo被称为弹药，并具有相同的功能。）
            // 默认情况下返回true；由于任何原因返回false将防止消耗弹药。
            // 请注意，返回true并不允许您强制消耗弹药；目前需要使用IL编辑或detours。

            // For this Analysis, the first shot will have a 20% chance to conserve ammo...
            // ...the second shot will have a 63% chance to conserve ammo...
            // ...and the third shot will have a 36% chance to conserve ammo.

            // 对于本分析，第一枪将有20％的几率保留弹药...
            // ...第二枪将有63％的几率保留弹药...
            // ...第三枪将有36％的几率保留弹药。
            if (player.ItemUsesThisAnimation == 0)
                return Main.rand.NextFloat() >= FreeAmmoChance1 / 100f;
            else if (player.ItemUsesThisAnimation == 1)
                return Main.rand.NextFloat() >= FreeAmmoChance2 / 100f;
            else if (player.ItemUsesThisAnimation == 2)
                return Main.rand.NextFloat() >= FreeAmmoChance3 / 100f;

            return true;
        }

        public override void OnConsumeAmmo(Item ammo, Player player)
        {
            // OnConsumeAmmo allows you to make things happen when ammo is successfully consumed.
            // (Its sister hook, OnConsumedAsAmmo, is called on the ammo, and has the same function.)
            // Here, we'll set a bool to true which dictates whether or not the next shot should receive a damage bonus.
            // This makes it so that shots which do consume ammunition gain a damage bonus in exchange for that consumption.

            // OnConsumeAmmo允许在成功消耗装填物品时执行操作。
            // （它的姐妹钩子OnConsumedAsAmmo被称为装填物品，并具有相同的功能。）
            // 在这里，我们将设置一个bool值为true，该值指示是否应对下一发射击进行伤害加成。
            // 这使得那些真正消耗了装填物品而获得伤害加成。
            consumptionDamageBoost = true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (consumptionDamageBoost)
            {
                damage = damage * (100 + AmmoUseDamageBoost) / 100;
            }
        }
    }
}
