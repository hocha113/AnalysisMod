using AnalysisMod.AnalysisContent.DamageClasses;
using AnalysisMod.AnalysisContent.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Weapons
{
    public class AnalysisCustomDamageWeapon : ModItem
    {
        public override string Texture => "AnalysisMod/Assets/ItemsVorGet/AdminiumSword_Three"; //TODO: remove when sprite is made for this
                                                                                                       //待办事项：当精灵图为此制作时，请删除

        public override void SetDefaults()
        {
            Item.DamageType = ModContent.GetInstance<AnalysisDamageClass>(); // Makes our item use our custom damage type.
                                                                             // 使我们的物品使用自定义伤害类型。
            Item.width = 40;
            Item.height = 40;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.autoReuse = true;
            Item.damage = 70;
            Item.knockBack = 4;
            Item.crit = 6;
            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AnalysisItem>(20)
                .AddTile<AnalysisWorkbench>()
                .Register();
        }
    }
}
