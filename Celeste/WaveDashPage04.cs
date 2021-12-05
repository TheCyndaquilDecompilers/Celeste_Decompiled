using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001D7 RID: 471
	public class WaveDashPage04 : WaveDashPage
	{
		// Token: 0x06000FD6 RID: 4054 RVA: 0x00043570 File Offset: 0x00041770
		public WaveDashPage04()
		{
			this.Transition = WaveDashPage.Transitions.FadeIn;
			this.ClearColor = Calc.HexToColor("f4cccc");
		}

		// Token: 0x06000FD7 RID: 4055 RVA: 0x00043590 File Offset: 0x00041790
		public override void Added(WaveDashPresentation presentation)
		{
			base.Added(presentation);
			List<MTexture> textures = this.Presentation.Gfx.GetAtlasSubtextures("playback/platforms");
			this.tutorial = new WaveDashPlaybackTutorial("wavedashppt", new Vector2(-126f, 0f), new Vector2(1f, 1f), new Vector2(1f, -1f));
			this.tutorial.OnRender = delegate()
			{
				textures[(int)(this.time % (float)textures.Count)].DrawCentered(Vector2.Zero);
			};
		}

		// Token: 0x06000FD8 RID: 4056 RVA: 0x00043620 File Offset: 0x00041820
		public override IEnumerator Routine()
		{
			yield return 0.5f;
			this.list = FancyText.Parse(Dialog.Get("WAVEDASH_PAGE4_LIST", null), base.Width, 32, 1f, new Color?(Color.Black * 0.7f), null);
			float delay = 0f;
			while (this.listIndex < this.list.Nodes.Count)
			{
				if (this.list.Nodes[this.listIndex] is FancyText.NewLine)
				{
					yield return base.PressButton();
				}
				else
				{
					delay += 0.008f;
					if (delay >= 0.016f)
					{
						delay -= 0.016f;
						yield return 0.016f;
					}
				}
				this.listIndex++;
			}
			yield break;
		}

		// Token: 0x06000FD9 RID: 4057 RVA: 0x0004362F File Offset: 0x0004182F
		public override void Update()
		{
			this.time += Engine.DeltaTime * 4f;
			this.tutorial.Update();
		}

		// Token: 0x06000FDA RID: 4058 RVA: 0x00043654 File Offset: 0x00041854
		public override void Render()
		{
			ActiveFont.DrawOutline(Dialog.Clean("WAVEDASH_PAGE4_TITLE", null), new Vector2(128f, 100f), Vector2.Zero, Vector2.One * 1.5f, Color.White, 2f, Color.Black);
			this.tutorial.Render(new Vector2((float)base.Width / 2f, (float)base.Height / 2f - 100f), 4f);
			if (this.list != null)
			{
				this.list.Draw(new Vector2(160f, (float)(base.Height - 400)), new Vector2(0f, 0f), Vector2.One, 1f, 0, this.listIndex);
			}
		}

		// Token: 0x04000B38 RID: 2872
		private WaveDashPlaybackTutorial tutorial;

		// Token: 0x04000B39 RID: 2873
		private FancyText.Text list;

		// Token: 0x04000B3A RID: 2874
		private int listIndex;

		// Token: 0x04000B3B RID: 2875
		private float time;
	}
}
