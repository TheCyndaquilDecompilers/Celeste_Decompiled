using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste
{
	// Token: 0x0200021C RID: 540
	public class CustomSpriteEffect : Effect
	{
		// Token: 0x06001175 RID: 4469 RVA: 0x00056976 File Offset: 0x00054B76
		public CustomSpriteEffect(Effect effect) : base(effect)
		{
			this.matrixParam = base.Parameters["MatrixTransform"];
		}

		// Token: 0x06001176 RID: 4470 RVA: 0x00056998 File Offset: 0x00054B98
		protected override void OnApply()
		{
			Viewport viewport = base.GraphicsDevice.Viewport;
			Matrix matrix = Matrix.CreateOrthographicOffCenter(0f, (float)viewport.Width, (float)viewport.Height, 0f, 0f, 1f);
			Matrix matrix2 = Matrix.CreateTranslation(-0.5f, -0.5f, 0f);
			this.matrixParam.SetValue(matrix2 * matrix);
			base.OnApply();
		}

		// Token: 0x04000D17 RID: 3351
		private EffectParameter matrixParam;
	}
}
