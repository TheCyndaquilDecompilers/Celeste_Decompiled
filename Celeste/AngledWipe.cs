using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x0200023D RID: 573
	public class AngledWipe : ScreenWipe
	{
		// Token: 0x06001233 RID: 4659 RVA: 0x0005D508 File Offset: 0x0005B708
		public AngledWipe(Scene scene, bool wipeIn, Action onComplete = null) : base(scene, wipeIn, onComplete)
		{
			for (int i = 0; i < this.vertexBuffer.Length; i++)
			{
				this.vertexBuffer[i].Color = ScreenWipe.WipeColor;
			}
		}

		// Token: 0x06001234 RID: 4660 RVA: 0x0005D554 File Offset: 0x0005B754
		public override void Render(Scene scene)
		{
			float num = 183.33333f;
			float num2 = -64f;
			float num3 = 1984f;
			for (int i = 0; i < 6; i++)
			{
				int num4 = i * 6;
				float num5 = num2;
				float num6 = -10f + (float)i * num;
				float num7 = 0f;
				float num8 = (float)i / 6f;
				float num9 = (this.WipeIn ? (1f - num8) : num8) * 0.3f;
				if (this.Percent > num9)
				{
					num7 = Math.Min(1f, (this.Percent - num9) / 0.7f);
				}
				if (this.WipeIn)
				{
					num7 = 1f - num7;
				}
				float num10 = num3 * num7;
				this.vertexBuffer[num4].Position = new Vector3(num5, num6, 0f);
				this.vertexBuffer[num4 + 1].Position = new Vector3(num5 + num10, num6, 0f);
				this.vertexBuffer[num4 + 2].Position = new Vector3(num5, num6 + num, 0f);
				this.vertexBuffer[num4 + 3].Position = new Vector3(num5 + num10, num6, 0f);
				this.vertexBuffer[num4 + 4].Position = new Vector3(num5 + num10 + 64f, num6 + num, 0f);
				this.vertexBuffer[num4 + 5].Position = new Vector3(num5, num6 + num, 0f);
			}
			if (this.WipeIn)
			{
				for (int j = 0; j < this.vertexBuffer.Length; j++)
				{
					this.vertexBuffer[j].Position.X = 1920f - this.vertexBuffer[j].Position.X;
					this.vertexBuffer[j].Position.Y = 1080f - this.vertexBuffer[j].Position.Y;
				}
			}
			ScreenWipe.DrawPrimitives(this.vertexBuffer);
		}

		// Token: 0x04000DDB RID: 3547
		private const int rows = 6;

		// Token: 0x04000DDC RID: 3548
		private const float angleSize = 64f;

		// Token: 0x04000DDD RID: 3549
		private VertexPositionColor[] vertexBuffer = new VertexPositionColor[36];
	}
}
