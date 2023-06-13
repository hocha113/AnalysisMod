using AnalysisMod.AnalysisContent.Buffs;
using AnalysisMod.AnalysisContent.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Weapons
{
    public class AnalysisWhipAdvanced : ModItem
    {
        // The texture doesn't have the same name as the item, so this property points to it.
        // 这个属性指向的是纹理名称与物品名称不同。
        public override string Texture => "AnalysisMod/AnalysisContent/Items/Weapons/AnalysisWhip";

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(AnalysisWhipAdvancedDebuff.TagDamagePercent);

        public override void SetDefaults()
        {
            // Call this method to quickly set some of the properties below.
            // 调用此方法可以快速设置下面列出的一些属性。

            //Item.DefaultToWhip(ModContent.ProjectileType<AnalysisWhipProjectileAdvanced>(), 20, 2, 4);

            Item.DamageType = DamageClass.SummonMeleeSpeed;
            Item.damage = 20;
            Item.knockBack = 2;
            Item.rare = ItemRarityID.Green;

            Item.shoot = ModContent.ProjectileType<AnalysisWhipProjectileAdvanced>();
            Item.shootSpeed = 4;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.UseSound = SoundID.Item152;
            Item.channel = true; // This is used for the charging functionality. Remove it if your whip shouldn't be chargeable.
                                 // 这是用于鞭子充能功能。如果你的鞭子不能被充能，请将其删除。
            Item.noMelee = true;
            Item.noUseGraphic = true;
        }

        // Please see AnalysisContent/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AnalysisItem>()
                .AddTile<Tiles.Furniture.AnalysisWorkbench>()
                .Register();
        }

        // Makes the whip receive melee prefixes
        // 使鞭子可以获得近战前缀
        public override bool MeleePrefix()
        {
            return true;
        }
    }
}
