using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x020002F2 RID: 754
	public class WindWipe : ScreenWipe
	{
		// Token: 0x0600175A RID: 5978 RVA: 0x0008DDD8 File Offset: 0x0008BFD8
		public WindWipe(Scene scene, bool wipeIn, Action onComplete = null) : base(scene, wipeIn, onComplete)
		{
			this.t = 40;
			this.columns = 1920 / this.t + 1;
			this.rows = 1080 / this.t + 1;
			this.vertexBuffer = new VertexPositionColor[this.columns * this.rows * 6];
			for (int i = 0; i < this.vertexBuffer.Length; i++)
			{
				this.vertexBuffer[i].Color = ScreenWipe.WipeColor;
			}
		}

		// Token: 0x0600175B RID: 5979 RVA: 0x0008DE64 File Offset: 0x0008C064
		public override void Render(Scene scene)
		{
			float num = (float)(this.columns * this.rows);
			int num2 = 0;
			for (int i = 0; i < this.columns; i++)
			{
				for (int j = 0; j < this.rows; j++)
				{
					int num3 = this.WipeIn ? (this.columns - i - 1) : i;
					float num4 = (float)((j + num3 % 2) % 2 * (this.rows + j / 2) + (j + num3 % 2 + 1) % 2 * (j / 2) + num3 * this.rows) / num * 0.5f;
					float num5 = num4 + 300f / num;
					float num6 = (Math.Max(num4, Math.Min(num5, this.WipeIn ? (1f - this.Percent) : this.Percent)) - num4) / (num5 - num4);
					float num7 = ((float)i - 0.5f) * (float)this.t;
					float num8 = ((float)j - 0.5f) * (float)this.t - (float)this.t * 0.5f * num6;
					float x = num7 + (float)this.t;
					float y = num8 + (float)this.t * num6;
					this.vertexBuffer[num2].Position = new Vector3(num7, num8, 0f);
					this.vertexBuffer[num2 + 1].Position = new Vector3(x, num8, 0f);
					this.vertexBuffer[num2 + 2].Position = new Vector3(num7, y, 0f);
					this.vertexBuffer[num2 + 3].Position = new Vector3(x, num8, 0f);
					this.vertexBuffer[num2 + 4].Position = new Vector3(x, y, 0f);
					this.vertexBuffer[num2 + 5].Position = new Vector3(num7, y, 0f);
					num2 += 6;
				}
			}
			ScreenWipe.DrawPrimitives(this.vertexBuffer);
		}

		// Token: 0x04001400 RID: 5120
		private int t;

		// Token: 0x04001401 RID: 5121
		private int columns;

		// Token: 0x04001402 RID: 5122
		private int rows;

		// Token: 0x04001403 RID: 5123
		private VertexPositionColor[] vertexBuffer;
	}
}
