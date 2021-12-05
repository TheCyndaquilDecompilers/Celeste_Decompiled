using System;
using Microsoft.Xna.Framework.Graphics;

namespace Monocle
{
	// Token: 0x0200011D RID: 285
	public class TagExcludeRenderer : Renderer
	{
		// Token: 0x060008EF RID: 2287 RVA: 0x00015404 File Offset: 0x00013604
		public TagExcludeRenderer(int excludeTag)
		{
			this.ExcludeTag = excludeTag;
			this.BlendState = BlendState.AlphaBlend;
			this.SamplerState = SamplerState.LinearClamp;
			this.Camera = new Camera();
		}

		// Token: 0x060008F0 RID: 2288 RVA: 0x000091E2 File Offset: 0x000073E2
		public override void BeforeRender(Scene scene)
		{
		}

		// Token: 0x060008F1 RID: 2289 RVA: 0x00015434 File Offset: 0x00013634
		public override void Render(Scene scene)
		{
			Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, this.BlendState, this.SamplerState, DepthStencilState.None, RasterizerState.CullNone, this.Effect, this.Camera.Matrix * Engine.ScreenMatrix);
			foreach (Entity entity in scene.Entities)
			{
				if (entity.Visible && (entity.Tag & this.ExcludeTag) == 0)
				{
					entity.Render();
				}
			}
			if (Engine.Commands.Open)
			{
				foreach (Entity entity2 in scene.Entities)
				{
					if ((entity2.Tag & this.ExcludeTag) == 0)
					{
						entity2.DebugRender(this.Camera);
					}
				}
			}
			Draw.SpriteBatch.End();
		}

		// Token: 0x060008F2 RID: 2290 RVA: 0x000091E2 File Offset: 0x000073E2
		public override void AfterRender(Scene scene)
		{
		}

		// Token: 0x04000607 RID: 1543
		public BlendState BlendState;

		// Token: 0x04000608 RID: 1544
		public SamplerState SamplerState;

		// Token: 0x04000609 RID: 1545
		public Effect Effect;

		// Token: 0x0400060A RID: 1546
		public Camera Camera;

		// Token: 0x0400060B RID: 1547
		public int ExcludeTag;
	}
}
