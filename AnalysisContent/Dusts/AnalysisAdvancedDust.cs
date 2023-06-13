using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Dusts
{
    // This Dust will show off Dust.customData, using vanilla dust texture, and some neat movement.
    // 这个 Dust 将展示 Dust.customData，使用原版的粉尘纹理和一些有趣的运动。
    internal class AnalysisAdvancedDust : ModDust
    {
        /*
			Spawning this dust is a little more involved because we need to assign a rotation, customData, and fix the position.
			Position must be fixed here because otherwise the first time the dust is drawn it'll draw in the incorrect place.
			This dust is not used in AnalysisMod yet, so you'll have to add some code somewhere. Try AnalysisPlayer.DrawEffects.

            生成这个粉尘需要更多步骤，因为我们需要分配旋转、customData 和修正位置。
            在这里必须固定位置，否则第一次绘制时它会绘制在错误的地方。
            此粉尘目前未用于 AnalysisMod 中，因此您需要在某处添加一些代码。试试 AnalysisPlayer.DrawEffects。

			Dust dust = Dust.NewDustDirect(Player.Center, 0, 0, ModContent.DustType<Content.Dusts.AdvancedDust>(), Scale: 2);
			dust.rotation = Main.rand.NextFloat(6.28f);
			dust.customData = Player;
			dust.position = Player.Center + Vector2.UnitX.RotatedBy(dust.rotation, Vector2.Zero) * dust.scale * 50;
		*/
        public override string Texture => null; // If we want to use vanilla texture
                                                // 如果我们想要使用原版纹理

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;

            // Since the vanilla dust texture has all the dust in 1 file, we'll need to do some math.
            // If you want to use a vanilla dust texture, you can copy and paste it, changing the desiredVanillaDustTexture

            // 由于原版粉尘纹理将所有粉尘放入一个文件中，所以我们需要进行一些数学计算。
            // 如果您想使用原版粉尘纹理，则可以复制并粘贴它，并更改 desiredVanillaDustTexture。
            int desiredVanillaDustTexture = 139;
            int frameX = desiredVanillaDustTexture * 10 % 1000;
            int frameY = desiredVanillaDustTexture * 10 / 1000 * 30 + Main.rand.Next(3) * 10;
            dust.frame = new Rectangle(frameX, frameY, 8, 8);

            dust.velocity = Vector2.Zero;
        }

        // This Update method shows off some interesting movement. Using customData assigned to a Player, we spiral around the Player while slowly getting closer. In practice, it looks like a vortex.
        // 此 Update 方法展示了一些有趣的运动。使用分配给 Player 的 customData，在缓慢靠近玩家时围绕玩家螺旋形移动。实际上看起来像一个漩涡。
        public override bool Update(Dust dust)
        {
            // Here we rotate and scale down the dust. The dustIndex % 2 == 0 part lets half the dust rotate clockwise and the other half counter clockwise
            // 在这里我们旋转并缩小了灰尘。dustIndex % 2 == 0 部分让半数灰尘顺时针旋转，另外半数逆时针旋转
            dust.rotation += 0.1f * (dust.dustIndex % 2 == 0 ? -1 : 1);
            dust.scale -= 0.05f;

            // Here we use the customData field. If customData is the type we expect, Player, we do some special movement.
            // 在这里我们使用 customData 字段。如果 customData 是预期类型 Player，则执行特殊移动操作。
            if (dust.customData != null && dust.customData is Player player)
            {
                // Here we assign position to some offset from the player that was assigned. This offset scales with dust.scale. The scale and rotation cause the spiral movement we desired.
                // 在这里我们将位置分配给从已分配的玩家偏移量。该偏移量随着 dust.scale 缩放。缩放和旋转导致了我们所期望的螺旋运动。
                dust.position = player.Center + Vector2.UnitX.RotatedBy(dust.rotation, Vector2.Zero) * dust.scale * 50;
            }

            // Here we make sure to kill any dust that get really small.
            // 在这里，我们确保杀死任何变得非常小的灰尘。
            if (dust.scale < 0.25f)
                dust.active = false;

            return false;
        }
    }
}
