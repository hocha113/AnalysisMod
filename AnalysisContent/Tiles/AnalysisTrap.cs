using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AnalysisMod.AnalysisContent.Tiles
{
    // This class shows off a number of less common ModTile methods. These methods help our trap tile behave like vanilla traps. 
    // In particular, hammer behavior is particularly tricky. The logic here is setup for multiple styles as well.

    // 这个类展示了一些不太常见的ModTile方法。这些方法帮助我们的陷阱瓦片表现得像普通的陷阱一样。
    // 特别是锤子行为特别棘手。这里的逻辑设置多种风格。
    public class AnalysisTrap : ModTile
    {
        public override void SetStaticDefaults()
        {
            TileID.Sets.DrawsWalls[Type] = true;
            TileID.Sets.DontDrawTileSliced[Type] = true;
            TileID.Sets.IgnoresNearbyHalfbricksWhenDrawn[Type] = true;
            TileID.Sets.IsAMechanism[Type] = true;

            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileFrameImportant[Type] = true;

            // These 2 AddMapEntry and GetMapOption show off multiple Map Entries per Tile. Delete GetMapOption and all but 1 of these for your own ModTile if you don't actually need it.
            // 这两个AddMapEntry和GetMapOption展示了每个瓦片有多个地图条目。如果您实际上不需要它，请删除GetMapOption和所有但其中1个，以便在自己的ModTile中使用。
            AddMapEntry(new Color(21, 179, 192), Language.GetText("MapObject.Trap")); // localized text for "Trap"
                                                                                      // "Trap" 的本地化文本
            AddMapEntry(new Color(0, 141, 63), Language.GetText("MapObject.Trap"));
        }

        // Read the comments above on AddMapEntry.
        // 请参阅上面关于AddMapEntry的注释。
        public override ushort GetMapOption(int i, int j) => (ushort)(Main.tile[i, j].TileFrameY / 18);

        public override bool IsTileDangerous(int i, int j, Player player) => true;

        // Because this tile does not use a TileObjectData, and consequently does not have "real" tile styles, the correct tile style value can't be determined automatically. This means that the correct item won't automatically drop, so we must use GetItemDrops to calculate the tile style to determine the item drop. 
        // 因为此瓦片没有使用TileObjectData，因此没有“真正”的瓦片样式，无法自动确定正确的瓦片样式值。这意味着正确的物品将不会自动掉落，因此我们必须使用GetItemDrops来计算瓦片样式以确定物品掉落。
        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            Tile t = Main.tile[i, j];
            int style = t.TileFrameY / 18;
            // It can be useful to share a single tile with multiple styles.
            // 具有多种风格可以很有用。
            yield return new Item(Mod.Find<ModItem>(Items.Placeable.AnalysisTrap.GetInternalNameFromStyle(style)).Type);

            // Here is an alternate approach:
            // 这是另一种方法：

            // int dropItem = TileLoader.GetItemDropFromTypeAndStyle(Type, style);
            // yield return new Item(dropItem);
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            int style = Main.tile[i, j].TileFrameY / 18;
            if (style == 0)
            {
                type = DustID.Glass; // A blue dust to match the tile
                                     // 一个蓝色粉尘匹配该块
            }
            if (style == 1)
            {
                type = DustID.JungleGrass; // A green dust for the 2nd style.
                                           // 第二款风格用一个绿色粉尘。
            }
            return true;
        }

        // PlaceInWorld is needed to facilitate styles and alternates since this tile doesn't use a TileObjectData. Placing left and right based on player direction is usually done in the TileObjectData, but the specifics of that don't work for how we want this tile to work. 
        // PlaceInWorld需要促进风格和备选项，因为该块不使用TileObjectData。根据玩家方向放置左右通常在TileObjectData中完成，但其具体细节与我们想要该块工作的方式不符。
        public override void PlaceInWorld(int i, int j, Item item)
        {
            int style = Main.LocalPlayer.HeldItem.placeStyle;
            Tile tile = Main.tile[i, j];
            tile.TileFrameY = (short)(style * 18);
            if (Main.LocalPlayer.direction == 1)
            {
                tile.TileFrameX += 18;
            }
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(-1, Player.tileTargetX, Player.tileTargetY, 1, TileChangeType.None);
            }
        }

        // This progression matches vanilla tiles, you don't have to follow it if you don't want. Some vanilla traps don't have 6 states, only 4. This can be implemented with different logic in Slope. Making 8 directions is also easily done in a similar manner.
        // 这个进展与香草瓷砖相匹配，如果您不想遵循它，可以不必这样做。一些香草陷阱只有4种状态而没有6种。这可以在Slope中使用不同的逻辑实现。制作8个方向也可以用类似的方法轻松完成。
        private static int[] frameXCycle = { 2, 3, 4, 5, 1, 0 };
        // We can use the Slope method to override what happens when this tile is hammered.
        // 我们可以使用Slope方法覆盖当此瓦片被锤击时发生的情况。
        public override bool Slope(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            int nextFrameX = frameXCycle[tile.TileFrameX / 18];
            tile.TileFrameX = (short)(nextFrameX * 18);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(-1, Player.tileTargetX, Player.tileTargetY, 1, TileChangeType.None);
            }
            return false;
        }

        public override void HitWire(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            int style = tile.TileFrameY / 18;
            Vector2 spawnPosition;
            // This logic here corresponds to the orientation of the sprites in the spritesheet, change it if your tile is different in design.
            // 这里的逻辑对应于精灵图表中精灵图的方向，请根据您设计的瓦片进行更改。
            int horizontalDirection = tile.TileFrameX == 0 ? -1 : tile.TileFrameX == 18 ? 1 : 0;
            int verticalDirection = tile.TileFrameX < 36 ? 0 : tile.TileFrameX < 72 ? -1 : 1;
            // Each trap style within this Tile shoots different projectiles.
            // 此Tile内每个陷阱风格都会射出不同类型的抛射物。
            if (style == 0)
            {
                // Wiring.CheckMech checks if the wiring cooldown has been reached. Put a longer number here for less frequent projectile spawns. 200 is the dart/flame cooldown. Spear is 90, spiky ball is 300
                // Wiring.CheckMech检查是否已达到布线冷却时间。将较长数字放在此处以减少投掷物生成频率。200是飞镖/火焰冷却时间。长枪是90，钉球是300
                if (Wiring.CheckMech(i, j, 60))
                {
                    spawnPosition = new Vector2(i * 16 + 8 + 0 * horizontalDirection, j * 16 + 9 + 0 * verticalDirection); // The extra numbers here help center the projectile spawn position if you need to.
                                                                                                                           // 这里额外添加了数字以帮助居中投掷物生成位置（如果需要）。

                    // In a real mod you should be spawning projectiles that are both hostile and friendly to do damage to both players and NPC, as Terraria traps do.
                    // Make sure to change velocity, projectile, damage, and knockback.

                    // 在真正的mod中，您应该产生既敌对又友好的抛射物来对玩家和NPC造成伤害，就像Terraria陷阱一样。
                    // 确保更改速度、抛射物、伤害和反弹力等参数。
                    Projectile.NewProjectile(Wiring.GetProjectileSource(i, j), spawnPosition, new Vector2(horizontalDirection, verticalDirection) * 6f, ProjectileID.IchorBullet, 20, 2f, Main.myPlayer);
                }
            }
            else if (style == 1)
            {
                // A longer cooldown for ChlorophyteBullet trap.
                // ChlorophyteBullet陷阱的较长冷却时间。
                if (Wiring.CheckMech(i, j, 200))
                {
                    spawnPosition = new Vector2(i * 16 + 8 + 0 * horizontalDirection, j * 16 + 9 + 0 * verticalDirection); // The extra numbers here help center the projectile spawn position.
                                                                                                                           // 这里的额外数字有助于将弹丸生成位置居中。

                    Projectile.NewProjectile(Wiring.GetProjectileSource(i, j), spawnPosition, new Vector2(horizontalDirection, verticalDirection) * 8f, ProjectileID.ChlorophyteBullet, 40, 2f, Main.myPlayer);
                }
            }
        }
    }
}