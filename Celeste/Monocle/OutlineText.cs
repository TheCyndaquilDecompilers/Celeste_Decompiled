using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monocle
{
	// Token: 0x020000F7 RID: 247
	public class OutlineText : Text
	{
		// Token: 0x0600068C RID: 1676 RVA: 0x0000AA68 File Offset: 0x00008C68
		public OutlineText(SpriteFont font, string text, Vector2 position, Color color, Text.HorizontalAlign horizontalAlign = Text.HorizontalAlign.Center, Text.VerticalAlign verticalAlign = Text.VerticalAlign.Center) : base(font, text, position, color, horizontalAlign, verticalAlign)
		{
		}

		// Token: 0x0600068D RID: 1677 RVA: 0x0000AA8B File Offset: 0x00008C8B
		public OutlineText(SpriteFont font, string text, Vector2 position, Text.HorizontalAlign horizontalAlign = Text.HorizontalAlign.Center, Text.VerticalAlign verticalAlign = Text.VerticalAlign.Center) : this(font, text, position, Color.White, horizontalAlign, verticalAlign)
		{
		}

		// Token: 0x0600068E RID: 1678 RVA: 0x0000AA9F File Offset: 0x00008C9F
		public OutlineText(SpriteFont font, string text) : this(font, text, Vector2.Zero, Color.White, Text.HorizontalAlign.Center, Text.VerticalAlign.Center)
		{
		}

		// Token: 0x0600068F RID: 1679 RVA: 0x0000AAB8 File Offset: 0x00008CB8
		public override void Render()
		{
			for (int i = -1; i < 2; i++)
			{
				for (int j = -1; j < 2; j++)
				{
					if (i != 0 || j != 0)
					{
						Draw.SpriteBatch.DrawString(base.Font, base.DrawText, base.RenderPosition + new Vector2((float)(i * this.OutlineOffset), (float)(j * this.OutlineOffset)), this.OutlineColor, this.Rotation, this.Origin, this.Scale, this.Effects, 0f);
					}
				}
			}
			base.Render();
		}

		// Token: 0x040004E6 RID: 1254
		public Color OutlineColor = Color.Black;

		// Token: 0x040004E7 RID: 1255
		public int OutlineOffset = 1;
	}
}
