using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020002EF RID: 751
	public class SpeedrunTimerDisplay : Entity
	{
		// Token: 0x06001742 RID: 5954 RVA: 0x0008D0AC File Offset: 0x0008B2AC
		public SpeedrunTimerDisplay()
		{
			base.Tag = (Tags.HUD | Tags.Global | Tags.PauseUpdate | Tags.TransitionUpdate);
			base.Depth = -100;
			base.Y = 60f;
			SpeedrunTimerDisplay.CalculateBaseSizes();
			base.Add(this.wiggler = Wiggler.Create(0.5f, 4f, null, false, false));
		}

		// Token: 0x06001743 RID: 5955 RVA: 0x0008D140 File Offset: 0x0008B340
		public static void CalculateBaseSizes()
		{
			PixelFont font = Dialog.Languages["english"].Font;
			float fontFaceSize = Dialog.Languages["english"].FontFaceSize;
			PixelFontSize pixelFontSize = font.Get(fontFaceSize);
			for (int i = 0; i < 10; i++)
			{
				float x = pixelFontSize.Measure(i.ToString()).X;
				if (x > SpeedrunTimerDisplay.numberWidth)
				{
					SpeedrunTimerDisplay.numberWidth = x;
				}
			}
			SpeedrunTimerDisplay.spacerWidth = pixelFontSize.Measure('.').X;
		}

		// Token: 0x06001744 RID: 5956 RVA: 0x0008D1C0 File Offset: 0x0008B3C0
		public override void Update()
		{
			Level level = base.Scene as Level;
			if (level.Completed)
			{
				if (this.CompleteTimer == 0f)
				{
					this.wiggler.Start();
				}
				this.CompleteTimer += Engine.DeltaTime;
			}
			bool flag = false;
			if (level.Session.Area.ID != 8 && !level.TimerHidden)
			{
				if (Settings.Instance.SpeedrunClock == SpeedrunType.Chapter)
				{
					if (this.CompleteTimer < 3f)
					{
						flag = true;
					}
				}
				else if (Settings.Instance.SpeedrunClock == SpeedrunType.File)
				{
					flag = true;
				}
			}
			this.DrawLerp = Calc.Approach(this.DrawLerp, (float)(flag ? 1 : 0), Engine.DeltaTime * 4f);
			base.Update();
		}

		// Token: 0x06001745 RID: 5957 RVA: 0x0008D280 File Offset: 0x0008B480
		public override void Render()
		{
			if (this.DrawLerp <= 0f)
			{
				return;
			}
			float num = -300f * Ease.CubeIn(1f - this.DrawLerp);
			Level level = base.Scene as Level;
			Session session = level.Session;
			if (Settings.Instance.SpeedrunClock == SpeedrunType.Chapter)
			{
				string timeString = TimeSpan.FromTicks(session.Time).ShortGameplayFormat();
				this.bg.Draw(new Vector2(num, base.Y));
				SpeedrunTimerDisplay.DrawTime(new Vector2(num + 32f, base.Y + 44f), timeString, 1f + this.wiggler.Value * 0.15f, session.StartedFromBeginning, level.Completed, session.BeatBestTime, 1f);
				return;
			}
			if (Settings.Instance.SpeedrunClock == SpeedrunType.File)
			{
				TimeSpan timeSpan = TimeSpan.FromTicks(session.Time);
				string timeString2;
				if (timeSpan.TotalHours >= 1.0)
				{
					timeString2 = (int)timeSpan.TotalHours + ":" + timeSpan.ToString("mm\\:ss");
				}
				else
				{
					timeString2 = timeSpan.ToString("mm\\:ss");
				}
				TimeSpan timeSpan2 = TimeSpan.FromTicks(SaveData.Instance.Time);
				int num2 = (int)timeSpan2.TotalHours;
				string timeString3 = num2.ToString() + timeSpan2.ToString("\\:mm\\:ss\\.fff");
				int num3 = (num2 < 10) ? 64 : ((num2 < 100) ? 96 : 128);
				Draw.Rect(num, base.Y, (float)(num3 + 2), 38f, Color.Black);
				this.bg.Draw(new Vector2(num + (float)num3, base.Y));
				SpeedrunTimerDisplay.DrawTime(new Vector2(num + 32f, base.Y + 44f), timeString3, 1f, true, false, false, 1f);
				this.bg.Draw(new Vector2(num, base.Y + 38f), Vector2.Zero, Color.White, 0.6f);
				SpeedrunTimerDisplay.DrawTime(new Vector2(num + 32f, base.Y + 40f + 26.400002f), timeString2, (1f + this.wiggler.Value * 0.15f) * 0.6f, session.StartedFromBeginning, level.Completed, session.BeatBestTime, 0.6f);
			}
		}

		// Token: 0x06001746 RID: 5958 RVA: 0x0008D4F0 File Offset: 0x0008B6F0
		public static void DrawTime(Vector2 position, string timeString, float scale = 1f, bool valid = true, bool finished = false, bool bestTime = false, float alpha = 1f)
		{
			PixelFont font = Dialog.Languages["english"].Font;
			float fontFaceSize = Dialog.Languages["english"].FontFaceSize;
			float num = scale;
			float num2 = position.X;
			float num3 = position.Y;
			Color color = Color.White * alpha;
			Color color2 = Color.LightGray * alpha;
			if (!valid)
			{
				color = Calc.HexToColor("918988") * alpha;
				color2 = Calc.HexToColor("7a6f6d") * alpha;
			}
			else if (bestTime)
			{
				color = Calc.HexToColor("fad768") * alpha;
				color2 = Calc.HexToColor("cfa727") * alpha;
			}
			else if (finished)
			{
				color = Calc.HexToColor("6ded87") * alpha;
				color2 = Calc.HexToColor("43d14c") * alpha;
			}
			foreach (char c in timeString)
			{
				if (c == '.')
				{
					num = scale * 0.7f;
					num3 -= 5f * scale;
				}
				Color color3 = (c == ':' || c == '.' || num < scale) ? color2 : color;
				float num4 = (((c == ':' || c == '.') ? SpeedrunTimerDisplay.spacerWidth : SpeedrunTimerDisplay.numberWidth) + 4f) * num;
				font.DrawOutline(fontFaceSize, c.ToString(), new Vector2(num2 + num4 / 2f, num3), new Vector2(0.5f, 1f), Vector2.One * num, color3, 2f, Color.Black);
				num2 += num4;
			}
		}

		// Token: 0x06001747 RID: 5959 RVA: 0x0008D698 File Offset: 0x0008B898
		public static float GetTimeWidth(string timeString, float scale = 1f)
		{
			float num = scale;
			float num2 = 0f;
			foreach (char c in timeString)
			{
				if (c == '.')
				{
					num = scale * 0.7f;
				}
				float num3 = (((c == ':' || c == '.') ? SpeedrunTimerDisplay.spacerWidth : SpeedrunTimerDisplay.numberWidth) + 4f) * num;
				num2 += num3;
			}
			return num2;
		}

		// Token: 0x040013E2 RID: 5090
		public float CompleteTimer;

		// Token: 0x040013E3 RID: 5091
		public const int GuiChapterHeight = 58;

		// Token: 0x040013E4 RID: 5092
		public const int GuiFileHeight = 78;

		// Token: 0x040013E5 RID: 5093
		private static float numberWidth;

		// Token: 0x040013E6 RID: 5094
		private static float spacerWidth;

		// Token: 0x040013E7 RID: 5095
		private MTexture bg = GFX.Gui["strawberryCountBG"];

		// Token: 0x040013E8 RID: 5096
		public float DrawLerp;

		// Token: 0x040013E9 RID: 5097
		private Wiggler wiggler;
	}
}
