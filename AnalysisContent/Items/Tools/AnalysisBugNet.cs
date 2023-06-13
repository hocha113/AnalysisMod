using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Tools
{
    // This is an Analysis bug net designed to demonstrate the use cases for various hooks related to catching NPCs such as critters with items.
    // 这是一个捕虫网分析案例，旨在演示与使用物品捕获NPC（如小动物）相关的各种钩子的用例。
    public class AnalysisBugNet : ModItem
    {
        public static readonly int LavaCatchChance = 20;
        public static readonly int WarmthLavaCatchChance = 50;
        public static readonly int BonusCritterChance = 5;

        public override string Texture => $"Terraria/Images/Item_{ItemID.BugNet}";

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(LavaCatchChance, WarmthLavaCatchChance, BonusCritterChance);

        public override void SetStaticDefaults()
        {
            // This set is needed to define an item as a tool for catching NPCs at all.
            // An additional set exists called LavaproofCatchingTool which will allow your item to freely catch the Underworld's lava critters. Use it accordingly.

            // 这个集合需要定义一个工具来捕获NPC。还有另外一组叫做LavaproofCatchingTool，
            // 它将允许你的物品自由地捕获地狱中的岩浆生物。请根据需要使用。
            ItemID.Sets.CatchingTool[Item.type] = true;
        }

        public override void SetDefaults()
        {
            // These are, with a few modifications, the properties applied to the base Bug Net; they're provided here so that you can mess with them as you please.
            // Explanations on them will be glossed over here, as they're not the primary point of the lesson.

            // 这些是基本Bug Net应用了一些修改后的属性；这里提供它们以便您随意更改。
            // 在此不会详细解释它们，因为它们不是本课程的重点。

            // Common Properties
            // 常见属性
            Item.width = 24;
            Item.height = 28;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(0, 0, 40);

            // Use Properties
            // 使用属性
            Item.useAnimation = 25;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
        }

        public override bool? CanCatchNPC(NPC target, Player player)
        {
            // This hook is used to determine whether or not your catching tool can catch a given NPC.
            // This returns null by default, which allows vanilla to decide whether or not the NPC should be caught.
            // Returning true forces the NPC to be caught, while returning false forces the NPC to not be caught.
            // If you're unsure what to return, return null.
            // For this Analysis, we'll give our Analysis bug net a 20% chance to catch lava critters successfully (50% with a Warmth Potion buff active).

            // 此钩子用于确定您的捕获工具是否可以捕获给定NPC。
            // 默认情况下返回null，这允许原版决定是否应该抓住NPC。
            // 返回true强制抓住NPC，而返回false则强制不抓住NPC。
            // 如果您不确定要返回什么，请返回null。
            // 对于此分析，我们将使我们的Analysis bug net成功抓取岩浆生物20％（搭配Warmth Potion buff时为50％）。
            if (ItemID.Sets.IsLavaBait[target.catchItem])
            {
                if (Main.rand.NextBool(player.resistCold ? WarmthLavaCatchChance : LavaCatchChance, 100))
                {
                    return true;
                }
            }

            // For all cases where true isn't explicitly returned, we'll return null so that vanilla catching rules and effects can take place.
            // 对于所有未明确返回true 的情况，我们将返回 null ，以便原版规则和效果能够发挥作用。
            return null;
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

    // This class is included here as a demonstration of how to use OnSpawn to modify the item spawned from catching an NPC or other entity.
    // 此类在此处包含，以演示如何使用OnSpawn修改从捕获NPC或其他实体生成的物品。
    public class AnalysisCatchItemModification : GlobalItem
    {
        public override void OnSpawn(Item item, IEntitySource source)
        {
            if (source is not EntitySource_Caught catchEntity)
            {
                return;
            }

            if (catchEntity.Entity is Player player)
            {
                // Gives a 5% chance for the Analysis Bug Net to duplicate caught NPCs.
                // 给Analysis Bug Net提供了5％的机会来复制已捕获的NPC。
                if (player.HeldItem.type == ModContent.ItemType<AnalysisBugNet>() && Main.rand.NextBool(AnalysisBugNet.BonusCritterChance, 100))
                {
                    item.stack *= 2;
                }
            }
        }
    }
}
