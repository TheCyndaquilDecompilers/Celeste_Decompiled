using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monocle
{
	// Token: 0x02000129 RID: 297
	public static class Draw
	{
		// Token: 0x170000EA RID: 234
		// (get) Token: 0x06000AA8 RID: 2728 RVA: 0x0001C127 File Offset: 0x0001A327
		// (set) Token: 0x06000AA9 RID: 2729 RVA: 0x0001C12E File Offset: 0x0001A32E
		public static Renderer Renderer { get; internal set; }

		// Token: 0x170000EB RID: 235
		// (get) Token: 0x06000AAA RID: 2730 RVA: 0x0001C136 File Offset: 0x0001A336
		// (set) Token: 0x06000AAB RID: 2731 RVA: 0x0001C13D File Offset: 0x0001A33D
		public static SpriteBatch SpriteBatch { get; private set; }

		// Token: 0x170000EC RID: 236
		// (get) Token: 0x06000AAC RID: 2732 RVA: 0x0001C145 File Offset: 0x0001A345
		// (set) Token: 0x06000AAD RID: 2733 RVA: 0x0001C14C File Offset: 0x0001A34C
		public static SpriteFont DefaultFont { get; private set; }

		// Token: 0x06000AAE RID: 2734 RVA: 0x0001C154 File Offset: 0x0001A354
		internal static void Initialize(GraphicsDevice graphicsDevice)
		{
			Draw.SpriteBatch = new SpriteBatch(graphicsDevice);
			Draw.DefaultFont = Engine.Instance.Content.Load<SpriteFont>("Monocle\\MonocleDefault");
			Draw.UseDebugPixelTexture();
		}

		// Token: 0x06000AAF RID: 2735 RVA: 0x0001C17F File Offset: 0x0001A37F
		public static void UseDebugPixelTexture()
		{
			MTexture parent = new MTexture(VirtualContent.CreateTexture("debug-pixel", 3, 3, Color.White));
			Draw.Pixel = new MTexture(parent, 1, 1, 1, 1);
			Draw.Particle = new MTexture(parent, 1, 1, 1, 1);
		}

		// Token: 0x06000AB0 RID: 2736 RVA: 0x0001C1B4 File Offset: 0x0001A3B4
		public static void Point(Vector2 at, Color color)
		{
			Draw.SpriteBatch.Draw(Draw.Pixel.Texture.Texture, at, new Rectangle?(Draw.Pixel.ClipRect), color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
		}

		// Token: 0x06000AB1 RID: 2737 RVA: 0x0001C200 File Offset: 0x0001A400
		public static void Line(Vector2 start, Vector2 end, Color color)
		{
			Draw.LineAngle(start, Calc.Angle(start, end), Vector2.Distance(start, end), color);
		}

		// Token: 0x06000AB2 RID: 2738 RVA: 0x0001C217 File Offset: 0x0001A417
		public static void Line(Vector2 start, Vector2 end, Color color, float thickness)
		{
			Draw.LineAngle(start, Calc.Angle(start, end), Vector2.Distance(start, end), color, thickness);
		}

		// Token: 0x06000AB3 RID: 2739 RVA: 0x0001C22F File Offset: 0x0001A42F
		public static void Line(float x1, float y1, float x2, float y2, Color color)
		{
			Draw.Line(new Vector2(x1, y1), new Vector2(x2, y2), color);
		}

		// Token: 0x06000AB4 RID: 2740 RVA: 0x0001C246 File Offset: 0x0001A446
		public static void Line(float x1, float y1, float x2, float y2, Color color, float thickness)
		{
			Draw.Line(new Vector2(x1, y1), new Vector2(x2, y2), color, thickness);
		}

		// Token: 0x06000AB5 RID: 2741 RVA: 0x0001C260 File Offset: 0x0001A460
		public static void LineAngle(Vector2 start, float angle, float length, Color color)
		{
			Draw.SpriteBatch.Draw(Draw.Pixel.Texture.Texture, start, new Rectangle?(Draw.Pixel.ClipRect), color, angle, Vector2.Zero, new Vector2(length, 1f), SpriteEffects.None, 0f);
		}

		// Token: 0x06000AB6 RID: 2742 RVA: 0x0001C2B0 File Offset: 0x0001A4B0
		public static void LineAngle(Vector2 start, float angle, float length, Color color, float thickness)
		{
			Draw.SpriteBatch.Draw(Draw.Pixel.Texture.Texture, start, new Rectangle?(Draw.Pixel.ClipRect), color, angle, new Vector2(0f, 0.5f), new Vector2(length, thickness), SpriteEffects.None, 0f);
		}

		// Token: 0x06000AB7 RID: 2743 RVA: 0x0001C305 File Offset: 0x0001A505
		public static void LineAngle(float startX, float startY, float angle, float length, Color color)
		{
			Draw.LineAngle(new Vector2(startX, startY), angle, length, color);
		}

		// Token: 0x06000AB8 RID: 2744 RVA: 0x0001C318 File Offset: 0x0001A518
		public static void Circle(Vector2 position, float radius, Color color, int resolution)
		{
			Vector2 vector = Vector2.UnitX * radius;
			Vector2 value = vector.Perpendicular();
			for (int i = 1; i <= resolution; i++)
			{
				Vector2 vector2 = Calc.AngleToVector((float)i * 1.5707964f / (float)resolution, radius);
				Vector2 vector3 = vector2.Perpendicular();
				Draw.Line(position + vector, position + vector2, color);
				Draw.Line(position - vector, position - vector2, color);
				Draw.Line(position + value, position + vector3, color);
				Draw.Line(position - value, position - vector3, color);
				vector = vector2;
				value = vector3;
			}
		}

		// Token: 0x06000AB9 RID: 2745 RVA: 0x0001C3B5 File Offset: 0x0001A5B5
		public static void Circle(float x, float y, float radius, Color color, int resolution)
		{
			Draw.Circle(new Vector2(x, y), radius, color, resolution);
		}

		// Token: 0x06000ABA RID: 2746 RVA: 0x0001C3C8 File Offset: 0x0001A5C8
		public static void Circle(Vector2 position, float radius, Color color, float thickness, int resolution)
		{
			Vector2 vector = Vector2.UnitX * radius;
			Vector2 value = vector.Perpendicular();
			for (int i = 1; i <= resolution; i++)
			{
				Vector2 vector2 = Calc.AngleToVector((float)i * 1.5707964f / (float)resolution, radius);
				Vector2 vector3 = vector2.Perpendicular();
				Draw.Line(position + vector, position + vector2, color, thickness);
				Draw.Line(position - vector, position - vector2, color, thickness);
				Draw.Line(position + value, position + vector3, color, thickness);
				Draw.Line(position - value, position - vector3, color, thickness);
				vector = vector2;
				value = vector3;
			}
		}

		// Token: 0x06000ABB RID: 2747 RVA: 0x0001C46B File Offset: 0x0001A66B
		public static void Circle(float x, float y, float radius, Color color, float thickness, int resolution)
		{
			Draw.Circle(new Vector2(x, y), radius, color, thickness, resolution);
		}

		// Token: 0x06000ABC RID: 2748 RVA: 0x0001C480 File Offset: 0x0001A680
		public static void Rect(float x, float y, float width, float height, Color color)
		{
			Draw.rect.X = (int)x;
			Draw.rect.Y = (int)y;
			Draw.rect.Width = (int)width;
			Draw.rect.Height = (int)height;
			Draw.SpriteBatch.Draw(Draw.Pixel.Texture.Texture, Draw.rect, new Rectangle?(Draw.Pixel.ClipRect), color);
		}

		// Token: 0x06000ABD RID: 2749 RVA: 0x0001C4EC File Offset: 0x0001A6EC
		public static void Rect(Vector2 position, float width, float height, Color color)
		{
			Draw.Rect(position.X, position.Y, width, height, color);
		}

		// Token: 0x06000ABE RID: 2750 RVA: 0x0001C502 File Offset: 0x0001A702
		public static void Rect(Rectangle rect, Color color)
		{
			Draw.rect = rect;
			Draw.SpriteBatch.Draw(Draw.Pixel.Texture.Texture, rect, new Rectangle?(Draw.Pixel.ClipRect), color);
		}

		// Token: 0x06000ABF RID: 2751 RVA: 0x0001C534 File Offset: 0x0001A734
		public static void Rect(Collider collider, Color color)
		{
			Draw.Rect(collider.AbsoluteLeft, collider.AbsoluteTop, collider.Width, collider.Height, color);
		}

		// Token: 0x06000AC0 RID: 2752 RVA: 0x0001C554 File Offset: 0x0001A754
		public static void HollowRect(float x, float y, float width, float height, Color color)
		{
			Draw.rect.X = (int)x;
			Draw.rect.Y = (int)y;
			Draw.rect.Width = (int)width;
			Draw.rect.Height = 1;
			Draw.SpriteBatch.Draw(Draw.Pixel.Texture.Texture, Draw.rect, new Rectangle?(Draw.Pixel.ClipRect), color);
			Draw.rect.Y = Draw.rect.Y + ((int)height - 1);
			Draw.SpriteBatch.Draw(Draw.Pixel.Texture.Texture, Draw.rect, new Rectangle?(Draw.Pixel.ClipRect), color);
			Draw.rect.Y = Draw.rect.Y - ((int)height - 1);
			Draw.rect.Width = 1;
			Draw.rect.Height = (int)height;
			Draw.SpriteBatch.Draw(Draw.Pixel.Texture.Texture, Draw.rect, new Rectangle?(Draw.Pixel.ClipRect), color);
			Draw.rect.X = Draw.rect.X + ((int)width - 1);
			Draw.SpriteBatch.Draw(Draw.Pixel.Texture.Texture, Draw.rect, new Rectangle?(Draw.Pixel.ClipRect), color);
		}

		// Token: 0x06000AC1 RID: 2753 RVA: 0x0001C699 File Offset: 0x0001A899
		public static void HollowRect(Vector2 position, float width, float height, Color color)
		{
			Draw.HollowRect(position.X, position.Y, width, height, color);
		}

		// Token: 0x06000AC2 RID: 2754 RVA: 0x0001C6AF File Offset: 0x0001A8AF
		public static void HollowRect(Rectangle rect, Color color)
		{
			Draw.HollowRect((float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height, color);
		}

		// Token: 0x06000AC3 RID: 2755 RVA: 0x0001C6D3 File Offset: 0x0001A8D3
		public static void HollowRect(Collider collider, Color color)
		{
			Draw.HollowRect(collider.AbsoluteLeft, collider.AbsoluteTop, collider.Width, collider.Height, color);
		}

		// Token: 0x06000AC4 RID: 2756 RVA: 0x0001C6F3 File Offset: 0x0001A8F3
		public static void Text(SpriteFont font, string text, Vector2 position, Color color)
		{
			Draw.SpriteBatch.DrawString(font, text, position.Floor(), color);
		}

		// Token: 0x06000AC5 RID: 2757 RVA: 0x0001C708 File Offset: 0x0001A908
		public static void Text(SpriteFont font, string text, Vector2 position, Color color, Vector2 origin, Vector2 scale, float rotation)
		{
			Draw.SpriteBatch.DrawString(font, text, position.Floor(), color, rotation, origin, scale, SpriteEffects.None, 0f);
		}

		// Token: 0x06000AC6 RID: 2758 RVA: 0x0001C734 File Offset: 0x0001A934
		public static void TextJustified(SpriteFont font, string text, Vector2 position, Color color, Vector2 justify)
		{
			Vector2 origin = font.MeasureString(text);
			origin.X *= justify.X;
			origin.Y *= justify.Y;
			Draw.SpriteBatch.DrawString(font, text, position.Floor(), color, 0f, origin, 1f, SpriteEffects.None, 0f);
		}

		// Token: 0x06000AC7 RID: 2759 RVA: 0x0001C794 File Offset: 0x0001A994
		public static void TextJustified(SpriteFont font, string text, Vector2 position, Color color, float scale, Vector2 justify)
		{
			Vector2 origin = font.MeasureString(text);
			origin.X *= justify.X;
			origin.Y *= justify.Y;
			Draw.SpriteBatch.DrawString(font, text, position.Floor(), color, 0f, origin, scale, SpriteEffects.None, 0f);
		}

		// Token: 0x06000AC8 RID: 2760 RVA: 0x0001C7EE File Offset: 0x0001A9EE
		public static void TextCentered(SpriteFont font, string text, Vector2 position)
		{
			Draw.Text(font, text, position - font.MeasureString(text) * 0.5f, Color.White);
		}

		// Token: 0x06000AC9 RID: 2761 RVA: 0x0001C813 File Offset: 0x0001AA13
		public static void TextCentered(SpriteFont font, string text, Vector2 position, Color color)
		{
			Draw.Text(font, text, position - font.MeasureString(text) * 0.5f, color);
		}

		// Token: 0x06000ACA RID: 2762 RVA: 0x0001C834 File Offset: 0x0001AA34
		public static void TextCentered(SpriteFont font, string text, Vector2 position, Color color, float scale)
		{
			Draw.Text(font, text, position, color, font.MeasureString(text) * 0.5f, Vector2.One * scale, 0f);
		}

		// Token: 0x06000ACB RID: 2763 RVA: 0x0001C861 File Offset: 0x0001AA61
		public static void TextCentered(SpriteFont font, string text, Vector2 position, Color color, float scale, float rotation)
		{
			Draw.Text(font, text, position, color, font.MeasureString(text) * 0.5f, Vector2.One * scale, rotation);
		}

		// Token: 0x06000ACC RID: 2764 RVA: 0x0001C88C File Offset: 0x0001AA8C
		public static void OutlineTextCentered(SpriteFont font, string text, Vector2 position, Color color, float scale)
		{
			Vector2 origin = font.MeasureString(text) / 2f;
			for (int i = -1; i < 2; i++)
			{
				for (int j = -1; j < 2; j++)
				{
					if (i != 0 || j != 0)
					{
						Draw.SpriteBatch.DrawString(font, text, position.Floor() + new Vector2((float)i, (float)j), Color.Black, 0f, origin, scale, SpriteEffects.None, 0f);
					}
				}
			}
			Draw.SpriteBatch.DrawString(font, text, position.Floor(), color, 0f, origin, scale, SpriteEffects.None, 0f);
		}

		// Token: 0x06000ACD RID: 2765 RVA: 0x0001C920 File Offset: 0x0001AB20
		public static void OutlineTextCentered(SpriteFont font, string text, Vector2 position, Color color, Color outlineColor)
		{
			Vector2 origin = font.MeasureString(text) / 2f;
			for (int i = -1; i < 2; i++)
			{
				for (int j = -1; j < 2; j++)
				{
					if (i != 0 || j != 0)
					{
						Draw.SpriteBatch.DrawString(font, text, position.Floor() + new Vector2((float)i, (float)j), outlineColor, 0f, origin, 1f, SpriteEffects.None, 0f);
					}
				}
			}
			Draw.SpriteBatch.DrawString(font, text, position.Floor(), color, 0f, origin, 1f, SpriteEffects.None, 0f);
		}

		// Token: 0x06000ACE RID: 2766 RVA: 0x0001C9B4 File Offset: 0x0001ABB4
		public static void OutlineTextCentered(SpriteFont font, string text, Vector2 position, Color color, Color outlineColor, float scale)
		{
			Vector2 origin = font.MeasureString(text) / 2f;
			for (int i = -1; i < 2; i++)
			{
				for (int j = -1; j < 2; j++)
				{
					if (i != 0 || j != 0)
					{
						Draw.SpriteBatch.DrawString(font, text, position.Floor() + new Vector2((float)i, (float)j), outlineColor, 0f, origin, scale, SpriteEffects.None, 0f);
					}
				}
			}
			Draw.SpriteBatch.DrawString(font, text, position.Floor(), color, 0f, origin, scale, SpriteEffects.None, 0f);
		}

		// Token: 0x06000ACF RID: 2767 RVA: 0x0001CA44 File Offset: 0x0001AC44
		public static void OutlineTextJustify(SpriteFont font, string text, Vector2 position, Color color, Color outlineColor, Vector2 justify)
		{
			Vector2 origin = font.MeasureString(text) * justify;
			for (int i = -1; i < 2; i++)
			{
				for (int j = -1; j < 2; j++)
				{
					if (i != 0 || j != 0)
					{
						Draw.SpriteBatch.DrawString(font, text, position.Floor() + new Vector2((float)i, (float)j), outlineColor, 0f, origin, 1f, SpriteEffects.None, 0f);
					}
				}
			}
			Draw.SpriteBatch.DrawString(font, text, position.Floor(), color, 0f, origin, 1f, SpriteEffects.None, 0f);
		}

		// Token: 0x06000AD0 RID: 2768 RVA: 0x0001CAD8 File Offset: 0x0001ACD8
		public static void OutlineTextJustify(SpriteFont font, string text, Vector2 position, Color color, Color outlineColor, Vector2 justify, float scale)
		{
			Vector2 origin = font.MeasureString(text) * justify;
			for (int i = -1; i < 2; i++)
			{
				for (int j = -1; j < 2; j++)
				{
					if (i != 0 || j != 0)
					{
						Draw.SpriteBatch.DrawString(font, text, position.Floor() + new Vector2((float)i, (float)j), outlineColor, 0f, origin, scale, SpriteEffects.None, 0f);
					}
				}
			}
			Draw.SpriteBatch.DrawString(font, text, position.Floor(), color, 0f, origin, scale, SpriteEffects.None, 0f);
		}

		// Token: 0x06000AD1 RID: 2769 RVA: 0x0001CB64 File Offset: 0x0001AD64
		public static void SineTextureH(MTexture tex, Vector2 position, Vector2 origin, Vector2 scale, float rotation, Color color, SpriteEffects effects, float sineCounter, float amplitude = 2f, int sliceSize = 2, float sliceAdd = 0.7853982f)
		{
			position = position.Floor();
			Rectangle clipRect = tex.ClipRect;
			clipRect.Width = sliceSize;
			int num = 0;
			while (clipRect.X < tex.ClipRect.X + tex.ClipRect.Width)
			{
				Vector2 value = new Vector2((float)(sliceSize * num), (float)Math.Round(Math.Sin((double)(sineCounter + sliceAdd * (float)num)) * (double)amplitude));
				Draw.SpriteBatch.Draw(tex.Texture.Texture, position, new Rectangle?(clipRect), color, rotation, origin - value, scale, effects, 0f);
				num++;
				clipRect.X += sliceSize;
				clipRect.Width = Math.Min(sliceSize, tex.ClipRect.X + tex.ClipRect.Width - clipRect.X);
			}
		}

		// Token: 0x06000AD2 RID: 2770 RVA: 0x0001CC44 File Offset: 0x0001AE44
		public static void SineTextureV(MTexture tex, Vector2 position, Vector2 origin, Vector2 scale, float rotation, Color color, SpriteEffects effects, float sineCounter, float amplitude = 2f, int sliceSize = 2, float sliceAdd = 0.7853982f)
		{
			position = position.Floor();
			Rectangle clipRect = tex.ClipRect;
			clipRect.Height = sliceSize;
			int num = 0;
			while (clipRect.Y < tex.ClipRect.Y + tex.ClipRect.Height)
			{
				Vector2 value = new Vector2((float)Math.Round(Math.Sin((double)(sineCounter + sliceAdd * (float)num)) * (double)amplitude), (float)(sliceSize * num));
				Draw.SpriteBatch.Draw(tex.Texture.Texture, position, new Rectangle?(clipRect), color, rotation, origin - value, scale, effects, 0f);
				num++;
				clipRect.Y += sliceSize;
				clipRect.Height = Math.Min(sliceSize, tex.ClipRect.Y + tex.ClipRect.Height - clipRect.Y);
			}
		}

		// Token: 0x06000AD3 RID: 2771 RVA: 0x0001CD24 File Offset: 0x0001AF24
		public static void TextureBannerV(MTexture tex, Vector2 position, Vector2 origin, Vector2 scale, float rotation, Color color, SpriteEffects effects, float sineCounter, float amplitude = 2f, int sliceSize = 2, float sliceAdd = 0.7853982f)
		{
			position = position.Floor();
			Rectangle clipRect = tex.ClipRect;
			clipRect.Height = sliceSize;
			int num = 0;
			while (clipRect.Y < tex.ClipRect.Y + tex.ClipRect.Height)
			{
				float num2 = (float)(clipRect.Y - tex.ClipRect.Y) / (float)tex.ClipRect.Height;
				clipRect.Height = (int)MathHelper.Lerp((float)sliceSize, 1f, num2);
				clipRect.Height = Math.Min(sliceSize, tex.ClipRect.Y + tex.ClipRect.Height - clipRect.Y);
				Vector2 value = new Vector2((float)Math.Round(Math.Sin((double)(sineCounter + sliceAdd * (float)num)) * (double)amplitude * (double)num2), (float)(clipRect.Y - tex.ClipRect.Y));
				Draw.SpriteBatch.Draw(tex.Texture.Texture, position, new Rectangle?(clipRect), color, rotation, origin - value, scale, effects, 0f);
				num++;
				clipRect.Y += clipRect.Height;
			}
		}

		// Token: 0x0400065F RID: 1631
		public static MTexture Particle;

		// Token: 0x04000660 RID: 1632
		public static MTexture Pixel;

		// Token: 0x04000661 RID: 1633
		private static Rectangle rect;
	}
}
