using System;
using Microsoft.Xna.Framework;

namespace Monocle
{
	// Token: 0x020000F2 RID: 242
	public class Image : GraphicsComponent
	{
		// Token: 0x0600062C RID: 1580 RVA: 0x00009468 File Offset: 0x00007668
		public Image(MTexture texture) : base(false)
		{
			this.Texture = texture;
		}

		// Token: 0x0600062D RID: 1581 RVA: 0x00009478 File Offset: 0x00007678
		internal Image(MTexture texture, bool active) : base(active)
		{
			this.Texture = texture;
		}

		// Token: 0x0600062E RID: 1582 RVA: 0x00009488 File Offset: 0x00007688
		public override void Render()
		{
			if (this.Texture != null)
			{
				this.Texture.Draw(base.RenderPosition, this.Origin, this.Color, this.Scale, this.Rotation, this.Effects);
			}
		}

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x0600062F RID: 1583 RVA: 0x000094C1 File Offset: 0x000076C1
		public virtual float Width
		{
			get
			{
				return (float)this.Texture.Width;
			}
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x06000630 RID: 1584 RVA: 0x000094CF File Offset: 0x000076CF
		public virtual float Height
		{
			get
			{
				return (float)this.Texture.Height;
			}
		}

		// Token: 0x06000631 RID: 1585 RVA: 0x000094DD File Offset: 0x000076DD
		public Image SetOrigin(float x, float y)
		{
			this.Origin.X = x;
			this.Origin.Y = y;
			return this;
		}

		// Token: 0x06000632 RID: 1586 RVA: 0x000094F8 File Offset: 0x000076F8
		public Image CenterOrigin()
		{
			this.Origin.X = this.Width / 2f;
			this.Origin.Y = this.Height / 2f;
			return this;
		}

		// Token: 0x06000633 RID: 1587 RVA: 0x00009529 File Offset: 0x00007729
		public Image JustifyOrigin(Vector2 at)
		{
			this.Origin.X = this.Width * at.X;
			this.Origin.Y = this.Height * at.Y;
			return this;
		}

		// Token: 0x06000634 RID: 1588 RVA: 0x0000955C File Offset: 0x0000775C
		public Image JustifyOrigin(float x, float y)
		{
			this.Origin.X = this.Width * x;
			this.Origin.Y = this.Height * y;
			return this;
		}

		// Token: 0x06000635 RID: 1589 RVA: 0x00009585 File Offset: 0x00007785
		public Image SetColor(Color color)
		{
			this.Color = color;
			return this;
		}

		// Token: 0x040004B3 RID: 1203
		public MTexture Texture;

		// Token: 0x040004B4 RID: 1204
		public bool TEST;
	}
}
