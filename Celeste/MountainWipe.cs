using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x0200023C RID: 572
	public class MountainWipe : ScreenWipe
	{
		// Token: 0x06001231 RID: 4657 RVA: 0x0005D1B4 File Offset: 0x0005B3B4
		public MountainWipe(Scene scene, bool wipeIn, Action onComplete = null) : base(scene, wipeIn, onComplete)
		{
			for (int i = 0; i < this.vertexBuffer.Length; i++)
			{
				this.vertexBuffer[i].Color = ScreenWipe.WipeColor;
			}
		}

		// Token: 0x06001232 RID: 4658 RVA: 0x0005D200 File Offset: 0x0005B400
		public override void Render(Scene scene)
		{
			float percent = this.Percent;
			int num = 1080;
			Vector2 vector = new Vector2(960f, (float)num - (float)(num * 2) * percent);
			Vector2 vector2 = new Vector2(-10f, (float)(num * 2) * (1f - percent));
			Vector2 vector3 = new Vector2((float)base.Right, (float)(num * 2) * (1f - percent));
			if (!this.WipeIn)
			{
				this.vertexBuffer[0].Position = new Vector3(vector, 0f);
				this.vertexBuffer[1].Position = new Vector3(vector2, 0f);
				this.vertexBuffer[2].Position = new Vector3(vector3, 0f);
				this.vertexBuffer[3].Position = new Vector3(vector2, 0f);
				this.vertexBuffer[4].Position = new Vector3(vector3, 0f);
				this.vertexBuffer[5].Position = new Vector3(vector2.X, vector2.Y + (float)num + 10f, 0f);
				this.vertexBuffer[6].Position = new Vector3(vector3, 0f);
				this.vertexBuffer[8].Position = new Vector3(vector3.X, vector3.Y + (float)num + 10f, 0f);
				this.vertexBuffer[7].Position = new Vector3(vector2.X, vector2.Y + (float)num + 10f, 0f);
			}
			else
			{
				this.vertexBuffer[0].Position = new Vector3(vector2.X, vector.Y - (float)num - 10f, 0f);
				this.vertexBuffer[1].Position = new Vector3(vector3.X, vector.Y - (float)num - 10f, 0f);
				this.vertexBuffer[2].Position = new Vector3(vector, 0f);
				this.vertexBuffer[3].Position = new Vector3(vector2.X, vector.Y - (float)num - 10f, 0f);
				this.vertexBuffer[4].Position = new Vector3(vector, 0f);
				this.vertexBuffer[5].Position = new Vector3(vector2, 0f);
				this.vertexBuffer[6].Position = new Vector3(vector3.X, vector.Y - (float)num - 10f, 0f);
				this.vertexBuffer[7].Position = new Vector3(vector3, 0f);
				this.vertexBuffer[8].Position = new Vector3(vector, 0f);
			}
			ScreenWipe.DrawPrimitives(this.vertexBuffer);
		}

		// Token: 0x04000DDA RID: 3546
		private VertexPositionColor[] vertexBuffer = new VertexPositionColor[9];
	}
}
