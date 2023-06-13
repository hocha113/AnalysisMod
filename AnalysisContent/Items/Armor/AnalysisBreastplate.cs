using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Armor
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Body value here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png sprite-sheet files to be placed next to the item's main texture.

    // AutoloadEquip属性会自动将装备纹理附加到该物品上。
    // 在此处提供EquipType.Body值将导致TML期望在物品的主纹理旁边放置X_Arms.png、X_Body.png和X_FemaleBody.png精灵表文件。
    [AutoloadEquip(EquipType.Body)]
    public class AnalysisBreastplate : ModItem
    {
        public static int MaxManaIncrease = 20;
        public static int MaxMinionIncrease = 1;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MaxManaIncrease, MaxMinionIncrease);

        public override void SetDefaults()
        {
            Item.width = 18; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
                                                  // 该物品价值多少硬币

            Item.rare = ItemRarityID.Green; // The rarity of the item
                                            // 该物品的稀有度

            Item.defense = 6; // The amount of defense the item will give when equipped
                              // 当装备时，该物品提供的防御值数量
        }

        public override void UpdateEquip(Player player)
        {
            player.buffImmune[BuffID.OnFire] = true; // Make the player immune to Fire
                                                     // 使玩家免疫火焰伤害

            player.statManaMax2 += MaxManaIncrease; // Increase how many mana points the player can have by 20
                                                    // 增加玩家可拥有的魔力点数20个

            player.maxMinions += MaxMinionIncrease; // Increase how many minions the player can have by one
                                                    // 增加玩家可拥有的随从数量1个
        }

        // Please see AnalysisContent/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<AnalysisItem>()
                .AddTile<Tiles.Furniture.AnalysisWorkbench>()
                .Register();
        }
    }
}
