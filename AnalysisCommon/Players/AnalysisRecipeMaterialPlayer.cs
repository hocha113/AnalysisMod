using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace AnalysisMod.AnalysisCommon.Players
{
    // This class showcases how to use items in the chest player stands on (if exists)
    // for crafting, even if it is not opened by the player
    // One use of this is allowing items in your custom bank to be used for crafting

    // 这个类展示了如何使用玩家站在上面的箱子中的物品进行合成，即使它没有被玩家打开
    // 其中一个用途是允许自定义储存箱中的物品用于合成
    public class AnalysisRecipeMaterialPlayer : ModPlayer
    {
        private int _chestIndexNearby = -1;

        // Nearby chest finding
        // 附近箱子查找
        public override void PostUpdateMiscEffects()
        {
            if (Main.netMode == NetmodeID.Server)
            {
                // We don't need to do any recipe stuff on the server
                // 我们不需要在服务器上执行任何配方操作
                return;
            }

            int oldChestIndex = _chestIndexNearby;

            // Gets leg position in tile coord for further chest searching
            // 获取腿部位置以进一步搜索箱子
            var legPosition = Player.Bottom - new Vector2(0f, 20f);
            var legPositionInTile = legPosition.ToTileCoordinates();

            _chestIndexNearby = -1;
            // Find a possible chest nearby
            // 查找附近可能存在的箱子
            for (int x = -1; x <= 1; x++)
            {
                var tile = Main.tile[legPositionInTile.X + x, legPositionInTile.Y];

                // Dressers are excluded to make search code simpler
                // 简化代码，排除梳妆台等其他类型的容器
                if (!tile.HasTile || !TileID.Sets.IsAContainer[tile.TileType] || tile.TileType is TileID.Dressers)
                {
                    continue;
                }

                // Gets the left-top position for the chest
                // 获取箱子左上角位置
                int left = legPositionInTile.X + x;
                int top = legPositionInTile.Y;

                if (tile.TileFrameX % 36 != 0)
                {
                    left--;
                }

                if (tile.TileFrameY != 0)
                {
                    top--;
                }

                int chestIndex = Chest.FindChest(left, top);
                if (chestIndex > -1 && !Chest.IsLocked(left, top))
                {
                    Chest chest = Main.chest[chestIndex];
                    // Unopened chests in multiplayer have not initialized the items inside of them, so we check for safety if the first item is not null (assuming that all others won't be null either)
                    // Ideally, we would want to write custom netcode to request chest contents, see how a mod like Recipe Browser handles this: https://github.com/JavidPack/RecipeBrowser/blob/1.4/RecipeBrowser.cs, look for usage of packets

                    // 多人游戏中未打开的箱子尚未初始化其中包含的物品，因此我们检查第一个物品是否为null（假设所有其他物品也都不为null）
                    // 理想情况下，我们希望编写自定义网络代码来请求存储内容，请参考Recipe Browser这样的模组处理方式：https://github.com/JavidPack/RecipeBrowser/blob/1.4/RecipeBrowser.cs，请查找数据包使用方法。
                    if (chest.item[0] != null)
                    {
                        _chestIndexNearby = chestIndex;
                        break;
                    }
                }
            }

            // If the nearby chest changed, call FindRecipes to refresh available recipes
            // Since FindRecipes takes a long time to run, we should try to avoid calling it frequently

            // 如果附近有新盒，则调用FindRecipes刷新可用配方
            // 由于FindRecipes运行时间较长，应尽量避免频繁调用它。
            if (oldChestIndex != _chestIndexNearby)
            {
                Recipe.FindRecipes();
            }
        }

        // Use items in the chest for crafting
        // 使用箱内物品进行制作
        public override IEnumerable<Item> AddMaterialsForCrafting(out ItemConsumedCallback itemConsumedCallback)
        {
            // Ensure there is a chest nearby that is not opened by the player, and wasn't destroyed last tick
            // 确保附近有一个未被玩家打开且上次更新时未被销毁的容器
            if (_chestIndexNearby is -1 || Player.chest == _chestIndexNearby || Main.chest[_chestIndexNearby] is not Chest chest)
                return base.AddMaterialsForCrafting(out itemConsumedCallback);

            // onUsedForCrafting invokes when the item is consumed, can be used to send packets in multiplayer mode
            // If there is no need for this, just set it to null

            // 当消耗该项时onUsedForCrafting被调用，可用于在多人游戏模式下发送数据包
            // 如果不需要此操作，请将其设置为null
            itemConsumedCallback = (_, index) =>
            {
                if (Main.netMode is NetmodeID.MultiplayerClient)
                {
                    // Sync chest data
                    // 同步箱子数据
                    NetMessage.SendData(MessageID.SyncChestItem, number: _chestIndexNearby, number2: index);
                }
            };

            // Returns the items in the chest to use them for crafting
            // The returned list should not be a cloned version of items otherwise items will not be consumed

            // 返回箱子中的物品以供制作使用
            // 返回的列表不应该是items的克隆版本，否则物品将无法消耗
            return chest.item;
        }
    }
}