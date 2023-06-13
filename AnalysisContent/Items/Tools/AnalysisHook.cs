using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Items.Tools
{
    internal class AnalysisHookItem : ModItem
    {
        public override void SetDefaults()
        {
            // Copy values from the Amethyst Hook
            //从紫水晶钩复制数值
            Item.CloneDefaults(ItemID.AmethystHook);
            Item.shootSpeed = 18f; // This defines how quickly the hook is shot.
                                   // 这定义了钩子发射的速度。

            Item.shoot = ModContent.ProjectileType<AnalysisHookProjectile>(); // Makes the item shoot the hook's projectile when used.
                                                                              // 使用时使物品发射钩子弹。

            // If you do not use Item.CloneDefaults(), you must set the following values for the hook to work properly:
            //如果您不使用Item.CloneDefaults()，则必须为钩子设置以下值才能正常工作：

            // Item.useStyle = ItemUseStyleID.None;
            // Item.useTime = 0;
            // Item.useAnimation = 0;
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

    internal class AnalysisHookProjectile : ModProjectile
    {
        private static Asset<Texture2D> chainTexture;

        public override void Load()
        { // This is called once on mod (re)load when this piece of content is being loaded.
          // This is the path to the texture that we'll use for the hook's chain. Make sure to update it.

          //当加载此内容片段时，在mod（重新）加载上调用一次。
          //这是我们将用于挂钩链的纹理路径。确保更新它。
            chainTexture = ModContent.Request<Texture2D>("AnalysisMod/AnalysisContent/Items/Tools/AnalysisHookChain");
        }

        public override void Unload()
        { // This is called once on mod reload when this piece of content is being unloaded.
          // It's currently pretty important to unload your static fields like this, to avoid having parts of your mod remain in memory when it's been unloaded.

          //在mod重新加载时，当卸载此内容片段时会调用一次。
          //目前非常重要的是像这样卸载静态字段，以避免在卸载模块后仍然保留模块部分内存。
            chainTexture = null;
        }

        /*
		public override void SetStaticDefaults() {
			// If you wish for your hook projectile to have ONE copy of it PER player, uncomment this section.
			ProjectileID.Sets.SingleGrappleHook[Type] = true;
		}
		*/

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.GemHookAmethyst); // Copies the attributes of the Amethyst hook's projectile.
                                                                    // 复制紫水晶勾项目的属性。
        }

        // Use this hook for hooks that can have multiple hooks mid-flight: Dual Hook, Web Slinger, Fish Hook, Static Hook, Lunar Hook.
        //对于可以在飞行中具有多个挂钩的挂钩，请使用此挂钩：双重勾、网投手、鱼勾、静态勾和月亮勾等。
        public override bool? CanUseGrapple(Player player)
        {
            int hooksOut = 0;
            for (int l = 0; l < 1000; l++)
            {
                if (Main.projectile[l].active && Main.projectile[l].owner == Main.myPlayer && Main.projectile[l].type == Projectile.type)
                {
                    hooksOut++;
                }
            }

            return hooksOut <= 2;
        }

        // Use this to kill oldest hook. For hooks that kill the oldest when shot, not when the newest latches on: Like SkeletronHand
        // You can also change the projectile like: Dual Hook, Lunar Hook

        //用于杀死最老的hook。对于那些在射击而不是新近附着时杀死最老者的掉落物：如SkeletronHand
        //您还可以更改projectile，例如：Dual Hook, Lunar Hook

        // public override void UseGrapple(Player player, ref int type)
        // {
        //	int hooksOut = 0;
        //	int oldestHookIndex = -1;
        //	int oldestHookTimeLeft = 100000;
        //	for (int i = 0; i < 1000; i++)
        //	{
        //		if (Main.projectile[i].active && Main.projectile[i].owner == projectile.whoAmI && Main.projectile[i].type == projectile.type)
        //		{
        //			hooksOut++;
        //			if (Main.projectile[i].timeLeft < oldestHookTimeLeft)
        //			{
        //				oldestHookIndex = i;
        //				oldestHookTimeLeft = Main.projectile[i].timeLeft;
        //			}
        //		}
        //	}
        //	if (hooksOut > 1)
        //	{
        //		Main.projectile[oldestHookIndex].Kill();
        //	}
        // }

        // Amethyst Hook is 300, Static Hook is 600.
        //紫水晶Hook为300，Static Hook为600.
        public override float GrappleRange()
        {
            return 500f;
        }

        public override void NumGrappleHooks(Player player, ref int numHooks)
        {
            numHooks = 2; // The amount of hooks that can be shot out
                          // 可发射出去的hook数量
        }

        // default is 11, Lunar is 24
        //默认值为11，Lunar为24
        public override void GrappleRetreatSpeed(Player player, ref float speed)
        {
            speed = 18f; // How fast the grapple returns to you after meeting its max shoot distance
                         // 达到最大射程后抓取返回你所需时间有多快
        }

        public override void GrapplePullSpeed(Player player, ref float speed)
        {
            speed = 10; // How fast you get pulled to the grappling hook projectile's landing position
                        // 抓钩弹射物着陆位置后你被拉到那里的速度有多快
        }

        // Adjusts the position that the player will be pulled towards. This will make them hang 50 pixels away from the tile being grappled.
        //调整玩家将被拉向的位置。这将使他们离挂钩砖块50像素远。
        public override void GrappleTargetPoint(Player player, ref float grappleX, ref float grappleY)
        {
            Vector2 dirToPlayer = Projectile.DirectionTo(player.Center);
            float hangDist = 50f;
            grappleX += dirToPlayer.X * hangDist;
            grappleY += dirToPlayer.Y * hangDist;
        }

        // Can customize what tiles this hook can latch onto, or force/prevent latching alltogether, like Squirrel Hook also latching to trees
        //可以自定义此挂钩可以附着在哪些瓷砖上，或者强制/防止全部附着，例如松鼠勾还可附着在树上
        public override bool? GrappleCanLatchOnTo(Player player, int x, int y)
        {
            // By default, the hook returns null to apply the vanilla conditions for the given tile position (this tile position could be air or an actuated tile!)
            // If you want to return true here, make sure to check for Main.tile[x, y].HasUnactuatedTile (and Main.tileSolid[Main.tile[x, y].TileType] and/or Main.tile[x, y].HasTile if needed)

            //默认情况下，hook返回null以应用给定瓦片位置的香草条件（该瓦片位置可能是空气或激活的瓦片！）
            //如果您想在此处返回true，请确保检查Main.tile[x, y].HasUnactuatedTile（和Main.tileSolid[Main.tile[x, y].TileType]和/或Main.tile[x, y].HasTile如有必要）

            // We make this hook latch onto trees just like Squirrel Hook
            //我们让这个勾子像松鼠勾一样附着在树上

            // Tree trunks cannot be actuated so we don't need to check for that here
            //树干不能被激活所以我们不需要在这里检查它
            Tile tile = Main.tile[x, y];
            if (TileID.Sets.IsATreeTrunk[tile.TileType] || tile.TileType == TileID.PalmTree)
            {
                return true;
            }

            // In any other case, behave like a normal hook
            //否则，就像普通挂钩一样行事
            return null;
        }

        // Draws the grappling hook's chain.
        //绘制抓取钩链。
        public override bool PreDrawExtras()
        {
            Vector2 playerCenter = Main.player[Projectile.owner].MountedCenter;
            Vector2 center = Projectile.Center;
            Vector2 directionToPlayer = playerCenter - Projectile.Center;
            float chainRotation = directionToPlayer.ToRotation() - MathHelper.PiOver2;
            float distanceToPlayer = directionToPlayer.Length();

            while (distanceToPlayer > 20f && !float.IsNaN(distanceToPlayer))
            {
                directionToPlayer /= distanceToPlayer; // get unit vector
                                                       // 获取单位向量

                directionToPlayer *= chainTexture.Height(); // multiply by chain link length
                                                            // 乘以链节长度

                center += directionToPlayer; // update draw position
                                             // 更新绘制位置

                directionToPlayer = playerCenter - center; // update distance
                                                           // 更新距离

                distanceToPlayer = directionToPlayer.Length();

                Color drawColor = Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16));

                // Draw chain
                // 绘制链条
                Main.EntitySpriteDraw(chainTexture.Value, center - Main.screenPosition,
                    chainTexture.Value.Bounds, drawColor, chainRotation,
                    chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
            }
            // Stop vanilla from drawing the default chain.
            // 阻止默认的链条绘制。
            return false;
        }
    }
}
