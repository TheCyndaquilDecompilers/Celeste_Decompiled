using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monocle
{
	// Token: 0x02000108 RID: 264
	public class MTexture
	{
		// Token: 0x060007E6 RID: 2022 RVA: 0x000026FC File Offset: 0x000008FC
		public MTexture()
		{
		}

		// Token: 0x060007E7 RID: 2023 RVA: 0x0000F9EC File Offset: 0x0000DBEC
		public MTexture(VirtualTexture texture)
		{
			this.Texture = texture;
			this.AtlasPath = null;
			this.ClipRect = new Rectangle(0, 0, this.Texture.Width, this.Texture.Height);
			this.DrawOffset = Vector2.Zero;
			this.Width = this.ClipRect.Width;
			this.Height = this.ClipRect.Height;
			this.SetUtil();
		}

		// Token: 0x060007E8 RID: 2024 RVA: 0x0000FA64 File Offset: 0x0000DC64
		public MTexture(MTexture parent, int x, int y, int width, int height)
		{
			this.Texture = parent.Texture;
			this.AtlasPath = null;
			this.ClipRect = parent.GetRelativeRect(x, y, width, height);
			this.DrawOffset = new Vector2(-Math.Min((float)x - parent.DrawOffset.X, 0f), -Math.Min((float)y - parent.DrawOffset.Y, 0f));
			this.Width = width;
			this.Height = height;
			this.SetUtil();
		}

		// Token: 0x060007E9 RID: 2025 RVA: 0x0000FAEF File Offset: 0x0000DCEF
		public MTexture(MTexture parent, Rectangle clipRect) : this(parent, clipRect.X, clipRect.Y, clipRect.Width, clipRect.Height)
		{
		}

		// Token: 0x060007EA RID: 2026 RVA: 0x0000FB10 File Offset: 0x0000DD10
		public MTexture(MTexture parent, string atlasPath, Rectangle clipRect, Vector2 drawOffset, int width, int height)
		{
			this.Texture = parent.Texture;
			this.AtlasPath = atlasPath;
			this.ClipRect = parent.GetRelativeRect(clipRect);
			this.DrawOffset = drawOffset;
			this.Width = width;
			this.Height = height;
			this.SetUtil();
		}

		// Token: 0x060007EB RID: 2027 RVA: 0x0000FB61 File Offset: 0x0000DD61
		public MTexture(MTexture parent, string atlasPath, Rectangle clipRect) : this(parent, clipRect)
		{
			this.AtlasPath = atlasPath;
		}

		// Token: 0x060007EC RID: 2028 RVA: 0x0000FB74 File Offset: 0x0000DD74
		public MTexture(VirtualTexture texture, Vector2 drawOffset, int frameWidth, int frameHeight)
		{
			this.Texture = texture;
			this.ClipRect = new Rectangle(0, 0, texture.Width, texture.Height);
			this.DrawOffset = drawOffset;
			this.Width = frameWidth;
			this.Height = frameHeight;
			this.SetUtil();
		}

		// Token: 0x060007ED RID: 2029 RVA: 0x0000FBC4 File Offset: 0x0000DDC4
		private void SetUtil()
		{
			this.Center = new Vector2((float)this.Width, (float)this.Height) * 0.5f;
			this.LeftUV = (float)this.ClipRect.Left / (float)this.Texture.Width;
			this.RightUV = (float)this.ClipRect.Right / (float)this.Texture.Width;
			this.TopUV = (float)this.ClipRect.Top / (float)this.Texture.Height;
			this.BottomUV = (float)this.ClipRect.Bottom / (float)this.Texture.Height;
		}

		// Token: 0x060007EE RID: 2030 RVA: 0x0000FC7C File Offset: 0x0000DE7C
		public void Unload()
		{
			this.Texture.Dispose();
			this.Texture = null;
		}

		// Token: 0x060007EF RID: 2031 RVA: 0x0000FC90 File Offset: 0x0000DE90
		public MTexture GetSubtexture(int x, int y, int width, int height, MTexture applyTo = null)
		{
			if (applyTo == null)
			{
				return new MTexture(this, x, y, width, height);
			}
			applyTo.Texture = this.Texture;
			applyTo.AtlasPath = null;
			applyTo.ClipRect = this.GetRelativeRect(x, y, width, height);
			applyTo.DrawOffset = new Vector2(-Math.Min((float)x - this.DrawOffset.X, 0f), -Math.Min((float)y - this.DrawOffset.Y, 0f));
			applyTo.Width = width;
			applyTo.Height = height;
			applyTo.SetUtil();
			return applyTo;
		}

		// Token: 0x060007F0 RID: 2032 RVA: 0x0000FD2C File Offset: 0x0000DF2C
		public MTexture GetSubtexture(Rectangle rect)
		{
			return new MTexture(this, rect);
		}

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x060007F1 RID: 2033 RVA: 0x0000FD35 File Offset: 0x0000DF35
		// (set) Token: 0x060007F2 RID: 2034 RVA: 0x0000FD3D File Offset: 0x0000DF3D
		public VirtualTexture Texture { get; private set; }

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x060007F3 RID: 2035 RVA: 0x0000FD46 File Offset: 0x0000DF46
		// (set) Token: 0x060007F4 RID: 2036 RVA: 0x0000FD4E File Offset: 0x0000DF4E
		public Rectangle ClipRect { get; private set; }

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x060007F5 RID: 2037 RVA: 0x0000FD57 File Offset: 0x0000DF57
		// (set) Token: 0x060007F6 RID: 2038 RVA: 0x0000FD5F File Offset: 0x0000DF5F
		public Vector2 DrawOffset { get; private set; }

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x060007F7 RID: 2039 RVA: 0x0000FD68 File Offset: 0x0000DF68
		// (set) Token: 0x060007F8 RID: 2040 RVA: 0x0000FD70 File Offset: 0x0000DF70
		public int Width { get; private set; }

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x060007F9 RID: 2041 RVA: 0x0000FD79 File Offset: 0x0000DF79
		// (set) Token: 0x060007FA RID: 2042 RVA: 0x0000FD81 File Offset: 0x0000DF81
		public int Height { get; private set; }

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x060007FB RID: 2043 RVA: 0x0000FD8A File Offset: 0x0000DF8A
		// (set) Token: 0x060007FC RID: 2044 RVA: 0x0000FD92 File Offset: 0x0000DF92
		public Vector2 Center { get; private set; }

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x060007FD RID: 2045 RVA: 0x0000FD9B File Offset: 0x0000DF9B
		// (set) Token: 0x060007FE RID: 2046 RVA: 0x0000FDA3 File Offset: 0x0000DFA3
		public float LeftUV { get; private set; }

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x060007FF RID: 2047 RVA: 0x0000FDAC File Offset: 0x0000DFAC
		// (set) Token: 0x06000800 RID: 2048 RVA: 0x0000FDB4 File Offset: 0x0000DFB4
		public float RightUV { get; private set; }

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x06000801 RID: 2049 RVA: 0x0000FDBD File Offset: 0x0000DFBD
		// (set) Token: 0x06000802 RID: 2050 RVA: 0x0000FDC5 File Offset: 0x0000DFC5
		public float TopUV { get; private set; }

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x06000803 RID: 2051 RVA: 0x0000FDCE File Offset: 0x0000DFCE
		// (set) Token: 0x06000804 RID: 2052 RVA: 0x0000FDD6 File Offset: 0x0000DFD6
		public float BottomUV { get; private set; }

		// Token: 0x06000805 RID: 2053 RVA: 0x0000FDE0 File Offset: 0x0000DFE0
		public override string ToString()
		{
			if (this.AtlasPath != null)
			{
				return this.AtlasPath;
			}
			if (this.Texture.Path != null)
			{
				return this.Texture.Path;
			}
			return string.Concat(new object[]
			{
				"Texture [",
				this.Texture.Width,
				", ",
				this.Texture.Height,
				"]"
			});
		}

		// Token: 0x06000806 RID: 2054 RVA: 0x0000FE5E File Offset: 0x0000E05E
		public Rectangle GetRelativeRect(Rectangle rect)
		{
			return this.GetRelativeRect(rect.X, rect.Y, rect.Width, rect.Height);
		}

		// Token: 0x06000807 RID: 2055 RVA: 0x0000FE80 File Offset: 0x0000E080
		public Rectangle GetRelativeRect(int x, int y, int width, int height)
		{
			int num = (int)((float)this.ClipRect.X - this.DrawOffset.X + (float)x);
			int num2 = (int)((float)this.ClipRect.Y - this.DrawOffset.Y + (float)y);
			int num3 = (int)MathHelper.Clamp((float)num, (float)this.ClipRect.Left, (float)this.ClipRect.Right);
			int num4 = (int)MathHelper.Clamp((float)num2, (float)this.ClipRect.Top, (float)this.ClipRect.Bottom);
			int width2 = Math.Max(0, Math.Min(num + width, this.ClipRect.Right) - num3);
			int height2 = Math.Max(0, Math.Min(num2 + height, this.ClipRect.Bottom) - num4);
			return new Rectangle(num3, num4, width2, height2);
		}

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x06000808 RID: 2056 RVA: 0x0000FF67 File Offset: 0x0000E167
		public int TotalPixels
		{
			get
			{
				return this.Width * this.Height;
			}
		}

		// Token: 0x06000809 RID: 2057 RVA: 0x0000FF78 File Offset: 0x0000E178
		public void Draw(Vector2 position)
		{
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), Color.White, 0f, -this.DrawOffset, 1f, SpriteEffects.None, 0f);
		}

		// Token: 0x0600080A RID: 2058 RVA: 0x0000FFC8 File Offset: 0x0000E1C8
		public void Draw(Vector2 position, Vector2 origin)
		{
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), Color.White, 0f, origin - this.DrawOffset, 1f, SpriteEffects.None, 0f);
		}

		// Token: 0x0600080B RID: 2059 RVA: 0x00010018 File Offset: 0x0000E218
		public void Draw(Vector2 position, Vector2 origin, Color color)
		{
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0f, origin - this.DrawOffset, 1f, SpriteEffects.None, 0f);
		}

		// Token: 0x0600080C RID: 2060 RVA: 0x00010064 File Offset: 0x0000E264
		public void Draw(Vector2 position, Vector2 origin, Color color, float scale)
		{
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0f, origin - this.DrawOffset, scale, SpriteEffects.None, 0f);
		}

		// Token: 0x0600080D RID: 2061 RVA: 0x000100AC File Offset: 0x0000E2AC
		public void Draw(Vector2 position, Vector2 origin, Color color, float scale, float rotation)
		{
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, origin - this.DrawOffset, scale, SpriteEffects.None, 0f);
		}

		// Token: 0x0600080E RID: 2062 RVA: 0x000100F4 File Offset: 0x0000E2F4
		public void Draw(Vector2 position, Vector2 origin, Color color, float scale, float rotation, SpriteEffects flip)
		{
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, origin - this.DrawOffset, scale, flip, 0f);
		}

		// Token: 0x0600080F RID: 2063 RVA: 0x0001013C File Offset: 0x0000E33C
		public void Draw(Vector2 position, Vector2 origin, Color color, Vector2 scale)
		{
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0f, origin - this.DrawOffset, scale, SpriteEffects.None, 0f);
		}

		// Token: 0x06000810 RID: 2064 RVA: 0x00010184 File Offset: 0x0000E384
		public void Draw(Vector2 position, Vector2 origin, Color color, Vector2 scale, float rotation)
		{
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, origin - this.DrawOffset, scale, SpriteEffects.None, 0f);
		}

		// Token: 0x06000811 RID: 2065 RVA: 0x000101CC File Offset: 0x0000E3CC
		public void Draw(Vector2 position, Vector2 origin, Color color, Vector2 scale, float rotation, SpriteEffects flip)
		{
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, origin - this.DrawOffset, scale, flip, 0f);
		}

		// Token: 0x06000812 RID: 2066 RVA: 0x00010214 File Offset: 0x0000E414
		public void Draw(Vector2 position, Vector2 origin, Color color, Vector2 scale, float rotation, Rectangle clip)
		{
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.GetRelativeRect(clip)), color, rotation, origin - this.DrawOffset, scale, SpriteEffects.None, 0f);
		}

		// Token: 0x06000813 RID: 2067 RVA: 0x0001025C File Offset: 0x0000E45C
		public void DrawCentered(Vector2 position)
		{
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), Color.White, 0f, this.Center - this.DrawOffset, 1f, SpriteEffects.None, 0f);
		}

		// Token: 0x06000814 RID: 2068 RVA: 0x000102B0 File Offset: 0x0000E4B0
		public void DrawCentered(Vector2 position, Color color)
		{
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0f, this.Center - this.DrawOffset, 1f, SpriteEffects.None, 0f);
		}

		// Token: 0x06000815 RID: 2069 RVA: 0x00010300 File Offset: 0x0000E500
		public void DrawCentered(Vector2 position, Color color, float scale)
		{
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0f, this.Center - this.DrawOffset, scale, SpriteEffects.None, 0f);
		}

		// Token: 0x06000816 RID: 2070 RVA: 0x0001034C File Offset: 0x0000E54C
		public void DrawCentered(Vector2 position, Color color, float scale, float rotation)
		{
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, this.Center - this.DrawOffset, scale, SpriteEffects.None, 0f);
		}

		// Token: 0x06000817 RID: 2071 RVA: 0x00010398 File Offset: 0x0000E598
		public void DrawCentered(Vector2 position, Color color, float scale, float rotation, SpriteEffects flip)
		{
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, this.Center - this.DrawOffset, scale, flip, 0f);
		}

		// Token: 0x06000818 RID: 2072 RVA: 0x000103E4 File Offset: 0x0000E5E4
		public void DrawCentered(Vector2 position, Color color, Vector2 scale)
		{
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0f, this.Center - this.DrawOffset, scale, SpriteEffects.None, 0f);
		}

		// Token: 0x06000819 RID: 2073 RVA: 0x00010430 File Offset: 0x0000E630
		public void DrawCentered(Vector2 position, Color color, Vector2 scale, float rotation)
		{
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, this.Center - this.DrawOffset, scale, SpriteEffects.None, 0f);
		}

		// Token: 0x0600081A RID: 2074 RVA: 0x0001047C File Offset: 0x0000E67C
		public void DrawCentered(Vector2 position, Color color, Vector2 scale, float rotation, SpriteEffects flip)
		{
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, this.Center - this.DrawOffset, scale, flip, 0f);
		}

		// Token: 0x0600081B RID: 2075 RVA: 0x000104C8 File Offset: 0x0000E6C8
		public void DrawJustified(Vector2 position, Vector2 justify)
		{
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), Color.White, 0f, new Vector2((float)this.Width * justify.X, (float)this.Height * justify.Y) - this.DrawOffset, 1f, SpriteEffects.None, 0f);
		}

		// Token: 0x0600081C RID: 2076 RVA: 0x00010538 File Offset: 0x0000E738
		public void DrawJustified(Vector2 position, Vector2 justify, Color color)
		{
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0f, new Vector2((float)this.Width * justify.X, (float)this.Height * justify.Y) - this.DrawOffset, 1f, SpriteEffects.None, 0f);
		}

		// Token: 0x0600081D RID: 2077 RVA: 0x000105A4 File Offset: 0x0000E7A4
		public void DrawJustified(Vector2 position, Vector2 justify, Color color, float scale)
		{
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0f, new Vector2((float)this.Width * justify.X, (float)this.Height * justify.Y) - this.DrawOffset, scale, SpriteEffects.None, 0f);
		}

		// Token: 0x0600081E RID: 2078 RVA: 0x0001060C File Offset: 0x0000E80C
		public void DrawJustified(Vector2 position, Vector2 justify, Color color, float scale, float rotation)
		{
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, new Vector2((float)this.Width * justify.X, (float)this.Height * justify.Y) - this.DrawOffset, scale, SpriteEffects.None, 0f);
		}

		// Token: 0x0600081F RID: 2079 RVA: 0x00010674 File Offset: 0x0000E874
		public void DrawJustified(Vector2 position, Vector2 justify, Color color, float scale, float rotation, SpriteEffects flip)
		{
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, new Vector2((float)this.Width * justify.X, (float)this.Height * justify.Y) - this.DrawOffset, scale, flip, 0f);
		}

		// Token: 0x06000820 RID: 2080 RVA: 0x000106DC File Offset: 0x0000E8DC
		public void DrawJustified(Vector2 position, Vector2 justify, Color color, Vector2 scale)
		{
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0f, new Vector2((float)this.Width * justify.X, (float)this.Height * justify.Y) - this.DrawOffset, scale, SpriteEffects.None, 0f);
		}

		// Token: 0x06000821 RID: 2081 RVA: 0x00010744 File Offset: 0x0000E944
		public void DrawJustified(Vector2 position, Vector2 justify, Color color, Vector2 scale, float rotation)
		{
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, new Vector2((float)this.Width * justify.X, (float)this.Height * justify.Y) - this.DrawOffset, scale, SpriteEffects.None, 0f);
		}

		// Token: 0x06000822 RID: 2082 RVA: 0x000107AC File Offset: 0x0000E9AC
		public void DrawJustified(Vector2 position, Vector2 justify, Color color, Vector2 scale, float rotation, SpriteEffects flip)
		{
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, new Vector2((float)this.Width * justify.X, (float)this.Height * justify.Y) - this.DrawOffset, scale, flip, 0f);
		}

		// Token: 0x06000823 RID: 2083 RVA: 0x00010814 File Offset: 0x0000EA14
		public void DrawOutline(Vector2 position)
		{
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if (i != 0 || j != 0)
					{
						Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float)i, (float)j), new Rectangle?(this.ClipRect), Color.Black, 0f, -this.DrawOffset, 1f, SpriteEffects.None, 0f);
					}
				}
			}
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), Color.White, 0f, -this.DrawOffset, 1f, SpriteEffects.None, 0f);
		}

		// Token: 0x06000824 RID: 2084 RVA: 0x000108D0 File Offset: 0x0000EAD0
		public void DrawOutline(Vector2 position, Vector2 origin)
		{
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if (i != 0 || j != 0)
					{
						Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float)i, (float)j), new Rectangle?(this.ClipRect), Color.Black, 0f, origin - this.DrawOffset, 1f, SpriteEffects.None, 0f);
					}
				}
			}
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), Color.White, 0f, origin - this.DrawOffset, 1f, SpriteEffects.None, 0f);
		}

		// Token: 0x06000825 RID: 2085 RVA: 0x00010990 File Offset: 0x0000EB90
		public void DrawOutline(Vector2 position, Vector2 origin, Color color)
		{
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if (i != 0 || j != 0)
					{
						Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float)i, (float)j), new Rectangle?(this.ClipRect), Color.Black, 0f, origin - this.DrawOffset, 1f, SpriteEffects.None, 0f);
					}
				}
			}
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0f, origin - this.DrawOffset, 1f, SpriteEffects.None, 0f);
		}

		// Token: 0x06000826 RID: 2086 RVA: 0x00010A4C File Offset: 0x0000EC4C
		public void DrawOutline(Vector2 position, Vector2 origin, Color color, float scale)
		{
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if (i != 0 || j != 0)
					{
						Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float)i, (float)j), new Rectangle?(this.ClipRect), Color.Black, 0f, origin - this.DrawOffset, scale, SpriteEffects.None, 0f);
					}
				}
			}
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0f, origin - this.DrawOffset, scale, SpriteEffects.None, 0f);
		}

		// Token: 0x06000827 RID: 2087 RVA: 0x00010B00 File Offset: 0x0000ED00
		public void DrawOutline(Vector2 position, Vector2 origin, Color color, float scale, float rotation)
		{
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if (i != 0 || j != 0)
					{
						Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float)i, (float)j), new Rectangle?(this.ClipRect), Color.Black, rotation, origin - this.DrawOffset, scale, SpriteEffects.None, 0f);
					}
				}
			}
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, origin - this.DrawOffset, scale, SpriteEffects.None, 0f);
		}

		// Token: 0x06000828 RID: 2088 RVA: 0x00010BB0 File Offset: 0x0000EDB0
		public void DrawOutline(Vector2 position, Vector2 origin, Color color, float scale, float rotation, SpriteEffects flip)
		{
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if (i != 0 || j != 0)
					{
						Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float)i, (float)j), new Rectangle?(this.ClipRect), Color.Black, rotation, origin - this.DrawOffset, scale, flip, 0f);
					}
				}
			}
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, origin - this.DrawOffset, scale, flip, 0f);
		}

		// Token: 0x06000829 RID: 2089 RVA: 0x00010C60 File Offset: 0x0000EE60
		public void DrawOutline(Vector2 position, Vector2 origin, Color color, Vector2 scale)
		{
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if (i != 0 || j != 0)
					{
						Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float)i, (float)j), new Rectangle?(this.ClipRect), Color.Black, 0f, origin - this.DrawOffset, scale, SpriteEffects.None, 0f);
					}
				}
			}
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0f, origin - this.DrawOffset, scale, SpriteEffects.None, 0f);
		}

		// Token: 0x0600082A RID: 2090 RVA: 0x00010D14 File Offset: 0x0000EF14
		public void DrawOutline(Vector2 position, Vector2 origin, Color color, Vector2 scale, float rotation)
		{
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if (i != 0 || j != 0)
					{
						Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float)i, (float)j), new Rectangle?(this.ClipRect), Color.Black, rotation, origin - this.DrawOffset, scale, SpriteEffects.None, 0f);
					}
				}
			}
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, origin - this.DrawOffset, scale, SpriteEffects.None, 0f);
		}

		// Token: 0x0600082B RID: 2091 RVA: 0x00010DC4 File Offset: 0x0000EFC4
		public void DrawOutline(Vector2 position, Vector2 origin, Color color, Vector2 scale, float rotation, SpriteEffects flip)
		{
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if (i != 0 || j != 0)
					{
						Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float)i, (float)j), new Rectangle?(this.ClipRect), Color.Black, rotation, origin - this.DrawOffset, scale, flip, 0f);
					}
				}
			}
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, origin - this.DrawOffset, scale, flip, 0f);
		}

		// Token: 0x0600082C RID: 2092 RVA: 0x00010E74 File Offset: 0x0000F074
		public void DrawOutlineCentered(Vector2 position)
		{
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if (i != 0 || j != 0)
					{
						Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float)i, (float)j), new Rectangle?(this.ClipRect), Color.Black, 0f, this.Center - this.DrawOffset, 1f, SpriteEffects.None, 0f);
					}
				}
			}
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), Color.White, 0f, this.Center - this.DrawOffset, 1f, SpriteEffects.None, 0f);
		}

		// Token: 0x0600082D RID: 2093 RVA: 0x00010F3C File Offset: 0x0000F13C
		public void DrawOutlineCentered(Vector2 position, Color color)
		{
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if (i != 0 || j != 0)
					{
						Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float)i, (float)j), new Rectangle?(this.ClipRect), Color.Black, 0f, this.Center - this.DrawOffset, 1f, SpriteEffects.None, 0f);
					}
				}
			}
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0f, this.Center - this.DrawOffset, 1f, SpriteEffects.None, 0f);
		}

		// Token: 0x0600082E RID: 2094 RVA: 0x00011000 File Offset: 0x0000F200
		public void DrawOutlineCentered(Vector2 position, Color color, float scale)
		{
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if (i != 0 || j != 0)
					{
						Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float)i, (float)j), new Rectangle?(this.ClipRect), Color.Black, 0f, this.Center - this.DrawOffset, scale, SpriteEffects.None, 0f);
					}
				}
			}
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0f, this.Center - this.DrawOffset, scale, SpriteEffects.None, 0f);
		}

		// Token: 0x0600082F RID: 2095 RVA: 0x000110BC File Offset: 0x0000F2BC
		public void DrawOutlineCentered(Vector2 position, Color color, float scale, float rotation)
		{
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if (i != 0 || j != 0)
					{
						Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float)i, (float)j), new Rectangle?(this.ClipRect), Color.Black, rotation, this.Center - this.DrawOffset, scale, SpriteEffects.None, 0f);
					}
				}
			}
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, this.Center - this.DrawOffset, scale, SpriteEffects.None, 0f);
		}

		// Token: 0x06000830 RID: 2096 RVA: 0x00011174 File Offset: 0x0000F374
		public void DrawOutlineCentered(Vector2 position, Color color, float scale, float rotation, SpriteEffects flip)
		{
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if (i != 0 || j != 0)
					{
						Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float)i, (float)j), new Rectangle?(this.ClipRect), Color.Black, rotation, this.Center - this.DrawOffset, scale, flip, 0f);
					}
				}
			}
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, this.Center - this.DrawOffset, scale, flip, 0f);
		}

		// Token: 0x06000831 RID: 2097 RVA: 0x0001122C File Offset: 0x0000F42C
		public void DrawOutlineCentered(Vector2 position, Color color, Vector2 scale)
		{
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if (i != 0 || j != 0)
					{
						Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float)i, (float)j), new Rectangle?(this.ClipRect), Color.Black, 0f, this.Center - this.DrawOffset, scale, SpriteEffects.None, 0f);
					}
				}
			}
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0f, this.Center - this.DrawOffset, scale, SpriteEffects.None, 0f);
		}

		// Token: 0x06000832 RID: 2098 RVA: 0x000112E8 File Offset: 0x0000F4E8
		public void DrawOutlineCentered(Vector2 position, Color color, Vector2 scale, float rotation)
		{
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if (i != 0 || j != 0)
					{
						Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float)i, (float)j), new Rectangle?(this.ClipRect), Color.Black, rotation, this.Center - this.DrawOffset, scale, SpriteEffects.None, 0f);
					}
				}
			}
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, this.Center - this.DrawOffset, scale, SpriteEffects.None, 0f);
		}

		// Token: 0x06000833 RID: 2099 RVA: 0x000113A0 File Offset: 0x0000F5A0
		public void DrawOutlineCentered(Vector2 position, Color color, Vector2 scale, float rotation, SpriteEffects flip)
		{
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if (i != 0 || j != 0)
					{
						Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float)i, (float)j), new Rectangle?(this.ClipRect), Color.Black, rotation, this.Center - this.DrawOffset, scale, flip, 0f);
					}
				}
			}
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, this.Center - this.DrawOffset, scale, flip, 0f);
		}

		// Token: 0x06000834 RID: 2100 RVA: 0x00011458 File Offset: 0x0000F658
		public void DrawOutlineJustified(Vector2 position, Vector2 justify)
		{
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if (i != 0 || j != 0)
					{
						Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float)i, (float)j), new Rectangle?(this.ClipRect), Color.Black, 0f, new Vector2((float)this.Width * justify.X, (float)this.Height * justify.Y) - this.DrawOffset, 1f, SpriteEffects.None, 0f);
					}
				}
			}
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), Color.White, 0f, new Vector2((float)this.Width * justify.X, (float)this.Height * justify.Y) - this.DrawOffset, 1f, SpriteEffects.None, 0f);
		}

		// Token: 0x06000835 RID: 2101 RVA: 0x0001155C File Offset: 0x0000F75C
		public void DrawOutlineJustified(Vector2 position, Vector2 justify, Color color)
		{
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if (i != 0 || j != 0)
					{
						Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float)i, (float)j), new Rectangle?(this.ClipRect), Color.Black, 0f, new Vector2((float)this.Width * justify.X, (float)this.Height * justify.Y) - this.DrawOffset, 1f, SpriteEffects.None, 0f);
					}
				}
			}
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0f, new Vector2((float)this.Width * justify.X, (float)this.Height * justify.Y) - this.DrawOffset, 1f, SpriteEffects.None, 0f);
		}

		// Token: 0x06000836 RID: 2102 RVA: 0x0001165C File Offset: 0x0000F85C
		public void DrawOutlineJustified(Vector2 position, Vector2 justify, Color color, float scale)
		{
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if (i != 0 || j != 0)
					{
						Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float)i, (float)j), new Rectangle?(this.ClipRect), Color.Black, 0f, new Vector2((float)this.Width * justify.X, (float)this.Height * justify.Y) - this.DrawOffset, scale, SpriteEffects.None, 0f);
					}
				}
			}
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0f, new Vector2((float)this.Width * justify.X, (float)this.Height * justify.Y) - this.DrawOffset, scale, SpriteEffects.None, 0f);
		}

		// Token: 0x06000837 RID: 2103 RVA: 0x00011758 File Offset: 0x0000F958
		public void DrawOutlineJustified(Vector2 position, Vector2 justify, Color color, float scale, float rotation)
		{
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if (i != 0 || j != 0)
					{
						Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float)i, (float)j), new Rectangle?(this.ClipRect), Color.Black, rotation, new Vector2((float)this.Width * justify.X, (float)this.Height * justify.Y) - this.DrawOffset, scale, SpriteEffects.None, 0f);
					}
				}
			}
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, new Vector2((float)this.Width * justify.X, (float)this.Height * justify.Y) - this.DrawOffset, scale, SpriteEffects.None, 0f);
		}

		// Token: 0x06000838 RID: 2104 RVA: 0x0001184C File Offset: 0x0000FA4C
		public void DrawOutlineJustified(Vector2 position, Vector2 justify, Color color, float scale, float rotation, SpriteEffects flip)
		{
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if (i != 0 || j != 0)
					{
						Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float)i, (float)j), new Rectangle?(this.ClipRect), Color.Black, rotation, new Vector2((float)this.Width * justify.X, (float)this.Height * justify.Y) - this.DrawOffset, scale, flip, 0f);
					}
				}
			}
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, new Vector2((float)this.Width * justify.X, (float)this.Height * justify.Y) - this.DrawOffset, scale, flip, 0f);
		}

		// Token: 0x06000839 RID: 2105 RVA: 0x00011944 File Offset: 0x0000FB44
		public void DrawOutlineJustified(Vector2 position, Vector2 justify, Color color, Vector2 scale)
		{
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if (i != 0 || j != 0)
					{
						Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float)i, (float)j), new Rectangle?(this.ClipRect), Color.Black, 0f, new Vector2((float)this.Width * justify.X, (float)this.Height * justify.Y) - this.DrawOffset, scale, SpriteEffects.None, 0f);
					}
				}
			}
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, 0f, new Vector2((float)this.Width * justify.X, (float)this.Height * justify.Y) - this.DrawOffset, scale, SpriteEffects.None, 0f);
		}

		// Token: 0x0600083A RID: 2106 RVA: 0x00011A40 File Offset: 0x0000FC40
		public void DrawOutlineJustified(Vector2 position, Vector2 justify, Color color, Vector2 scale, float rotation)
		{
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if (i != 0 || j != 0)
					{
						Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float)i, (float)j), new Rectangle?(this.ClipRect), Color.Black, rotation, new Vector2((float)this.Width * justify.X, (float)this.Height * justify.Y) - this.DrawOffset, scale, SpriteEffects.None, 0f);
					}
				}
			}
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, new Vector2((float)this.Width * justify.X, (float)this.Height * justify.Y) - this.DrawOffset, scale, SpriteEffects.None, 0f);
		}

		// Token: 0x0600083B RID: 2107 RVA: 0x00011B34 File Offset: 0x0000FD34
		public void DrawOutlineJustified(Vector2 position, Vector2 justify, Color color, Vector2 scale, float rotation, SpriteEffects flip)
		{
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if (i != 0 || j != 0)
					{
						Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position + new Vector2((float)i, (float)j), new Rectangle?(this.ClipRect), Color.Black, rotation, new Vector2((float)this.Width * justify.X, (float)this.Height * justify.Y) - this.DrawOffset, scale, flip, 0f);
					}
				}
			}
			Monocle.Draw.SpriteBatch.Draw(this.Texture.Texture, position, new Rectangle?(this.ClipRect), color, rotation, new Vector2((float)this.Width * justify.X, (float)this.Height * justify.Y) - this.DrawOffset, scale, flip, 0f);
		}

		// Token: 0x04000573 RID: 1395
		public string AtlasPath;
	}
}
