using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000310 RID: 784
	public static class ActiveFont
	{
		// Token: 0x170001BD RID: 445
		// (get) Token: 0x060018D2 RID: 6354 RVA: 0x0009BD12 File Offset: 0x00099F12
		public static PixelFont Font
		{
			get
			{
				return Fonts.Get(Dialog.Language.FontFace);
			}
		}

		// Token: 0x170001BE RID: 446
		// (get) Token: 0x060018D3 RID: 6355 RVA: 0x0009BD23 File Offset: 0x00099F23
		public static PixelFontSize FontSize
		{
			get
			{
				return ActiveFont.Font.Get(ActiveFont.BaseSize);
			}
		}

		// Token: 0x170001BF RID: 447
		// (get) Token: 0x060018D4 RID: 6356 RVA: 0x0009BD34 File Offset: 0x00099F34
		public static float BaseSize
		{
			get
			{
				return Dialog.Language.FontFaceSize;
			}
		}

		// Token: 0x170001C0 RID: 448
		// (get) Token: 0x060018D5 RID: 6357 RVA: 0x0009BD40 File Offset: 0x00099F40
		public static float LineHeight
		{
			get
			{
				return (float)ActiveFont.Font.Get(ActiveFont.BaseSize).LineHeight;
			}
		}

		// Token: 0x060018D6 RID: 6358 RVA: 0x0009BD57 File Offset: 0x00099F57
		public static Vector2 Measure(char text)
		{
			return ActiveFont.Font.Get(ActiveFont.BaseSize).Measure(text);
		}

		// Token: 0x060018D7 RID: 6359 RVA: 0x0009BD6E File Offset: 0x00099F6E
		public static Vector2 Measure(string text)
		{
			return ActiveFont.Font.Get(ActiveFont.BaseSize).Measure(text);
		}

		// Token: 0x060018D8 RID: 6360 RVA: 0x0009BD85 File Offset: 0x00099F85
		public static float WidthToNextLine(string text, int start)
		{
			return ActiveFont.Font.Get(ActiveFont.BaseSize).WidthToNextLine(text, start);
		}

		// Token: 0x060018D9 RID: 6361 RVA: 0x0009BD9D File Offset: 0x00099F9D
		public static float HeightOf(string text)
		{
			return ActiveFont.Font.Get(ActiveFont.BaseSize).HeightOf(text);
		}

		// Token: 0x060018DA RID: 6362 RVA: 0x0009BDB4 File Offset: 0x00099FB4
		public static void Draw(char character, Vector2 position, Vector2 justify, Vector2 scale, Color color)
		{
			ActiveFont.Font.Draw(ActiveFont.BaseSize, character, position, justify, scale, color);
		}

		// Token: 0x060018DB RID: 6363 RVA: 0x0009BDCC File Offset: 0x00099FCC
		private static void Draw(string text, Vector2 position, Vector2 justify, Vector2 scale, Color color, float edgeDepth, Color edgeColor, float stroke, Color strokeColor)
		{
			ActiveFont.Font.Draw(ActiveFont.BaseSize, text, position, justify, scale, color, edgeDepth, edgeColor, stroke, strokeColor);
		}

		// Token: 0x060018DC RID: 6364 RVA: 0x0009BDF8 File Offset: 0x00099FF8
		public static void Draw(string text, Vector2 position, Color color)
		{
			ActiveFont.Draw(text, position, Vector2.Zero, Vector2.One, color, 0f, Color.Transparent, 0f, Color.Transparent);
		}

		// Token: 0x060018DD RID: 6365 RVA: 0x0009BE2C File Offset: 0x0009A02C
		public static void Draw(string text, Vector2 position, Vector2 justify, Vector2 scale, Color color)
		{
			ActiveFont.Draw(text, position, justify, scale, color, 0f, Color.Transparent, 0f, Color.Transparent);
		}

		// Token: 0x060018DE RID: 6366 RVA: 0x0009BE58 File Offset: 0x0009A058
		public static void DrawOutline(string text, Vector2 position, Vector2 justify, Vector2 scale, Color color, float stroke, Color strokeColor)
		{
			ActiveFont.Draw(text, position, justify, scale, color, 0f, Color.Transparent, stroke, strokeColor);
		}

		// Token: 0x060018DF RID: 6367 RVA: 0x0009BE80 File Offset: 0x0009A080
		public static void DrawEdgeOutline(string text, Vector2 position, Vector2 justify, Vector2 scale, Color color, float edgeDepth, Color edgeColor, float stroke = 0f, Color strokeColor = default(Color))
		{
			ActiveFont.Draw(text, position, justify, scale, color, edgeDepth, edgeColor, stroke, strokeColor);
		}
	}
}
