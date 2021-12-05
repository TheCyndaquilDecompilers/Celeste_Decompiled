using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002EC RID: 748
	public class BestTimeDisplay : Component
	{
		// Token: 0x06001723 RID: 5923 RVA: 0x0008C53C File Offset: 0x0008A73C
		public BestTimeDisplay(BestTimeDisplay.Modes mode, TimeSpan time) : base(true, true)
		{
			this.time = time;
			this.UpdateString();
			this.wiggler = Wiggler.Create(0.5f, 3f, null, false, false);
			this.wiggler.UseRawDeltaTime = true;
			switch (mode)
			{
			default:
				this.icon = GFX.Game["gui/bestTime"];
				this.iconColor = BestTimeDisplay.IconColor;
				return;
			case BestTimeDisplay.Modes.BestFullClear:
				this.icon = GFX.Game["gui/bestFullClearTime"];
				this.iconColor = BestTimeDisplay.FullClearColor;
				return;
			case BestTimeDisplay.Modes.Current:
				this.icon = null;
				return;
			}
		}

		// Token: 0x06001724 RID: 5924 RVA: 0x0008C5DD File Offset: 0x0008A7DD
		private void UpdateString()
		{
			this.sTime = this.time.ShortGameplayFormat();
		}

		// Token: 0x06001725 RID: 5925 RVA: 0x0008C5F0 File Offset: 0x0008A7F0
		public void Wiggle()
		{
			this.wiggler.Start();
			this.flashTimer = 0.5f;
		}

		// Token: 0x17000191 RID: 401
		// (get) Token: 0x06001726 RID: 5926 RVA: 0x0008C608 File Offset: 0x0008A808
		// (set) Token: 0x06001727 RID: 5927 RVA: 0x0008C610 File Offset: 0x0008A810
		public TimeSpan Time
		{
			get
			{
				return this.time;
			}
			set
			{
				if (this.time != value)
				{
					this.time = value;
					this.UpdateString();
					this.wiggler.Start();
					this.flashTimer = 0.5f;
				}
			}
		}

		// Token: 0x06001728 RID: 5928 RVA: 0x0008C643 File Offset: 0x0008A843
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

		// Token: 0x06001729 RID: 5929 RVA: 0x0008C684 File Offset: 0x0008A884
		public override void Render()
		{
			if (this.WillRender)
			{
				Vector2 value = this.RenderPosition;
				value -= Vector2.UnitY * this.wiggler.Value * 3f;
				Color color = Color.White;
				if (this.flashTimer > 0f && base.Scene.BetweenRawInterval(0.05f))
				{
					color = StrawberriesCounter.FlashColor;
				}
				if (this.icon != null)
				{
					this.icon.DrawOutlineCentered(value + new Vector2(-4f, -3f), this.iconColor);
				}
				ActiveFont.DrawOutline(this.sTime, value + new Vector2(0f, 4f), new Vector2(0.5f, 0f), Vector2.One, color, 2f, Color.Black);
			}
		}

		// Token: 0x17000192 RID: 402
		// (get) Token: 0x0600172A RID: 5930 RVA: 0x0008C761 File Offset: 0x0008A961
		public Vector2 RenderPosition
		{
			get
			{
				return (base.Entity.Position + this.Position).Round();
			}
		}

		// Token: 0x17000193 RID: 403
		// (get) Token: 0x0600172B RID: 5931 RVA: 0x0008C77E File Offset: 0x0008A97E
		public bool WillRender
		{
			get
			{
				return this.time > TimeSpan.Zero;
			}
		}

		// Token: 0x040013C1 RID: 5057
		private static readonly Color IconColor = Color.Lerp(Calc.HexToColor("7CFF70"), Color.Black, 0.25f);

		// Token: 0x040013C2 RID: 5058
		private static readonly Color FullClearColor = Color.Lerp(Calc.HexToColor("FF3D57"), Color.Black, 0.25f);

		// Token: 0x040013C3 RID: 5059
		public Vector2 Position;

		// Token: 0x040013C4 RID: 5060
		private TimeSpan time;

		// Token: 0x040013C5 RID: 5061
		private string sTime;

		// Token: 0x040013C6 RID: 5062
		private Wiggler wiggler;

		// Token: 0x040013C7 RID: 5063
		private MTexture icon;

		// Token: 0x040013C8 RID: 5064
		private float flashTimer;

		// Token: 0x040013C9 RID: 5065
		private Color iconColor;

		// Token: 0x020006A0 RID: 1696
		public enum Modes
		{
			// Token: 0x04002B70 RID: 11120
			Best,
			// Token: 0x04002B71 RID: 11121
			BestFullClear,
			// Token: 0x04002B72 RID: 11122
			Current
		}
	}
}
