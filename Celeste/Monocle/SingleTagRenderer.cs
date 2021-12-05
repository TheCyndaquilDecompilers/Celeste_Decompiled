using System;
using Microsoft.Xna.Framework.Graphics;

namespace Monocle
{
	// Token: 0x0200011C RID: 284
	public class SingleTagRenderer : Renderer
	{
		// Token: 0x060008EB RID: 2283 RVA: 0x000152D2 File Offset: 0x000134D2
		public SingleTagRenderer(BitTag tag)
		{
			this.Tag = tag;
			this.BlendState = BlendState.AlphaBlend;
			this.SamplerState = SamplerState.LinearClamp;
			this.Camera = new Camera();
		}

		// Token: 0x060008EC RID: 2284 RVA: 0x000091E2 File Offset: 0x000073E2
		public override void BeforeRender(Scene scene)
		{
		}

		// Token: 0x060008ED RID: 2285 RVA: 0x00015304 File Offset: 0x00013504
		public override void Render(Scene scene)
		{
			Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, this.BlendState, this.SamplerState, DepthStencilState.None, RasterizerState.CullNone, this.Effect, this.Camera.Matrix * Engine.ScreenMatrix);
			foreach (Entity entity in scene[this.Tag])
			{
				if (entity.Visible)
				{
					entity.Render();
				}
			}
			if (Engine.Commands.Open)
			{
				foreach (Entity entity2 in scene[this.Tag])
				{
					entity2.DebugRender(this.Camera);
				}
			}
			Draw.SpriteBatch.End();
		}

		// Token: 0x060008EE RID: 2286 RVA: 0x000091E2 File Offset: 0x000073E2
		public override void AfterRender(Scene scene)
		{
		}

		// Token: 0x04000602 RID: 1538
		public BitTag Tag;

		// Token: 0x04000603 RID: 1539
		public BlendState BlendState;

		// Token: 0x04000604 RID: 1540
		public SamplerState SamplerState;

		// Token: 0x04000605 RID: 1541
		public Effect Effect;

		// Token: 0x04000606 RID: 1542
		public Camera Camera;
	}
}
