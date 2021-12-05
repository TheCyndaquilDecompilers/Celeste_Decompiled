using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x020001D8 RID: 472
	public class WaveDashPage06 : WaveDashPage
	{
		// Token: 0x06000FDB RID: 4059 RVA: 0x00043722 File Offset: 0x00041922
		public WaveDashPage06()
		{
			this.Transition = WaveDashPage.Transitions.Rotate3D;
			this.ClearColor = Calc.HexToColor("d9d2e9");
		}

		// Token: 0x06000FDC RID: 4060 RVA: 0x00043741 File Offset: 0x00041941
		public override IEnumerator Routine()
		{
			yield return 1f;
			Audio.Play("event:/new_content/game/10_farewell/ppt_happy_wavedashing");
			this.title = new AreaCompleteTitle(new Vector2((float)base.Width / 2f, 150f), Dialog.Clean("WAVEDASH_PAGE6_TITLE", null), 2f, true);
			yield return 1.5f;
			yield break;
		}

		// Token: 0x06000FDD RID: 4061 RVA: 0x00043750 File Offset: 0x00041950
		public override void Update()
		{
			if (this.title != null)
			{
				this.title.Update();
			}
		}

		// Token: 0x06000FDE RID: 4062 RVA: 0x00043768 File Offset: 0x00041968
		public override void Render()
		{
			this.Presentation.Gfx["Bird Clip Art"].DrawCentered(new Vector2((float)base.Width, (float)base.Height) / 2f, Color.White, 1.5f);
			if (this.title != null)
			{
				this.title.Render();
			}
		}

		// Token: 0x04000B3C RID: 2876
		private AreaCompleteTitle title;
	}
}
