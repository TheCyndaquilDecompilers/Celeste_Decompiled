using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000217 RID: 535
	public class Overlay : Entity
	{
		// Token: 0x06001154 RID: 4436 RVA: 0x00055560 File Offset: 0x00053760
		public Overlay()
		{
			base.Tag = Tags.HUD;
			base.Depth = -100000;
		}

		// Token: 0x06001155 RID: 4437 RVA: 0x00055584 File Offset: 0x00053784
		public override void Added(Scene scene)
		{
			IOverlayHandler overlayHandler = scene as IOverlayHandler;
			if (overlayHandler != null)
			{
				overlayHandler.Overlay = this;
			}
			base.Added(scene);
		}

		// Token: 0x06001156 RID: 4438 RVA: 0x000555AC File Offset: 0x000537AC
		public override void Removed(Scene scene)
		{
			IOverlayHandler overlayHandler = scene as IOverlayHandler;
			if (overlayHandler != null && overlayHandler.Overlay == this)
			{
				overlayHandler.Overlay = null;
			}
			base.Removed(scene);
		}

		// Token: 0x06001157 RID: 4439 RVA: 0x000555DA File Offset: 0x000537DA
		public IEnumerator FadeIn()
		{
			while (this.Fade < 1f)
			{
				yield return null;
				this.Fade += Engine.DeltaTime * 4f;
			}
			this.Fade = 1f;
			yield break;
		}

		// Token: 0x06001158 RID: 4440 RVA: 0x000555E9 File Offset: 0x000537E9
		public IEnumerator FadeOut()
		{
			while (this.Fade > 0f)
			{
				yield return null;
				this.Fade -= Engine.DeltaTime * 4f;
			}
			yield break;
		}

		// Token: 0x06001159 RID: 4441 RVA: 0x000555F8 File Offset: 0x000537F8
		public void RenderFade()
		{
			Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * Ease.CubeInOut(this.Fade) * 0.95f);
		}

		// Token: 0x04000CF8 RID: 3320
		public float Fade;

		// Token: 0x04000CF9 RID: 3321
		public bool XboxOverlay;
	}
}
