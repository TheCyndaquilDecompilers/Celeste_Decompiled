using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001D6 RID: 470
	public class WaveDashPage05 : WaveDashPage
	{
		// Token: 0x06000FD1 RID: 4049 RVA: 0x000433AA File Offset: 0x000415AA
		public WaveDashPage05()
		{
			this.Transition = WaveDashPage.Transitions.Spiral;
			this.ClearColor = Calc.HexToColor("fff2cc");
		}

		// Token: 0x06000FD2 RID: 4050 RVA: 0x000433D4 File Offset: 0x000415D4
		public override void Added(WaveDashPresentation presentation)
		{
			base.Added(presentation);
			this.displays.Add(new WaveDashPage05.Display(new Vector2((float)base.Width * 0.28f, (float)(base.Height - 600)), Dialog.Get("WAVEDASH_PAGE5_INFO1", null), "too_close", new Vector2(-50f, 20f)));
			this.displays.Add(new WaveDashPage05.Display(new Vector2((float)base.Width * 0.72f, (float)(base.Height - 600)), Dialog.Get("WAVEDASH_PAGE5_INFO2", null), "too_far", new Vector2(-50f, -35f)));
		}

		// Token: 0x06000FD3 RID: 4051 RVA: 0x00043484 File Offset: 0x00041684
		public override IEnumerator Routine()
		{
			yield return 0.5f;
			yield break;
		}

		// Token: 0x06000FD4 RID: 4052 RVA: 0x0004348C File Offset: 0x0004168C
		public override void Update()
		{
			foreach (WaveDashPage05.Display display in this.displays)
			{
				display.Update();
			}
		}

		// Token: 0x06000FD5 RID: 4053 RVA: 0x000434DC File Offset: 0x000416DC
		public override void Render()
		{
			ActiveFont.DrawOutline(Dialog.Clean("WAVEDASH_PAGE5_TITLE", null), new Vector2(128f, 100f), Vector2.Zero, Vector2.One * 1.5f, Color.White, 2f, Color.Black);
			foreach (WaveDashPage05.Display display in this.displays)
			{
				display.Render();
			}
		}

		// Token: 0x04000B37 RID: 2871
		private List<WaveDashPage05.Display> displays = new List<WaveDashPage05.Display>();

		// Token: 0x020004D9 RID: 1241
		private class Display
		{
			// Token: 0x06002445 RID: 9285 RVA: 0x000F30A0 File Offset: 0x000F12A0
			public Display(Vector2 position, string text, string tutorial, Vector2 tutorialOffset)
			{
				this.Position = position;
				this.Info = FancyText.Parse(text, 896, 8, 1f, new Color?(Color.Black * 0.6f), null);
				this.Tutorial = new WaveDashPlaybackTutorial(tutorial, tutorialOffset, new Vector2(1f, 1f), new Vector2(1f, 1f));
				this.Tutorial.OnRender = delegate()
				{
					Draw.Line(-64f, 20f, 64f, 20f, Color.Black);
				};
				this.routine = new Coroutine(this.Routine(), true);
			}

			// Token: 0x06002446 RID: 9286 RVA: 0x000F314E File Offset: 0x000F134E
			private IEnumerator Routine()
			{
				PlayerPlayback playback = this.Tutorial.Playback;
				int step = 0;
				for (;;)
				{
					int frameIndex = playback.FrameIndex;
					if (step % 2 == 0)
					{
						this.Tutorial.Update();
					}
					if (frameIndex != playback.FrameIndex && playback.FrameIndex == playback.FrameCount - 1)
					{
						while (this.time < 3f)
						{
							yield return null;
						}
						yield return 0.1f;
						while (this.xEase < 1f)
						{
							this.xEase = Calc.Approach(this.xEase, 1f, Engine.DeltaTime * 4f);
							yield return null;
						}
						this.xEase = 1f;
						yield return 0.5f;
						this.xEase = 0f;
						this.time = 0f;
					}
					int num = step;
					step = num + 1;
					yield return null;
				}
				yield break;
			}

			// Token: 0x06002447 RID: 9287 RVA: 0x000F315D File Offset: 0x000F135D
			public void Update()
			{
				this.time += Engine.DeltaTime;
				this.routine.Update();
			}

			// Token: 0x06002448 RID: 9288 RVA: 0x000F317C File Offset: 0x000F137C
			public void Render()
			{
				this.Tutorial.Render(this.Position, 4f);
				this.Info.DrawJustifyPerLine(this.Position + Vector2.UnitY * 200f, new Vector2(0.5f, 0f), Vector2.One * 0.8f, 1f, 0, int.MaxValue);
				if (this.xEase > 0f)
				{
					Vector2 vector = Calc.AngleToVector((1f - this.xEase) * 0.1f + 0.7853982f, 1f);
					Vector2 value = vector.Perpendicular();
					float num = 0.5f + (1f - this.xEase) * 0.5f;
					float thickness = 64f * num;
					float scaleFactor = 300f * num;
					Vector2 position = this.Position;
					Draw.Line(position - vector * scaleFactor, position + vector * scaleFactor, Color.Red, thickness);
					Draw.Line(position - value * scaleFactor, position + value * scaleFactor, Color.Red, thickness);
				}
			}

			// Token: 0x040023E5 RID: 9189
			public Vector2 Position;

			// Token: 0x040023E6 RID: 9190
			public FancyText.Text Info;

			// Token: 0x040023E7 RID: 9191
			public WaveDashPlaybackTutorial Tutorial;

			// Token: 0x040023E8 RID: 9192
			private Coroutine routine;

			// Token: 0x040023E9 RID: 9193
			private float xEase;

			// Token: 0x040023EA RID: 9194
			private float time;
		}
	}
}
