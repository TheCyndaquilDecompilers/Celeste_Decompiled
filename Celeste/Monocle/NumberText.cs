using System;
using Microsoft.Xna.Framework.Graphics;

namespace Monocle
{
	// Token: 0x020000F6 RID: 246
	public class NumberText : GraphicsComponent
	{
		// Token: 0x06000685 RID: 1669 RVA: 0x0000A91D File Offset: 0x00008B1D
		public NumberText(SpriteFont font, string prefix, int value, bool centered = false) : base(false)
		{
			this.font = font;
			this.prefix = prefix;
			this.value = value;
			this.centered = centered;
			this.UpdateString();
		}

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06000686 RID: 1670 RVA: 0x0000A949 File Offset: 0x00008B49
		// (set) Token: 0x06000687 RID: 1671 RVA: 0x0000A954 File Offset: 0x00008B54
		public int Value
		{
			get
			{
				return this.value;
			}
			set
			{
				if (this.value != value)
				{
					int obj = this.value;
					this.value = value;
					this.UpdateString();
					if (this.OnValueUpdate != null)
					{
						this.OnValueUpdate(obj);
					}
				}
			}
		}

		// Token: 0x06000688 RID: 1672 RVA: 0x0000A994 File Offset: 0x00008B94
		public void UpdateString()
		{
			this.drawString = this.prefix + this.value.ToString();
			if (this.centered)
			{
				this.Origin = (this.font.MeasureString(this.drawString) / 2f).Floor();
			}
		}

		// Token: 0x06000689 RID: 1673 RVA: 0x0000A9EC File Offset: 0x00008BEC
		public override void Render()
		{
			Draw.SpriteBatch.DrawString(this.font, this.drawString, base.RenderPosition, this.Color, this.Rotation, this.Origin, this.Scale, this.Effects, 0f);
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x0600068A RID: 1674 RVA: 0x0000AA38 File Offset: 0x00008C38
		public float Width
		{
			get
			{
				return this.font.MeasureString(this.drawString).X;
			}
		}

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x0600068B RID: 1675 RVA: 0x0000AA50 File Offset: 0x00008C50
		public float Height
		{
			get
			{
				return this.font.MeasureString(this.drawString).Y;
			}
		}

		// Token: 0x040004E0 RID: 1248
		private SpriteFont font;

		// Token: 0x040004E1 RID: 1249
		private int value;

		// Token: 0x040004E2 RID: 1250
		private string prefix;

		// Token: 0x040004E3 RID: 1251
		private string drawString;

		// Token: 0x040004E4 RID: 1252
		private bool centered;

		// Token: 0x040004E5 RID: 1253
		public Action<int> OnValueUpdate;
	}
}
