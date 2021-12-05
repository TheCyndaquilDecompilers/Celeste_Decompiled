using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x020002F3 RID: 755
	public class CurtainWipe : ScreenWipe
	{
		// Token: 0x0600175C RID: 5980 RVA: 0x0008E064 File Offset: 0x0008C264
		public CurtainWipe(Scene scene, bool wipeIn, Action onComplete = null) : base(scene, wipeIn, onComplete)
		{
			for (int i = 0; i < this.vertexBufferLeft.Length; i++)
			{
				this.vertexBufferLeft[i].Color = ScreenWipe.WipeColor;
			}
		}

		// Token: 0x0600175D RID: 5981 RVA: 0x0008E0C4 File Offset: 0x0008C2C4
		public override void Render(Scene scene)
		{
			float num = (this.WipeIn ? Ease.CubeInOut : Ease.CubeInOut)(this.WipeIn ? (1f - this.Percent) : this.Percent);
			float num2 = Math.Min(1f, num / 0.3f);
			float num3 = Math.Max(0f, Math.Min(1f, (num - 0.1f) / 0.9f / 0.9f));
			Vector2 vector = new Vector2(0f, 540f * num2);
			Vector2 vector2 = new Vector2(1920f, 1592f) / 2f;
			Vector2 control = (vector + vector2) / 2f + Vector2.UnitY * 1080f * 0.25f;
			Vector2 vector3 = new Vector2(896f + 200f * num, -350f + 256f * num2);
			Vector2 point = new SimpleCurve(vector, vector2, control).GetPoint(num3);
			Vector2 vector4 = new Vector2(point.X + 64f * num, 1080f);
			int i = 0;
			this.vertexBufferLeft[i++].Position = new Vector3(-10f, -10f, 0f);
			this.vertexBufferLeft[i++].Position = new Vector3(vector3.X, -10f, 0f);
			this.vertexBufferLeft[i++].Position = new Vector3(vector3.X, vector3.Y, 0f);
			this.vertexBufferLeft[i++].Position = new Vector3(-10f, -10f, 0f);
			this.vertexBufferLeft[i++].Position = new Vector3(-10f, point.Y, 0f);
			this.vertexBufferLeft[i++].Position = new Vector3(point.X, point.Y, 0f);
			this.vertexBufferLeft[i++].Position = new Vector3(point.X, point.Y, 0f);
			this.vertexBufferLeft[i++].Position = new Vector3(-10f, point.Y, 0f);
			this.vertexBufferLeft[i++].Position = new Vector3(-10f, 1090f, 0f);
			this.vertexBufferLeft[i++].Position = new Vector3(point.X, point.Y, 0f);
			this.vertexBufferLeft[i++].Position = new Vector3(-10f, 1090f, 0f);
			this.vertexBufferLeft[i++].Position = new Vector3(vector4.X, vector4.Y + 10f, 0f);
			int num4 = i;
			Vector2 value = vector3;
			while (i < this.vertexBufferLeft.Length)
			{
				Vector2 point2 = new SimpleCurve(vector3, point, (vector3 + point) / 2f + new Vector2(0f, 384f * num3)).GetPoint((float)(i - num4) / (float)(this.vertexBufferLeft.Length - num4 - 3));
				this.vertexBufferLeft[i].Position = new Vector3(-10f, -10f, 0f);
				this.vertexBufferLeft[i + 1].Position = new Vector3(value, 0f);
				this.vertexBufferLeft[i + 2].Position = new Vector3(point2, 0f);
				value = point2;
				i += 3;
			}
			for (i = 0; i < this.vertexBufferLeft.Length; i++)
			{
				this.vertexBufferRight[i] = this.vertexBufferLeft[i];
				this.vertexBufferRight[i].Position.X = 1920f - this.vertexBufferRight[i].Position.X;
			}
			ScreenWipe.DrawPrimitives(this.vertexBufferLeft);
			ScreenWipe.DrawPrimitives(this.vertexBufferRight);
		}

		// Token: 0x04001404 RID: 5124
		private VertexPositionColor[] vertexBufferLeft = new VertexPositionColor[192];

		// Token: 0x04001405 RID: 5125
		private VertexPositionColor[] vertexBufferRight = new VertexPositionColor[192];
	}
}
