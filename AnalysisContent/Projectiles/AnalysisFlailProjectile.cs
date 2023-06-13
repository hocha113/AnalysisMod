using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using AnalysisMod.AnalysisContent.Dusts;
using Terraria.GameContent;
using ReLogic.Content;

namespace AnalysisMod.AnalysisContent.Projectiles
{
    // AnalysisFlail and AnalysisFlailProjectile show the minimum amount of code needed for a flail using the existing vanilla code and behavior. AnalysisAdvancedFlail and AnalysisAdvancedFlailProjectile need to be consulted if more advanced customization is desired, or if you want to learn more advanced modding techniques.
    // AnalysisFlailProjectile is a copy of the Sunfury flail projectile.

    // AnalysisFlail和AnalysisFlailProjectile展示了使用现有的基础代码和行为所需的最少量代码。如果需要更高级的自定义，或者想要学习更高级的modding技术，则需要参考AnalysisAdvancedFlail和AnalysisAdvancedFlailProjectile。
    // AnalysisFlailProjectile是Sunfury链球弹幕的复制品。
    internal class AnalysisFlailProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true; // This ensures that the projectile is synced when other players join the world.
                                            // 这确保了其他玩家加入世界时弹幕同步。

            Projectile.width = 22; // The width of your projectile
                                   // 你的弹幕宽度

            Projectile.height = 22; // The height of your projectile
                                    // 你的弹幕高度

            Projectile.friendly = true; // Deals damage to enemies
                                        // 对敌人造成伤害

            Projectile.penetrate = -1; // Infinite pierce
                                       // 无限穿透

            Projectile.DamageType = DamageClass.Melee; // Deals melee damage
                                                       // 近战伤害
            Projectile.scale = 0.8f;
            Projectile.usesLocalNPCImmunity = true; // Used for hit cooldown changes in the ai hook
                                                    // 用于ai hook中打击冷却时间变化

            Projectile.localNPCHitCooldown = 10; // This facilitates custom hit cooldown logic
                                                 // 这有助于定制打击冷却逻辑

            // Here we reuse the flail projectile aistyle and set the aitype to the Sunfury. These lines will get our projectile to behave exactly like Sunfury would. This only affects the AI code, you'll need to adapt other code for the other behaviors you wish to use.
            // 在此处我们重用链球弹幕aistyle并将aitype设置为Sunfury。这些行将使我们的弹丸像Sunfury一样运作。这仅影响AI代码，您需要调整其他代码以使用其他所需行为。
            Projectile.aiStyle = ProjAIStyleID.Flail;
            AIType = ProjectileID.Sunfury;

            // These help center the projectile as it rotates since its hitbox and scale doesn't match the actual texture size
            // 这些帮助在旋转时使投射物居中，因为其碰撞箱和比例不匹配实际纹理大小。
            DrawOffsetX = -6;
            DrawOriginOffsetY = -6;
        }

        // All of the following methods are additional behaviors of Sunfury that are not automatically inherited by AnalysisFlailProjectile through the use of Projectile.aiStyle and AIType. You'll need to find corresponding code in the decompiled source code if you wish to clone a different vanilla projectile as a starting point.
        // 以下所有方法都是Sunfury具有但通过使用Projectile.aiStyle和AIType不能自动继承到AnalysisFlailProjectile中去除外部源码之外找到相应代码之外没有别的方法可以克隆一个不同类型原始投射物作为起点.

        // Draw the projectile in full brightness, ignoring lighting conditions.
        // 在完全亮度下绘制弹丸，忽略照明条件。
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        // In PreDrawExtras, we trick the game into thinking the projectile is actually a Sunfury projectile. After PreDrawExtras, the Terraria code will draw the chain. Drawing the chain ourselves is quite complicated, AnalysisAdvancedFlailProjectile has an Analysis of that. Then, in PreDraw, we restore the Projectile.type back to normal so we don't break anything.  
        // 在PreDrawExtras中，我们欺骗游戏认为弹丸实际上是Sunfury弹丸。在PreDrawExtras之后，Terraria代码将绘制链条。自己绘制链条非常复杂，AnalysisAdvancedFlailProjectile对此进行了分析。然后，在PreDraw中，我们将Projectile.type恢复到正常状态，以免破坏任何东西。
        public override bool PreDrawExtras()
        {
            Projectile.type = ProjectileID.Sunfury;
            return base.PreDrawExtras();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.type = ModContent.ProjectileType<AnalysisFlailProjectile>();

            // This code handles the after images.
            // 此代码处理残影效果。
            if (Projectile.ai[0] == 1f)
            {
                Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
                Vector2 drawPosition = Projectile.position + new Vector2(Projectile.width, Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
                Vector2 drawOrigin = new Vector2(projectileTexture.Width, projectileTexture.Height) / 2f;
                Color drawColor = Projectile.GetAlpha(lightColor);
                drawColor.A = 127;
                drawColor *= 0.5f;
                int launchTimer = (int)Projectile.ai[1];
                if (launchTimer > 5)
                {
                    launchTimer = 5;
                }

                SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                for (float transparancy = 1f; transparancy >= 0f; transparancy -= 0.125f)
                {
                    float opacity = 1f - transparancy;
                    Vector2 drawAdjustment = Projectile.velocity * -launchTimer * transparancy;
                    Main.EntitySpriteDraw(projectileTexture, drawPosition + drawAdjustment, null, drawColor * opacity, Projectile.rotation, drawOrigin, Projectile.scale * 1.15f * MathHelper.Lerp(0.5f, 1f, opacity), spriteEffects, 0);
                }
            }

            return base.PreDraw(ref lightColor);
        }

        // Another thing that won't automatically be inherited by using Projectile.aiStyle and AIType are effects that happen when the projectile hits something. Here we see the code responcible for applying the OnFire debuff to players and enemies.
        // 另一件不会通过使用Projectile.aiStyle和AIType自动继承的事情是当投射物击中某物时发生的效果。这里我们看到负责向玩家和敌人应用OnFire debuff的代码。
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(2))
            {
                target.AddBuff(BuffID.OnFire, 300);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Main.rand.NextBool(4))
            {
                target.AddBuff(BuffID.OnFire, 180, quiet: false);
            }
        }

        // Finally, you can slightly customize the AI if you read and understand the vanilla aiStyle source code. You can't customize the range, retract speeds, or anything else. If you need to customize those things, you'll need to follow AnalysisAdvancedFlailProjectile. This Analysis spawns a Grenade right when the flail starts to retract. 
        // 最后，如果您阅读并理解基础aiStyle源代码，则可以稍微定制AI。您无法自定义范围、缩回速度或其他任何内容。如果需要自定义这些内容，则需要遵循AnalysisAdvancedFlailProjectile.该分析在链球开始收缩时生成一个手榴弹.
        public override void AI()
        {
            // The only reason this code works is because the author read the vanilla code and comprehended it well enough to tack on additional logic.
            // 这段代码之所以有效是因为作者阅读了基础代码，并足够理解它来添加额外逻辑。
            if (Main.myPlayer == Projectile.owner && Projectile.ai[0] == 2f && Projectile.ai[1] == 0f)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ProjectileID.Grenade, Projectile.damage, Projectile.knockBack, Main.myPlayer);
                Projectile.ai[1]++;
            }
        }
    }
}
