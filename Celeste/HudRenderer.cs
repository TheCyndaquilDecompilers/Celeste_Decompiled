using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste
{
	// Token: 0x02000312 RID: 786
	public class HudRenderer : HiresRenderer
	{
		// Token: 0x060018E4 RID: 6372 RVA: 0x0009BF34 File Offset: 0x0009A134
		public override void RenderContent(Scene scene)
		{
			if (scene.Entities.HasVisibleEntities(Tags.HUD) || this.BackgroundFade > 0f)
			{
				HiresRenderer.BeginRender(null, null);
				if (this.BackgroundFade > 0f)
				{
					Draw.Rect(-1f, -1f, 1922f, 1082f, Color.Black * this.BackgroundFade * 0.7f);
				}
				scene.Entities.RenderOnly(Tags.HUD);
				HiresRenderer.EndRender();
			}
		}

		// Token: 0x04001553 RID: 5459
		public float BackgroundFade;
	}
}
