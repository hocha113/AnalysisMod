using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Armor
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Head value here will result in TML expecting a X_Head.png file to be placed next to the item's main texture.

    // AutoloadEquip属性会自动将装备纹理附加到该物品。
    // 在此处提供EquipType.Head值将导致TML期望在物品的主纹理旁边放置一个X_Head.png文件。
    [AutoloadEquip(EquipType.Head)]
    public class AnalysisHood : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
                                                  // 该物品价值多少硬币

            Item.rare = ItemRarityID.Green; // The rarity of the item
                                            // 该物品的稀有度

            Item.defense = 4; // The amount of defense the item will give when equipped
                              // 当装备时，该物品提供的防御量
        }

        // IsArmorSet determines what armor pieces are needed for the setbonus to take effect
        // IsArmorSet确定了套装效果所需的盔甲部件
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<AnalysisBreastplate>() && legs.type == ModContent.ItemType<AnalysisLeggings>();
        }

        // UpdateArmorSet allows you to give set bonuses to the armor.
        // UpdateArmorSet允许您为盔甲提供套装加成。
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "reduces mana cost by 10%";  // This is the setbonus tooltip
                                                           // 这是套装加成工具提示

            player.manaCost -= 0.1f; // Reduces mana cost by 10%
                                     // 减少10%法力消耗
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
