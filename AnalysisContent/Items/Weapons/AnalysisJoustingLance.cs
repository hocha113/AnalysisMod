using Terraria;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Weapons
{
    public class AnalysisJoustingLance : ModItem
    {
        public override void SetDefaults()
        {
            // A special method that sets a variety of item parameters that make the item act like a spear weapon.
            // To see everything DefaultToSpear() does, right click the method in Visual Studios and choose "Go To Definition" (or press F12). You can also hover over DefaultToSpear to see the documentation.
            // The shoot speed will affect how far away the projectile spawns from the player's hand.
            // If you are using the custom AI in your projectile (and not aiStyle 19 and AIType = ProjectileID.JoustingLance), the standard value is 1f.
            // If you are using aiStyle 19 and AIType = ProjectileID.JoustingLance, then multiply the value by about 3.5f.

            // 一种特殊的方法，可以设置各种物品参数，使其像长矛武器一样运作。
            // 要查看DefaultToSpear()的所有内容，请在Visual Studios中右键单击该方法并选择“转到定义”（或按F12）。您还可以将鼠标悬停在DefaultToSpear上以查看文档。
            // 射速会影响抛射物从玩家手中产生的距离。
            // 如果您正在使用自定义AI来控制抛射物（而不是aiStyle 19和AIType = ProjectileID.JoustingLance），则标准值为1f。
            // 如果您正在使用aiStyle 19和AIType = ProjectileID.JoustingLance，则将该值乘以约3.5f。
            Item.DefaultToSpear(ModContent.ProjectileType<Projectiles.AnalysisJoustingLanceProjectile>(), 1f, 24);

            Item.DamageType = DamageClass.MeleeNoSpeed; // We need to use MeleeNoSpeed here so that attack speed doesn't effect our held projectile.
                                                        // 我们需要在这里使用MeleeNoSpeed，以便攻击速度不会影响我们持有的投射物。

            Item.SetWeaponValues(56, 12f, 0); // A special method that sets the damage, knockback, and bonus critical strike chance.
                                              // 一种特殊的方法，可设置伤害、击退力和额外暴击率。

            Item.SetShopValues(ItemRarityColor.LightRed4, Item.buyPrice(0, 6)); // A special method that sets the rarity and value.
                                                                                // 一种特殊的方法，可设置稀有度和价值。

            Item.channel = true; // Channel is important for our projectile.
                                 // Channel对于我们的投射物非常重要。

            // This will make sure our projectile completely disappears on hurt.
            // It's not enough just to stop the channel, as the lance can still deal damage while being stowed
            // If two players charge at each other, the first one to hit should cancel the other's lance

            // 这将确保我们的投射物完全消失，并且不会受到伤害。
            // 仅停止通道是不够的，因为长矛仍然可能在收起时造成伤害
            // 如果两个玩家相互冲撞，则先碰撞的玩家应该取消另一个人的长矛
            Item.StopAnimationOnHurt = true;
        }

        // This will allow our Jousting Lance to receive the same modifiers as melee weapons.
        // 这将允许我们的Jousting Lance接收与近战武器相同的修饰符。
        public override bool MeleePrefix()
        {
            return true;
        }

        // Please see AnalysisContent/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AnalysisItem>(5)
                .AddTile<Tiles.Furniture.AnalysisWorkbench>()
                .Register();
        }
    }
}