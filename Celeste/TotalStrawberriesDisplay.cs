using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002E1 RID: 737
	public class TotalStrawberriesDisplay : Entity
	{
		// Token: 0x060016BA RID: 5818 RVA: 0x00086A3C File Offset: 0x00084C3C
		public TotalStrawberriesDisplay()
		{
			base.Y = 96f;
			base.Depth = -101;
			base.Tag = (Tags.HUD | Tags.Global | Tags.PauseUpdate | Tags.TransitionUpdate);
			this.bg = GFX.Gui["strawberryCountBG"];
			base.Add(this.strawberries = new StrawberriesCounter(false, SaveData.Instance.TotalStrawberries, 0, false));
		}

		// Token: 0x060016BB RID: 5819 RVA: 0x00086ACC File Offset: 0x00084CCC
		public override void Update()
		{
			base.Update();
			Level level = base.Scene as Level;
			if (SaveData.Instance.TotalStrawberries > this.strawberries.Amount && this.strawberriesUpdateTimer <= 0f)
			{
				this.strawberriesUpdateTimer = 0.4f;
			}
			if (SaveData.Instance.TotalStrawberries > this.strawberries.Amount || this.strawberriesUpdateTimer > 0f || this.strawberriesWaitTimer > 0f || (level.Paused && level.PauseMainMenuOpen))
			{
				this.DrawLerp = Calc.Approach(this.DrawLerp, 1f, 1.2f * Engine.RawDeltaTime);
			}
			else
			{
				this.DrawLerp = Calc.Approach(this.DrawLerp, 0f, 2f * Engine.RawDeltaTime);
			}
			if (this.strawberriesWaitTimer > 0f)
			{
				this.strawberriesWaitTimer -= Engine.RawDeltaTime;
			}
			if (this.strawberriesUpdateTimer > 0f && this.DrawLerp == 1f)
			{
				this.strawberriesUpdateTimer -= Engine.RawDeltaTime;
				if (this.strawberriesUpdateTimer <= 0f)
				{
					if (this.strawberries.Amount < SaveData.Instance.TotalStrawberries)
					{
						StrawberriesCounter strawberriesCounter = this.strawberries;
						int amount = strawberriesCounter.Amount;
						strawberriesCounter.Amount = amount + 1;
					}
					this.strawberriesWaitTimer = 2f;
					if (this.strawberries.Amount < SaveData.Instance.TotalStrawberries)
					{
						this.strawberriesUpdateTimer = 0.3f;
					}
				}
			}
			if (this.Visible)
			{
				float num = 96f;
				if (!level.TimerHidden)
				{
					if (Settings.Instance.SpeedrunClock == SpeedrunType.Chapter)
					{
						num += 58f;
					}
					else if (Settings.Instance.SpeedrunClock == SpeedrunType.File)
					{
						num += 78f;
					}
				}
				base.Y = Calc.Approach(base.Y, num, Engine.DeltaTime * 800f);
			}
			this.Visible = (this.DrawLerp > 0f);
		}

		// Token: 0x060016BC RID: 5820 RVA: 0x00086CC8 File Offset: 0x00084EC8
		public override void Render()
		{
			Vector2 vector = Vector2.Lerp(new Vector2((float)(-(float)this.bg.Width), base.Y), new Vector2(32f, base.Y), Ease.CubeOut(this.DrawLerp));
			vector = vector.Round();
			this.bg.DrawJustified(vector + new Vector2(-96f, 12f), new Vector2(0f, 0.5f));
			this.strawberries.Position = vector + new Vector2(0f, -base.Y);
			this.strawberries.Render();
		}

		// Token: 0x0400133B RID: 4923
		private const float NumberUpdateDelay = 0.4f;

		// Token: 0x0400133C RID: 4924
		private const float ComboUpdateDelay = 0.3f;

		// Token: 0x0400133D RID: 4925
		private const float AfterUpdateDelay = 2f;

		// Token: 0x0400133E RID: 4926
		private const float LerpInSpeed = 1.2f;

		// Token: 0x0400133F RID: 4927
		private const float LerpOutSpeed = 2f;

		// Token: 0x04001340 RID: 4928
		public static readonly Color FlashColor = Calc.HexToColor("FF5E76");

		// Token: 0x04001341 RID: 4929
		private MTexture bg;

		// Token: 0x04001342 RID: 4930
		public float DrawLerp;

		// Token: 0x04001343 RID: 4931
		private float strawberriesUpdateTimer;

		// Token: 0x04001344 RID: 4932
		private float strawberriesWaitTimer;

		// Token: 0x04001345 RID: 4933
		private StrawberriesCounter strawberries;
	}
}
