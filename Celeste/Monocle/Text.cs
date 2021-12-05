using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monocle
{
	// Token: 0x020000F8 RID: 248
	public class Text : GraphicsComponent
	{
		// Token: 0x06000690 RID: 1680 RVA: 0x0000AB44 File Offset: 0x00008D44
		public Text(SpriteFont font, string text, Vector2 position, Color color, Text.HorizontalAlign horizontalAlign = Text.HorizontalAlign.Center, Text.VerticalAlign verticalAlign = Text.VerticalAlign.Center) : base(false)
		{
			this.font = font;
			this.text = text;
			this.Position = position;
			this.Color = color;
			this.horizontalOrigin = horizontalAlign;
			this.verticalOrigin = verticalAlign;
			this.UpdateSize();
		}

		// Token: 0x06000691 RID: 1681 RVA: 0x0000AB80 File Offset: 0x00008D80
		public Text(SpriteFont font, string text, Vector2 position, Text.HorizontalAlign horizontalAlign = Text.HorizontalAlign.Center, Text.VerticalAlign verticalAlign = Text.VerticalAlign.Center) : this(font, text, position, Color.White, horizontalAlign, verticalAlign)
		{
		}

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x06000692 RID: 1682 RVA: 0x0000AB94 File Offset: 0x00008D94
		// (set) Token: 0x06000693 RID: 1683 RVA: 0x0000AB9C File Offset: 0x00008D9C
		public SpriteFont Font
		{
			get
			{
				return this.font;
			}
			set
			{
				this.font = value;
				this.UpdateSize();
			}
		}

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x06000694 RID: 1684 RVA: 0x0000ABAB File Offset: 0x00008DAB
		// (set) Token: 0x06000695 RID: 1685 RVA: 0x0000ABB3 File Offset: 0x00008DB3
		public string DrawText
		{
			get
			{
				return this.text;
			}
			set
			{
				this.text = value;
				this.UpdateSize();
			}
		}

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x06000696 RID: 1686 RVA: 0x0000ABC2 File Offset: 0x00008DC2
		// (set) Token: 0x06000697 RID: 1687 RVA: 0x0000ABCA File Offset: 0x00008DCA
		public Text.HorizontalAlign HorizontalOrigin
		{
			get
			{
				return this.horizontalOrigin;
			}
			set
			{
				this.horizontalOrigin = value;
				this.UpdateCentering();
			}
		}

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x06000698 RID: 1688 RVA: 0x0000ABD9 File Offset: 0x00008DD9
		// (set) Token: 0x06000699 RID: 1689 RVA: 0x0000ABE1 File Offset: 0x00008DE1
		public Text.VerticalAlign VerticalOrigin
		{
			get
			{
				return this.verticalOrigin;
			}
			set
			{
				this.verticalOrigin = value;
				this.UpdateCentering();
			}
		}

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x0600069A RID: 1690 RVA: 0x0000ABF0 File Offset: 0x00008DF0
		public float Width
		{
			get
			{
				return this.size.X;
			}
		}

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x0600069B RID: 1691 RVA: 0x0000ABFD File Offset: 0x00008DFD
		public float Height
		{
			get
			{
				return this.size.Y;
			}
		}

		// Token: 0x0600069C RID: 1692 RVA: 0x0000AC0A File Offset: 0x00008E0A
		private void UpdateSize()
		{
			this.size = this.font.MeasureString(this.text);
			this.UpdateCentering();
		}

		// Token: 0x0600069D RID: 1693 RVA: 0x0000AC2C File Offset: 0x00008E2C
		private void UpdateCentering()
		{
			if (this.horizontalOrigin == Text.HorizontalAlign.Left)
			{
				this.Origin.X = 0f;
			}
			else if (this.horizontalOrigin == Text.HorizontalAlign.Center)
			{
				this.Origin.X = this.size.X / 2f;
			}
			else
			{
				this.Origin.X = this.size.X;
			}
			if (this.verticalOrigin == Text.VerticalAlign.Top)
			{
				this.Origin.Y = 0f;
			}
			else if (this.verticalOrigin == Text.VerticalAlign.Center)
			{
				this.Origin.Y = this.size.Y / 2f;
			}
			else
			{
				this.Origin.Y = this.size.Y;
			}
			this.Origin = this.Origin.Floor();
		}

		// Token: 0x0600069E RID: 1694 RVA: 0x0000ACF8 File Offset: 0x00008EF8
		public override void Render()
		{
			Draw.SpriteBatch.DrawString(this.font, this.text, base.RenderPosition, this.Color, this.Rotation, this.Origin, this.Scale, this.Effects, 0f);
		}

		// Token: 0x040004E8 RID: 1256
		private SpriteFont font;

		// Token: 0x040004E9 RID: 1257
		private string text;

		// Token: 0x040004EA RID: 1258
		private Text.HorizontalAlign horizontalOrigin;

		// Token: 0x040004EB RID: 1259
		private Text.VerticalAlign verticalOrigin;

		// Token: 0x040004EC RID: 1260
		private Vector2 size;

		// Token: 0x0200039C RID: 924
		public enum HorizontalAlign
		{
			// Token: 0x04001EF2 RID: 7922
			Left,
			// Token: 0x04001EF3 RID: 7923
			Center,
			// Token: 0x04001EF4 RID: 7924
			Right
		}

		// Token: 0x0200039D RID: 925
		public enum VerticalAlign
		{
			// Token: 0x04001EF6 RID: 7926
			Top,
			// Token: 0x04001EF7 RID: 7927
			Center,
			// Token: 0x04001EF8 RID: 7928
			Bottom
		}
	}
}
