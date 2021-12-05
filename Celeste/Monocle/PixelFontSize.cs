using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;

namespace Monocle
{
	// Token: 0x0200012E RID: 302
	public class PixelFontSize
	{
		// Token: 0x06000ADE RID: 2782 RVA: 0x0001D30C File Offset: 0x0001B50C
		public string AutoNewline(string text, int width)
		{
			if (string.IsNullOrEmpty(text))
			{
				return text;
			}
			this.temp.Clear();
			string[] array = Regex.Split(text, "(\\s)");
			float num = 0f;
			string[] array2 = array;
			int i = 0;
			while (i < array2.Length)
			{
				string text2 = array2[i];
				float x = this.Measure(text2).X;
				if (x + num <= (float)width)
				{
					goto IL_6D;
				}
				this.temp.Append('\n');
				num = 0f;
				if (!text2.Equals(" "))
				{
					goto IL_6D;
				}
				IL_128:
				i++;
				continue;
				IL_6D:
				if (x > (float)width)
				{
					int j = 1;
					int num2 = 0;
					while (j < text2.Length)
					{
						if (j - num2 > 1 && this.Measure(text2.Substring(num2, j - num2 - 1)).X > (float)width)
						{
							this.temp.Append(text2.Substring(num2, j - num2 - 1));
							this.temp.Append('\n');
							num2 = j - 1;
						}
						j++;
					}
					string text3 = text2.Substring(num2, text2.Length - num2);
					this.temp.Append(text3);
					num += this.Measure(text3).X;
					goto IL_128;
				}
				num += x;
				this.temp.Append(text2);
				goto IL_128;
			}
			return this.temp.ToString();
		}

		// Token: 0x06000ADF RID: 2783 RVA: 0x0001D45C File Offset: 0x0001B65C
		public PixelFontCharacter Get(int id)
		{
			PixelFontCharacter result = null;
			if (this.Characters.TryGetValue(id, out result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x06000AE0 RID: 2784 RVA: 0x0001D480 File Offset: 0x0001B680
		public Vector2 Measure(char text)
		{
			PixelFontCharacter pixelFontCharacter = null;
			if (this.Characters.TryGetValue((int)text, out pixelFontCharacter))
			{
				return new Vector2((float)pixelFontCharacter.XAdvance, (float)this.LineHeight);
			}
			return Vector2.Zero;
		}

		// Token: 0x06000AE1 RID: 2785 RVA: 0x0001D4B8 File Offset: 0x0001B6B8
		public Vector2 Measure(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return Vector2.Zero;
			}
			Vector2 vector = new Vector2(0f, (float)this.LineHeight);
			float num = 0f;
			for (int i = 0; i < text.Length; i++)
			{
				if (text[i] == '\n')
				{
					vector.Y += (float)this.LineHeight;
					if (num > vector.X)
					{
						vector.X = num;
					}
					num = 0f;
				}
				else
				{
					PixelFontCharacter pixelFontCharacter = null;
					if (this.Characters.TryGetValue((int)text[i], out pixelFontCharacter))
					{
						num += (float)pixelFontCharacter.XAdvance;
						int num2;
						if (i < text.Length - 1 && pixelFontCharacter.Kerning.TryGetValue((int)text[i + 1], out num2))
						{
							num += (float)num2;
						}
					}
				}
			}
			if (num > vector.X)
			{
				vector.X = num;
			}
			return vector;
		}

		// Token: 0x06000AE2 RID: 2786 RVA: 0x0001D598 File Offset: 0x0001B798
		public float WidthToNextLine(string text, int start)
		{
			if (string.IsNullOrEmpty(text))
			{
				return 0f;
			}
			float num = 0f;
			int num2 = start;
			int length = text.Length;
			while (num2 < length && text[num2] != '\n')
			{
				PixelFontCharacter pixelFontCharacter = null;
				if (this.Characters.TryGetValue((int)text[num2], out pixelFontCharacter))
				{
					num += (float)pixelFontCharacter.XAdvance;
					int num3;
					if (num2 < length - 1 && pixelFontCharacter.Kerning.TryGetValue((int)text[num2 + 1], out num3))
					{
						num += (float)num3;
					}
				}
				num2++;
			}
			return num;
		}

		// Token: 0x06000AE3 RID: 2787 RVA: 0x0001D620 File Offset: 0x0001B820
		public float HeightOf(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return 0f;
			}
			int num = 1;
			if (text.IndexOf('\n') >= 0)
			{
				for (int i = 0; i < text.Length; i++)
				{
					if (text[i] == '\n')
					{
						num++;
					}
				}
			}
			return (float)(num * this.LineHeight);
		}

		// Token: 0x06000AE4 RID: 2788 RVA: 0x0001D674 File Offset: 0x0001B874
		public void Draw(char character, Vector2 position, Vector2 justify, Vector2 scale, Color color)
		{
			if (char.IsWhiteSpace(character))
			{
				return;
			}
			PixelFontCharacter pixelFontCharacter = null;
			if (this.Characters.TryGetValue((int)character, out pixelFontCharacter))
			{
				Vector2 vector = this.Measure(character);
				Vector2 value = new Vector2(vector.X * justify.X, vector.Y * justify.Y);
				Vector2 val = position + (new Vector2((float)pixelFontCharacter.XOffset, (float)pixelFontCharacter.YOffset) - value) * scale;
				pixelFontCharacter.Texture.Draw(val.Floor(), Vector2.Zero, color, scale);
			}
		}

		// Token: 0x06000AE5 RID: 2789 RVA: 0x0001D708 File Offset: 0x0001B908
		public void Draw(string text, Vector2 position, Vector2 justify, Vector2 scale, Color color, float edgeDepth, Color edgeColor, float stroke, Color strokeColor)
		{
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			Vector2 zero = Vector2.Zero;
			float num = (justify.X != 0f) ? this.WidthToNextLine(text, 0) : 0f;
			Vector2 value = new Vector2(num * justify.X, this.HeightOf(text) * justify.Y);
			for (int i = 0; i < text.Length; i++)
			{
				if (text[i] == '\n')
				{
					zero.X = 0f;
					zero.Y += (float)this.LineHeight;
					if (justify.X != 0f)
					{
						value.X = this.WidthToNextLine(text, i + 1) * justify.X;
					}
				}
				else
				{
					PixelFontCharacter pixelFontCharacter = null;
					if (this.Characters.TryGetValue((int)text[i], out pixelFontCharacter))
					{
						Vector2 vector = position + (zero + new Vector2((float)pixelFontCharacter.XOffset, (float)pixelFontCharacter.YOffset) - value) * scale;
						if (stroke > 0f && !this.Outline)
						{
							if (edgeDepth > 0f)
							{
								pixelFontCharacter.Texture.Draw(vector + new Vector2(0f, -stroke), Vector2.Zero, strokeColor, scale);
								for (float num2 = -stroke; num2 < edgeDepth + stroke; num2 += stroke)
								{
									pixelFontCharacter.Texture.Draw(vector + new Vector2(-stroke, num2), Vector2.Zero, strokeColor, scale);
									pixelFontCharacter.Texture.Draw(vector + new Vector2(stroke, num2), Vector2.Zero, strokeColor, scale);
								}
								pixelFontCharacter.Texture.Draw(vector + new Vector2(-stroke, edgeDepth + stroke), Vector2.Zero, strokeColor, scale);
								pixelFontCharacter.Texture.Draw(vector + new Vector2(0f, edgeDepth + stroke), Vector2.Zero, strokeColor, scale);
								pixelFontCharacter.Texture.Draw(vector + new Vector2(stroke, edgeDepth + stroke), Vector2.Zero, strokeColor, scale);
							}
							else
							{
								pixelFontCharacter.Texture.Draw(vector + new Vector2(-1f, -1f) * stroke, Vector2.Zero, strokeColor, scale);
								pixelFontCharacter.Texture.Draw(vector + new Vector2(0f, -1f) * stroke, Vector2.Zero, strokeColor, scale);
								pixelFontCharacter.Texture.Draw(vector + new Vector2(1f, -1f) * stroke, Vector2.Zero, strokeColor, scale);
								pixelFontCharacter.Texture.Draw(vector + new Vector2(-1f, 0f) * stroke, Vector2.Zero, strokeColor, scale);
								pixelFontCharacter.Texture.Draw(vector + new Vector2(1f, 0f) * stroke, Vector2.Zero, strokeColor, scale);
								pixelFontCharacter.Texture.Draw(vector + new Vector2(-1f, 1f) * stroke, Vector2.Zero, strokeColor, scale);
								pixelFontCharacter.Texture.Draw(vector + new Vector2(0f, 1f) * stroke, Vector2.Zero, strokeColor, scale);
								pixelFontCharacter.Texture.Draw(vector + new Vector2(1f, 1f) * stroke, Vector2.Zero, strokeColor, scale);
							}
						}
						if (edgeDepth > 0f)
						{
							pixelFontCharacter.Texture.Draw(vector + Vector2.UnitY * edgeDepth, Vector2.Zero, edgeColor, scale);
						}
						pixelFontCharacter.Texture.Draw(vector, Vector2.Zero, color, scale);
						zero.X += (float)pixelFontCharacter.XAdvance;
						int num3;
						if (i < text.Length - 1 && pixelFontCharacter.Kerning.TryGetValue((int)text[i + 1], out num3))
						{
							zero.X += (float)num3;
						}
					}
				}
			}
		}

		// Token: 0x06000AE6 RID: 2790 RVA: 0x0001DB64 File Offset: 0x0001BD64
		public void Draw(string text, Vector2 position, Color color)
		{
			this.Draw(text, position, Vector2.Zero, Vector2.One, color, 0f, Color.Transparent, 0f, Color.Transparent);
		}

		// Token: 0x06000AE7 RID: 2791 RVA: 0x0001DB98 File Offset: 0x0001BD98
		public void Draw(string text, Vector2 position, Vector2 justify, Vector2 scale, Color color)
		{
			this.Draw(text, position, justify, scale, color, 0f, Color.Transparent, 0f, Color.Transparent);
		}

		// Token: 0x06000AE8 RID: 2792 RVA: 0x0001DBC8 File Offset: 0x0001BDC8
		public void DrawOutline(string text, Vector2 position, Vector2 justify, Vector2 scale, Color color, float stroke, Color strokeColor)
		{
			this.Draw(text, position, justify, scale, color, 0f, Color.Transparent, stroke, strokeColor);
		}

		// Token: 0x06000AE9 RID: 2793 RVA: 0x0001DBF0 File Offset: 0x0001BDF0
		public void DrawEdgeOutline(string text, Vector2 position, Vector2 justify, Vector2 scale, Color color, float edgeDepth, Color edgeColor, float stroke = 0f, Color strokeColor = default(Color))
		{
			this.Draw(text, position, justify, scale, color, edgeDepth, edgeColor, stroke, strokeColor);
		}

		// Token: 0x0400068D RID: 1677
		public List<MTexture> Textures;

		// Token: 0x0400068E RID: 1678
		public Dictionary<int, PixelFontCharacter> Characters;

		// Token: 0x0400068F RID: 1679
		public int LineHeight;

		// Token: 0x04000690 RID: 1680
		public float Size;

		// Token: 0x04000691 RID: 1681
		public bool Outline;

		// Token: 0x04000692 RID: 1682
		private StringBuilder temp = new StringBuilder();
	}
}
