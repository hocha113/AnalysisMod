using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Tools
{
    // Magic Mirror is one of the only vanilla items that does its action somewhere other than the start of its animation, which is why we use code in UseStyle NOT UseItem.
    // It may prove a useful guide for ModItems with similar behaviors.

    // 魔镜是唯一一个在动画开始时执行其操作的香草物品之外，因此我们在UseStyle而不是UseItem中使用代码。
    // 它可能对具有类似行为的ModItems提供有用的指导。
    internal class AnalysisMagicMirror : AnalysisItem
    {
        private static readonly Color[] itemNameCycleColors = {
            new Color(254, 105, 47),
            new Color(190, 30, 209),
            new Color(34, 221, 151),
            new Color(0, 106, 185),
        };

        public override string Texture => $"Terraria/Images/Item_{ItemID.IceMirror}"; // Copies the texture for the Ice Mirror, make your own texture if need be.
                                                                                      // 复制冰镜子的纹理，如果需要，请自己制作纹理。

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.IceMirror); // Copies the defaults from the Ice Mirror.
                                                  // 从冰镜子中复制默认值。

            Item.color = Color.Violet; // Sets the item color
                                       // 设置物品颜色
        }

        // UseStyle is called each frame that the item is being actively used.
        // UseStyle每帧调用一次正在积极使用该物品的帧数。
        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            // Each frame, make some dust
            // 每帧都会产生一些尘土
            if (Main.rand.NextBool())
            {
                Dust.NewDust(player.position, player.width, player.height, DustID.MagicMirror, 0f, 0f, 150, Color.White, 1.1f); // Makes dust from the player's position and copies the hitbox of which the dust may spawn. Change these arguments if needed.
                                                                                                                                // 从玩家位置生成灰尘，并复制可能生成灰尘的碰撞箱。如有需要，请更改这些参数。
            }

            // This sets up the itemTime correctly.
            // 这样可以正确设置itemTime。
            if (player.itemTime == 0)
            {
                player.ApplyItemTime(Item);
            }
            else if (player.itemTime == player.itemTimeMax / 2)
            {
                // This code runs once halfway through the useTime of the Item. You'll notice with magic mirrors you are still holding the item for a little bit after you've teleported.
                // 此代码在项目useTime的一半运行一次。您会注意到，在魔术镜子上，即使您传送后仍然持有该项也要等待片刻时间才能放下它。

                // Make dust 70 times for a cool effect.
                // 70次发出粉尘以获得很酷的效果。
                for (int d = 0; d < 70; d++)
                {
                    Dust.NewDust(player.position, player.width, player.height, DustID.MagicMirror, player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 150, default, 1.5f);
                }

                // This code releases all grappling hooks and kills/despawns them.
                // 此代码释放所有抓钩并杀死/消失它们。
                player.RemoveAllGrapplingHooks();

                // The actual method that moves the player back to bed/spawn.
                // 将玩家移回床/重生点实际方法.
                player.Spawn(PlayerSpawnContext.RecallFromItem);

                // Make dust 70 times for a cool effect. This dust is the dust at the destination.
                // 发出70次粉尘以获得很酷的效果。这种灰尘是目标地点处的灰尘.
                for (int d = 0; d < 70; d++)
                {
                    Dust.NewDust(player.position, player.width, player.height, DustID.MagicMirror, 0f, 0f, 150, default, 1.5f);
                }
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // This code shows using Color.Lerp,  Main.GameUpdateCount, and the modulo operator (%) to do a neat effect cycling between 4 custom colors.
            // 此代码显示了使用Color.Lerp、Main.GameUpdateCount和模运算符(%)来循环4种自定义颜色之间的漂亮效果。
            int numColors = itemNameCycleColors.Length;

            foreach (TooltipLine line2 in tooltips)
            {
                if (line2.Mod == "Terraria" && line2.Name == "ItemName")
                {
                    float fade = Main.GameUpdateCount % 60 / 60f;
                    int index = (int)(Main.GameUpdateCount / 60 % numColors);
                    int nextIndex = (index + 1) % numColors;

                    line2.OverrideColor = Color.Lerp(itemNameCycleColors[index], itemNameCycleColors[nextIndex], fade);
                }
            }
        }

        // Please see Content/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AnalysisItem>()
                .AddTile<Tiles.Furniture.AnalysisWorkbench>()
                .Register();
        }
    }
}
