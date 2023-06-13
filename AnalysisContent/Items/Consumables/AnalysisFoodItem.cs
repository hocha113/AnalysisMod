using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Consumables
{
    public class AnalysisFoodItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;

            // This is to show the correct frame in the inventory
            // The MaxValue argument is for the animation speed, we want it to be stuck on frame 1
            // Setting it to max value will cause it to take 414 days to reach the next frame
            // No one is going to have game open that long so this is fine
            // The second argument is the number of frames, which is 3
            // The first frame is the inventory texture, the second frame is the holding texture,
            // and the third frame is the placed texture

            // 这是为了在库存中显示正确的框架
            // MaxValue参数用于动画速度，我们希望它停留在第1帧
            // 将其设置为最大值将导致需要414天才能到达下一帧
            // 没有人会打开游戏那么长时间，所以这很好
            // 第二个参数是帧数，即3个
            // 第一帧是库存纹理，第二帧是持握纹理，
            // 第三个框架是放置的纹理
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));

            // This allows you to change the color of the crumbs that are created when you eat.
            // The numbers are RGB (Red, Green, and Blue) values which range from 0 to 255.
            // Most foods have 3 crumb colors, but you can use more or less if you desire.
            // Depending on if you are making solid or liquid food switch out FoodParticleColors
            // with DrinkParticleColors. The difference is that food particles fly outwards
            // whereas drink particles fall straight down and are slightly transparent

            // 这允许您更改吃东西时创建的面包屑的颜色。
            // 数字是RGB（红色、绿色和蓝色）值，范围从0到255。
            // 大多数食物都有3种碎屑颜色，但如果您愿意，则可以使用更多或更少。
            // 根据您制作实体食品还是液体食品，在FoodParticleColors中切换为DrinkParticleColors。
            // 区别在于食物粒子向外飞出，
            // 而饮料粒子直落并且稍微透明。
            ItemID.Sets.FoodParticleColors[Item.type] = new Color[3] {
                new Color(249, 230, 136),
                new Color(152, 93, 95),
                new Color(174, 192, 192)
            };

            ItemID.Sets.IsFood[Type] = true; //This allows it to be placed on a plate and held correctly
                                             //这使得它可以放在盘子上并正确地握住
        }

        public override void SetDefaults()
        {
            // This code matches the ApplePie code.
            // 这段代码与ApplePie代码匹配

            // DefaultToFood sets all of the food related item defaults such as the buff type, buff duration, use sound, and animation time.
            // DefaultToFood设置所有与食品相关的项目默认值，如增益类型、增益持续时间、使用声音和动画时间。
            Item.DefaultToFood(22, 22, BuffID.WellFed3, 57600); // 57600 is 16 minutes: 16 * 60 * 60
                                                                // 57600等于16分钟：16 * 60 * 60
            Item.value = Item.buyPrice(0, 3);
            Item.rare = ItemRarityID.Blue;
        }

        // If you want multiple buffs, you can apply the remainder of buffs with this method.
        // Make sure the primary buff is set in SetDefaults so that the QuickBuff hotkey can work properly.

        // 如果您想要多个增益效果，可以使用此方法应用其余的增益效果。
        // 确保将主要的buff设置为SetDefaults中，以便QuickBuff热键能够正常工作。
        public override void OnConsumeItem(Player player)
        {
            player.AddBuff(BuffID.SugarRush, 3600);
        }

        //Please see AnalysisContent/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AnalysisItem>()
                .AddIngredient(ItemID.Apple, 3)
                .AddTile<Tiles.Furniture.AnalysisWorkbench>()
                .Register();
        }
    }
}