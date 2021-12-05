using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x0200023B RID: 571
	public class FallWipe : ScreenWipe
	{
		// Token: 0x0600122F RID: 4655 RVA: 0x0005CE08 File Offset: 0x0005B008
		public FallWipe(Scene scene, bool wipeIn, Action onComplete = null) : base(scene, wipeIn, onComplete)
		{
			for (int i = 0; i < this.vertexBuffer.Length; i++)
			{
				this.vertexBuffer[i].Color = ScreenWipe.WipeColor;
			}
		}

		// Token: 0x06001230 RID: 4656 RVA: 0x0005CE54 File Offset: 0x0005B054
		public override void Render(Scene scene)
		{
			float percent = this.Percent;
			Vector2 vector = new Vector2(960f, 1080f - 2160f * percent);
			Vector2 vector2 = new Vector2(-10f, 2160f * (1f - percent));
			Vector2 vector3 = new Vector2((float)base.Right, 2160f * (1f - percent));
			if (!this.WipeIn)
			{
				this.vertexBuffer[0].Position = new Vector3(vector, 0f);
				this.vertexBuffer[1].Position = new Vector3(vector2, 0f);
				this.vertexBuffer[2].Position = new Vector3(vector3, 0f);
				this.vertexBuffer[3].Position = new Vector3(vector2, 0f);
				this.vertexBuffer[4].Position = new Vector3(vector3, 0f);
				this.vertexBuffer[5].Position = new Vector3(vector2.X, vector2.Y + 1080f + 10f, 0f);
				this.vertexBuffer[6].Position = new Vector3(vector3, 0f);
				this.vertexBuffer[8].Position = new Vector3(vector3.X, vector3.Y + 1080f + 10f, 0f);
				this.vertexBuffer[7].Position = new Vector3(vector2.X, vector2.Y + 1080f + 10f, 0f);
			}
			else
			{
				this.vertexBuffer[0].Position = new Vector3(vector2.X, vector.Y - 1080f - 10f, 0f);
				this.vertexBuffer[1].Position = new Vector3(vector3.X, vector.Y - 1080f - 10f, 0f);
				this.vertexBuffer[2].Position = new Vector3(vector, 0f);
				this.vertexBuffer[3].Position = new Vector3(vector2.X, vector.Y - 1080f - 10f, 0f);
				this.vertexBuffer[4].Position = new Vector3(vector, 0f);
				this.vertexBuffer[5].Position = new Vector3(vector2, 0f);
				this.vertexBuffer[6].Position = new Vector3(vector3.X, vector.Y - 1080f - 10f, 0f);
				this.vertexBuffer[7].Position = new Vector3(vector3, 0f);
				this.vertexBuffer[8].Position = new Vector3(vector, 0f);
			}
			for (int i = 0; i < this.vertexBuffer.Length; i++)
			{
				this.vertexBuffer[i].Position.Y = 1080f - this.vertexBuffer[i].Position.Y;
			}
			ScreenWipe.DrawPrimitives(this.vertexBuffer);
		}

		// Token: 0x04000DD9 RID: 3545
		private VertexPositionColor[] vertexBuffer = new VertexPositionColor[9];
	}
}
