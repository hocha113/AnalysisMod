using AnalysisMod.AnalysisContent.Items.Placeable;
using AnalysisMod.AnalysisContent.TileEntities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AnalysisMod.AnalysisContent.Tiles
{
    /// <summary>
    /// This is a more advanced variation of the <seealso cref="AnalysisPylonTile"/> implementation
    /// in tandem with <seealso cref="AdvancedPylonTileEntity"/>, which shows off what advanced techniques you can apply with ModPylons.
    /// If you want to use ModPylons with your own Tile Entities or with multi-tiles that do not conform to vanilla's standards, then
    /// this is the Analysis for you. If you just want normal pylons that act like the ones in vanilla do, check out <seealso cref="AnalysisPylonTile"/>.<br/>
    /// 这是一个更高级的 <seealso cref="AnalysisPylonTile"/> 实现变体，与 <seealso cref="AdvancedPylonTileEntity"/> 一起展示了您可以使用 ModPylons 应用的高级技术。
    /// 如果您想要将 ModPylons 与自己的 Tile Entities 或不符合基本标准的多个 Tiles 结合使用，则此分析适用于您。如果您只想要像基础版 Pylon 那样运作的普通 Pylons，请查看 <seealso cref="AnalysisPylonTile"/>。
    /// </summary>
    /// <remarks>
    /// Note that since this is an advanced Analysis, things that were already explained in <seealso cref="AnalysisPylonTile"/> will not
    /// be as thoroughly explained. They will still be explained if needed in context.<br/>
    /// 注意，由于这是一个高级分析，因此在 <seealso cref="AnalysisPylonTile"/> 中已经解释过的内容将不会被详细解释。如果需要，在上下文中仍然会进行解释。
    /// </remarks>
    public class AnalysisPylonTileAdvanced : ModPylon
    {
        public const int CrystalVerticalFrameCount = 8;

        public Asset<Texture2D> crystalTexture;
        public Asset<Texture2D> crystalHighlightTexture;
        public Asset<Texture2D> mapIcon;

        public override void Load()
        {
            // We'll still use the other Analysis Pylon's sprites, but we need to adjust the texture values first to do so.
            // 我们仍将使用其他 Analysis Pylon 的精灵图图像，但首先需要调整纹理值才能实现。
            crystalTexture = ModContent.Request<Texture2D>(Texture.Replace("Advanced", "") + "_Crystal");
            crystalHighlightTexture = ModContent.Request<Texture2D>(Texture.Replace("Advanced", "") + "_CrystalHighlight");
            mapIcon = ModContent.Request<Texture2D>(Texture.Replace("Advanced", "") + "_MapIcon");
        }

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;

            // This time around, we'll have a tile that is 2x3 instead of 3x4.
            // 这次我们将有一个大小为2x3而非3x4的瓷砖。
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Origin = new Point16(0, 2);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            // Since we are going to need more in-depth functionality, we can't use vanilla's Pylon TE's OnPlace or CanPlace:
            // 由于我们需要更深入地功能性，所以不能使用 vanilla 的 Pylon TE's OnPlace 或 CanPlace：
            AdvancedPylonTileEntity advancedEntity = ModContent.GetInstance<AdvancedPylonTileEntity>();
            TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(advancedEntity.PlacementPreviewHook_CheckIfCanPlace, 1, 0, true);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(advancedEntity.Hook_AfterPlacement, -1, 0, false);

            TileObjectData.addTile(Type);

            TileID.Sets.InteractibleByNPCs[Type] = true;
            TileID.Sets.PreventsSandfall[Type] = true;

            AddToArray(ref TileID.Sets.CountsAsPylon);

            LocalizedText pylonName = CreateMapEntryName();
            AddMapEntry(Color.Black, pylonName);
        }

        public override NPCShop.Entry GetNPCShopEntry()
        {
            // Let's say that our pylon is for sale no matter what for any NPC under all circumstances.
            // 假设我们无论如何都向任何 NPC 出售塔楼。
            return new NPCShop.Entry(ModContent.ItemType<AnalysisPylonItemAdvanced>());
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        {
            return true;
        }

        public override bool RightClick(int i, int j)
        {
            Main.mapFullscreen = true;
            SoundEngine.PlaySound(SoundID.MenuOpen);
            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Main.LocalPlayer.cursorItemIconEnabled = true;
            Main.LocalPlayer.cursorItemIconID = ModContent.ItemType<AnalysisPylonItemAdvanced>();
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            ModContent.GetInstance<AdvancedPylonTileEntity>().Kill(i, j);
        }

        // For the sake of Analysis, we will allow this pylon to always be teleported to as long as it is on, so we make sure these two checks return true.
        // 出于分析目的，只要它处于开启状态就允许随时传送到该塔楼，并确保这两个检查返回 true。
        public override bool ValidTeleportCheck_NPCCount(TeleportPylonInfo pylonInfo, int defaultNecessaryNPCCount)
        {
            return true;
        }

        public override bool ValidTeleportCheck_AnyDanger(TeleportPylonInfo pylonInfo)
        {
            return true;
        }

        // These two steps below are simply determining whether or not either side of the coin is valid, which is to say:
        // Is the destination pylon (the pylon clicked on the map) a valid pylon, and is the pylon the player standing near (the nearby pylon)
        // a valid pylon? If either one of these checks fail, a errorKey wil be set to a custom localization key and a message will go to the player with
        // said text (after its been localized, of course).

        // 下面这两步仅确定硬币正反面是否有效，也就是说：
        // 目标塔楼（在地图上单击的塔楼）是否为有效塔楼，玩家附近的塔楼是否为有效塔楼？
        // 如果其中任何一个检查失败，则会将 errorKey 设置为自定义本地化密钥，
        // 并向玩家发送带有该文本的消息（当然，在进行本地化后）。
        public override void ValidTeleportCheck_DestinationPostCheck(TeleportPylonInfo destinationPylonInfo, ref bool destinationPylonValid, ref string errorKey)
        {
            // If you are unfamiliar with pattern matching notation, all this is asking is:
            // 1) The Tile Entity at the given position is an AdvancedPylonTileEntity (AKA not null or something else)
            // 2) The Tile Entity's isActive value is false

            // 如果您不熟悉模式匹配符号表示法，则所有这些都是在询问以下问题：
            // 1) 给定位置处的 Tile Entity 是否为 AdvancedPylonTileEntity（即非 null 或其他内容）
            // 2) Tile Entity 的 isActive 值是否为 false
            if (TileEntity.ByPosition[destinationPylonInfo.PositionInTiles] is AdvancedPylonTileEntity { isActive: false })
            {
                //Given that both of these things are true, set the error key to our own special message (check the localization file), and make the destination value invalid (false)
                // 接下来的检查是确定附近的 Pylon 是否可能不稳定，如果是，则如果其未激活，我们还会阻止传送。
                destinationPylonValid = false;
                errorKey = "Mods.AnalysisMod.MessageInfo.UnstablePylonIsOff";
            }
        }

        public override void ValidTeleportCheck_NearbyPostCheck(TeleportPylonInfo nearbyPylonInfo, ref bool destinationPylonValid, ref bool anyNearbyValidPylon, ref string errorKey)
        {
            // The next check is determining whether or not the nearby pylon is potentially unstable, and if so, if it's not active, we also prevent teleportation.
            // 接下来的检查是确定附近的 Pylon 是否可能不稳定，如果是，则如果其未激活，我们还会阻止传送。
            if (TileEntity.ByPosition[nearbyPylonInfo.PositionInTiles] is AdvancedPylonTileEntity { isActive: false })
            {
                destinationPylonValid = false;
                errorKey = "Mods.AnalysisMod.MessageInfo.NearbyUnstablePylonIsOff";
            }
        }

        public override void ModifyTeleportationPosition(TeleportPylonInfo destinationPylonInfo, ref Vector2 teleportationPosition)
        {
            // Now, for the fun of it and for the showcase of this hook, let's put a player a bit into the air above the pylon when they teleport.
            // 现在出于乐趣和展示此挂钩功能，请将玩家传送到离开 Pylon 一点点高空中。
            teleportationPosition = destinationPylonInfo.PositionInTiles.ToWorldCoordinates(8f, -32f);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            // Same as the basic Analysis, but our light will be the disco color like the crystal
            // 和基础版分析一样，但我们的光线颜色将与水晶相同
            r = Main.DiscoColor.R / 255f * 0.75f;
            g = Main.DiscoColor.G / 255f * 0.75f;
            b = Main.DiscoColor.B / 255f * 0.75f;
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            // This time, we'll ONLY draw the crystal if the pylon is active
            // We need to check the framing here in order to guarantee we that we are trying to grab the TE ONLY when in the top left corner, where it is
            // located. If we don't do this check, we will be attempting to grab the TE in position where it doesn't exist, throwing errors and causing
            // loads of visual bugs.

            // 这次只有当 Pylon 处于活动状态时才绘制水晶
            // 我们需要在此处检查框架以保证仅尝试抓取 TE 时位于左上角。
            // 否则，我们将尝试在不存在它的位置抓取 TE，
            // 导致错误并引起大量视觉错误。
            if (drawData.tileFrameX % 36 == 0 && drawData.tileFrameY == 0 && TileEntity.ByPosition.TryGetValue(new Point16(i, j), out TileEntity entity) && entity is AdvancedPylonTileEntity { isActive: true })
            {
                Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
            }
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            // This code is essentially identical to how it is in the basic Analysis, but this time the crystal color is the disco (rainbow) color instead
            // Also, since we want the pylon crystal to be drawn at the same height as vanilla (since our tile is one tile smaller), we have to move up the crystal accordingly with the crystalOffset parameter

            // 此代码与基础版分析中的代码本质上相同，但这次水晶颜色为迪斯科（彩虹）颜色
            // 另外，由于我们希望 Pylon 水晶在与 vanilla 相同的高度绘制（因为我们的瓷砖比其小一个），所以必须相应地将水晶向上移动 crystalOffset 参数
            DefaultDrawPylonCrystal(spriteBatch, i, j, crystalTexture, crystalHighlightTexture, new Vector2(0f, -18f), Main.DiscoColor * 0.1f, Main.DiscoColor, 1, CrystalVerticalFrameCount);
        }

        public override void DrawMapIcon(ref MapOverlayDrawContext context, ref string mouseOverText, TeleportPylonInfo pylonInfo, bool isNearPylon, Color drawColor, float deselectedScale, float selectedScale)
        {
            if (!TileEntity.ByPosition.TryGetValue(pylonInfo.PositionInTiles, out var te) || te is not AdvancedPylonTileEntity entity)
            {
                // If for some reason we don't find the tile entity, we won't draw anything.
                // 如果出现某些原因导致找不到 Tile Entity，则不会绘制任何内容。
                return;
            }

            // Depending on the whether or not the pylon is active, the color of the icon will change;
            // otherwise, it acts as normal.

            // 根据 Pylon 是否处于活动状态，图标的颜色将发生变化；
            // 否则，它就像正常一样运作。
            drawColor = !entity.isActive ? Color.Gray * 0.5f : drawColor;
            bool mouseOver = DefaultDrawMapIcon(ref context, mapIcon, pylonInfo.PositionInTiles.ToVector2() + new Vector2(1, 1.5f), drawColor, deselectedScale, selectedScale);
            DefaultMapClickHandle(mouseOver, pylonInfo, ModContent.GetInstance<AnalysisPylonItemAdvanced>().DisplayName.Key, ref mouseOverText);
        }
    }
}
