using AnalysisMod.AnalysisContent.Items;
using AnalysisMod.AnalysisContent.Items.Placeable;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AnalysisMod.AnalysisContent.Tiles
{
    // An enum for the 3 stages of herb growth
    // 一个用于描述草药生长的三个阶段的枚举
    public enum PlantStage : byte
    {
        Planted,
        Growing,
        Grown
    }

    // A plant with 3 stages, planted, growing and grown
    // Sadly, modded plants are unable to be grown by the flower boots
    //TODO smart cursor support for herbs, see SmartCursorHelper.Step_AlchemySeeds
    //TODO Staff of Regrowth:
    //- Player.PlaceThing_Tiles_BlockPlacementForAssortedThings: check where type == 84 (grown herb)
    //- Player.ItemCheck_GetTileCutIgnoreList: maybe generalize?
    //TODO vanilla seeds to replace fully grown herb

    // 这是一种有着三个阶段（种植、成长和成熟）的植物
    // 不幸的是，经过修改后的植物无法通过花靴进行生长。
    //TODO 智能光标支持草药，请参见SmartCursorHelper.Step_AlchemySeeds
    //TODO 生命之杖：
    //- Player.PlaceThing_Tiles_BlockPlacementForAssortedThings: 检查type == 84 (已成熟的草本)
    //- Player.ItemCheck_GetTileCutIgnoreList: 可以泛化吗？
    //TODO 使用普通种子替换完全成熟的草本
    public class AnalysisHerb : ModTile
    {
        private const int FrameWidth = 18; // A constant for readability and to kick out those magic numbers
                                           // 为了提高可读性并避免使用魔数而设置常量。

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;
            Main.tileCut[Type] = true;
            Main.tileNoFail[Type] = true;
            TileID.Sets.ReplaceTileBreakUp[Type] = true;
            TileID.Sets.IgnoredInHouseScore[Type] = true;
            TileID.Sets.IgnoredByGrowingSaplings[Type] = true;
            TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]); // Make this tile interact with golf balls in the same way other plants do
                                                                                       // 使这个方块与高尔夫球互动，就像其他植物一样。

            // We do not use this because our tile should only be spelunkable when it's fully grown. That's why we use the IsTileSpelunkable hook instead
            // 我们不使用它，因为我们希望只有在完全成熟时才能探索该方块。这就是为什么我们使用IsTileSpelunkable钩子而不是此方法。

            //Main.tileSpelunker[Type] = true;

            // Do NOT use this, it causes many unintended side effects
            // 不要使用此方法，它会导致许多意外副作用。

            //Main.tileAlch[Type] = true;

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(128, 128, 128), name);

            TileObjectData.newTile.CopyFrom(TileObjectData.StyleAlch);
            TileObjectData.newTile.AnchorValidTiles = new int[] {
                TileID.Grass,
                TileID.HallowedGrass,
                ModContent.TileType<AnalysisBlock>()
            };
            TileObjectData.newTile.AnchorAlternateTiles = new int[] {
                TileID.ClayPot,
                TileID.PlanterBox
            };
            TileObjectData.addTile(Type);

            HitSound = SoundID.Grass;
            DustType = DustID.Ambient_DarkBrown;
        }

        public override bool CanPlace(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j); // Safe way of getting a tile instance
                                                     // 安全地获取一个方块实例。

            if (tile.HasTile)
            {
                int tileType = tile.TileType;
                if (tileType == Type)
                {
                    PlantStage stage = GetStage(i, j); // The current stage of the herb
                                                       // 当前草药所处阶段。

                    // Can only place on the same herb again if it's grown already
                    // 只有当该位置上已经存在完全成熟状态下的同类植物时才能再次种植。
                    return stage == PlantStage.Grown;
                }
                else
                {
                    // Support for vanilla herbs/grasses:
                    // 支持普通草本/草地：
                    if (Main.tileCut[tileType] || TileID.Sets.BreakableWhenPlacing[tileType] || tileType == TileID.WaterDrip || tileType == TileID.LavaDrip || tileType == TileID.HoneyDrip || tileType == TileID.SandDrip)
                    {
                        bool foliageGrass = tileType == TileID.Plants || tileType == TileID.Plants2;
                        bool moddedFoliage = tileType >= TileID.Count && (Main.tileCut[tileType] || TileID.Sets.BreakableWhenPlacing[tileType]);
                        bool harvestableVanillaHerb = Main.tileAlch[tileType] && WorldGen.IsHarvestableHerbWithSeed(tileType, tile.TileFrameX / 18);

                        if (foliageGrass || moddedFoliage || harvestableVanillaHerb)
                        {
                            WorldGen.KillTile(i, j);
                            if (!tile.HasTile && Main.netMode == NetmodeID.MultiplayerClient)
                            {
                                NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, i, j);
                            }

                            return true;
                        }
                    }

                    return false;
                }
            }

            return true;
        }

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            if (i % 2 == 0)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            offsetY = -2; // This is -1 for tiles using StyleAlch, but vanilla sets to -2 for herbs, which causes a slight visual offset between the placement preview and the placed tile.
                          // 对于使用StyleAlch的方块，这个值为-1，但是对于草本来说，vanilla将其设置为-2，这会导致放置预览和实际放置的方块之间存在轻微的视觉偏差。
        }

        public override bool CanDrop(int i, int j)
        {
            PlantStage stage = GetStage(i, j);

            if (stage == PlantStage.Planted)
            {
                // Do not drop anything when just planted
                // 刚种下时不要掉落任何东西
                return false;
            }
            return true;
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            PlantStage stage = GetStage(i, j);

            Vector2 worldPosition = new Vector2(i, j).ToWorldCoordinates();
            Player nearestPlayer = Main.player[Player.FindClosest(worldPosition, 16, 16)];

            int herbItemType = ModContent.ItemType<AnalysisItem>();
            int herbItemStack = 1;

            int seedItemType = ModContent.ItemType<AnalysisHerbSeeds>();
            int seedItemStack = 1;

            if (nearestPlayer.active && nearestPlayer.HeldItem.type == ItemID.StaffofRegrowth)
            {
                // Increased yields with Staff of Regrowth, even when not fully grown
                // 即使没有完全成熟也可以通过生命之杖增加产量
                herbItemStack = Main.rand.Next(1, 3);
                seedItemStack = Main.rand.Next(1, 6);
            }
            else if (stage == PlantStage.Grown)
            {
                // Default yields, only when fully grown
                // 默认产量，在完全成熟时才有效
                herbItemStack = 1;
                seedItemStack = Main.rand.Next(1, 4);
            }

            if (herbItemType > 0 && herbItemStack > 0)
            {
                yield return new Item(herbItemType, herbItemStack);
            }

            if (seedItemType > 0 && seedItemStack > 0)
            {
                yield return new Item(seedItemType, seedItemStack);
            }
        }

        public override bool IsTileSpelunkable(int i, int j)
        {
            PlantStage stage = GetStage(i, j);

            // Only glow if the herb is grown
            // 只有在该草药已经成熟时才发光。
            return stage == PlantStage.Grown;
        }

        public override void RandomUpdate(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            PlantStage stage = GetStage(i, j);

            // Only grow to the next stage if there is a next stage. We don't want our tile turning pink!
            // 只有当还有下一个阶段时才进行生长。我们不希望我们的方块变成粉色！
            if (stage != PlantStage.Grown)
            {
                // Increase the x frame to change the stage
                // 增加x帧以改变阶段
                tile.TileFrameX += FrameWidth;

                // If in multiplayer, sync the frame change
                // 如果在多人游戏中，则同步框架更改
                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    NetMessage.SendTileSquare(-1, i, j, 1);
                }
            }
        }

        // A helper method to quickly get the current stage of the herb (assuming the tile at the coordinates is our herb)
        // 一个快速获取当前草药所处阶段的辅助方法（假设该位置上的方块是我们自己定义的）
        private static PlantStage GetStage(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            return (PlantStage)(tile.TileFrameX / FrameWidth);
        }
    }
}
