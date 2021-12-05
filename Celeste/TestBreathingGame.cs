using System;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x02000258 RID: 600
	public class TestBreathingGame : Scene
	{
		// Token: 0x060012B7 RID: 4791 RVA: 0x00065593 File Offset: 0x00063793
		public TestBreathingGame()
		{
			this.game = new BreathingMinigame(true, null);
			base.Add(this.game);
		}

		// Token: 0x060012B8 RID: 4792 RVA: 0x000655B4 File Offset: 0x000637B4
		public override void BeforeRender()
		{
			this.game.BeforeRender();
			base.BeforeRender();
		}

		// Token: 0x060012B9 RID: 4793 RVA: 0x000655C7 File Offset: 0x000637C7
		public override void Render()
		{
			Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, null, Engine.ScreenMatrix);
			this.game.Render();
			Draw.SpriteBatch.End();
		}

		// Token: 0x04000EB3 RID: 3763
		private BreathingMinigame game;
	}
}
