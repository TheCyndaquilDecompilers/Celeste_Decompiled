using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x02000294 RID: 660
	public class ParticleRenderer : Renderer
	{
		// Token: 0x06001471 RID: 5233 RVA: 0x0006F9B4 File Offset: 0x0006DBB4
		public ParticleRenderer(params ParticleSystem[] system)
		{
			this.Systems = new List<ParticleSystem>();
			this.Systems.AddRange(system);
		}

		// Token: 0x06001472 RID: 5234 RVA: 0x0006F9D4 File Offset: 0x0006DBD4
		public override void Update(Scene scene)
		{
			foreach (ParticleSystem particleSystem in this.Systems)
			{
				particleSystem.Update();
			}
			base.Update(scene);
		}

		// Token: 0x06001473 RID: 5235 RVA: 0x0006FA2C File Offset: 0x0006DC2C
		public override void Render(Scene scene)
		{
			Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Engine.ScreenMatrix);
			foreach (ParticleSystem particleSystem in this.Systems)
			{
				particleSystem.Render();
			}
			Draw.SpriteBatch.End();
		}

		// Token: 0x04001025 RID: 4133
		public List<ParticleSystem> Systems;
	}
}
