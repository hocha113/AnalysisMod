using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameInput;
using Mono.Cecil;
using System;
using AnalysisMod.Staitd;
using System.Security.Cryptography.X509Certificates;

namespace AnalysisMod.AnalysisContent.Items.Weapons
{
    public class AnalysisSword : ModItem
    {
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.AnalysisMod.hjson file.
        // 此物品的显示名称和工具提示可以在 Localization/en-US_Mods.AnalysisMod.hjson 文件中进行编辑。       

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 11));
        }

        public override void SetDefaults()
        {
            Item.damage = 500;
            Item.DamageType = DamageClass.Melee;
            Item.width = 80;
            Item.height = 80;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.shoot = ProjectileID.PurificationPowder; // For some reason, all the guns in the vanilla source have this.
                                                          // 由于某种原因，原始源代码中的所有枪械都有这个属性
            Item.shootSpeed = 16f; // The speed of the projectile (measured in pixels per frame.)
                                   // 抛射物速度（以像素每帧为单位）
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.DirtBlock, 10);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            MyModPlayer modPlayer = Main.LocalPlayer.GetModPlayer<MyModPlayer>();
            Random random = new Random();
            int randomInt = random.Next(0, 10);

            if (modPlayer.ETR <= 5000)
            {
                type = ProjectileID.EnchantedBeam;
            }
            if (modPlayer.ETR > 5000 && modPlayer.ETR <= 10000)
            {
                type = ProjectileID.DemonScythe;
                modPlayer.ETR -= 20;
            }
            if (modPlayer.ETR > 10000 && modPlayer.ETR <= 15000)
            {
                type = ProjectileID.NightBeam;
                modPlayer.ETR -= 20;
            }
            if (modPlayer.ETR > 15000 && modPlayer.ETR <= 20000)
            {
                type = ProjectileID.LightBeam;
                modPlayer.ETR -= 20;
            }
            if (modPlayer.ETR > 20000 && modPlayer.ETR <= 25000)
            {
                type = ProjectileID.TerraBeam;
                modPlayer.ETR -= 20;
            }
            if (modPlayer.ETR > 25000 && modPlayer.ETR <= 30000)
            {
                type = ProjectileID.Typhoon;
                modPlayer.ETR -= 20;
            }
            if (modPlayer.ETR > 30000 && modPlayer.ETR <= 35000)
            {
                type = ProjectileID.StarWrath;
                modPlayer.ETR -= 20;
            }
            if (modPlayer.ETR > 35000 && modPlayer.ETR <= 40000)
            {
                type = ProjectileID.LunarFlare;
                modPlayer.ETR -= 20;
            }
            if (modPlayer.ETR > 40000 && modPlayer.ETR <= 45000)
            {
                type = ProjectileID.FinalFractal;
                modPlayer.ETR -= 20;
            }
            if (modPlayer.ETR > 45000)
            {
                type = ProjectileID.FirstFractal;
                modPlayer.ETR -= 20;
            }

            //switch (randomInt)
            //{
            //    case 0:
            //        type = ProjectileID.TerraBeam;
            //        break;
            //    case 1:
            //        type = ProjectileID.LightBeam;
            //        break;
            //    case 2:
            //        type = ProjectileID.NightBeam;
            //        break;
            //    case 3:
            //        type = ProjectileID.EnchantedBeam;
            //        break;
            //    case 4:
            //        type = ProjectileID.VampireHeal;
            //        break;
            //    case 5:
            //        type = ProjectileID.Typhoon;
            //        break;
            //    case 6:
            //        type = ProjectileID.StarWrath;
            //        break;
            //    case 7:
            //        type = ProjectileID.LunarFlare;
            //        break;
            //    case 8:
            //        type = ProjectileID.FirstFractal;
            //        break;
            //    case 9:
            //        type = ProjectileID.FinalFractal;
            //        break;
            //}
            //type = ProjectileID.FirstFractal;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            // Draw the periodic glow effect behind the item when dropped in the world (hence PreDrawInWorld)
            //当物品掉落在世界中时（因此需要在PreDrawInWorld中绘制），绘制该物品背后的周期性发光效果
            Texture2D texture = TextureAssets.Item[Item.type].Value;

            Rectangle frame;

            if (Main.itemAnimations[Item.type] != null)
            {
                // In case this item is animated, this picks the correct frame
                //如果该物品是动画的，那么该代码会选择正确的帧
                frame = Main.itemAnimations[Item.type].GetFrame(texture, Main.itemFrameCounter[whoAmI]);
            }
            else
            {
                frame = texture.Frame();
            }

            Vector2 frameOrigin = frame.Size() / 2f;
            Vector2 offset = new Vector2(Item.width / 2 - frameOrigin.X, Item.height - frame.Height);
            Vector2 drawPos = Item.position - Main.screenPosition + frameOrigin + offset;

            float time = Main.GlobalTimeWrappedHourly;
            float timer = Item.timeSinceItemSpawned / 240f + time * 0.04f;

            time %= 4f;
            time /= 2f;

            if (time >= 1f)
            {
                time = 2f - time;
            }

            time = time * 0.5f + 0.5f;

            for (float i = 0f; i < 1f; i += 0.25f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;

                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, new Color(0, 70, 255, 50), rotation, frameOrigin, scale, SpriteEffects.None, 0);
            }

            for (float i = 0f; i < 1f; i += 0.34f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;

                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, new Color(255, 12, 25, 77), rotation, frameOrigin, scale, SpriteEffects.None, 0);
            }

            return true;
        }
    }
}