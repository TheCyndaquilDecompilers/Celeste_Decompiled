using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monocle
{
	// Token: 0x020000F9 RID: 249
	public class TimerText : GraphicsComponent
	{
		// Token: 0x17000067 RID: 103
		// (get) Token: 0x0600069F RID: 1695 RVA: 0x0000AD44 File Offset: 0x00008F44
		// (set) Token: 0x060006A0 RID: 1696 RVA: 0x0000AD4C File Offset: 0x00008F4C
		public string Text { get; private set; }

		// Token: 0x060006A1 RID: 1697 RVA: 0x0000AD58 File Offset: 0x00008F58
		public TimerText(SpriteFont font, TimerText.TimerModes mode, TimerText.CountModes countMode, int frames, Vector2 justify, Action onComplete = null) : base(true)
		{
			this.font = font;
			this.timerMode = mode;
			this.CountMode = countMode;
			this.frames = frames;
			this.justify = justify;
			this.OnComplete = onComplete;
			this.UpdateText();
			this.CalculateOrigin();
		}

		// Token: 0x060006A2 RID: 1698 RVA: 0x0000ADA8 File Offset: 0x00008FA8
		private void UpdateText()
		{
			if (this.timerMode == TimerText.TimerModes.SecondsMilliseconds)
			{
				this.Text = ((float)(this.frames / 60) + (float)(this.frames % 60) * 0.016666668f).ToString("0.00");
			}
		}

		// Token: 0x060006A3 RID: 1699 RVA: 0x0000ADED File Offset: 0x00008FED
		private void CalculateOrigin()
		{
			this.Origin = (this.font.MeasureString(this.Text) * this.justify).Floor();
		}

		// Token: 0x060006A4 RID: 1700 RVA: 0x0000AE18 File Offset: 0x00009018
		public override void Update()
		{
			base.Update();
			if (this.CountMode == TimerText.CountModes.Down)
			{
				if (this.frames > 0)
				{
					this.frames--;
					if (this.frames == 0 && this.OnComplete != null)
					{
						this.OnComplete();
					}
					this.UpdateText();
					this.CalculateOrigin();
					return;
				}
			}
			else
			{
				this.frames++;
				this.UpdateText();
				this.CalculateOrigin();
			}
		}

		// Token: 0x060006A5 RID: 1701 RVA: 0x0000AE8C File Offset: 0x0000908C
		public override void Render()
		{
			Draw.SpriteBatch.DrawString(this.font, this.Text, base.RenderPosition, this.Color, this.Rotation, this.Origin, this.Scale, this.Effects, 0f);
		}

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x060006A6 RID: 1702 RVA: 0x0000AED8 File Offset: 0x000090D8
		// (set) Token: 0x060006A7 RID: 1703 RVA: 0x0000AEE0 File Offset: 0x000090E0
		public SpriteFont Font
		{
			get
			{
				return this.font;
			}
			set
			{
				this.font = value;
				this.CalculateOrigin();
			}
		}

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x060006A8 RID: 1704 RVA: 0x0000AEEF File Offset: 0x000090EF
		// (set) Token: 0x060006A9 RID: 1705 RVA: 0x0000AEF7 File Offset: 0x000090F7
		public int Frames
		{
			get
			{
				return this.frames;
			}
			set
			{
				if (this.frames != value)
				{
					this.frames = value;
					this.UpdateText();
					this.CalculateOrigin();
				}
			}
		}

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x060006AA RID: 1706 RVA: 0x0000AF15 File Offset: 0x00009115
		// (set) Token: 0x060006AB RID: 1707 RVA: 0x0000AF1D File Offset: 0x0000911D
		public Vector2 Justify
		{
			get
			{
				return this.justify;
			}
			set
			{
				this.justify = value;
				this.CalculateOrigin();
			}
		}

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x060006AC RID: 1708 RVA: 0x0000AF2C File Offset: 0x0000912C
		public float Width
		{
			get
			{
				return this.font.MeasureString(this.Text).X;
			}
		}

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x060006AD RID: 1709 RVA: 0x0000AF44 File Offset: 0x00009144
		public float Height
		{
			get
			{
				return this.font.MeasureString(this.Text).Y;
			}
		}

		// Token: 0x040004ED RID: 1261
		private const float DELTA_TIME = 0.016666668f;

		// Token: 0x040004EE RID: 1262
		private SpriteFont font;

		// Token: 0x040004EF RID: 1263
		private int frames;

		// Token: 0x040004F0 RID: 1264
		private TimerText.TimerModes timerMode;

		// Token: 0x040004F1 RID: 1265
		private Vector2 justify;

		// Token: 0x040004F3 RID: 1267
		public Action OnComplete;

		// Token: 0x040004F4 RID: 1268
		public TimerText.CountModes CountMode;

		// Token: 0x0200039E RID: 926
		public enum CountModes
		{
			// Token: 0x04001EFA RID: 7930
			Down,
			// Token: 0x04001EFB RID: 7931
			Up
		}

		// Token: 0x0200039F RID: 927
		public enum TimerModes
		{
			// Token: 0x04001EFD RID: 7933
			SecondsMilliseconds
		}
	}
}
