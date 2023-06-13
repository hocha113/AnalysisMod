using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

// ATTENTION: Below this point is custom config UI element.
// Be aware that mods using custom config elements will break with the next few tModLoader updates until their design is finalized.
// You will need to be very active in updating your mod if you use these as they can break in any update.

// This file defines a custom ConfigElement based on Corner enum
// with custom drawing implemented that can be used in ModConfig classes.

// 注意：以下内容是自定义配置UI元素。
// 请注意，使用自定义配置元素的模组将在接下来的几个tModLoader更新中出现问题，直到它们的设计被最终确定。
// 如果您使用这些元素，则需要非常积极地更新您的模组，因为它们可能会在任何更新中出现问题。

// 此文件定义了基于Corner枚举的自定义ConfigElement，
// 并实现了可用于ModConfig类中的自定义绘制。

namespace AnalysisMod.AnalysisCommon.Configs.CustomUI
{
    // This custom config UI element shows a completely custom config element that handles setting and getting the values in addition to custom drawing.
    // 这个自定义配置 UI 元素展示了一个完全定制的配置元素，它处理设置和获取值以及自定义绘图。
    [JsonConverter(typeof(StringEnumConverter))]
	[CustomModConfigItem(typeof(CornerElement))]
	public enum Corner
	{
		TopLeft,
		TopRight,
		BottomLeft,
		BottomRight
	}

	class CornerElement : ConfigElement
	{
		Texture2D circleTexture;
		string[] valueStrings;

		public override void OnBind() {
			base.OnBind();
			circleTexture = Main.Assets.Request<Texture2D>("Images/UI/Settings_Toggle", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			valueStrings = Enum.GetNames(MemberInfo.Type);
			TextDisplayFunction = () => Label + ": " + GetStringValue();
		}

		void SetValue(Corner value) => SetObject(value);

		Corner GetValue() => (Corner)GetObject();

		string GetStringValue() {
			return valueStrings[(int)GetValue()];
		}

		public override void LeftClick(UIMouseEvent evt) {
			base.LeftClick(evt);
			SetValue(GetValue().NextEnum());
		}

		public override void RightClick(UIMouseEvent evt) {
			base.RightClick(evt);
			SetValue(GetValue().PreviousEnum());
		}

		public override void Draw(SpriteBatch spriteBatch) {
			base.Draw(spriteBatch);
			CalculatedStyle dimensions = GetDimensions();
			var circleSourceRectangle = new Rectangle(0, 0, (circleTexture.Width - 2) / 2, circleTexture.Height);
			spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)(dimensions.X + dimensions.Width - 25), (int)(dimensions.Y + 4), 22, 22), Color.LightGreen);
			Corner corner = GetValue();
			var circlePositionOffset = new Vector2((int)corner % 2 * 8, (int)corner / 2 * 8);
			spriteBatch.Draw(circleTexture, new Vector2(dimensions.X + dimensions.Width - 25, dimensions.Y + 4) + circlePositionOffset, circleSourceRectangle, Color.White);
		}
	}
}
