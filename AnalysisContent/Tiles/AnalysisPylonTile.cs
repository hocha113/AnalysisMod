using AnalysisMod.AnalysisCommon;
using AnalysisMod.AnalysisCommon.Systems;
using AnalysisMod.AnalysisContent.Biomes;
using AnalysisMod.AnalysisContent.Items.Placeable;
using AnalysisMod.AnalysisContent.TileEntities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;
using Terraria.ObjectData;

namespace AnalysisMod.AnalysisContent.Tiles
{
    /// <summary>
    /// An Analysis for creating a Pylon, identical to how they function in Vanilla. Shows off <seealso cref="ModPylon"/>, an abstract
    /// extension of <seealso cref="ModTile"/> that has additional functionality for Pylon specific tiles.<br/>
    /// If you are going to make multiple pylons that all act the same (like in Vanilla), it is recommended you make a base class
    /// with override functionality in order to prevent writing boilerplate. (For Analysis, making a "CrystalTexture" property that you can
    /// override in order to streamline that process.)<br/>
    ///分析如何创建一个与原版功能相同的Pylon。展示了<seealso cref="ModPylon"/>，这是<seealso cref="ModTile"/>的抽象扩展，具有针对特定于Pylon的瓷砖的附加功能。<br/>
    /// 如果您要制作多个行为相同（如在Vanilla中）的支柱，则建议您制作一个基类，并具有覆盖功能，以防止编写样板文件。（例如，在Analysis中，可以创建“CrystalTexture”属性以简化该过程。）
    /// </summary>
    public class AnalysisPylonTile : ModPylon
    {
        public const int CrystalVerticalFrameCount = 8;

        public Asset<Texture2D> crystalTexture;
        public Asset<Texture2D> crystalHighlightTexture;
        public Asset<Texture2D> mapIcon;

        public override void Load()
        {
            // We'll need these textures for later, it's best practice to cache them on load instead of continually requesting every draw call.
            // 我们将需要这些纹理稍后使用，最好在加载时缓存它们而不是每次绘制调用时都请求一次。
            crystalTexture = ModContent.Request<Texture2D>(Texture + "_Crystal");
            crystalHighlightTexture = ModContent.Request<Texture2D>(Texture + "_CrystalHighlight");
            mapIcon = ModContent.Request<Texture2D>(Texture + "_MapIcon");
        }

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            // These definitions allow for vanilla's pylon TileEntities to be placed.
            // tModLoader has a built in Tile Entity specifically for modded pylons, which we must extend (see SimplePylonTileEntity)

            // 这些定义允许放置香草支柱TileEntities。
            // tModLoader内置了专门用于模组支柱的Tile Entity，我们必须进行扩展（请参见SimplePylonTileEntity）
            TEModdedPylon moddedPylon = ModContent.GetInstance<SimplePylonTileEntity>();
            TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(moddedPylon.PlacementPreviewHook_CheckIfCanPlace, 1, 0, true);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(moddedPylon.Hook_AfterPlacement, -1, 0, false);

            TileObjectData.addTile(Type);

            TileID.Sets.InteractibleByNPCs[Type] = true;
            TileID.Sets.PreventsSandfall[Type] = true;
            TileID.Sets.AvoidedByMeteorLanding[Type] = true;

            // Adds functionality for proximity of pylons; if this is true, then being near this tile will count as being near a pylon for the teleportation process.
            // 添加接近支柱的功能；如果为true，则靠近此瓷砖将计入传送过程中靠近支柱的位置。
            AddToArray(ref TileID.Sets.CountsAsPylon);

            LocalizedText pylonName = CreateMapEntryName(); //Name is in the localization file
                                                            //名称在本地化文件中

            AddMapEntry(Color.White, pylonName);
        }

        public override NPCShop.Entry GetNPCShopEntry()
        {
            // return a new NPCShop.Entry with the desired conditions for sale.
            // 返回带有所需销售条件的新NPCShop.Entry。

            // As an Analysis, if we want to sell the pylon if we're in the Analysis surface, or Analysis underground, when there is another NPC nearby.
            // Lets assume we don't care about happiness or crimson or corruption, so we won't include those conditions
            // This does not affect the teleport conditions, only the sale conditions

            // 作为分析结果，如果我们想在分析表面或分析地下挂起并且附近还有其他NPC，则出售塔楼。
            // 假设我们不关心幸福、血腥或腐化，因此我们不会包括这些条件
            // 这不影响传送条件，只影响销售条件
            return new NPCShop.Entry(ModContent.ItemType<AnalysisPylonItem>(), Condition.AnotherTownNPCNearby, AnalysisConditions.InAnalysisBiome);

            // Other standard pylon conditions are:
            // 其他标准支柱条件是：

            // Condition.HappyEnoughToSellPylons
            // Condition.NotInEvilBiome
        }

        public override void MouseOver(int i, int j)
        {
            // Show a little pylon icon on the mouse indicating we are hovering over it.
            // 在鼠标上显示一个小的支柱图标，表示我们正在悬停在它上面。
            Main.LocalPlayer.cursorItemIconEnabled = true;
            Main.LocalPlayer.cursorItemIconID = ModContent.ItemType<AnalysisPylonItem>();
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            // We need to clean up after ourselves, since this is still a "unique" tile, separate from Vanilla Pylons, so we must kill the TileEntity.
            // 我们需要清理自己的东西，因为这仍然是一个“独特”的瓷砖，与Vanilla Pylons分开，所以我们必须杀死TileEntity。
            ModContent.GetInstance<SimplePylonTileEntity>().Kill(i, j);
        }

        public override bool ValidTeleportCheck_NPCCount(TeleportPylonInfo pylonInfo, int defaultNecessaryNPCCount)
        {
            // Let's say for fun sake that no NPCs need to be nearby in order for this pylon to function. If you want your pylon to function just like vanilla,
            // you don't need to override this method at all.

            // 假设出于乐趣而言，在附近没有NPC需要使该塔楼正常工作。
            // 如果您希望您的Pylon像香草一样运行，则根本不需要覆盖此方法。
            return true;
        }

        public override bool ValidTeleportCheck_BiomeRequirements(TeleportPylonInfo pylonInfo, SceneMetrics sceneData)
        {
            // Right before this hook is called, the sceneData parameter exports its information based on wherever the destination pylon is,
            // and by extension, it will call ALL ModSystems that use the TileCountsAvailable method. This means, that if you determine biomes
            // based off of tile count, when this hook is called, you can simply check the tile threshold, like we do here. In the context of AnalysisMod,
            // something is considered within the Analysis Surface/Underground biome if there are 40 or more Analysis blocks at that location.

            // 就在调用此钩子之前，sceneData参数基于目标Pylon导出其信息，
            // 并且通过扩展将调用使用TileCountsAvailable方法的所有ModSystems。这意味着如果你确定生物群系
            // 基于平铺计数时，在调用此挂钩时，您可以简单地检查平铺阈值（如下所示）。在AnalysisMod的背景下，
            // 如果该位置有40个或更多个Analysis块，则认为某些内容属于Analysis Surface / Underground生物群系。
            return ModContent.GetInstance<AnalysisBiomeTileCount>().AnalysisBlockCount >= 40;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            // Pylons in vanilla light up, which is just a simple functionality we add using ModTile's ModifyLight.
            // Let's just add a simple white light for our pylon:

            // 香草中的支柱发光, 这只是我们使用 ModTile 的 ModifyLight 添加了一个简单功能。
            // 让我们为我们的支柱添加一个简单的白光：
            r = g = b = 0.75f;
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            // We want to draw the pylon crystal the exact same way vanilla does, so we can use this built in method in ModPylon for default crystal drawing:
            // For the sake of Analysis, lets make our pylon create a bit more dust by decreasing the dustConsequent value down to 1. If you want your dust spawning to be identical to vanilla, set dustConsequent to 4.
            // We also multiply the pylonShadowColor in order to decrease its opacity, so it actually looks like a "shadow"

            // 我们希望以与香草完全相同的方式绘制塔楼水晶，因此可以使用ModPylon中内置的方法进行默认水晶绘制：
            // 为了分析，让我们使我们的支柱产生更多灰尘，将dustConsequent值降低到1。如果您希望您的粉尘生成与香草完全相同，请将dustConsequent设置为4。
            // 我们还乘以pylonShadowColor以减少其不透明度，从而实际上看起来像“阴影”
            DefaultDrawPylonCrystal(spriteBatch, i, j, crystalTexture, crystalHighlightTexture, new Vector2(0f, -12f), Color.White * 0.1f, Color.White, 1, CrystalVerticalFrameCount);
        }

        public override void DrawMapIcon(ref MapOverlayDrawContext context, ref string mouseOverText, TeleportPylonInfo pylonInfo, bool isNearPylon, Color drawColor, float deselectedScale, float selectedScale)
        {
            // Just like in SpecialDraw, we want things to be handled the EXACT same way vanilla would handle it, which ModPylon also has built in methods for:
            // 就像在SpecialDraw中一样，我们希望事情被处理与Vanilla完全相同的方式，并且ModPylon也具有内置方法：
            bool mouseOver = DefaultDrawMapIcon(ref context, mapIcon, pylonInfo.PositionInTiles.ToVector2() + new Vector2(1.5f, 2f), drawColor, deselectedScale, selectedScale);
            DefaultMapClickHandle(mouseOver, pylonInfo, ModContent.GetInstance<AnalysisPylonItem>().DisplayName.Key, ref mouseOverText);
        }
    }
}
