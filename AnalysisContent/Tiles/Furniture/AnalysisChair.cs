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
    public class AnalysisChair : ModTile
    {
        public const int NextStyleHeight = 40; // Calculated by adding all CoordinateHeights + CoordinatePaddingFix.Y applied to all of them + 2

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
            AdjTiles = new int[] { TileID.Chairs };

            // Names
            AddMapEntry(new Color(200, 200, 200), Language.GetText("MapObject.Chair"));

            // Placement
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
            TileObjectData.newTile.CoordinatePaddingFix = new Point16(0, 2);
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            // The following 3 lines are needed if you decide to add more styles and stack them vertically
            // 如果您决定添加更多样式并将它们垂直堆叠，则需要以下3行代码
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleHorizontal = true;

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1); // Facing right will use the second texture style
                                            // 面向右将使用第二个纹理样式
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
            // 非常重要的是，这会在玩家和NPC上都被调用，因此不要使用Main.LocalPlayer进行分析，请使用info.restingEntity。
            Tile tile = Framing.GetTileSafely(i, j);

            //info.directionOffset = info.restingEntity is Player ? 6 : 2; // Default to 6 for players, 2 for NPCs
            // 默认情况下，对于玩家为6，对于NPC为2。

            //info.visualOffset = Vector2.Zero; // Defaults to (0,0)
                                                //默认值为（0,0）

            info.TargetDirection = -1;
            if (tile.TileFrameX != 0)
            {
                info.TargetDirection = 1; // Facing right if sat down on the right alternate (added through addAlternate in SetStaticDefaults earlier)
                                          // 如果坐在右边的备选项上（通过SetStaticDefaults中的addAlternate添加），则面向右
            }

            // The anchor represents the bottom-most tile of the chair. This is used to align the entity hitbox
            // Since i and j may be from any coordinate of the chair, we need to adjust the anchor based on that

            // 锚点表示椅子最底部的瓷砖。这用于对齐实体碰撞箱
            // 由于i和j可以来自椅子的任何坐标，因此我们需要根据那个来调整锚点。
            info.AnchorTilePosition.X = i; // Our chair is only 1 wide, so nothing special required
                                           // 我们的椅子只有1格宽，所以不需要特殊处理。
            info.AnchorTilePosition.Y = j;

            if (tile.TileFrameY % NextStyleHeight == 0)
            {
                info.AnchorTilePosition.Y++; // Here, since our chair is only 2 tiles high, we can just check if the tile is the top-most one, then move it 1 down
                                             // 在这里，由于我们的椅子只有2格高度，所以我们只需检查该瓷砖是否是最顶部的一个，并将其向下移动1格即可。
            }
        }

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;

            if (player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
            { // Avoid being able to trigger it from long range
              // 避免能够从远处触发它
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
              // 匹配RightClick中条件。仅当单击时执行某些操作时才应显示交互
                return;
            }

            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<Items.Placeable.Furniture.AnalysisChair>();

            if (Main.tile[i, j].TileFrameX / 18 < 1)
            {
                player.cursorItemIconReversed = true;
            }
        }
    }
}
