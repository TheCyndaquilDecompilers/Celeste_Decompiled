using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001DA RID: 474
	public class WaveDashPage01 : WaveDashPage
	{
		// Token: 0x06000FE4 RID: 4068 RVA: 0x000439E4 File Offset: 0x00041BE4
		public WaveDashPage01()
		{
			this.Transition = WaveDashPage.Transitions.ScaleIn;
			this.ClearColor = Calc.HexToColor("9fc5e8");
		}

		// Token: 0x06000FE5 RID: 4069 RVA: 0x000437F3 File Offset: 0x000419F3
		public override void Added(WaveDashPresentation presentation)
		{
			base.Added(presentation);
		}

		// Token: 0x06000FE6 RID: 4070 RVA: 0x00043A03 File Offset: 0x00041C03
		public override IEnumerator Routine()
		{
			Audio.SetAltMusic("event:/new_content/music/lvl10/intermission_powerpoint");
			yield return 1f;
			this.title = new AreaCompleteTitle(new Vector2((float)base.Width / 2f, (float)base.Height / 2f - 100f), Dialog.Clean("WAVEDASH_PAGE1_TITLE", null), 2f, true);
			yield return 1f;
			while (this.subtitleEase < 1f)
			{
				this.subtitleEase = Calc.Approach(this.subtitleEase, 1f, Engine.DeltaTime);
				yield return null;
			}
			yield return 0.1f;
			yield break;
		}

		// Token: 0x06000FE7 RID: 4071 RVA: 0x00043A12 File Offset: 0x00041C12
		public override void Update()
		{
			if (this.title != null)
			{
				this.title.Update();
			}
		}

		// Token: 0x06000FE8 RID: 4072 RVA: 0x00043A28 File Offset: 0x00041C28
		public override void Render()
		{
			if (this.title != null)
			{
				this.title.Render();
			}
			if (this.subtitleEase > 0f)
			{
				Vector2 position = new Vector2((float)base.Width / 2f, (float)base.Height / 2f + 80f);
				float x = 1f + Ease.BigBackIn(1f - this.subtitleEase) * 2f;
				float y = 0.25f + Ease.BigBackIn(this.subtitleEase) * 0.75f;
				ActiveFont.Draw(Dialog.Clean("WAVEDASH_PAGE1_SUBTITLE", null), position, new Vector2(0.5f, 0.5f), new Vector2(x, y), Color.Black * 0.8f);
			}
		}

		// Token: 0x04000B41 RID: 2881
		private AreaCompleteTitle title;

		// Token: 0x04000B42 RID: 2882
		private float subtitleEase;
	}
}
