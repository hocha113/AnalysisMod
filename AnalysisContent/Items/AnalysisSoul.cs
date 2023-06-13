using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items
{
    public class AnalysisSoul : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Registers a vertical animation with 4 frames and each one will last 5 ticks (1/12 second)
            // 注册一个垂直动画，包含4帧，每一帧持续5个刻（1/12秒）
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 4));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true; // Makes the item have an animation while in world (not held.). Use in combination with RegisterItemAnimation
                                                          // 使物品在世界中拥有动画效果（非手持状态）。与RegisterItemAnimation结合使用

            ItemID.Sets.ItemIconPulse[Item.type] = true; // The item pulses while in the player's inventory
                                                         // 物品在玩家的背包中会发出脉冲光

            ItemID.Sets.ItemNoGravity[Item.type] = true; // Makes the item have no gravity
                                                         // 使物品没有重力

            Item.ResearchUnlockCount = 25; // Configure the amount of this item that's needed to research it in Journey mode.
                                           // 在旅行模式下配置需要收集多少该物品才能研究它。
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 1000; // Makes the item worth 1 gold.
                               // 将此物品价值设为1金币。

            Item.rare = ItemRarityID.Orange;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.WhiteSmoke.ToVector3() * 0.55f * Main.essScale); // Makes this item glow when thrown out of inventory.
                                                                                                  // 当从库存中抛出时，使此物品发光。
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
