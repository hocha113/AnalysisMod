using System;
using AnalysisMod.AnalysisContent.Tiles;
using AnalysisMod.AnalysisContent.Tiles.Furniture;
using AnalysisMod.AnalysisContent.Walls;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Ammo
{
    public class AnalysisSolutionItem : ModItem
    {
        public override string Texture => AnalysisMod.AssetPath + "Textures/Items/AnalysisSolution";

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
        {
            Item.DefaultToSolution(ModContent.ProjectileType<AnalysisSolutionProjectile>());
            Item.value = Item.buyPrice(0, 0, 25);
            Item.rare = ItemRarityID.Orange;
        }

        // Please see Content/AnalysisRecipes.cs for a detailed explanation of recipe creation.
        //请查看Content/AnalysisRecipes.cs以获取有关配方创建的详细说明。
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AnalysisItem>()
                .AddTile<Tiles.Furniture.AnalysisWorkbench>()
                .Register();
        }
    }

    public class AnalysisSolutionProjectile : ModProjectile
    {
        public override string Texture => AnalysisMod.AssetPath + "Textures/Projectiles/AnalysisSolution";

        public ref float Progress => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            // This method quickly sets the projectile properties to match other sprays.
            //此方法快速设置弹丸属性以匹配其他喷雾
            Projectile.DefaultToSpray();
            Projectile.aiStyle = 0; // Here we set aiStyle back to 0 because we have custom AI code
                                    // 在这里，我们将aiStyle设置回0，因为我们有自定义AI代码
        }

        public override void AI()
        {
            // Set the dust type to AnalysisSolution
            //将粉尘类型设置为AnalysisSolution
            int dustType = ModContent.DustType<Dusts.AnalysisSolution>();

            if (Projectile.owner == Main.myPlayer)
            {
                Convert((int)(Projectile.position.X + Projectile.width * 0.5f) / 16, (int)(Projectile.position.Y + Projectile.height * 0.5f) / 16, 2);
            }

            if (Projectile.timeLeft > 133)
            {
                Projectile.timeLeft = 133;
            }

            if (Progress > 7f)
            {
                float dustScale = 1f;

                if (Progress == 8f)
                {
                    dustScale = 0.2f;
                }
                else if (Progress == 9f)
                {
                    dustScale = 0.4f;
                }
                else if (Progress == 10f)
                {
                    dustScale = 0.6f;
                }
                else if (Progress == 11f)
                {
                    dustScale = 0.8f;
                }

                Progress += 1f;


                var dust = Dust.NewDustDirect(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100);

                dust.noGravity = true;
                dust.scale *= 1.75f;
                dust.velocity.X *= 2f;
                dust.velocity.Y *= 2f;
                dust.scale *= dustScale;
            }
            else
            {
                Progress += 1f;
            }

            Projectile.rotation += 0.3f * Projectile.direction;
        }

        private static void Convert(int i, int j, int size = 4)
        {
            for (int k = i - size; k <= i + size; k++)
            {
                for (int l = j - size; l <= j + size; l++)
                {
                    if (WorldGen.InWorld(k, l, 1) && Math.Abs(k - i) + Math.Abs(l - j) < Math.Sqrt(size * size + size * size))
                    {
                        int type = Main.tile[k, l].TileType;
                        int wall = Main.tile[k, l].WallType;

                        // Convert all walls to AnalysisWall (or AnalysisWallUnsafe for SpiderUnsafe)
                        //将所有墙壁转换为AnalysisWall（或SpiderUnsafe的AnalysisWallUnsafe）
                        if (wall != 0 && wall != ModContent.WallType<AnalysisWallUnsafe>())
                        {
                            if (wall == WallID.SpiderUnsafe)
                                Main.tile[k, l].WallType = (ushort)ModContent.WallType<AnalysisWallUnsafe>();
                            else
                                Main.tile[k, l].WallType = (ushort)ModContent.WallType<AnalysisWall>();
                            WorldGen.SquareWallFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        // If the tile is stone, convert to AnalysisBlock
                        //如果瓷砖是石头，则转换为AnalysisBlock
                        if (TileID.Sets.Conversion.Stone[type])
                        {
                            Main.tile[k, l].TileType = (ushort)ModContent.TileType<AnalysisBlock>();
                            WorldGen.SquareTileFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        // If the tile is sand, convert to AnalysisSand
                        //如果瓷砖是沙子，则转换为AnalysisSand
                        // else if (TileID.Sets.Conversion.Sand[type]) {
                        // 	Main.tile[k, l].type = (ushort)TileType<AnalysisSand>();
                        // 	WorldGen.SquareTileFrame(k, l);
                        // 	NetMessage.SendTileSquare(-1, k, l, 1);
                        // }
                        // If the tile is a chair, convert to AnalysisChair
                        // 如果该块是椅子，则转换成 AnalysisChair

                        else if (type == TileID.Chairs && Main.tile[k, l - 1].TileType == TileID.Chairs)
                        {
                            Main.tile[k, l].TileType = (ushort)ModContent.TileType<AnalysisChair>();
                            Main.tile[k, l - 1].TileType = (ushort)ModContent.TileType<AnalysisChair>();
                            WorldGen.SquareTileFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }
                        // If the tile is a workbench, convert to AnalysisWorkBench
                        // 如果该块是工作台，则转换成 AnalysisWorkBench
                        else if (type == TileID.WorkBenches && Main.tile[k - 1, l].TileType == TileID.WorkBenches)
                        {
                            Main.tile[k, l].TileType = (ushort)ModContent.TileType<AnalysisWorkbench>();
                            Main.tile[k - 1, l].TileType = (ushort)ModContent.TileType<AnalysisWorkbench>();
                            WorldGen.SquareTileFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }
                    }
                }
            }
        }
    }
}