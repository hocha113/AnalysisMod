using AnalysisMod.AnalysisContent.Projectiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Weapons
{
    /// <summary>
    /// This weapon and its corresponding projectile showcase the CloneDefaults() method, which allows for cloning of other items.
    /// For this Analysis, we shall copy the Meowmere and its projectiles with the CloneDefaults() method, while also changing them slightly.
    /// For a more detailed description of each item field used here, check out <see cref="AnalysisTwoSword" />.<br/>
    /// 这个武器及其对应的弹药展示了CloneDefaults()方法，允许克隆其他物品。
    /// 在本分析中，我们将使用CloneDefaults()方法复制Meowmere和它的弹药，并稍微修改它们。
    /// 有关此处使用的每个项目字段的更详细描述，请查看<see cref="AnalysisTwoSword" />。
    /// </summary>
    public class AnalysisCloneWeapon : ModItem
    {
        public override void SetDefaults()
        {
            // This method right here is the backbone of what we're doing here; by using this method, we copy all of
            // the meowmere's SetDefault stats (such as Item.melee and Item.shoot) on to our item, so we don't have to
            // go into the source and copy the stats ourselves. It saves a lot of time and looks much cleaner; if you're
            // going to copy the stats of an item, use CloneDefaults().

            // 这里的方法是我们所做的一切工作的支柱；通过使用此方法，我们将所有Meowmere SetDefault统计信息（例如Item.melee和Item.shoot）复制到我们的物品上，
            // 因此不必进入源代码并自己复制统计信息。这节省了很多时间，并且看起来更加清晰；如果您要复制一个项目的统计信息，请使用CloneDefaults()。

            Item.CloneDefaults(ItemID.Meowmere);

            // After CloneDefaults has been called, we can now modify the stats to our wishes, or keep them as they are.
            // For the sake of Analysis, let's swap the vanilla Meowmere projectile shot from our item for our own projectile by changing Item.shoot:

            // 调用CloneDefaults之后，现在可以根据需要修改统计数据或保持原样。
            // 为了进行分析，让我们通过改变Item.shoot来从我们自己发射一个弹药替换Vanilla Meowmere projectile shot：

            Item.shoot = ModContent.ProjectileType<AnalysisCloneProjectile>(); // Remember that we must use ProjectileType<>() since it is a modded projectile!
                                                                               // 请记住，由于这是modded projectile, 我们必须使用ProjectileType <>()！

                                                                               // Check out AnalysisCloneProjectile to see how this projectile is different from the Vanilla Meowmere projectile.
                                                                               // 查看AnalysisCloneProjectile以查看该projectile与Vanilla Meowmere projectile有何不同。

            // While we're at it, let's make our weapon's stats a bit stronger than the Meowmere, which can be done
            // by using math on each given stat.

            // 顺便说一下，让我们使我们武器比Meowmere更加强大
            // 在每个给定状态上都可以进行数学运算。

            Item.damage *= 2; // Makes this weapon's damage double the Meowmere's damage.
                              // 使该武器造成两倍于Meowmere的伤害。

            Item.shootSpeed *= 1.25f; // Makes this weapon's projectiles shoot 25% faster than the Meowmere's projectiles.
                                      // 使该武器发射的弹药比Meowmere的弹药快25%。
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
