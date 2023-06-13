using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Consumables
{
    public class AnalysisHealingPotion : ModItem
    {
        public static LocalizedText RestoreLifeText { get; private set; }

        public override void SetStaticDefaults()
        {
            RestoreLifeText = this.GetLocalization(nameof(RestoreLifeText));

            Item.ResearchUnlockCount = 30;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 26;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item3;
            Item.maxStack = 30;
            Item.consumable = true;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 1);

            Item.healLife = 100; // While we change the actual healing value in GetHealLife, Item.healLife still needs to be higher than 0 for the item to be considered a healing item
                                 // 虽然我们在GetHealLife中更改了实际治疗值，但Item.healLife仍需要大于0才能被视为治疗物品。

            Item.potion = true; // Makes it so this item applies potion sickness on use and allows it to be used with quick heal
                                // 使得该物品在使用时会导致药水疾病，并允许它与快速治愈一起使用。
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // Find the tooltip line that corresponds to 'Heals ... life'
            // See https://tmodloader.github.io/tModLoader/html/class_terraria_1_1_mod_loader_1_1_tooltip_line.html for a list of vanilla tooltip line names

            // 找到与“治疗...生命”相对应的工具提示行
            // 请参阅 https://tmodloader.github.io/tModLoader/html/class_terraria_1_1_mod_loader_1_1_tooltip_line.html 以获取原始工具提示行名称列表
            TooltipLine line = tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "HealLife");

            if (line != null)
            {
                // Change the text to 'Heals max/2 (max/4 when quick healing) life'
                // 将文本更改为“治疗最大值/2（快速治愈时为最大值/4）生命”
                line.Text = Language.GetTextValue("CommonItemTooltip.RestoresLife", RestoreLifeText.Format(Main.LocalPlayer.statLifeMax2 / 2, Main.LocalPlayer.statLifeMax2 / 4));
            }
        }

        public override void GetHealLife(Player player, bool quickHeal, ref int healValue)
        {
            // Make the item heal half the player's max health normally, or one fourth if used with quick heal
            // 让物品在正常情况下恢复玩家最大生命值的一半，或者如果使用快速治愈，则恢复四分之一
            healValue = player.statLifeMax2 / (quickHeal ? 4 : 2);
        }

        // Please see AnalysisContent/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AnalysisItem>()
                .AddTile(TileID.Bottles) // Making this recipe be crafted at bottles will automatically make Alchemy Table's effect apply to its ingredients.
                                         // 将此配方制作成药水瓶会自动使魔法药台的效果适用于其材料。
                .Register();
        }
    }
}