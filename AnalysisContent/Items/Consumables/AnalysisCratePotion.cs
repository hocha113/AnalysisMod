using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Consumables
{
    public class AnalysisCratePotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;

            // Dust that will appear in these colors when the item with ItemUseStyleID.DrinkLiquid is used
            // 当使用ItemUseStyleID.DrinkLiquid的物品时，会出现这些颜色的尘埃
            ItemID.Sets.DrinkParticleColors[Type] = new Color[3] {
                new Color(240, 240, 240),
                new Color(200, 200, 200),
                new Color(140, 140, 140)
            };
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 26;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item3;
            Item.maxStack = 30;
            Item.consumable = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(silver: 8);
            Item.buffType = ModContent.BuffType<Buffs.AnalysisCrateBuff>(); // Specify an existing buff to be applied when used.
                                                                            // 指定在使用时应用的现有增益效果。

            Item.buffTime = 3 * 60 * 60; // The amount of time the buff declared in Item.buffType will last in ticks. Set to 3 minutes, as 60 ticks = 1 second.
                                         // 在Item.buffType中声明的增益效果持续时间，以tick为单位。设置为3分钟，因为60个ticks = 1秒。
        }

        // Please see AnalysisContent/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CratePotion, 4)
                .AddTile(TileID.CrystalBall)
                .Register();
        }
    }
}
