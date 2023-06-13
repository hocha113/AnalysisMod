using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Armor
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Head value here will result in TML expecting a X_Head.png file to be placed next to the item's main texture.

    // AutoloadEquip属性会自动将装备纹理附加到该物品上。
    // 在此提供EquipType.Head值将导致TML期望在物品的主纹理旁边放置一个X_Head.png文件。
    [AutoloadEquip(EquipType.Head)]
    public class AnalysisHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            // If your head equipment should draw hair while drawn, use one of the following:
            // 如果您的头部装备在绘制时应该绘制头发，请使用以下之一：

            //根本不画头。用于太空生物面具
            // ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false; // Don't draw the head at all. Used by Space Creature Mask

            // 像戴帽子一样画头发。用于巫师帽
            // ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true; // Draw hair as if a hat was covering the top. Used by Wizards Hat

            // 正常绘制所有头发。用于哑剧面具、太阳镜
            // ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true; // Draw all hair as normal. Used by Mime Mask, Sunglasses

            // ArmorIDs.Head.Sets.DrawBackHair[Item.headSlot] = true;
            // ArmorIDs.Head.Sets.DrawsBackHairWithoutHeadgear[Item.headSlot] = true; 
        }

        public override void SetDefaults()
        {
            Item.width = 18; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
                                                  // 该物品价值多少硬币

            Item.rare = ItemRarityID.Green; // The rarity of the item
                                            // 该物品的稀有程度

            Item.defense = 5; // The amount of defense the item will give when equipped
                              // 当装备时，该道具提供的防御量是多少。
        }

        // IsArmorSet determines what armor pieces are needed for the setbonus to take effect
        // IsArmorSet确定了套装效果所需的盔甲件数。
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<AnalysisBreastplate>() && legs.type == ModContent.ItemType<AnalysisLeggings>();
        }

        // UpdateArmorSet allows you to give set bonuses to the armor.
        // UpdateArmorSet允许您为盔甲提供套装加成。
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Increases dealt damage by 20%"; // This is the setbonus tooltip
                                                               // 这是套装加成工具提示信息

            player.GetDamage(DamageClass.Generic) += 0.2f; // Increase dealt damage for all weapon classes by 20%
                                                           // 增加所有武器类别造成20%伤害
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
