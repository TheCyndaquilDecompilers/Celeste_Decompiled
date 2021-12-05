using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Monocle
{
	// Token: 0x020000F3 RID: 243
	public class PixelText : Component
	{
		// Token: 0x1700004B RID: 75
		// (get) Token: 0x06000636 RID: 1590 RVA: 0x0000958F File Offset: 0x0000778F
		// (set) Token: 0x06000637 RID: 1591 RVA: 0x00009597 File Offset: 0x00007797
		public PixelFont Font
		{
			get
			{
				return this.font;
			}
			set
			{
				if (value != this.font)
				{
					this.dirty = true;
				}
				this.font = value;
			}
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x06000638 RID: 1592 RVA: 0x000095B0 File Offset: 0x000077B0
		// (set) Token: 0x06000639 RID: 1593 RVA: 0x000095BD File Offset: 0x000077BD
		public float Size
		{
			get
			{
				return this.size.Size;
			}
			set
			{
				if (value != this.size.Size)
				{
					this.dirty = true;
				}
				this.size = this.font.Get(value);
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x0600063A RID: 1594 RVA: 0x000095E6 File Offset: 0x000077E6
		// (set) Token: 0x0600063B RID: 1595 RVA: 0x000095EE File Offset: 0x000077EE
		public string Text
		{
			get
			{
				return this.text;
			}
			set
			{
				if (value != this.text)
				{
					this.dirty = true;
				}
				this.text = value;
			}
		}

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x0600063C RID: 1596 RVA: 0x0000960C File Offset: 0x0000780C
		// (set) Token: 0x0600063D RID: 1597 RVA: 0x00009614 File Offset: 0x00007814
		public int Width { get; private set; }

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x0600063E RID: 1598 RVA: 0x0000961D File Offset: 0x0000781D
		// (set) Token: 0x0600063F RID: 1599 RVA: 0x00009625 File Offset: 0x00007825
		public int Height { get; private set; }

		// Token: 0x06000640 RID: 1600 RVA: 0x00009630 File Offset: 0x00007830
		public PixelText(PixelFont font, string text, Color color) : base(false, true)
		{
			this.Font = font;
			this.Text = text;
			this.Color = color;
			this.Text = text;
			this.size = this.Font.Sizes[0];
			this.Refresh();
		}

		// Token: 0x06000641 RID: 1601 RVA: 0x000096A0 File Offset: 0x000078A0
		public void Refresh()
		{
			this.dirty = false;
			this.characters.Clear();
			int num = 0;
			int num2 = 1;
			Vector2 zero = Vector2.Zero;
			for (int i = 0; i < this.text.Length; i++)
			{
				if (this.text[i] == '\n')
				{
					zero.X = 0f;
					zero.Y += (float)this.size.LineHeight;
					num2++;
				}
				PixelFontCharacter pixelFontCharacter = this.size.Get((int)this.text[i]);
				if (pixelFontCharacter != null)
				{
					this.characters.Add(new PixelText.Char
					{
						Offset = zero + new Vector2((float)pixelFontCharacter.XOffset, (float)pixelFontCharacter.YOffset),
						CharData = pixelFontCharacter,
						Bounds = pixelFontCharacter.Texture.ClipRect
					});
					if (zero.X > (float)num)
					{
						num = (int)zero.X;
					}
					zero.X += (float)pixelFontCharacter.XAdvance;
				}
			}
			this.Width = num;
			this.Height = num2 * this.size.LineHeight;
		}

		// Token: 0x06000642 RID: 1602 RVA: 0x000097CC File Offset: 0x000079CC
		public override void Render()
		{
			if (this.dirty)
			{
				this.Refresh();
			}
			for (int i = 0; i < this.characters.Count; i++)
			{
				this.characters[i].CharData.Texture.Draw(this.Position + this.characters[i].Offset, Vector2.Zero, this.Color);
			}
		}

		// Token: 0x040004B5 RID: 1205
		private List<PixelText.Char> characters = new List<PixelText.Char>();

		// Token: 0x040004B6 RID: 1206
		private PixelFont font;

		// Token: 0x040004B7 RID: 1207
		private PixelFontSize size;

		// Token: 0x040004B8 RID: 1208
		private string text;

		// Token: 0x040004B9 RID: 1209
		private bool dirty;

		// Token: 0x040004BA RID: 1210
		public Vector2 Position;

		// Token: 0x040004BB RID: 1211
		public Color Color = Color.White;

		// Token: 0x040004BC RID: 1212
		public Vector2 Scale = Vector2.One;

		// Token: 0x02000398 RID: 920
		private struct Char
		{
			// Token: 0x04001EE5 RID: 7909
			public Vector2 Offset;

			// Token: 0x04001EE6 RID: 7910
			public PixelFontCharacter CharData;

			// Token: 0x04001EE7 RID: 7911
			public Rectangle Bounds;
		}
	}
}
