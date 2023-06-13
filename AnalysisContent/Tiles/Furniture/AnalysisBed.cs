using AnalysisMod.AnalysisContent.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AnalysisMod.AnalysisContent.Tiles.Furniture
{
    public class AnalysisBed : ModTile
    {
        public const int NextStyleHeight = 38; //Calculated by adding all CoordinateHeights + CoordinatePaddingFix.Y applied to all of them + 2
                                               //通过将所有CoordinateHeights +应用于它们的CoordinatePaddingFix.Y相加+ 2进行计算

        public override void SetStaticDefaults()
        {
            // Properties
            // 属性
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.CanBeSleptIn[Type] = true; // Facilitates calling ModifySleepingTargetInfo
                                                   // 方便调用ModifySleepingTargetInfo

            TileID.Sets.InteractibleByNPCs[Type] = true; // Town NPCs will palm their hand at this tile
                                                         // 城镇NPC会在这个图块上手掌心向下

            TileID.Sets.IsValidSpawnPoint[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair); // Beds count as chairs for the purpose of suitable room creation
                                                                 // 床对于适合房间创建而言算作椅子

            DustType = ModContent.DustType<Sparkle>();
            AdjTiles = new int[] { TileID.Beds };

            // Placement
            // 放置
            TileObjectData.newTile.CopyFrom(TileObjectData.Style4x2); // this style already takes care of direction for us
                                                                      // 此样式已经为我们处理了方向问题。

            TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
            TileObjectData.newTile.CoordinatePaddingFix = new Point16(0, -2);
            TileObjectData.addTile(Type);

            // Etc
            AddMapEntry(new Color(191, 142, 111), Language.GetText("ItemName.Bed"));
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        {
            return true;
        }

        public override void ModifySmartInteractCoords(ref int width, ref int height, ref int frameWidth, ref int frameHeight, ref int extraY)
        {
            // Because beds have special smart interaction, this splits up the left and right side into the necessary 2x2 sections
            // 因为床具有特殊的智能交互，所以将左右两侧分成必要的2x2部分。

            width = 2; // Default to the Width defined for TileObjectData.newTile
                       // 默认使用TileObjectData.newTile定义的宽度。

            height = 2; // Default to the Height defined for TileObjectData.newTile
                        // 默认使用TileObjectData.newTile定义的高度。

            //extraY = 0; // Depends on how you set up frameHeight and CoordinateHeights and CoordinatePaddingFix.Y
                          //取决于您如何设置frameHeight和CoordinateHeights以及CoordinatePaddingFix.Y.
        }

        public override void ModifySleepingTargetInfo(int i, int j, ref TileRestingInfo info)
        {
            // Default values match the regular vanilla bed
            // You might need to mess with the info here if your bed is not a typical 4x2 tile

            // 默认值与常规香草床匹配
            // 如果您的床不是典型的4x2瓷砖，则可能需要更改此处信息。
            info.VisualOffset.Y += 4f; // Move player down a notch because the bed is not as high as a regular bed
                                       // 将玩家下移一格，因为该床不像普通床那么高。
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 1;
        }

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[i, j];
            int spawnX = i - tile.TileFrameX / 18 + (tile.TileFrameX >= 72 ? 5 : 2);
            int spawnY = j + 2;

            if (tile.TileFrameY % NextStyleHeight != 0)
            {
                spawnY--;
            }

            if (!Player.IsHoveringOverABottomSideOfABed(i, j))
            { // This assumes your bed is 4x2 with 2x2 sections. You have to write your own code here otherwise
              // 这假定您的床是4x2，并且有2x2节。否则，您必须编写自己的代码。
                if (player.IsWithinSnappngRangeToTile(i, j, PlayerSleepingHelper.BedSleepingMaxDistance))
                {
                    player.GamepadEnableGrappleCooldown();
                    player.sleeping.StartSleeping(player, i, j);
                }
            }
            else
            {
                player.FindSpawn();

                if (player.SpawnX == spawnX && player.SpawnY == spawnY)
                {
                    player.RemoveSpawn();
                    Main.NewText(Language.GetTextValue("Game.SpawnPointRemoved"), byte.MaxValue, 240, 20);
                }
                else if (Player.CheckSpawn(spawnX, spawnY))
                {
                    player.ChangeSpawn(spawnX, spawnY);
                    Main.NewText(Language.GetTextValue("Game.SpawnPointSet"), byte.MaxValue, 240, 20);
                }
            }

            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;

            if (!Player.IsHoveringOverABottomSideOfABed(i, j))
            {
                if (player.IsWithinSnappngRangeToTile(i, j, PlayerSleepingHelper.BedSleepingMaxDistance))
                { // Match condition in RightClick. Interaction should only show if clicking it does something
                  // 在RightClick中匹配条件。只有在单击时执行某些操作时才显示交互。
                    player.noThrow = 2;
                    player.cursorItemIconEnabled = true;
                    player.cursorItemIconID = ItemID.SleepingIcon;
                }
            }
            else
            {
                player.noThrow = 2;
                player.cursorItemIconEnabled = true;
                player.cursorItemIconID = ModContent.ItemType<Items.Placeable.Furniture.AnalysisBed>();
            }
        }
    }
}
