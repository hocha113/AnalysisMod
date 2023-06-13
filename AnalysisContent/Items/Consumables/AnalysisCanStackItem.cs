using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AnalysisMod.AnalysisContent.Items.Consumables
{
    // This showcases how the CanStack hook can be used in conjunction with custom data
    // Custom data is also shown in AnalysisDataItem, but here we need to use more hooks

    // 这展示了如何将CanStack钩子与自定义数据结合使用
    // 自定义数据也显示在AnalysisDataItem中，但这里需要使用更多的钩子

    // This item, when crafted, stores the players name, and only lets other players open it. Bags with the same stored name aren't stackable
    // 当制作此物品时，它会存储玩家名称，并且只允许其他玩家打开它。具有相同存储名称的袋子不能堆叠
    public class AnalysisCanStackItem : ModItem
    {
        // We set this when the item is crafted. In other contexts, this will be an empty string
        // 我们在制作物品时设置这个。在其他情况下，这将是一个空字符串。
        public string craftedPlayerName = string.Empty;

        public override void SetDefaults()
        {
            Item.maxStack = Item.CommonMaxStack; // This item is stackable, otherwise the Analysis wouldn't work
                                                 // 该物品可堆叠，否则分析无法工作。

            Item.consumable = true;
            Item.width = 22;
            Item.height = 26;
            Item.rare = ItemRarityID.Blue;
        }

        public override bool CanRightClick()
        {
            // The bag can't be opened if it wasn't crafted
            // 如果未经过制作，则无法打开袋子。
            if (craftedPlayerName == string.Empty)
            {
                return false;
            }

            // The bag can't be opened by the player who crafted it
            // 制造该物品的玩家无法打开袋子。
            return Main.LocalPlayer.name != craftedPlayerName;
        }

        public override bool CanStack(Item source)
        {
            // The bag can only be stacked with other bags if the names match
            // 只有当名称匹配时，才能将该包与其他包堆叠

            // We have to cast the second item to the class (This is safe to do as the hook is only called on items of the same type)
            // 我们必须将第二个项目转换为类（因为钩子仅在相同类型的项目上调用，所以这样做是安全的）
            var name1 = craftedPlayerName;
            var name2 = ((AnalysisCanStackItem)source.ModItem).craftedPlayerName;

            // let items which have been spawned in and not assigned to a player, to stack with other bags the the current player owns
            // This lets you craft multiple items into the mouse-held stack

            // 让已生成但尚未分配给玩家的物品与当前玩家拥有的其他包一起堆叠
            // 这使您可以将多个项目制成鼠标持有栈
            if (name1 == string.Empty)
            {
                name1 = Main.LocalPlayer.name;
            }
            if (name2 == string.Empty)
            {
                name2 = Main.LocalPlayer.name;
            }

            return name1 == name2;
        }

        public override void OnStack(Item source, int numToTransfer)
        {
            // Combined with CanStack above, this ensures that empty spawned items can combine with bags made by the current player
            // 结合上面的CanStack，确保空生成项可以与当前播放器创建的包组合
            if (craftedPlayerName == string.Empty)
            {
                craftedPlayerName = ((AnalysisCanStackItem)source.ModItem).craftedPlayerName;
            }
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            LeadingConditionRule hardmodeCondition = new(new Conditions.IsHardmode());
            hardmodeCondition.OnSuccess(ItemDropRule.Common(ItemID.ChocolateChipCookie));
            hardmodeCondition.OnFailedConditions(ItemDropRule.Common(ItemID.Coconut));
            itemLoot.Add(hardmodeCondition);
        }

        // The following 4 hooks are needed if your item data should be persistent between saves, and work in multiplayer
        // 如果您希望项目数据在保存之间保持持久并在多人游戏中工作，则需要以下4个挂钩
        public override void SaveData(TagCompound tag)
        {
            tag.Add("craftedPlayerName", craftedPlayerName);
        }

        public override void LoadData(TagCompound tag)
        {
            craftedPlayerName = tag.GetString("craftedPlayerName");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(craftedPlayerName);
        }

        public override void NetReceive(BinaryReader reader)
        {
            craftedPlayerName = reader.ReadString();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (craftedPlayerName != string.Empty)
            {
                // Here we make a distinction to disclose that the bag can't be opened by the player who crafted it
                // 在此处我们进行区分以披露制造该包的玩家无法打开它
                if (Main.LocalPlayer.name == craftedPlayerName)
                {
                    tooltips.Add(new TooltipLine(Mod, "CraftedPlayerNameCannotOpen", $"You crafted this bag and cannot open it!"));
                }
                else
                {
                    tooltips.Add(new TooltipLine(Mod, "CraftedPlayerNameOther", $"This is a bag from {craftedPlayerName}, open it to receive a gift!"));
                }
            }
            else
            {
                tooltips.Add(new TooltipLine(Mod, "CraftedPlayerNameEmpty", $"This bag was not crafted, it will do nothing"));
            }
        }

        public override void OnCreated(ItemCreationContext context)
        {
            if (context is RecipeItemCreationContext)
            {
                // If the item was crafted, store the crafting players name
                // 如果已制作物品，请存储制作玩家的名称
                craftedPlayerName = Main.LocalPlayer.name;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AnalysisItem>(20).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
