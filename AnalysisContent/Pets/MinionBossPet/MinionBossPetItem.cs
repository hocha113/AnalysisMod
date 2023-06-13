using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Pets.MinionBossPet
{
    // You can find a simple pet Analysis in the AnalysisMod\Content\Pets\AnalysisPet\ folder
    // 在 AnalysisMod\Content\Pets\AnalysisPet\ 文件夹中可以找到一个简单的宠物分析。
    public class MinionBossPetItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ModContent.ProjectileType<MinionBossPetProjectile>(), ModContent.BuffType<MinionBossPetBuff>()); // Vanilla has many useful methods like these, use them! It sets rarity and value aswell, so we have to overwrite those after
                                                                                                                                     // Vanilla 有许多这样有用的方法，要好好利用！它们也设置了稀有度和价值，所以我们需要在之后覆盖掉这些内容。

            Item.width = 28;
            Item.height = 20;
            Item.rare = ItemRarityID.Master;
            Item.master = true; // This makes sure that "Master" displays in the tooltip, as the rarity only changes the item name color
                                // 这确保了“大师”会显示在工具提示中，因为稀有度只改变物品名称颜色。

            Item.value = Item.sellPrice(0, 5);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2); // The item applies the buff, the buff spawns the projectile
                                              // 该物品应用了增益效果，增益效果生成抛射物。

            return false;
        }
    }
}
