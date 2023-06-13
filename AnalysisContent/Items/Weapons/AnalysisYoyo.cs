using System;
using AnalysisMod.AnalysisContent.Projectiles;
//using AnalysisMod.AnalysisContent.Rarities;
using AnalysisMod.AnalysisContent.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Weapons
{
    public class AnalysisYoyo : ModItem
    {
        public override void SetStaticDefaults()
        {
            // These are all related to gamepad controls and don't seem to affect anything else
            // 这些都与手柄控制有关，似乎不会影响其他任何内容。

            ItemID.Sets.Yoyo[Item.type] = true; // Used to increase the gamepad range when using Strings.
                                                // 用于增加使用 Strings 时的手柄范围。

            ItemID.Sets.GamepadExtraRange[Item.type] = 15; // Increases the gamepad range. Some vanilla values: 4 (Wood), 10 (Valor), 13 (Yelets), 18 (The Eye of Cthulhu), 21 (Terrarian).
                                                           // 增加手柄范围。一些基础数值：4（木），10（瓦洛尔(女武神)），13（叶莱特斯），18（克苏鲁之眼），21（泰拉球）。

            ItemID.Sets.GamepadSmartQuickReach[Item.type] = true; // Unused, but weapons that require aiming on the screen are in this set.
                                                                  // 未使用，但需要在屏幕上瞄准的武器包含在此集合中。
        }

        public override void SetDefaults()
        {
            Item.width = 24; // The width of the item's hitbox.
            Item.height = 24; // The height of the item's hitbox.

            Item.useStyle = ItemUseStyleID.Shoot; // The way the item is used (e.g. swinging, throwing, etc.)
            Item.useTime = 25; // All vanilla yoyos have a useTime of 25.
            Item.useAnimation = 25; // All vanilla yoyos have a useAnimation of 25.
            Item.noMelee = true; // This makes it so the item doesn't do damage to enemies (the projectile does that).
            Item.noUseGraphic = true; // Makes the item invisible while using it (the projectile is the visible part).
            Item.UseSound = SoundID.Item1; // The sound that will play when the item is used.

            Item.damage = 40; // The amount of damage the item does to an enemy or player.
            Item.DamageType = DamageClass.MeleeNoSpeed; // The type of damage the weapon does. MeleeNoSpeed means the item will not scale with attack speed.
            Item.knockBack = 2.5f; // The amount of knockback the item inflicts.
            Item.crit = 8; // The percent chance for the weapon to deal a critical strike. Defaults to 4.
            Item.channel = true; // Set to true for items that require the attack button to be held out (e.g. yoyos and magic missile weapons)
            Item.rare = 12; // The item's rarity. This changes the color of the item's name.
            Item.value = Item.buyPrice(gold: 1); // The amount of money that the item is can be bought for.

            Item.shoot = ModContent.ProjectileType<AnalysisYoyoProjectile>(); // Which projectile this item will shoot. We set this to our corresponding projectile.
            Item.shootSpeed = 16f; // The velocity of the shot projectile.			
        }

        // Here is an Analysis of blacklisting certain modifiers. Remove this section for standard vanilla behavior.
        // In this Analysis, we are blacklisting the ones that reduce damage of a melee weapon.
        // Make sure that your item can even receive these prefixes (check the vanilla wiki on prefixes).

        // 这里是黑名单修改器的分析。删除此部分以获取标准基础行为。
        // 在这个分析中，我们将列出那些降低近战武器伤害的修饰符。
        // 确保您的物品可以接收这些前缀 (查看原版维基百科)。
        private static readonly int[] unwantedPrefixes = new int[] { PrefixID.Terrible, PrefixID.Dull, PrefixID.Shameful, PrefixID.Annoying, PrefixID.Broken, PrefixID.Damaged, PrefixID.Shoddy };

        public override bool AllowPrefix(int pre)
        {
            // return false to make the game reroll the prefix.
            // 返回 false 以使游戏重新随机前缀

            // DON'T DO THIS BY ITSELF:
            // return false;
            // This will get the game stuck because it will try to reroll every time. Instead, make it have a chance to return true.

            // 不要仅仅返回 false:
            // 这会让游戏卡住因为它每次都会尝试重新随机。
            // 相反地，请让它有一个返回 true 的机会。

            if (Array.IndexOf(unwantedPrefixes, pre) > -1)
            {
                // IndexOf returns a positive index of the element you search for. If not found, it's less than 0.
                // Here we check if the selected prefix is positive (it was found).
                // If so, we found a prefix that we don't want. Reroll.

                // IndexOf 返回您搜索元素的正索引。如果没有找到，则小于0。
                // 在这里，我们检查所选前缀是否为正数(已找到)。
                // 如果是，则表示我们发现了一个不想要的前缀。重新随机。
                return false;
            }

            // Don't reroll
            // 不要重新投掷
            return true;
        }

        // Please see AnalysisContent/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AnalysisItem>()
                .AddTile<AnalysisWorkbench>()
                .Register();
        }
    }
}
