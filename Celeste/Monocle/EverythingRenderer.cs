using System;
using Microsoft.Xna.Framework.Graphics;

namespace Monocle
{
	// Token: 0x0200011A RID: 282
	public class EverythingRenderer : Renderer
	{
		// Token: 0x060008E2 RID: 2274 RVA: 0x0001521F File Offset: 0x0001341F
		public EverythingRenderer()
		{
			this.BlendState = BlendState.AlphaBlend;
			this.SamplerState = SamplerState.LinearClamp;
			this.Camera = new Camera();
		}

		// Token: 0x060008E3 RID: 2275 RVA: 0x000091E2 File Offset: 0x000073E2
		public override void BeforeRender(Scene scene)
		{
		}

		// Token: 0x060008E4 RID: 2276 RVA: 0x00015248 File Offset: 0x00013448
		public override void Render(Scene scene)
		{
			Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, this.BlendState, this.SamplerState, DepthStencilState.None, RasterizerState.CullNone, this.Effect, this.Camera.Matrix * Engine.ScreenMatrix);
			scene.Entities.Render();
			if (Engine.Commands.Open)
			{
				scene.Entities.DebugRender(this.Camera);
			}
			Draw.SpriteBatch.End();
		}

		// Token: 0x060008E5 RID: 2277 RVA: 0x000091E2 File Offset: 0x000073E2
		public override void AfterRender(Scene scene)
		{
		}

		// Token: 0x040005FD RID: 1533
		public BlendState BlendState;

		// Token: 0x040005FE RID: 1534
		public SamplerState SamplerState;

		// Token: 0x040005FF RID: 1535
		public Effect Effect;

		// Token: 0x04000600 RID: 1536
		public Camera Camera;
	}
}
