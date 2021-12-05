using System;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x02000311 RID: 785
	public class GameplayRenderer : Renderer
	{
		// Token: 0x060018E0 RID: 6368 RVA: 0x0009BEA0 File Offset: 0x0009A0A0
		public GameplayRenderer()
		{
			GameplayRenderer.instance = this;
			this.Camera = new Camera(320, 180);
		}

		// Token: 0x060018E1 RID: 6369 RVA: 0x0009BEC3 File Offset: 0x0009A0C3
		public static void Begin()
		{
			Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, GameplayRenderer.instance.Camera.Matrix);
		}

		// Token: 0x060018E2 RID: 6370 RVA: 0x0009BEF4 File Offset: 0x0009A0F4
		public override void Render(Scene scene)
		{
			GameplayRenderer.Begin();
			scene.Entities.RenderExcept(Tags.HUD);
			if (Engine.Commands.Open)
			{
				scene.Entities.DebugRender(this.Camera);
			}
			GameplayRenderer.End();
		}

		// Token: 0x060018E3 RID: 6371 RVA: 0x000556B2 File Offset: 0x000538B2
		public static void End()
		{
			Draw.SpriteBatch.End();
		}

		// Token: 0x04001551 RID: 5457
		public Camera Camera;

		// Token: 0x04001552 RID: 5458
		private static GameplayRenderer instance;
	}
}
