using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisCommon.GlobalItems
{
    // This file shows a very simple Analysis of a GlobalItem class. GlobalItem hooks are called on all items in the game and are suitable for sweeping changes like
    // adding additional data to all items in the game. Here we simply adjust the damage of the Copper Shortsword item, as it is simple to understand.
    // See other GlobalItem classes in AnalysisMod to see other ways that GlobalItem can be used.
    // 这个文件展示了一个非常简单的GlobalItem类分析。GlobalItem钩子会在游戏中所有物品上调用，并适合进行像添加额外数据到游戏中所有物品这样的扫描式更改。在这里，我们只是调整铜短剑物品的伤害，因为它很容易理解。
    // 查看AnalysisMod中其他GlobalItem类以查看可以使用GlobalItem的其他方式。
    public class ShortswordGlobalItem : GlobalItem
    {
        // Here we make sure to only instance this GlobalItem for the Copper Shortsword, by checking item.type
        // 通过检查 item.type，我们确保仅为铜短剑实例化此 GlobalItem
        public override bool AppliesToEntity(Item item, bool lateInstatiation)
        {
            return item.type == ItemID.CopperShortsword;
        }

        public override void SetDefaults(Item item)
        {
            item.StatsModifiedBy.Add(Mod); // Notify the game that we've made a functional change to this item.
                                           // 通知游戏，我们对此物品进行了功能性更改。

            item.damage = 50; // Change damage to 50!
        }

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Make it shoot grenades for no reason
            // 让它无缘无故地发射榴弹
            Projectile.NewProjectileDirect(source, player.Center, velocity * 5f, ProjectileID.Grenade, damage, knockback, player.whoAmI);
            // Returning false prevents vanilla's shooting behavior from running.
            // In this case it prevents the shortsword's blade stabbing animation, as the blade itself is a projectile.
            // 返回 false 可以防止基础射击行为的运行。
            // 在这种情况下，它可以阻止短剑刺杀动画的播放，因为短剑本身就是一个抛射物。
            return true;
        }
    }
}
