using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002ED RID: 749
	public class DeathsCounter : Component
	{
		// Token: 0x0600172D RID: 5933 RVA: 0x0008C7D0 File Offset: 0x0008A9D0
		public DeathsCounter(AreaMode mode, bool centeredX, int amount, int minDigits = 0) : base(true, true)
		{
			this.CenteredX = centeredX;
			this.amount = amount;
			this.minDigits = minDigits;
			this.UpdateString();
			this.wiggler = Wiggler.Create(0.5f, 3f, null, false, false);
			this.wiggler.StartZero = true;
			this.wiggler.UseRawDeltaTime = true;
			this.iconWiggler = Wiggler.Create(0.5f, 3f, null, false, false);
			this.iconWiggler.UseRawDeltaTime = true;
			this.SetMode(mode);
			this.x = GFX.Gui["x"];
		}

		// Token: 0x0600172E RID: 5934 RVA: 0x0008C8A4 File Offset: 0x0008AAA4
		private void UpdateString()
		{
			if (this.minDigits > 0)
			{
				this.sAmount = this.amount.ToString("D" + this.minDigits);
				return;
			}
			this.sAmount = this.amount.ToString();
		}

		// Token: 0x17000194 RID: 404
		// (get) Token: 0x0600172F RID: 5935 RVA: 0x0008C8F2 File Offset: 0x0008AAF2
		// (set) Token: 0x06001730 RID: 5936 RVA: 0x0008C8FA File Offset: 0x0008AAFA
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
					this.UpdateString();
					if (this.CanWiggle)
					{
						this.wiggler.Start();
						this.flashTimer = 0.5f;
					}
				}
			}
		}

		// Token: 0x17000195 RID: 405
		// (get) Token: 0x06001731 RID: 5937 RVA: 0x0008C930 File Offset: 0x0008AB30
		// (set) Token: 0x06001732 RID: 5938 RVA: 0x0008C938 File Offset: 0x0008AB38
		public int MinDigits
		{
			get
			{
				return this.minDigits;
			}
			set
			{
				if (this.minDigits != value)
				{
					this.minDigits = value;
					this.UpdateString();
				}
			}
		}

		// Token: 0x06001733 RID: 5939 RVA: 0x0008C950 File Offset: 0x0008AB50
		public void SetMode(AreaMode mode)
		{
			if (mode == AreaMode.Normal)
			{
				this.icon = GFX.Gui["collectables/skullBlue"];
			}
			else if (mode == AreaMode.BSide)
			{
				this.icon = GFX.Gui["collectables/skullRed"];
			}
			else
			{
				this.icon = GFX.Gui["collectables/skullGold"];
			}
			this.iconWiggler.Start();
		}

		// Token: 0x06001734 RID: 5940 RVA: 0x0008C9B2 File Offset: 0x0008ABB2
		public void Wiggle()
		{
			this.wiggler.Start();
			this.flashTimer = 0.5f;
		}

		// Token: 0x06001735 RID: 5941 RVA: 0x0008C9CC File Offset: 0x0008ABCC
		public override void Update()
		{
			base.Update();
			if (this.wiggler.Active)
			{
				this.wiggler.Update();
			}
			if (this.iconWiggler.Active)
			{
				this.iconWiggler.Update();
			}
			if (this.flashTimer > 0f)
			{
				this.flashTimer -= Engine.RawDeltaTime;
			}
		}

		// Token: 0x06001736 RID: 5942 RVA: 0x0008CA30 File Offset: 0x0008AC30
		public override void Render()
		{
			Vector2 value = this.RenderPosition;
			float num = ActiveFont.Measure(this.sAmount).X;
			float num2 = 62f + (float)this.x.Width + 2f + num;
			Color color = this.Color;
			Color color2 = Color.Black;
			if (this.flashTimer > 0f && base.Scene != null && base.Scene.BetweenRawInterval(0.05f))
			{
				color = StrawberriesCounter.FlashColor;
			}
			if (this.Alpha < 1f)
			{
				color *= this.Alpha;
				color2 *= this.Alpha;
			}
			if (this.CenteredX)
			{
				value -= Vector2.UnitX * (num2 / 2f) * this.Scale;
			}
			this.icon.DrawCentered(value + new Vector2(30f, 0f) * this.Scale, Color.White * this.Alpha, this.Scale * (1f + this.iconWiggler.Value * 0.2f));
			this.x.DrawCentered(value + new Vector2(62f + (float)(this.x.Width / 2), 2f) * this.Scale, color, this.Scale);
			ActiveFont.DrawOutline(this.sAmount, value + new Vector2(num2 - num / 2f, -this.wiggler.Value * 18f) * this.Scale, new Vector2(0.5f, 0.5f), Vector2.One * this.Scale, color, this.Stroke, color2);
		}

		// Token: 0x17000196 RID: 406
		// (get) Token: 0x06001737 RID: 5943 RVA: 0x0008CBFD File Offset: 0x0008ADFD
		public Vector2 RenderPosition
		{
			get
			{
				return (((base.Entity != null) ? base.Entity.Position : Vector2.Zero) + this.Position).Round();
			}
		}

		// Token: 0x040013CA RID: 5066
		private const int IconWidth = 60;

		// Token: 0x040013CB RID: 5067
		public Vector2 Position;

		// Token: 0x040013CC RID: 5068
		public bool CenteredX;

		// Token: 0x040013CD RID: 5069
		public bool CanWiggle = true;

		// Token: 0x040013CE RID: 5070
		public float Alpha = 1f;

		// Token: 0x040013CF RID: 5071
		public float Scale = 1f;

		// Token: 0x040013D0 RID: 5072
		public float Stroke = 2f;

		// Token: 0x040013D1 RID: 5073
		public Color Color = Color.White;

		// Token: 0x040013D2 RID: 5074
		private int amount;

		// Token: 0x040013D3 RID: 5075
		private int minDigits;

		// Token: 0x040013D4 RID: 5076
		private Wiggler wiggler;

		// Token: 0x040013D5 RID: 5077
		private Wiggler iconWiggler;

		// Token: 0x040013D6 RID: 5078
		private float flashTimer;

		// Token: 0x040013D7 RID: 5079
		private string sAmount;

		// Token: 0x040013D8 RID: 5080
		private MTexture icon;

		// Token: 0x040013D9 RID: 5081
		private MTexture x;
	}
}
