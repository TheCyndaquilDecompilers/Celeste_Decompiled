using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x0200023A RID: 570
	public class KeyDoorWipe : ScreenWipe
	{
		// Token: 0x0600122D RID: 4653 RVA: 0x0005C7E4 File Offset: 0x0005A9E4
		public KeyDoorWipe(Scene scene, bool wipeIn, Action onComplete = null) : base(scene, wipeIn, onComplete)
		{
			for (int i = 0; i < this.vertex.Length; i++)
			{
				this.vertex[i].Color = ScreenWipe.WipeColor;
			}
		}

		// Token: 0x0600122E RID: 4654 RVA: 0x0005C830 File Offset: 0x0005AA30
		public override void Render(Scene scene)
		{
			int num = 1090;
			int num2 = 540;
			float num3 = this.WipeIn ? (1f - this.Percent) : this.Percent;
			float num4 = Ease.SineInOut(Math.Min(1f, num3 / 0.5f));
			float num5 = Ease.SineInOut(1f - Calc.Clamp((num3 - 0.5f) / 0.3f, 0f, 1f));
			float num6 = num4;
			float num7 = 1f + (1f - num4) * 0.5f;
			float num8 = 960f * num4;
			float num9 = 128f * num5 * num6;
			float num10 = 128f * num5 * num7;
			float num11 = (float)num2 - (float)num2 * 0.3f * num5 * num7;
			float y = (float)num2 + (float)num2 * 0.5f * num5 * num7;
			float angleRadians = 0f;
			int i = 0;
			this.vertex[i++].Position = new Vector3(-10f, -10f, 0f);
			this.vertex[i++].Position = new Vector3(num8, -10f, 0f);
			this.vertex[i++].Position = new Vector3(num8, num11 - num10, 0f);
			for (int j = 1; j <= 8; j++)
			{
				float angleRadians2 = -1.5707964f - (float)(j - 1) / 8f * 1.5707964f;
				angleRadians = -1.5707964f - (float)j / 8f * 1.5707964f;
				this.vertex[i++].Position = new Vector3(-10f, -10f, 0f);
				this.vertex[i++].Position = new Vector3(new Vector2(num8, num11) + Calc.AngleToVector(angleRadians2, 1f) * new Vector2(num9, num10), 0f);
				this.vertex[i++].Position = new Vector3(new Vector2(num8, num11) + Calc.AngleToVector(angleRadians, 1f) * new Vector2(num9, num10), 0f);
			}
			this.vertex[i++].Position = new Vector3(-10f, -10f, 0f);
			this.vertex[i++].Position = new Vector3(num8 - num9, num11, 0f);
			this.vertex[i++].Position = new Vector3(-10f, (float)num, 0f);
			for (int k = 1; k <= 6; k++)
			{
				float angleRadians2 = 3.1415927f - (float)(k - 1) / 8f * 1.5707964f;
				angleRadians = 3.1415927f - (float)k / 8f * 1.5707964f;
				this.vertex[i++].Position = new Vector3(-10f, (float)num, 0f);
				this.vertex[i++].Position = new Vector3(new Vector2(num8, num11) + Calc.AngleToVector(angleRadians2, 1f) * new Vector2(num9, num10), 0f);
				this.vertex[i++].Position = new Vector3(new Vector2(num8, num11) + Calc.AngleToVector(angleRadians, 1f) * new Vector2(num9, num10), 0f);
			}
			this.vertex[i++].Position = new Vector3(-10f, (float)num, 0f);
			this.vertex[i++].Position = new Vector3(new Vector2(num8, num11) + Calc.AngleToVector(angleRadians, 1f) * new Vector2(num9, num10), 0f);
			this.vertex[i++].Position = new Vector3(num8 - num9 * 0.8f, y, 0f);
			this.vertex[i++].Position = new Vector3(-10f, (float)num, 0f);
			this.vertex[i++].Position = new Vector3(num8 - num9 * 0.8f, y, 0f);
			this.vertex[i++].Position = new Vector3(num8, y, 0f);
			this.vertex[i++].Position = new Vector3(-10f, (float)num, 0f);
			this.vertex[i++].Position = new Vector3(num8, y, 0f);
			this.vertex[i++].Position = new Vector3(num8, (float)num, 0f);
			ScreenWipe.DrawPrimitives(this.vertex);
			for (i = 0; i < this.vertex.Length; i++)
			{
				this.vertex[i].Position.X = 1920f - this.vertex[i].Position.X;
			}
			ScreenWipe.DrawPrimitives(this.vertex);
		}

		// Token: 0x04000DD8 RID: 3544
		private VertexPositionColor[] vertex = new VertexPositionColor[57];
	}
}
