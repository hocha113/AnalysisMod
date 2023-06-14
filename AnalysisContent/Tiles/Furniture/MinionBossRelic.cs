using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Enums;
using System;
using ReLogic.Content;
using Terraria.Localization;

namespace AnalysisMod.AnalysisContent.Tiles.Furniture
{
    // Common code for a Master Mode boss relic
    // Supports optional Item.placeStyle handling if you wish to add more relics but use the same tile type (then it would be wise to name this class something more generic like BossRelic)
    // If you want to add more relics but don't want to use the Item.placeStyle approach, see the inheritance Analysis at the bottom of the file

    // ��ʦģʽBoss�����ͨ�ô���
    // ���������Ӹ������ﵫʹ����ͬ��ͼ�����ͣ���֧�ֿ�ѡ��Item.placeStyle������ô�����ܵ���������Ϊ��ͨ�õ����ƣ���BossRelic��
    // ���Ҫ��Ӹ������ﵫ����ʹ��Item.placeStyle��������μ��ļ��ײ��ļ̳з���
    public class MinionBossRelic : ModTile
    {
        public const int FrameWidth = 18 * 3;
        public const int FrameHeight = 18 * 4;
        public const int HorizontalFrames = 1;
        public const int VerticalFrames = 1; // Optional: Increase this number to match the amount of relics you have on your extra sheet, if you choose to use the Item.placeStyle approach
                                             // ��ѡ�����Ӵ�������ƥ���������ӵ�е��������������ѡ��ʹ��Item.placeStyle����

        public Asset<Texture2D> RelicTexture;

        // Every relic has its own extra floating part, should be 50x50. Optional: Expand this sheet if you want to add more, stacked vertically
        // If you do not use the Item.placeStyle approach, and you extend from this class, you can override this to point to a different texture

        // ÿ�����ﶼ���Լ����⸡�����֣�ӦΪ50x50�� ��ѡ�����Ҫ��Ӹ������ݣ������չ���˹���������ֱ�ѵ�
        // �������ʹ��Item.placeStyle���������ҴӸ�����չ������Ը�������ָ��ͬ������
        public virtual string RelicTextureName => "AnalysisMod/AnalysisContent/Tiles/Furniture/MinionBossRelic";

        // All relics use the same pedestal texture, this one is copied from vanilla
        // �������ﶼʹ����ͬ�Ļ�����������Ǵ�ԭ�渴�ƹ�����
        public override string Texture => "AnalysisMod/AnalysisContent/Tiles/Furniture/RelicPedestal";

        public override void Load()
        {
            if (!Main.dedServ)
            {
                RelicTexture = ModContent.Request<Texture2D>(RelicTextureName);
            }
        }

        public override void Unload()
        {
            // Unload the extra texture displayed on the pedestal
            // ж����ʾ�ڻ����϶�������
            RelicTexture = null;
        }

        public override void SetStaticDefaults()
        {
            Main.tileShine[Type] = 400; // Responsible for golden particles
                                        // �����ɫ����Ч��

            Main.tileFrameImportant[Type] = true; // Any multitile requires this
                                                  // �κζ�ͼ�鶼��Ҫ���������

            TileID.Sets.InteractibleByNPCs[Type] = true; // Town NPCs will palm their hand at this tile
                                                         // ����NPC�������ͼ�鴦��������������

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4); // Relics are 3x4
                                                                      // �����СΪ3x4.

            TileObjectData.newTile.LavaDeath = false; // Does not break when lava touches it
                                                      // ���ҽ��Ӵ�ʱ�������ѡ�

            TileObjectData.newTile.DrawYOffset = 2; // So the tile sinks into the ground
                                                    // ��˸�ͼ���������档

            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft; // Player faces to the left
                                                                              // ��ҳ������

            TileObjectData.newTile.StyleHorizontal = false; // Based on how the alternate sprites are positioned on the sprite (by default, true)
                                                            // ���ڱ��þ���ͼ�ھ���ͼ�ϵ�λ�ã�Ĭ��Ϊtrue��

            // This controls how styles are laid out in the texture file. This tile is special in that all styles will use the same texture section to draw the pedestal.
            // ���������ʽ�������ļ��еĲ��֡����ͼ������⣬��Ϊ������ʽ����ʹ����ͬ�������������ƻ�����
            TileObjectData.newTile.StyleWrapLimitVisualOverride = 2;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.styleLineSkipVisualOverride = 0; // This forces the tile preview to draw as if drawing the 1st style.
                                                                    // ��ǿ��ͼ��Ԥ������Ϊ���Ƶ�һ����ʽ��

            // Register an alternate tile data with flipped direction
            // ע��һ����ת��������ͼ������
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile); // Copy everything from above, saves us some code
                                                                          // ���������������ݣ����Խ�ʡһЩ����

            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight; // Player faces to the right
                                                                                    // �������Ҳ�
            TileObjectData.addAlternate(1);

            // Register the tile data itself
            // ע��ͼ�����ݱ���
            TileObjectData.addTile(Type);

            // Register map name and color
            // "MapObject.Relic" refers to the translation key for the vanilla "Relic" text

            // ע���ͼ���ƺ���ɫ
            // "MapObject.Relic"��ָԭ�桰Relic���ı��ķ����
            AddMapEntry(new Color(233, 207, 94), Language.GetText("MapObject.Relic"));
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            return false;
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            // This forces the tile to draw the pedestal even if the placeStyle differs. 
            // �⽫ǿ�ƻ��ƻ�������ʹplaceStyle��ͬ��
            tileFrameX %= FrameWidth; // Clamps the frameX
                                      // �н�frameX
            tileFrameY %= FrameHeight * 2; // Clamps the frameY (two horizontally aligned place styles, hence * 2)
                                           // �н�frameY������ˮƽ����ķ�����ʽ�����* 2��
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            // Since this tile does not have the hovering part on its sheet, we have to animate it ourselves
            // Therefore we register the top-left of the tile as a "special point"
            // This allows us to draw things in SpecialDraw

            //���ڸ�ͼ���������û����ͣ���֣�������Ǳ����Լ����ж�������
            // ��ˣ����ǽ����ϽǵĴ�שע��Ϊ������㡱
            //������������SpecialDraw�л��ƶ���
            if (drawData.tileFrameX % FrameWidth == 0 && drawData.tileFrameY % FrameHeight == 0)
            {
                Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
            }
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            // This is lighting-mode specific, always include this if you draw tiles manually
            //������ֶ�����ͼ�飬����ƹ�ģʽ�йأ���ʼ�հ���������
            Vector2 offScreen = new Vector2(Main.offScreenRange);
            if (Main.drawToScreen)
            {
                offScreen = Vector2.Zero;
            }

            // Take the tile, check if it actually exists

            Point p = new Point(i, j);
            Tile tile = Main.tile[p.X, p.Y];
            if (tile == null || !tile.HasTile)
            {
                return;
            }

            // Get the initial draw parameters
            //��ȡ��ʼ���Ʋ���
            Texture2D texture = RelicTexture.Value;

            int frameY = tile.TileFrameX / FrameWidth; // Picks the frame on the sheet based on the placeStyle of the item
                                                       // ������Ʒ��placeStyleѡ����ϵĿ��

            Rectangle frame = texture.Frame(HorizontalFrames, VerticalFrames, 0, frameY);

            Vector2 origin = frame.Size() / 2f;
            Vector2 worldPos = p.ToWorldCoordinates(24f, 64f);

            Color color = Lighting.GetColor(p.X, p.Y);

            bool direction = tile.TileFrameY / FrameHeight != 0; // This is related to the alternate tile data we registered before
                                                                 // ��������֮ǰע��ı��ô�ש�������

            SpriteEffects effects = direction ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // Some math magic to make it smoothly move up and down over time
            //һЩ��ѧ����ʹ���ܹ�����ʱ��ƽ�ȵ������ƶ�
            const float TwoPi = (float)Math.PI * 2f;
            float offset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 5f);
            Vector2 drawPos = worldPos + offScreen - Main.screenPosition + new Vector2(0f, -40f) + new Vector2(0f, offset * 4f);

            // Draw the main texture
            // ����������
            spriteBatch.Draw(texture, drawPos, frame, color, 0f, origin, 1f, effects, 0f);

            // Draw the periodic glow effect
            // ���������Է���Ч��
            float scale = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 2f) * 0.3f + 0.7f;
            Color effectColor = color;
            effectColor.A = 0;
            effectColor = effectColor * 0.1f * scale;
            for (float num5 = 0f; num5 < 1f; num5 += 355f / (678f * (float)Math.PI))
            {
                spriteBatch.Draw(texture, drawPos + (TwoPi * num5).ToRotationVector2() * (6f + offset * 2f), frame, effectColor, 0f, origin, 1f, effects, 0f);
            }
        }
    }

    // If you want to make more relics but do not use the Item.placeStyle approach, you can use inheritance to avoid using duplicate code:
    // Your tile code would then inherit from the MinionBossRelic class (which you should make abstract) and should look like this:

    // ���Ҫ�����������ﵫ��ʹ��Item.placeStyle�����������ʹ�ü̳��������ظ����룺
    //Ȼ�����tile������MinionBossRelic�ࣨ��Ӧ�ó��󻯣��м̳У�����Ӧ����������
    /*
	public class MyBossRelic : MinionBossRelic
	{
		public override string RelicTextureName => "AnalysisMod/AnalysisContent/Tiles/Furniture/MyBossRelic";

		public override void SetStaticDefaults() {
			base.SetStaticDefaults();
		}
	}
	*/

    // Your item code would then just use the MyBossRelic tile type, and keep placeStyle on 0
    // The textures for MyBossRelic item/tile have to be supplied separately

    // Ȼ��������Ŀ����ֻ��ʹ��MyBossRelic tile���ͣ�������placeStyleΪ0
    // MyBossRelic item / tile ��������뵥���ṩ
}
