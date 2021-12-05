using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001D5 RID: 469
	public class WaveDashPage03 : WaveDashPage
	{
		// Token: 0x06000FCC RID: 4044 RVA: 0x000431E7 File Offset: 0x000413E7
		public WaveDashPage03()
		{
			this.Transition = WaveDashPage.Transitions.Blocky;
			this.ClearColor = Calc.HexToColor("d9ead3");
			this.title = Dialog.Clean("WAVEDASH_PAGE3_TITLE", null);
			this.titleDisplayed = "";
		}

		// Token: 0x06000FCD RID: 4045 RVA: 0x00043222 File Offset: 0x00041422
		public override void Added(WaveDashPresentation presentation)
		{
			base.Added(presentation);
			this.clipArt = presentation.Gfx["moveset"];
		}

		// Token: 0x06000FCE RID: 4046 RVA: 0x00043241 File Offset: 0x00041441
		public override IEnumerator Routine()
		{
			while (this.titleDisplayed.Length < this.title.Length)
			{
				this.titleDisplayed += this.title[this.titleDisplayed.Length].ToString();
				yield return 0.05f;
			}
			yield return base.PressButton();
			Audio.Play("event:/new_content/game/10_farewell/ppt_wavedash_whoosh");
			while (this.clipArtEase < 1f)
			{
				this.clipArtEase = Calc.Approach(this.clipArtEase, 1f, Engine.DeltaTime);
				yield return null;
			}
			yield return 0.25f;
			this.infoText = FancyText.Parse(Dialog.Get("WAVEDASH_PAGE3_INFO", null), base.Width - 240, 32, 1f, new Color?(Color.Black * 0.7f), null);
			yield return base.PressButton();
			Audio.Play("event:/new_content/game/10_farewell/ppt_its_easy");
			this.easyText = new AreaCompleteTitle(new Vector2((float)base.Width / 2f, (float)(base.Height - 150)), Dialog.Clean("WAVEDASH_PAGE3_EASY", null), 2f, true);
			yield return 1f;
			yield break;
		}

		// Token: 0x06000FCF RID: 4047 RVA: 0x00043250 File Offset: 0x00041450
		public override void Update()
		{
			if (this.easyText != null)
			{
				this.easyText.Update();
			}
		}

		// Token: 0x06000FD0 RID: 4048 RVA: 0x00043268 File Offset: 0x00041468
		public override void Render()
		{
			ActiveFont.DrawOutline(this.titleDisplayed, new Vector2(128f, 100f), Vector2.Zero, Vector2.One * 1.5f, Color.White, 2f, Color.Black);
			if (this.clipArtEase > 0f)
			{
				Vector2 scale = Vector2.One * (1f + (1f - this.clipArtEase) * 3f) * 0.8f;
				float rotation = (1f - this.clipArtEase) * 8f;
				Color color = Color.White * this.clipArtEase;
				this.clipArt.DrawCentered(new Vector2((float)base.Width / 2f, (float)base.Height / 2f - 90f), color, scale, rotation);
			}
			if (this.infoText != null)
			{
				this.infoText.Draw(new Vector2((float)base.Width / 2f, (float)(base.Height - 350)), new Vector2(0.5f, 0f), Vector2.One, 1f, 0, int.MaxValue);
			}
			if (this.easyText != null)
			{
				this.easyText.Render();
			}
		}

		// Token: 0x04000B31 RID: 2865
		private string title;

		// Token: 0x04000B32 RID: 2866
		private string titleDisplayed;

		// Token: 0x04000B33 RID: 2867
		private MTexture clipArt;

		// Token: 0x04000B34 RID: 2868
		private float clipArtEase;

		// Token: 0x04000B35 RID: 2869
		private FancyText.Text infoText;

		// Token: 0x04000B36 RID: 2870
		private AreaCompleteTitle easyText;
	}
}
