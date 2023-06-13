using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AnalysisMod.AnalysisContent.Items
{
    public class AnalysisStackableDurabilityItem : ModItem
    {
        // 0 to 1
        // All items in the stack have the same durability
        // Durability is combined and averaged when stacking

        // 0到1
        // 栈中所有物品的耐久度相同
        // 堆叠时，耐久度会被合并和平均
        public float durability;

        public override void SetDefaults()
        {
            Item.maxStack = Item.CommonMaxStack; // This item is stackable, otherwise the Analysis wouldn't work
                                                 // 该物品可堆叠，否则分析无法进行。
            Item.width = 8;
            Item.height = 8;
        }

        public override void SaveData(TagCompound tag)
        {
            tag["durability"] = durability;
        }

        public override void LoadData(TagCompound tag)
        {
            durability = tag.Get<float>("durability");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(durability);
        }

        public override void NetReceive(BinaryReader reader)
        {
            durability = reader.ReadSingle();
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (durability == 1)
            {
                return;
            }

            Vector2 spriteSize = frame.Size() * scale;

            spriteBatch.Draw(TextureAssets.MagicPixel.Value,
                position: new Vector2(position.X, position.Y + spriteSize.Y * 0.9f),
                sourceRectangle: new Rectangle(0, 0, 1, 1),
                Color.LightGreen,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: new Vector2(spriteSize.X * durability, 2f),
                SpriteEffects.None,
                layerDepth: 0f);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "AnalysisStackableDurabilityItem", $"Durability: {(int)(durability * 100)}%") { OverrideColor = Color.LightGreen });
        }

        private static float WeightedAverage(float durability1, int stack1, float durability2, int stack2)
        {
            return (durability1 * stack1 + durability2 * stack2) / (stack1 + stack2);
        }

        public override void OnStack(Item source, int numToTransfer)
        {
            var incomingDurability = ((AnalysisStackableDurabilityItem)source.ModItem).durability;
            durability = WeightedAverage(durability, Item.stack, incomingDurability, numToTransfer);
        }

        //SplitStack:  This Analysis does not need to use SplitStack because durability will be the intended value from being cloned.
        // SplitStack：此分析不需要使用SplitStack，因为从克隆中获得的预期值将是耐久度。

        public override void OnCreated(ItemCreationContext context)
        {
            if (context is RecipeItemCreationContext)
            {
                durability = Main.rand.NextFloat();
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AnalysisItem>()
                .AddTile<Tiles.Furniture.AnalysisWorkbench>()
                .Register();
        }
    }
}
