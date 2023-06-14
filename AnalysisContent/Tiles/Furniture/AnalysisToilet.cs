using AnalysisMod.AnalysisContent.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AnalysisMod.AnalysisContent.Tiles.Furniture
{
    //Very similar to AnalysisChair, but has special HitWire code and potentially additional AdjTiles
    //与AnalysisChair非常相似，但具有特殊的HitWire代码和可能的附加AdjTiles
    public class AnalysisToilet : ModTile
    {
        public const int NextStyleHeight = 40; // Calculated by adding all CoordinateHeights + CoordinatePaddingFix.Y applied to all of them + 2
                                               // 通过将所有CoordinateHeights + CoordinatePaddingFix.Y应用于它们中的所有坐标+ 2来计算

        public override void SetStaticDefaults()
        {
            // Properties
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.CanBeSatOnForNPCs[Type] = true; // Facilitates calling ModifySittingTargetInfo for NPCs
                                                        // 方便为NPC调用ModifySittingTargetInfo

            TileID.Sets.CanBeSatOnForPlayers[Type] = true; // Facilitates calling ModifySittingTargetInfo for Players
                                                           // 方便为玩家调用ModifySittingTargetInfo

            TileID.Sets.DisableSmartCursor[Type] = true;

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);

            DustType = ModContent.DustType<Sparkle>();
            AdjTiles = new int[] { TileID.Toilets }; // Condider adding TileID.Chairs to AdjTiles to mirror "(regular) Toilet" and "Golden Toilet" behavior for crafting stations
                                                     // 考虑添加TileID.Chairs到AdjTiles以镜像“(普通)厕所”和“黄金马桶”的行为以供制作工作站使用。

            // Names
            AddMapEntry(new Color(200, 200, 200), Language.GetText("MapObject.Toilet"));

            // Placement
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
            TileObjectData.newTile.CoordinatePaddingFix = new Point16(0, 2);
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            // The following 3 lines are needed if you decide to add more styles and stack them vertically
            // 如果您决定添加更多样式并将它们垂直堆叠，则需要以下3行。
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleHorizontal = true;

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1); // Facing right will use the second texture style
                                            // 面向右将使用第二个纹理风格
            TileObjectData.addTile(Type);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        {
            return settings.player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance); // Avoid being able to trigger it from long range
                                                                                                                  // 避免能够从远处触发它
        }

        public override void ModifySittingTargetInfo(int i, int j, ref TileRestingInfo info)
        {
            // It is very important to know that this is called on both players and NPCs, so do not use Main.LocalPlayer for Analysis, use info.restingEntity
            //非常重要的是要知道这会在玩家和NPC上都被调用，因此不要使用Main.LocalPlayer进行分析，请使用info.restingEntity。
            Tile tile = Framing.GetTileSafely(i, j);

            //info.directionOffset = info.restingEntity is Player ? 6 : 2; // Default to 6 for players, 2 for NPCs
                                                                           // 默认情况下，对于玩家为6，对于NPC为2.
            //info.visualOffset = Vector2.Zero; // Defaults to (0,0)

            info.TargetDirection = -1;

            if (tile.TileFrameX != 0)
            {
                info.TargetDirection = 1; // Facing right if sat down on the right alternate (added through addAlternate in SetStaticDefaults earlier)
                                          // 如果坐在右边交替（通过SetStaticDefaults中的addAlternate添加），则面向右。
            }

            // The anchor represents the bottom-most tile of the chair. This is used to align the entity hitbox
            // Since i and j may be from any coordinate of the chair, we need to adjust the anchor based on that

            //锚点表示椅子最底部的瓷砖。这用于对齐实体碰撞箱
            //由于i和j可以来自椅子的任何坐标，因此我们需要根据那个调整锚点
            info.AnchorTilePosition.X = i; // Our chair is only 1 wide, so nothing special required
                                           // 我们的椅子只有1个单位宽度，因此不需要特别处理。

            info.AnchorTilePosition.Y = j;

            if (tile.TileFrameY % NextStyleHeight == 0)
            {
                info.AnchorTilePosition.Y++; // Here, since our chair is only 2 tiles high, we can just check if the tile is the top-most one, then move it 1 down
                                             // 在这里，由于我们的椅子只有2个瓷砖高，所以我们可以检查该瓷砖是否是最顶部的一个，然后将其向下移动1个位置。
            }

            // Here we add a custom fun effect to this tile that vanilla toilets do not have. This shows how you can type cast the restingEntity to Player and use visualOffset as well.
            // 在此处为此瓷砖添加自定义fun效果，而香草厕所没有。这展示了如何将restingEntity强制转换为Player并使用visualOffset。
            if (info.RestingEntity is Player player && player.HasBuff(BuffID.Stinky))
            {
                info.VisualOffset = Main.rand.NextVector2Circular(2, 2);
            }
        }

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;

            if (player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
            {   // Avoid being able to trigger it from long range
                // 避免能够从远距离触发它
                player.GamepadEnableGrappleCooldown();
                player.sitting.SitDown(player, i, j);
            }

            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;

            if (!player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
            { // Match condition in RightClick. Interaction should only show if clicking it does something
              // 在RightClick中匹配条件。交互应仅在单击时执行某些操作时显示
                return;
            }

            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<Items.Placeable.Furniture.AnalysisToilet>();

            if (Main.tile[i, j].TileFrameX / 18 < 1)
            {
                player.cursorItemIconReversed = true;
            }
        }

        public override void HitWire(int i, int j)
        {
            // Spawn the toilet effect here when triggered by a signal
            // 当通过信号触发时，在此处生成马桶效果
            Tile tile = Main.tile[i, j];

            int spawnX = i;
            int spawnY = j - tile.TileFrameY % NextStyleHeight / 18;

            Wiring.SkipWire(spawnX, spawnY);
            Wiring.SkipWire(spawnX, spawnY + 1);

            if (Wiring.CheckMech(spawnX, spawnY, 60))
            {
                Projectile.NewProjectile(Wiring.GetProjectileSource(spawnX, spawnY), spawnX * 16 + 8, spawnY * 16 + 12, 0f, 0f, ProjectileID.ToiletEffect, 0, 0f, Main.myPlayer);
            }
        }
    }
}
