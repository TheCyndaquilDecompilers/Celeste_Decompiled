using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x0200023E RID: 574
	public class DropWipe : ScreenWipe
	{
		// Token: 0x06001235 RID: 4661 RVA: 0x0005D774 File Offset: 0x0005B974
		public DropWipe(Scene scene, bool wipeIn, Action onComplete = null) : base(scene, wipeIn, onComplete)
		{
			this.color = ScreenWipe.WipeColor;
			this.meetings = new float[10];
			for (int i = 0; i < 10; i++)
			{
				this.meetings[i] = 0.05f + Calc.Random.NextFloat() * 0.9f;
			}
		}

		// Token: 0x06001236 RID: 4662 RVA: 0x0005D7D0 File Offset: 0x0005B9D0
		public override void Render(Scene scene)
		{
			float num = this.WipeIn ? (1f - this.Percent) : this.Percent;
			float num2 = 192f;
			Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Engine.ScreenMatrix);
			if (num >= 0.995f)
			{
				Draw.Rect(-10f, -10f, (float)(Engine.Width + 20), (float)(Engine.Height + 20), this.color);
			}
			else
			{
				for (int i = 0; i < 10; i++)
				{
					float num3 = (float)i / 10f;
					float num4 = (this.WipeIn ? (1f - num3) : num3) * 0.3f;
					if (num > num4)
					{
						float num5 = Ease.CubeIn(Math.Min(1f, (num - num4) / 0.7f));
						float num6 = 1080f * this.meetings[i] * num5;
						float num7 = 1080f * (1f - this.meetings[i]) * num5;
						Draw.Rect((float)i * num2 - 1f, -10f, num2 + 2f, num6 + 10f, this.color);
						Draw.Rect((float)i * num2 - 1f, 1080f - num7, num2 + 2f, num7 + 10f, this.color);
					}
				}
			}
			Draw.SpriteBatch.End();
		}

		// Token: 0x04000DDE RID: 3550
		private const int columns = 10;

		// Token: 0x04000DDF RID: 3551
		private float[] meetings;

		// Token: 0x04000DE0 RID: 3552
		private Color color;
	}
}
