using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

namespace Celeste
{
	// Token: 0x02000239 RID: 569
	public class HeartWipe : ScreenWipe
	{
		// Token: 0x0600122B RID: 4651 RVA: 0x0005C210 File Offset: 0x0005A410
		public HeartWipe(Scene scene, bool wipeIn, Action onComplete = null) : base(scene, wipeIn, onComplete)
		{
			for (int i = 0; i < this.vertex.Length; i++)
			{
				this.vertex[i].Color = ScreenWipe.WipeColor;
			}
		}

		// Token: 0x0600122C RID: 4652 RVA: 0x0005C25C File Offset: 0x0005A45C
		public override void Render(Scene scene)
		{
			float num = ((this.WipeIn ? this.Percent : (1f - this.Percent)) - 0.2f) / 0.8f;
			if (num <= 0f)
			{
				Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, null, Engine.ScreenMatrix);
				Draw.Rect(-1f, -1f, (float)(Engine.Width + 2), (float)(Engine.Height + 2), ScreenWipe.WipeColor);
				Draw.SpriteBatch.End();
				return;
			}
			Vector2 vector = new Vector2((float)Engine.Width, (float)Engine.Height) / 2f;
			float num2 = (float)Engine.Width * 0.75f * num;
			float num3 = (float)Engine.Width * num;
			float num4 = -0.25f;
			float num5 = -1.5707964f;
			Vector2 vector2 = vector + new Vector2(-(float)Math.Cos((double)num4) * num2, -num2 / 2f);
			int i = 0;
			for (int j = 1; j <= 16; j++)
			{
				float angleRadians = num4 + (num5 - num4) * ((float)(j - 1) / 16f);
				float angleRadians2 = num4 + (num5 - num4) * ((float)j / 16f);
				this.vertex[i++].Position = new Vector3(vector.X, -num3, 0f);
				this.vertex[i++].Position = new Vector3(vector2 + Calc.AngleToVector(angleRadians, num2), 0f);
				this.vertex[i++].Position = new Vector3(vector2 + Calc.AngleToVector(angleRadians2, num2), 0f);
			}
			this.vertex[i++].Position = new Vector3(vector.X, -num3, 0f);
			this.vertex[i++].Position = new Vector3(vector2 + new Vector2(0f, -num2), 0f);
			this.vertex[i++].Position = new Vector3(-num3, -num3, 0f);
			this.vertex[i++].Position = new Vector3(-num3, -num3, 0f);
			this.vertex[i++].Position = new Vector3(vector2 + new Vector2(0f, -num2), 0f);
			this.vertex[i++].Position = new Vector3(-num3, vector2.Y, 0f);
			float num6 = 2.3561945f;
			for (int k = 1; k <= 16; k++)
			{
				float angleRadians3 = -1.5707964f - (float)(k - 1) / 16f * num6;
				float angleRadians4 = -1.5707964f - (float)k / 16f * num6;
				this.vertex[i++].Position = new Vector3(-num3, vector2.Y, 0f);
				this.vertex[i++].Position = new Vector3(vector2 + Calc.AngleToVector(angleRadians3, num2), 0f);
				this.vertex[i++].Position = new Vector3(vector2 + Calc.AngleToVector(angleRadians4, num2), 0f);
			}
			Vector2 value = vector2 + Calc.AngleToVector(-1.5707964f - num6, num2);
			Vector2 value2 = vector + new Vector2(0f, num2 * 1.8f);
			this.vertex[i++].Position = new Vector3(-num3, vector2.Y, 0f);
			this.vertex[i++].Position = new Vector3(value, 0f);
			this.vertex[i++].Position = new Vector3(-num3, (float)Engine.Height + num3, 0f);
			this.vertex[i++].Position = new Vector3(-num3, (float)Engine.Height + num3, 0f);
			this.vertex[i++].Position = new Vector3(value, 0f);
			this.vertex[i++].Position = new Vector3(value2, 0f);
			this.vertex[i++].Position = new Vector3(-num3, (float)Engine.Height + num3, 0f);
			this.vertex[i++].Position = new Vector3(value2, 0f);
			this.vertex[i++].Position = new Vector3(vector.X, (float)Engine.Height + num3, 0f);
			ScreenWipe.DrawPrimitives(this.vertex);
			for (i = 0; i < this.vertex.Length; i++)
			{
				this.vertex[i].Position.X = 1920f - this.vertex[i].Position.X;
			}
			ScreenWipe.DrawPrimitives(this.vertex);
		}

		// Token: 0x04000DD7 RID: 3543
		private VertexPositionColor[] vertex = new VertexPositionColor[111];
	}
}
