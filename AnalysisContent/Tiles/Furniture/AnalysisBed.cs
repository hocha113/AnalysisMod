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
                                               //ͨ��������CoordinateHeights +Ӧ�������ǵ�CoordinatePaddingFix.Y���+ 2���м���

        public override void SetStaticDefaults()
        {
            // Properties
            // ����
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.CanBeSleptIn[Type] = true; // Facilitates calling ModifySleepingTargetInfo
                                                   // �������ModifySleepingTargetInfo

            TileID.Sets.InteractibleByNPCs[Type] = true; // Town NPCs will palm their hand at this tile
                                                         // ����NPC�������ͼ��������������

            TileID.Sets.IsValidSpawnPoint[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair); // Beds count as chairs for the purpose of suitable room creation
                                                                 // �������ʺϷ��䴴��������������

            DustType = ModContent.DustType<Sparkle>();
            AdjTiles = new int[] { TileID.Beds };

            // Placement
            // ����
            TileObjectData.newTile.CopyFrom(TileObjectData.Style4x2); // this style already takes care of direction for us
                                                                      // ����ʽ�Ѿ�Ϊ���Ǵ����˷������⡣

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
            // ��Ϊ��������������ܽ��������Խ���������ֳɱ�Ҫ��2x2���֡�

            width = 2; // Default to the Width defined for TileObjectData.newTile
                       // Ĭ��ʹ��TileObjectData.newTile����Ŀ��ȡ�

            height = 2; // Default to the Height defined for TileObjectData.newTile
                        // Ĭ��ʹ��TileObjectData.newTile����ĸ߶ȡ�

            //extraY = 0; // Depends on how you set up frameHeight and CoordinateHeights and CoordinatePaddingFix.Y
                          //ȡ�������������frameHeight��CoordinateHeights�Լ�CoordinatePaddingFix.Y.
        }

        public override void ModifySleepingTargetInfo(int i, int j, ref TileRestingInfo info)
        {
            // Default values match the regular vanilla bed
            // You might need to mess with the info here if your bed is not a typical 4x2 tile

            // Ĭ��ֵ�볣����ݴ�ƥ��
            // ������Ĵ����ǵ��͵�4x2��ש���������Ҫ���Ĵ˴���Ϣ��
            info.VisualOffset.Y += 4f; // Move player down a notch because the bed is not as high as a regular bed
                                       // ���������һ����Ϊ�ô�������ͨ����ô�ߡ�
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
              // ��ٶ����Ĵ���4x2��������2x2�ڡ������������д�Լ��Ĵ��롣
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
                  // ��RightClick��ƥ��������ֻ���ڵ���ʱִ��ĳЩ����ʱ����ʾ������
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