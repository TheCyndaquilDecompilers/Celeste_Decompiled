using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002F0 RID: 752
	public class StrawberriesCounter : Component
	{
		// Token: 0x06001749 RID: 5961 RVA: 0x0008D6FC File Offset: 0x0008B8FC
		public StrawberriesCounter(bool centeredX, int amount, int outOf = 0, bool showOutOf = false) : base(true, true)
		{
			this.CenteredX = centeredX;
			this.amount = amount;
			this.outOf = outOf;
			this.showOutOf = showOutOf;
			this.UpdateStrings();
			this.wiggler = Wiggler.Create(0.5f, 3f, null, false, false);
			this.wiggler.StartZero = true;
			this.wiggler.UseRawDeltaTime = true;
			this.x = GFX.Gui["x"];
		}

		// Token: 0x17000198 RID: 408
		// (get) Token: 0x0600174A RID: 5962 RVA: 0x0008D7B3 File Offset: 0x0008B9B3
		// (set) Token: 0x0600174B RID: 5963 RVA: 0x0008D7BC File Offset: 0x0008B9BC
		public int Amount
		{
			get
			{
				return this.amount;
			}
			set
			{
				if (this.amount != value)
				{
					this.amount = value;
					this.UpdateStrings();
					if (this.CanWiggle)
					{
						if (this.OverworldSfx)
						{
							Audio.Play(this.Golden ? "event:/ui/postgame/goldberry_count" : "event:/ui/postgame/strawberry_count");
						}
						else
						{
							Audio.Play("event:/ui/game/increment_strawberry");
						}
						this.wiggler.Start();
						this.flashTimer = 0.5f;
					}
				}
			}
		}

		// Token: 0x17000199 RID: 409
		// (get) Token: 0x0600174C RID: 5964 RVA: 0x0008D82C File Offset: 0x0008BA2C
		// (set) Token: 0x0600174D RID: 5965 RVA: 0x0008D834 File Offset: 0x0008BA34
		public int OutOf
		{
			get
			{
				return this.outOf;
			}
			set
			{
				this.outOf = value;
				this.UpdateStrings();
			}
		}

		// Token: 0x1700019A RID: 410
		// (get) Token: 0x0600174E RID: 5966 RVA: 0x0008D843 File Offset: 0x0008BA43
		// (set) Token: 0x0600174F RID: 5967 RVA: 0x0008D84B File Offset: 0x0008BA4B
		public bool ShowOutOf
		{
			get
			{
				return this.showOutOf;
			}
			set
			{
				if (this.showOutOf != value)
				{
					this.showOutOf = value;
					this.UpdateStrings();
				}
			}
		}

		// Token: 0x1700019B RID: 411
		// (get) Token: 0x06001750 RID: 5968 RVA: 0x0008D863 File Offset: 0x0008BA63
		public float FullHeight
		{
			get
			{
				return Math.Max(ActiveFont.LineHeight, (float)GFX.Gui["collectables/strawberry"].Height);
			}
		}

		// Token: 0x06001751 RID: 5969 RVA: 0x0008D884 File Offset: 0x0008BA84
		private void UpdateStrings()
		{
			this.sAmount = this.amount.ToString();
			if (this.outOf > -1)
			{
				this.sOutOf = "/" + this.outOf.ToString();
				return;
			}
			this.sOutOf = "";
		}

		// Token: 0x06001752 RID: 5970 RVA: 0x0008D8D2 File Offset: 0x0008BAD2
		public void Wiggle()
		{
			this.wiggler.Start();
			this.flashTimer = 0.5f;
		}

		// Token: 0x06001753 RID: 5971 RVA: 0x0008D8EA File Offset: 0x0008BAEA
		public override void Update()
		{
			base.Update();
			if (this.wiggler.Active)
			{
				this.wiggler.Update();
			}
			if (this.flashTimer > 0f)
			{
				this.flashTimer -= Engine.RawDeltaTime;
			}
		}

		// Token: 0x06001754 RID: 5972 RVA: 0x0008D92C File Offset: 0x0008BB2C
		public override void Render()
		{
			Vector2 value = this.RenderPosition;
			Vector2 vector = Calc.AngleToVector(this.Rotation, 1f);
			Vector2 value2 = new Vector2(-vector.Y, vector.X);
			string text = this.showOutOf ? this.sOutOf : "";
			float num = ActiveFont.Measure(this.sAmount).X;
			float num2 = ActiveFont.Measure(text).X;
			float num3 = 62f + (float)this.x.Width + 2f + num + num2;
			Color color = this.Color;
			if (this.flashTimer > 0f && base.Scene != null && base.Scene.BetweenRawInterval(0.05f))
			{
				color = StrawberriesCounter.FlashColor;
			}
			if (this.CenteredX)
			{
				value -= vector * (num3 / 2f) * this.Scale;
			}
			string id = this.Golden ? "collectables/goldberry" : "collectables/strawberry";
			GFX.Gui[id].DrawCentered(value + vector * 60f * 0.5f * this.Scale, Color.White, this.Scale);
			this.x.DrawCentered(value + vector * (62f + (float)this.x.Width * 0.5f) * this.Scale + value2 * 2f * this.Scale, color, this.Scale);
			ActiveFont.DrawOutline(this.sAmount, value + vector * (num3 - num2 - num * 0.5f) * this.Scale + value2 * (this.wiggler.Value * 18f) * this.Scale, new Vector2(0.5f, 0.5f), Vector2.One * this.Scale, color, this.Stroke, Color.Black);
			if (text != "")
			{
				ActiveFont.DrawOutline(text, value + vector * (num3 - num2 / 2f) * this.Scale, new Vector2(0.5f, 0.5f), Vector2.One * this.Scale, this.OutOfColor, this.Stroke, Color.Black);
			}
		}

		// Token: 0x1700019C RID: 412
		// (get) Token: 0x06001755 RID: 5973 RVA: 0x0008DBB5 File Offset: 0x0008BDB5
		public Vector2 RenderPosition
		{
			get
			{
				return (((base.Entity != null) ? base.Entity.Position : Vector2.Zero) + this.Position).Round();
			}
		}

		// Token: 0x040013EA RID: 5098
		public static readonly Color FlashColor = Calc.HexToColor("FF5E76");

		// Token: 0x040013EB RID: 5099
		private const int IconWidth = 60;

		// Token: 0x040013EC RID: 5100
		public bool Golden;

		// Token: 0x040013ED RID: 5101
		public Vector2 Position;

		// Token: 0x040013EE RID: 5102
		public bool CenteredX;

		// Token: 0x040013EF RID: 5103
		public bool CanWiggle = true;

		// Token: 0x040013F0 RID: 5104
		public float Scale = 1f;

		// Token: 0x040013F1 RID: 5105
		public float Stroke = 2f;

		// Token: 0x040013F2 RID: 5106
		public float Rotation;

		// Token: 0x040013F3 RID: 5107
		public Color Color = Color.White;

		// Token: 0x040013F4 RID: 5108
		public Color OutOfColor = Color.LightGray;

		// Token: 0x040013F5 RID: 5109
		public bool OverworldSfx;

		// Token: 0x040013F6 RID: 5110
		private int amount;

		// Token: 0x040013F7 RID: 5111
		private int outOf = -1;

		// Token: 0x040013F8 RID: 5112
		private Wiggler wiggler;

		// Token: 0x040013F9 RID: 5113
		private float flashTimer;

		// Token: 0x040013FA RID: 5114
		private string sAmount;

		// Token: 0x040013FB RID: 5115
		private string sOutOf;

		// Token: 0x040013FC RID: 5116
		private MTexture x;

		// Token: 0x040013FD RID: 5117
		private bool showOutOf;
	}
}
